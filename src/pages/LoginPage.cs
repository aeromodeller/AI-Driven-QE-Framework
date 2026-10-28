using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AI.Driven.QE.Framework.Pages;

/// <summary>
/// Encapsulates interactions with the application's login page.
/// </summary>
public sealed class LoginPage
{
    private const string LoginUrl = "https://www.saucedemo.com/";

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
        await _page.GotoAsync(LoginUrl);
        await _usernameInput.FillAsync(user);
        await _passwordInput.FillAsync(pass);
        await _loginButton.ClickAsync();
    }
}
