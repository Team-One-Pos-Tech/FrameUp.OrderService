Feature: Get Processing Orders

Scenario: Get Processing Orders
    Given I am logged in
	And There is processing orders
	When I get processing orders
	Then I should see a list of my processing orders