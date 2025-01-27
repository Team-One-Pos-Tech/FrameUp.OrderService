Feature: Cancel Processing Order

Scenario: Cancel Processing Order
    Given I am logged in
	And There is processing orders
	When I cancel processing order
	Then Order Status Should Be Cancelled