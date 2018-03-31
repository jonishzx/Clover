using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public class RepeatArea : Area
    {
        
        
        
        public List<DataColumn> DataColumns { get; set; }
        
        
        
        public List<string> MergeColumns { get; set; }
        
        
        
        public Dictionary<string, bool> MergeTop { get; set; }

        public override bool IsRepeat
        {
            get { return true; }
        }

        public RepeatArea(XmlNode areaNode, Worksheet worksheet)
            : base(areaNode, worksheet)
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

        
        
        
        
        public override void SetXmlNode(XmlNode areaNode)
        {
            base.SetXmlNode(areaNode);
            DataColumns = GetColumns(areaNode);
            MergeColumns = GetMergeColumns(areaNode);
            MergeTop = GetMergeTop(areaNode);

        }

        public override void InitArea()
        {
            base.InitArea();
            foreach(DataColumn clolumn in DataColumns)
            {
                clolumn.ResetFormula();
            }
        }

    }
}
