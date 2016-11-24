using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallNetCore.Cache;
using WallNetCore.Cache.Advanced;
using WallNetCore.Random;

namespace WallNetCoreTest.Cache
{
    [TestClass]
    public class CacheBuilderSpec
    {
        [TestMethod]
        public void ExpireAfterAccessUnsetTurnsToDefault()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Assert.IsNull(cacheBuilder.ExpireAfterAccessMilliseconds);
        }

        [TestMethod]
        public void ExpireAfterWriteUnsetTurnsToDefault()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Assert.IsNull(cacheBuilder.ExpireAfterWriteMilliseconds);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidExpireAfterAccessThrows()
        {
            CacheBuilder<int, string>.NewBuilder().WithExpireAfterAccess(TimeSpan.FromSeconds(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidExpireAfterWriteThrows()
        {
            CacheBuilder<int, string>.NewBuilder().WithExpireAfterWrite(TimeSpan.FromSeconds(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidExpireNullRemovalListenerThrows()
        {
            CacheBuilder<int, string>.NewBuilder().WithRemovalListener(null);
        }

        [TestMethod]
        public void RemovalListenerSet()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Action<RemovalNotification<int, string>> notification = removalNotification => { };
            cacheBuilder.WithRemovalListener(notification);
            Assert.IsNotNull(cacheBuilder.RemovalListener);
            Assert.AreEqual(notification, cacheBuilder.RemovalListener);
        }

        [TestMethod]
        public void RemovalListenerUnsetIsNull()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Assert.IsNull(cacheBuilder.RemovalListener);
        }

        [TestMethod]
        public void TestExpireAfterAccessSet()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            TimeSpan expireAfterAccessSeconds = TimeSpan.FromSeconds(ThreadLocalRandom.Current.Next());
            cacheBuilder.WithExpireAfterAccess(expireAfterAccessSeconds);
            Assert.AreEqual((long) Math.Round(expireAfterAccessSeconds.TotalMilliseconds),
                cacheBuilder.ExpireAfterAccessMilliseconds);
        }

        [TestMethod]
        public void TestExpireAfterWriteSet()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            TimeSpan expireAfterWriteSeconds = TimeSpan.FromSeconds(ThreadLocalRandom.Current.Next());
            cacheBuilder.WithExpireAfterAccess(expireAfterWriteSeconds);
            Assert.AreEqual((long) Math.Round(expireAfterWriteSeconds.TotalMilliseconds),
                cacheBuilder.ExpireAfterAccessMilliseconds);
        }
    }
}