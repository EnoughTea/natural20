using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Natural20;

namespace Natural20Tests {
    [Binding]
    public class DiceChainSerializationSteps {
        private Die r50t100;
        private Die deserializedR50t100;
        private MemoryStream serializedR50t100;
        private DataContractSerializer dieSerializer;

        private DiceChain d2d20mr3t6p40;
        private DiceChain deserializedD2d20mr3t6p40;
        private MemoryStream serializedD2d20mr3t6p40;
        private DataContractSerializer diceChainSerializer;

        #region Serialize a die

        [Given("created interval die 50-100")]
        public void CreatedR50T100() {
            r50t100 = new Die(50, 100);
        }

        [When("I serialize interval die 50-100 using data contracts")]
        public void SerializeIntervalDie50T100UsingDataContracts() {
            dieSerializer = new DataContractSerializer(typeof(Die));
            serializedR50t100 = new MemoryStream();
            dieSerializer.WriteObject(serializedR50t100, r50t100);
            serializedR50t100.Position = 0;
        }

        [Then("interval die 50-100 should be validly serialized")]
        public void IntervalDie50T100ShouldBeValidlySerialized() {
            deserializedR50t100 = (Die)dieSerializer.ReadObject(serializedR50t100);
            serializedR50t100.Close();
            Assert.AreEqual(r50t100.Maximum, deserializedR50t100.Maximum, "Serialized die maximum is wrong");
            Assert.AreEqual(r50t100.Minimum, deserializedR50t100.Minimum, "Serialized die minimum is wrong");
            Assert.AreEqual(r50t100.ToString(), deserializedR50t100.ToString(), "Serialized die repr is wrong");
        }

        #endregion


        #region Serialize a dice chain

        [Given("created dice chain d2d20 multiplied by interval 3-6 plus 40")]
        public void CreatedDiceChainD2D20Mr3T6P40() {
            d2d20mr3t6p40 = Dice.Take(Dice.D2).D(20).MultiplyTake(1).I(3, 6).Plus(40);
        }

        [When("I serialize dice chain using data contracts")]
        public void SerializeDiceChainUsingDataContracts() {
            diceChainSerializer = new DataContractSerializer(typeof(DiceChain));
            serializedD2d20mr3t6p40 = new MemoryStream();
            diceChainSerializer.WriteObject(serializedD2d20mr3t6p40, d2d20mr3t6p40);
            serializedD2d20mr3t6p40.Position = 0;
        }

        [Then("dice chain should be validly serialized")]
        public void DiceChainShouldBeValidlySerialized() {
            deserializedD2d20mr3t6p40 = (DiceChain)diceChainSerializer.ReadObject(serializedD2d20mr3t6p40);
            serializedD2d20mr3t6p40.Close();
            Assert.AreEqual(d2d20mr3t6p40.Maximum, deserializedD2d20mr3t6p40.Maximum, "Serialized die maximum is wrong");
            Assert.AreEqual(d2d20mr3t6p40.Minimum, deserializedD2d20mr3t6p40.Minimum, "Serialized die minimum is wrong");
            Assert.AreEqual(d2d20mr3t6p40.ToString(), deserializedD2d20mr3t6p40.ToString(), "Serialized die repr is wrong");
        }

        #endregion
    }
}