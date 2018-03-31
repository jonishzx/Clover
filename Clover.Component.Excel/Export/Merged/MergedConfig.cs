using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Export.Merged
{
    
   public class MergedConfig
    {

       #region 合并分类枚举

       
       
       
       public static short RowMergedType = 0x1;

       
       
       
       public static short ColumnMergedType = 0x2;


       
       
       
       public static short AllMergedType = 0x3;

       #endregion

       #region 合并优先枚举

       
       
       
       public static short RowMergedTypePriority = 0x1;


       
       
       
       public static short ColumnMergedTypePriority = 0x2;

       #endregion

       
       
       
       public string AreaId { get; set; }
       
       
        
        
        public string FromRow { get; set; }

        
        
        
        public string FromColumn { get; set; }

        
        
        
        public string ToRow { get; set; }

        
        
        
        public string ToColumn { get; set; }

       
       
       
       
       
       
        public short MergedType { get; set; }

        
        
        
        
        
        
        public short MergedTypePriority { get; set; }

        
        
        
        public bool NeedValid { get; set; }

       
       
       
        public bool IgnoreEmpty { get; set; }
       
   }
}
