namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;

	
	#endregion

	

	
	
	
	
	public class CacheItemHolder<T>
	{
		#region 构造函数.
		

		
		
		
		public CacheItemHolder()
		{
		}

		
		
		
		
		public CacheItemHolder(
			CacheManager cache )
		{
			this.cache = cache;
		}

		
		
		
		
		public CacheItemHolder(
			T value )
		{
			this.Value = value;
		}

		
		
		
		
		
		public CacheItemHolder(
			CacheManager cache,
			T value )
		{
			this.cache = cache;
			this.Value = value;
		}

		
		
		
		
		public CacheItemHolder(
			CacheSetting cacheItemInfo )
		{
			this.cacheItemInfo = cacheItemInfo;
		}

		
		
		
		
		
		public CacheItemHolder(
			CacheManager cache,
			CacheSetting cacheItemInfo )
		{
			this.cache = cache;
			this.cacheItemInfo = cacheItemInfo;
		}

		
		
		
		
		
		public CacheItemHolder(
			T value,
			CacheSetting cacheItemInfo )
		{
			this.Value = value;
			this.cacheItemInfo = cacheItemInfo;
		}

		
		
		
		
		
		
		public CacheItemHolder(
			CacheManager cache,
			T value,
			CacheSetting cacheItemInfo )
		{
			this.cache = cache;
			this.Value = value;
			this.cacheItemInfo = cacheItemInfo;
		}

		
		
		
		
		public CacheItemHolder(
			CacheItemGroup cacheItemGroup )
		{
			this.cacheItemInfo.Group = cacheItemGroup;
		}

		
		
		
		
		
		public CacheItemHolder(
			CacheManager cache,
			CacheItemGroup cacheItemGroup )
		{
			this.cache = cache;
			this.cacheItemInfo.Group = cacheItemGroup;
		}

		
		
		
		
		
		public CacheItemHolder(
			T value,
			CacheItemGroup cacheItemGroup )
		{
			this.Value = value;
			this.cacheItemInfo.Group = cacheItemGroup;
		}

		
		
		
		
		
		
		public CacheItemHolder(
			CacheManager cache,
			T value,
			CacheItemGroup cacheItemGroup )
		{
			this.cache = cache;
			this.Value = value;
			this.cacheItemInfo.Group = cacheItemGroup;
		}

		
		#endregion

		#region 公共属性.
		

		
		
		
		
		public T Value
		{
			get
			{
				if ( string.IsNullOrEmpty( cacheKey ) )
				{
					return default( T );
				}
				else
				{
					return (T)cache.Get( cacheKey );
				}
			}
			set
			{
				Remove();

				if ( value != null )
				{
					cacheKey = value.GetHashCode().ToString();
					cache.Set( cacheKey, value, cacheItemInfo );
				}
			}
		}

		
		#endregion

		#region 私有方法.
		

		
		
		
		private void Remove()
		{
			if ( !string.IsNullOrEmpty( cacheKey ) )
			{
				cache.Remove( cacheKey );
				cacheKey = null;
			}
		}

		
		#endregion

		#region 私有变量.
		

		private string cacheKey;
		private CacheSetting cacheItemInfo =
			CacheSetting.Default;

		private CacheManager cache = defaultCacheManager;

		
		
		
		
		private static CacheManager defaultCacheManager
		{
			get
			{
                return new CacheManager();
			}
		}

		
		#endregion
	}

	
}