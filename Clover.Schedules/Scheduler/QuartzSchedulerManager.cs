using System;
using System.Collections.Generic;
using System.Text;
using Clover.Core.Logging;
using Quartz;

namespace Clover.Schedules
{
    
    
    
    public class QuartzSchedulerManager
    {
        
        private static readonly object sync = new object();

        private static IDictionary<string, QuartzScheduler> Schedulers = new Dictionary<string, QuartzScheduler>();

        
        
        
        public static List<QuartzScheduler> SchedulerList
        {
            get { return new List<QuartzScheduler>(Schedulers.Values); }
        }

        
        
        
        public static int SchedulerQty
        {
            get { return Schedulers.Values.Count; }
        }

        
        
        
        
        public static int RunningSchedulerQty
        {
            get { return SchedulerList.FindAll(o => o.IsStop == false).Count; }
        }

        
        
        
        
        
        
        
        
        
        
        public static bool AddAndRun(string schedulerName, string jobsConfigPath, ILogger logger,
                                     IJobListener globalJobListener, JobToBeExecuted globalBeforeExecuted,
                                     JobWasExecuted globalAfterExecuted)
        {
            return Add(schedulerName, jobsConfigPath, true, logger, globalJobListener, globalBeforeExecuted,
                       globalAfterExecuted);
        }


        
        
        
        
        
        
        
        
        
        
        public static bool Add(string schedulerName, string jobsConfigPath, bool isRun, ILogger logger,
                               IJobListener globalJobListener, JobToBeExecuted globalBeforeExecuted,
                               JobWasExecuted globalAfterExecuted)
        {
            schedulerName = schedulerName.Trim().ToLower();
            jobsConfigPath = jobsConfigPath.Trim().ToLower();

            if (Schedulers.ContainsKey(schedulerName))
                throw new Exception(string.Concat("已存在相同名称的调度器，参数：schedulerName='", schedulerName, "'"));
            lock (sync)
            {
                Schedulers.Add(schedulerName,
                               new QuartzScheduler(schedulerName, jobsConfigPath, logger, globalJobListener,
                                                   globalBeforeExecuted, globalAfterExecuted));
                if (isRun) Schedulers[schedulerName].Start();
            }
            return true;
        }


        
        
        
        
        
        
        
        
        
        
        
        public static bool SetAndRun(string schedulerName, string jobsConfigPath, ILogger logger,
                                     IJobListener globalJobListener, JobToBeExecuted globalBeforeExecuted,
                                     JobWasExecuted globalAfterExecuted)
        {
            return Set(schedulerName, jobsConfigPath, true, logger, globalJobListener, globalBeforeExecuted,
                       globalAfterExecuted);
        }


        
        
        
        
        
        
        
        
        
        
        
        public static bool Set(string schedulerName, string jobsConfigPath, bool isRun, ILogger logger,
                               IJobListener globalJobListener, JobToBeExecuted globalBeforeExecuted,
                               JobWasExecuted globalAfterExecuted)
        {
            schedulerName = schedulerName.Trim().ToLower();
            jobsConfigPath = jobsConfigPath.Trim().ToLower();

            if (Schedulers.ContainsKey(schedulerName))
            {
                if (jobsConfigPath == Schedulers[schedulerName].JobsConfigPath)
                    throw new Exception("所设置的调度器已经存在,不做任何操作");

                Schedulers[schedulerName].Stop();
                lock (sync)
                {
                    Schedulers.Remove(schedulerName);
                }
            }
            lock (sync)
            {
                Schedulers.Add(schedulerName,
                               new QuartzScheduler(schedulerName, jobsConfigPath, logger, globalJobListener,
                                                   globalBeforeExecuted, globalAfterExecuted));
                if (isRun) Schedulers[schedulerName].Start();
            }
            return true;
        }

        
        
        
        
        
        public static QuartzScheduler Get(string schedulerName)
        {
            schedulerName = schedulerName.Trim().ToLower();

            if (!Schedulers.ContainsKey(schedulerName))
                return null;
            else
                return Schedulers[schedulerName];
        }


        
        
        
        
        
        public static void Remove(string schedulerName)
        {
            schedulerName = schedulerName.Trim().ToLower();

            if (Schedulers.ContainsKey(schedulerName))
            {
                Schedulers[schedulerName].Stop();
                lock (sync)
                {
                    Schedulers.Remove(schedulerName);
                }
                return;
            }
            throw new Exception(string.Concat("不存在名称为：“", schedulerName, "”的调度器"));
        }

        
        
        
        public static void Clear()
        {
            lock (sync)
            {
                if (Schedulers.Count > 0)
                {
                    foreach (QuartzScheduler s in Schedulers.Values)
                    {
                        s.Stop();
                    }
                    Schedulers.Clear();
                }
            }
        }
    }
}