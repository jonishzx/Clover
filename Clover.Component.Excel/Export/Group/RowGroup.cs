using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel.Export.Group
{
    
    public class RowGroup:GroupBase
    {
        
        
        
        public int RowStartIndex { get; set; }

        
        
        
        public int RowEndIndex { get; set; }


        #region Overrides of GroupBase
        
        
        
        public override void DoGroup()
        {
            hsheet.GroupRow(RowStartIndex, RowEndIndex);
        }
        #endregion 
    }
}
