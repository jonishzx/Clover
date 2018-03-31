using System.Collections.Generic;
using System.Xml;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Import
{
    public class ExcelImportProvider
    {
        #region 构造方法
        public ExcelImportProvider(XmlNode section)
        {
            TypeName    = XmlUtility.getNodeAttributeStringValue(section, "typename");
            Worksheets  = GetWorksheets(section);
        }
        
        
        
        
        
        private List<Worksheet> GetWorksheets(XmlNode section)
        {
            List<Worksheet> worksheets = new List<Worksheet>();
            XmlNodeList nodeList = section.ChildNodes;
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    Worksheet workSheet = new Worksheet(node);
                    worksheets.Add(workSheet);
                }
            }
            return worksheets;
        }
        #endregion
        #region 属性定义
        private string _typeName;
        
        
        
        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }
        private List<Worksheet> _worksheets = null;
        
        
        
        public List<Worksheet> Worksheets
        {
            get { return _worksheets; }
            set { _worksheets = value; }
        }
        #endregion

        

    }
}
