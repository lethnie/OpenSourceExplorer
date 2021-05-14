using System;

namespace OpenSource.GitHub.Core.Cache
{
    public interface ICacheService
    {
        /// <summary>
        /// Gets value associated with given key.
        /// </summary>
        /// <typeparam name="TValue">Requested value type.</typeparam>
        /// <param name="key">Entity key.</param>
        /// <returns>Requested value.</returns>
        TValue GetValue<TValue>(string key);

        /// <summary>
        /// Sets value in cache.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key associated with value.</param>
        /// <param name="value">Value.</param>
        /// <param name="absoluteExpirationRelativeToNow">Absolute cache expiration time, relative to now.</param>
        void SetValue<TValue>(string key, TValue value, TimeSpan? absoluteExpirationRelativeToNow = null);

        /// <summary>
        /// Removes value from cache.
        /// </summary>
        /// <param name="key">Key associated with value.</param>
        void Remove(string key);
    }
}
