using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Export
{
    public class StaticArea : Area
    {
        
        
        
        public List<DataCell> DataCells { get; set; }

        
        
        
        public override bool IsRepeat
        {
            get
            {
                return false;
            }
        }

        public StaticArea(XmlNode areaNode, Worksheet worksheet)
            : base(areaNode, worksheet)
        {
            DataCells = GetCells(areaNode);
            RowSpan = XmlUtility.getNodeAttributeIntValue(areaNode, "rowspan");
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

        
        
        
        
        public override void SetXmlNode(XmlNode areaNode)
        {
            base.SetXmlNode(areaNode);
            DataCells = GetCells(areaNode);
            RowSpan = XmlUtility.getNodeAttributeIntValue(areaNode, "rowspan");

        }

    }
}
