using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallNetCore.DataStructure;
using WallNetCore.Random;

namespace WallNetCoreTest.DataStructure
{
    [TestClass]
    public class CircularBufferSpec
    {
        [TestMethod]
        public void BuffersSimply()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            for(int i = 0; i < capacity; ++i)
            {
                intBuffer.Add(i);
                Assert.AreEqual(i + 1, intBuffer.Count);
                for(int j = 0; j <= i; ++j)
                {
                    Assert.AreEqual(i - j, intBuffer[j]);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void OutOfBoundsElements()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            int elementsBuffered = ThreadLocalRandom.Current.Next(capacity - 1);
            for(int i = 0; i < elementsBuffered; ++i)
            {
                int element = ThreadLocalRandom.Current.Next();
                intBuffer.Add(element);
            }
            int shouldThrow = intBuffer[capacity - 1];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void OutOfBoundsNoElements()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            int shouldThrow = intBuffer[0];
        }

        [TestMethod]
        public void OverflowHandled()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            for(int i = 0; i < capacity * 4; ++i)
            {
                intBuffer.Add(i);
                Assert.AreEqual(Math.Min(i + 1, capacity), intBuffer.Count);
                for(int j = 0; j <= i; ++j)
                {
                    Assert.AreEqual((i - j) % capacity, intBuffer[j % capacity] % capacity);
                }
            }
        }

        [TestMethod]
        public void Peek()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            int retrieved;
            bool exists = intBuffer.Peek(out retrieved);
            Assert.IsFalse(exists);

            for(int i = 0; i < capacity * 10; ++i)
            {
                int value = ThreadLocalRandom.Current.Next();
                intBuffer.Add(value);

                exists = intBuffer.Peek(out retrieved);
                Assert.IsTrue(exists);
                Assert.AreEqual(value, retrieved);
            }
        }
    }
}