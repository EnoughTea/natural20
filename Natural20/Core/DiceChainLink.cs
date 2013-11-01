// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;

namespace Natural20.Core {
    /// <summary> Repesents a link in the dice chain used to tie two dice nodes together via 
    /// <see cref="DiceOperation"/>. </summary>
    [DataContract, KnownType("GetDieTypes")]
    public sealed class DiceChainLink : IDie {
        private DiceOperation operation;
        private SeveralDice node;

        /// <summary> Gets the maximum possible die value. </summary>
        public double Maximum {
            get {
                switch (Operation) {
                    case DiceOperation.Plus: return GetPreviousMax() + GetNodeMax();
                    case DiceOperation.Minus: return GetPreviousMax() - GetNodeMin();
                    case DiceOperation.Divide: return (Node != null) ? GetPreviousMax() / GetNodeMin() : 0;
                    case DiceOperation.Multiply: return GetPreviousMax() * GetNodeMax();
                    case DiceOperation.EachPlus: return GetPreviousMax() + GetPreviousMaxCount() * GetNodeMax();
                    case DiceOperation.EachMinus: return GetPreviousMax() - GetPreviousMaxCount() * GetNodeMin();
                    default: throw new InvalidOperationException("Unknown dice operation.");
                }
            }
        }

        /// <summary> Gets the minimum possible die value. </summary>
        public double Minimum {
            get {
                switch (Operation) {
                    case DiceOperation.Plus: return GetPreviousMin() + GetNodeMin();
                    case DiceOperation.Minus: return GetPreviousMin() - GetNodeMax();
                    case DiceOperation.Divide: return (Node != null) ? GetPreviousMin() / GetNodeMax() : 0;
                    case DiceOperation.Multiply: return GetPreviousMin() * GetNodeMin();
                    case DiceOperation.EachPlus: return GetPreviousMin() + GetPreviousMinCount() * GetNodeMin();
                    case DiceOperation.EachMinus: return GetPreviousMin() - GetPreviousMinCount() * GetNodeMax();
                    default: throw new InvalidOperationException("Unknown dice operation.");
                }
            }
        }

        /// <summary> Gets the current chain node. </summary>
        [DataMember]
        public SeveralDice Node {
            get { return node; }
            set {
                Contract.Requires(value != null);
                node = value;
            }
        }

        /// <summary> Gets the dice operation to perform with current link. <see cref="DiceOperation.Plus"/> for a 
        /// first link. </summary>
        [DataMember]
        public DiceOperation Operation {
            get { return operation; }
            set {
                Contract.Requires(value != DiceOperation.None);
                operation = value;
            }
        }

        /// <summary> Gets the link to previous node in the dice chain. Null for a first link. </summary>
        [DataMember]
        public DiceChainLink Previous { get; private set; }

        /// <summary> Initializes a new instance of the <see cref="DiceChainLink" /> class. </summary>
        /// <param name="operation">The dice operation which will be used with a dice chain node.</param>
        /// <param name="node">This dice chain node.</param>
        internal DiceChainLink(DiceOperation operation, SeveralDice node)
            : this(operation, node, null) {
            Contract.Requires(operation != DiceOperation.None);
            Contract.Requires(node != null);
        }

        /// <summary> Initializes a new instance of the <see cref="DiceChainLink" /> class. </summary>
        /// <param name="operation">The dice operation which will be used with a dice chain node.</param>
        /// <param name="node">This dice chain node.</param>
        /// <param name="previous">The link to previous node in the dice chain.</param>
        internal DiceChainLink(DiceOperation operation, SeveralDice node, DiceChainLink previous) {
            Contract.Requires(operation != DiceOperation.None);
            Contract.Requires(node != null);

            Operation = operation;
            Node = node;
            Previous = previous;
        }

        /// <summary> Performs a single roll and returns its value. </summary>
        /// <returns> Roll value. </returns>
        public double Roll() {
            int rollCount;
            return Roll(out rollCount);
        }

        /// <summary> Performs a specified amount of rolls and returns their values. </summary>
        /// <param name="count"> The roll count. </param>
        /// <returns> Roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        public IEnumerable<double> Roll(int count) {
            for (int i = 0; i < count; i++) {
                int rollCount;
                yield return Roll(out rollCount);
            }
        }

        /// <summary> Returns a <see cref="String"/> that represents this instance. </summary>
        /// <returns> A <see cref="String"/> that represents this instance. </returns>
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            bool isEach = DiceOperationTools.IsEach(Operation);
            string eachBegin = isEach ? "(" : String.Empty;
            string eachEnd = isEach ? ")" : String.Empty;
            string previous = (Previous != null)
                ? Previous + eachBegin + DiceOperationTools.ToOpString(Operation)
                : String.Empty;
            return previous + Node + eachEnd;
        }

        /// <summary> Rolls the dice chain once up to this link accumulating step-by-step data.
        /// Use this when you need to know roll values besides their totals. </summary>
        /// <remarks>Compared to usual <see cref="Roll()"/> methods, this one is not as efficient, and it allocates an 
        /// array per dice chain node.</remarks>
        /// <param name="steps">Roll values of each step of the dice chain.</param>
        /// <returns>Value of a single roll.</returns>
        internal double RollStepByStep(out List<DiceChainRollStep> steps) {
            steps = new List<DiceChainRollStep>();
            // First let's gather roll entire dice chain step by step and save results into steps:
            var traversal = this;
            while (traversal != null) {
                steps.Add(new DiceChainRollStep(traversal.Node.RollSeparately().ToArray(), traversal.Operation));
                traversal = traversal.Previous;
            }

            steps.Reverse();
            double result = 0;
            for (int i = 0; i < steps.Count; i++) {
                var previousStepCount = (i > 0) ? steps[i - 1].Rolls.Length : 1;
                var stepValue = steps[i].Total;
                result = Rolls.PerformRollCombination(result, steps[i].Operation, stepValue, previousStepCount);
            }

            return result;
        }

        /// <summary> Performs a single roll and returns its value. </summary>
        /// <param name="rollCount">The roll count for current node.</param>
        /// <returns> Roll value. </returns>
        private double Roll(out int rollCount) {
            var previousRoll = GetPreviousRoll();
            var nodeRoll = GetNodeRoll();
            rollCount = nodeRoll.Item2;
            return Rolls.PerformRollCombination(previousRoll.Item1, Operation, nodeRoll.Item1, previousRoll.Item2);
        }

        private double GetPreviousMin() {
            return (Previous != null) ? Previous.Minimum : 0;
        }

        private double GetPreviousMinCount() {
            return (Previous != null) ? Previous.Node.MinimumCount : 0;
        }

        private double GetPreviousMaxCount() {
            return (Previous != null) ? Previous.Node.MaximumCount : 0;
        }

        private double GetPreviousMax() {
            return (Previous != null) ? Previous.Maximum : 0;
        }

        private double GetNodeMin() {
            return (Node != null) ? Node.Minimum : 0;
        }

        private double GetNodeMax() {
            return (Node != null) ? Node.Maximum : 0;
        }

        /// <summary> Get results of the previous chain rolls. </summary>
        /// <returns>(rollValue, rollCount)</returns>
        private Tuple<double, int> GetPreviousRoll() {
            int rollCount = 1;
            double rollValue = (Previous != null) ? Previous.Roll(out rollCount) : 0;
            return Tuple.Create(rollValue, rollCount);
        }

        /// <summary> Get results of this node roll. </summary>
        /// <returns>(rollValue, rollCount)</returns>
        private Tuple<double, int> GetNodeRoll() {
            int rollCount = 1;
            double rollValue = (Node != null) ? Node.Roll(out rollCount) : 0;
            return Tuple.Create(rollValue, rollCount);
        }

// ReSharper disable UnusedMember.Local
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Called by data contract serializer via [KnownType] attribute.")]
        private static IEnumerable<Type> GetDieTypes() {
            return Dice.GetAllDieTypes();
        }
    }
}