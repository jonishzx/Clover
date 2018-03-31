
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.I18n
{
    
    
    
    public interface IGettextParserRequestor
    {
        
        
        
        void Handle(string key, string[] value);

        
        
        
        void Handle(string key, string value);
    }

    
    
    
    public class DictionaryGettextParserRequestor : Dictionary<String, object>, IGettextParserRequestor
    {
        #region IGettextParserRequestor Members

        public void Handle(string key, string value)
        {
            this[key] = value;
        }

        public void Handle(string key, string[] value)
        {
            this[key] = value;
        }
        #endregion
    }
}
