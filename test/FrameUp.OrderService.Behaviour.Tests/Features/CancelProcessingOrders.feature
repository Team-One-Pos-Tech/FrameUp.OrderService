Feature: Cancel Processing Order

Scenario: Cancel Processing Order
    Given There is processing orders
	When I cancel processing order
	Then Order status should be cancelled