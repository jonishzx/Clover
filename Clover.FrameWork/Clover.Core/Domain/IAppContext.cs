using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Clover.Core.Domain
{
    
    
    
    public interface IAppContext
    {
        string AccountID { get; }
        Clover.Core.Domain.IAccount CurrentUser { get; set; }
        int GetIntParm(string pname);
        int GetIntParm(string pname, int defaultVal);
        string GetParm(string pname);
        string GetParm(string pname, string defaultVal);
        Dictionary<string, string> GetContextParams();
        bool LoggedIn { get; }
        string Username { get; set; }
        string LanguageCode { get; set; }
    }
}
