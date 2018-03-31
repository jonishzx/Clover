using System.Collections.Generic;
using System.Xml;

using NPOI.SS.UserModel;

namespace Clover.Component.Excel.Common
{
    
    
    
    public class WorksheetBase
    {
        #region 属性定义

        
        
        
        public string ID { get; protected set; }

        
        
        
        public string Name { get; protected set; }

        
        
        
        public string Title { get; set; }

        
        
        
        public int Row { get; protected set; }

        
        
        
        public int Column { get; protected set; }
       
        
        
        
        public List<Area> Areas
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }
        private List<Area> _area;
        
        
        
        
        public ISheet ISheet { get; set; }

        
        
        
        public XmlNode HSSFSheetXmlNode { get; set; }

        #endregion

        #region ctor

        
        
        
        
        public WorksheetBase(XmlNode worksheetNode)
        {
            ID = XmlUtility.getNodeAttributeStringValue(worksheetNode, "id");
            Name = XmlUtility.getNodeAttributeStringValue(worksheetNode, "name");
            Title = XmlUtility.getNodeAttributeStringValue(worksheetNode, "title");
            Row = XmlUtility.getNodeAttributeIntValue(worksheetNode, "row");
            Column = XmlUtility.getNodeAttributeIntValue(worksheetNode, "col");

            Areas = GetAreas(worksheetNode);
            HSSFSheetXmlNode = worksheetNode;
        }

        
        
        
        
        
        public List<Area> GetAreas(XmlNode worksheetNode)
        {
            List<Area> areas = new List<Area>();
            XmlNodeList xNodes = worksheetNode.ChildNodes;
            if (xNodes != null && xNodes.Count > 0)
            {
                foreach (XmlNode xNode in xNodes)
                {
                    if (xNode.Name == "StaticArea")
                    {
                        Area area = new StaticArea(xNode, this);
                        areas.Add(area);
                    }
                    else if (xNode.Name == "RepeatArea")
                    {
                        Area area = new RepeatArea(xNode, this);
                        areas.Add(area);
                    }
                }
            }
            return areas;
        }

        #endregion
    }
}
