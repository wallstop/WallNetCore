﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallNetCore.Cache.Advanced;
using WallNetCore.Random;

namespace WallNetCoreTest.Cache
{
    [TestClass]
    public class LocalManualCacheConcurrencySpec
    {
        private const int NumRuntimes = 100;
        private const int MaxThreads = 10000;

        [TestMethod]
        public void ConcurrentAccessDoesntLockUp()
        {
            Action testFunction = () =>
            {
                ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().Build();

                List<Task> readers = new List<Task>(MaxThreads);
                for(int i = 0; i < MaxThreads; ++i)
                {
                    int initialKey = i;
                    Task reader = new Task(() => cache.Get(initialKey, key => key.ToString()));
                    readers.Add(reader);
                    reader.Start();
                }
                readers.ForEach(reader => reader.Wait());
                for(int i = 0; i < MaxThreads; ++i)
                {
                    string foundValue;
                    bool found = cache.GetIfPresent(i, out foundValue);
                    Assert.IsTrue(found, "Didn't find " + i);
                    Assert.AreEqual(i.ToString(), foundValue);
                }
            };
            testFunction.RunMultipleTimes(NumRuntimes);
        }

        [TestMethod]
        public void NoDataRaceOnKeyCollision()
        {
            Action testFunction = () =>
            {
                ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().Build();
                int seed = ThreadLocalRandom.Current.Next(10, 500);
                int originalSeed = seed;
                int key = ThreadLocalRandom.Current.Next();

                List<Task> readers = new List<Task>(MaxThreads);
                for(int i = 0; i < MaxThreads; ++i)
                {
                    Task reader = new Task(() => cache.Get(key, _ => Interlocked.Add(ref seed, 1).ToString()));
                    readers.Add(reader);
                    reader.Start();
                }
                readers.ForEach(reader => reader.Wait());

                Assert.AreEqual(cache.Count, 1);
                string foundValue;
                bool found = cache.GetIfPresent(key, out foundValue);

                Assert.IsTrue(found);
                Assert.AreEqual(originalSeed + 1, seed);
                Assert.AreEqual(foundValue, (originalSeed + 1).ToString());
            };
            testFunction.RunMultipleTimes(NumRuntimes);
        }
    }
}