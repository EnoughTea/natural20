using System.Linq;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Natural20;

namespace Natural20Tests {
    [Binding]
    public class RollADiceChainSteps {
        private DiceChain c2d6p3;
        private DiceChain cd4d6mr2t5;
        private DiceChain c3d8ep3;
        private DiceChain cd2d10edd2p10;
        private double[] rolls;

        #region Create and roll a 2d6+3 manually

        [Given("2 integral dice with 6 faces and a plus 3 bonus")]
        public void DieWith6FacesShowingADifferentIntegralNumberFrom1To6() {
            var c2d6 = new DiceChain(new SeveralDice(new Die(6), 2));
            c2d6p3 = c2d6.Append(DiceOperation.Plus, new SeveralDice(3));
        }

        [When("2d6 plus 3 is rolled 100000 times")]
        public void D6IsRolled100000Times() {
            rolls = Enumerable.Range(0, 100000).Select(_ => c2d6p3.Roll()).ToArray();
        }

        [Then("resulting amount for each roll should be between 5 and 15 inclusively")]
        public void ResultingAmountForEachRollShouldBeBetween1And6Inclusively() {
            Assert.IsFalse(rolls.Any(roll => roll < 5 || roll > 15), "Some rolls were out of [5, 15]");
        }

        [Then("2d6 plus 3 distribution should be good")]
        public void D6DistributionShouldBeGood() {
            AssertTools.IsUniformishDistribution(rolls, c2d6p3.Minimum, c2d6p3.Maximum);
        }

        #endregion


        #region Create and roll a d4d6 - 1r[2-5] using fluent interface

        [Given("d4d6 and minus one interval 2-5")]
        public void D4D6MinusInterval2to5() {
            cd4d6mr2t5 = Dice.Take(Dice.D4).D(6).Minus(new Die(2, 5));
        }

        [When("d4d6 minus 1 interval 2-5 is rolled 100000 times")]
        public void D4D6IsRolled100000Times() {
            rolls = cd4d6mr2t5.Roll(100000).ToArray();
        }

        [Then("resulting amount for each roll should be between -4 and 22 inclusively")]
        public void ResultingAmountForEachRollShouldBeBetween4And22Inclusively() {
            Assert.IsFalse(rolls.Any(roll => roll < -4 || roll > 22), "Some rolls were out of [-4, 22]");
        }

        [Then("d4d6 minus 1 interval 2-5 distribution should be good")]
        public void D4D6DistributionShouldBeGood() {
            // We're combining several random dice, so distribution is not uniform at all, 
            // since sums in lower range have higher chance of appearing. 
            var statistics = new DescriptiveStatistics(rolls);
            Assert.AreEqual(cd4d6mr2t5.Minimum, statistics.Minimum, "Expected min differs from actual min");
            Assert.AreEqual(cd4d6mr2t5.Maximum, statistics.Maximum, "Expected max differs from actual max");
            Assert.AreEqual(5.25, statistics.Mean, 1, "Expected mean differs to much from actual mean");
            Assert.Greater(statistics.Skewness, 0.2, "Distribution is not skewed enough");
            Assert.Less(statistics.Kurtosis, 0, "Distribution is not platykurtic at all");
        }

        #endregion 


        #region Create and roll a 3d8(+3) using fluent interface

        [Given("3d8 with plus 3 to each roll")]
        public void D3D8WithPlus3ToEach() {
            c3d8ep3 = Dice.Take(3).D(8).EachPlus(3);
        }

        [When("3d8 with plus 3 to each roll is rolled 100000 times")]
        public void D3D8WithPlus3ToEachIsRolled10000Times() {
            rolls = c3d8ep3.Roll(100000).ToArray();
        }

        [Then("resulting amount for each roll should be between 12 and 33 inclusively")]
        public void ResultingAmountForEachRollShouldBeBetween12And33Inclusively() {
            Assert.IsFalse(rolls.Any(roll => roll < 12 || roll > 33), "Some rolls were out of [12, 33]");
        }

        [Then("3d8 with plus 3 to each roll distribution should be good")]
        public void D3D8Plus3ToEachDistributionShouldBeGood() {
            AssertTools.IsUniformishDistribution(rolls, c3d8ep3.Minimum, c3d8ep3.Maximum);
        }

        #endregion


        #region Create and roll a d2d10/d2*2 using fluent interface

        [Given("d2d10 divide by d2 mul 2")]
        public void D2D10DivideByD2MultiplyBy2() {
            cd2d10edd2p10 = Dice.Take(Dice.D2).D(10).Divide(Dice.D2).Multiply(2);
        }

        [When("d2d10 divide by d2 mul 2 is rolled 100000 times")]
        public void D2D10DivideByD2MultiplyBy2IsRolled10000Times() {
            rolls = cd2d10edd2p10.Roll(100000).ToArray();
        }

        [Then("resulting amount for each roll should be between 1 and 40 inclusively")]
        public void ResultingAmountForEachRollShouldBeBetween1And40Inclusively() {
            Assert.IsFalse(rolls.Any(roll => roll < 1 || roll > 40), "Some rolls were out of [1, 40]");
        }

        [Then("d2d10 divide by d2 mul 2 distribution should be good")]
        public void D2D10DivideByD2MultiplyBy2DistributionShouldBeGood() {
            var statistics = new DescriptiveStatistics(rolls);
            Assert.AreEqual(cd2d10edd2p10.Minimum, statistics.Minimum, "Expected min differs from actual min");
            Assert.AreEqual(cd2d10edd2p10.Maximum, statistics.Maximum, "Expected max differs from actual max");
            Assert.AreEqual(12.25, statistics.Mean, 1, "Expected mean differs to much from actual mean");
            Assert.Greater(statistics.Skewness, 0.8, "Distribution is not skewed enough");
            Assert.Less(statistics.Kurtosis, 0, "Distribution is not platykurtic at all");
        }

        #endregion
    }
}