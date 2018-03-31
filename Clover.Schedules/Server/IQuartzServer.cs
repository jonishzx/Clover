using System;

namespace Clover.Schedules.Server
{
    
    
    
    public interface IQuartzServer
    {
        
        
        
        
        void Initialize();

        
        
        
        void Start();

        
        
        
        void Stop();

        
        
        
        void Pause();

        
        
        
        void Resume();
    }
}