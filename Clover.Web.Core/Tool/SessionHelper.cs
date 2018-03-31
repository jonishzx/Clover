using System;
using System.Collections.Generic;
using System.Web;

namespace Clover.Web.Core
{
    
    
    
    public class SessionHelper
    {
        
        
        
        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();        
        }

        
        
        
        
        
        public static bool ContainsInSession(string key)
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[key] != null)
                return true;
            return false;
        }

        
        
        
        
        public static void RemoveFromSession(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        
        
        
        
        
        public static string GetQueryStringValue(string key)
        {
            return HttpContext.Current.Request.QueryString.Get(key);
        }

        
        
        
        
        
        public static void SetInSession(string key, object value)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return;
            }
            HttpContext.Current.Session[key] = value;
        }

        
        
        
        
        
        public static T GetFromSession<T>(string key)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return default(T);
            }
            return (T)HttpContext.Current.Session[key];
        }

        
        
        
        
        
        public static object GetFromSession(string key)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return null;
            }

            return HttpContext.Current.Session[key];
        }

        
        
        
        
        
        public static void UpdateInSession(string key, object value)
        {
            if (HttpContext.Current == null)
                throw new ArgumentNullException(Properties.RS.Str_SessionHelper_HttpContextNullError);

            HttpContext.Current.Session[key] = value;           
        }
    }
}
