using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AI.Driven.QE.Framework.Pages;

/// <summary>
/// Encapsulates actions on the SauceDemo products inventory page.
/// </summary>
public sealed class ProductsPage
{
    private readonly IPage _page;
    private readonly ILocator _backpackAddButton;
    private readonly ILocator _backpackRemoveButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsPage"/> class.
    /// </summary>
    /// <param name="page">The Playwright page showing the products inventory.</param>
    public ProductsPage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        _page = page;
        _backpackAddButton = page.Locator("[data-test='add-to-cart-sauce-labs-backpack']");
        _backpackRemoveButton = page.Locator("[data-test='remove-sauce-labs-backpack']");
    }

    /// <summary>
    /// Adds the Sauce Labs Backpack to the cart.
    /// </summary>
    public async Task AddBackpackToCart()
    {
        if (!await _backpackRemoveButton.IsVisibleAsync())
        {
            await _backpackAddButton.ClickAsync();
        }
    }

    /// <summary>
    /// Opens the shopping cart.
    /// </summary>
    public async Task OpenCart()
    {
        await _page.Locator("[data-test='shopping-cart-link']").ClickAsync();
    }
}
