using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallNetCore.Random;
using WallNetCore.Validate;

namespace WallNetCoreTest.Validate
{
    [TestClass]
    public class ValidatorSpec
    {
        private const string TestMessage = nameof(ValidatorSpec) + " failed";

        private static readonly Validator DoNothingValidator = new Validator(_ => { });

        [TestMethod]
        public void IsElementOf()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);

            List<int> elements = Enumerable.Range(0, capacity).Select(_ => ThreadLocalRandom.Current.Next()).ToList();

            foreach(int element in elements)
            {
                bool validated = DoNothingValidator.IsElementOf(elements, element);
                Assert.IsTrue(validated);
            }
            for(int i = 0; i < capacity; ++i)
            {
                int generatedElement;
                do
                {
                    generatedElement = ThreadLocalRandom.Current.Next();
                } while(elements.Contains(generatedElement));
                bool validated = DoNothingValidator.IsElementOf(elements, generatedElement);
                Assert.IsFalse(validated);
            }
        }

        [TestMethod]
        public void IsFalse()
        {
            bool validated = DoNothingValidator.IsFalse(false);
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void IsFalseWithString()
        {
            bool validated = DoNothingValidator.IsFalse(false, TestMessage);
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void IsFalseWithStringSupplier()
        {
            bool validated = DoNothingValidator.IsFalse(false, TestMessageSupplier);
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void IsInClosedInterval()
        {
            const int min = -100;
            const int max = 50;
            bool validated = DoNothingValidator.IsInClosedInterval(min, min, max);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.IsInClosedInterval(max, min, max);
            Assert.IsTrue(validated);
            for(int i = min; i <= max; ++i)
            {
                validated = DoNothingValidator.IsInClosedInterval(i, min, max);
                Assert.IsTrue(validated);
            }

            validated = DoNothingValidator.IsInClosedInterval(min - 1, min, max);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsInClosedInterval(max + 1, min, max);
            Assert.IsFalse(validated);
        }

        [TestMethod]
        public void IsInOpenInterval()
        {
            const int min = -400;
            const int max = -244;
            bool validated = DoNothingValidator.IsInOpenInterval(min, min, max);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsInOpenInterval(max, min, max);
            Assert.IsFalse(validated);
            for(int i = min + 1; i < max; ++i)
            {
                validated = DoNothingValidator.IsInOpenInterval(i, min, max);
                Assert.IsTrue(validated);
            }
        }

        [TestMethod]
        public void IsNegative()
        {
            bool validated = DoNothingValidator.IsNegative(0.1);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsNegative(1);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsNegative(int.MaxValue);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsNegative(0.0);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsNegative(-0.0);
            Assert.IsFalse(validated);
            for(int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(0, int.MaxValue);
                validated = DoNothingValidator.IsNegative(value);
                Assert.IsFalse(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(0, double.MaxValue);
                validated = DoNothingValidator.IsNegative(value);
                Assert.IsFalse(validated);
            }

            validated = DoNothingValidator.IsNegative(-0.1);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.IsNegative(-0.00000001);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.IsNegative(-1111.4);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.IsNegative(int.MinValue);
            Assert.IsTrue(validated);

            for(int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(int.MinValue, -1);
                validated = DoNothingValidator.IsNegative(value);
                Assert.IsTrue(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(double.MinValue, -0);
                validated = DoNothingValidator.IsNegative(value);
                Assert.IsTrue(validated);
            }
        }

        [TestMethod]
        public void IsNotElementOf()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);

            List<int> elements = Enumerable.Range(0, capacity).Select(_ => ThreadLocalRandom.Current.Next()).ToList();

            foreach(int element in elements)
            {
                bool validated = DoNothingValidator.IsNotElementOf(elements, element);
                Assert.IsFalse(validated);
            }
            for(int i = 0; i < capacity; ++i)
            {
                int generatedElement;
                do
                {
                    generatedElement = ThreadLocalRandom.Current.Next();
                } while(elements.Contains(generatedElement));
                bool validated = DoNothingValidator.IsNotElementOf(elements, generatedElement);
                Assert.IsTrue(validated);
            }
        }

        [TestMethod]
        public void IsPositive()
        {
            bool validated = DoNothingValidator.IsPositive(0.1);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.IsPositive(1);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.IsPositive(int.MaxValue);
            Assert.IsTrue(validated);
            for(int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(1, int.MaxValue);
                validated = DoNothingValidator.IsPositive(value);
                Assert.IsTrue(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(0.0000001, double.MaxValue);
                validated = DoNothingValidator.IsPositive(value);
                Assert.IsTrue(validated);
            }

            validated = DoNothingValidator.IsPositive(-0.1);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsPositive(0.0);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsPositive(-0.0);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsPositive(-1111.4);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.IsPositive(int.MinValue);
            Assert.IsFalse(validated);

            for(int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(int.MinValue, 0);
                validated = DoNothingValidator.IsPositive(value);
                Assert.IsFalse(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(double.MinValue, 0);
                validated = DoNothingValidator.IsPositive(value);
                Assert.IsFalse(validated);
            }
        }

        [TestMethod]
        public void IsTrue()
        {
            bool validated = DoNothingValidator.IsTrue(true);
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void IsTrueWithString()
        {
            bool validated = DoNothingValidator.IsTrue(true, TestMessage);
            Assert.IsTrue(validated);
        }

        [TestMethod]
        public void IsTrueWithStringSupplier()
        {
            bool validated = DoNothingValidator.IsTrue(true, TestMessageSupplier);
            Assert.IsTrue(validated);
        }

        private static string TestMessageSupplier()
        {
            return TestMessage;
        }
    }
}