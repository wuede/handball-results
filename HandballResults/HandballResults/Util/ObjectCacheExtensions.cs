using System;
using System.Runtime.Caching;

namespace HandballResults.Util
{
    public static class ObjectCacheExtensions
    {
        public static T AddOrGetFromCache<T>(this ObjectCache cache, string key, Func<T> valueFactory, DateTimeOffset expiration)
        {
            var newValue = new Lazy<T>(valueFactory);

            // the line belows returns existing item or adds the new value if it doesn't exist
            var obj = cache.AddOrGetExisting(key, newValue, expiration);
            var value = (Lazy<T>)obj;
            try
            {
                return (value ?? newValue).Value; // Lazy<T> handles the locking itself
            }
            catch (Exception)
            {
                cache.Remove(key);
                throw;
            }
        }
    }
}