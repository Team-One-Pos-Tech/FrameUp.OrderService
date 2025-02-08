Feature: Get Processing Orders

Scenario: Get Processing Orders
    Given There is processing orders
	When I get processing orders
	Then I should see a list of my processing orders

Scenario: Get Processing Orders With Packages
    Given There is concluded processing orders
	When I get processing orders
	Then I have concluded processing orders with pacakges