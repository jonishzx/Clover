using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;

namespace Clover.Component.Excel.Export.Group
{
    
    public class SheetGroup
    {
        
        
        
        public ISheet HssSheet { get; set; }

        
        
        
        public DataRow SheetData { get; set; }

        
        
        
        public int RowFrom { get; set; }

        
        
        
        public int CurrentRow { get; set; }

        
        
        
        public List<GroupConfig> GroupConfigs { get; set; }

        
        
        
        public void DoGroup()
        {
            foreach(GroupConfig config in GroupConfigs)
            {
                DoSigelGroupDispacher(config);
            }
        }

        
        
        
        
        private void DoSigelGroupDispacher(GroupConfig config)
        {
            if(config.GroupType==GroupConfig.RowGroupType)
            {
                DoSigelRowGroup(config);
            }
            else
            {
                DoSigelColumnGroup(config);
            }
        }

        
        
        
        
        private void DoSigelColumnGroup(GroupConfig config)
        {
            HssSheet.GroupColumn(config.FromColumn, config.ToColumn);
        }


        
        
        
        
        private void DoSigelRowGroup(GroupConfig config)
        {
            int rowFrom = CurrentRow+1;
            int rowTo = CurrentRow+1;
            int rowOffset=0;
            int rowEnd = RowFrom;
            string childRows;
            if(config.FromRow>-1)
            {
                rowFrom = config.FromRow;
            }
            if (config.ToRow > -1)
            {
                rowTo = config.ToRow;
            }
            if(!string.IsNullOrEmpty(config.ColumnField))
            {
                childRows = SheetData[config.ColumnField] + "";
                rowOffset = GetMaxOffet(childRows);
            }
            rowOffset += RowFrom;
            rowOffset += 1;
            rowTo = Math.Max(rowTo, rowOffset);
            if(rowFrom<rowTo)
            {
                HssSheet.GroupRow(rowFrom, rowTo);
            }
        }


        
        
        
        
        
        private int GetMaxOffet(string childIndexs)
        {
            string[] childArray = childIndexs.Split(',');
            int lastChildPos = 0;
            foreach(string childPos in childArray)
            {
                int tempPos = 0;
                if(int.TryParse(childPos,out tempPos))
                {
                    lastChildPos = Math.Max(lastChildPos, tempPos);
                }
            }
            return lastChildPos;
        }

    }
}
