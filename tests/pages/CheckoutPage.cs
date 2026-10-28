using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AI.Driven.QE.Framework.Pages;

/// <summary>
/// Encapsulates SauceDemo checkout information, order review, and completion actions.
/// </summary>
public sealed class CheckoutPage
{
    private readonly IPage _page;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutPage"/> class.
    /// </summary>
    /// <param name="page">The Playwright page showing checkout.</param>
    public CheckoutPage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        _page = page;
    }

    /// <summary>
    /// Enters checkout information and proceeds to order review.
    /// </summary>
    /// <param name="firstName">The test customer's first name.</param>
    /// <param name="lastName">The test customer's last name.</param>
    /// <param name="postalCode">The test customer's postal code.</param>
    public async Task EnterInformation(string firstName, string lastName, string postalCode)
    {
        await _page.Locator("[data-test='firstName']").FillAsync(firstName);
        await _page.Locator("[data-test='lastName']").FillAsync(lastName);
        await _page.Locator("[data-test='postalCode']").FillAsync(postalCode);
        await _page.Locator("[data-test='continue']").ClickAsync();
    }

    /// <summary>
    /// Completes the order from the order review page.
    /// </summary>
    public async Task CompleteOrder()
    {
        await _page.Locator("[data-test='finish']").ClickAsync();
    }
}
