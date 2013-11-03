# Natural20

Natural20 is a D&D-inspired dice roller which aims to make random-based mechanics clearer for random-heavy projects like certain RPGs and roguelikes.

Natural20 focuses mainly on separating complex dice rolls uinto several pieces.
There may be some level of 'enterprise overhead', so if your project does not need complex dice chains, chances are you would be better off using `Random.Next()`.

This is a young library which was just extracted from a small game and cleaned up so others could benefit from it, hopefully.

## Highlights

### Pros
  * Dice can be combined into dice chains.
  * Dice chains can show their minimum and maximum possible rolled values.
  * Dice and dice chains can be serialized via data contract serializer.
  * Dice and dice chains can be converted to string and parsed from string.
  * New dice types can be created via `IDie` implementation (one could also inherit from a `CustomDie`). Dice chains play nicely with custom dice; `Dice.Parse()` method can work with custom dice after extension via `Dice.AddNodeParser()`. 
  * Some neat extension methods for `Random` type.
  * Very permissive license.

### Cons
  * May contain some unnecessary complexity for simple dice-related tasks.
  * Built-in dice provide only integral numbers.
  * There should be more interesting dice with wacky distributions.


## Quickstart

### Roll a standard cubic die once
    var d6 = new Die(6);
    var roll = d6.Roll();


### Roll an interval die from 10 to 20 four times
    var interval10t20 = new Die(10, 20);
    roll = interval10t20.Roll(4).Sum();


### There is also a fixed die, which always returns the same value
It's useful when something expects IDie as value provider and you want to pass a constant value.
    
    var fixed50 = new FixedDie(50);

### Roll two d10's
    var d2d10 =new SeveralDice(Dice.D10, 2);
    // Compared to simple Dice.D10.Roll(2), SeveralDice provides statistics and serialization
    roll = d2d10.Roll();
    Console.WriteLine("Possible roll value range: " + d2d10.Minimum + " to " + d2d10.Maximum);


### Why were these dice rolled without rng?
Built-in dice get random numbers from thread-local `Dice.Random` for convenience.


### Roll a d4d10 / d2 * 3
Dice setups like these are better expressed as a dice chain.

    var d4d10divD2mul3 = Dice.Take(Dice.D4).D(10).Divide(Dice.D2).Multiply(3);
    // Math operations in dice chains are always executed from left to right, so this chain translates to:
    // take a 1-4 of d10 dice and divide their roll value by 2 with 50 % chance, then multiply resulting roll value by 3.
    roll = d4d10divD2mul3.Roll();

Dice chains also provide statistics and serialization.
First chain was created via fluent interface, but it is also possible to create chains in a more explicit way:

    d4d10divD2mul3 = new DiceChain(
        new SeveralDice(new Die(10), new Die(4)))               // d4d10
        .Append(DiceOperation.Divide, new SeveralDice(2))       // / 2
        .Append(DiceOperation.Multiply, new SeveralDice(3));    // * 3


### Simple real life example: typical ability scores roll
Roll 4d6 for each of the six ability scores. Drop the low die in each roll.

    var scores = from ability in Enumerable.Range(0, 6)         // For each of six ability scores
                 let dieRolls = new Die(6).Roll(4).ToList()       // roll d6 four times,
                 let lowDie = dieRolls.Min()              // find a low die, and
                 let fitRolls = dieRolls.Where(roll => roll != lowDie)  // drop the low die from the rolls.
                 select fitRolls.Sum();                         // Calculate an ability score.


### Getting warmer: trailing 'lucky tens'
Roll 2d10, then perform additional d10 roll for each die which rolled 9 or 10.

    // With helper method:
    var rolls = Rolls.SpawnContinuously(Dice.D10.Roll(2), roll => roll == 9 || roll == 10, () => Dice.D10.Roll(1));

    // Manual way would be:
    var d10 = new Die(10);
    rolls = PropagateLuckyRolls(d10.Roll(2));

    private static IEnumerable<double> PropagateLuckyRolls(IEnumerable<double> rolls) {
        var d10 = new Die(10);  // You probably shouldn't allocate the same die for each recursive call in real code.
        foreach (var roll in rolls) {
            yield return roll;
            if (roll == 9 || roll == 10) {
                foreach (double nextRoll in PropagateLuckyRolls(d10.Roll(1))) {
                    yield return nextRoll;
                }
            }
        }
    }


### Hot stuff: Burning Hands, a damage dealing spell
  * 1st level spell - 1d8 damage  + Intelligence modifier.
  * 3rd level spell - 2d8 damage  + Intelligence modifier for each die.
  * 5th level spell - 4d8 damage  + Intelligence modifier for each die.
  * 7th level spell - 6d8 damage  + Intelligence modifier for each die.
  * 9th level spell - 10d8 damage + Intelligence modifier for each die.

There are some additional simple classes involved here: Character, IntModProvider and BurningHandsDiceCountProvider. 
They are located at the end of this example:

    // Let's create a smart sorcerer first:
    var sorcerer = new Character { Int = 16, BurningHandsSpellLevel = 1 };
    // Then let's create a provider for a modifier derived from the sorcerer's Intelligence:
    var intMod = new IntModProvider { Character = sorcerer };
    // Create a provider for a spell damage dice count, which depends on the sorcerer's current spell level.
    var spellDiceCount = new BurningHandsDiceCountProvider { Character = sorcerer };

    // Merge these into a chain:
    DiceChain damageDice = Dice.Take(spellDiceCount).D(8).EachPlus(intMod);
    // Let's burn:
    double damage = damageDice.Roll();


    // Let's say we want to display the spell damage, so check the dice chain stats:
    Trace.WriteLine("Damage: " + damageDice.Minimum + " to " + damageDice.Maximum);
    // Outputs 'Damage: 4 to 11', which is equal to '1d8+3', looks good.

    // What if sorcerer gains two levels of proficiency in the spell?
    sorcerer.BurningHandsSpellLevel += 2;
    Console.WriteLine("Damage: " + damageDice.Minimum + " to " + damageDice.Maximum);
    // Outputs 'Damage: 8 to 22', which is equal to '2d8(+3)', once again looks good.
    // Parens mean that 3 is apllied to each die separately instead of affecting dice sum.


    // Now let's say when you miss an enemy with burning hands, you still can deal some damage.
    // In this case damage dealt to the target equals to sum of each damage die that rolled its maximum result.
    // Normally you don't want to deal with individual roll values when using dice chain, 
    // but there is a method just in case:

    List<DiceChainRollStep> rollParts;
    // Normal hit damage is returned by dice chain as usual:
    damage = damageDice.RollStepByStep(out rollParts);

    // Damage on a miss is calculated as a sum of each damage die that rolled its maximum result. 
    // Looking at dice chain 'Xd8+Y' we can see that damage dice are rolled in the first (leftmost) step.
    double missedDamage = rollParts[0].Rolls.Where(roll => roll == Dice.D8.Maximum).Sum();


    // For the sake of completeness, there is a way of achieving the same thing without a dice chain.
    // That way we have to do all calculations manually.
    // Let's bundle D8 with spell damage dice count:
    var rawSpellDamage = new SeveralDice(Dice.D8, spellDiceCount);
    // Make a damage roll, rolling each die separately:
    var rawRolls = rawSpellDamage.RollSeparately().ToArray();
    // .ToArray() is important here. Otherwise different roll values would be generated on each enumeration.

    // Damage on a miss:
    missedDamage = rawRolls.Where(roll => roll == Dice.D8.Maximum).Sum();
    // Damage on a hit:
    damage = rawRolls.Select(roll => roll + intMod.Roll()).Sum();

    // Classes that were used:
    public class Character {
        public int Int { get; set; }
        public int IntMod { get { return (int)Math.Floor((Int - 10) / 2d); } }
        public int BurningHandsSpellLevel { get; set; }
    }

    public class BurningHandsDiceCountProvider : IDie {
        // (Spell level, Damage dice count)
        private readonly Dictionary<int, int> progressionTable = new Dictionary<int, int> {
            {1, 1}, {2, 1}, {3, 2}, {4, 2}, {5, 4}, {6, 4}, {7, 6}, {8, 6}, {9, 10}
        };

        public Character Character { get; set; }

        // Same as minimum.
        public double Maximum { get { return Minimum; } }

        // Returns Burning Hands damage die count for the character's knowledge of this spell.
        public double Minimum { get { return progressionTable[Character.BurningHandsSpellLevel]; } }

        public double Roll() { return Minimum; }

        public IEnumerable<double> Roll(int count) { for (int i = 0; i < count; i++) { yield return Roll(); } }

        // I chose not to print '1' for dice count providers since 'd8' is preferable to '1d8' for me:
        public override string ToString() { return (Minimum > 1) ? Minimum.ToString() : String.Empty; }
    }

    public class IntModProvider : IDie {
        public Character Character { get; set; }

        // Same as Minimum.
        public double Maximum { get { return Minimum; } }

        // Character's Intelligence modifier.
        public double Minimum { get { return Character.IntMod; } }

        // Returns character's Intelligence modifier.
        public double Roll() { return Minimum; }

        public IEnumerable<double> Roll(int count) { for (int i = 0; i < count; i++) { yield return Roll(); } }

        public override string ToString() { return Minimum.ToString(); }
    }


## NuGet references
You may notice that NuGet packages are not in the repository, so do not forget to set up package restoration in Visual Studio:

Tools menu → Options → Package Manager → General → "Allow NuGet to download missing packages during build" should be selected. 

If you have a build server then it needs to be setup with an environment variable 'EnableNuGetPackageRestore' set to true. 