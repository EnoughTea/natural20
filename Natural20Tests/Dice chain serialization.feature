Feature: Data contract serialization
	In order to preserve my complex dice chains
	I want to be able to serialize and deserialize dice chains using data contracts

Scenario: Serialize a die
	Given created interval die 50-100
	When I serialize interval die 50-100 using data contracts
	Then interval die 50-100 should be validly serialized

Scenario: Serialize a dice chain
	Given created dice chain d2d20 multiplied by interval 3-6 plus 40
	When I serialize dice chain using data contracts
	Then dice chain should be validly serialized