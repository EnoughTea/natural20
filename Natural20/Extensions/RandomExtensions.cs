// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Natural20 {
    /// <summary> Extension methods for <see cref="Random"/>. </summary>
    public static class RandomExtensions {
        /// <summary> Picks random item from the given collection. </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The collection to select value from.</param>
        public static TItem NextItem<TItem>(this Random random, ICollection<TItem> items) {
            Contract.Requires(random != null);
            Contract.Requires(items != null);

            int length = items.Count;
            return (length != 0) ? items.ElementAt(random.Next(length)) : default(TItem);
        }

        /// <summary> Picks random item from the given list. </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The list to select value from.</param>
        public static TItem NextItem<TItem>(this Random random, IList<TItem> items) {
            Contract.Requires(random != null);
            Contract.Requires(items != null);

            int length = items.Count;
            return (length != 0) ? items[random.Next(length)] : default(TItem);
        }

        /// <summary> Picks several elements chosen at random non-repeating indices from the given list. </summary>
        /// <typeparam name="TItem">The type of the items.</typeparam>
        /// <param name="random">The random number generator.</param>
        /// <param name="list">The list to chose from.</param>
        /// <param name="count">The amount of elements to chose.</param>
        public static IEnumerable<TItem> NextSubset<TItem>(this Random random, IList<TItem> list, int count) {
            Contract.Requires(random != null);
            Contract.Requires(list != null);
            Contract.Requires(count <= list.Count);

            if (count == 0) { yield break; }

            var indices = new List<int>(Enumerable.Range(0, list.Count));
            for (int i = 0; i < count; i++) {
                int chosenIndex = random.Next(indices.Count);
                int indexOfChosenElement = indices[chosenIndex];
                indices.RemoveAt(chosenIndex);
                yield return list[indexOfChosenElement];
            }
        }

        /// <summary> Adds together several random numbers within a specified interval. </summary>
        /// <remarks> Honestly calls <see cref="Random.Next(int, int)"/> 'count' times. </remarks>
        /// <param name="random">The random number generator.</param>
        /// <param name="count">Number of times to get a random number from the interval.</param>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned.</param>
        public static int NextSeveral(this Random random, int count, int min, int max) {
            Contract.Requires(random != null);
            Contract.Requires(count >= 0);
            Contract.Requires(min <= max);

            int total = 0;
            for (int i = 0; i < count; i++) {
                total += random.Next(min, max);
            }

            return total;
        }

        /// <summary> Returns true with odds of 1 to 'chance'. </summary>
        public static bool OneIn(this Random random, int chance) {
            Contract.Requires(random != null);
            Contract.Requires(chance > 0);

            return random.Next(chance) == 0;
        }
    
        /// <summary> Cubic dice emulation. To roll your typical D&amp;D "1d10 + 5" call 'Roll(1, 10) + 5'. </summary>
        /// <param name="random">The random number generator.</param>
        /// <param name="dice">Number of the dice to roll.</param>
        /// <param name="faces">Face count of the single die.</param>
        /// <returns>Total roll score.</returns>
        public static int Roll(this Random random, int dice, int faces) {
            Contract.Requires(random != null);
            Contract.Requires(dice > 0);
            Contract.Requires(faces > 0);

            return NextSeveral(random, dice, 1, faces + 1);
        }

        /// <summary> Randomly shuffles a list using Fisher–Yates shuffle. </summary>
        /// <param name="random">The random number generator.</param>
        /// <param name="items">The list to shuffle.</param>
        public static void Shuffle<T>(this Random random, IList<T> items) {
            Contract.Requires(random != null);
            Contract.Requires(items != null);

            // Implementation of http://en.wikipedia.org/wiki/Fisher–Yates_shuffle :
            for (int i = items.Count - 1; i > 0; i--) {
                int swapIndex = random.Next(i + 1);
                items.Swap(swapIndex, i);
            }
        }
    }
}
