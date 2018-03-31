using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Clover.Component.Excel.Common;

namespace Clover.Component.Excel.Export
{
    public class Formula
    {
        
        
        
        public FormulaTypeEnums Type { get; set; }
        
        
        
        public string Target { get; set; }
        
        
        
        public string FormulaText { get; set; }
        
        
        
        public string ColumnFormulaText { get; set; }
        
        
        
        public string Operation { get; set; }
        
        
        
        public string ColumnOperation { get; set; }
        
        
        
        public string RowsField { get; set; }

        
        
        
        public string FactorValue { get; set; }

        
        
        
        
        
        
        
        public ColumnPriorityEnums ColumnPriority { get; set; }

        public Formula(XmlNode formulaNode)
        {
            string type = XmlUtility.getNodeAttributeStringValue(formulaNode, "type");
            if (!string.IsNullOrEmpty(type))
                Type = GetFormulaType(formulaNode);
            else
                throw new Exception("需要属性\"type\"没有提供");
            Target = XmlUtility.getNodeAttributeStringValue(formulaNode, "target");
            FormulaText = XmlUtility.getNodeAttributeStringValue(formulaNode, "formulatext");
            Operation = XmlUtility.getNodeAttributeStringValue(formulaNode, "operation");
            RowsField = XmlUtility.getNodeAttributeStringValue(formulaNode, "rowsfield");
            FactorValue = XmlUtility.getNodeAttributeStringValue(formulaNode, "FactorValue");
            ColumnFormulaText = XmlUtility.getNodeAttributeStringValue(formulaNode, "colformulatext");
            ColumnOperation = XmlUtility.getNodeAttributeStringValue(formulaNode, "coloperation");
            ColumnPriority = GetColumnPriority(formulaNode);
        }

        public FormulaTypeEnums GetFormulaType(XmlNode formulaNode)
        {
            switch (XmlUtility.getNodeAttributeStringValue(formulaNode, "type", "").ToLower())
            {
                case "column":
                    return FormulaTypeEnums.Column;
                case "row" :
                    return FormulaTypeEnums.Row;
                case "rowandcolumn":
                    return FormulaTypeEnums.RowAndColumn;
                default:
                case "cell":
                    return FormulaTypeEnums.Cell;
            }
        }

        public ColumnPriorityEnums GetColumnPriority(XmlNode formulaNode)
        {
            switch (XmlUtility.getNodeAttributeStringValue(formulaNode, "columnpriority", "").ToLower())
            {
                case "row":
                default:
                    return ColumnPriorityEnums.Row;
                case "column":
                    return ColumnPriorityEnums.Column;
            }
        }
    }
}
