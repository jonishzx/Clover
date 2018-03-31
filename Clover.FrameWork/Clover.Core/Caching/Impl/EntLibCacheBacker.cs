namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
	using System.Collections.Generic;
	using Clover.Core.Logging;
	using System.Diagnostics;
	using Clover.Core.Common;
    using Clover.Core.Reflection;

    using Microsoft.Practices.EnterpriseLibrary.Caching;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
	
	#endregion

	

	
	
	
	public class EntLibCacheBacker :
		ICacheBacker
	{

        ICacheManager client = EnterpriseLibraryContainer.Current.GetInstance<ICacheManager>();

		#region ICacheBackend 成员.
		

		
		
		
		private object thisLock = new object();

        public EntLibCacheBacker() { }

		
		
		
		public void Initialize()
		{
		}

        
        
        
        
        public void RemoveAll(
            string groupName)
        {
            lock (thisLock)
            {
                this.client.Flush();
            }      
        }



		
        
		
        
		public void RemoveAll(
			CacheItemGroup group )
		{

            lock (thisLock)
            {
                this.client.Flush();
            }      	
		}

		
		
		
		
		
		public bool Remove(
			string key )
		{

            lock (thisLock)
            {
                try
                {
                    client.Remove(key);
                    return true;
                }
                catch(Exception ex){
                    throw ex;
                }               
            }
		}

		
		
		
        
		
		public object Get(
			string key )
		{
            lock (thisLock)
            {
                return client.GetData(key);
            }
		}

		
		
		
        
		
		
		public void Set(
			string key,
			object value,
			CacheSetting itemInfo )
		{

            lock (thisLock)
            {
                DateTime expiredtime = DateTime.Now;
                ICacheItemExpiration expire = null;
                switch (itemInfo.ExpirationMode)
                {
                    case CacheExpirationMode.Absolute:
                        expiredtime = itemInfo.AbsoluteExpiration;
                        expire = new CacheItemExpiration(expiredtime);
                        break;
                    case CacheExpirationMode.Sliding:
                        expiredtime = expiredtime.Add(itemInfo.SlidingExpiration);
                        expire = new SlidingCacheItemExpiration(itemInfo.SlidingExpiration);
                        break;
                    case CacheExpirationMode.Never:
                        client.Add(key, value);
                        return;
                    default:
                        break;
                }

                client.Add(key, value, CacheItemPriority.Normal, new NullCacheItemRefreshAction(), expire);
            }
        }

        
        
        
        
        
        
        public virtual void Set(
            string key,
            object value,
            DateTime absoluteDateTime)
        {
            lock (thisLock)
            {
                DateTime expiredtime = DateTime.Now;
                ICacheItemExpiration expire = null;
                expire = new CacheItemExpiration(absoluteDateTime);
                client.Add(key, value, CacheItemPriority.Normal, new NullCacheItemRefreshAction(), expire);
            }
        }

        
        
        
        
        
        
        public virtual void Set(
            string key,
            object value,
            TimeSpan ts)
        {
            lock (thisLock)
            {
                CacheSetting cs = new CacheSetting();
                cs.AbsoluteExpiration = DateTime.Now.Add(ts);
                Set(key, value, cs);
            }          
        }

		
		
		
		
		public int Count
		{
			get
			{
				lock ( thisLock )
				{
					return client.Count;
				}
			}
		}

        int maxpoolsize = 100;

        public int MaxPoolSize
        {
            get
            {
                return maxpoolsize;
            }
            set
            {
                maxpoolsize = value;
            }
        }

		
		
		
		
		
        
        
		
		public bool Contains(
			string key )
		{
            return client.Contains(key);
		}


        
        
        
        
        
		public bool IsEmpty
		{
			get
			{
				return Count <= 0;
			}
		}

        
        
        
        
        public IList<CacheItem> GetList() {
            return null;
        }
		
		#endregion

		#region 管理过期缓冲的Helper类.

        [Serializable]
        
        
        
        public class CacheItemExpiration : ICacheItemExpiration
        {

            public CacheItemExpiration() { }

            DateTime expiredtime = DateTime.Now;
            public CacheItemExpiration(DateTime dt)
            {
                expiredtime = dt;
            }

            #region ICacheItemExpiration 成员

            public bool HasExpired()
            {
                return expiredtime < DateTime.Now;
            }

            public void Initialize(Microsoft.Practices.EnterpriseLibrary.Caching.CacheItem owningCacheItem)
            {
                owningCacheItem.TouchedByUserAction(false);
            }

            public void Notify()
            {

            }

            #endregion
        }

        [Serializable]
        
        
        
        public class SlidingCacheItemExpiration : ICacheItemExpiration
        {

            public SlidingCacheItemExpiration() { }

            TimeSpan slidingexpiredtime = TimeSpan.Zero;
            Microsoft.Practices.EnterpriseLibrary.Caching.CacheItem item = null;
            public SlidingCacheItemExpiration(TimeSpan timespan)
            {
                slidingexpiredtime = timespan;
            }

            #region ICacheItemExpiration 成员

            public bool HasExpired()
            {
                if (item.LastAccessedTime.Add(slidingexpiredtime) <= DateTime.Now)
                {
                    item.TouchedByUserAction(false);
                    return false;
                }
                else
                    return true;
            }

            public void Initialize(Microsoft.Practices.EnterpriseLibrary.Caching.CacheItem owningCacheItem)
            {
                item = owningCacheItem;
            }

            public void Notify()
            {

            }

            #endregion
        }


        [Serializable]
        public class NullCacheItemRefreshAction : ICacheItemRefreshAction
        {

            #region ICacheItemRefreshAction 成员

            public void Refresh(string removedKey, object expiredValue, CacheItemRemovedReason removalReason)
            {
                
                return;
            }

            #endregion
        }

		#endregion

	
	}

	
}