using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Xml;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public abstract class CellStyleBase
    {
        #region Properties
        
        
        
        public string ID { get; protected set; }

        
        
        
        public List<string> Properties { get; protected set; }

        
        
        
        
        public List<Argument> Arguments { get; protected set; }

        #endregion

        #region ctor

        public CellStyleBase(XmlNode baseNode)
        {
            ID = XmlUtility.getNodeAttributeStringValue(baseNode, "id");
            InitPropertiesAndArguments(baseNode);
        }

        #endregion

        #region Methods

        
        
        
        
        protected void InitPropertiesAndArguments(XmlNode baseNode)
        {
            Properties = new List<string>();
            Arguments = new List<Argument>();

            string propertiesConfiged = XmlUtility.getNodeAttributeStringValue(baseNode, "properties", "");
            string[] propertiesConfigedArray = propertiesConfiged.Split(';');

            string argumentsConfiged = XmlUtility.getNodeAttributeStringValue(baseNode, "arguments", "");
            string[] argumentsConfigedArray = argumentsConfiged.Split(';');
            
            if (propertiesConfigedArray.Length != argumentsConfigedArray.Length)
                throw new Exception("Arguments 和 Properties 配置的数量不匹配");
            
            foreach (string propertyConfiged in propertiesConfigedArray)
            {
                if (!string.IsNullOrEmpty(propertyConfiged))
                    Properties.Add(propertyConfiged.Trim());
            }
            
            foreach (string argumentConfiged in argumentsConfigedArray)
            {
                if (!string.IsNullOrEmpty(argumentConfiged))
                    Arguments.Add(InitArgument(argumentConfiged.Trim()));
            }
        }

        
        
        
        
        
        protected Argument InitArgument(string argumentConfiged)
        {
            ArgumentTypeEnums argumentType;
            
            string[] argumentConfigArray = argumentConfiged.Split(':');
            
            switch (argumentConfigArray[0].Trim())
            {
                case "Short":
                    argumentType = ArgumentTypeEnums.Short;
                    break;
                case "Byte":
                    argumentType = ArgumentTypeEnums.Byte;
                    break;
                case "Boolean":
                    argumentType = ArgumentTypeEnums.Boolean;
                    break;
                case "Reflect":
                    argumentType = ArgumentTypeEnums.Reflect;
                    break;
                case "String":
                default:
                    argumentType = ArgumentTypeEnums.String;
                    break;
            }
            
            string configValue = argumentConfigArray[1].Trim();
            return new Argument(argumentType, configValue);
        }
        #endregion
    }

    public class Argument
    {
        #region Properties

        
        
        
        public ArgumentTypeEnums ArgumentType { get; set; }
        
        
        
        public string ConfigValue { get; set; }

        #endregion

        #region ctor

        public Argument(ArgumentTypeEnums argumentType, string configValue)
        {
            ArgumentType = argumentType;
            ConfigValue = configValue;
        }

        #endregion

        #region Methods

        
        
        
        
        public object GetValue()
        {
            switch (ArgumentType)
            {
                case ArgumentTypeEnums.String:
                default:
                    return ConfigValue;
                case ArgumentTypeEnums.Short:
                    return short.Parse(ConfigValue);
                case ArgumentTypeEnums.Byte:
                    return byte.Parse(ConfigValue);
                case ArgumentTypeEnums.Boolean:
                    return bool.Parse(ConfigValue);
                case ArgumentTypeEnums.Reflect:  
                    return GetReflectValue();
            }
        }

        
        
        
        
        public object GetReflectValue()
        {
            
            
            string[] argumentArray = ConfigValue.Split('@');
            string[] reflectArray = argumentArray[0].Split(','); 
            string[] reflectValueArray = argumentArray[1].Split(','); 
            object obj = null; 
            for (int iLoop = 0; iLoop < reflectArray.Length; iLoop++)
            {
                switch (reflectArray[iLoop])
                {
                    case "Assembly": 
                        Assembly assembly = Assembly.Load(reflectValueArray[iLoop]);
                        obj = assembly;
                        break;
                    case "Class": 
                        Type type;
                        if (obj != null) 
                            type = (obj as Assembly).GetType(reflectValueArray[iLoop].Trim());
                        else 
                            type = Type.GetType(reflectValueArray[iLoop]);
                        obj = type;
                        break;
                    case "Property": 
                        Type typePro;
                        if (obj is Type) 
                            typePro = obj as Type;
                        else
                            typePro = obj.GetType();  
                        PropertyInfo propertyInfo = typePro.GetProperty(reflectValueArray[iLoop].Trim()); 
                        obj = propertyInfo.GetValue(obj, null); 
                        break;
                    case "Field": 
                        Type typeField;
                        if (obj is Type) 
                            typeField = obj as Type;
                        else
                            typeField = obj.GetType();
                        FieldInfo fieldInfo = typeField.GetField(reflectValueArray[iLoop].Trim()); 
                        obj = fieldInfo.GetValue(obj); 
                        break;
                    case "Function":
                        Type typeFun;
                        if (obj is Type)
                            typeFun = obj as Type;
                        else
                            typeFun = obj.GetType();
                        MethodInfo methodInfo = typeFun.GetMethod(reflectValueArray[iLoop].Trim()); 
                        obj = methodInfo.Invoke(obj, null); 
                        break;
                }
            }
            return obj;
        }

        #endregion
    }
}
