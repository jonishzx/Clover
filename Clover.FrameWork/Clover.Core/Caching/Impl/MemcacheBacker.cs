using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Configuration;
using System.Net;
using StructureMap;

namespace Clover.Core.Caching
{
    
    
    
    public class MemcacheBacker :
        ICacheBacker
    {

        MemcachedClient client = null;

        private object thisLock = new object();

        
        
        

        public MemcacheBacker() {
            Initialize();
        }

        #region ICacheBacker 成员

        public IList<CacheItem> GetList()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            if (client == null)
            {
                lock (thisLock)
                {
                    if (client == null)
                        client = new MemcachedClient();
                }
            }
        }

        
        
        
        
        public void RemoveAll(CacheItemGroup group)
        {
            lock (thisLock)
            {                
                this.client.FlushAll();
            }
        }

        
        
        
        
        public void RemoveAll(string groupName)
        {
            lock (thisLock)
            {
                this.client.FlushAll();
            }
        }

        public bool Remove(string key)
        {
            lock (thisLock)
            {
                return client.Remove(key);
            }
        }

        public object Get(string key)
        {
            lock (thisLock)
            {
                return client.Get(key);
            }
        }

        public bool Contains(string key)
        {
            lock (thisLock)
            {
                return client.Get(key) == null;
            }
        }

        public void Set(string key, object value, CacheSetting itemInfo)
        {
            lock (thisLock)
            {
                DateTime expiredtime = DateTime.Now;

                switch (itemInfo.ExpirationMode)
                {
                    case CacheExpirationMode.Absolute:
                        expiredtime = itemInfo.AbsoluteExpiration;                      
                        break;
                    case CacheExpirationMode.Sliding:
                        expiredtime = expiredtime.Add(itemInfo.SlidingExpiration);
                        break;
                    case CacheExpirationMode.Never:
                        client.Store(StoreMode.Set, key, value);
                        return;
                        
                    default:
                        break;
                }

                client.Store(StoreMode.Set, key, value, expiredtime);
            }
        }

        public void Set(
          string key,
          object value,
          DateTime absoluteDateTime)
        {
            lock (thisLock)
            {
                client.Store(StoreMode.Set, key, value, absoluteDateTime);
            }
        }

        public void Set(
         string key,
         object value,
         TimeSpan ts)
        {
            lock (thisLock)
            {
                client.Store(StoreMode.Set, key, value, ts);
            }
        }

        
        
        
        public int Count
        {
            get {
                throw new NotImplementedException();
            }
        }

        int maxpoolsize = 100;

        public int MaxPoolSize {
            get {
                return maxpoolsize;
            }
            set{
                maxpoolsize = value;
            }
        }

        
        
        
        public bool IsEmpty
        {
            get {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
