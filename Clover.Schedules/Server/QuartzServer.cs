using System;
using System.Threading;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;

namespace Clover.Schedules.Server
{
	
	
	
	public class QuartzServer : IQuartzServer
	{
		private readonly ILog logger;
		private ISchedulerFactory schedulerFactory;
		private IScheduler scheduler;

        
        
        
	    public QuartzServer()
	    {
	        logger = LogManager.GetLogger(GetType());
            
            XMLSchedulingDataProcessor p = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
	    }

	    
		
		
		public virtual void Initialize()
		{
			try
			{				
				schedulerFactory = CreateSchedulerFactory();                
				scheduler = GetScheduler();
			}
			catch (Exception e)
			{
				logger.Error("Server initialization failed:" + e.Message, e);
				throw;
			}
		}

        
        
        
        
	    protected virtual IScheduler GetScheduler()
	    {
	        return schedulerFactory.GetScheduler();
	    }

        
        
        
        
	    protected virtual IScheduler Scheduler
	    {
	        get { return scheduler; }
	    }

	    
        
        
        
        
	    protected virtual ISchedulerFactory CreateSchedulerFactory()
	    {
	        return new StdSchedulerFactory();
	    }

	    
		
		
		public virtual void Start()
		{
			scheduler.Start();

			try 
			{
				Thread.Sleep(3000);
			} 
			catch (ThreadInterruptedException) 
			{
			}

			logger.Info("Scheduler started successfully");
		}

		
		
		
		public virtual void Stop()
		{
			scheduler.Shutdown(true);
			logger.Info("Scheduler shutdown complete");
		}

        
        
        
	    public virtual void Dispose()
	    {
	        
	    }

        
        
        
	    public virtual void Pause()
	    {
	        scheduler.PauseAll();
	    }

        
        
        
	    public void Resume()
	    {
	        scheduler.ResumeAll();
	    }
	}
}
