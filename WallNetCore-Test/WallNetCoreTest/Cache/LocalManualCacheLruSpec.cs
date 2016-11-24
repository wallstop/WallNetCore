using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallNetCore.Cache;
using WallNetCore.Cache.Advanced;

namespace WallNetCoreTest.Cache
{
    [TestClass]
    public class LocalManualCacheLruSpec
    {
        [TestMethod]
        public void LruCacheCanCacheElementsAfterFullInvalidation()
        {
            const int maxElements = 100;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string valueBase = nameof(LruCacheCanCacheElementsAfterFullInvalidation);

            for(int i = 0; i < maxElements; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);
            }

            cache.InvalidateAll();

            for(int i = 0; i < maxElements * 10; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);

                string retrievedValue;
                bool success = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsTrue(success);
                Assert.AreSame(value, retrievedValue);
            }
        }

        [TestMethod]
        public void LruCacheHandlesDuplicateKeysCorrectly()
        {
            const int maxElements = 10;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string value = nameof(LruCacheHandlesDuplicateKeysCorrectly) + "_original";
            const string overriddenValue = nameof(LruCacheHandlesDuplicateKeysCorrectly) + "_overridden";

            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);
                cache.Put(i, overriddenValue);

                /* Cache eviction */
                for(int j = 0; (j < i - maxElements) && (0 <= j); ++j)
                {
                    string shouldNotExist;
                    bool exists = cache.GetIfPresent(j, out shouldNotExist);
                    Assert.IsFalse(exists);
                }

                int minKey = i - maxElements + 1;
                /* Overridden values */
                for(int j = minKey; (0 <= j) && (j <= i); ++j)
                {
                    string shouldExist;
                    bool exists = cache.GetIfPresent(j, out shouldExist);
                    Assert.IsTrue(exists);
                    Assert.AreEqual(shouldExist, overriddenValue);
                }
            }
        }

        [TestMethod]
        public void LruCacheInsertsElements()
        {
            const int maxElements = 100;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string valueBase = nameof(LruCacheInsertsElements);

            for(int i = 0; i < maxElements * 10; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);

                string retrievedValue;
                bool success = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsTrue(success);
                Assert.AreSame(value, retrievedValue);

                for(int j = i - maxElements + 1; (j <= i) && (0 <= j); ++j)
                {
                    string otherCacheValue;
                    bool exists = cache.GetIfPresent(j, out otherCacheValue);
                    Assert.IsTrue(exists);
                    string expected = valueBase + j;
                    Assert.AreEqual(expected, otherCacheValue);
                }
            }
        }

        [TestMethod]
        public void LruCacheInvalidatesAllElements()
        {
            const int maxElements = 100;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string valueBase = nameof(LruCacheInvalidatesAllElements);

            for(int i = 0; i < maxElements; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);
            }

            cache.InvalidateAll();

            for(int i = 0; i < maxElements; ++i)
            {
                string value;
                bool valueExists = cache.GetIfPresent(i, out value);
                Assert.IsFalse(valueExists);
            }
        }

        [TestMethod]
        public void LruCacheInvalidatesElements()
        {
            const int maxElements = 100;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string valueBase = nameof(LruCacheInvalidatesElements);

            for(int i = 0; i < maxElements * 10; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);

                string retrievedValue;
                bool success = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsTrue(success);
                Assert.AreSame(value, retrievedValue);

                cache.Invalidate(i);
                bool shouldFail = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsFalse(shouldFail);
            }

            Assert.AreEqual(0, cache.Count);
        }

        [TestMethod]
        public void NoRemovalNotificationOnPutSameValue()
        {
            HashSet<int> expiredKeys = new HashSet<int>();

            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                bool newKey = expiredKeys.Add(notification.Key);
                Assert.IsTrue(newKey);
            };

            const int maxElements = 10;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .WithRemovalListener(removalNotifier)
                    .Build();

            const string value = nameof(NoRemovalNotificationOnPutSameValue);

            for(int i = 0; i < maxElements; ++i)
            {
                cache.Put(i, value);
                cache.Put(i, value);

                string foundValue;
                bool exists = cache.GetIfPresent(i, out foundValue);
                Assert.IsTrue(exists);
                Assert.AreSame(foundValue, value);
                Assert.IsFalse(expiredKeys.Any());
            }
        }

        [TestMethod]
        public void SingleThreadLruEvictedNotification()
        {
            HashSet<int> expiredKeys = new HashSet<int>();

            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                Assert.AreEqual(RemovalCause.Evicted, notification.RemovalCause);
                bool newKey = expiredKeys.Add(notification.Key);
                Assert.IsTrue(newKey);
            };

            const int maxElements = 10;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .WithRemovalListener(removalNotifier)
                    .Build();

            const string value = nameof(SingleThreadLruEvictedNotification);

            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);

                string lruValue;
                int lruKey = i - maxElements;
                bool exists = cache.GetIfPresent(lruKey, out lruValue);
                Assert.IsFalse(exists);
                Assert.IsTrue(cache.Count <= maxElements);
                if(0 <= lruKey)
                {
                    Assert.IsTrue(expiredKeys.Contains(lruKey));
                }
            }
        }

        [TestMethod]
        public void SingleThreadLruExpiration()
        {
            const int maxElements = 10;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string value = nameof(SingleThreadLruExpiration);
            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);

                string lruValue;
                bool exists = cache.GetIfPresent(i - maxElements, out lruValue);
                Assert.IsFalse(exists);
                Assert.IsTrue(cache.Count <= maxElements);
            }
        }

        [TestMethod]
        public void SingleThreadRandomAccessLruExpiration()
        {
            const int maxElements = 10;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string value = nameof(SingleThreadRandomAccessLruExpiration);

            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);

                /* This should make it so that the first elements in the cache are the warmest, always, and will never be removed */
                for(int j = 0; (j < i) && (j < maxElements - 1); ++j)
                {
                    string doesntMatter;
                    bool exists = cache.GetIfPresent(j, out doesntMatter);
                    Assert.IsTrue(exists);
                }

                for(int j = maxElements; j < i; ++j)
                {
                    string shouldntExist;
                    bool itExistedOhNo = cache.GetIfPresent(j, out shouldntExist);
                    Assert.IsFalse(itExistedOhNo);
                }
            }
        }
    }
}