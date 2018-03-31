using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Clover.Web.Core
{
    
    
    
    [AttributeUsage( AttributeTargets.Class,Inherited=true)]
    public class BasePageAttribute:Attribute
    {
        string m_strPagePrivilegeCode;

        
        
        
        public string PagePrivilegeCode
        {
            get {
                return m_strPagePrivilegeCode;
            }
            set{
                m_strPagePrivilegeCode = value;
            }
        }

        public BasePageAttribute(string pagepcode)
        {
            this.m_strPagePrivilegeCode = pagepcode;
        }

        public BasePageAttribute()
        {

        }
    }

}