﻿using System.Collections.Concurrent;

namespace WallNetCore.Extension
{
    public static class ProducerConsumerCollectionExtensions
    {
        public static void Clear<T>(this IProducerConsumerCollection<T> queue)
        {
            T ignored;
            while(queue.TryTake(out ignored))
            {
                // This page intentionally left blank
            }
        }
    }
}