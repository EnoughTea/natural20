using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Natural20;

namespace Natural20Tests {
    [Binding]
    public class RollAnArbitraryDiceSteps {
        private Die d6;
        private Die interval;
        private double[] rolls;

        #region Create and roll a d6

        [Given("die with 6 faces with each face showing a different integral number from 1 to 6")]
        public void DieWith6FacesShowingADifferentIntegralNumberFrom1To6() {
            d6 = new Die(6);
        }

        [When("d6 is rolled 100000 times")]
        public void D6IsRolled10000Times() {
            rolls = d6.Roll(100000).ToArray();
        }

        [Then("resulting amount for each roll should be between 1 and 6 inclusively")]
        public void ResultingAmountForEachRollShouldBeBetween1And6Inclusively() {
            Assert.IsFalse(rolls.Any(roll => roll < 1 || roll > 6), "Some rolls were out of [1, 6]");
        }

        [Then("d6 distribution should be good")]
        public void D6DistributionShouldBeGood() {
            AssertTools.IsUniformishDistribution(rolls, d6.Minimum, d6.Maximum);
        }
        
        #endregion


        #region Create and roll a 50-100 interval die

        [Given("die with a single inclusive interval from 50 to 100")]
        public void DieWithASingleIntervalFrom50To100Inclusive() {
            interval = new Die(50, 100);
        }

        [When("interval die is rolled 100000 times")]
        public void IntervalDieIsRolled10000Times() {
            rolls = interval.Roll(100000).ToArray();
        }

        [Then("resulting amount for each roll should be between 50 and 100 inclusively")]
        public void ResultingAmountForEachRollShouldBeBetween50And100Inclusively() {
            Assert.IsFalse(rolls.Any(roll => roll < 50 || roll > 100), "Some rolls were out of [50, 100]");
        }

        [Then("interval die distribution should be good")]
        public void IntervalDieDistributionShouldBeGood() {
            AssertTools.IsUniformishDistribution(rolls, interval.Minimum, interval.Maximum);
        }

        #endregion
    }
}