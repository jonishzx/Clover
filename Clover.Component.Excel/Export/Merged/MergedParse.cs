using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export.Merged
{
    
    public class MergedParse
    {
        #region 合并分类枚举

        
        
        
        public static string RowMergedType = "row";

        
        
        
        public static string ColumnMergedType = "column";


        
        
        
        public static string AllMergedType = "all";

        #endregion

        #region 合并优先枚举

        
        
        
        public static string RowMergedTypePriority = "row";


        
        
        
        public static string ColumnMergedTypePriority = "column";

        #endregion
        
        
        
        private XmlNode sheetNode;

        
        
        
        
        public MergedParse(XmlNode sheetConfigNode)
        {
            this.sheetNode = sheetConfigNode;
        }

        public List<MergedConfig> ParseMergedConfig()
        {
            List<MergedConfig> configs = new List<MergedConfig>();
            XmlNodeList xNodes = sheetNode.SelectNodes("Merged");
            if (xNodes != null && xNodes.Count > 0)
            {
                configs.AddRange(from XmlNode xNode in xNodes select PareseSigleConfig(xNode));
            }
            return configs;
        }

        
        
        
        
        
        private MergedConfig PareseSigleConfig(XmlNode xNode)
        {
            MergedConfig config = new MergedConfig();
            string mergedType = XmlUtility.getNodeAttributeStringValue(xNode, "type");
            if (string.IsNullOrEmpty(mergedType))
                throw new ArgumentException("Merged配置项中type不能为空");
            if (string.Compare(mergedType, RowMergedType, true) == 0)
            {
                config.MergedType = MergedConfig.RowMergedType;
            }
            else if (string.Compare(mergedType, ColumnMergedType, true) == 0)
            {
                config.MergedType = MergedConfig.ColumnMergedType;
            }
            else if (string.Compare(mergedType, AllMergedType, true) == 0)
            {
                config.MergedType = MergedConfig.AllMergedType;
            }
            else
            {
                throw new ArgumentException("Merged配置项中type无效");
            }

            string areaID = XmlUtility.getNodeAttributeStringValue(xNode, "AreaID");
            if (string.IsNullOrEmpty(areaID))
            {
                throw new ArgumentException("Merged配置项中AreaID无效");
            }
            config.AreaId = areaID;
            config.FromColumn = XmlUtility.getNodeAttributeStringValue(xNode, "FromColumn");
            config.FromRow = XmlUtility.getNodeAttributeStringValue(xNode, "FromRow");
            config.ToColumn = XmlUtility.getNodeAttributeStringValue(xNode, "ToColumn");
            config.ToRow = XmlUtility.getNodeAttributeStringValue(xNode, "ToRow");
            string mergedTypePriority = XmlUtility.getNodeAttributeStringValue(xNode, "MergedTypePriority");
            if (string.Compare(mergedTypePriority,ColumnMergedTypePriority,true)==0)
            {
                config.MergedTypePriority = MergedConfig.ColumnMergedTypePriority;
            }
            else
            {
                config.MergedTypePriority = MergedConfig.RowMergedType;
            }
            string needValid = XmlUtility.getNodeAttributeStringValue(xNode, "NeedValid");
            if (string.Compare(needValid,"false",true)==0)
            {
                config.NeedValid = false;
            }
            else
            {
                config.NeedValid = true;
            }

            string ignoreEmpty = XmlUtility.getNodeAttributeStringValue(xNode, "IgnoreEmpty");
            if (string.Compare(ignoreEmpty, "false", true) == 0)
            {
                config.IgnoreEmpty = false;
            }
            else
            {
                config.IgnoreEmpty = true;
            }
            return config;

        }
    }
}
