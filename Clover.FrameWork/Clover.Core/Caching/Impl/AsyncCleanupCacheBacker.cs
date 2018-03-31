namespace Clover.Core.Caching
{
	#region 引用命名空间
	

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
		#region 公共方法.
		

		
		
		
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

		#region 私有方法.
		

        
        
        
        
        
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
					@"[缓存] 在 '{0}' 进行异步缓存清理.",
					d1 ) );

				int removedCount = CleanupOldEntries();

				DateTime d2 = DateTime.Now;
				LogCentral.Current.Debug(
					string.Format(
                    @"[缓存] 在 '{0}' 成功清除缓存, 共清除了 {1} 项目, 经过时间 '{2}'.",
					d2,
					removedCount,
					d2 - d1 ) );
			}
		}

		
		#endregion

		#region 私有变量.
		

		
		
		
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

		#region IDisposable 成员.
		

		
		
		
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