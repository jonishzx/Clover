using System;
using System.Xml.Serialization;

using Clover.Web.Core;
using Clover.Core.Collection;

namespace Clover.Config.Menu
{
    
    
    
    
    
    
    public class MenuItem : ISNode, IComparable<MenuItem>
    {
        #region ISNode 成员

        
        
        
        [XmlAttribute("Id")]
        public string Id { get; set; }

        
        
        
        [XmlAttribute("Name")]
        public string Name { get; set; }

        
        
        
        [XmlAttribute("ParentId")]
        public string ParentId { get; set; }

        
        
        
        [XmlAttribute("ViewOrder")]
        public int ViewOrder { get; set; }

        string _url;
        
        
        
        [XmlAttribute("Url")]
        public String Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = Utility.ConvertAbsoulteUrl(value);
            }
        }


        #endregion

   
        
        
        
        [XmlAttribute("Target")]
        public String Target
        {
            get;
            set;
        }

        
        
        
        [XmlAttribute("IconCls")]
        public String IconCls;
 
        
        
        
        [XmlAttribute("Permission")]
        public String Permission;

        
        
        
        [XmlAttribute("Visible")]
        public bool Visible = true;


        #region IComparable<T> 成员

        public int CompareTo(MenuItem other)
        {
            return this.ViewOrder - other.ViewOrder;
        }

        #endregion
    }
}
