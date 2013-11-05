// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Natural20.Core {
    /// <summary> Serves as the <see cref="DiceChain"/> creator and guard for incomplete setups for 
    /// the fluent interface. </summary>
    [DataContract, KnownType("GetDieTypes")]
    public sealed class DiceChainSetup {
        /// <summary> Gets the amount of dice added to the created <see cref="DiceChain"/>. </summary>
        [DataMember]
        internal IDie Amount { get; private set; }

        /// <summary> Gets the previous <see cref="DiceChain"/> to carry on to next one. </summary>
        [DataMember]
        internal DiceChain PreviousDice { get; private set; }

        /// <summary> Gets he operation to use with previous <see cref="DiceChain"/>. </summary>
        [DataMember]
        internal DiceOperation PreviousOperation { get; private set; }
        
        /// <summary> Initializes a new instance of the <see cref="DiceChainSetup" /> struct. </summary>
        /// <param name="amountProvider">The amount of dice added to the created <see cref="DiceChain" />.</param>
        public DiceChainSetup(IDie amountProvider)
            : this(amountProvider, DiceOperation.Plus, null) {
            Contract.Requires(amountProvider != null);
        }

        /// <summary> Initializes a new instance of the <see cref="DiceChainSetup" /> struct. </summary>
        /// <param name="amountProvider">The amount of dice added to the created <see cref="DiceChain" />.</param>
        /// <param name="previousOperation">The operation to use with previous <see cref="DiceChain"/>.</param>
        /// <param name="previousDice">The previous <see cref="DiceChain"/> to carry on to next one.</param>
        public DiceChainSetup(IDie amountProvider, DiceOperation previousOperation,
            DiceChain previousDice) {
            Contract.Requires(previousOperation != DiceOperation.None);
            Contract.Requires(amountProvider != null);

            Amount = amountProvider;
            PreviousOperation = previousOperation;
            PreviousDice = previousDice;
        }

        /// <summary> Creates <see cref="DiceChain"/> used to hold the <see cref="Natural20.Die"/> 
        /// with given amount of faces. </summary>
        /// <param name="faces">The face count for the die.</param>
        public DiceChain D(int faces) {
            Contract.Requires(faces > 0);

            return AppendDie(new Die(faces));
        }

        /// <summary> Creates <see cref="DiceChain"/> used to hold the <see cref="Natural20.Die"/>
        /// representing [min, max] interval of integers. </summary>
        /// <param name="min">The inclusive lower bound of the interval.</param>
        /// <param name="max">The inclusive upper bound of the interval.</param>
        public DiceChain I(int min, int max) {
            Contract.Requires(min <= max);

            return AppendDie(new Die(min, max));
        }

        /// <summary> Creates <see cref="DiceChain"/> used to hold the <see cref="FixedDie"/>. </summary>
        /// <param name="value">The fixed value.</param>
        public DiceChain Fixed(double value) {
            Contract.Requires(!Double.IsNaN(value));

            return AppendDie(new FixedDie(value));
        }

        private DiceChain AppendDie(IDie die) {
            Contract.Requires(die != null);

            var result = PreviousDice;
            if (result != null) {
                if (PreviousOperation == DiceOperation.None) {
                    throw new InvalidOperationException("Can't append dice with unknown operation.");
                }

                result.Append(PreviousOperation, new SeveralDice(die, Amount));
            }
            else {
                result = new DiceChain(new SeveralDice(die, Amount));
            }

            return result;
        }

// ReSharper disable UnusedMember.Local
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Called by data contract serializer via [KnownType] attribute.")]
        private static IEnumerable<Type> GetDieTypes() {
            return Dice.GetAllDieTypes();
        }
    }
}