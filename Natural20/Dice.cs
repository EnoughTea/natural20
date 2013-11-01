// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Natural20.Core;

namespace Natural20 {
    /// <summary> Serves as a root for a fluent interface and holds convenient shortcuts for commonly used dice.
    /// </summary>
    public static class Dice {
        /// <summary> Gets or sets the random number generator used for dice rolls. It uses ThreadLocal under the hood,
        /// so instances of Random in getter/setter differ between threads. </summary>
        public static Random Random { get { return threadRandom.Value; } set { threadRandom.Value = value; } }

        /// <summary>Gets or sets the convenient d2 (coin) reference. </summary>
        public static Die D2 {
            get { return d2; }
            set {
                Contract.Requires(value != null);
                d2 = value;
            }
        }

        /// <summary>Gets or sets the convenient d3 reference. </summary>
        public static Die D3 {
            get { return d3; }
            set {
                Contract.Requires(value != null);
                d3 = value;
            }
        }

        /// <summary>Gets or sets the convenient d4 reference. </summary>
        public static Die D4 {
            get { return d4; }
            set {
                Contract.Requires(value != null);
                d4 = value;
            }
        }

        /// <summary>Gets or sets the convenient d6 reference. </summary>
        public static Die D6 {
            get { return d6; }
            set {
                Contract.Requires(value != null);
                d6 = value;
            }
        }

        /// <summary>Gets or sets the convenient d8 reference. </summary>
        public static Die D8 {
            get { return d8; }
            set {
                Contract.Requires(value != null);
                d8 = value;
            }
        }

        /// <summary>Gets or sets the convenient d10 reference. </summary>
        public static Die D10 {
            get { return d10; }
            set {
                Contract.Requires(value != null);
                d10 = value;
            }
        }

        /// <summary>Gets or sets the convenient d12 reference. </summary>
        public static Die D12 {
            get { return d12; }
            set {
                Contract.Requires(value != null);
                d12 = value;
            }
        }

        /// <summary>Gets or sets the convenient d20 reference. </summary>
        public static Die D20 {
            get { return d20; }
            set {
                Contract.Requires(value != null);
                d20 = value;
            }
        }

        /// <summary>Gets or sets the convenient d100 reference. </summary>
        public static Die D100 {
            get { return d100; }
            set {
                Contract.Requires(value != null);
                d100 = value;
            }
        }

        /// <summary> Takes a specified amount of untyped dice from the box. </summary>
        /// <param name="amount">The amount of dice to take.</param>
        public static DiceChainSetup Take(int amount) {
            Contract.Requires(amount > 0);
            return Take(new FixedDie(amount));
        }

        /// <summary> Takes a specified amount of untyped dice from the box. </summary>
        /// <param name="amountProvider">The provider of an amount of dice to take.</param>
        public static DiceChainSetup Take(IDie amountProvider) {
            Contract.Requires(amountProvider != null);
            return new DiceChainSetup(amountProvider);
        }

        /// <summary> Converts the string representation of a dice chain to an actual dice chain. </summary>
        /// <param name="representation">A string that contains a dice chain to parse.</param>
        /// <returns> Parsed dice chain or null, if string does not represent a known die type. </returns>
        /// <exception cref="InvalidOperationException">Can't parse node.</exception>
        public static DiceChain Parse(string representation) {
            Contract.Requires(!String.IsNullOrWhiteSpace(representation));

            string cleaned = representation.Trim();
            int opIndex = cleaned.IndexOfAny(diceOps);
            if (opIndex < 0) {
                var singleNode = ToNode(cleaned);
                if (singleNode == null) { throw new InvalidOperationException("Can't parse node " + cleaned); }
                return new DiceChain(singleNode);
            }

            string firstPart = cleaned.Substring(0, opIndex);
            firstPart = firstPart.Remove("(").Remove(")");
            var firstNode = ToNode(firstPart);
            if (firstNode == null) { throw new InvalidOperationException("Can't parse node " + firstPart); }

            var result = new DiceChain(firstNode);
            while (opIndex > -1) {
                // Determine whether there is a next dice chain node or not:
                int nextOpIndex = cleaned.IndexOfAny(diceOps, opIndex + 1);
                string nodePart = (nextOpIndex > -1)    // If there is a nest dice chain node, extract text before it.
                    ? cleaned.Substring(opIndex + 1, nextOpIndex - opIndex - 1)
                    : cleaned.Substring(opIndex + 1);   // Otherwise just get all remaining text.
                nodePart = nodePart.Remove("(").Remove(")");
                var node = ToNode(nodePart);
                if (node == null) { throw new InvalidOperationException("Can't parse node " + nodePart); }

                result.Append(DiceOperationTools.FromNodeString(cleaned, opIndex), node);
                opIndex = nextOpIndex;
            }

            return result;
        }

        /// <summary> Adds a custom node parser to use in <see cref="Parse"/>. Parser is a function that tries to parse
        /// a string representation of a part of dice chain like 'd104' or 'd2[10-25]' into a chain node. 
        /// If a parser can't parse a given string, it should return null. It should not throw. </summary>
        /// <remarks>Nodes are being separated by dice operations, so a string like "d10 + [5-10] * 3" will yield
        /// 3 nodes to parse: "d10", "[5-10]", "3".</remarks>
        /// <param name="parser">The custom parser.</param>
        public static void AddNodeParser(Func<string, SeveralDice> parser) {
            Contract.Requires(parser != null);

            if (!nodeParsers.Contains(parser)) { nodeParsers.Add(parser); }
        }

        /// <summary> Removes previously added custom parser. Parser is a function that tries to parse a string 
        /// representation of a part of dice chain like 'd104' or 'd2[10-25]' into a chain node. If a parser can't 
        /// parse a given string, it should return null.</summary>
        public static bool RemoveNodeParser(Func<string, SeveralDice> parser) {
            return nodeParsers.Remove(parser);
        }

        /// <summary> Gets all non-abstract types in a current appdomain which implement IDie interface. </summary>
        /// <returns> All non-abstract types in a current appdomain which implement IDie interface. </returns>
        internal static IEnumerable<Type> GetAllDieTypes() {
            return typeof(IDie).GetAssignableTypes().Where(type => !type.IsAbstract);
        }

        /// <summary> Converts string representation of a single node in a dice chain to an actual node. </summary>
        private static SeveralDice ToNode(string nodeRepresentation) {
            SeveralDice result = null;

            foreach (var parser in nodeParsers) {    // Parsers should not throw, but let's enforce the rule.
                try { result = parser(nodeRepresentation); }
                catch { result = null; }

                if (result != null) { break; }
            }

            return result;
        }

        private static SeveralDice ParseBuiltinDice(string nodeRepresentation) {
            SeveralDice result = null;
            double fixedValue;
            // Try to match the simplest fixed die first:
            if (Double.TryParse(nodeRepresentation, out fixedValue)) {
                result = new SeveralDice(new FixedDie(fixedValue));
            }
            else {  // Then try to match other known die types:
                foreach (var nodePattern in knownNodePatterns) {
                    var dieMatches = Regex.Matches(nodeRepresentation, nodePattern, RegexOptions.Compiled);
                    result = MatchToBuiltinDiceNode(dieMatches);
                    if (result != null) { break; }  // Stop trying on successful match.
                }
            }

            return result;
        }

        /// <summary> If there is just 1 match in the collection, it will be a value provider. 
        /// If there are 2 matches, first will be a die count provider and second will be a value provider. </summary>
        private static SeveralDice MatchToBuiltinDiceNode(MatchCollection dieMatches) {
            Contract.Requires(dieMatches != null);
            Die value = null;
            Die count = null;
            if (dieMatches.Count > 0) {
                // For built-in dice there is always a single match consisting of 3 groups:
                var match = dieMatches[0];
                if (match.Groups.Count > 2) {
                    string countRepr = match.Groups[1].Value; // Second group is a 'count' group. It's optional.
                    string valueRepr = match.Groups[2].Value; // Third group is a 'value' group. It's always present.

                    Die.TryParse(valueRepr, out value);
                    if (!String.IsNullOrEmpty(countRepr)) {
                        double countValue; // 'count' can be either a double or a built-in die.
                        if (!Double.TryParse(countRepr, out countValue)) {
                            Die.TryParse(countRepr, out count);
                        }
                    }
                }
            }

            return (value != null) ? new SeveralDice(value, count) : null;
        }


        private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(() => {
            lock (globalLock) {
                // The seed is derived from a global (static) instance of Random rather than time.
                return new Random(globalRandom.Next());
            }  
        });


        /// <summary> Random number generator used to generate seeds, which are then used to create new random number
        /// generators on a per-thread basis. </summary>
        private static readonly Random globalRandom = new Random();
        private static readonly object globalLock = new object();

        // Convenient dice shortcuts:
        private static Die d2 = new Die(2);
        private static Die d3 = new Die(3);
        private static Die d4 = new Die(4);
        private static Die d6 = new Die(6);
        private static Die d8 = new Die(8);
        private static Die d10 = new Die(10);
        private static Die d12 = new Die(12);
        private static Die d20 = new Die(20);
        private static Die d100 = new Die(100);

        // Parsing stuff below:
        /// <summary> Possible dice operation symbols used as node delimeters. </summary>
        private static readonly char[] diceOps = new[] { '+', '-', '*', '/' };

        /// <summary> Represents node containing cubic die like d50 and optional 'count' part. </summary>
        private const string DieNodePattern = CountDiePattern + "(?<die>d\\d+)$";

        /// <summary> Represents node containing interval die like [2, 8] and optional 'count' part. </summary>
        private const string IntervalNodePattern = CountDiePattern + "(?<interval>" + IntervalPattern + ")$";

        /// <summary> Represents optional 'count' part of the node. </summary>
        private const string CountDiePattern = @"(?<count>(?:d?\d+)|(?:" + IntervalPattern + "))?";
        
        /// <summary> Common part for matching [x, y]. </summary>
        private const string IntervalPattern = @"\[\d+,\s?\d+\]";

        private static readonly List<Func<string, SeveralDice>> nodeParsers =
            new List<Func<string, SeveralDice>> { ParseBuiltinDice };

        private static readonly string[] knownNodePatterns = new[] { DieNodePattern, IntervalNodePattern };
    }
}
