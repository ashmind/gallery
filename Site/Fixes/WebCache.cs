using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using System.Web.Caching;

using Web = System.Web;
using Runtime = System.Runtime;

namespace AshMind.Gallery.Site.Fixes {
    // superhacky fix for the MemoryCache not being able to define file dependencies due to 
    // https://connect.microsoft.com/VisualStudio/feedback/details/565313/breaking-problem-with-datetimeoffset-that-affects-multiple-framework-classes
    public class WebCache : ObjectCache {
        private readonly IDictionary<CacheItemRemovedReason, CacheEntryRemovedReason> reasonMap = new Dictionary<CacheItemRemovedReason, CacheEntryRemovedReason> {
            { CacheItemRemovedReason.DependencyChanged, CacheEntryRemovedReason.ChangeMonitorChanged },
            { CacheItemRemovedReason.Expired, CacheEntryRemovedReason.Expired },
            { CacheItemRemovedReason.Removed, CacheEntryRemovedReason.Removed },
            { CacheItemRemovedReason.Underused, CacheEntryRemovedReason.Evicted }
        };
        
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null) {
            return this.AddOrInsert(
                HttpContext.Current.Cache.Add,
                key, value, policy, regionName
            );
        }

        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy) {
            var existing = this.AddOrGetExisting(value.Key, value.Value, policy, value.RegionName);
            return existing != null ? new CacheItem(value.Key, existing, value.RegionName) : null;
        }

        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null) {
            return this.AddOrGetExisting(key, value, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration }, regionName);
        }

        public override bool Contains(string key, string regionName = null) {
            throw new NotSupportedException();
        }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null) {
            throw new NotImplementedException();
        }

        public override object Get(string key, string regionName = null) {
            return HttpContext.Current.Cache.Get(key);
        }

        public override CacheItem GetCacheItem(string key, string regionName = null) {
            return new CacheItem(key, this.Get(key, regionName), regionName);
        }

        public override long GetCount(string regionName = null) {
            return HttpContext.Current.Cache.Count;
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null) {
            throw new NotImplementedException();
        }

        public override object Remove(string key, string regionName = null) {
            return HttpContext.Current.Cache.Remove(key);
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null) {
            this.AddOrInsert(
                (k, v, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback) => {
                    HttpContext.Current.Cache.Insert(k, v, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
                    return null;
                },
                key, value, policy, regionName
            );
        }

        public override void Set(CacheItem item, CacheItemPolicy policy) {
            this.Set(item.Key, item.Value, policy, item.RegionName);
        }

        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null) {
            this.Set(key, value, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration }, regionName);
        }

        private object AddOrInsert(
            Func<string, object, CacheDependency, DateTime, TimeSpan, Web.Caching.CacheItemPriority, CacheItemRemovedCallback, object> action,
            string key, object value, CacheItemPolicy policy, string regionName = null
        ) {
            var fileNames = new List<string>();
            foreach (var monitor in policy.ChangeMonitors) {
                var fileMonitor = monitor as FileChangeMonitor;
                if (fileMonitor == null)
                    throw new NotImplementedException();

                fileNames.AddRange(fileMonitor.FilePaths);
            }

            var dependency = fileNames.Count > 0
                           ? new CacheDependency(fileNames.ToArray())
                           : null;

            var callback = (CacheItemRemovedCallback)null;
            if (policy.RemovedCallback != null) {
                callback = (removedKey, removedValue, reason) => policy.RemovedCallback(
                    new CacheEntryRemovedArguments(this, reasonMap[reason], new CacheItem(removedKey, removedValue))
                );
            }

            return action(
                key, value, dependency,
                policy.AbsoluteExpiration != ObjectCache.InfiniteAbsoluteExpiration ? policy.AbsoluteExpiration.UtcDateTime : Cache.NoAbsoluteExpiration,
                policy.SlidingExpiration != ObjectCache.NoSlidingExpiration ? policy.SlidingExpiration : Cache.NoSlidingExpiration,
                policy.Priority == Runtime.Caching.CacheItemPriority.NotRemovable
                    ? Web.Caching.CacheItemPriority.NotRemovable
                    : Web.Caching.CacheItemPriority.Default,
                callback
            );
        }

        public override object this[string key] {
            get { return this.Get(key); }
            set { this.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration, null); }
        }

        public override string Name {
            get { return "Default"; }
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities {
            get { 
                return DefaultCacheCapabilities.AbsoluteExpirations 
                     | DefaultCacheCapabilities.SlidingExpirations
                     | DefaultCacheCapabilities.CacheEntryChangeMonitors
                     | DefaultCacheCapabilities.InMemoryProvider; 
            }
        }
    }
}