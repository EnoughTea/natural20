// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;

namespace Natural20 {
    /// <summary>Represents a way to perform a dice roll and a way to see how many rolls should be performed.</summary>
    [DataContract, KnownType("GetDieTypes")]
    public class SeveralDice : IDie {
        private IDie rollValueProvider;

        /// <summary> Gets the maximum possible die value. </summary>
        public double Maximum {
            get {
                Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));

                int rollMax = (RollValueProvider != null) ? (int)RollValueProvider.Maximum : 0;
                return MaximumCount * rollMax;
            }
        }

        /// <summary> Gets the minimum possible die value. </summary>
        public double Minimum {
            get {
                Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));

                int rollMin = (RollValueProvider != null) ? (int)RollValueProvider.Minimum : 0;
                return MinimumCount * rollMin;
            }
        }

        /// <summary> Gets the maximum possible roll count. </summary>
        public int MaximumCount {
            get { return (RollCountProvider != null) ? (int)RollCountProvider.Maximum : 1; }
        }

        /// <summary> Gets the minimum possible roll count. </summary>
        public int MinimumCount {
            get { return (RollCountProvider != null) ? (int)RollCountProvider.Minimum : 1; }
        }

        /// <summary> Gets something which can provide amount of rolls. Null value means 1 roll. </summary>
        [DataMember]
        public IDie RollCountProvider { get; set; }

        /// <summary> Gets something which can provide roll value. </summary>
        [DataMember]
        public IDie RollValueProvider {
            get { return rollValueProvider; }
            set { 
                Contract.Requires(value != null);

                rollValueProvider = value;
            }
        }

        /// <summary> Initializes a new instance of the <see cref="SeveralDice"/> class. </summary>
        /// <param name="rollValue">Roll value.</param>
        public SeveralDice(double rollValue)
            : this(rollValue, 1) {
                Contract.Requires(!Double.IsNaN(rollValue));
        }

        /// <summary> Initializes a new instance of the <see cref="SeveralDice"/> class. </summary>
        /// <param name="rollValue">Roll value.</param>
        /// <param name="rollCount">Amount of rolls.</param>
        public SeveralDice(double rollValue, int rollCount)
            : this(new FixedDie(rollValue), new FixedDie(rollCount)) {
            Contract.Requires(!Double.IsNaN(rollValue));
            Contract.Requires(rollCount > 0);
        }

        /// <summary> Initializes a new instance of the <see cref="SeveralDice"/> class. </summary>
        /// <param name="rollValueProvider">Something which can provide roll value.</param>
        /// <param name="rollCount">Amount of rolls. </param>
        public SeveralDice(IDie rollValueProvider, int rollCount) 
            : this (rollValueProvider, new FixedDie(rollCount)) {
            Contract.Requires(rollValueProvider != null);
            Contract.Requires(rollCount > 0);
        }

        /// <summary> Initializes a new instance of the <see cref="SeveralDice"/> class. </summary>
        /// <param name="rollValueProvider">Something which can provide roll value.</param>
        public SeveralDice(IDie rollValueProvider) 
            : this(rollValueProvider, null) {
            Contract.Requires(rollValueProvider != null);
        }
            
        /// <summary> Initializes a new instance of the <see cref="SeveralDice"/> class. </summary>
        /// <param name="rollValueProvider">Something which can provide roll value.</param>
        /// <param name="rollCountProvider">Something which can provide amount of rolls. Null value means 1 roll. 
        /// </param>
        public SeveralDice(IDie rollValueProvider, IDie rollCountProvider) {
            Contract.Requires(rollValueProvider != null);

            RollValueProvider = rollValueProvider;
            RollCountProvider = rollCountProvider;
        }

        /// <summary> Perfroms an amount of rolls specified by <see cref="RollCountProvider"/> and returns their total.
        /// </summary>
        /// <returns> Rolls total. </returns>
        public double Roll() {
            int count = (RollCountProvider != null) ? (int)RollCountProvider.Roll() : 1;
            return (RollValueProvider != null) ? RollValueProvider.Roll(count).Sum() : 0;
        }

        /// <summary> Performs a specified amount of rolls and returns their values. </summary>
        /// <param name="count"> The roll count. </param>
        /// <returns> Roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        public IEnumerable<double> Roll(int count) {
            for (int i = 0; i < count; i++) {
                yield return Roll();
            }
        }

        /// <summary> Perfroms an amount of rolls specified by <see cref="RollCountProvider" />
        /// and returns their values.
        /// </summary>
        /// <returns> Roll values. </returns>
        public IEnumerable<double> RollSeparately() {
            Contract.Ensures(Contract.Result<IEnumerable<double>>() != null);

            int count = (RollCountProvider != null) ? (int)RollCountProvider.Roll() : 1;
            for (int i = 0; i < count; i++) {
                yield return RollValueProvider.Roll();

            }
        }

        /// <summary> Returns a <see cref="String" /> that represents this instance. </summary>
        /// <returns> A <see cref="String" /> that represents this instance. </returns>
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);

            // Flag which will stop "1" being shown as a dice count, so "1d6" will be shown as just"d6".
            bool useCountString;
            if (RollCountProvider is FixedDie) {
                int fixedCount = (int)((FixedDie)RollCountProvider).Roll();
                useCountString = (fixedCount > 1);
            }
            else { useCountString = (RollCountProvider != null); }

            string count = useCountString ? RollCountProvider.ToString() : String.Empty;
            string value = RollValueProvider.ToString();
            return count + value;
        }

        internal double Roll(out int rollCount) {
            Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));

            rollCount = (RollCountProvider != null) ? (int)RollCountProvider.Roll() : 1;
            return RollValueProvider.Roll(rollCount).Sum();
        }

// ReSharper disable UnusedMember.Local
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Called by data contract serializer via [KnownType] attribute.")]
        private static IEnumerable<Type> GetDieTypes() {
            return Dice.GetAllDieTypes();
        }
    }
}