using System.Xml;


namespace Clover.Component.Excel.Common
{
    
    
    
    public abstract class Area
    {
        #region 属性定义

        
        
        
        public string ID { get; protected set; }

        
        
        
        public string DataTable { get; set; }

        
        
        
        public int Row { get; protected set; }

        
        
        
        public int Column { get; protected set; }

        
        
        
        public int RowSpan { get; set; }

        
        
        
        public abstract bool IsRepeatArea { get; }

        
        
        
        public bool IsStaticArea { get; protected set; }

        
        
        
        public WorksheetBase Worksheet { get; protected set; }

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

        
        
        
        
        public Area(XmlNode areaNode)
        {
            this.SetXmlNode(areaNode);
        }

        
        
        
        
        
        public Area(XmlNode areaNode, WorksheetBase worksheet)
            : this(areaNode)
        {
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

        
        
        
        
        public virtual void SetXmlNode(XmlNode areaNode)
        {
            ID = XmlUtility.getNodeAttributeStringValue(areaNode, "id");
            DataTable = XmlUtility.getNodeAttributeStringValue(areaNode, "datatable");
            Row = XmlUtility.getNodeAttributeIntValue(areaNode, "row");
            Column = XmlUtility.getNodeAttributeIntValue(areaNode, "col");
        }

        #endregion
    }
}
