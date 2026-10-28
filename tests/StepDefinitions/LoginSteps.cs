using System;
using System.Threading.Tasks;
using AI.Driven.QE.Framework.Pages;
using AI.Driven.QE.Framework.Utils;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace AI.Driven.QE.Framework.Tests.StepDefinitions;

/// <summary>
/// Connects SauceDemo Gherkin steps to the existing Playwright page objects.
/// </summary>
[Binding]
public sealed class LoginSteps
{
    private readonly ScenarioContext _scenarioContext;

    public LoginSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("I am on the SauceDemo login page")]
    public async Task GivenIAmOnTheSauceDemoLoginPage()
    {
        await new LoginPage(Page).NavigateAsync();
    }

    [When("I log in as the standard user")]
    public async Task WhenILogInAsTheStandardUser()
    {
        await LoginPage.SubmitCredentialsAsync(
            TestSettings.GetCredential("QE_USERNAME", "standard_user"),
            TestSettings.GetCredential("QE_PASSWORD", "secret_sauce"));
    }

    [When("I log in as the locked-out user")]
    public async Task WhenILogInAsTheLockedOutUser()
    {
        await LoginPage.SubmitCredentialsAsync(
            TestSettings.GetCredential("QE_LOCKED_OUT_USERNAME", "locked_out_user"),
            TestSettings.GetCredential("QE_PASSWORD", "secret_sauce"));
    }

    [When("I complete checkout for the Sauce Labs Backpack")]
    public async Task WhenICompleteCheckoutForTheSauceLabsBackpack()
    {
        await Page.WaitForURLAsync("**/inventory.html");
        var productsPage = new ProductsPage(Page);
        await productsPage.AddBackpackToCart();
        await productsPage.OpenCart();

        var cartPage = new CartPage(Page);
        await cartPage.WaitForItem("Sauce Labs Backpack");
        await cartPage.StartCheckout();

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.EnterInformation("QE", "Demo", "10001");
        await checkoutPage.CompleteOrder();
    }

    [Then("I should see the products page")]
    public async Task ThenIShouldSeeTheProductsPage()
    {
        await Page.WaitForURLAsync("**/inventory.html");
        await Page.GetByText("Products", new PageGetByTextOptions { Exact = true })
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.That(Page.Url, Does.Contain("/inventory.html"));
    }

    [Then("I should see the locked-out error message")]
    public async Task ThenIShouldSeeTheLockedOutErrorMessage()
    {
        var errorMessage = Page.Locator("[data-test='error']");
        await errorMessage.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.That(
            await errorMessage.InnerTextAsync(),
            Does.Contain("Epic sadface: Sorry, this user has been locked out"));
    }

    [Then("I should see the order confirmation")]
    public async Task ThenIShouldSeeTheOrderConfirmation()
    {
        var confirmation = Page.GetByText("Thank you for your order!", new PageGetByTextOptions { Exact = true });
        await confirmation.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        Assert.That(Page.Url, Does.Contain("/checkout-complete.html"));
    }

    private LoginPage LoginPage => new(Page);

    private IPage Page =>
        _scenarioContext.TryGetValue("Page", out var value) && value is IPage page
            ? page
            : throw new InvalidOperationException("The Playwright page was not initialized for this scenario.");
}
