using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using StructureMap;

namespace Clover.Web.Core
{
    
    
    
    public class PageBase : System.Web.UI.Page
    {

        public string permissionId = string.Empty;

        private IWebContext _webcontext;
        
        
        
        
        protected IWebContext WebContext
        {
            get {
                return _webcontext;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.Expires = 0;

            if (_webcontext.CurrentUser == null)
            {
                Utility.RunClientScript(this, "notlogin('会话过期，请重新登录')");
            }
            else
            {
                string privilegecode = PrivilegeCode();
                if (_webcontext.CurrentUser.AccountCode != SystemVar.AdminId && (privilegecode != string.Empty && !SystemVar.PermissionList.Contains(privilegecode)))
                    
                    Response.Redirect("~/controls/privilegedeiny.htm");
            }

            base.OnLoad(e);

        }

        private long ProgramBeginRunTime;
        private long programRunTime;

        public PageBase()
        {
            _webcontext = ObjectFactory.GetInstance<IWebContext>();

            this.ProgramBeginRunTime = System.Environment.TickCount; 
        }


        
        
        
        
        private void ProgramRunTime()
        {
            long ProgramEndRunTime = System.Environment.TickCount;
            programRunTime = ProgramEndRunTime - this.ProgramBeginRunTime;
            System.Web.UI.LiteralControl literal = new System.Web.UI.LiteralControl("<!-- 程序运行时间 " + programRunTime.ToString() + " 毫秒 -->");
            this.Page.Controls.Add(literal); 
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            
            base.Render(writer);
        }

        
        
        
        
        public void Info(string msg)
        {          
            Utility.ScriptAlert(this, msg);

        }

   

        
        
        
        
        private string PrivilegeCode()
        { 
            string privilegeCode = "";

            object[] objs = this.GetType().GetCustomAttributes(typeof(BasePageAttribute), true);

            if (objs.Length >= 1)
            {
                BasePageAttribute attr = objs[0] as BasePageAttribute;
                if(attr!=null)
                    privilegeCode = attr.PagePrivilegeCode;
            }

            return privilegeCode;
        }

        
        
        
        protected string GetEditReturnUrl()
        {
            return GetReturnUrl("Edit", "Main", "");
        }
        
        
        
        protected string GetSortReturnUrl()
        {
            return GetReturnUrl("Sort", "Main", "");
        }
        
        
        
        protected string GetSortTargetUrl()
        {
            return GetReturnUrl("Main", "Sort", "");
        }
        
        
        
        protected string GetEditTargetUrl()
        {
            return GetReturnUrl("Main", "Edit", "");
        }

        
        
        
        protected string GetReturnUrl(string currpageflag, string returnpageflag, string querystring)
        {

            string filename = VirtualPathUtility.GetFileName(this.Request.Url.AbsolutePath).Replace(currpageflag, returnpageflag);
            return VirtualPathUtility.Combine(VirtualPathUtility.GetDirectory(this.Request.Url.AbsolutePath), filename) + "?" + querystring;

        }
    }
}
