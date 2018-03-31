using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Configuration;


namespace Clover.Web.Core
{

    
    
    
    public class SystemVar
    {
        
        
        
        public static int CurrPageIndex
        {
            get
            {
                return Utility.GetIntParm("p", 1);
            }
        }

        
        
        
        public const string AdminId = "90EC66D8-F9A3-40DC-B532-7E350BDF3169";

        
        
        
        public const string AdminRoleCode = "Administrator";

        
        
        
        public const string PMAdminRoleCode = "PMAdministrator";

        
        
        
        public const int AdminDepIdId = 1000;

      

		
        
        
        public static string UserRole
        {
            get { return Utility.GetSessionObject("UserRole"); }
            set { Utility.SetSessionObject("UserRole", value); }
        }

        

        
        
        
        public static string Permission
        {
            get { return Utility.GetSessionObject("Permission"); }
            set {
                Utility.SetSessionObject("Permission", value);

                permissionlist.Clear();

                string[] permissions = SystemVar.Permission.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);

                permissionlist.AddRange(permissions);
            }
        }

        static List<string> permissionlist = new List<string>();
        
        
        
        public static List<string> PermissionList
        {
            get { return permissionlist; }
       
        }

        
        
        
        public static string ValidCode
        {
            get {

                if (HttpContext.Current.Session["CheckCode"] != null)
                    return HttpContext.Current.Session["CheckCode"].ToString();
                else
                    return "";
            }
        }

     
    }      
}