// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using Natural20.Core;

namespace Natural20 {
    /// <summary> Represents a couple of dice chained together. Typically created via <see cref="Dice.Take(int)"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // 2d6+3
    /// var d6plus3 = Dice.Take(2).D(6).Plus(3);
    /// var d6plus3_manual = new DiceChain(new DiceChainNode(new Die(6), 2)).Plus(3);
    /// // d4d6 - [2-5]
    /// var d4d6minus2_5 = Dice.Take(DiceBox.D4).D(6).Minus(new Die(2, 5));
    /// var d4d6minus2_5_manual = new DiceChain(new DiceChainNode(new Die(6), new Die(4))).Minus(new Die(2, 5));
    /// </code>
    /// </example>
    [DataContract, KnownType("GetDieTypes")]
    public class DiceChain : IDie {
        /// <summary> Gets the last link in the dice chain. </summary>
        [DataMember]
        public DiceChainLink LastLink { get; private set; }

        /// <summary> Gets the first link in the dice chain. </summary>
        public DiceChainLink FirstLink {
            get {
                DiceChainLink result = LastLink;
                while (result.Previous != null) {
                    result = result.Previous;
                }

                return result;
            }
        }

        /// <summary> Gets the maximum possible die value. </summary>
        public double Maximum { get { return LastLink.Maximum; } }

        /// <summary> Gets the minimum possible die value. </summary>
        public double Minimum { get { return LastLink.Minimum; } }

        /// <summary> Initializes a new instance of the <see cref="DiceChain"/> class. </summary>
        /// <param name="node">The first node for the dice chain.</param>
        public DiceChain(SeveralDice node) {
            Contract.Requires(node != null);

            LastLink = new DiceChainLink(DiceOperation.Plus, node);
        }

        /// <summary> Appends a new chain node to the dice chain using a specified operation.</summary>
        /// <remarks> Append(DiceOperation.Minus, Dice.D10) is equivalent to Minus(Dice.D10). </remarks>
        /// <param name="linkOperation">The operation used to operate on dice rolls between chain nodes.</param>
        /// <param name="linkContent">Actual content of the chain node.</param>
        /// <returns>Dice chain with new node appended.</returns>
        public DiceChain Append(DiceOperation linkOperation, SeveralDice linkContent) {
            Contract.Requires(linkOperation != DiceOperation.None);
            Contract.Requires(linkContent != null);
            
            LastLink = new DiceChainLink(linkOperation, linkContent, LastLink);
            return this;
        }

        #region Divide

        /// <summary> Appends a fixed value to the dice chain for the previous chain value to be divided by.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Dice chain with a fixed value appended.</returns>
        public DiceChain Divide(double value) {
            return Divide(new FixedDie(value));
        }

        /// <summary> Appends a value provider to the dice chain for the previous chain value to be divided by.
        /// </summary>
        /// <param name="valueProvider">The provider of the value to add.</param>
        /// <returns>Dice chain with a value provider appended.</returns>
        public DiceChain Divide(IDie valueProvider) {
            Contract.Requires(valueProvider != null);

            return Append(DiceOperation.Divide, new SeveralDice(valueProvider));
        }

        /// <summary> Prepares several dice to append to the dice chain tfor the previous chain value to be divided by.
        /// </summary>
        /// <param name="amount">Amount of dice to append to the dice chain.</param>
        public DiceChainSetup DivideTake(double amount) {
            Contract.Requires(!Double.IsNaN(amount));
            Contract.Requires(amount > 0);

            return DivideTake(new FixedDie(amount));
        }

        /// <summary> Prepares several dice to append to the dice chain for the previous chain value to be divided by.
        /// </summary>
        /// <param name="amountProvider">Provider of the amount of dice to append to the dice chain.</param>
        public DiceChainSetup DivideTake(IDie amountProvider) {
            Contract.Requires(amountProvider != null);

            return new DiceChainSetup(amountProvider, DiceOperation.Divide, this);
        }

        #endregion Divide

        #region Plus

        /// <summary> Appends a fixed value to the dice chain to add to each of the previous chain rolls.
        /// </summary>
        /// <param name="value">The value to add to the previous chain rolls.</param>
        /// <returns>Dice chain with a fixed value appended.</returns>
        public DiceChain EachPlus(double value) {
            return EachPlus(new FixedDie(value));
        }


        /// <summary> Appends a value provider to the dice chain to add to each of the previous chain rolls.
        /// </summary>
        /// <param name="valueProvider">The provider of the value to add to the previous chain rolls.</param>
        /// <returns>Dice chain with a value provider appended.</returns>
        public DiceChain EachPlus(IDie valueProvider) {
            Contract.Requires(valueProvider != null);

            return Append(DiceOperation.EachPlus, new SeveralDice(valueProvider));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be added to 
        /// each of the previous chain rolls. </summary>
        /// <param name="amount">Amount of dice to append to the dice chain.</param>
        public DiceChainSetup EachPlusTake(double amount) {
            Contract.Requires(!Double.IsNaN(amount));
            Contract.Requires(amount > 0);

            return EachPlusTake(new FixedDie(amount));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be added to 
        /// each of the previous chain rolls. </summary>
        /// <param name="amountProvider">Provider of the amount of dice to append to the dice chain.</param>
        public DiceChainSetup EachPlusTake(IDie amountProvider) {
            Contract.Requires(amountProvider != null);

            return new DiceChainSetup(amountProvider, DiceOperation.EachPlus, this);
        }

        /// <summary> Appends a fixed value to the dice chain which will be added to the previous chain value. </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Dice chain with a fixed value appended.</returns>
        public DiceChain Plus(double value) {
            return Plus(new FixedDie(value));
        }

        /// <summary> Appends a value provider to the dice chain which will be added to the previous chain value. </summary>
        /// <param name="valueProvider">The provider of the value to add.</param>
        /// <returns>Dice chain with a value provider appended.</returns>
        public DiceChain Plus(IDie valueProvider) {
            Contract.Requires(valueProvider != null);

            return Append(DiceOperation.Plus, new SeveralDice(valueProvider));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be added to 
        /// the previous chain value. </summary>
        /// <param name="amount">Amount of dice to append to the dice chain.</param>
        public DiceChainSetup PlusTake(double amount) {
            Contract.Requires(!Double.IsNaN(amount));
            Contract.Requires(amount > 0);

            return PlusTake(new FixedDie(amount));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be added to 
        /// the previous chain value. </summary>
        /// <param name="amountProvider">Provider of the amount of dice to append to the dice chain.</param>
        public DiceChainSetup PlusTake(IDie amountProvider) {
            Contract.Requires(amountProvider != null);

            return new DiceChainSetup(amountProvider, DiceOperation.Plus, this);
        }

        #endregion Plus

        #region Minus

        /// <summary> Appends a fixed value to the dice chain to subtract from each of the previous chain rolls.
        /// </summary>
        /// <param name="value">The value to subtract from the previous chain rolls.</param>
        /// <returns>Dice chain with a fixed value appended.</returns>
        public DiceChain EachMinus(double value) {
            return EachMinus(new FixedDie(value));
        }


        /// <summary> Appends a value provider to the dice chain to subtract from each of the previous chain rolls.
        /// </summary>
        /// <param name="valueProvider">The provider of the value to subtract from the previous chain rolls.</param>
        /// <returns>Dice chain with a value provider appended.</returns>
        public DiceChain EachMinus(IDie valueProvider) {
            Contract.Requires(valueProvider != null);

            return Append(DiceOperation.EachMinus, new SeveralDice(valueProvider));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be subtracted from 
        /// the previous chain value. </summary>
        /// <param name="amount">Amount of dice to append to the dice chain.</param>
        public DiceChainSetup EachMinusTake(double amount) {
            Contract.Requires(!Double.IsNaN(amount));
            Contract.Requires(amount > 0);

            return EachMinusTake(new FixedDie(amount));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be subtracted from 
        /// the previous chain value. </summary>
        /// <param name="amountProvider">Provider of the amount of dice to append to the dice chain.</param>
        public DiceChainSetup EachMinusTake(IDie amountProvider) {
            Contract.Requires(amountProvider != null);

            return new DiceChainSetup(amountProvider, DiceOperation.EachMinus, this);
        }

        /// <summary> Appends a fixed value to the dice chain which will be subtract from the previous chain value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Dice chain with a fixed value appended.</returns>
        public DiceChain Minus(double value) {
            return Minus(new FixedDie(value));
        }

        /// <summary>Appends a value provider to the dice chain which will be subtracted from the previous chain value.
        /// </summary>
        /// <param name="valueProvider">The provider of the value to add.</param>
        /// <returns>Dice chain with a value provider appended.</returns>
        public DiceChain Minus(IDie valueProvider) {
            Contract.Requires(valueProvider != null);

            return Append(DiceOperation.Minus, new SeveralDice(valueProvider));
        }

        /// <summary> Prepares several dice to append to the dice chain to subtract from the previous chain value.
        /// </summary>
        /// <param name="amount">Amount of dice to append to the dice chain.</param>
        public DiceChainSetup MinusTake(double amount) {
            Contract.Requires(!Double.IsNaN(amount));
            Contract.Requires(amount > 0);

            return MinusTake(new FixedDie(amount));
        }

        /// <summary> Prepares several dice to append to the dice chain to subtract from the previous chain value.
        /// </summary>
        /// <param name="amountProvider">Provider of the amount of dice to append to the dice chain.</param>
        public DiceChainSetup MinusTake(IDie amountProvider) {
            Contract.Requires(amountProvider != null);

            return new DiceChainSetup(amountProvider, DiceOperation.Minus, this);
        }

        #endregion Minus

        #region Multiply

        /// <summary> Appends a fixed value to the dice chain which will be multiplied by the previous chain value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Dice chain with a fixed value appended.</returns>
        public DiceChain Multiply(double value) {
            return Multiply(new FixedDie(value));
        }

        /// <summary> Appends a value provider to the dice chain which will be multiplied by the previous chain value.
        /// </summary>
        /// <param name="valueProvider">The provider of the value to add.</param>
        /// <returns>Dice chain with a value provider appended.</returns>
        public DiceChain Multiply(IDie valueProvider) {
            Contract.Requires(valueProvider != null);

            return Append(DiceOperation.Multiply, new SeveralDice(valueProvider));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be used to multiply 
        /// the previous chain value. </summary>
        /// <param name="amount">Amount of dice to append to the dice chain.</param>
        public DiceChainSetup MultiplyTake(double amount) {
            Contract.Requires(!Double.IsNaN(amount));
            Contract.Requires(amount > 0);

            return MultiplyTake(new FixedDie(amount));
        }

        /// <summary> Prepares appending of several dice to the dice chain. They will be used to multiply 
        /// the previous chain value. </summary>
        /// <param name="amountProvider">Provider of the amount of dice to append to the dice chain.</param>
        public DiceChainSetup MultiplyTake(IDie amountProvider) {
            Contract.Requires(amountProvider != null);

            return new DiceChainSetup(amountProvider, DiceOperation.Multiply, this);
        }

        #endregion Multiply

        /// <summary> Performs a single roll of the entire chain and returns its value. </summary>
        /// <returns> Entire chain roll value. </returns>
        public double Roll() {
            return LastLink.Roll();
        }

        /// <summary> Performs a specified amount of rolls of the entire chain and returns their values. </summary>
        /// <param name="count">The chain roll count.</param>
        /// <returns> Entire chain roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        public IEnumerable<double> Roll(int count) {
            return LastLink.Roll(count);
        }

        /// <summary> Rolls the dice chain once and accumulates step-by-step roll data for each chain node.
        /// It's not as efficient as usual <see cref="Roll()"/> methods and should be used only if
        /// individual roll values are needed. </summary>
        /// <remarks>This method allocates an array per dice chain node.</remarks>
        /// <param name="steps">Roll values of each step of the dice chain. Steps in dice chain go from left to right.
        /// </param>
        /// <returns>Value of a single roll.</returns>
        public double RollStepByStep(out List<DiceChainRollStep> steps) {
            return LastLink.RollStepByStep(out steps);
        }

        /// <summary> Returns a <see cref="String"/> that represents this instance. </summary>
        /// <returns> A <see cref="String"/> that represents this instance. </returns>
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);

            return LastLink.ToString();
        }

// ReSharper disable UnusedMember.Local
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Called by data contract serializer via [KnownType] attribute.")]
        private static IEnumerable<Type> GetDieTypes() {
            return Dice.GetAllDieTypes();
        }
    }
}
