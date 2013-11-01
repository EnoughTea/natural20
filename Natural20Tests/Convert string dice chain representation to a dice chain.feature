Feature: Convert string dice chain representation to a dice chain
	In order to store a dice chain as a string
	I want to be able to convert between a dice chain and its string representation

Scenario: Convert d2d20 * d4[3,6] + 40 to string
	Given created d2d20 multiply by d4 interval 3-6 plus 40
	When d2d20 multiply by d4 interval 3-6 plus 40 dice chain is converted to string
	Then result should be a valid string representation for a d2d20 multiply by d4 interval 3-6 plus 40 dice chain

Scenario: Convert d10d100 (-50) + 10 to string
	Given created d10d100 minus 50 to each roll plus 10 to total
	When d10d100 minus 50 to each roll plus 10 to total dice chain is converted to string
	Then result should be a valid string representation for a d10d100 minus 50 to each roll plus 10 to total dice chain