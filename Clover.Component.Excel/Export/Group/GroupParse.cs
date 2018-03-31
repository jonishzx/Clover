using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Export.Group
{
    
    public class GroupParse
    {
        #region 组合分类枚举

        
        
        
        public static string RowGroupType = "row";

        
        
        
        public static string ColumnGroupType = "column";

        #endregion

        
        
        
        public static string ColumnField = "GroupFiled";

        
        
        
        private XmlNode currentSheetNode;

        
        
        
        
        public GroupParse(XmlNode sheetNode)
        {
            currentSheetNode = sheetNode;
        }

        
        
        
        
        public List<GroupConfig> ParseGroup()
        {
            List<GroupConfig> configs = new List<GroupConfig>();
            XmlNodeList xNodes = currentSheetNode.SelectNodes("Group");
            if (xNodes != null && xNodes.Count > 0)
            {
                configs.AddRange(from XmlNode xNode in xNodes select PareseSigleConfig(xNode));
            }
            return configs;
        }

        
        
        
        
        
        private GroupConfig PareseSigleConfig(XmlNode node)
        {
            GroupConfig config = new GroupConfig();
            string groupType = XmlUtility.getNodeAttributeStringValue(node, "type");
            if(string.IsNullOrEmpty(groupType))
            {
                throw new ArgumentException("Group配置节中的type不能为空");
            }
            else if(string.Compare(groupType,RowGroupType,true)==0)
            {
                config.GroupType = GroupConfig.RowGroupType;
            }
            else if (string.Compare(groupType, ColumnGroupType, true) == 0)
            {
                config.GroupType = GroupConfig.ColumnGroupType;
            }

            string areaID = XmlUtility.getNodeAttributeStringValue(node, "AreaID");
            if (string.IsNullOrEmpty(areaID))
            {
                throw new ArgumentException("Merged配置项中AreaID无效");
            }
            config.AreaId = areaID;
            string fromColumn = XmlUtility.getNodeAttributeStringValue(node, "FromColumn");
            config.FromColumn = (short)ParseStringToInt(fromColumn);
            string fromRow = XmlUtility.getNodeAttributeStringValue(node, "FromRow");
            config.FromRow = ParseStringToInt(fromRow);
            string toColumn = XmlUtility.getNodeAttributeStringValue(node, "ToColumn");
            config.ToColumn = (short)ParseStringToInt(toColumn);
            string toRow = XmlUtility.getNodeAttributeStringValue(node, "ToRow");
            config.ToRow = ParseStringToInt(toRow);

            string groupField = XmlUtility.getNodeAttributeStringValue(node, ColumnField);
            config.ColumnField = groupField;

            return config;
        }

        
        
        
        
        
        private int ParseStringToInt(string strNum)
        {
            int tempInt = -1;
            if(!int.TryParse(strNum,out tempInt))
            {
                tempInt = -1;
            }
            return tempInt;
        }
    }
}
