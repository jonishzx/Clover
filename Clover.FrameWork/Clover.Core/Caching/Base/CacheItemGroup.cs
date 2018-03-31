namespace Clover.Core.Caching
{
	#region ���������ռ�
	

	using System;
	using System.Diagnostics;

	
	#endregion

	

	
	
	
	[DebuggerDisplay( @"Name = {name}" )]
	public class CacheItemGroup
	{
        
        
        
        public const string DefaultGroup = "Default";

		#region ��������.
		

		
		
		
		
		
        
		
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

		#region ��������.
		

		
		
		
		
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

		#region ˽�б���.
		

		
		
		
		private string name;

		
		#endregion
	}

	
}