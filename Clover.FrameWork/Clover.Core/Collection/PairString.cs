namespace Clover.Core.Collection
{
    #region ���õ������ռ�.
    

    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    
    #endregion

    

    
    
    
    
    [Serializable]
    [ComVisible(false)]
    public class StringPair :
        Pair<string, string>
    {
        #region ��������.
        

        
        
        
        public StringPair()
            :
            base()
        {
        }

        
        
        
        
        public StringPair(
            string name)
            :
            base(name)
        {
        }

        
        
        
        
        
        public StringPair(
            string name,
            string val)
            :
            base(name, val)
        {
        }

        
        #endregion
    }

    
}