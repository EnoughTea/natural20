Feature: Roll an arbitrary dice
	In order to be able to roll an integral or floating point die with any amount of faces
	I want to create a die with appropriate roll function

Scenario: Create and roll a standard d6
	Given die with 6 faces with each face showing a different integral number from 1 to 6
	When d6 is rolled 100000 times
	Then resulting amount for each roll should be between 1 and 6 inclusively
	Then d6 distribution should be good

Scenario: Create and roll a 50-100 interval die
	Given die with a single inclusive interval from 50 to 100
	When interval die is rolled 100000 times
	Then resulting amount for each roll should be between 50 and 100 inclusively
	Then interval die distribution should be good