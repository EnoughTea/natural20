using MathNet.Numerics.Statistics;
using NUnit.Framework;

namespace Natural20Tests {
    /// <summary> Helper methods for assertion. </summary>
    internal static class AssertTools {
        /// <summary> Determines whether a specified values are distributed in a 'uniformish' way: 
        /// mean value is as expected, distribution is platykurtic and not skewed. </summary>
        /// <param name="randomValues">The random values.</param>
        /// <param name="minValue">The minimum possible value.</param>
        /// <param name="maxValue">The maximum possible value.</param>
        public static void IsUniformishDistribution(double[] randomValues, double minValue, double maxValue) {
            var statistics = new DescriptiveStatistics(randomValues);
            double meanMargin = (maxValue < 1) ? maxValue / 5 : 1;

            Assert.AreEqual((minValue + maxValue) / 2, statistics.Mean, meanMargin, 
                "Expected mean differs to much from actual mean");
            Assert.AreEqual(minValue, statistics.Minimum, "Expected min differs from actual min");
            Assert.AreEqual(maxValue, statistics.Maximum, "Expected max differs from actual max");
            Assert.AreEqual(0, statistics.Skewness, 0.1, "Distribution is skewed too much");
            Assert.Less(statistics.Kurtosis, 0, "Distribution is not platykurtic at all");
        }
    }
}
