using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Clover.Config.Sys
{
	
	
    
	
	public class ConnectionStrings
	{

        private List<ConnectionString> paramslist = new List<ConnectionString>();
        
        
        
        [XmlArray("ConnectionStrings")]
        public ConnectionString[] ConnectionString
        {
            get
            {
                return paramslist.ToArray();
            }
            set
            {
                if (value == null)
                    return;

                paramslist.AddRange(value);
            }
        }

        public void AddFieldItem(ConnectionString con)
        {

            paramslist.Add(con);
        }

	    
	    
	    
	    
	    public ConnectionString getFieldItem(string id)
	    {
	        return paramslist.FirstOrDefault(param => param.Key == id);
	    }
	}
	
}
