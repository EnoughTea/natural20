// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Globalization;

namespace Natural20 {
    /// <summary> Extension methods for <see cref="Double"/> </summary>
    internal static class DoubleExtensions {
        /// <summary> Test to see if a value equals zero using epsilon. </summary>
        /// <param name="d">The value to test.</param>
        /// <param name="epsilon">The epsilon used for comparing the double value with zero.</param>
        /// <returns> True if value nearly equals zero, false otherwise. </returns>
        public static bool EqualsZero(this double d, double epsilon = 0.0001f) {
            return (d >= 0 - epsilon) && (d <= 0 + epsilon);
        }

        /// <summary> Returns a short string of invariant culture representing a specified <see cref="Double"/>. 
        /// </summary>
        /// <param name="d">The double value.</param>
        public static string ToShortString(this double d) {
            return d.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}
