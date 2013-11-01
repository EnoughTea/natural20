using NUnit.Framework;
using TechTalk.SpecFlow;
using Natural20;

namespace Natural20Tests {
    [Binding]
    public class ConvertStringDieRepresentationToADieSteps {
        private Die d12;
        private string d12repr;

        private string rm100tm10repr;
        private Die rm100tm10;

        #region Convert d12 to string

        [Given("created d12")]
        public void CreatedD12() {
            d12 = new Die(12);
        }

        [When("d12 is converted to string")]
        public void D12IsConvertedToString() {
            d12repr = d12.ToString();
        }

        [Then("result should be a valid string representation for a d12")]
        public void ResultShouldBeAVaildStringRepresentationForAD12() {
            Die parsedDie = Die.Parse(d12repr);
            Assert.AreEqual(d12.Maximum, parsedDie.Maximum, "Parsed die maximum is wrong");
            Assert.AreEqual(d12.Minimum, parsedDie.Minimum, "Parsed die minimum is wrong");
            Assert.AreEqual(d12repr, parsedDie.ToString(), "Parsed die representaion is wrong");
        }

        #endregion


        #region Convert string to interval [-100, -10]

        [Given("string representing interval from -100 to -10")]
        public void StringRepresentingIntervalFromMinus100ToMinus10() {
            rm100tm10repr = "[-100, -10]";
        }

        [When("the interval string parsed into a die")]
        public void IntervalStringParsedIntoADie() {
            rm100tm10 = Die.Parse(rm100tm10repr);
        }

        [Then("result should be a valid string representation for interval from -100 to -10")]
        public void ResultShouldBeAVaildStringRepresentationForIntervalFromMinus100ToMinus10() {
            Assert.AreEqual(-10, rm100tm10.Maximum, "Parsed die maximum is wrong");
            Assert.AreEqual(-100, rm100tm10.Minimum, "Parsed die minimum is wrong");
            Assert.AreEqual(rm100tm10repr, rm100tm10.ToString(), "Parsed die representaion is wrong");
        }

        #endregion
    }
}