using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NPOI.SS.UserModel;
using System.Reflection;

namespace Clover.Component.Excel.Export
{
    public class Font : CellStyleBase
    {
        #region Properties

        
        
        
        protected IFont IFont { get; set; }

        public CellStyle CellStyle { get; set; }

        #endregion

        #region ctor

        public Font(XmlNode fontNode, CellStyle cellStyle)
            : base(fontNode)
        {
            CellStyle = cellStyle;
        }

        #endregion

        #region Methods

        
        
        
        
        
        public void SetCellFont(IWorkbook hssfWorkbook)
        {
            SetCellFont(CellStyle.ICellStyle, hssfWorkbook);
        }

        
        
        
        
        
        public void SetCellFont(ICellStyle hssfCellStyle, IWorkbook hssfWorkbook)
        {
            
            
            if (IFont == null)
            {
                IFont = hssfWorkbook.CreateFont();
                Type hssfFontType = IFont.GetType();
                
                for (int iLoop = 0; iLoop < Properties.Count; iLoop++)
                {
                    PropertyInfo property = hssfFontType.GetProperty(Properties[iLoop]);
                    object value = Arguments[iLoop].GetValue();
                    property.SetValue(IFont, value, null);
                }
            }
            hssfCellStyle.SetFont(IFont);
        }

        #endregion
    }

    
}
