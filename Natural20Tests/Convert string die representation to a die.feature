Feature: Convert string die representation to a die
	In order to store a die as a string
	I want to be able to convert between a die and its string representation

Scenario: Convert d12 to string
	Given created d12
	When d12 is converted to string
	Then result should be a valid string representation for a d12

Scenario: Convert string to interval [-100, -10]
	Given string representing interval from -100 to -10
	When the interval string parsed into a die
	Then result should be a valid string representation for interval from -100 to -10