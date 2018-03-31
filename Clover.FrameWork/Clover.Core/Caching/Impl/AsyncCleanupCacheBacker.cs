namespace Clover.Core.Caching
{
	#region ���������ռ�
	

	using System;
	using System.Threading;
	using Clover.Core.Common;
	using System.Configuration;
	using Clover.Core.Logging;

	
	#endregion

	

	
    
	
	
	
	
	
	
	
	
	public class AsyncCleanupCacheBacker :
		SimpleCacheBacker,
		IDisposable
	{
		#region ��������.
		

		
		
		
		public AsyncCleanupCacheBacker()
		{
			if ( isAsyncEnabled )
			{
				LogCentral.Current.Debug(
					string.Format(
					@"[Cache] Constructing asynchron cache backend with 'isAsyncEnabled' set to {0}.",
					isAsyncEnabled ) );

				TimerCallback timerDelegate =
					new TimerCallback( DoAsyncCleanup );

				
				asyncTimer = new Timer(
					DoAsyncCleanup,
					null,
					timerIntervalMilliSeconds,
					timerIntervalMilliSeconds
					);
			}
		}

		
		#endregion

		#region ˽�з���.
		

        
        
        
        
        
		protected override void OnAfterValueSet()
		{
			if ( isAsyncEnabled )
			{
				base.OnAfterValueSet();
			}
			else
			{
				
				
			}
		}

		
		
		
		
		private void DoAsyncCleanup(
			object state )
		{
			lock ( thisLock )
			{
				DateTime d1 = DateTime.Now;
				LogCentral.Current.Debug(
					string.Format(
					@"[����] �� '{0}' �����첽��������.",
					d1 ) );

				int removedCount = CleanupOldEntries();

				DateTime d2 = DateTime.Now;
				LogCentral.Current.Debug(
					string.Format(
                    @"[����] �� '{0}' �ɹ��������, ������� {1} ��Ŀ, ����ʱ�� '{2}'.",
					d2,
					removedCount,
					d2 - d1 ) );
			}
		}

		
		#endregion

		#region ˽�б���.
		

		
		
		
		private object thisLock = new object();

		
		
		
		private Timer asyncTimer;

		
		
		
		private static readonly int timerIntervalMilliSeconds =
			ConvertHelper.ToInt32(
			ConfigurationManager.AppSettings[@"asyncThreadCleanupInterval"],
			60 * 1000 );

		
		
		
		private static readonly bool isAsyncEnabled =
			!ConvertHelper.ToBoolean(
			ConfigurationManager.AppSettings[@"disableAsyncThreadCleanup"],
			false );

		
		#endregion

		#region IDisposable ��Ա.
		

		
		
		
		public void Dispose()
		{
			if ( asyncTimer != null )
			{
				Timer t = asyncTimer;
				asyncTimer = null;

				t.Dispose();
			}
		}

		
		#endregion
	}

	
}