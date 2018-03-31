namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
    using System.Collections.Generic;

	
	#endregion

	

	
	
	
	public interface ICacheBacker
	{
		#region 接口方法.
		

        
        
        
        IList<CacheItem> GetList();

		
		
		
		void Initialize();

		
		
		
		
		void RemoveAll(
			CacheItemGroup group );

        
        
        
        
        void RemoveAll(
            string groupName);

		
        
		
		
		
		bool Remove(
			string key );

		
		
		
		
		
		object Get(
			string key );

		
		
		
		
		
		
		
		bool Contains(
			string key );

		
		
		
		
		
		
		void Set(
			string key,
			object value,
			CacheSetting itemInfo );

        void Set(
            string key,
            object value,
            DateTime absoluteDateTime);

        void Set(
            string key,
            object value,
            TimeSpan ts);
		
		#endregion

		#region 接口属性.
		

		
		
		
		
		int Count
		{
			get;
		}

        
        
        
        int MaxPoolSize
        {
            get;
            set;
        }

		
		
		
        
		
		bool IsEmpty
		{
			get;
		}

		
		#endregion
	}

	
}