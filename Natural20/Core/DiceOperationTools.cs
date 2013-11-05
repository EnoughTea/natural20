// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Diagnostics.Contracts;

namespace Natural20.Core {
    /// <summary>Helper methods for <see cref="DiceOperation"/>.</summary>
    internal static class DiceOperationTools
    {
        /// <summary> Determines whether a specified operation is applied to each roll value instead of total. </summary>
        /// <param name="op">The dice operation.</param>
        /// <returns> True if each roll is modified. </returns>
        public static bool IsEach(DiceOperation op) {
            return op == DiceOperation.EachPlus || op == DiceOperation.EachMinus;
        }

        /// <summary>Returns a <see cref="String" /> that represents a specified <see cref="DiceOperation"/>.</summary>
        /// <param name="diceOperation">The dice operation.</param>
        /// <returns> A <see cref="String" /> that represents a specified <see cref="DiceOperation"/>. </returns>
        public static string ToOpString(DiceOperation diceOperation) {
            Contract.Ensures(Contract.Result<string>() != null);

            switch (diceOperation) {
                case DiceOperation.Divide: return "/";
                case DiceOperation.EachPlus:
                case DiceOperation.Plus: return "+";
                case DiceOperation.EachMinus:
                case DiceOperation.Minus: return "-";
                case DiceOperation.Multiply: return "*";
                default: return String.Empty;
            }
        }

        /// <summary> Returns a <see cref="DiceOperation"/> that corresponds to a specified character. </summary>
        /// <param name="diceOperation">The dice operation.</param>
        /// <returns> A <see cref="DiceOperation"/> that corresponds to a specified character. </returns>
        public static DiceOperation FromOpChar(char diceOperation) {
            switch (diceOperation) {
                case '/':   return DiceOperation.Divide;
                case '+':   return DiceOperation.Plus;
                case '-':   return DiceOperation.Minus;
                case '*':   return DiceOperation.Multiply;
                default:    return DiceOperation.None;
            }
        }

        /// <summary> Returns a <see cref="DiceOperation"/> that corresponds to a character with a specified index in
        /// a given string. Takes into account 'each roll' modifiers. </summary>
        /// <returns> A <see cref="DiceOperation"/> that corresponds to a character with a specified index in a
        /// given string.  </returns>
        public static DiceOperation FromNodeString(string nodeRepr, int opIndex) {
            Contract.Requires(!String.IsNullOrWhiteSpace(nodeRepr));
            Contract.Requires(opIndex >= 0);
            Contract.Requires(opIndex < nodeRepr.Length);
            Contract.Ensures(Contract.Result<DiceOperation>() != DiceOperation.None);

            var diceOp = FromOpChar(nodeRepr[opIndex]);
            if (diceOp == DiceOperation.None) {
                throw new ArgumentException("Operation index was wrong or unknown operation character was at index.");
            }

            if (opIndex > 0) {
                string beforeOp = nodeRepr.Substring(0, opIndex).Trim();
                if (beforeOp.EndsWith("(")) {
                    if (diceOp == DiceOperation.Plus) {
                        diceOp = DiceOperation.EachPlus;
                    }
                    else if (diceOp == DiceOperation.Minus) {
                        diceOp = DiceOperation.EachMinus;
                    }
                }
            }

            return diceOp;
        }
    }
}