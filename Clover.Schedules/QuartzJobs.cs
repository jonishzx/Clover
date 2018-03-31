using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Clover.Core.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;
using Clover.Core.IO;
using StructureMap;

namespace Clover.Schedules
{
    
    
    
    
    [PluginFamily("Default")]
    public class QuartzJobs
    {
        
        
        
        public static Dictionary<string, string> JOBExucteDetails = new Dictionary<string, string>(5);

        #region helper

        public static string ConvertCronToManual(string cron)
        {
            StringBuilder sb = new StringBuilder();

            string[] vals = cron.Split(new char[] {' '});
            
            
            string sec = convertChar(vals[0], "秒");
            string min = convertChar(vals[1], "分钟");
            string hr = convertChar(vals[2], "小时");
            string date = convertChar(vals[3], "");
            string month = convertChar(vals[4], "月");
            string weekdate = convertChar(vals[5], "");
            string year = convertChar("*", "年");

            if (vals.Length > 6)
                year = convertChar(vals[6], "年");

            sb.AppendLine("执行年份:" + year);

            sb.AppendLine("执行月份:" + month);

            if (date != "?")
                sb.AppendLine("执行日(每月):" + date);

            if (weekdate != "?")
                sb.AppendLine("执行星期:" + weekdate);


            sb.Append("执行时间(时:分:秒):(" + hr + ")");
            sb.Append(":(" + min + ")");
            sb.Append(":(" + sec + ")");

            return sb.ToString();
        }

        private static string convertChar(string a, string flag)
        {
            string rst = a.Replace(",", "以及").Replace("-", "至").Replace("*", "任何");

            if (rst.IndexOf("/") >= 0)
            {
                string[] vals = rst.Split(new char[] {'/'});

                rst = rst.Replace("/", "每") + flag + "间隔执行";
            }
            return rst;
        }

        #endregion

        #region -- 单例 --

        private static volatile QuartzJobs instance;
        private static object syncRoot = new Object();

        public QuartzJobs()
        {
            sf = new StdSchedulerFactory();
            processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
            Init();

            JobCfgFileWatch();
        }

        public QuartzJobs(string jobsconfigpath)
        {
            this.jobsconfigpath = jobsconfigpath;
            sf = new StdSchedulerFactory();
            processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());

            Init();

            JobCfgFileWatch();
        }

        
        
        
        public static QuartzJobs Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new QuartzJobs();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region -- 成员 --

        private string jobsconfigpath = "/config/QuartzJobs.config";

        
        
        
        public string JobsConfigPath
        {
            get { return jobsconfigpath; }
            set { jobsconfigpath = value; }
        }

        private bool autoreload = false;

        
        
        
        public bool AutoReload
        {
            get { return autoreload; }
            set
            {
                autoreload = value;

                if (watcher != null)
                {
                    if (autoreload)
                        watcher.Start();
                    else
                        watcher.Stop();
                }
            }
        }

        private bool _isStop = true;

        
        public bool IsStop
        {
            get { return _isStop; }
        }

        private ISchedulerFactory sf;
        private XMLSchedulingDataProcessor processor;

        
        public IScheduler JobsScheduler
        {
            get { return sched; }
        }

        private IScheduler sched;

        private JobToBeExecuted GlobalBeforeExecuted { get; set; }
        private JobWasExecuted GlobalAfterExecuted { get; set; }

        
        private QuartzJobsListener GlobalJobListener;


        private Clover.Core.IO.FileWatcher watcher = null;

        #endregion

        
        
        
        public void Start()
        {
            Start(GlobalBeforeExecuted, GlobalAfterExecuted);
        }

        
        
        
        public System.Collections.Generic.ICollection<IScheduler> CurrentScheduledJobs
        {
            get { return sf.AllSchedulers; }
        }

        public void Init()
        {
            if (sched != null && !sched.IsShutdown) return;

            sched = sf.GetScheduler();
            var path = PathTool.getInstance().Map(JobsConfigPath);

            processor.ProcessFileAndScheduleJobs(path, sched);

            if (GlobalJobListener != null)
            {
                if (sched.ListenerManager.GetJobListener(GlobalJobListener.Name) == null)
                {
                    sched.ListenerManager.AddJobListener(GlobalJobListener, null);
                }
            }
        }

        
        
        
        
        
        public void Start(JobToBeExecuted globalBeforeExecuted, JobWasExecuted globalAfterExecuted)
        {
            GlobalBeforeExecuted = globalBeforeExecuted;
            GlobalAfterExecuted = globalAfterExecuted;
            try
            {
                sched.Start();

                _isStop = false;

                LogCentral.Current.Info("Quartz Task Start Success");
            }
            catch (Exception ex)
            {
                LogCentral.Current.Error("Quartz Task Start Error", ex);

                if (sched.IsShutdown)
                {
                    Start(GlobalBeforeExecuted, GlobalAfterExecuted);
                }
            }
        }

        
        
        
        public void Stop()
        {
            if (sched != null)
            {
                _isStop = true;
                
                sched.PauseAll();
                sched.Shutdown(true);
                sched = null;
                sf = new StdSchedulerFactory();
                processor.IgnoreDuplicates = true;

                Init();
            }
        }

        public void JobReStart()
        {
            Stop();

            Start();
        }

        
        
        
        private void JobCfgFileWatch()
        {
            if (AutoReload)
            {
                string path = PathTool.getInstance().Map(JobsConfigPath);
                watcher = new FileWatcher(Path.GetDirectoryName(path), Path.GetFileName(path),
                                          NotifyFilters.LastWrite);
                watcher.FileChangeEvent += new Clover.Core.Base.TAction<FileSystemEventArgs>(watcher_FileChangeEvent);
                watcher.Start();
            }
        }

        private void watcher_FileChangeEvent(FileSystemEventArgs arg)
        {
            JobReStart();
        }
    }
}