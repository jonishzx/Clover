using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Clover.Web.Core
{
    
    
    
    public sealed class CookieHelper
    {
        
        
        
        
        
        
        public static void CreateCookieValue(string cookieName, string cookieValue, DateTime cookieTime)
        {
            CreateCookieValue(HttpContext.Current.Response, cookieName, cookieValue, cookieTime);
        }

        
        
        
        
        
        
        
        
        public static void CreateCookieValue(string cookieName, string subCookieName, string cookieValue, string subCookieValue, DateTime cookieTime)
        {
            CreateCookieValue(HttpContext.Current.Response, cookieName,subCookieName, cookieValue, subCookieValue, cookieTime);
        }


        
        
        
        
        
        public static string GetCookieValue(string cookieName)
        {
            return GetCookieValue(HttpContext.Current.Request, cookieName);
        }

        
        
        
        
        public static void RemoveCookieValue(string cookieName)
        {
            RemoveCookieValue(HttpContext.Current.Response, cookieName);
        }

        
        
        
        
        
        
        
        public static void CreateCookieValue(HttpResponse rps, string cookieName, string cookieValue, DateTime cookieTime)
        {
            HttpCookie cookie = rps.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
            }

            cookie.Value = cookieValue;
            cookie.Expires = cookieTime;

            rps.Cookies.Add(cookie);
        }

        
        
        
        
        
        public static void CreateCookieValue(HttpResponse rps, string cookieName, string cookieValue, string subCookieName, string subCookieValue, DateTime cookieTime)
        {
            HttpCookie cookie = rps.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
            }

            cookie.Value = cookieValue;
            cookie[subCookieName] = subCookieValue;
            cookie.Expires = cookieTime;
            rps.Cookies.Add(cookie);
        }

        
        
        
        
        
        
        public static string GetCookieValue(HttpRequest rp, string cookieName)
        {
            string cookieValue = String.Empty;
            HttpCookie cookie = rp.Cookies[cookieName];
            if (null == cookie)
            {
                cookieValue = String.Empty;
            }
            else
            {
                cookieValue = cookie.Value;
            }
            return cookieValue;
        }

        
        
        
        
        
        public static void RemoveCookieValue(HttpResponse rps, string cookieName)
        {
            string dt = "1900-01-01 12:00:00";
            CreateCookieValue(rps, cookieName, String.Empty, Convert.ToDateTime(dt));
        }
    }
}
