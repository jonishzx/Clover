using System.Collections.Generic;
using System.Xml.Serialization;

using Clover.Core.Collection;
using Clover.Core.Configuration;

namespace Clover.Config.Menu
{
    
    
    
    [XmlRoot("MenuItems")]
    public class MenuItems : IConfigInfo
    {

        private List<MenuItem> paramslist = new List<MenuItem>();
        private Tree<MenuItem> _tree;
        private static object lk = new object();

        
        
        
        [XmlElement("MenuItem")]
        public MenuItem[] OMenuItems
        {
            get
            {
                return paramslist.ToArray();
            }
            set
            {
                if (value == null)
                    return;

                paramslist.Clear();
                paramslist.AddRange(value);
            }
        }

        
        
        
        
        public Tree<MenuItem> GetTree()
        {
            if (_tree == null)
            {
                lock (lk)
                {
                    if (_tree == null)
                        PInitTree();
                }
            }

            return _tree;
        }

        
        
        
        
        public Tree<MenuItem> InitTree()
        {
            lock (lk)
            {
                PInitTree();
            }
     
            return _tree;
        }

        private void PInitTree()
        {
            List<MenuItem> copylist = new List<MenuItem>(paramslist);

            
            List<string> hiddenroots = new List<string>(10);
            copylist.ForEach(delegate(MenuItem it)
            {
                if(string.IsNullOrEmpty(it.ParentId) && !it.Visible)
                    hiddenroots.Add(it.Id);
            });

            
            copylist.RemoveAll(delegate(MenuItem it)
            {
                return !it.Visible || hiddenroots.Contains(it.ParentId);
            });

            _tree = new Tree<MenuItem>(copylist);
        }
    }
}
