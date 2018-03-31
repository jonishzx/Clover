
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Clover.I18n
{
    public class GettextResourceSet : System.Resources.ResourceSet
    {
        public GettextResourceSet(string filename)
            : base(new GettextResourceReader(File.OpenRead(filename)))
        {
        }

        public GettextResourceSet(Stream stream)
            : base(new GettextResourceReader(stream))
        {
        }

        public override Type GetDefaultReader()
        {
            return typeof(Clover.I18n.GettextResourceReader);
        }

        #region 特殊字符串处理

        
        
        
        
        
        
        
        
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

        
        
        
        
        
        
        
        
        
        public virtual String GetPluralString(String msgid, String msgidPlural, long n)
        {
            Object value = GetObject(msgid);
            if (value == null || value is String)
                return (String)value;
            else if (value is String[])
            {
                
                String[] choices = (String[])value;
                
                long index = PluralEval(n);
                return choices[index >= 0 && index < choices.Length ? index : 0];
            }
            else
                throw new InvalidOperationException("resource for \"" + msgid + "\" in " + GetType().FullName + " is not a string");
        }

        
        
        
        protected virtual long PluralEval(long n)
        {
            return (n == 1 ? 0 : 1);
        }

        
        
        
        public virtual System.Collections.ICollection Keys
        {
            get
            {
                return Table.Keys;
            }
        }
        #endregion
    }
}
