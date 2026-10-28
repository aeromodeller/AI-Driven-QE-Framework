Feature: SauceDemo login and checkout
  As a quality engineer
  I want to validate key customer journeys
  So that login and checkout behavior is covered in readable acceptance scenarios

  Background:
    Given I am on the SauceDemo login page

  Scenario: Standard user signs in successfully
    When I log in as the standard user
    Then I should see the products page

  Scenario: Locked-out user receives an explanatory error
    When I log in as the locked-out user
    Then I should see the locked-out error message

  Scenario: Standard user completes a backpack purchase
    When I log in as the standard user
    And I complete checkout for the Sauce Labs Backpack
    Then I should see the order confirmation
