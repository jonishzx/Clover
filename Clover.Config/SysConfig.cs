using System;
using Clover.Core.Configuration;
using Clover.Config.Sys;

namespace Clover.Config.Sys
{
	
	
	
    public class SysConfig : ConfigCenter<SysManager, SysConfigInfo>
    {       
        
        
        
        
        
        public static ConnectionString GetConnecting(string key)
        {          
            return Config.ConnectionStrings.getFieldItem(key);
        }

       
	}
}
