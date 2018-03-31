using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Clover.Core.Domain;
using StructureMap;

namespace Clover.Web.Core.Impl
{
    /// <summary>
    /// 用户会话
    /// </summary>
    [Pluggable("Default")]
    public class UserSession : IUserSession
    {
        private IWebContext _webContext; 

        public UserSession()
        {
            _webContext = ObjectFactory.GetInstance<IWebContext>();
        }

        public bool LoggedIn
        {
            get
            {
                return _webContext.LoggedIn;
            }
        }

        public IAccount CurrentUser
        {
            get
            {
                return _webContext.CurrentUser;
            }
            set
            {
                _webContext.CurrentUser = value;
            }
        }

        public string Username
        {
            get
            {
                return _webContext.Username;
            }

            set
            {
                _webContext.Username = value;
            }
        }
    }
}
