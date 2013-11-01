// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Natural20 {
    /// <summary> Represents a dice which always return the same value. </summary>
    [DataContract]
    public struct FixedDie : IEquatable<FixedDie>, IDie {
        [DataMember]
        private readonly double roll;

        /// <summary> Gets the minimum possible die value. </summary>
        public double Minimum { get { return roll; } }

        /// <summary> Gets the maximum possible die value. </summary>
        public double Maximum { get { return roll; } }

        /// <summary>Gets or sets the minus one die (always returns -1). </summary>
        public static FixedDie MinusOne { get { return new FixedDie(-1); } }

        /// <summary>Gets or sets the zero die (always returns 0). </summary>
        public static FixedDie Zero { get { return new FixedDie(); } }

        /// <summary>Gets or sets the one die (always returns 1). </summary>
        public static FixedDie One { get { return new FixedDie(1); } }

        /// <summary> Initializes a new instance of the <see cref="Die"/> class. </summary>
        /// <param name="value">The fixed value.</param>
        public FixedDie(double value) : this() {
            Contract.Requires(!Double.IsNaN(value));
            
            roll = value;
        }

        /// <summary> Returns fixed value. </summary>
        /// <returns> Fixed value. </returns>
        public double Roll() {
            return roll;
        }

        /// <summary> Returns a specified amount of fixed values. </summary>
        /// <param name="count"> The roll count. </param>
        /// <returns> Fixed roll values. </returns>
        /// <remarks> Business as usual with generated enumerations: keep in mind that it will regenerate roll values
        /// every time you enumerate, so call something like '.ToArray()' when you want to keep previous values.
        /// </remarks>
        public IEnumerable<double> Roll(int count) {
            for (int i = 0; i < count; i++) {
                yield return roll;
            }
        }

        /// <summary>  Indicates whether the current object is equal to another object of the same type. </summary>
        /// <returns> true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(FixedDie other) { return roll.Equals(other.roll); }

        /// <summary> Indicates whether this instance and a specified object are equal. </summary>
        /// <returns> true if <paramref name="obj"/> and this instance are the same type and represent the same value;
        /// otherwise, false. </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is FixedDie && Equals((FixedDie)obj);
        }

        /// <summary> Returns the hash code for this instance. </summary>
        /// <returns> A 32-bit signed integer that is the hash code for this instance. </returns>
        public override int GetHashCode() {
            return roll.GetHashCode();
        }

        /// <summary> Returns a <see cref="String" /> that represents this instance. </summary>
        /// <returns> A <see cref="String" /> that represents this instance. </returns>
        public override string ToString() {
            return roll.ToShortString();
        }
        
        public static bool operator ==(FixedDie left, FixedDie right) {
            return left.Equals(right);
        }

        public static bool operator !=(FixedDie left, FixedDie right) {
            return !left.Equals(right);
        }
    }
}
