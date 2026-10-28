using System;
using System.Threading.Tasks;
using AI.Driven.QE.Framework.Utils;
using Microsoft.Playwright;

namespace AI.Driven.QE.Framework.Pages;

/// <summary>
/// Encapsulates interactions with the application's login page.
/// </summary>
public sealed class LoginPage
{
    private readonly IPage _page;
    private readonly ILocator _usernameInput;
    private readonly ILocator _passwordInput;
    private readonly ILocator _loginButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// </summary>
    /// <param name="page">The Playwright page used to interact with the browser.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="page"/> is <see langword="null"/>.
    /// </exception>
    public LoginPage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        _page = page;
        _usernameInput = page.Locator("#user-name");
        _passwordInput = page.Locator("#password");
        _loginButton = page.Locator("#login-button");
    }

    /// <summary>
    /// Enters the supplied credentials and submits the login form.
    /// </summary>
    /// <param name="user">The username to enter.</param>
    /// <param name="pass">The password to enter.</param>
    /// <returns>A task that completes when the login form has been submitted.</returns>
    public async Task Login(string user, string pass)
    {
        await NavigateAsync();
        await SubmitCredentialsAsync(user, pass);
    }

    /// <summary>
    /// Navigates to the configured login page.
    /// </summary>
    public async Task NavigateAsync()
    {
        await _page.GotoAsync(TestSettings.LoginUrl);
    }

    /// <summary>
    /// Submits credentials on the currently displayed login page.
    /// </summary>
    /// <param name="user">The username to enter.</param>
    /// <param name="pass">The password to enter.</param>
    public async Task SubmitCredentialsAsync(string user, string pass)
    {
        await _usernameInput.FillAsync(user);
        await _passwordInput.FillAsync(pass);
        await _loginButton.ClickAsync();
    }
}
