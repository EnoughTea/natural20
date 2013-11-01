// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;

namespace Natural20 {
    /// <summary> Used to represent a part of a break-up of a dice chain rolls. </summary>
    [DataContract]
    public sealed class DiceChainRollStep {
        /// <summary> Gets the roll values which were rolled in this part of a dice chain. </summary>
        /// <remarks> It's a roll values for one step, not the results of entire dice chain up to this step.</remarks>
        [DataMember]
        public double[] Rolls { get; private set; }

        /// <summary> Gets the sum of all values in <see cref="Rolls"/>. </summary>
        [DataMember]
        public double Total { get; private set; }

        /// <summary> Gets operation defining how these roll values affect previous part of a dice chain. </summary>
        [DataMember]
        public DiceOperation Operation { get; private set; }

        internal DiceChainRollStep(double[] rolls, DiceOperation operationToUseWithPreviousStep) {
            Contract.Requires(rolls != null);
            Contract.Requires(operationToUseWithPreviousStep != DiceOperation.None);

            Rolls = rolls;
            Total = rolls.Sum();
            Operation = operationToUseWithPreviousStep;
        }

        /// <summary> Returns a <see cref="string" /> that represents this instance. </summary>
        /// <returns> A <see cref="string" /> that represents this instance. </returns>
        public override string ToString() {
            return Rolls.Length + " roll(s) totaling " + Total.ToShortString();
        }
    }
}