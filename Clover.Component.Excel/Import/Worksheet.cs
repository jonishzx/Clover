


using System.Collections.Generic;
using Clover.Component.Excel.Common;
using System.Xml;

namespace Clover.Component.Excel.Import
{
    public class Worksheet
    {
        #region Property Define

        
        
        
        private List<Dictionary<string, object>> _dataList = new List<Dictionary<string, object>>();
        
        
        
        public List<Dictionary<string, object>> DataList
        {
            get { return _dataList; }
            set { _dataList = value; }
        }

        
        
        
        public string Title
        {
            get;
            set;
        }
        
        
        
        public string Name
        {
            get;
            set;
        }
        
        
        
        public int Row
        {
            get;
            set;
        }
        
        
        
        public int Column
        {
            get;
            set;
        }
        
        
        
        public string DataTable
        {
            get;
            set;
        }
        
        
        
        public string EndFlag
        {
            get;
            set;
        }
        private List<DataColumn> _datacolumns = null;
        
        
        
        public List<DataColumn> DataColumns
        {
            get { return _datacolumns; }
            set { _datacolumns = value; }
        }

        
        
        
        public Dictionary<string, int> FieldIndex { get; private set; }
        #endregion

        #region 初始化

        public Worksheet()
        {
        }

        public Worksheet(XmlNode section)
        {
            Name        = XmlUtility.getNodeAttributeStringValue(section, "name");
            Title       = XmlUtility.getNodeAttributeStringValue(section, "title");
            Row         = XmlUtility.getNodeAttributeIntValue(section, "row");
            Column      = XmlUtility.getNodeAttributeIntValue(section, "col");
            DataTable   = XmlUtility.getNodeAttributeStringValue(section, "datatable");
            DataColumns = GetDataColumns(section);

            FieldIndex = GetFieldIndex();
        }
        #endregion

        #region 函数

        
        
        
        
        public Worksheet GetCopyWorkSheet()
        {
            Worksheet rtn = new Worksheet();
            rtn.Name = this.Name;
            rtn.Title = this.Title;
            rtn.Row = this.Row;
            rtn.Column = this.Column;
            rtn.DataTable = this.DataTable;
            rtn.DataColumns = this.DataColumns;
            rtn.FieldIndex = this.FieldIndex;

            return rtn;
        }

        
        
        
        
        
        private List<DataColumn> GetDataColumns(XmlNode section)
        {
            List<DataColumn> datacolumns = new List<DataColumn>();
            XmlNodeList nodeList = section.ChildNodes;
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    DataColumn dataColumn = new DataColumn(node);
                    datacolumns.Add(dataColumn);
                }
            }
            return datacolumns;
        }

        
        
        
        
        private Dictionary<string, int> GetFieldIndex()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            
            foreach (DataColumn col in this.DataColumns)
            {
                result.Add(col.Field, this.Column + col.Offset);
            }
            return result;
        }

        #endregion

    }
}
