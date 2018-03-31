namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
	using System.Collections.Generic;
	using Clover.Core.Logging;
	using System.Diagnostics;
	using Clover.Core.Common;
    using Clover.Core.Reflection;

	
	#endregion

	

	
	
	
	public class SimpleCacheBacker :
		ICacheBacker
	{
		#region ICacheBackend 成员.
		

		
		
		
		private object thisLock = new object();

        public SimpleCacheBacker() { }

		
		
		
		public void Initialize()
		{
		}

        
        
        
        
        public void RemoveAll(
            string groupName)
        {
            lock (thisLock)
            {
                if (string.IsNullOrEmpty(groupName))
                {

                    
                    int count = inMemoryCache.Count;

                    if (count > 0)
                    {
                        LogCentral.Current.Debug(
                            string.Format(
                            @"[缓冲]  在 '{1}', 忽略其键值 移除 {0} 个缓冲项." +
                            @"这些项目包含:",
                            count,
                            DateTime.Now));

                        int index = 1;
                        foreach (string key in inMemoryCache.Keys)
                        {
                            LogCentral.Current.Debug(
                                string.Format(
                                @"[缓冲] 移除第 {0} 项目: 键(key) = '{1}', " +
                                @"值(value) = '{2}'.",
                                index + 1,
                                key,
                                inMemoryCache[key].Dump()));

                            index++;
                        }
                    }

                    
                    inMemoryCache.Clear();

                    if (count > 0)
                    {
                        LogCentral.Current.Debug(
                            string.Format(
                            @"[缓冲] 共移 {0} 个缓冲项.",
                            count));
                    }
                }
                else
                {
                    
                    List<string> keysToRemove = new List<string>();
                 
                    
                    foreach (string key in inMemoryCache.Keys)
                    {
                        
                        CacheItem itemInfo =
                            inMemoryCache[key];

                        if (!string.IsNullOrEmpty(groupName) &&
                            itemInfo != null)
                        {
                            if (groupName.StartsWith(groupName,
                                StringComparison.InvariantCultureIgnoreCase))
                            {
                                keysToRemove.Add(key);
                            }
                        }
                    }
                   
                    
                    foreach (string key in keysToRemove)
                    {
                        
                        LogCentral.Current.Debug(
                            string.Format(
                            @"[缓冲] 在 '{0}' 移除缓冲项: " +
                            @"键(key) = '{1}', 值(value) = '{2}'.",
                            DateTime.Now,
                            key,
                            inMemoryCache[key].Dump()));

                        inMemoryCache.Remove(key);
                    }
                }
            }
        }



		
        
		
        
		public void RemoveAll(
			CacheItemGroup group )
		{

            RemoveAll(group.Name);			
		}

		
		
		
		
		
		public bool Remove(
			string key )
		{
			lock ( thisLock )
			{
				if ( inMemoryCache.ContainsKey( key ) )
				{
					LogCentral.Current.Debug(
						string.Format(
							@"[缓冲] 在 '{0}' 移除缓冲项: " +
                            @"键(key) = '{1}', 值(value) = '{2}'.",
						DateTime.Now,
						key,
						inMemoryCache[key].Dump()));
					
					inMemoryCache.Remove( key );

					return true;
				}

                return false;				
			}
		}

		
		
		
        
		
		public object Get(
			string key )
		{
			lock ( thisLock )
			{
                if (inMemoryCache.ContainsKey(key))
				{
                    if (!inMemoryCache[key].CheckIfRemove())
                        return inMemoryCache[key].Value;
                    else
                    {
                        LogCentral.Current.Debug(
                         string.Format(
                             @"[缓冲] 在 '{0}' 移除缓冲项: " +
                             @"键(key) = '{1}', 值(value) = '{2}'.",
                         DateTime.Now,
                         key,
                         inMemoryCache[key].Dump()));

                        inMemoryCache.Remove(key);

                        return null;
                    }
				}
				else
				{
					return null;
				}
			}
		}

		
		
		
        
		
		
		public void Set(
			string key,
			object value,
			CacheSetting itemInfo )
		{
			lock ( thisLock )
			{
				OnBeforeValueSet();

				inMemoryCache[key] =
					new CacheItem(
					key,
					value,
					itemInfo );

				LogCentral.Current.Debug(
					string.Format(
					@"[缓冲] 在'{0}' 添加了以下的缓冲组: " +
                    @"键(key) ='{1}', 值(value) '{2}'.",
					DateTime.Now,
					key,
					inMemoryCache[key].Dump() ) );

                CheckCurrentSize();

				OnAfterValueSet();
			}
		}




        
        
        
        protected void CheckCurrentSize() {

            if (inMemoryCache.Count > MaxPoolSize)
            {
                

                
                int cleancount = CleanupOldEntries();

                
                if (inMemoryCache.Count > MaxPoolSize)
                {
                    
                    SortedDictionary<string, string> lastaccesslist = new SortedDictionary<string, string>();

                    foreach (string key in inMemoryCache.Keys)
                    {
                        lastaccesslist.Add(inMemoryCache[key].LastAccessTime.ToString("yyyyMMddHHmmss") +  key, key);                       
                    }
                  
                    
                    int size = lastaccesslist.Count - lastaccesslist.Count / 2 - 1;
                    string[] keys = new string[lastaccesslist.Count];
                    string[] removekeys = new string[size];

                    lastaccesslist.Values.CopyTo(keys,0);

                    size = 0;
                    for (int i = 0; i < lastaccesslist.Count/2; i++)
                    {
                        removekeys[size] = keys[i];

                        size++;
                    }
                      
                    
                    foreach (string key in removekeys)
                    {
                        inMemoryCache.Remove(key);

                        LogCentral.Current.Debug(
                            string.Format(
                            @"[缓冲] 删除旧的缓冲项'{0}'.",
                            key));
                    }
                }
            }
        }

        
        
        
        
        
        
        public virtual void Set(
            string key,
            object value,
            DateTime absoluteDateTime)
        {
            lock (thisLock)
            {
                CacheSetting cs = new CacheSetting();
                cs.AbsoluteExpiration = absoluteDateTime;
                Set(key, value, cs);
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
					return inMemoryCache.Count;
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
			return Get( key ) != null;
		}


        
        
        
        
        
		public bool IsEmpty
		{
			get
			{
				return Count <= 0;
			}
		}

        
        
        
        
        public IList<CacheItem> GetList() {
            return new List<CacheItem>(inMemoryCache.Values);
        }
		
		#endregion

		#region 管理过期缓冲的Helper类.
	
		#endregion

		#region 私有方法.
		

		
		
		
		
		
		protected virtual void OnBeforeValueSet()
		{
			CleanupOldEntries();
		}

		
        
        
        
		
		protected virtual void OnAfterValueSet()
		{
		}

		
		
		
		
		
		
		protected int CleanupOldEntries()
		{
			lock ( thisLock )
			{
				int countBefore = inMemoryCache.Count;

				List<string> keysToRemove = new List<string>();

				
				foreach ( string key in inMemoryCache.Keys )
				{
					CacheItem info = inMemoryCache[key];

					if ( info.CheckIfRemove() )
					{
						keysToRemove.Add( key );
					}
				}

				

				
				foreach ( string key in keysToRemove )
				{
					inMemoryCache.Remove( key );

					LogCentral.Current.Debug(
						string.Format(
						@"[缓冲] 删除旧的缓冲项'{0}'.",
						key ) );
				}

				int countAfter = inMemoryCache.Count;

				return countAfter - countBefore;
			}
		}

		
		#endregion

		#region 私有变量.
		

		
		
		
		private Dictionary<string, CacheItem> inMemoryCache =
			new Dictionary<string, CacheItem>();

		
		#endregion
	}

	
}