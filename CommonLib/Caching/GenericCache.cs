using System;
using System.Web;

// TO DO: write a macro for cachename+'-'+id.ToString()

namespace CommonLib.Caching
{
    public static class GenericCache
    {
        private static string Cachekey(string cachename, Guid id)
        {
            return (cachename + '-' + id.ToString());
        }

        private static string Cachekey(string cachename, string id)
        {
            return (cachename + '-' + id);
        }

        public static void AddCacheItem(string cachename, object value)
        {
            var _cache = HttpContext.Current.Cache;
            _cache.Insert(cachename, value);
        }

        public static void AddCacheItem(string cachename, Guid id,object value)
        {
            var _cache = HttpContext.Current.Cache;
            if (id != null)
                _cache.Insert(Cachekey(cachename, id), value);
        }

        public static void AddCacheItem(string cachename, string id, object value)
        {
            var _cache = HttpContext.Current.Cache;
            if (id != null)
                _cache.Insert(Cachekey(cachename, id), value);
        }

        public static void RemoveCacheItem(string cachename)
        {
            var _cache = HttpContext.Current.Cache;
            _cache.Remove(cachename);
        }

        public static void RemoveCacheItem(string cachename, Guid id)
        {
            var _cache = HttpContext.Current.Cache;
            _cache.Remove(Cachekey(cachename, id));
        }

        public static void RemoveCacheItem(string cachename, string id)
        {
            var _cache = HttpContext.Current.Cache;
            _cache.Remove(Cachekey(cachename, id));
        }

        public static object GetCacheItem(string cachename)
        {
            var _cache = HttpContext.Current.Cache;
            return (_cache[cachename]);
        }

        public static object GetCacheItem(string cachename, Guid id)
        {
            var _cache = HttpContext.Current.Cache;
            return (_cache[Cachekey(cachename, id)]);
        }

        public static object GetCacheItem(string cachename, string id)
        {
            var _cache = HttpContext.Current.Cache;
            return (_cache[Cachekey(cachename, id)]);
        }
    }
}
