using System;
using System.Collections.Generic;
using Clover.Core.Logging;
using System.Diagnostics;
using Clover.Core.Common;
using Clover.Core.Reflection;

namespace Clover.Core.Caching
{
    

    
    
    
    [DebuggerDisplay(@"Key = {key}, CacheInfo = {cacheInfo}")]
    public class CacheItem :
        IDumpable
    {
        #region 公共方法.
       

        
        
        
        
        
        
        public CacheItem(
            string key,
            object value,
            CacheSetting cacheInfo)
        {
            this.key = key;
            this.value = value;
            this.cacheInfo = cacheInfo;
        }

        
        
        
        
        public bool CheckIfRemove()
        {
            if (cacheInfo == null)
            {
                return false;
            }
            else
            {
                return ExpiresAfter <= DateTime.Now;
            }
        }

        
        #endregion

        #region 公共属性.
        public string Key
        {
            get
            {
                return key;
            }
        }

        
        
        
        
        public DateTime LastAccessTime
        {
            get
            {
                return lastAccessed;
            }
        }


        
        
        
        
        public CacheSetting CacheSetting
        {
            get
            {
                return cacheInfo;
            }
        }


        
        
        
        
        public object Value
        {
            get
            {
                lastAccessed = DateTime.Now;
                return value;
            }
        }

        
        
        
        
        public DateTime ExpiresAfter
        {
            get
            {
                if (cacheInfo == null)
                {
                    return DateTime.MaxValue;
                }
                else
                {
                    switch (cacheInfo.ExpirationMode)
                    {
                        case CacheExpirationMode.Absolute:
                            return cacheInfo.AbsoluteExpiration;
                        case CacheExpirationMode.Sliding:
                            return lastAccessed.Add(cacheInfo.SlidingExpiration);
                        default:
                            return DateTime.MaxValue;
                    }
                }
            }
        }

        
        
        
        
        public CacheItemGroup Group
        {
            get
            {
                return cacheInfo.Group;
            }
        }

        #endregion

        #region 私有变量.

        private string key;
        private object value;
        private CacheSetting cacheInfo;

        private DateTime dateAdded = DateTime.Now;
        private DateTime lastAccessed = DateTime.Now;

        #endregion

        #region 堆存储 members.

        
        
        
        
        
        
        
        
        
        
        
        
        public string Dump(
            int indentLevel,
            bool deep)
        {
            DumpBuilder d = new DumpBuilder(
                indentLevel,
                deep,
                GetType());

            d.AddLine(@"添加的时间", this.dateAdded);
            d.AddLine(@"(键)Key", this.key);
            d.AddLine(@"(值)Value", this.value);
            d.AddLine(@"最后的访问时间", this.lastAccessed);
            d.AddLine(@"过期的时间", this.ExpiresAfter);

            return d.ToString();
        }

        
        
        
        
        
        
        
        
        
        public string Dump()
        {
            return Dump(0, false);
        }

        #endregion
    }

    
}
