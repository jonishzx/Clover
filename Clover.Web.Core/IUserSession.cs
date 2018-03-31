using Clover.Core.Domain;
using StructureMap;

namespace Clover.Web.Core
{
    /// <summary>
    /// ��¼session�ӿ�
    /// </summary>
    [PluginFamily("Default")]
    public interface IUserSession
    {
        bool LoggedIn { get;}
        string Username { get; set; }
        IAccount CurrentUser { get; set; }
    }
}