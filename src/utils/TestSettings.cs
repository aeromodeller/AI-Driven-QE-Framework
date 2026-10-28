using System;
using System.IO;

namespace AI.Driven.QE.Framework.Utils;

/// <summary>
/// Provides shared configuration and artifact-path helpers for test runs.
/// </summary>
public static class TestSettings
{
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
