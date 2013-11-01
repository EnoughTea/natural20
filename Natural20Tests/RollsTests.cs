using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Natural20;

namespace Natural20Tests {
    [TestFixture]
    public class RollsTests {
        [Test]
        public void ModifyEmptyWithTotals() {
            var emptyRolls = Enumerable.Empty<double>().ToArray();

            var actual = Rolls.ApplyOperation(emptyRolls, DiceOperation.Plus, 10).ToArray();
            CollectionAssert.AreEqual(new double[] { 10 }, actual, "Plus was applied incorrectly to empty rolls.");

            actual = Rolls.ApplyOperation(emptyRolls, DiceOperation.Minus, 10).ToArray();
            CollectionAssert.AreEqual(new double[] { -10 }, actual, "Minus was applied incorrectly to empty rolls.");

            actual = Rolls.ApplyOperation(emptyRolls, DiceOperation.Multiply, 10).ToArray();
            CollectionAssert.AreEqual(new double[0], actual, "Multiply was applied incorrectly to empty rolls.");

            actual = Rolls.ApplyOperation(emptyRolls, DiceOperation.Divide, 10).ToArray();
            CollectionAssert.AreEqual(new double[0], actual, "Divide was applied incorrectly to empty rolls.");
        }

        [Test]
        public void ModifyEmptyWithEach() {
            var emptyRolls = Enumerable.Empty<double>().ToArray();

            var actual = Rolls.ApplyOperation(emptyRolls, DiceOperation.EachPlus, 10).ToArray();
            CollectionAssert.AreEqual(new double[0], actual, "Plus to each roll was applied incorrectly to empty rolls.");

            actual = Rolls.ApplyOperation(emptyRolls, DiceOperation.EachMinus, 10).ToArray();
            CollectionAssert.AreEqual(new double[0], actual, "Minus to each roll was applied incorrectly to empty rolls.");
        }

        [Test]
        public void ModifyRollsWithEach() {
            var rolls = new double[] { 5, 10, 15, 20 };

            var actual = Rolls.ApplyOperation(rolls, DiceOperation.EachPlus, 5).ToArray();
            CollectionAssert.AreEqual(rolls.Select(roll => roll + 5), actual, "Plus to each roll was applied incorrectly to rolls.");

            actual = Rolls.ApplyOperation(rolls, DiceOperation.EachMinus, 10).ToArray();
            CollectionAssert.AreEqual(rolls.Select(roll => roll - 10), actual, "Minus to each roll was applied incorrectly to rolls.");
        }

        [Test]
        public void ModifyRolls() {
            var rolls = new double[] { -10, 0, 10 };

            var actual = Rolls.ApplyOperation(rolls, DiceOperation.Plus, 5).ToArray();
            CollectionAssert.AreEqual(new double[] { -5, 0, 10}, actual, "Plus was applied incorrectly to rolls.");

            actual = Rolls.ApplyOperation(rolls, DiceOperation.Minus, 10).ToArray();
            CollectionAssert.AreEqual(new double[] { -20, 0, 10 }, actual, "Minus was applied incorrectly to rolls.");

            actual = Rolls.ApplyOperation(rolls, DiceOperation.Divide, 5).ToArray();
            CollectionAssert.AreEqual(new double[] { -2, 0, 2 }, actual, "Divide was applied incorrectly to rolls.");

            actual = Rolls.ApplyOperation(rolls, DiceOperation.Multiply, 4).ToArray();
            CollectionAssert.AreEqual(new double[] { -40, 0, 40 }, actual, "Multiply was applied incorrectly to rolls.");
        }

        [Test]
        public void DivideRollsByZero() {
            var rolls = new double[] { -10, 0, 10 };

            var actual = Rolls.ApplyOperation(rolls, DiceOperation.Divide, 0).ToArray();
            CollectionAssert.AreEqual(new double[] { 0, 0, 0 }, actual, "Division by zero was applied incorrectly to rolls.");
        }
    }
}
