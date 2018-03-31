namespace Clover.Core.Base
{
	#region 引用的命名空间.
	

	using System;
	using Clover.Core.Logging;

	
	#endregion

	

	
	
	
	public class GenericBase
	{
		#region 公共属性.
		

		
		
		
		private static object typeLock = new object();

		
		
		
		
		public static IApplicationEnvironment ApplicationEnvironment
		{
			get
			{
				lock ( typeLock )
				{
					return applicationEnvironment;
				}
			}
			set
			{
				lock ( typeLock )
				{
					applicationEnvironment = value;
				}
			}
		}

		
		
		
		
		public static string MagicKey
		{
			get
			{
				return @"DCBE5938EFC54967ABDDE79BA639128B";
			}
		}

		
		#endregion

		#region Public routines.
		

		
		
		
		public GenericBase()
		{
			
			AppDomain.CurrentDomain.UnhandledException +=
				new UnhandledExceptionEventHandler( UnhandledException );
		}

		
		#endregion

		#region Private member.
		

		
		
		
		
		protected virtual void HandleApplicationError(
			Exception e )
		{
			LogCentral.Current.Error(
				@"Application error occured.",
				e );
		}

		
		
		
		
		
		
		private void UnhandledException(
			object sender,
			UnhandledExceptionEventArgs e )
		{
			try
			{
				Exception x = e.ExceptionObject as Exception;
				HandleApplicationError( x );
			}
			catch ( Exception )
			{
				
			}
		}

		
		
		
		private static IApplicationEnvironment
			applicationEnvironment;

		
		#endregion
	}

	
}