namespace Clover.Core.Collection
{
    #region 引用
    

    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    
    #endregion

    

    
    
    
    
    [Serializable]
    [DebuggerDisplay(@"Name = {name}, Value = {value}")]
    [ComVisible(false)]
    public class Pair<K, V>
    {
        #region 公共方法.
        

        
        
        
        public Pair()
        {
        }

        
        
        
        
        public Pair(
            K name)
        {
            Name = name;
        }

        
        
        
        
        
        public Pair(
            K name,
            V val)
        {
            Name = name;
            Value = val;
        }

        
        
        
        
        
        
        
        
        public override string ToString()
        {
            if (Name == null)
            {
                return null;
            }
            else
            {
                return Name.ToString();
            }
        }

        
        #endregion

        #region 公共属性.
        


        
        
        
        
        public K Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        
        
        
        
        public V Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

  
        
        #endregion

        #region 私有方法.
        

        
        
        
        
        
        
        private int DoCompare(
            object a,
            object b)
        {
            IComparable x = a as IComparable;
            IComparable y = b as IComparable;

            if (x == null && y == null)
            {
                return 0;
            }
            else if (x != null)
            {
                return x.CompareTo(y);
            }
            else if (y != null)
            {
                return -y.CompareTo(x);
            }
            else
            {
                return 0;
            }
        }

        
        #endregion

        #region 私有变量.
        

        private K name;
        private V value;

        
        #endregion
    }

    

    
    
    
    
    [Serializable]
    [ComVisible(false)]
    public class Pair :
        Pair<object, object>
    {
        #region 公共方法.
        

        
        
        
        public Pair()
            :
            base()
        {
        }

        
        
        
        
        public Pair(
            object name)
            :
            base(name)
        {
        }

        
        
        
        
        
        public Pair(
            object name,
            object val)
            :
            base(name, val)
        {
        }

        
        #endregion
    }

    
}