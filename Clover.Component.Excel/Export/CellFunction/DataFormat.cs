using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public class DataFormat
    {
        
        
        
        
        public string DataFormatString { get; set; }
        
        
        
        private bool isInitDataFormat { get; set; }
        
        
        
        protected short SDataFormat { get; set; }

        public CellStyle CellStyle{get;set;}

        public DataFormat(XmlNode dataFormatNode, CellStyle cellStyle)
        {
            DataFormatString = XmlUtility.getNodeAttributeStringValue(dataFormatNode, "dataformatstring", "");
            isInitDataFormat = false;
            SDataFormat = -1;
            CellStyle = cellStyle;
        }

        
        
        
        
        
        public void SetDataFormat(IWorkbook hssfWorkbook)
        {
            SetDataFormat(CellStyle.ICellStyle, hssfWorkbook);
        }

        
        
        
        
        
        public void SetDataFormat(ICellStyle hssfCellStyle, IWorkbook hssfWorkbook)
        {
            if (!isInitDataFormat && !string.IsNullOrEmpty(DataFormatString))
            {
                
                if (HSSFDataFormat.GetBuiltinFormat(DataFormatString) == -1)
                {
                    IDataFormat hssfDataFormat = hssfWorkbook.CreateDataFormat();
                    SDataFormat = hssfDataFormat.GetFormat(DataFormatString);
                }
                else
                {
                    SDataFormat = HSSFDataFormat.GetBuiltinFormat(DataFormatString);
                }
            }
            hssfCellStyle.DataFormat = SDataFormat;
        }


    }
}
