using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Core.Caching
{
    #region 公共枚举.
    

    
    
    
    public enum CacheExpirationMode
    {
        #region 枚举成员.

        
        
        
        Absolute,

        
        
        
        Sliding,

        
        
        
        Never

        #endregion
    }

    


    

    
    
    
    public enum CacheStoreMode
    {
        #region 枚举成员.

        
        
        
        Add = 1,
        
        
        
        Replace = 2,
        
        
        
        Set = 3

        #endregion
    }

    
    #endregion
}
