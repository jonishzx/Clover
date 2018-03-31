namespace Clover.Core.Caching
{
	#region 引用命名空间
	

	using System;
	using System.Diagnostics;
	using System.Text;

	
	#endregion

	

	
	
	
	[DebuggerDisplay( @"Key = {key}" )]
	public class SmartCacheKey :
		IEquatable<SmartCacheKey>
	{
		#region 公共方法.
		

		
		
		
		
		
		public static SmartCacheKey Build(
			params object[] keys )
		{
			StringBuilder k = new StringBuilder();

			foreach ( object key in keys )
			{
				if ( k.Length > 0 )
				{
					k.Append( @"," );
				}

				if ( key == null )
				{
					k.Append( @"(null)" );
				}
				else
				{
					k.Append( key.ToString());
				}
			}

			SmartCacheKey result = new SmartCacheKey( k.ToString() );
			return result;
		}

		
		
		
		
		
		
		public override string ToString()
		{
			return key;
		}

        
        
        
        
        
        
        
		public override bool Equals(
			object obj )
		{
			return Equals( obj as SmartCacheKey );
		}

		
		
		
		
		
		
		public override int GetHashCode()
		{
			return key.GetHashCode();
		}

		
		#endregion

		#region 公共属性.
		

		
		
		
		
		public string Key
		{
			get
			{
				return key;
			}
		}

		
		#endregion

		#region 私有变量.
		

		
		
		
		private string key;

		
		#endregion

		#region 私有方法.
		

		
		
		
		
		private SmartCacheKey(
			string key )
		{
			this.key = key;
		}

		
		#endregion

		#region IEquatable<SmartCacheKey> members.
		

		
		
		
		
		
		
		
		public bool Equals(
			SmartCacheKey other )
		{
			return this.key == other.key;
		}

		
		#endregion
	}

	
}