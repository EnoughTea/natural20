// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Diagnostics.Contracts;

namespace Natural20 {
    /// <summary> Extension methods for <see cref="String"/>. </summary>
    public static class StringExtensions {
        /// <summary> Removes all occurences of a specified target string from the current string. </summary>
        /// <param name="self">The current string.</param>
        /// <param name="target">The string to remove.</param>
        /// <returns> The current string with removed occurences of a specified target string. </returns>
        public static string Remove(this string self, string target) {
            Contract.Requires(self != null);
            Contract.Requires(!String.IsNullOrEmpty(target));

            return self.Replace(target, String.Empty);
        }
    }
}
