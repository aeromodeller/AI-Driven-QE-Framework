using System;
using System.IO;

namespace AI.Driven.QE.Framework.Utils;

/// <summary>
/// Provides shared configuration and artifact-path helpers for test runs.
/// </summary>
public static class TestSettings
{
    /// <summary>
    /// Gets the login page URL, defaulting to the public SauceDemo sample site.
    /// </summary>
    public static string LoginUrl
    {
        get
        {
            var value = Environment.GetEnvironmentVariable("QE_LOGIN_URL");
            var loginUrl = string.IsNullOrWhiteSpace(value)
                ? "https://www.saucedemo.com/"
                : value;

            if (!Uri.TryCreate(loginUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException(
                    "QE_LOGIN_URL must be an absolute HTTP or HTTPS URL.");
            }

            return uri.ToString();
        }
    }

    /// <summary>
    /// Gets a non-empty credential value from the environment, with an optional POC default.
    /// </summary>
    /// <param name="name">The environment variable name.</param>
    /// <param name="defaultValue">The public demo credential used when unset.</param>
    /// <returns>The configured credential or the supplied demo default.</returns>
    public static string GetCredential(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    /// <summary>
    /// Gets the selected Playwright browser, defaulting to Chromium.
    /// </summary>
    public static string BrowserName =>
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BROWSER")?.ToLowerInvariant() ?? "chromium";

    /// <summary>
    /// Gets the directory used for test screenshots and traces.
    /// </summary>
    public static string ArtifactDirectory
    {
        get
        {
            var configuredDirectory = Environment.GetEnvironmentVariable("QE_ARTIFACTS_DIR");
            return string.IsNullOrWhiteSpace(configuredDirectory)
                ? Path.Combine(Environment.CurrentDirectory, "test-artifacts")
                : configuredDirectory;
        }
    }

    /// <summary>
    /// Replaces platform-invalid filename characters with underscores.
    /// </summary>
    /// <param name="name">The filename value to sanitize.</param>
    /// <returns>A filename value that can be used as a path component.</returns>
    public static string SanitizeFileName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidCharacter, '_');
        }

        return name;
    }
}
