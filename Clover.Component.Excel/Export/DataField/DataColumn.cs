using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.Reflection;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public class DataColumn : DataFieldBase
    {
        #region Property
        
        
        
        public int Offset { get; set; }

        private bool isSetMergeColumn = false;
        private bool _isMergeColumn = false;
        
        
        
        public bool IsMergeColumn 
        {
            get
            {
                if (!isSetMergeColumn)
                {
                    isSetMergeColumn = true;
                    _isMergeColumn = (this.Area as RepeatArea).MergeColumns.Contains(this.ID);
                }
                return _isMergeColumn;
            }
        }

        #endregion

        #region ctor

        public DataColumn(XmlNode dataColumnNode, Area area) 
            : base(dataColumnNode, area)
        {
            Offset = XmlUtility.getNodeAttributeIntValue(dataColumnNode, "offset");
        }
        
        
        
        
        protected override string GetExcelColumnNum()
        {
            int sum = (Offset + Area.Column + Area.Worksheet.Column) + 1;
            int value1 = sum / 26;
            int value2 = sum % 26;
            if (value2 == 0)
            {
                value1--;
                value2 = 26;
            }
            return (value1 == 0 ? "" : ((char)(value1 + 64)).ToString()) + ((char)(value2 + 64)).ToString();
        }

        #endregion
    }
}
