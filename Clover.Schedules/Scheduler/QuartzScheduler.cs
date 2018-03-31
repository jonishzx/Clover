using System;
using System.Collections.Generic;
using System.Text;
using Clover.Core.IO;
using Clover.Core.Logging;
using Quartz;
using Quartz.Simpl;
using Quartz.Xml;
using Quartz.Impl;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Threading;

namespace Clover.Schedules
{
    public class QuartzScheduler
    {
        #region -- 成员 --

        
        
        
        public string Name { get; private set; }

        
        
        
        public string JobsConfigPath { get; private set; }


        
        public bool IsStop { get; private set; }

        private ISchedulerFactory sf;
        private XMLSchedulingDataProcessor processor;

        private IScheduler sched;

        
        public IScheduler JobsScheduler
        {
            get { return sched; }
        }


        public JobToBeExecuted GlobalBeforeExecuted { get; set; }
        public JobWasExecuted GlobalAfterExecuted { get; set; }

        
        public IJobListener GlobalJobListener { get; set; }

        
        private FileWatcher _fsWatcher;

        private ILogger Log { get; set; }

        #endregion

        
        
        
        
        
        
        
        
        
        public QuartzScheduler(string name, string jobsConfigPath, ILogger logger, IJobListener globalJobListener,
                               JobToBeExecuted globalBeforeExecuted, JobWasExecuted globalAfterExecuted)
        {
            JobsConfigPath = jobsConfigPath;
            JobCfgFileWatch();

            if (logger == null) Log = LogCentral.Current;
            else Log = logger;

            if (name == null) Name = jobsConfigPath;
            else Name = name;

            GlobalJobListener = globalJobListener;
            GlobalBeforeExecuted = globalBeforeExecuted;
            GlobalAfterExecuted = globalAfterExecuted;

            IsStop = true;
        }


        
        
        
        public void Start()
        {
            
            if (!IsStop) return;
            if (sched != null && !sched.IsShutdown)
            {
                IsStop = true;
                return;
            }

            sf = new StdSchedulerFactory();
            processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());

            try
            {
                if (sf.GetScheduler(Name) == null)
                    sched = sf.GetScheduler();
                else
                    sched = sf.GetScheduler(Name);

                processor.ProcessFileAndScheduleJobs(JobsConfigPath, sched);
             
                
                if (GlobalJobListener == null)
                {
                        if (GlobalBeforeExecuted != null || GlobalAfterExecuted != null)
                        GlobalJobListener = new QuartzJobsListener(Name, GlobalBeforeExecuted, GlobalAfterExecuted);
                }
                if (GlobalJobListener != null)
                {
                   
                    if (sched.ListenerManager.GetJobListener(Name)==null)
                    {
                        sched.ListenerManager.AddJobListener(GlobalJobListener, null);
                    }
                }

                sched.Start();
                IsStop = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                if (sched!=null && sched.IsShutdown)
                {
                    Start();
                }
            }
            Log.Info(string.Concat("QuartzScheduler:“", Name, "”启动!", JobsConfigPath));
        }


        
        
        
        public void Stop()
        {
            if (IsStop) return;

            if (sched != null)
            {
                IsStop = true;
                
                
                sched.Shutdown(true);
                sched = null;
                
                
            }

            Log.Info(string.Concat("QuartzScheduler:“", Name, "”停止!", JobsConfigPath));
        }

        #region -- 监听Quartz.Net配置文件 --

        
        
        
        private void JobCfgFileWatch()
        {
            string path = PathTool.getInstance().Map(JobsConfigPath);
            _fsWatcher = new FileWatcher(Path.GetDirectoryName(path), Path.GetFileName(path),
                                                  NotifyFilters.LastWrite);
            _fsWatcher.FileChangeEvent += new Clover.Core.Base.TAction<FileSystemEventArgs>(FsWatcherFileChangeEvent);
            _fsWatcher.Start();
        }

        void FsWatcherFileChangeEvent(FileSystemEventArgs arg)
        {
            if (!IsStop)
                JobReStart();
        }

        private void JobReStart()
        {
            Stop();
            Thread.Sleep(1000);
            Start();
        }

        #endregion
    }
}