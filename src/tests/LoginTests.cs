using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Allure.NUnit;
using AI.Driven.QE.Framework.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AI.Driven.QE.Framework.Tests;

/// <summary>
/// Contains end-to-end coverage for the login experience.
/// </summary>
[TestFixture]
[AllureNUnit]
public sealed class LoginTests
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

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
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        Page = await _browser.NewPageAsync();
    }

    /// <summary>
    /// Closes browser resources created for the current test.
    /// </summary>
    [TearDown]
    public async Task TearDown()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
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

        // Act
        await loginPage.Login("standard_user", "secret_sauce");

        // Assert
        Assert.That(Page.Url, Does.Contain("/inventory.html"));
    }

    /// <summary>
    /// Verifies that an incorrect password does not complete the login flow.
    /// </summary>
    [Test]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("A user with an incorrect password receives a login error.")]
    public async Task Login_WithLockedOutUser_ShowsError()
    {
        // Arrange
        var loginPage = new LoginPage(Page);

        // Act
        await loginPage.Login("locked_out_user", "secret_sauce");

        // Assert
        Assert.That(
            await Page.Locator("[data-test='error']").InnerTextAsync(),
            Does.Contain("Epic sadface: Sorry, this user has been locked out"));
    }
}
