using NUnit.Framework;
using Natural20;
using TechTalk.SpecFlow;

namespace Natural20Tests {
    [Binding]
    public class ConvertStringDiceChainRepresentationToADiceChainSteps {
        private DiceChain d2d20mr3t6p40;
        string d2d20mr3t6p40repr;

        private DiceChain d10d100em50p10;
        string d10d100em50p10repr;

        #region Convert d2d20 * d4[3,6] + 40 to string

        [Given("created d2d20 multiply by d4 interval 3-6 plus 40")]
        public void CreatedD2D20() {
            d2d20mr3t6p40 = Dice.Take(Dice.D2).D(20).MultiplyTake(Dice.D4).I(3, 6).Plus(40);
        }

        [When("d2d20 multiply by d4 interval 3-6 plus 40 dice chain is converted to string")]
        public void D2D20IsConvertedToString() {
            d2d20mr3t6p40repr = d2d20mr3t6p40.ToString();
        }

        [Then("result should be a valid string representation for a d2d20 multiply by d4 interval 3-6 plus 40 dice chain")]
        public void ResultShouldBeAVaildStringRepresentationForAD2D20DiceChain() {
            var parsedD2d20mr3t6p40 = Dice.Parse(d2d20mr3t6p40repr);
            Assert.AreEqual(d2d20mr3t6p40.Maximum, parsedD2d20mr3t6p40.Maximum, "Parsed die maximum is wrong");
            Assert.AreEqual(d2d20mr3t6p40.Minimum, parsedD2d20mr3t6p40.Minimum, "Parsed die minimum is wrong");
            Assert.AreEqual(d2d20mr3t6p40repr, parsedD2d20mr3t6p40.ToString(), "Parsed die representaion is wrong");
        }

        #endregion


        #region Convert d10d100 (-50) + 10 to string 
        
        [Given("created d10d100 minus 50 to each roll plus 10 to total")]
        public void CreatedD10D100Em50P10() {
            d10d100em50p10 = Dice.Take(Dice.D10).D(100).EachMinus(50).Plus(10);
        }

        [When("d10d100 minus 50 to each roll plus 10 to total dice chain is converted to string")]
        public void D10D100Em50P10IsConvertedToString() {
            d10d100em50p10repr = d10d100em50p10.ToString();
        }

        [Then("result should be a valid string representation for a d10d100 minus 50 to each roll plus 10 to total dice chain")]
        public void ResultShouldBeAVaildStringRepresentationForAD10D100Em50P10DiceChain() {
            var parsedD10d100em50p10 = Dice.Parse(d10d100em50p10repr);
            Assert.AreEqual(d10d100em50p10.Maximum, parsedD10d100em50p10.Maximum, "Parsed die maximum is wrong");
            Assert.AreEqual(d10d100em50p10.Minimum, parsedD10d100em50p10.Minimum, "Parsed die minimum is wrong");
            Assert.AreEqual(d10d100em50p10repr, parsedD10d100em50p10.ToString(), "Parsed die representaion is wrong");
        }

        #endregion
    }
}