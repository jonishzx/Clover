namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
	using System.Diagnostics;
    using StructureMap;
	
	#endregion

	

	
	
	
	
    [PluginFamily("Default")]
	[DebuggerDisplay( @"Count = {Count}, Backend = {backend}" )]
	public class CacheManager
	{
		#region 公共方法.
		

		
		
		
		private object thisLock = new object();

		
		
		
		public virtual void Initialize()
		{
			lock ( thisLock )
			{
				if ( backer != null )
				{
					backer.Initialize();
				}
			}
		}

		
		
		
		public virtual void RemoveAll()
		{
			lock ( thisLock )
			{
				if ( backer != null ) 
				{
					backer.RemoveAll( string.Empty );
				}
			}
		}

        
        
        
        
		public virtual void RemoveAll(
			CacheItemGroup group )
		{
			lock ( thisLock )
			{
				if ( backer != null )
				{
					backer.RemoveAll( group );
				}
			}
		}

        
        
        
        
        
		public virtual object Remove(
			string key )
		{
			lock ( thisLock )
			{
				if ( backer == null )
				{
					return null;
				}
				else
				{
					return backer.Remove( key );
				}
			}
		}

		
		
		
		
		
		public virtual object Get(
			string key )
		{
			lock ( thisLock )
			{
				if ( backer == null )
				{
					return null;
				}
				else
				{
					return backer.Get( key );
				}
			}
		}

		
		
		
		
		
		public virtual void Set(
			string key,
			object value )
		{
			Set( key, value, CacheSetting.Default );
		}

		
        
		
        
        
		
		public virtual void Set(
			string key,
			object value,
			CacheSetting itemInfo )
		{
			lock ( thisLock )
			{
				if ( backer != null )
				{
					backer.Set( key, value, itemInfo);
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
                if (backer != null)
                {
                    backer.Set(key, value, absoluteDateTime);
                }
            }
        }

        
        
        
        
        
        
        public virtual void Set(
            string key,
            object value,
            TimeSpan ts)
        {
            lock (thisLock)
            {
                if (backer != null)
                {
                    backer.Set(key, value, ts);
                }
            }
        }

		
		#endregion

		#region 公共属性.
		

		
		
		
		
		public virtual object this[string key]
		{
			get
			{
				return Get( key );
			}
			set
			{
				Set( key, value );
			}
		}

		
		
		
		
		public virtual int Count
		{
			get
			{
				lock ( thisLock )
				{
					if ( backer == null )
					{
						return 0;
					}
					else
					{
						return backer.Count;
					}
				}
			}
		}

		
		
		
		
		public bool IsEmpty
		{
			get
			{
				return Count <= 0;
			}
		}

		
        
		
		
		public ICacheBacker Backend
		{
			get
			{
				lock ( thisLock )
				{
					return backer;
				}
			}
			set
			{
				lock ( thisLock )
				{
					backer = value;

					
					if ( backer == null )
					{
						backer = new SimpleCacheBacker();
					}
				}
			}
		}

		
		#endregion

		#region 私有变量.
		

		
		
		
		private ICacheBacker backer =
			new SimpleCacheBacker();

		
        
		
		protected static volatile CacheManager currentCacheManager = null;

		
		#endregion
	}

	
}