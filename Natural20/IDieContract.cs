// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Natural20 {
    [ContractClassFor(typeof(IDie))]
    internal abstract class IDieContract : IDie {
        /// <summary> Gets the minimum possible generated value. </summary>
        public double Minimum {
            get {
                Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));
                Contract.Ensures(!Double.IsInfinity(Contract.Result<double>()));

                return 0;
            }
        }

        /// <summary> Gets the maximum possible generated value. </summary>
        public double Maximum {
            get {
                Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));
                Contract.Ensures(!Double.IsInfinity(Contract.Result<double>()));

                return 0;
            }
        }

        /// <summary> Performs a single roll and returns its value. </summary>
        /// <returns> Roll value. </returns>
        public double Roll() {
            Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));
            Contract.Ensures(!Double.IsInfinity(Contract.Result<double>()));

            return 0;
        }

        /// <summary> Performs a specified amount of rolls and returns their values. </summary>
        /// <param name="count"> The roll count. </param>
        /// <returns> Roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        public IEnumerable<double> Roll(int count) {
            Contract.Requires(count > 0);
            Contract.Ensures(Contract.Result<IEnumerable<double>>() != null);

            yield break;
        }
    }
}