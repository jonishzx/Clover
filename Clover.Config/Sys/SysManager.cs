using System;
using System.Text;
using System.Web;
using System.IO;

using Clover.Core.Configuration;


namespace Clover.Config.Sys
{

    
    
    
    public sealed class SysManager : DefaultConfigFileManager<SysConfigInfo>
    {

           
        
        
        const string CONST_DEFAULTPATH = "~/config/sys.config";

         
        
        
        public SysManager()
            : this(CONST_DEFAULTPATH)
        {            
        }

        
        
        
        public SysManager(string configfilepath)
            : base(configfilepath)
        {
        
        }        
    }
}
