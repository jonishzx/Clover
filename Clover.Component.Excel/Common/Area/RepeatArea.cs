using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


namespace Clover.Component.Excel.Common
{
    
    
    
    
    
    
    
    public class RepeatArea : Area
    {
        #region 属性定义

        
        
        
        public List<DataColumn> DataColumns { get; set; }
        
        
        
        public List<string> MergeColumns { get; set; }
        
        
        
        public Dictionary<string, bool> MergeTop { get; set; }

        #endregion

        public RepeatArea(XmlNode areaNode, WorksheetBase worksheet)
            : base(areaNode, worksheet)
        {
            this.SetRepeatAreaXmlNode(areaNode);
        }

        
        
        
        public override bool IsRepeatArea
        {
            get { return true; }
        }

        
        
        
        
        public override void SetXmlNode(XmlNode areaNode)
        {
            base.SetXmlNode(areaNode);

            this.SetRepeatAreaXmlNode(areaNode);
        }

        #region 私有方法
       
        
        
        
        
        private void SetRepeatAreaXmlNode(XmlNode areaNode)
        {
            
            DataColumns = GetColumns(areaNode);
            
            MergeColumns = GetMergeColumns(areaNode);
            
            MergeTop = GetMergeTop(areaNode);
        }

        
        
        
        
        
        protected List<DataColumn> GetColumns(XmlNode areaNode)
        {
            List<DataColumn> columns = new List<DataColumn>();
            XmlNodeList columnNodes = areaNode.ChildNodes;
            if (columnNodes != null && columnNodes.Count > 0)
                foreach (XmlNode columnNode in columnNodes)
                {
                    if (columnNode.Name.ToLower() == "column")
                    {
                        DataColumn dataColumn = new DataColumn(columnNode, this);
                        columns.Add(dataColumn);
                    }
                }
            return columns;
        }

        
        
        
        
        
        protected List<string> GetMergeColumns(XmlNode areaNode)
        {
            string mergeColumns = XmlUtility.getNodeAttributeStringValue(areaNode, "mergecolumns");
            if (string.IsNullOrEmpty(mergeColumns))
                return new List<string>();
            string[] list = mergeColumns.Split(',');
            if (list != null)
                return list.ToList();
            else
                return new List<string>();
        }

        
        
        
        
        
        protected Dictionary<string, bool> GetMergeTop(XmlNode areaNode)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            
            string mergeTopStr = XmlUtility.getNodeAttributeStringValue(areaNode, "mergetop");
            
            if (string.IsNullOrEmpty(mergeTopStr))
            {
                foreach (string mergeColumnID in MergeColumns)
                {
                    dict.Add(mergeColumnID, false);
                }

                return dict;
            }

            string[] mergeTops = mergeTopStr.Split(',');
            if (mergeTops.Length != MergeColumns.Count)
                throw new Exception("MergeTop和MergeColumns配置的数量不匹配");

            for (int iLoop = 0; iLoop < mergeTops.Length; iLoop++)
            {
                string mergeTop = mergeTops[iLoop];
                
                if (mergeTop.ToLower() == "true")
                    dict.Add(MergeColumns[iLoop], true);
                else
                    dict.Add(MergeColumns[iLoop], false);
            }
            return dict;
        }
        
        #endregion
    }
}
