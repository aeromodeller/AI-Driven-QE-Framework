using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Allure.NUnit;
using AI.Driven.QE.Framework.Pages;
using AI.Driven.QE.Framework.Utils;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AI.Driven.QE.Framework.Tests;

/// <summary>
/// Contains end-to-end coverage for the login experience.
/// </summary>
[TestFixture]
[AllureNUnit]
public sealed class LoginTests
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private bool _tracingStarted;

    /// <summary>
    /// Gets the browser page used by the current test.
    /// </summary>
    public IPage Page { get; private set; } = null!;

    /// <summary>
    /// Starts a browser and creates an isolated page for each test.
    /// </summary>
    [SetUp]
    public async Task SetUp()
    {
        _playwright = await Playwright.CreateAsync();
        var browserType = TestSettings.BrowserName switch
        {
            "chromium" => _playwright.Chromium,
            "webkit" => _playwright.Webkit,
            _ => throw new InvalidOperationException(
                $"Unsupported PLAYWRIGHT_BROWSER '{TestSettings.BrowserName}'. Use chromium or webkit.")
        };

        _browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Timeout = 30_000
        });
        _context = await _browser.NewContextAsync();
        await _context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
        _tracingStarted = true;
        Page = await _context.NewPageAsync();
    }

    /// <summary>
    /// Closes browser resources created for the current test.
    /// </summary>
    [TearDown]
    public async Task TearDown()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var artifactDirectory = TestSettings.ArtifactDirectory;
        var testName = TestSettings.SanitizeFileName(TestContext.CurrentContext.Test.Name);
        var browserName = TestSettings.SanitizeFileName(TestSettings.BrowserName);
        var screenshotPath = Path.Combine(artifactDirectory, $"{testName}-{browserName}.png");
        var tracePath = Path.Combine(artifactDirectory, $"{testName}-{browserName}-trace.zip");

        try
        {
            if (_context is not null && Page is not null)
            {
                Directory.CreateDirectory(artifactDirectory);
                var screenshot = await Page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
                await File.WriteAllBytesAsync(screenshotPath, screenshot);
                TestContext.AddTestAttachment(
                    screenshotPath,
                    failed ? "Failure screenshot" : "Test completion screenshot");
                AllureApi.AddAttachment(
                    failed ? "Failure screenshot" : "Test completion screenshot",
                    "image/png",
                    screenshot,
                    "png");
            }
        }
        finally
        {
            try
            {
                if (_tracingStarted && _context is not null)
                {
                    await _context.Tracing.StopAsync(new TracingStopOptions
                    {
                        Path = failed ? tracePath : null
                    });
                    _tracingStarted = false;
                }
            }
            finally
            {
                try
                {
                    if (failed && File.Exists(tracePath))
                    {
                        TestContext.AddTestAttachment(tracePath, "Playwright trace");
                        AllureApi.AddAttachment("Playwright trace", "application/zip", tracePath);
                    }
                }
                finally
                {
                    try
                    {
                        if (_context is not null)
                        {
                            await _context.CloseAsync();
                        }
                    }
                    finally
                    {
                        try
                        {
                            if (_browser is not null)
                            {
                                await _browser.CloseAsync();
                            }
                        }
                        finally
                        {
                            _playwright?.Dispose();
                        }

                    }
                }
            }
        }
    }

    /// <summary>
    /// Verifies that valid credentials complete the login flow.
    /// </summary>
    [Test]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("A user with valid credentials can sign in successfully.")]
    public async Task Login_WithValidCredentials_ShowsSuccess()
    {
        // Arrange
        var loginPage = new LoginPage(Page);
        var username = TestSettings.GetCredential("QE_USERNAME", "standard_user");
        var password = TestSettings.GetCredential("QE_PASSWORD", "secret_sauce");

        // Act
        await loginPage.Login(username, password);

        // Assert
        await Page.WaitForURLAsync("**/inventory.html");
        await Page.GetByText("Products", new PageGetByTextOptions { Exact = true })
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.That(Page.Url, Does.Contain("/inventory.html"));
    }

    /// <summary>
    /// Verifies that SauceDemo blocks a locked-out user from signing in.
    /// </summary>
    [Test]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("A locked-out user receives an explanatory login error.")]
    public async Task Login_WithLockedOutUser_ShowsError()
    {
        // Arrange
        var loginPage = new LoginPage(Page);
        var username = TestSettings.GetCredential("QE_LOCKED_OUT_USERNAME", "locked_out_user");
        var password = TestSettings.GetCredential("QE_PASSWORD", "secret_sauce");

        // Act
        await loginPage.Login(username, password);

        // Assert
        var errorMessage = Page.Locator("[data-test='error']");
        await errorMessage.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.That(
            await errorMessage.InnerTextAsync(),
            Does.Contain("Epic sadface: Sorry, this user has been locked out"));
    }

    /// <summary>
    /// Verifies an authenticated user can add an item to the cart and complete checkout.
    /// </summary>
    [Test]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("A user can purchase a product through the end-to-end checkout flow.")]
    public async Task Checkout_WithAvailableProduct_CompletesOrder()
    {
        // Arrange
        var loginPage = new LoginPage(Page);
        var productsPage = new ProductsPage(Page);
        var cartPage = new CartPage(Page);
        var checkoutPage = new CheckoutPage(Page);
        var username = TestSettings.GetCredential("QE_USERNAME", "standard_user");
        var password = TestSettings.GetCredential("QE_PASSWORD", "secret_sauce");
        const string productName = "Sauce Labs Backpack";

        // Act
        await loginPage.Login(username, password);
        await Page.WaitForURLAsync("**/inventory.html");
        await productsPage.AddBackpackToCart();
        await productsPage.OpenCart();
        await cartPage.WaitForItem(productName);
        await cartPage.StartCheckout();
        await checkoutPage.EnterInformation("QE", "Demo", "10001");
        await checkoutPage.CompleteOrder();

        // Assert
        var confirmation = Page.GetByText("Thank you for your order!", new PageGetByTextOptions { Exact = true });
        await confirmation.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.That(Page.Url, Does.Contain("/checkout-complete.html"));
    }
}
