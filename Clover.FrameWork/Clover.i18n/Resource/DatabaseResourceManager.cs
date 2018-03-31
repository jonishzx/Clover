
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Configuration;

namespace Clover.I18n
{
    public class DatabaseResourceManager : System.Resources.ResourceManager
    {
        private string dsn;
        private string sp;

        public DatabaseResourceManager()
            : base()
        {
            this.dsn = ConfigurationManager.AppSettings["Gettext.ConnectionString"] ?? ConfigurationManager.ConnectionStrings["Gettext"].ConnectionString;
            ResourceSets = new System.Collections.Hashtable();
        }

        public DatabaseResourceManager(string storedProcedure)
            : this()
        {
            this.sp = storedProcedure;
        }

        
        public DatabaseResourceManager(string name, string path, string fileformat)
            : this()
        {
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            DatabaseResourceSet rs = null;
            if (culture == null || culture.Equals(CultureInfo.InvariantCulture)) return null;

            if (ResourceSets.Contains(culture.Name))
            {
                rs = ResourceSets[culture.Name] as DatabaseResourceSet;
            }
            else
            {
                lock (ResourceSets)
                {
                    
                    if (ResourceSets.Contains(culture.Name))
                    {
                        rs = ResourceSets[culture.Name] as DatabaseResourceSet;
                    }
                    else
                    {
                        rs = new DatabaseResourceSet(dsn, culture, sp);
                        ResourceSets.Add(culture.Name, rs);
                    }
                }
            }
            
            return rs; 
        }

        
        
        
        
        
        
        
        public override String GetString(String msgid, CultureInfo culture)
        {
            DatabaseResourceSet rs = InternalGetResourceSet(culture, true, true) as DatabaseResourceSet;
            {
                String translation = rs.GetString(msgid);
                if (translation != null)
                    return translation;
            }
            
            return msgid;
        }

        
        
        
        
        
        
        
        
        public override String GetString(String msgid)
        {
            return GetString(msgid, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }
    }
}
