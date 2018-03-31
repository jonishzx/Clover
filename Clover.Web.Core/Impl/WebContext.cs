using System;
using System.Collections.Generic;
using System.Web;
using Clover.Core.Domain;
using StructureMap;

namespace Clover.Web.Core.Impl
{
    
    
    
    [Serializable]
    [Pluggable("Default")]
    public class WebContext : IWebContext
    {       
        public HttpFileCollection Files
        {
            get
            {
                if (HttpContext.Current.Request.Files != null)
                    return HttpContext.Current.Request.Files;
                else
                    return null;
            }
        }

       
       
       
        public Int32 PageNumber
        {
            get
            {
                return GetIntParm("p",1);
            }
        }
            
        public string RootUrl
        {
            get
            {
                string result;

                string Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
                if (Port == null || Port == "80" || Port == "443")
                    Port = "";
                else
                    Port = ":" + Port;

                string Protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
                if (Protocol == null || Protocol == "0")
                    Protocol = "http://";
                else
                    Protocol = "https://";

                result = Protocol + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] +
                    Port + HttpContext.Current.Request.ApplicationPath;

                return result;
            }
        }      

        public string AccountID
        {
            get
            {
                if (ContainsInSession("CurrentUser"))
                {
                    IAccount user = GetFromSession<IAccount>("CurrentUser");
                    return user.UniqueId;
                }

                return string.Empty;
            }
        }
    
        
        
        
        public virtual IAccount CurrentUser
        {
            get
            {
                if(ContainsInSession("CurrentUser"))
                {
                    return GetFromSession("CurrentUser") as IAccount;
                }

                return null;
            }
            set
            {
                SetInSession("CurrentUser", value);
                if(value!=null)
                    SystemVar.Permission = value.Permission;
            }
        }

        
        
        
        public virtual string LanguageCode
        {
            get
            {
                if (ContainsInSession("LanguageCode"))
                {
                    return GetFromSession("LanguageCode").ToString();
                }

                
                return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            }
            set
            {
                SetInSession("LanguageCode", value);           
            }
        }


        
        
        
        public string Username
        {
            get
            {             
                IAccount curruser = CurrentUser;
                if (curruser != null)
                    return CurrentUser.UserName;

                return string.Empty;
            }

            set
            {
                CurrentUser.UserName = value;
            }
        }
        
        
        
        
        public bool LoggedIn
        {
            get
            {
                
                if(CurrentUser != null && CurrentUser.UniqueId != Clover.Core.Domain.AnonymousAccount.AnonymousID)
                {
                    return true;                
                }
                else
                {
                    return false;
                }
            }   
        }

        #region session

        public void ClearSession()
        {
            SessionHelper.ClearSession();
        }

        public bool ContainsInSession(string key)
        {
            return SessionHelper.ContainsInSession(key);
        }

        public void RemoveFromSession(string key)
        {
            SessionHelper.RemoveFromSession(key);
        }

        private void SetInSession(string key, object value)
        {
            SessionHelper.SetInSession(key, value);
        }

        private object GetFromSession(string key)
        {
            return SessionHelper.GetFromSession(key);
        }

        private T GetFromSession<T>(string key)
        {
            return SessionHelper.GetFromSession<T>(key);
        }

        private void UpdateInSession(string key, object value)
        {
            SessionHelper.UpdateInSession(key, value);
        }
        #endregion

        #region Request
        public string GetParm(string pname, string defaultVal)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[pname]))
                return HttpContext.Current.Request.QueryString[pname];
            else
                return defaultVal;
        }

        public string GetParm(string pname)
        {
            return GetParm(pname, "");
        }

        public int GetIntParm(string pname)
        {
            return GetIntParm(pname, 0);
        }

        public int GetIntParm(string pname, int defaultVal)
        {
            string val = GetParm(pname);
            int rtn = 0;

            int.TryParse(val, out rtn);
            if (rtn == 0)
                rtn = defaultVal;
            return rtn;
        }
        #endregion       

        public Dictionary<string, string> GetContextParams() {
            Dictionary<string, string> list = new Dictionary<string, string>(10);

            
            foreach (var key in HttpContext.Current.Request.QueryString.AllKeys)
            {
                if (key == null)
                    continue;

                var mkey = key.ToLower();
                if (list.ContainsKey(mkey))
                    list[mkey] = HttpContext.Current.Request.QueryString[key];
                else
                    list.Add(mkey, HttpContext.Current.Request.QueryString[key]);
            }
            
            foreach (var key in HttpContext.Current.Request.Form.AllKeys)
            {
                if (key == null)
                    continue;

                var mkey = key.ToLower();
                if (list.ContainsKey(mkey))
                    list[mkey] = HttpContext.Current.Request.Form[key];
                else
                    list.Add(mkey, HttpContext.Current.Request.Form[key]);
               
            }

            
            list.Add("#env.CurrentUser.UserId#", AccountID);

            
            if (CurrentUser.GroupIds != null && CurrentUser.GroupIds.Count > 0)
            {
                List<string> groupList = CurrentUser.GroupIds as List<string>;
                string groupids = "'" + string.Join("','", groupList.ToArray()) + "'";
                list.Add("#env.CurrentUser.GroupId#", groupids);
            }

            return list;
        }
    }
}
