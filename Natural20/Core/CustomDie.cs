// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.Serialization;

namespace Natural20.Core {
    /// <summary> Represents a base for a custom die. </summary>
    [DataContract]
    public abstract class CustomDie : IDie {
        private double maximum;
        private double minimum;

        /// <summary> Gets the minimum possible die value. </summary>
        [DataMember]
        public double Minimum {
            get { return minimum; }
            set {
                Contract.Requires(!Double.IsNaN(value));
                Contract.Requires(!Double.IsInfinity(value));

                minimum = value;
            }
        }

        /// <summary> Gets the maximum possible die value. </summary>
        [DataMember]
        public double Maximum {
            get { return maximum; }
            set {
                Contract.Requires(!Double.IsNaN(value));
                Contract.Requires(!Double.IsInfinity(value));

                maximum = value;
            }
        }

        /// <summary> Initializes a new instance of the <see cref="CustomDie"/> class. </summary>
        /// <param name="min">The minimum possible die value.</param>
        /// <param name="max">The maximum possible die value.</param>
        protected CustomDie(double min, double max) {
            Contract.Requires(!Double.IsNaN(min));
            Contract.Requires(!Double.IsNaN(max));
            Contract.Requires(!Double.IsInfinity(min));
            Contract.Requires(!Double.IsInfinity(max));

            Minimum = min;
            Maximum = max;
        }

        /// <summary> Performs a single roll and returns its value. </summary>
        /// <returns> Roll value. </returns>
        public double Roll() {
            return MakeSingleRoll(Dice.Random);
        }

        /// <summary> Performs a specified amount of rolls and returns their values. </summary>
        /// <param name="count"> The roll count. </param>
        /// <returns> Roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        public IEnumerable<double> Roll(int count) {
            for (int i = 0; i < count; i++) {
                yield return MakeSingleRoll(Dice.Random);
            }
        }

        /// <summary> Returns a <see cref="String" /> that represents this instance. </summary>
        /// <returns> A <see cref="String" /> that represents this instance. </returns>
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);

            if ((Minimum - 1).EqualsZero()) {
                return "d" + ((int)Maximum).ToString(CultureInfo.InvariantCulture);
            }

            return "[" + ((int)Minimum).ToString(CultureInfo.InvariantCulture) + ", " +
                ((int)Maximum).ToString(CultureInfo.InvariantCulture) + "]";
        }

        /// <summary> Returns one value from possible die roll values. </summary>
        /// <param name="random">The random number generator.</param>
        /// <returns>Single value from die distribution.</returns>
        protected abstract double MakeSingleRoll(Random random);
    }
}