using Clover.Core.Domain;
using StructureMap;

namespace Clover.Web.Core
{
    /// <summary>
    /// µÇÂ¼session½Ó¿Ú
    /// </summary>
    [PluginFamily("Default")]
    public interface IUserSession
    {
        bool LoggedIn { get;}
        string Username { get; set; }
        IAccount CurrentUser { get; set; }
    }
}