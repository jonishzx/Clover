using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Web;

namespace Clover.Web.Core
{
    
    
    
    public class SystemBase
    {
        
        
        
        public static List<string> OnlineStaffList = new List<string>();

     
        #region ctor
        static SystemBase()
        {

           
        }
        #endregion

        
        
        
        
        
        public static string GetPagePermissionId(string path,DataTable dt)

        {
            string rtn = null;            
            foreach (DataRow dr in dt.Rows)
            {
                if (Regex.IsMatch(dr["url"].ToString(),@"~/(([/.A-Za-z0-9_@])*)"))
                {
                    string pureurl = Regex.Match(dr["url"].ToString(), @"~/(([/.A-Za-z0-9_@])*)").Value.Replace("?", "");

                    if (VirtualPathUtility.ToAbsolute(pureurl) == path)
                    {
                        rtn = dr["permission"].ToString();
                        break;
                    }
                }

            }


            return rtn;
        }

        
        
        
        
        public static void CheckHasPrivilege(HttpContext context,DataTable dt)
        {
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        
        
        

        

        
        

        

        
            
        
        

        
        
        
        
        

        
        

        

        

        
        
        
        
        
        
        

        
        
        
        
        
        

        

        
        
        
        
        
        
    }
}
