namespace Clover.Core.Base
{
	#region ���õ������ռ�.
	

	using System;
	using System.Runtime.InteropServices;

	
	#endregion

	

	
	
	
	[ComVisible( false )]
	public interface IApplicationEnvironment
	{
		#region �ӿڳ�Ա.
		

		
		
		
		void Pump();

		
		
		
		
		
		void Pump( 
			string message );

		
		#endregion
	}

	
}