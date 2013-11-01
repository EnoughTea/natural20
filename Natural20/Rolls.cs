// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Natural20 {
    /// <summary> Helper methods for manipulating dice roll values. </summary>
    public static class Rolls {
        /// <summary> Applies specified dice operation to the given roll values. </summary>
        /// <remarks> <see cref="DiceOperation.Plus"/> and <see cref="DiceOperation.Minus"/> are modifying roll totals,
        /// so there isn't any good way to shove them in. For these operations 
        /// <paramref name="opValue"/> is being added to the first roll value.</remarks>
        /// <param name="rolls">Roll values to modify.</param>
        /// <param name="operation"></param>
        /// <param name="opValue">A value which will be used for modification.</param>
        /// <returns>Rolls modified with <paramref name="opValue"/> according to the given operation. 
        /// </returns>
        public static IEnumerable<double> ApplyOperation(IEnumerable<double> rolls, DiceOperation operation,
            double opValue) {
            Contract.Requires(rolls != null);
            Contract.Requires(operation != DiceOperation.None);
            Contract.Ensures(Contract.Result<IEnumerable<double>>() != null);

            return ApplyOperation(rolls, operation, new FixedDie(opValue));
        }

        /// <summary> Applies specified dice operation to the given roll values. </summary>
        /// <remarks> <see cref="DiceOperation.Plus"/> and <see cref="DiceOperation.Minus"/> are modifying roll totals,
        /// so there isn't any good way to shove them in. For these operations 
        /// <paramref name="opValueProvider"/>'s value is being added to the first roll value.</remarks>
        /// <param name="rolls">Roll values to modify.</param>
        /// <param name="operation"></param>
        /// <param name="opValueProvider">Rolls a value which will be used for modification.</param>
        /// <returns>Rolls modified with <paramref name="opValueProvider"/>'s value according to the given operation. 
        /// </returns>
        public static IEnumerable<double> ApplyOperation(IEnumerable<double> rolls, DiceOperation operation,
            IDie opValueProvider) {
            Contract.Requires(rolls != null);
            Contract.Requires(operation != DiceOperation.None);
            Contract.Requires(opValueProvider != null);
            Contract.Ensures(Contract.Result<IEnumerable<double>>() != null);

            // Plus and Minus are modifying roll totals, so there isn't any good way to shove them in.
            // Let's just add them to first value.
            if (operation == DiceOperation.Minus || operation == DiceOperation.Plus) {
                bool applied = false;
                foreach (var roll in rolls) {
                    if (!applied) {
                        yield return PerformRollCombination(roll, operation, opValueProvider.Roll());
                        applied = true;
                    }
                    else { yield return roll; }
                }
                // If rolls were empty, return the total modifier as is:
                if (!applied) {
                    var value = opValueProvider.Roll();
                    yield return (operation == DiceOperation.Minus) ? -value : value;
                }
            }
            else {  // For other operations just modify each roll value using the value provider.
                foreach (var roll in rolls) {
                    yield return PerformRollCombination(roll, operation, opValueProvider.Roll());
                }
            }
        }

        /// <summary> Adds spawned roll values to initial roll values when a given predicate evaluates to true 
        /// on any roll value. Predicate will be tests on spawned rolls as well. </summary>
        /// <param name="initialRolls">The initial roll values to check.</param>
        /// <param name="predicate">The predicate used for spawn checking.</param>
        /// <param name="spawnedRolls">The additional roll values which will be added to initial roll values if a given
        /// predicate evaluates to true. Those additional roll values are also tested by a predicate. </param>
        /// <returns> Resulting roll values. </returns>
        public static IEnumerable<double> SpawnContinuously(IEnumerable<double> initialRolls, Func<double, bool> predicate,
            Func<IEnumerable<double>> spawnedRolls) {
            Contract.Requires(initialRolls != null);
            Contract.Requires(predicate != null);
            Contract.Requires(spawnedRolls != null);
            Contract.Ensures(Contract.Result<IEnumerable<double>>() != null);

            foreach (var roll in initialRolls) {
                yield return roll;
                if (predicate(roll)) {
                    foreach (double nextRoll in SpawnContinuously(spawnedRolls(), predicate, spawnedRolls)) {
                        yield return nextRoll;
                    }
                }
            }
        }

        /// <summary> Adds spawned roll values to initial roll values when a given predicate evaluates to true 
        /// on any roll value. Predicate will be tested on initial rolls until first match. </summary>
        /// <param name="initialRolls">The initial roll values to check.</param>
        /// <param name="predicate">The predicate used for spawn checking.</param>
        /// <param name="spawnedRolls">The additional roll values which will be added to initial roll values if a given
        /// predicate evaluates to true.</param>
        /// <returns> Resulting roll values. </returns>
        public static IEnumerable<double> SpawnOnce(IEnumerable<double> initialRolls, Func<double, bool> predicate,
            Func<IEnumerable<double>> spawnedRolls) {
            Contract.Requires(initialRolls != null);
            Contract.Requires(predicate != null);
            Contract.Requires(spawnedRolls != null);
            Contract.Ensures(Contract.Result<IEnumerable<double>>() != null);

            bool spawnedOnce = false;
            foreach (var roll in initialRolls) {
                yield return roll;
                if (!spawnedOnce && predicate(roll)) {
                    foreach (double nextRoll in spawnedRolls()) {
                        yield return nextRoll;
                    }

                    spawnedOnce = true;
                }
            }
        }

        internal static double PerformRollCombination(double left, DiceOperation op, double right,
            int leftRollCount = 1) {
            switch (op) {
                case DiceOperation.Plus: return left + right;
                case DiceOperation.Minus: return left - right;
                case DiceOperation.Divide: return !right.EqualsZero() ? left / right : 0;
                case DiceOperation.Multiply: return left * right;
                case DiceOperation.EachPlus: return left + (leftRollCount * right);
                case DiceOperation.EachMinus: return left - (leftRollCount * right);
                default: throw new InvalidOperationException("Unknown dice operation.");
            }
        }
    }
}