using System;
using System.Collections.Generic;
using System.Text;
using Quartz;

namespace Clover.Schedules
{
    
    
    
    public class QuartzJobsListener : IJobListener
    {
        JobToBeExecuted _beforeExecuted = null;
        
        
        
        public JobToBeExecuted BeforeExecuted { set { _beforeExecuted = value; } private get { return _beforeExecuted; } }

        JobWasExecuted _afterExecuted = null;
        
        
        
        public JobWasExecuted AfterExecuted { set { _afterExecuted = value; } private get { return _afterExecuted; } }


        
        
        
        
        
        
        public QuartzJobsListener(string name, JobToBeExecuted beforeExecuted, JobWasExecuted afterExecuted)
        {
            if (Clover.Core.Common.StringHelper.IsEmpty(name)) Name = "QuartzJobsListener";
            else Name = name;

            _beforeExecuted = beforeExecuted;
            _afterExecuted = afterExecuted;
        }

        
        
        
        
        
        public QuartzJobsListener(JobToBeExecuted beforeExecuted, JobWasExecuted afterExecuted) : this(null,beforeExecuted, afterExecuted)
        {
        }

        #region -- 实现接口 --
        
        
        
        public string Name { get; private set; }

        
        
        
        
        public void JobExecutionVetoed(IJobExecutionContext context)
        {

        }

        
        
        
        
        public void JobToBeExecuted(IJobExecutionContext context)
        {
            if (BeforeExecuted != null) BeforeExecuted(context);
        }
        
        
        
        
        
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (AfterExecuted != null) AfterExecuted(context, jobException);
        }

        #endregion

    }

    
    
    
    
    public delegate void JobToBeExecuted(IJobExecutionContext context);

    
    
    
    
    
    public delegate void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException);


}
