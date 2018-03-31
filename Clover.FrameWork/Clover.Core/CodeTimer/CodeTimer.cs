using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Clover.Core.CodeTimer
{
    
    
    
    public static class CodeTimer
    {

        static HighResolutionTimer queryperfcounter = new HighResolutionTimer();
     
        public delegate void ActionDelegate();

        static CodeTimer()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        #region 公共方法
        
        
        
        
        
        
        public static AnalysisData? Time(string name, ActionDelegate action)
        {
            return Time(name, 1, action);

        }

        
        
        
        
        
        
        
        public static AnalysisData? Time(string name, int iteration, ActionDelegate action)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            if (action == null)
            {
                return null;
            }

            
            ConsoleColor currentForeColor = PringHeadMessage(name);

            
            int[] gcCounts = StartGc();            

            
            
            queryperfcounter.Reset();
            queryperfcounter.Start();

            
            Stopwatch watch = new Stopwatch();
            watch.Start();


            queryperfcounter.Start();

            for (int i = 0; i < iteration; i++) action();

            double ticks = queryperfcounter.Stop();

            watch.Stop();

            
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i) - gcCounts[i];
            }

            AnalysisData adata = new AnalysisData(ticks, watch.ElapsedMilliseconds, iteration, gcCounts);

            
            PringResult(adata, currentForeColor, adata.GcCounts);

            
            return adata;

        }

        #endregion

        #region 打印
        static ConsoleColor PringHeadMessage(string name)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            return currentForeColor;
        }

        static int[] StartGc()
        {
            
            
            GC.Collect(GC.MaxGeneration);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            return gcCounts;
        }

        static void PringResult(AnalysisData? adata, ConsoleColor currentForeColor, int[] gcCounts)
        {
            if (adata != null)
            {
                
                Console.ForegroundColor = currentForeColor;
                Console.WriteLine("\tTime Elapsed:\t\t" +
                   adata.Value.TotalWatchTime.ToString("N0") + "ms");
                Console.WriteLine("\tTime Elapsed (one time):" +
                   adata.Value.WatchTime.ToString("N0") + "ms");

                Console.WriteLine("\tCPU time:\t\t" + adata.Value.TotalCpuTime.ToString("N0")
                   + "ns");
                Console.WriteLine("\tCPU time (one time):\t" + adata.Value.CpuTime.ToString("N0") + "ns");

                
                for (int i = 0; i < gcCounts.Length; i++)
                {
                    Console.WriteLine("\tGen " + i + ": \t\t\t" + gcCounts[i]);
                }

                Console.WriteLine();
            }
        }
        #endregion
    }
}
