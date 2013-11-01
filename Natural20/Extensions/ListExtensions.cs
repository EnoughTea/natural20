// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Natural20 {
    /// <summary> Extension methods for <see cref="IList{T}"/> </summary>
    internal static class ListExtensions {
        /// <summary> Swaps the elements with the specified indices in the list. </summary>
        /// <typeparam name="T">List element type.</typeparam>
        /// <param name="list">The list where swapping will occur.</param>
        /// <param name="firstIndex">Index of the first element.</param>
        /// <param name="secondIndex">Index of the second element.</param>
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex) {
            Contract.Requires(list != null);

            var tmp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = tmp;
        }
    }
}
