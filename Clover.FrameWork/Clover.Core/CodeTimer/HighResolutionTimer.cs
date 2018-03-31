using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Clover.Core.CodeTimer
{
    
    
    
    internal class HighResolutionTimer
    {
        
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryPerformanceCounter(
          [Out] out long lpPerformanceCount);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryPerformanceFrequency(
          [Out] out long lpFrequency);

        
        private long m_Frequency;
        private long m_StartCounter;
        private long m_StopCounter;
        private double m_Overhead;
        Decimal multiplier = new Decimal(1.0e9); 

        
        public HighResolutionTimer() : this(false) { }
        public HighResolutionTimer(bool start)
        {
            if (!QueryPerformanceFrequency(out m_Frequency))
            {             
                return;
            }

            
            Start();
            Stop();

            m_Overhead = CalcDurationWithoutOverhead();
            Reset();

            if (start)
                Start();
        }

     

        
        private double CalcDurationWithoutOverhead()
        {
            return ((double)(m_StopCounter - m_StartCounter) * (double)multiplier) / (double)m_Frequency;
        }
        private double CalcDuration()
        {
            return CalcDurationWithoutOverhead() - m_Overhead;
        }

        
        public void Reset()
        {
            m_StartCounter = 0;
            m_StopCounter = 0;
        }
        public void Start()
        {
            Reset();
            if (!QueryPerformanceCounter(out m_StartCounter))
                Debug.WriteLine("HighResolutionTimer.Start(): Error occurred while calling QueryPerformanceCounter.");
        }
        public double Stop()
        {
            if (!QueryPerformanceCounter(out m_StopCounter))
            {
                Debug.WriteLine("HighResolutionTimer.Stop(): Error occurred while calling QueryPerformanceCounter.");
                return Double.NaN;
            }

            return Duration;
        }

        
        public override string ToString()
        {
            return CalcDuration().ToString("0.######") + " seconds";
        }

        
        public double Duration
        {
            get
            {
                return CalcDuration();
            }
        }
    }
}
