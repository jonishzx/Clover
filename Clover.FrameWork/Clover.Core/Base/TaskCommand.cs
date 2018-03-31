namespace Clover.Core.Base
{
	#region ���õ������ռ�.
	

	using System;
	using System.Collections;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using Clover.Core.Common;
	using System.Collections.Generic;
	using Clover.Core.Properties;
    using Clover.I18n;
	using Clover.Core.Logging;

	
	#endregion

	

	
	
	
	
	public class TaskCommand
	{
		#region ��������.
		

		
		
		
		public TaskCommand()
		{
		}

		
		
		
		
		
		public TaskCommand(
			string symbolicName )
		{
			this.symbolicName = symbolicName;
		}

		
		
		
		
		
		
		public TaskCommand(
			string symbolicName,
			string description )
		{
			this.symbolicName = symbolicName;
			this.description = description;
		}

		
		#endregion

		#region ��������.
		

		
		
		
		
		public string SymbolicName
		{
			get
			{
				return symbolicName;
			}
			set
			{
				symbolicName = value;
			}
		}

		
		
		
		
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		
		#endregion

		#region ˽�б���.
		

		private string symbolicName;
		private string description;

		
		#endregion
	}

	
}