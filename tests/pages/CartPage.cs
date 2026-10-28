using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AI.Driven.QE.Framework.Pages;

/// <summary>
/// Encapsulates assertions and actions on the SauceDemo shopping cart.
/// </summary>
public sealed class CartPage
{
    private readonly IPage _page;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartPage"/> class.
    /// </summary>
    /// <param name="page">The Playwright page showing the shopping cart.</param>
    public CartPage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        _page = page;
    }

    /// <summary>
    /// Waits for the specified item to appear in the cart.
    /// </summary>
    /// <param name="itemName">The product's visible name.</param>
    public async Task WaitForItem(string itemName)
    {
        await _page.GetByText(itemName, new PageGetByTextOptions { Exact = true })
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    /// <summary>
    /// Starts checkout from the cart.
    /// </summary>
    public async Task StartCheckout()
    {
        await _page.Locator("[data-test='checkout']").ClickAsync();
    }
}
