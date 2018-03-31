using System;
using Clover.Core.Configuration;
using Clover.Core.Common;
using Clover.Web.Core;

using Clover.Config.WebSiteSetting;
using System.Text.RegularExpressions;

namespace Clover.Config
{
	
	
	
    public class WebSiteConfig : ConfigCenter<WebSiteManager, WebSiteConfigInfo>
	{

        
        
        
        
        
        public static bool CheckAdminIpAccess(out string messsage)
        {
           
            
            if (Config.Adminipaccess.Trim() != "")
            {
                string[] regctrl = StringHelper.SplitString(Config.Adminipaccess, "\n");

                if (!StringHelper.InIpArea(Utility.GetViewerIP(), regctrl))
                {
                    messsage = Config.AdminipaccessTip;
                    
                    return false;
                }
            }
            messsage = string.Empty;
            return true;
        }

        
        
        
        
        
        public static bool CheckPasswordStrategy(string password, out string message)
        {
            Regex MyRegex = new Regex(Config.PasswordRegex,
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                );

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("密码参数不能为空");

            if (!Config.UsePassWordStrategy || MyRegex.IsMatch(password))
            {
                message = string.Empty;
                return true;
            }
            else {
                message = Config.PasswordNotMatchMessage;
                return false;
            }
            
        }

        static WebSiteConfig()
        {           
        }

        
        
        
        
        
        public static bool CheckResIpAccess(out string messsage)
        {

            
            if (Config.Resipaccess.Trim() != "")
            {
                string[] regctrl = StringHelper.SplitString(Config.Resipaccess, "\n");

                if (!StringHelper.InIpArea(Utility.GetViewerIP(), regctrl))
                {
                    messsage = Config.ResipaccessTip;

                    return false;
                }
            }
            messsage = string.Empty;
            return true;
        }

        
        
        
        public static void SaveConfig(WebSiteConfigInfo info)
        {
            Config = info;
            SaveConfig();
        }

		
		
		
		
		public static int GetDefaultTemplateID()
		{
            return Config.Templateid;
    	}

		
		
		
		
		public static void SetIpDenyAccess(string denyipaccess)
		{
            Config.Ipdenyaccess = denyipaccess;
            SaveConfig();
		}	
	}
}
