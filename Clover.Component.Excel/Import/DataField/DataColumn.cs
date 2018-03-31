


using System;
using System.Xml;

using System.Reflection;
using NPOI.SS.UserModel;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Import
{
    public class DataColumn
    {
        #region 属性
        
        
        
        public string Field
        {
            get;
            set;
        }
        
        
        
        public int Offset
        {
            get;
            set;
        }
        
        
        
        public DataTypeEnums DataType
        {
            get;
            set;
        }

        
        
        
        public string DefaultValueConfig
        {
            get;
            set;
        }
        
        private object _defaultValue;
        
        
        
        public object DefaultValue
        {
            get
            {
                switch(DefaultValueSource)
                {
                    case DefaultValueSourceEnum.Constant:           
                        return DefaultValueConfig;
                    case DefaultValueSourceEnum.Method:             
                        if (_defaultValue == null)
                        {
                            _defaultValue = GetDefaultValue(BindingFlags.InvokeMethod);
                        }
                        break;
                    case DefaultValueSourceEnum.Property:
                        if (_defaultValue == null)
                        {
                            _defaultValue = GetDefaultValue(BindingFlags.GetProperty);
                        }
                        break;
                    case DefaultValueSourceEnum.Field:
                        if (_defaultValue == null)
                        {
                            _defaultValue = GetDefaultValue(BindingFlags.GetField);
                        }
                        break;
                }
                return _defaultValue;
            }
        }

        
        
        
        
        
        public DefaultValueSourceEnum DefaultValueSource
        {
            get;
            set;
        }
        
        
        
        public bool AllowNull
        {
            get;
            set;
        }
        
        
        
        public string ErrorMessage
        {
            get;
            set;
        }

        public bool NeedBind
        {
            get;
            set;
        }

        public string Match
        {
            get;
            set;
        }

        
        
        
        public bool HasMerged
        {
            get;
            set;
        }

        
        
        
        public string Label
        {
            get;
            set;
        }

        
        
        
        public string Validator
        {
            get;
            set;
        }

        #endregion

        #region 初始化
        public DataColumn(XmlNode section)
        {
            Field           = XmlUtility.getNodeAttributeStringValue(section, "field");
            Offset          = XmlUtility.getNodeAttributeIntValue(section, "offset");
            DataType        = (DataTypeEnums)Enum.Parse(typeof(DataTypeEnums),XmlUtility.getNodeAttributeStringValue(section, "datatype","String"));
            AllowNull       = XmlUtility.getNodeAttributeBooleanValue(section, "allownull");
            ErrorMessage    = XmlUtility.getNodeAttributeStringValue(section, "errMsg");
            NeedBind        = XmlUtility.getNodeAttributeStringValue(section, "needbind", "true") == "false" ? false : true;
            Match           = XmlUtility.getNodeAttributeStringValue(section, "match", "");
            
            HasMerged = XmlUtility.getNodeAttributeStringValue(section, "hasMerged", "false") == "false" ? false : true;
            Label = XmlUtility.getNodeAttributeStringValue(section, "label", Field);
            Validator = XmlUtility.getNodeAttributeStringValue(section, "validator", "");

            string defaultValueSource = XmlUtility.getNodeAttributeStringValue(section, "defaultvaluesource", "Constant");
            DefaultValueSource = (DefaultValueSourceEnum)Enum.Parse(typeof(DefaultValueSourceEnum), string.IsNullOrEmpty(defaultValueSource) ? "Constant" : defaultValueSource);
            DefaultValueConfig = XmlUtility.getNodeAttributeStringValue(section, "defaultvalue");
        }
        
        public DataColumn(XmlNode section , Area area)
        :this(section){
        }
        
        #endregion

        #region Protected Method

        
        
        
        
        
        protected virtual object GetDefaultValue(BindingFlags bindFlag)
        {
            object defaultValue;
            string[] config = DefaultValueConfig.Split(',');
            if (config.Length == 3)
            {
                Assembly assembly = Assembly.Load(config[2]);
                Type type = assembly.GetType(config[1]);
                defaultValue = type.InvokeMember(config[0], bindFlag, null, null, null);
            }
            else
            {
                Type type = Type.GetType(config[1]);
                defaultValue = type.InvokeMember(config[0], bindFlag, null, null, null);
            }
            return defaultValue;
        }

        #endregion

        #region Public Method

        public void SetDefaultNull()
        {
            _defaultValue = null;
        }

        public string GetMatchValue(ICell hssfCell)
        {
            string[] matchValues = Match.Split(',');
            string[] cellValues = matchValues[0].Split(':');
            string[] resultValues = matchValues[1].Split(':');

            for (int iLoop = 0; iLoop < cellValues.Length; iLoop++)
            {
                string cellValue = cellValues[iLoop];
                if (cellValue == hssfCell.StringCellValue)
                {
                    return resultValues[iLoop];
                }
            }
            return "";
        }

        #endregion
    }
}
