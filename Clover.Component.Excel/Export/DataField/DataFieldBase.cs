using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Reflection;
using System.Data;
using NPOI.SS.UserModel;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public abstract class DataFieldBase
    {
        #region Property

        
        
        
        public string ID { get; protected set; }
        
        
        
        public string Field { get; protected set; }
        
        
        
        public string ValueConfig { get; protected set; }
        
        
        
        
        public string Match { get; set; }

        
        
        
        public string FilterValue { get; set; }

        
        
        
        public string SummaryFlagField { get; set; }

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
        
        
        
        public DataTypeEnums DataType { get; set; }
        
        
        
        public ValueSourceEnums ValueSource { get; set; }

        
        
        
        public Area Area { get; protected set; }

        protected Formula _formula;
        
        
        
        public Formula Formula { get; set; }

        
        
        
        public CellStyle CellStyle { get; set; }

        
        
        
        public CellStyle SummaryCellStyle { get; set; }

        
        
        
        public DataRow DataRow { get; set; }

        #endregion

        #region ctor

        public DataFieldBase(XmlNode dataFieldNode, Area area)
        {
            ID = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "id");
            Field = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "field");
            SummaryFlagField = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "summaryflagfield");
            ValueConfig = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "value");
            FilterValue = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "FilterValue");
            Match = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "match");
            DataType = GetDataType(dataFieldNode);
            ValueSource = GetValueSource(dataFieldNode);
            Area = area;
            CellStyle = GetCellStyle(dataFieldNode);
            SummaryCellStyle = GetSummaryCellStyle(dataFieldNode);
            Formula = GetFormula();
        }
        #endregion

        #region Init
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

        protected CellStyle GetSummaryCellStyle(XmlNode dataFieldNode)
        {
            string cellStyle = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "summarycellstyle", "");
            return this.Area.Worksheet.ExcelExportProvider.CellStyles.FirstOrDefault(p => p.ID == cellStyle);
        }

        protected CellStyle GetCellStyle(XmlNode dataFieldNode)
        {
            string cellStyle = XmlUtility.getNodeAttributeStringValue(dataFieldNode, "cellstyle", "");
            return this.Area.Worksheet.ExcelExportProvider.CellStyles.FirstOrDefault(p => p.ID == cellStyle);
        }

        protected Formula GetFormula()
        {
            return this.Area.Worksheet.Formulas.FirstOrDefault(p => p.Target == string.Format("{0}.{1}.{2}", Area.Worksheet.ID, Area.ID, ID));
        }

        public void ResetFormula()
        {
            Formula = GetFormula();
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

        
        
        
        
        
        protected string GetMatchValue(DataRow dataRow)
        {
            string[] matchValues = Match.Split(',');
            string[] cellValues = matchValues[0].Split(':');
            string[] resultValues = matchValues[1].Split(':');

            for (int iLoop = 0; iLoop < cellValues.Length; iLoop++)
            {
                string cellValue = cellValues[iLoop];
                if (cellValue == dataRow[Field] + "")
                {
                    return resultValues[iLoop];
                }
            }
            return "";
        }

        
        
        
        
        
        
        public object GetCellPrepareValue(ICell hssfCell, DataRow dataRow)
        {
            object valueResult;
            
            if (Formula == null ||
                (Formula.Type == FormulaTypeEnums.Column && (string.IsNullOrEmpty(Formula.RowsField) || string.IsNullOrEmpty(dataRow[Formula.RowsField] + ""))))
            {
                
                if (string.IsNullOrEmpty(Field))
                {
                    
                    
                    if (!string.IsNullOrEmpty(ValueConfig))
                        valueResult = Value;
                    else
                        valueResult = "";
                }
                else
                {
                    
                    
                    if (string.IsNullOrEmpty(Match))
                        valueResult = dataRow[Field];
                    
                    else
                        valueResult = GetMatchValue(dataRow);
                }
            }
            else
            {
                string[] operations = Formula.Operation.Split(',');
                string formulaValue = string.Empty;
                
                switch (Formula.Type)
                {
                    case FormulaTypeEnums.Cell:
                        formulaValue= GetCellFormula(operations);
                        break;
                    case FormulaTypeEnums.Column:
                        formulaValue = GetColumnFormula(operations, dataRow, false);
                        break;
                    case FormulaTypeEnums.Row:
                        formulaValue = GetRowFormula(hssfCell, operations);
                        break;
                    case FormulaTypeEnums.RowAndColumn:
                        if (string.IsNullOrEmpty(dataRow[Formula.RowsField] + ""))
                            formulaValue = GetRowFormula(hssfCell, operations);
                        else
                            formulaValue = GetColumnFormula(Formula.ColumnOperation.Split(','), dataRow, true);
                        break;

                }
                if (!string.IsNullOrEmpty(formulaValue))
                {
                    if(!string.IsNullOrEmpty(Formula.FactorValue))
                    {
                        string factor=dataRow[Formula.FactorValue]+"";
                        if (!string.IsNullOrEmpty(factor))
                        {
                            decimal tempInt = 0;
                            decimal.TryParse(factor, out tempInt);
                            if (tempInt != 1 && tempInt != 0)
                            {
                                formulaValue = "(" + formulaValue + ")/" + factor;
                            }
                        }
                    }
                    return formulaValue;
                }
                valueResult = null;
            }
            return valueResult;
        }

        
        
        
        
        
        protected string GetCellFormula(string[] operations)
        {
            List<string> result = new List<string>();
            
            Dictionary<string, StaticArea> dictArea = new Dictionary<string, StaticArea>();
            
            Dictionary<string, Worksheet> dictWorksheet = new Dictionary<string, Worksheet>();
            foreach (string operation in operations)
            {
                
                string[] areaDotCell = operation.Split('.');
                ExcelExportProvider provider = Area.Worksheet.ExcelExportProvider;
                
                if (!dictWorksheet.ContainsKey(areaDotCell[0]))
                    dictWorksheet.Add(areaDotCell[0], provider.Worksheets.First(p => p.ID == areaDotCell[0]));
                Worksheet worksheet = dictWorksheet[areaDotCell[0]];
                
                if (!dictArea.ContainsKey(areaDotCell[1]))
                    dictArea.Add(areaDotCell[1], worksheet.Areas.First(p => p.ID == areaDotCell[1]) as StaticArea);
                
                if (worksheet.ID == Area.Worksheet.ID)
                    result.Add(dictArea[areaDotCell[1]].
                        DataCells.First(p => p.ID == areaDotCell[2]).ExcelCellName);
                else 
                    result.Add(worksheet.Name + "!" + dictArea[areaDotCell[1]].
                        DataCells.First(p => p.ID == areaDotCell[2]).ExcelCellName);
            }
            return string.Format(Formula.FormulaText, result.ToArray());
        }

        
        
        
        
        
        
        protected string GetColumnFormula(string[] operations, DataRow dataRow, bool isColumnAndRow)
        {
            List<string> result = new List<string>();
            
            Dictionary<string, Area> dictArea = new Dictionary<string, Area>();
            
            Dictionary<string, Worksheet> dictWorksheet = new Dictionary<string, Worksheet>();
            
            string[] rowsFields = (dataRow[Formula.RowsField] + "").Split(',');
            int[] rowsFieldInt = new int[rowsFields.Length];
            int index = 0;
            foreach (string item in rowsFields)
            {
                rowsFieldInt[index++] = int.Parse(item);
            }
            Array.Sort<int>(rowsFieldInt);

            string resultFormulaText = "", tempFormulaText;
            if (isColumnAndRow)
                tempFormulaText = Formula.ColumnFormulaText;
            else
                tempFormulaText = Formula.FormulaText;
            bool[] isRepeat = new bool[operations.Length];
            string[][] operas = new string[operations.Length][];

            for (int iLoop = 0; iLoop < operations.Length; iLoop++)
            {
                string operation = operations[iLoop];
                
                string[] areaDotCell = operation.Split('.');
                ExcelExportProvider provider = Area.Worksheet.ExcelExportProvider;
                
                if (!dictWorksheet.ContainsKey(areaDotCell[0]))
                    dictWorksheet.Add(areaDotCell[0], provider.Worksheets.First(p => p.ID == areaDotCell[0]));
                Worksheet worksheet = dictWorksheet[areaDotCell[0]];
                
                if (!dictArea.ContainsKey(areaDotCell[1]))
                    dictArea.Add(areaDotCell[1], worksheet.Areas.First(p => p.ID == areaDotCell[1]));
                Area area = dictArea[areaDotCell[1]];
                if (area is StaticArea)
                {
                    isRepeat[iLoop] = false;
                    operas[iLoop] = new string[1];
                    
                    string cellName;
                    if (worksheet.ID == Area.Worksheet.ID)
                        cellName = (area as StaticArea).DataCells.First(p => p.ID == areaDotCell[2]).ExcelCellName;
                    else 
                        cellName = worksheet.Name + "!" + (area as StaticArea).DataCells.First(p => p.ID == areaDotCell[2]).ExcelCellName;
                    
                    operas[iLoop][0] = cellName;
                }
                else if (area is RepeatArea)
                {
                    isRepeat[iLoop] = true;
                    RepeatArea repeatArea = area as RepeatArea;
                    
                    if (repeatArea.ID != Area.ID || Area.Worksheet.ID != worksheet.ID)
                    {
                        result.Add("");
                        continue;
                    }
                    
                    string columnNum = repeatArea.DataColumns.First(p => p.ID == areaDotCell[2]).ExcelColumnNum;
                    
                    int topRowSpan = repeatArea.TopRowSpan + repeatArea.Row;
                    operas[iLoop] = new string[rowsFieldInt.Length];
                    for (int jLoop = 0; jLoop < rowsFieldInt.Length; jLoop++)
                    {
                        
                        operas[iLoop][jLoop] = columnNum + (rowsFieldInt[jLoop] + topRowSpan).ToString();
                    }
                }
            }
            if (Formula.ColumnPriority == ColumnPriorityEnums.Row)
            {
                for (int iLoop = 0; iLoop < rowsFieldInt.Length; iLoop++)
                {
                    string[] param = new string[isRepeat.Length];
                    for (int jLoop = 0; jLoop < operas.Length; jLoop++)
                    {
                        if (!isRepeat[jLoop])
                            param[jLoop] = operas[jLoop][0];
                        else
                            param[jLoop] = operas[jLoop][iLoop];
                    }
                    resultFormulaText += string.Format(tempFormulaText, param) + "+";
                }
                resultFormulaText = resultFormulaText.Remove(resultFormulaText.Length - 1);
            }
            else
            {
                string[] param = new string[operas.Length];
                for (int iLoop = 0; iLoop < operas.Length; iLoop++)
                {
                    for (int jLoop = 0; jLoop < operas[iLoop].Length; jLoop++)
                    {
                        param[iLoop] += operas[iLoop][jLoop] + "+";
                    }
                    param[iLoop] = param[iLoop].Remove(param[iLoop].Length - 1);
                }
                resultFormulaText = string.Format(tempFormulaText, param);
            }
            
            
            
            
            return resultFormulaText;
        }

        protected string GetRowFormula(ICell hssfCell, string[] operations)
        {
            List<string> result = new List<string>();
            
            Dictionary<string, Area> dictArea = new Dictionary<string, Area>();
            
            Dictionary<string, Worksheet> dictWorksheet = new Dictionary<string, Worksheet>();
            foreach (string operation in operations)
            {
                string[] areaDotCell = operation.Split('.');

                
                if (areaDotCell.Length == 1) 
                {
                    string columnNum = (Area as RepeatArea).DataColumns.First(p => p.ID == operation).ExcelColumnNum;
                    int rowIndex = hssfCell.RowIndex + 1;
                    result.Add(columnNum + rowIndex.ToString());
                }
                else 
                {
                    ExcelExportProvider provider = Area.Worksheet.ExcelExportProvider;
                    if (!dictWorksheet.ContainsKey(areaDotCell[0]))
                        dictWorksheet.Add(areaDotCell[0], provider.Worksheets.First(p => p.ID == areaDotCell[0]));
                    Worksheet worksheet = dictWorksheet[areaDotCell[0]];
                    if (!dictArea.ContainsKey(areaDotCell[1]))
                        dictArea.Add(areaDotCell[1], worksheet.Areas.First(p => p.ID == areaDotCell[1]));
                    Area area = dictArea[areaDotCell[1]];
                    if (area is StaticArea)
                    {
                        if (worksheet.ID == Area.Worksheet.ID)
                            result.Add((area as StaticArea).
                                DataCells.First(p => p.ID == areaDotCell[2]).ExcelCellName);
                        else
                            result.Add(worksheet.Name + "!" + (area as StaticArea).
                                DataCells.First(p => p.ID == areaDotCell[2]).ExcelCellName);
                    }
                }
            }
            return string.Format(Formula.FormulaText, result.ToArray());
        }

        public void SetCellValue(ICell hssfCell, object value, DataRow dataRow)
        {
            if (Formula == null ||
                (Formula.Type == FormulaTypeEnums.Column && (string.IsNullOrEmpty(Formula.RowsField) || string.IsNullOrEmpty(dataRow[Formula.RowsField] + ""))))
            {
                
                
                if (!string.IsNullOrEmpty(FilterValue))
                {
                    string[] filterValues = FilterValue.Split(',');
                    if (filterValues.Any(filter => string.Compare(value + "", filter, false) == 0))
                    {
                        value = "";
                    }
                }
                switch (DataType)
                {
                    case Export.DataTypeEnums.DateTime:
                        DateTime dateTimeValue;
                        if (DateTime.TryParse(value + "", out dateTimeValue))
                            hssfCell.SetCellValue(dateTimeValue);
                        break;
                    case Export.DataTypeEnums.Numeric:
                        double doubleValue;
                        if (double.TryParse(value + "", out doubleValue))
                        {
                            
                            
                             hssfCell.SetCellValue(doubleValue);
                            
                        }
                        break;
                    case Export.DataTypeEnums.String:
                        hssfCell.SetCellValue(value + "");
                        break;
                    case Export.DataTypeEnums.Boolean:
                        bool boolValue;
                        if (bool.TryParse(value + "", out boolValue))
                            hssfCell.SetCellValue(boolValue);
                        break;
                    case Export.DataTypeEnums.Formula:
                        
                        string valueStr = value + "";
                        if (!string.IsNullOrEmpty(valueStr))
                        {
                            if (valueStr[0] == '=')
                                hssfCell.SetCellFormula(valueStr.Substring(1));
                            else
                            {
                                double doubleFValue;
                                if (double.TryParse(value + "", out doubleFValue))
                                {
                                    hssfCell.SetCellValue(doubleFValue);
                                }
                                else
                                {
                                    hssfCell.SetCellValue(valueStr);
                                }

                            }
                        }
                        break;
                }
            }
            else 
            {
                hssfCell.SetCellFormula(value.ToString());
            }
            
            bool useSummary = false;
            if(!string.IsNullOrEmpty(SummaryFlagField))
            {
                useSummary = !string.IsNullOrEmpty(dataRow[SummaryFlagField]+"");
            }
            SetCellStyle(hssfCell, useSummary);
        }

        protected void SetCellStyle(ICell hssfCell,bool useSummary)
        {
            IWorkbook hssfWorkbook = this.Area.Worksheet.ExcelExportProvider.IWorkbook;
            if (useSummary)
            {
                if (SummaryCellStyle != null)
                    SummaryCellStyle.SetCellStyle(hssfCell, hssfWorkbook);
            }
            else
            {
                if (CellStyle != null)
                    CellStyle.SetCellStyle(hssfCell, hssfWorkbook);
            }

            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            

        }

        protected abstract string GetExcelColumnNum();

        #endregion
    }
}
