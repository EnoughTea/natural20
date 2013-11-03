Feature: Roll a dice chain
	In order to combine rolls of the several dice in a certain way
	I want to create a dice chain specifying how each dice should be combined

Scenario: Create and roll a 2d6+3 manually
	Given 2 integral dice with 6 faces and a plus 3 bonus
	When 2d6 plus 3 is rolled 100000 times
	Then resulting amount for each roll should be between 5 and 15 inclusively
	Then 2d6 plus 3 distribution should be good

Scenario: Create and roll a d4d6 - [2-5] using fluent interface
	Given d4d6 and minus one interval 2-5
	When d4d6 minus 1 interval 2-5 is rolled 100000 times
	Then resulting amount for each roll should be between -4 and 22 inclusively
	Then d4d6 minus 1 interval 2-5 distribution should be good

Scenario: Create and roll a 3d8(+3) using fluent interface
	Given 3d8 with plus 3 to each roll
	When 3d8 with plus 3 to each roll is rolled 100000 times
	Then resulting amount for each roll should be between 12 and 33 inclusively
	Then 3d8 with plus 3 to each roll distribution should be good

Scenario: Create and roll a d2d10(/d2)*2 using fluent interface
	Given d2d10 divide by d2 mul 2
	When d2d10 divide by d2 mul 2 is rolled 100000 times
	Then resulting amount for each roll should be between 1 and 40 inclusively
	Then d2d10 divide by d2 mul 2 distribution should be good