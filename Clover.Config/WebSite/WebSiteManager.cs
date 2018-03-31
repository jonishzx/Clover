using System;
using System.Text;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

using Clover.Core.Configuration;

namespace Clover.Config.WebSiteSetting
{
    
    
    
    public sealed class WebSiteManager : DefaultConfigFileManager<WebSiteConfigInfo>
    {
        
        
        
        const string CONST_DEFAULTPATH = "~/config/website.config";

         
        
        
        public WebSiteManager()
            : this(CONST_DEFAULTPATH)
        {            
            
        }

        
        
        
        public WebSiteManager(string configfilepath)
            : base(configfilepath)
        {
          
        }            
    }
}

