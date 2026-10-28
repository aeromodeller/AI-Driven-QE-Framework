using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons;
using AI.Driven.QE.Framework.Utils;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace AI.Driven.QE.Framework.Tests.Hooks;

/// <summary>
/// Manages an isolated Playwright session and test evidence for each BDD scenario.
/// </summary>
[Binding]
public sealed class PlaywrightHooks
{
    private const string PlaywrightKey = "Playwright";
    private const string BrowserKey = "Browser";
    private const string ContextKey = "BrowserContext";
    private const string PageKey = "Page";
    private const string TracingKey = "TracingStarted";
    private readonly ScenarioContext _scenarioContext;

    public PlaywrightHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public async Task StartBrowser()
    {
        var playwright = await Playwright.CreateAsync();
        _scenarioContext[PlaywrightKey] = playwright;

        var browserType = TestSettings.BrowserName switch
        {
            "chromium" => playwright.Chromium,
            "webkit" => playwright.Webkit,
            _ => throw new InvalidOperationException(
                $"Unsupported PLAYWRIGHT_BROWSER '{TestSettings.BrowserName}'. Use chromium or webkit.")
        };

        var browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Timeout = 30_000
        });
        _scenarioContext[BrowserKey] = browser;

        var context = await browser.NewContextAsync();
        _scenarioContext[ContextKey] = context;
        await context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
        _scenarioContext[TracingKey] = true;
        _scenarioContext[PageKey] = await context.NewPageAsync();
    }

    [AfterScenario]
    public async Task StopBrowser()
    {
        var failed = _scenarioContext.TestError is not null;
        var artifactDirectory = TestSettings.ArtifactDirectory;
        var scenarioName = TestSettings.SanitizeFileName(_scenarioContext.ScenarioInfo.Title);
        var browserName = TestSettings.SanitizeFileName(TestSettings.BrowserName);
        var screenshotPath = Path.Combine(artifactDirectory, $"{scenarioName}-{browserName}.png");
        var tracePath = Path.Combine(artifactDirectory, $"{scenarioName}-{browserName}-trace.zip");
        var page = GetResource<IPage>(PageKey);
        var context = GetResource<IBrowserContext>(ContextKey);
        var browser = GetResource<IBrowser>(BrowserKey);
        var playwright = GetResource<IPlaywright>(PlaywrightKey);

        try
        {
            if (page is not null && context is not null)
            {
                Directory.CreateDirectory(artifactDirectory);
                var screenshot = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
                await File.WriteAllBytesAsync(screenshotPath, screenshot);
                TestContext.AddTestAttachment(
                    screenshotPath,
                    failed ? "Failure screenshot" : "Scenario completion screenshot");
                AllureApi.AddAttachment(
                    failed ? "Failure screenshot" : "Scenario completion screenshot",
                    "image/png",
                    screenshot,
                    "png");
            }
        }
        finally
        {
            try
            {
                if (context is not null && IsTracingStarted())
                {
                    await context.Tracing.StopAsync(new TracingStopOptions
                    {
                        Path = failed ? tracePath : null
                    });
                    _scenarioContext[TracingKey] = false;
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
                        if (context is not null)
                        {
                            await context.CloseAsync();
                        }
                    }
                    finally
                    {
                        try
                        {
                            if (browser is not null)
                            {
                                await browser.CloseAsync();
                            }
                        }
                        finally
                        {
                            playwright?.Dispose();
                        }
                    }
                }
            }
        }
    }

    private T? GetResource<T>(string key) where T : class =>
        _scenarioContext.TryGetValue(key, out var value) ? value as T : null;

    private bool IsTracingStarted() =>
        _scenarioContext.TryGetValue(TracingKey, out var value) && value is true;
}
