using System;
using System.Collections.Generic;
using System.Web;
using Clover.Core.Domain;
using StructureMap;

namespace Clover.Web.Core
{
    [PluginFamily("Default")]
    public interface IWebContext : IAppContext
    {
        void ClearSession();
        bool ContainsInSession(string key);
        System.Web.HttpFileCollection Files { get; }
        int PageNumber { get; }
        void RemoveFromSession(string key);
        string RootUrl { get; }
      
    }
}