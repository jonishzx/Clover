using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Core.Domain.Impl
{
    public class AppContext : IAppContext
    {
        public string AccountID
        {
            get { throw new NotImplementedException(); }
        }

        public IAccount CurrentUser
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int GetIntParm(string pname)
        {
            throw new NotImplementedException();
        }

        public int GetIntParm(string pname, int defaultVal)
        {
            throw new NotImplementedException();
        }

        public string GetParm(string pname)
        {
            throw new NotImplementedException();
        }

        public string GetParm(string pname, string defaultVal)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetContextParams()
        {
            throw new NotImplementedException();
        }

        public bool LoggedIn
        {
            get { throw new NotImplementedException(); }
        }

        public string Username
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string LanguageCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
