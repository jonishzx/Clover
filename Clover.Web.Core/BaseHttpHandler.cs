using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Clover.Web.Core;

using StructureMap;

namespace Clover.Web.Core
{
    
    
    
    public class BaseHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private IWebContext _webcontext;

        
        
        
        protected IWebContext WebContext
        {
            get
            {
                return _webcontext;
            }
        }

        public BaseHttpHandler()
        {
            _webcontext = ObjectFactory.GetInstance<IWebContext>();
        }

        #region IHttpHandler 成员

        public virtual bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            
        }
        #endregion
    }
}
