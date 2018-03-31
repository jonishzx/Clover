using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Export.Group
{
    
    public class ColumnGroup:GroupBase
    {
        
        
        
        public short ColumnStartIndex { get; set; }

        
        
        
        public short ColumnEndIndex { get; set; }
        
        
        #region Overrides of GroupBase
        
        
        
        public override void DoGroup()
        {
            hsheet.GroupColumn(ColumnStartIndex, ColumnEndIndex);
        }

        #endregion
    }
}
