using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Natural20;

namespace Natural20Tests {
    [TestFixture]
    public class RandomExtensionsTests {
        private readonly Random rng = new Random();
        private const int TotalItems = 10000;   // Amount of random picks for each test.
        private const int MinValue = 1;
        private const int MaxValue = 100;
        // List of values [MinValue, MaxValue] which will be used to pick random values from:
        private readonly List<int> values = Enumerable.Range(MinValue, MaxValue - MinValue + 1).ToList();

        [Test]
        public void NextItemFromListShouldHaveGoodDistribution() {
            double[] pickedValues = new double[TotalItems];
            for (int i = 0; i < TotalItems; i++) {
                pickedValues[i] = rng.NextItem(values);
            }

            AssertTools.IsUniformishDistribution(pickedValues, MinValue, MaxValue);
        }
        
        [Test]
        public void NextItemFromCollectionShouldHaveGoodDistribution() {
            double[] pickedValues = new double[TotalItems];
            for (int i = 0; i < TotalItems; i++) {
                pickedValues[i] = rng.NextItem((ICollection<int>)values);   // Cast so a needed overload will be chosen
            }

            AssertTools.IsUniformishDistribution(pickedValues, MinValue, MaxValue);
        }

        [Test]
        public void NextSubsetShouldHaveGoodDistribution() {
            const int subsetSize = 50;
            double[] pickedValues = new double[TotalItems];
            // Make lots of subsets:
            for (int subsetStart = 0; subsetStart < TotalItems; subsetStart += subsetSize) { 
                var subset = rng.NextSubset(values, subsetSize).ToList();
                // And gather values from each subset:
                for (int i = 0; i < subset.Count; i++) { 
                    pickedValues[subsetStart + i] = subset[i];   
                }
            }

            AssertTools.IsUniformishDistribution(pickedValues, MinValue, MaxValue);
        }

        [Test]
        public void NextsShouldHaveGoodDistribution() {
            double[] pickedValues = new double[TotalItems];
            for (int i = 0; i < TotalItems; i++) {
                pickedValues[i] = rng.NextSeveral(1, MinValue, MaxValue + 1);
            }

            AssertTools.IsUniformishDistribution(pickedValues, MinValue, MaxValue);
        }

        [Test]
        public void OneInShouldHaveGoodDistribution() {
            double[] pickedValues = new double[TotalItems];
            for (int i = 0; i < TotalItems; i++) {  // Simulate coin tosses:
                pickedValues[i] = rng.OneIn(2) ? 1 : 0;
            }

            AssertTools.IsUniformishDistribution(pickedValues, 0, 1);
        }

        [Test]
        public void RollShouldHaveGoodDistribution() {
            double[] pickedValues = new double[TotalItems];
            for (int i = 0; i < TotalItems; i++) {  // Simulate standard cubic dice rolls:
                pickedValues[i] = rng.Roll(1, 6);
            }

            AssertTools.IsUniformishDistribution(pickedValues, 1, 6);
        }

        [Test]
        public void ShuffleShouldHaveGoodDistribution() {
            double[] pickedValues = new double[TotalItems];
            for (int i = 0; i < TotalItems; i++) {  // Simulate standard cubic dice rolls:
                pickedValues[i] = rng.Roll(1, 6);
            }

            AssertTools.IsUniformishDistribution(pickedValues, 1, 6);
        }
    }
}
