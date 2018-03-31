using System;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;

namespace Clover.Web.Core
{
    
    
    
    public class TokenUtil : IFormToken
    {
        public const string TokenKey_InSessions = "tokenkey";
        public const string TokenKey_InForm = "Param_TokenKey";

        public string GetFormToken() { 
           
            return Utility.GetFormParm(TokenKey_InForm);
        }

        
        
        
        
        public bool IsValidToken()
        {
            HttpContext context = HttpContext.Current;
            HttpSessionState session = context.Session;
            if (session == null) return false;
            if (session[TokenKey_InSessions] == null) return false;
            string formtoken = context.Request.Form[TokenKey_InForm];
           
            if (context.Handler is Page)  
            {
                Control ctrl = null;
                Utility.FindControlById((context.Handler as Page).Controls, TokenKey_InForm, ref ctrl);

                if (ctrl != null)
                {
                    var hiddenfield = ctrl as HiddenField;
                    formtoken = hiddenfield.Value;
                }
            }

            return !string.IsNullOrEmpty(formtoken) && SessionHelper.GetFromSession<string>(TokenKey_InSessions).Equals(formtoken);
        }

        public void CreateToken()
        {
            HttpContext context = HttpContext.Current;

            string distinctkey = Guid.NewGuid().ToString();

            
            HttpSessionState session = context.Session;
            if (HttpContext.Current.Session == null) return;
            SessionHelper.SetInSession(TokenKey_InSessions, distinctkey);
        }

        
        
        
        public void ClearToken() {
            SessionHelper.RemoveFromSession(TokenKey_InSessions);
        }
    }
}
