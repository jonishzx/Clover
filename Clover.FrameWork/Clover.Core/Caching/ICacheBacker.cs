namespace Clover.Core.Caching
{
	#region ���������ռ�
	

	using System;
    using System.Collections.Generic;

	
	#endregion

	

	
	
	
	public interface ICacheBacker
	{
		#region �ӿڷ���.
		

        
        
        
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

		#region �ӿ�����.
		

		
		
		
		
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