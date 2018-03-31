namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
	using Clover.Core.Properties;
	using System.Diagnostics;

	
	#endregion

	

	
    
	
	[DebuggerDisplay( @"GroupName = {groupName}" )]
	public class CacheSetting
	{
       

		#region 公共属性.
		

		
		
		
		
		public DateTime AbsoluteExpiration
		{
			get
			{
				return absoluteExpiration;
			}
			set
			{
				absoluteExpiration = value;
			}
		}

		
        
		
        
		public TimeSpan SlidingExpiration
		{
			get
			{
				return slidingExpiration;
			}
			set
			{
				slidingExpiration = value;
			}
		}

		
        
		
		
		public CacheExpirationMode ExpirationMode
		{
			get
			{
				if ( absoluteExpiration != NoAbsoluteExpiration )
				{
					return CacheExpirationMode.Absolute;
				}
				else if ( slidingExpiration != NoSlidingExpiration )
				{
					return CacheExpirationMode.Sliding;
				}
				else
				{
					return CacheExpirationMode.Never;
				}
			}
		}

		
		
		
		
		public static CacheSetting Default
		{
			get
			{                
				CacheSetting info = new CacheSetting();
                info.groupName = CacheItemGroup.DefaultGroup;
                info.AbsoluteExpiration = NoAbsoluteExpiration;
                info.SlidingExpiration = NoSlidingExpiration;
              
				return info;
			}
		}

        	
		
        
		
		
		
        public string GroupName {
            get {
                return groupName;
            }          
        }


		
		
        
		
		
		
		public CacheItemGroup Group
		{
			get
			{
                if (group == null)
                {
                    lock (group)
                    {
                        if(group == null)
                            group = new CacheItemGroup(groupName);
                    }
                }
                return group;
			}
			set
			{
				
				if ( value == null || value.IsEmpty )
				{
					throw new ArgumentException(
					Resources.Str_Clover_Core_Caching_CacheItemInformation_01,
						@"Group" );
				}
				else
				{
                    group = value;
					groupName = value.Name;
				}
			}
		}

		
		
		
		public static readonly DateTime NoAbsoluteExpiration =
			DateTime.MaxValue;

		
		
		
		public static readonly TimeSpan NoSlidingExpiration =
			TimeSpan.Zero;

		
		#endregion

		#region 私有变量.
		

		
		
		
		private DateTime absoluteExpiration =
			DateTime.Now.Add( TimeSpan.FromMinutes( 20 ) );

		
        
		
		private TimeSpan slidingExpiration =
			TimeSpan.FromMinutes( 20 );

		
		
		
		private string groupName = null;

        
        
        
        private CacheItemGroup group = null;
		
		#endregion
	}

	
}