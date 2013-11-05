// This program is free software. It comes without any warranty, to the extent permitted by applicable law. 
// You can redistribute it and/or modify it under the terms of the Do What The Fuck You Want To Public License,
// Version 2, as published by Sam Hocevar. See the LICENSE file for more details.
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.Serialization;
using Natural20.Core;

namespace Natural20 {
    /// <summary> Can represent either a standard die with arbitrary amount of faces or a die rolling 
    /// within [min, max] interval of integers. </summary>
    [DataContract]
    public class Die : CustomDie {
        /// <summary> Initializes a new instance of the <see cref="Die"/> class. </summary>
        /// <param name="faces">The face count for a die.</param>
        public Die(int faces)
            : base(1, faces) {
            Contract.Requires(faces > 0);
        }

        /// <summary> Initializes a new instance of the <see cref="Die"/> class. </summary>
        /// <param name="min">The inclusive lower bound of the interval.</param>
        /// <param name="max">The inclusive upper bound of the interval.</param>
        public Die(int min, int max)
            : base(min, max) {
            Contract.Requires(min <= max);
        }

        /// <summary> Converts the string representation of a die to the actual die. </summary>
        /// <param name="representation">A string that contains a die to parse.</param>
        /// <returns> Parsed die. </returns>
        /// <exception cref="System.ArgumentException"> String is too short for a die.
        /// or
        /// Die face count is less than 1.
        /// or
        /// String is too short.
        /// or
        /// Interval minimum can't be greater than maximum.
        /// or
        /// String represents unknown die type.
        /// </exception>
        /// <exception cref="FormatException">Interval lacks a delimiter ','.</exception>
        public static Die Parse(string representation) {
            Contract.Ensures(Contract.Result<Die>() != null);
            
            Die result = null;
            if (!String.IsNullOrWhiteSpace(representation)) {
                string clean = representation.Trim();
                // It could be either a die or an interval, lets check for both:
                if (clean.StartsWith("d")) {    // die, "d[x]"
                    if (clean.Length < 2) { throw new ArgumentException("String is too short.", "representation"); }

                    int faces = Convert.ToInt32(clean.Substring(1), CultureInfo.InvariantCulture);
                    if (faces < 1) { throw new ArgumentException("Die face count is less than 1.", "representation"); }
                    result = new Die(faces);
                }
                else if (clean.StartsWith("[")) {   // interval, one of "r[x, y]", "r[-x, y]", "r[-x, -y]"
                    if (clean.Length < 5) { throw new ArgumentException("String is too short.", "representation"); }

                    int delimiter = clean.IndexOf(','); // Find interval delimiter.
                    if (delimiter > -1) {
                        // Parse interval minimum and maximum:
                        const int minStart = 1;
                        int maxStart = delimiter + 1;
                        int min = Convert.ToInt32(clean.Substring(minStart, delimiter - minStart).Trim(),
                            CultureInfo.InvariantCulture);
                        int max = Convert.ToInt32(clean.Substring(maxStart, clean.Length - 1 - maxStart).Trim(),
                            CultureInfo.InvariantCulture);
                        if (min > max) {
                            throw new ArgumentException("Interval minimum is greater than maximum.", "representation");
                        }

                        result = new Die(min, max);
                    }
                    else { throw new FormatException("Interval lacks a delimiter ','."); }
                }
                else { throw new ArgumentException("String represents unknown die type."); }
            }

            return result;
        }

        /// <summary> Converts the string representation of a die to the actual die. 
        /// A return value indicates whether the conversion succeeded or failed. </summary>
        /// <param name="representation"> A string that contains a die to parse. </param>
        /// <param name="die"> When this method returns, contains the parsed die equivalent to the given string, if the
        /// conversion succeeded, or null if the conversion failed. This parameter is passed uninitialized. </param>
        /// <returns>true if string was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string representation, out Die die) {
            // TODO: That's not a good style, better to do proper format validation someday.
            try {
                die = Parse(representation);
                return die != null;
            }
            catch {
                die = null;
                return false;
            }
        }

        protected override double MakeSingleRoll(Random random) {
            Contract.Ensures(!Double.IsInfinity(Contract.Result<double>()));
            Contract.Ensures(!Double.IsNaN(Contract.Result<double>()));
            
            int min = (int)Minimum; 
            int max = (int)Maximum;
            if (min > max) { throw new InvalidOperationException("Can't roll a custom die with Minimum > Maximum."); }

            return random.Next(min, max + 1);
        }
    }
}