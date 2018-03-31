using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NPOI.SS.UserModel;
using System.Reflection;

namespace Clover.Component.Excel.Export
{
    public class CellStyle : CellStyleBase
    {
        
        
        
        public ICellStyle ICellStyle { get; set; }

        public ExcelExportProvider ExcelExportProvider { get; set; }

        
        
        
        public Font Font { get; set; }

        public DataFormat DataFormat { get; set; }

        public CellStyle(XmlNode cellStyleNode, ExcelExportProvider excelExportProvider)
            : base(cellStyleNode)
        {
            XmlNode XNode = cellStyleNode.SelectSingleNode("Font");
            if (XNode != null)
                Font = new Font(XNode, this);
            XNode = cellStyleNode.SelectSingleNode("DataFormat");
            if (XNode != null)
                DataFormat = new DataFormat(XNode, this);
            this.ExcelExportProvider = excelExportProvider;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        

        
        
        
        
        public void SetCellStyle(ICell hssfCell, IWorkbook hssfWorkbook)
        {
            if (ICellStyle == null)
            {
                ICellStyle = hssfWorkbook.CreateCellStyle();
                if (Properties.Count != 0)
                {
                    Type hssfCellType = ICellStyle.GetType();
                    
                    for (int iLoop = 0; iLoop < Properties.Count; iLoop++)
                    {
                        PropertyInfo property = hssfCellType.GetProperty(Properties[iLoop]);
                        object value = Arguments[iLoop].GetValue();
                        property.SetValue(ICellStyle, value, null);
                    }
                }
                if(Font != null)
                    Font.SetCellFont(hssfWorkbook);
                if (DataFormat != null)
                    DataFormat.SetDataFormat(hssfWorkbook);
            }
            hssfCell.CellStyle = ICellStyle;
        }
    }
}
