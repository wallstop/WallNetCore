using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallNetCore.Cache;
using WallNetCore.Cache.Advanced;
using WallNetCore.Random;

namespace WallNetCoreTest.Cache
{
    [TestClass]
    public class LocalManualCacheSpec
    {
        [TestMethod]
        public void ExpireAfterAccessRespectsTimeout()
        {
            TimeSpan accessExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(100, 150));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithExpireAfterAccess(accessExpiry).Build();

            string outValue;
            bool foundValue;

            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(ExpireAfterAccessRespectsTimeout);
            arbitraryCache.Put(key, value);
            int maxAttempts = ThreadLocalRandom.Current.Next(5, 200);
            for(int i = 0; i < maxAttempts; ++i)
            {
                foundValue = arbitraryCache.GetIfPresent(key, out outValue);
                Assert.IsTrue(foundValue);
                Assert.AreEqual(value, outValue);
                /* Sleep for a little bit */
                Thread.Sleep((int) (accessExpiry.TotalMilliseconds / 4));
            }
            /* Sleep for a lot */
            Thread.Sleep((int) (accessExpiry.TotalMilliseconds * 2));
            foundValue = arbitraryCache.GetIfPresent(key, out outValue);
            Assert.IsFalse(foundValue);
        }

        [TestMethod]
        public void ExpireAfterWriteRespectsTimeout()
        {
            TimeSpan writeExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(100, 150));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithExpireAfterWrite(writeExpiry).Build();

            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(ExpireAfterWriteRespectsTimeout);
            arbitraryCache.Put(key, value);
            string outValue;
            bool foundValue = arbitraryCache.GetIfPresent(key, out outValue);
            Assert.IsTrue(foundValue);
            Assert.AreEqual(value, outValue);
            Thread.Sleep((int) (writeExpiry.TotalMilliseconds * 2));

            foundValue = arbitraryCache.GetIfPresent(key, out outValue);
            Assert.IsFalse(foundValue);
        }

        [TestMethod]
        public void NoRemovalNotificationSameEntry()
        {
            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier =
                notification =>
                {
                    Assert.Fail("Removal notification was called when calling put with same key value pair");
                };

            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).Build();

            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(NoRemovalNotificationSameEntry);
            arbitraryCache.Put(key, value);
            arbitraryCache.Put(key, value);
            Assert.IsFalse(removalCalled);
        }

        [TestMethod]
        public void RemovalNotificationExpiredAccessTimeout()
        {
            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(RemovalNotificationExpiredAccessTimeout);

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.IsFalse(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Expired, notification.RemovalCause);
            };

            TimeSpan accessExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(1000, 1500));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithRemovalListener(removalNotifier)
                    .WithExpireAfterAccess(accessExpiry)
                    .Build();

            arbitraryCache.Put(key, value);
            string outValue;
            bool foundValue = arbitraryCache.GetIfPresent(key, out outValue);

            Assert.IsTrue(foundValue);
            Assert.AreEqual(value, outValue);
            Thread.Sleep((int) accessExpiry.TotalMilliseconds * 2);
            foundValue = arbitraryCache.GetIfPresent(key, out outValue);
            Assert.IsFalse(foundValue);
            /* Force a removal notification via a "modification" operation */
            arbitraryCache.Put(key, value + "_overridden");
            Assert.IsTrue(removalCalled);
        }

        [TestMethod]
        public void RemovalNotificationExpiredWriteTimeout()
        {
            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(RemovalNotificationExpiredWriteTimeout);

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.IsFalse(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Expired, notification.RemovalCause);
            };

            TimeSpan writeExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(1000, 1500));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithRemovalListener(removalNotifier)
                    .WithExpireAfterWrite(writeExpiry)
                    .Build();

            arbitraryCache.Put(key, value);
            Thread.Sleep((int) writeExpiry.TotalMilliseconds * 2);
            string outValue;
            bool foundValue = arbitraryCache.GetIfPresent(key, out outValue);
            Assert.IsFalse(foundValue);
            /* Force a removal notification via a "modification" operation */
            arbitraryCache.Put(key, value + "_overridden");
            Assert.IsTrue(removalCalled);
        }

        [TestMethod]
        public void RemovalNotificationExplicitInvalidate()
        {
            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(RemovalNotificationExplicitInvalidate);

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.IsFalse(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Explicit, notification.RemovalCause);
            };
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).Build();

            arbitraryCache.Put(key, value);
            arbitraryCache.Invalidate(key);
            Assert.IsTrue(removalCalled);
        }

        [TestMethod]
        public void RemovalNotificationReplacedEntry()
        {
            int key = ThreadLocalRandom.Current.Next();
            const string value = nameof(RemovalNotificationReplacedEntry);

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.IsFalse(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Replaced, notification.RemovalCause);
            };
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).Build();

            arbitraryCache.Put(key, value);

            const string differentValue = value + "_overridden";
            arbitraryCache.Put(key, differentValue);
            Assert.IsTrue(removalCalled);
        }
    }
}