﻿using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;
using PersistedQueue.Sqlite;
using System.Threading.Tasks;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class SqlitePersistedQueueBenchmarks
    {
        private const string PersistenceFilename = @"C:\Test\persistence.db";

        [Params(10000)]
        public int totalItems;

        [Params(100, 10000)]
        public int itemsToKeepInMemory;

        [Params(false)]
        public bool useLargeData;

        private IPersistence<LargeData> largePersistence;
        private PersistedQueue<LargeData> largeQueue;

        private IPersistence<int> smallPersistence;
        private PersistedQueue<int> smallQueue;

        [IterationSetup]
        public void IterationSetup()
        {
            if (useLargeData)
            {
                largePersistence = new SqlitePersistence<LargeData>(PersistenceFilename);
                PersistedQueueConfiguration config = new PersistedQueueConfiguration { MaxItemsInMemory = itemsToKeepInMemory };
                largeQueue = new PersistedQueue<LargeData>(largePersistence, config);
            }
            else
            {
                smallPersistence = new SqlitePersistence<int>(PersistenceFilename);
                PersistedQueueConfiguration config = new PersistedQueueConfiguration { MaxItemsInMemory = itemsToKeepInMemory };
                smallQueue = new PersistedQueue<int>(smallPersistence, config);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            if (useLargeData)
            {
                largePersistence?.Clear();
                largePersistence?.Dispose();
            }
            else
            {
                smallPersistence?.Clear();
                smallPersistence?.Dispose();
            }
        }

        [Benchmark]
        public Task PersistentQueueSqliteFilePersistence()
        {
            if (useLargeData)
            {
                return LargeData();
            }
            else
            {
                return Int();
            }
        }

        private async Task Int()
        {
            for (int i = 0; i < totalItems; i++)
            {
                smallQueue.Enqueue(i);
            }
            for (int i = 0; i < totalItems; i++)
            {
                await smallQueue.DequeueAsync();
            }
        }

        private async Task LargeData()
        {
            for (int i = 0; i < totalItems; i++)
            {
                largeQueue.Enqueue(new LargeData());
            }
            for (int i = 0; i < totalItems; i++)
            {
                await largeQueue.DequeueAsync();
            }
        }
    }
}
