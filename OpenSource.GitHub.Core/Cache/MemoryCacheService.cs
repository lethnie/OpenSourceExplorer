using Microsoft.Extensions.Caching.Memory;
using System;

namespace OpenSource.GitHub.Core.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            this._cache = cache;
        }

        /// <summary>
        /// Gets value associated with given key.
        /// </summary>
        /// <typeparam name="TValue">Requested value type.</typeparam>
        /// <param name="key">Entity key.</param>
        /// <returns>Requested value.</returns>
        public TValue GetValue<TValue>(string key)
        {
            return this._cache.Get<TValue>(key);
        }

        /// <summary>
        /// Sets value in cache.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key associated with value.</param>
        /// <param name="value">Value.</param>
        /// <param name="absoluteExpirationRelativeToNow">Absolute cache expiration time, relative to now.</param>
        public void SetValue<TValue>(string key, TValue value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            this._cache.Set(key, value, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow });
        }

        /// <summary>
        /// Removes value from cache.
        /// </summary>
        /// <param name="key">Key associated with value.</param>
        public void Remove(string key)
        {
            this._cache.Remove(key);
        }
    }
}
