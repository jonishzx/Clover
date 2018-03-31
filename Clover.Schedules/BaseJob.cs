using System;
using System.Collections.Generic;
using System.Text;
using Clover.Core.Logging;
using Quartz;
using Clover.Core;

namespace Clover.Schedules
{
    
    
    
    [DisallowConcurrentExecution]
    public abstract class BaseJob : IJob
    {
       
        
        
        
        protected string JobName { get; private set; }

        
        
        
        protected ILogger JobLogger { get; private set; }

        
        
        
        
        protected BaseJob(string jobName)
        {
            JobName = jobName;
            
        }

        
        
        
        protected string GetJobDescription(IJobExecutionContext context) {
            return string.Format("执行时间:{0},上次执行时间:{1},下次执行时间:{2}",
               (context.FireTimeUtc.HasValue ? context.FireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : ""),
               (context.PreviousFireTimeUtc.HasValue ? context.PreviousFireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : ""),
               (context.NextFireTimeUtc.HasValue ? context.NextFireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : "")
               );
        }

        protected BaseJob()
        {            
        }
    

        #region IJob 成员

        public abstract void Execute(IJobExecutionContext context);

        #endregion
    }
}
