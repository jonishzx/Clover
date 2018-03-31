using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NPOI.SS.UserModel;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public class ExcelExportProvider
    {

        #region private 成员

        private XmlNode excelNode;

        #endregion

        #region 属性定义
        
        
        
        public string TypeName { get; set; }

        public string TemplateFile { get; set; }

        public IWorkbook IWorkbook { get; set; }

        
        
        
        public List<Worksheet> Worksheets { get; set; }

        private List<CellStyle> _cellStyles;
        
        
        
        public List<CellStyle> CellStyles
        {
            get
            {
                return _cellStyles;
            }
            set
            {
                _cellStyles = value;
            }
        }

        #endregion

        public ExcelExportProvider(XmlNode section)
        {
            excelNode = section;
            TypeName = XmlUtility.getNodeAttributeStringValue(section, "typename");
            TemplateFile = XmlUtility.getNodeAttributeStringValue(section, "templatefile");
            CellStyles = GetCellStyles(section);
            Worksheets = GetWorksheets(section);
        }

        
        
        
        
        
        private List<Worksheet> GetWorksheets(XmlNode section)
        {
            List<Worksheet> worksheets = new List<Worksheet>();
            XmlNodeList nodeList = section.SelectNodes("Worksheet");
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    Worksheet workSheet = new Worksheet(node, this);
                    worksheets.Add(workSheet);
                }
            }
            return worksheets;
        }

        
        
        
        
        
        public void AddWorkSheet(XmlNodeList sheetList)
        {
            if(Worksheets==null)
            {
                Worksheets = new List<Worksheet>();
            }
            if (sheetList != null)
            {
                foreach (XmlNode node in sheetList)
                {
                    Worksheet workSheet = null;
                    string refSheetId = XmlUtility.getNodeAttributeStringValue(node, "refsheetid");
                    string refSheetName = XmlUtility.getNodeAttributeStringValue(node, "refsheetName");
                    string newsheetName = XmlUtility.getNodeAttributeStringValue(node, "name");
                    string newsheetId = XmlUtility.getNodeAttributeStringValue(node, "id");
                    if (!string.IsNullOrEmpty(refSheetId))
                    {
                        XmlNodeList nodeList = excelNode.SelectNodes("Worksheet");
                        if (nodeList != null)
                        {
                            
                            foreach (XmlNode orgialNode in nodeList)
                            {
                                string sheetId = XmlUtility.getNodeAttributeStringValue(orgialNode, "id");
                                if(string.Compare(sheetId,refSheetId,true)!=0)
                                {
                                    continue;
                                }
                                workSheet = new Worksheet(orgialNode, this, refSheetId, refSheetName, newsheetId, newsheetName);
                                Worksheets.Add(workSheet);
                            }
                        }


                    }
                    else
                    {
                        workSheet = new Worksheet(node, this);
                        Worksheets.Add(workSheet);

                    }
                }
            }

        }

        private List<CellStyle> GetCellStyles(XmlNode section)
        {
            List<CellStyle> cellStyles = new List<CellStyle>();
            XmlNodeList xNodes = section.SelectNodes("CellStyle");
            if (xNodes != null && xNodes.Count > 0)
            {
                foreach (XmlNode xNode in xNodes)
                {
                    CellStyle cellStyle = new CellStyle(xNode, this);
                    cellStyles.Add(cellStyle);
                }
            }
            return cellStyles;
        }
    }
}
