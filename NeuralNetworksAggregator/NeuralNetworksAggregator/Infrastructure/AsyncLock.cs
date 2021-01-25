using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NeuralNetworksAggregator.Infrastructure
{
    public class AsyncLock
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> keysToLock =
            new ConcurrentDictionary<string, SemaphoreSlim>();

        public async Task AcquireLockAsync(string key)
        {
            if (!keysToLock.ContainsKey(key))
                keysToLock[key] = new SemaphoreSlim(1, 1);

            await keysToLock[key].WaitAsync();
        }

        public void Release(string key)
        {
            if (keysToLock.TryGetValue(key, out var value))
                value.Release();
        }
    }
}
