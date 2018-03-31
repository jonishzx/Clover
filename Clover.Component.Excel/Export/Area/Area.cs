using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Export
{
    public abstract class Area
    {
        #region Property

        
        
        
        public string ID { get; protected set; }

        
        
        
        public string DataTable { get; set; }

        
        
        
        public int Row { get; protected set; }

        
        
        
        public int Column { get; protected set; }

        
        
        
        public abstract bool IsRepeat { get; }

        
        
        
        public bool IsStaticArea { get; protected set; }

        
        
        
        public Worksheet Worksheet { get; protected set; }

        
        
        
        public int RowSpan { get; set; }


        protected bool isComputeTopSpan = false;

        protected int _topRowSpan;

        
        
        
        public int TopRowSpan 
        {
            get
            {
                if (!isComputeTopSpan)
                    _topRowSpan = GetTopRowSpan();
                return _topRowSpan;
            }
        }

        #endregion

        #region ctor

        
        
        
        
        
        public Area(XmlNode areaNode, Worksheet worksheet)
        {
            ID = XmlUtility.getNodeAttributeStringValue(areaNode, "id");
            DataTable = XmlUtility.getNodeAttributeStringValue(areaNode, "datatable");
            Row = XmlUtility.getNodeAttributeIntValue(areaNode, "row");
            Column = XmlUtility.getNodeAttributeIntValue(areaNode, "col");
            string staticArea = XmlUtility.getNodeAttributeStringValue(areaNode, "IsStaticArea");
            if(staticArea=="1")
            {
                IsStaticArea = true;
            }
            else
            {
                IsStaticArea = false;
            }
            Worksheet = worksheet;
        }
        #endregion

        #region Method
        
        
        
        
        protected int GetTopRowSpan()
        {
            
            int num = Worksheet.Row;
            
            foreach (Area area in Worksheet.Areas)
            {
                if (area.ID != ID)
                    num += area.Row + area.RowSpan;
                else
                    break;
            }
            return num;
        }

        #endregion

        
        
        
        
        public virtual void SetXmlNode(XmlNode areaNode)
        {
            ID = XmlUtility.getNodeAttributeStringValue(areaNode, "id");
            DataTable = XmlUtility.getNodeAttributeStringValue(areaNode, "datatable");
            Row = XmlUtility.getNodeAttributeIntValue(areaNode, "row");
            Column = XmlUtility.getNodeAttributeIntValue(areaNode, "col");
        }

        
        
        
        
        
        public void SetAreaBaseInfo(string areaId,string tableName)
        {
            ID = areaId;
            DataTable = tableName;
        }

        
        
        
        public virtual void InitArea()
        {

        }

    }
}
