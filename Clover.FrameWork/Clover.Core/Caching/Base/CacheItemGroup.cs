namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
	using System.Diagnostics;

	
	#endregion

	

	
	
	
	[DebuggerDisplay( @"Name = {name}" )]
	public class CacheItemGroup
	{
        
        
        
        public const string DefaultGroup = "Default";

		#region 公共方法.
		

		
		
		
		
		
        
		
		public static bool IsNullOrEmpty(
			CacheItemGroup group )
		{
			return group == null || group.IsEmpty;
		}

		
		
		
		public CacheItemGroup()
		{
		}

		
		
		
		
		public CacheItemGroup(
			string name )
		{
			this.name = name;
		}

		
		#endregion

		#region 公共属性.
		

		
		
		
		
		public string Name
		{
			get
			{
				return name;
			}
		}

		
		
		
        
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty( name );
			}
		}

		
		#endregion

		#region 私有变量.
		

		
		
		
		private string name;

		
		#endregion
	}

	
}