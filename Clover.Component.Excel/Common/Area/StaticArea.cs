using System.Collections.Generic;
using System.Xml;


namespace Clover.Component.Excel.Common
{
    
    
    
    
    
    
    
    public class StaticArea : Area
    {
        
        
        
        public List<DataCell> DataCells { get; set; }

        
        
        
        
        
        public StaticArea(XmlNode areaNode, WorksheetBase worksheet)
            : base(areaNode, worksheet)
        {
            this.SetStaticAreaXmlNode(areaNode);
        }

        
        
        
        public override bool IsRepeatArea
        {
            get
            {
                return false;
            }
        }

        
        
        
        
        public override void SetXmlNode(XmlNode areaNode)
        {
            base.SetXmlNode(areaNode);
            this.SetStaticAreaXmlNode(areaNode);
        }

        #region 私有方法

        
        
        
        
        private void SetStaticAreaXmlNode(XmlNode areaNode)
        {
            
            RowSpan = XmlUtility.getNodeAttributeIntValue(areaNode, "rowspan");
            
            DataCells = GetCells(areaNode);
        }

        
        
        
        
        
        private List<DataCell> GetCells(XmlNode areaNode)
        {
            List<DataCell> cells = new List<DataCell>();
            XmlNodeList cellNodes = areaNode.ChildNodes;
            if (cellNodes != null && cellNodes.Count > 0)
                foreach (XmlNode cellNode in cellNodes)
                {
                    if (cellNode.Name.ToLower() == "cell")
                    {
                        DataCell cell = new DataCell(cellNode, this);
                        cells.Add(cell);
                    }
                }
            return cells;
        }

        #endregion
    }
}
