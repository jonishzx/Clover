
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Globalization;

namespace Clover.I18n
{
    public class DatabaseResourceSet : ResourceSet
    {
        internal DatabaseResourceSet(string dsn, CultureInfo culture)
            : base (new DatabaseResourceReader(dsn, culture))
        {
        }

        internal DatabaseResourceSet(string dsn, CultureInfo culture, string sp)
            : base(new DatabaseResourceReader(dsn, culture, sp))
        {
        }

        public override Type GetDefaultReader()
        {
            return typeof(DatabaseResourceReader);
        }

        
        
        
        
        
        
        
        
        public override String GetString(String msgid)
        {
            return GetString(msgid, false);
        }

        
        
        
        
        
        
        
        
        
        
        
        public override String GetString(String msgid, bool ignoreCase)
        {
            Object value = GetObject(msgid, ignoreCase);
            if (value == null || value is String)
                return (String)value;
            else if (value is String[])
                
                return ((String[])value)[0];
            else
                throw new InvalidOperationException("resource for \"" + msgid + "\" in " + GetType().FullName + " is not a string");
        }

    }

}
