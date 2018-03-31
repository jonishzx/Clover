namespace Clover.Core.Base
{
	#region 引用的命名空间.
	

	using System;
	using System.Runtime.InteropServices;

	
	#endregion

	

	
	
	
	[ComVisible( false )]
	public interface IApplicationEnvironment
	{
		#region 接口成员.
		

		
		
		
		void Pump();

		
		
		
		
		
		void Pump( 
			string message );

		
		#endregion
	}

	
}