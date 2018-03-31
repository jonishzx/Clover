using System;
namespace Clover.Core.Configuration
{
    
    
    
    
    public interface IConfigFileManager<T> where T : IConfigInfo
    {
        T ConfigInfo { get; set; }
        T LoadConfig();
        bool SaveConfig();
        event Clover.Core.Base.TAction<T> ChangeConfigEvent;
    }
}
