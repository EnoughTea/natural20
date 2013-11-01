// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Natural20 {
    /// <summary> Interface for any die. </summary>
    [ContractClass(typeof(IDieContract))]
    public interface IDie {
        /// <summary> Gets the maximum possible die value. </summary>
        double Maximum { get; }

        /// <summary> Gets the minimum possible die value. </summary>
        double Minimum { get; }

        /// <summary> Performs a single roll and returns its value. </summary>
        /// <returns> Roll value. </returns>
        double Roll();

        /// <summary> Performs a specified amount of rolls and returns their values. </summary>
        /// <param name="count"> The roll count. </param>
        /// <returns> Roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        IEnumerable<double> Roll(int count);
    }
}