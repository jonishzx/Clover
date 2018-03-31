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
    
    
    
    public class TokenModule : IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.PostAcquireRequestState += new EventHandler(token_PostAcquireRequestState);

        }

        #endregion

        private void token_PostAcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            
            if (app.Context.Handler is Page)
            {
                (app.Context.Handler as Page).PreRenderComplete += new EventHandler(PreRenderComplete);
            }
        }

        private void PreRenderComplete(object sender, EventArgs arg)
        {
            Page page = (sender as Page);
            if (page == null) return;
            if (page.Form == null) return;
            HttpSessionState session = HttpContext.Current.Session;
            if (session[TokenUtil.TokenKey_InSessions] != null)
            {
                
                HiddenField ctrl = new HiddenField();
                ctrl.ID = TokenUtil.TokenKey_InForm;
                ctrl.Value = session[TokenUtil.TokenKey_InSessions].ToString();
#if CSHARP40 
                ctrl.ClientIDMode = ClientIDMode.Static;
#endif
                page.Form.Controls.Add(ctrl);
            }
        }
    }
}
