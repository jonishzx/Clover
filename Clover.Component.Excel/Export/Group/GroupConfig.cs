using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Export.Group
{
    
    public class GroupConfig
    {
        #region 行组合枚举

        
        
        
        public static short RowGroupType = 0x1;

        
        
        
        public static short ColumnGroupType = 0x2;

        #endregion

        
        
        
        public short GroupType { get; set; }

        
        
        
        public string AreaId { get; set; }

        
        
        
        public string ColumnField { get; set; }

        #region 行组合属性

        
        
        
        public int FromRow { get; set; }

        
        
        
        public int ToRow { get; set; }

        #endregion

        #region 列组合配置

        
        
        
        public short FromColumn { get; set; }

        
        
        
        public short ToColumn { get; set; }
        
        #endregion


    }
}
