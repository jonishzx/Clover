using System;
using System.Reflection;
using System.Xml;
using Clover.Component.Excel.Export;


namespace Clover.Component.Excel.Common
{
    
    
    
    public abstract class DataFieldBase
    {
        #region 属性定义

        
        
        
        public string ID { get; protected set; }

        
        
        
        public string Field { get; protected set; }

        
        
        
        public string ValueConfig { get; protected set; }

        
        
        
        public DataTypeEnums DataType { get; set; }

        private object _value;
        
        public object Value
        {
            get
            {
                switch (ValueSource)
                {
                    case ValueSourceEnums.Constant:           
                        return ValueConfig;
                    case ValueSourceEnums.Method:             
                        if (_value == null)
                        {
                            _value = GetNotFieldValue(BindingFlags.InvokeMethod);
                        }
                        break;
                    case ValueSourceEnums.Property:
                        if (_value == null)
                        {
                            _value = GetNotFieldValue(BindingFlags.GetProperty);
                        }
                        break;
                    case ValueSourceEnums.Field:
                        if (_value == null)
                        {
                            _value = GetNotFieldValue(BindingFlags.GetField);
                        }
                        break;
                }
                return _value;
            }
        }

        
        
        
        public ValueSourceEnums ValueSource { get; set; }

        
        
        
        
        public string Match { get; set; }

        
        
        
        public Area Area { get; protected set; }

        private string _excelColumnNum;
        
        
        
        public string ExcelColumnNum
        {
            get
            {
                if (string.IsNullOrEmpty(_excelColumnNum))
                    _excelColumnNum = GetExcelColumnNum();
                return _excelColumnNum;
            }
        }

        #endregion

        #region ctor

        public DataFieldBase(XmlNode dataFieldNode, Area area)
        {
            ID = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "id");
            Field = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "field");
            DataType = GetDataType(dataFieldNode);
            ValueConfig = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "value");            
            ValueSource = GetValueSource(dataFieldNode);
            Match = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "match");

            
            Area = area;
        }

        protected DataTypeEnums GetDataType(XmlNode dataFieldNode)
        {
            string dataType = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "datatype", "");

            switch (dataType.ToLower())
            {
                case "numeric":
                    return DataTypeEnums.Numeric;
                case "datetime":
                    return DataTypeEnums.DateTime;
                case "boolean":
                    return DataTypeEnums.Boolean;
                case "formula":
                    return DataTypeEnums.Formula;
                case "string":
                default:
                    return DataTypeEnums.String;
            }
        }

        protected ValueSourceEnums GetValueSource(XmlNode dataFieldNode)
        {
            string valueSource = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "valuesource");

            switch ((valueSource + "").ToLower())
            {
                case "field":
                    return ValueSourceEnums.Field;
                case "method":
                    return ValueSourceEnums.Method;
                case "property":
                    return ValueSourceEnums.Property;
                case "constant":
                default:
                    return ValueSourceEnums.Constant;
            }
        }

        #endregion

        #region Method

        
        
        
        
        
        protected virtual object GetNotFieldValue(BindingFlags bindFlag)
        {
            string[] config = ValueConfig.Split(',');
            if (config.Length == 3)
            {
                Assembly assembly = Assembly.Load(config[2]);
                Type type = assembly.GetType(config[1]);
                _value = type.InvokeMember(config[0], bindFlag, null, null, null);
            }
            else
            {
                Type type = Type.GetType(config[1]);
                _value = type.InvokeMember(config[0], bindFlag, null, null, null);
            }
            return _value;
        }

        
        
        
        
        protected abstract string GetExcelColumnNum();

        #endregion
    }
}
