namespace Clover.Core.Common
{
	#region ���õ������ռ�.
	

	using System;
	using System.Configuration;
	using System.Collections;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Clover.Core.Common;
	using System.Data;
	using System.Data.OleDb;
	using Clover.Core.Properties;
	using System.Runtime.InteropServices;

	
	#endregion

	

	
	
	
	[ComVisible( false )]
	public interface IDumpable
	{
		#region �ӿڳ�Ա.
		

		
		
		
		
		
		
		
		
		
		
		
		
		string Dump(
			int indentLevel,
			bool deep );

		
		
		
		
		
		
		
		
		
		string Dump();

		
		#endregion
	}

	
}