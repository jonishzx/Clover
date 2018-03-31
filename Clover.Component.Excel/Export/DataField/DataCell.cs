using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Export
{
    public class DataCell : DataFieldBase
    {
        #region Property
        
        
        
        public int Row { get; protected set; }
        
        
        
        public int Column { get; protected set; }

        private string _excelRowNum;
        
        
        
        public string ExcelRowNum 
        {
            get
            { 
                if (string.IsNullOrEmpty(_excelRowNum))
                    _excelRowNum = GetExcelRowNum();
                return _excelRowNum;
            }
        }

        private string _excelCellName;
        
        
        
        public string ExcelCellName
        {
            get
            {
                if (string.IsNullOrEmpty(_excelCellName))
                    _excelCellName = ExcelColumnNum + ExcelRowNum;
                return _excelCellName;
            }
        }
        


        #endregion

        #region ctor

        public DataCell(XmlNode cellNode, Area area) 
            : base(cellNode, area)
        {
            Row = XmlUtility.getNodeAttributeIntValue(cellNode, "row");
            Column = XmlUtility.getNodeAttributeIntValue(cellNode, "col");
        }

        
        
        
        
        private string GetExcelRowNum()
        {
            
            int num = Area.TopRowSpan;
            
            num += Area.Row;
            
            num += this.Row;
            return (num + 1).ToString(); 
        }

        
        
        
        
        protected override string GetExcelColumnNum()
        {
            int sum = (Column + Area.Column + Area.Worksheet.Column) + 1;
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
