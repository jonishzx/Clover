using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Core.CodeTimer
{
    
    
    
    public struct AnalysisData 
    {
                
        
        public double TotalCpuTime;

        
        
        
        public double TotalWatchTime;


        
        
        
        public double CpuTime;

        
        
        
        public double WatchTime;

        
        
        
        public int RunTimes;

        
        
        
        public int[] GcCounts;

        
        
        
        
        
        
        public AnalysisData(double totalcputime, double totalwatchtime, int times, int[] gccounts)
        {
            if (times <= 0)
                throw new ArgumentException("times 参数必须大于0");

            this.TotalCpuTime = totalcputime / 1000;
            this.TotalWatchTime = totalwatchtime;
            this.RunTimes = times;

            this.CpuTime = TotalCpuTime / (times * 1000);
            this.WatchTime = TotalWatchTime / (times * 1000);

            this.GcCounts = gccounts;
        }

        
        
        
        
        
        public AnalysisData(double totalcputime, double totalwatchtime, int[] gccounts)
            : this(totalcputime, totalwatchtime, 1, gccounts)
        {

        }
    }
}
