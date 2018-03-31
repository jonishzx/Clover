









using System.Data;
using System;
using System.IO;
using System.Linq;


using System.Collections.Generic;
using Clover.Component.Excel.Export.Merged;
using Clover.Component.Excel.Import.EventArgument;
using Clover.Component.Excel.Import;
using Clover.Component.Excel.Export;
using Clover.Component.Excel.Export.EventArgument;
using Clover.Component.Excel.Util;
using Clover.Component.Excel.Common.Validate;
using System.Web;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
namespace Clover.Component.Excel
{

    public partial class ExcelUtilities
    {
        #region 事件声明

        #region Import

        
        
        
        public event EventHandler<ExcelImportArgs> OnBeforeImportExcel;
        
        
        
        public event EventHandler<ExcelImportWorksheetArgs> OnBeforeReadWorksheet;
        
        
        
        public event EventHandler<ExcelImportRowArgs> OnBeforeReadExcelRow;
        
        
        
        public event EventHandler<ExcelImportItemArgs> OnCellItemReading;

        
        
        
        public event EventHandler<ExcelImportArgs> OnAfterImportExcel;
        
        
        
        public event EventHandler<ExcelImportWorksheetArgs> OnAfterWorksheetRead;
        
        
        
        public event EventHandler<ExcelImportRowArgs> OnAfterExcelRowRead;

        #endregion

        #region Export
        
        
        
        public event EventHandler<ExcelExportArgs> OnBeforeExportExcel;
        
        
        
        public event EventHandler<ExcelExportWorksheetArgs> OnBeforeWriteWorksheet;
        
        
        
        public event EventHandler<ExcelExportAreaArgs> OnBeforeWriteArea;
        
        
        
        public event EventHandler<ExcelExportRowArgs> OnBeforeWriteExcelRow;
        
        
        
        public event EventHandler<ExcelExportItemArgs> OnCellItemWriting;

        
        
        
        public event EventHandler<ExcelExportArgs> OnAfterExportExcel;
        
        
        
        public event EventHandler<ExcelExportWorksheetArgs> OnAfterWorksheetWritten;
        
        
        
        public event EventHandler<ExcelExportAreaArgs> OnAfterAreaWritten;
        
        
        
        public event EventHandler<ExcelExportRowArgs> OnAfterExcelRowWritten;
        #endregion

        #endregion

        #region 属性声明

        
        
        
        
        
        public List<object> Parameters
        {
            get;
            set;
        }

        #endregion

        #region ctor

        public ExcelUtilities()
        {
            this.Parameters = new List<object>();
        }

        #endregion

        #region Import
        
        
        
        
        
        
        
        public virtual ExcelResult ImportExcel(DataSet container, string excelFile, string typeName)
        {
            
            ExcelResult result = new ExcelResult();

            
            ExcelImportProvider provider = ExcelImportConfiguration.Current.ExcelConfigs[typeName];

            
            

            #region OnBeforeImportExcel
            ExcelImportArgs excelImportArgs = new ExcelImportArgs(excelFile, container, result.ExcelMessages);
            
            if (OnBeforeImportExcel != null)
            {
                OnBeforeImportExcel(this, excelImportArgs);
                
                if (excelImportArgs.Canceled)
                {
                    ExcelMessageFactory.PrepareEventMessage(result, "OnBeforeImportExcel", excelImportArgs.Canceled);
                    return result;
                }
            }
            #endregion

            
            IWorkbook hssfWorkbook;
            using (FileStream file = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
            {
                hssfWorkbook = new HSSFWorkbook(file);
            }

            
            foreach (Import.Worksheet worksheet in provider.Worksheets)
            {

                
                
               ISheet hssfSheet;
                
                if (string.IsNullOrEmpty(worksheet.Name))
                {
                    hssfSheet = hssfWorkbook.GetSheetAt(0);
                }
                else
                {
                    hssfSheet = hssfWorkbook.GetSheet(worksheet.Name);
                }
                
                if (hssfSheet == null)
                {
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateWorksheet(
                        string.Format("Excel文件中无法找到名称为 {0} 的Worksheet", worksheet.Name), true));
                    break;
                }


                
                DataTable dataTable = container.Tables[worksheet.DataTable];

                #region OnBeforeReadWorksheet
                ExcelImportWorksheetArgs sheetArgs = new ExcelImportWorksheetArgs(hssfSheet, worksheet, dataTable, result.ExcelMessages);

                if (OnBeforeReadWorksheet != null)
                {
                    OnBeforeReadWorksheet(this, sheetArgs);
                    string eventName = "OnBeforeReadWorksheet";
                    ExcelMessageFactory.PrepareSheetMessage(result, worksheet.Name, sheetArgs.WorksheetStatus, eventName);
                    if (sheetArgs.WorksheetStatus == WorksheetStatus.Skip)
                        continue;
                    else if (sheetArgs.WorksheetStatus == WorksheetStatus.Canceled)
                        return result;
                }
                #endregion

                
                int startColIdx = worksheet.Column;
                
                int startRowIdx = worksheet.Row;
                
                int lastRowIdx = hssfSheet.LastRowNum;
                
                int iLoopRow = -1, jLoopCol = -1;

                
                for (iLoopRow = startRowIdx; iLoopRow < lastRowIdx + 1; iLoopRow++)
                {
                    bool breakFromIngore = false;

                    IRow hssfRow = hssfSheet.GetRow(iLoopRow);

                    DataRow dataRow = dataTable.NewRow();
                    
                    ExcelImportRowArgs rowArgs = new ExcelImportRowArgs(dataRow, hssfRow, hssfSheet, worksheet, result.ExcelMessages);

                    
                    #region OnBeforeReadExcelRow

                    if (OnBeforeReadExcelRow != null)
                    {
                        string eventName = "OnBeforeReadExcelRow";
                        OnBeforeReadExcelRow(this, rowArgs);
                        ExcelMessageFactory.PrepareRowMessage(result, iLoopRow, rowArgs.RowStatus, eventName);
                        if (rowArgs.RowStatus == RowStatus.Canceled)
                            return result;
                        else if (rowArgs.RowStatus == RowStatus.Finish)
                            break;
                        else if (rowArgs.RowStatus == RowStatus.Skip)
                            continue;
                    }

                    #endregion

                    
                    foreach (Import.DataColumn dataColunm in worksheet.DataColumns)
                    {
                        
                        dataColunm.SetDefaultNull();
                        jLoopCol = startColIdx + dataColunm.Offset;
                        
                        ICell hssfCell = hssfRow.GetCell(jLoopCol);

                        
                        if (dataColunm.NeedBind)
                        {
                            
                            object oldCellValue = null;
                            object newCellValue = null;
                            if (hssfCell != null)
                            {
                                newCellValue = oldCellValue = GetCellValue(hssfCell, dataColunm);
                            }
                            

                            #region OnCellItemReading

                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            #endregion
                            
                            
                            
                            
                        }
                        else 
                        {
                            object value = dataColunm.DefaultValue;
                            if (value != null)
                                dataRow[dataColunm.Field] = value;
                        }

                        
                        if (!dataColunm.AllowNull && string.IsNullOrEmpty(dataRow[dataColunm.Field] + ""))
                        {
                            result.ExcelMessages.Add(ExcelMessageFactory.GenerateEvent(
                                        string.Format("第 {0} 行第 {1} 列不允许为空", iLoopRow, jLoopCol), true));
                            result.Result = ExcelResultType.Failed;
                            return result;
                        }
                    }

                    if (!breakFromIngore)
                    {
                        
                        #region OnAfterExcelRowRead
                        if (OnAfterExcelRowRead != null)
                        {
                            
                            rowArgs.RowStatus = RowStatus.Continue;
                            rowArgs.DataRow = dataRow;
                            string eventName = "OnAfterExcelRowRead";
                            OnAfterExcelRowRead(this, rowArgs);
                            ExcelMessageFactory.PrepareRowMessage(result, iLoopRow, rowArgs.RowStatus, eventName);
                            if (rowArgs.RowStatus == RowStatus.Canceled)
                                return result;
                            else if (rowArgs.RowStatus == RowStatus.Finish)
                                break;
                            else if (rowArgs.RowStatus == RowStatus.Skip)
                                continue;
                        }
                        #endregion
                        
                        
                        dataTable.Rows.Add(dataRow);
                    }
                       
                }

                
                #region OnAfterWorksheetRead
                if (OnAfterWorksheetRead != null)
                {
                    sheetArgs.DataTable = dataTable;
                    sheetArgs.WorksheetStatus = WorksheetStatus.Continue;
                    OnAfterWorksheetRead(this, sheetArgs);
                    string eventName = "OnAfterWorksheetRead";
                    ExcelMessageFactory.PrepareSheetMessage(result, worksheet.Name, sheetArgs.WorksheetStatus, eventName);
                    if (sheetArgs.WorksheetStatus == WorksheetStatus.Skip)
                        continue;
                    else if (sheetArgs.WorksheetStatus == WorksheetStatus.Canceled)
                        return result;
                }
                #endregion
            }

            
            #region OnAfterImportExcel
            if (OnAfterImportExcel != null)
            {
                excelImportArgs.Canceled = false;
                excelImportArgs.Container = container;
                OnAfterImportExcel(this, excelImportArgs);
                
                if (excelImportArgs.Canceled)
                {
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateEvent("在事件OnAfterImportExcel中被取消", true));
                    result.Result = ExcelResultType.Canceled;
                    return result;
                }
            }
            #endregion

            

            if (result.ExcelMessages.Count(p => p.IsException) == 0)
                result.Result = ExcelResultType.Succeed;
            else
                result.Result = ExcelResultType.CompletedWithException;

            return result;
        }

        
        
        
        
        
        
        
        public virtual ExcelResult ImportExcel(List<Import.Worksheet> container, string excelFile, string typeName)
        {
            
            if (container == null)
            {
                container = new List<Import.Worksheet>();
            }

            
            ExcelResult result = new ExcelResult();

            
            ExcelImportProvider provider = ExcelImportConfiguration.Current.ExcelConfigs[typeName];

            
            ConfigImportValidate(provider, excelFile, typeName);

            #region OnBeforeImportExcel
            
            
            
            
            
            
            
            
            
            
            
            
            #endregion

            
            IWorkbook hssfWorkbook = GetHSSFWorkbook(excelFile);

            
            foreach (Import.Worksheet item in provider.Worksheets)
            {
                
                Import.Worksheet worksheet = item.GetCopyWorkSheet();

                
                ISheet hssfSheet = GetHSSFSheet(hssfWorkbook, worksheet);

                
                if (hssfSheet == null)
                {
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateWorksheet(
                        string.Format("Excel文件中无法找到名称为 {0} 的Worksheet", worksheet.Name), true));
                    result.Result = ExcelResultType.Canceled;

                    return result;
                }

                #region OnBeforeReadWorksheet
                

                
                
                
                
                
                
                
                
                
                
                #endregion

                int startColIdx = worksheet.Column; 
                int startRowIdx = worksheet.Row; 
                int lastRowIdx = hssfSheet.LastRowNum; 
                int iLoopRow = -1; 
                int jLoopCol = -1; 

                
                for (iLoopRow = startRowIdx; iLoopRow < lastRowIdx + 1; iLoopRow++)
                {
                    IRow hssfRow = hssfSheet.GetRow(iLoopRow); 
                    Dictionary<string, object> dataRow = new Dictionary<string, object>(); 
                    bool isLegalRowResult = true; 

                    
                    #region OnBeforeReadExcelRow
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    

                    #endregion

                    
                    foreach (Import.DataColumn dataColunm in worksheet.DataColumns)
                    {
                        
                        dataColunm.SetDefaultNull(); 
                        
                        jLoopCol = startColIdx + dataColunm.Offset;

                        ICell hssfCell = hssfRow.GetCell(jLoopCol); 
                        if (hssfCell == null)
                        {
                            hssfCell = hssfRow.CreateCell(jLoopCol);
                        }

                        object cellValue = null; 

                        
                        if (dataColunm.NeedBind)
                        {
                            cellValue = GetCellValue(hssfCell, dataColunm);
                        }
                        else 
                        {
                            cellValue = dataColunm.DefaultValue;
                        }

                        
                        OperationState validstate = new OperationState();
                        
                        
                        
                        
                        
                        

                        
                        

                        #region OnCellItemReading
                        ExcelImportItemArgs itemArgs = new ExcelImportItemArgs(iLoopRow, jLoopCol, cellValue, dataColunm, hssfCell, validstate, dataRow);
                        
                        if (OnCellItemReading != null)
                        {
                            OnCellItemReading(this, itemArgs);
                            ExcelMessageFactory.PrepareCellMessage(result, iLoopRow, jLoopCol, itemArgs.CellStatus, "OnCellItemReading");
                            if (itemArgs.CellStatus == CellStatus.Canceled)
                                return result;
                            else if (itemArgs.CellStatus == CellStatus.IngoreRow)
                                break;
                        }
                        #endregion

                        
                        

                        
                        if (validstate.Result)
                        {
                            
                            dataRow.Add(dataColunm.Field, cellValue);
                        }
                        else
                        {
                            
                            
                            
                            
                            
                            


                            
                            ICellStyle errorStyle = hssfWorkbook.CreateCellStyle();
                            errorStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.RED.index;
                            errorStyle.FillPattern = FillPatternType.FINE_DOTS;
                            errorStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.RED.index;

                            
                            IFont font = hssfWorkbook.CreateFont();
                            font.IsItalic = true;
                            
                            errorStyle.SetFont(font);

                            hssfCell.CellStyle = errorStyle;


                            dataRow.Add(dataColunm.Field, null);
                            hssfCell.SetCellValue(cellValue + "\n" + validstate.MessageList[0]);

                            
                            isLegalRowResult = false;
                            result.Result = ExcelResultType.Failed;
                            
                            
                            foreach (string error in validstate.FullMessageList)
                            {
                                ExcelMessage message = new ExcelMessage();
                                message.Message = error;

                                result.ExcelMessages.Add(message);
                            }
                        }

                    }

                    
                    if(isLegalRowResult)
                    {
                        
                        worksheet.DataList.Add(dataRow);
                    }

                    
                    #region OnAfterExcelRowRead
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    #endregion
                }

                
                container.Add(worksheet);

                
                #region OnAfterWorksheetRead
                
                
                
                
                
                
                
                
                
                
                
                
                #endregion
            }

            
            #region OnAfterImportExcel
            
            
            
            
            
            
            
            
            
            
            
            
            
            #endregion

            if (result.Result == ExcelResultType.Failed)
            {
                
                FileStream file = new FileStream(excelFile, FileMode.Create);
                hssfWorkbook.Write(file);
                file.Close();
            }

            return result;
        }

        
        
        
        
        
        
        private static ISheet GetHSSFSheet(IWorkbook hssfWorkbook, Import.Worksheet worksheet)
        {
            ISheet hssfSheet;
            
            if (string.IsNullOrEmpty(worksheet.Name))
            {
                hssfSheet = hssfWorkbook.GetSheetAt(0);
            }
            else
            {
                hssfSheet = hssfWorkbook.GetSheet(worksheet.Name);
            }
            return hssfSheet;
        }

        
        
        
        
        
        private static IWorkbook GetHSSFWorkbook(string excelFile)
        {
            using (FileStream file = new FileStream(excelFile, FileMode.Open, FileAccess.ReadWrite))
            {
                return new HSSFWorkbook(file);
            }
        }

        
        
        
        
        
        
        
        
        protected virtual bool ConfigImportValidate(ExcelImportProvider provider, string excelFile, string typeName)
        {
           
            
            if (!File.Exists(excelFile)) throw new FileNotFoundException(string.Format("无法找到文件:{0}!", excelFile));
            

            if (provider == null) throw new Exception(string.Format("配置中不存在类型为:{0}的导入配置项!", typeName));
            
            if (provider.Worksheets == null || provider.Worksheets.Count == 0) throw new Exception(string.Format("配置中类型为:{0}的配置节中不包含Worksheet的定义", typeName));

            
            int nullNameWorksheetCount = 0;
            if ((nullNameWorksheetCount = provider.Worksheets.Count(p => string.IsNullOrEmpty(p.Name))) > 1) throw new Exception(string.Format("配置中类型为:{0}的配置节中存在 {1} 项Worksheet名字为空的项", typeName, nullNameWorksheetCount));

            return true;
        }

        
        
        
        
        
        
        protected object GetCellValue(ICell hssfCell, Import.DataColumn dataColumn)
        {
            ICell valueCell = hssfCell;

            
            if (dataColumn.HasMerged)
            {
                valueCell = GetRealValueCell(hssfCell);
            }

            if (string.IsNullOrEmpty(dataColumn.Match))
            {
                switch (dataColumn.DataType)
                {
                    case Import.DataTypeEnums.String:
                        try
                        {
                            string value = valueCell.StringCellValue;
                            return value;
                        }
                        catch
                        {
                            try
                            {
                                return valueCell + "";
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    case Import.DataTypeEnums.Numeric:
                        try
                        {
                            if (valueCell + "" == "")
                                return null;
                            double value = valueCell.NumericCellValue;
                            return value;
                        }
                        catch
                        {
                            return null;
                        }
                    case Import.DataTypeEnums.DateTime:
                        try
                        {
                            string cellContent = valueCell + "";
                            if (string.IsNullOrEmpty(cellContent))
                            {
                                return null;
                            }
                            DateTime dateTime;
                            bool parseResult = DateTime.TryParse(cellContent, out dateTime);
                            if (parseResult)
                            {
                                return dateTime;
                            }
                            else
                            {
                                DateTime value = valueCell.DateCellValue;
                                return value;
                            }
                        }
                        catch
                        {
                            return null;
                        }
                    default:
                        return null;
                }
            }
            else
            {
                return dataColumn.GetMatchValue(valueCell);
            }
        }

        
        
        
        
        
        private ICell GetRealValueCell(ICell hssfCell)
        {
            ISheet hssfSheet = hssfCell.Sheet;
            int rowIndex = hssfCell.RowIndex;
            int colIndex = hssfCell.ColumnIndex;

            for (int i = 0; i < hssfSheet.NumMergedRegions; i++)
            {
                CellRangeAddress region = hssfSheet.GetMergedRegion(i);
                
                if (rowIndex >= region.FirstRow && rowIndex <= region.LastRow && colIndex >= region.FirstColumn && colIndex <= region.LastColumn)
                {
                    return hssfSheet.GetRow(region.FirstRow).GetCell(region.FirstColumn);
                }
            }

            return hssfCell;
        }

        #endregion

        #region Export
        
        
        
        
        
        
        
        
        public virtual ExcelResult ExportExcel(DataSet container, string excelFile, string typeName, string templateFile)
        {
            ExcelExportProvider provider = null;
            if (ExcelExportConfiguration.IsActive)
            {
                provider = ExcelExportConfiguration.ExcelConfigsActive[typeName];
                ExcelExportConfiguration.IsActive = false;
            }
            else
                provider = ExcelExportConfiguration.Current.ExcelConfigs[typeName];
            
            
            ConfigExportValidate(provider, container, typeName);
           
            ExcelResult result = new ExcelResult();

            
            string fileName;
            if (!string.IsNullOrEmpty(templateFile))
                fileName = templateFile;
            else if (!string.IsNullOrEmpty(provider.TemplateFile))
                fileName = provider.TemplateFile;
            else
                throw new Exception("参数excelFile及预设的TemplateFile都为空，无法打开Excel文件");

            
            FileStream sourcefs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            IWorkbook hssfWorkbook = new HSSFWorkbook(sourcefs);

            provider.IWorkbook = hssfWorkbook;


            #region OnBeforeExportExcel 事件

            ExcelExportArgs xlsEptArgs = new ExcelExportArgs(fileName, container, result.ExcelMessages,provider);
            if (OnBeforeExportExcel != null)
            {
                OnBeforeExportExcel(this, xlsEptArgs);

                if (xlsEptArgs.Canceled)
                {
                    ExcelMessageFactory.PrepareEventMessage(result, "OnBeforeExportExcel", xlsEptArgs.Canceled);
                    return result;
                }
            }
            #endregion

            
            foreach (Export.Worksheet worksheet in provider.Worksheets)
            {
                ISheet hssfSheet = hssfWorkbook.GetSheet(worksheet.Name);

                
                if (hssfSheet == null)
                {
                    hssfSheet = hssfWorkbook.CreateSheet(worksheet.Name);
                    ISheet oldSheet = hssfWorkbook.GetSheet(worksheet.RefSheetName);
                    if (oldSheet==null)
                        throw new Exception(string.Format("不存在Sheet名称为{0}的工作表", worksheet.RefSheetName));

                    NpoiUtil.CopySheet(oldSheet, hssfWorkbook, hssfSheet);

                    
                }
                worksheet.ISheet = hssfSheet;

                #region OnBeforeWriteWorksheet 事件

                ExcelExportWorksheetArgs xlsEptSheetArgs = new ExcelExportWorksheetArgs(hssfSheet, worksheet, result.ExcelMessages, container);
                if (OnBeforeWriteWorksheet != null)
                {
                    OnBeforeWriteWorksheet(this, xlsEptSheetArgs);
                    string eventName = "OnBeforeReadWorksheet";
                    ExcelMessageFactory.PrepareSheetMessage(result, worksheet.Name, xlsEptSheetArgs.WorksheetStatus, eventName);
                    if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Skip)
                        continue;
                    else if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Canceled)
                        return result;
                }

                #endregion

                
                
                int totalRowSpan = worksheet.Row;
                foreach (Clover.Component.Excel.Export.Area area in worksheet.Areas)
                {
                    DataTable dataTable = container.Tables[area.DataTable];

                    #region OnBeforeWriteArea 事件

                    ExcelExportAreaArgs xlsAreaArgs = new ExcelExportAreaArgs(hssfSheet, area, dataTable, result.ExcelMessages);
                    if (OnBeforeWriteArea != null)
                    {
                        OnBeforeWriteArea(this, xlsAreaArgs);
                        string eventName = "OnBeforeWriteArea";
                        ExcelMessageFactory.PrepareAreaMessage(result, worksheet.Name, xlsAreaArgs.AreaStatus, eventName);
                        if (xlsAreaArgs.AreaStatus == AreaStatus.Skip)
                            continue;
                        else if (xlsAreaArgs.AreaStatus == AreaStatus.Canceled)
                            return result;
                    }
                    #endregion

                    totalRowSpan += area.Row; 
                    int originRowSpan = totalRowSpan;

                    if (area.IsRepeat)
                    {
                        
                        if (hssfSheet.GetRow(totalRowSpan) == null)
                            hssfSheet.CreateRow(totalRowSpan);

                        if (!area.IsStaticArea&&dataTable.Rows.Count != 0)
                            hssfSheet.ShiftRows(totalRowSpan, hssfSheet.LastRowNum, dataTable.Rows.Count);

                        
                        RepeatArea repeatArea = area as RepeatArea;
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            IRow hssfRow = GetRow(hssfSheet, totalRowSpan);
                            bool breakFromIngoreRow = false;
                            #region OnBeforeWriteExcelRow 事件

                            ExcelExportRowArgs xlsRowArgs = new ExcelExportRowArgs(dataRow, hssfRow, hssfSheet,hssfWorkbook,
                                area, result.ExcelMessages);
                            if (OnBeforeWriteExcelRow != null)
                            {
                                OnBeforeWriteExcelRow(this, xlsRowArgs);
                                string eventName = "OnBeforeWriteExcelRow";
                                ExcelMessageFactory.PrepareRowMessage(result, hssfRow.RowNum, xlsRowArgs.RowStatus, eventName);
                                if (xlsRowArgs.RowStatus == RowStatus.Skip)
                                {
                                    SkipRow(hssfSheet, totalRowSpan, hssfRow);
                                    continue;
                                }
                                else if (xlsRowArgs.RowStatus == RowStatus.Canceled)
                                    return result;
                            }

                            #endregion

                            
                            foreach (Export.DataColumn dataColumn in (repeatArea).DataColumns)
                            {
                                ICell hssfCell = GetCell(hssfRow, worksheet.Column + area.Column + dataColumn.Offset);

                                
                                dataColumn.DataRow = dataRow;

                                object oldValue, newValue;
                                oldValue = newValue = dataColumn.GetCellPrepareValue(hssfCell, dataRow);

                                #region OnCellItemWriting
                                ExcelExportItemArgs xlsItemArgs = new ExcelExportItemArgs(hssfCell.RowIndex, hssfCell.ColumnIndex,
                                    oldValue, newValue, dataColumn, hssfCell, result.ExcelMessages);
                                if (OnCellItemWriting != null)
                                {
                                    OnCellItemWriting(this, xlsItemArgs);
                                    breakFromIngoreRow = ExcelMessageFactory.PrepareCellMessage(result, hssfCell.RowIndex, hssfCell.ColumnIndex, xlsItemArgs.CellStatus, "OnCellItemWriting");
                                    if (xlsItemArgs.CellStatus == CellStatus.IngoreRow)
                                    {
                                        SkipRow(hssfSheet, totalRowSpan, hssfRow);
                                        break;
                                    }
                                    else if (xlsItemArgs.CellStatus == CellStatus.Canceled)
                                        return result;
                                }
                                #endregion

                                dataColumn.SetCellValue(hssfCell, xlsItemArgs.NewValue, dataRow);
                            }

                            if (!breakFromIngoreRow)
                            {
                                #region OnAfterExcelRowWritten 事件

                                xlsRowArgs = new ExcelExportRowArgs(dataRow, hssfRow, hssfSheet,hssfWorkbook,
                                    area, result.ExcelMessages);
                                if (OnAfterExcelRowWritten != null)
                                {
                                    OnAfterExcelRowWritten(this, xlsRowArgs);
                                    string eventName = "OnAfterExcelRowWritten";
                                    ExcelMessageFactory.PrepareRowMessage(result, hssfRow.RowNum, xlsRowArgs.RowStatus, eventName);
                                    
                                    if (xlsRowArgs.RowStatus == RowStatus.Skip)
                                    {
                                        SkipRow(hssfSheet, totalRowSpan, hssfRow);
                                        continue;
                                    }
                                    else if (xlsRowArgs.RowStatus == RowStatus.Canceled)
                                        return result;
                                }
                                #endregion

                                

                                foreach (Export.DataColumn dataColumn in (repeatArea).DataColumns)
                                {
                                    
                                    if ((repeatArea.MergeTop.ContainsKey(dataColumn.ID) && repeatArea.MergeTop[dataColumn.ID]) || totalRowSpan != originRowSpan)
                                    {
                                        if (dataColumn.IsMergeColumn)
                                        {
                                            int columnIndex = worksheet.Column + area.Column + dataColumn.Offset;
                                            ICell hssfCell = GetCell(hssfRow, columnIndex);
                                            ICell hssfCell_1 = GetCell(GetRow(hssfSheet, hssfRow.RowNum - 1), columnIndex);
                                            if (hssfCell + "" == hssfCell_1 + "")
                                            {
                                                
                                                hssfSheet.AddMergedRegion(new CellRangeAddress(hssfRow.RowNum - 1,
                                                                                     hssfCell.ColumnIndex, hssfRow.RowNum,
                                                                                     hssfCell.ColumnIndex));
                                                
                                                
                                                
                                                
                                            }
                                        }
                                    }
                                }

                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                


                                totalRowSpan++; 
                            }
                        }
                        area.RowSpan = totalRowSpan - originRowSpan;

                        List<MergedConfig> mergedconfigs = worksheet.MergedConfigs.Where(config => config.AreaId == area.ID).ToList();
                        if(mergedconfigs!=null&&mergedconfigs.Count>0)
                        {
                            
                            Merged merged = new Merged();
                            merged.CurrentSheet = hssfSheet;
                            merged.MergedConfigs = mergedconfigs;
                            merged.StartRow = originRowSpan;
                            merged.EndRow = totalRowSpan;
                            merged.DoMerged();
                        }
                    } 
                    else
                    {
                        
                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow dataRow = dataTable.Rows[0];
                            
                            foreach (Export.DataCell dataCell in (area as StaticArea).DataCells)
                            {
                                ICell hssfCell = GetCell(GetRow(hssfSheet, totalRowSpan + dataCell.Row),
                                    worksheet.Column + area.Column + dataCell.Column);

                                
                                dataCell.DataRow = dataRow;

                                #region OnCellItemWriting

                                object oldValue, newValue;
                                oldValue = newValue = dataCell.GetCellPrepareValue(hssfCell, dataRow);
                                ExcelExportItemArgs xlsItemArgs = new ExcelExportItemArgs(hssfCell.RowIndex, hssfCell.ColumnIndex,
                                    oldValue, newValue, dataCell, hssfCell, result.ExcelMessages);
                                if (OnCellItemWriting != null)
                                {
                                    OnCellItemWriting(this, xlsItemArgs);
                                    ExcelMessageFactory.PrepareCellMessage(result, hssfCell.RowIndex, hssfCell.ColumnIndex, xlsItemArgs.CellStatus, "OnCellItemWriting");
                                    if (xlsItemArgs.CellStatus == CellStatus.IngoreRow) 
                                        continue;
                                    else if (xlsItemArgs.CellStatus == CellStatus.Canceled)
                                        return result;
                                }

                                #endregion

                                dataCell.SetCellValue(hssfCell, xlsItemArgs.NewValue, dataRow);
                            }
                        }
                        totalRowSpan += (area as StaticArea).RowSpan; 
                    }
                    #region OnAfterAreaWritten 事件

                    xlsAreaArgs = new ExcelExportAreaArgs(hssfSheet, area, dataTable, result.ExcelMessages);
                    if (OnAfterAreaWritten != null)
                    {
                        OnAfterAreaWritten(this, xlsAreaArgs);
                        string eventName = "OnAfterAreaWritten";
                        ExcelMessageFactory.PrepareAreaMessage(result, worksheet.Name, xlsAreaArgs.AreaStatus, eventName);
                        if (xlsAreaArgs.AreaStatus == AreaStatus.Skip)
                        {
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            continue;
                        }
                        else if (xlsAreaArgs.AreaStatus == AreaStatus.Canceled)
                            return result;
                    }

                    #endregion

                }

                #region OnAfterWorksheetWritten 事件

                xlsEptSheetArgs = new ExcelExportWorksheetArgs(hssfSheet, worksheet, result.ExcelMessages, container);
                if (OnAfterWorksheetWritten != null)
                {
                    OnAfterWorksheetWritten(this, xlsEptSheetArgs);
                    string eventName = "OnAfterWorksheetWritten";
                    ExcelMessageFactory.PrepareSheetMessage(result, worksheet.Name, xlsEptSheetArgs.WorksheetStatus, eventName);
                    if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Skip)
                        continue;
                    else if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Canceled)
                        return result;
                }

                #endregion

                
                worksheet.AutoSizeColumn(hssfSheet);
                
                hssfSheet.ForceFormulaRecalculation = true;
                
            }

            #region OnAfterExportExcel

            xlsEptArgs = new ExcelExportArgs(fileName, container, result.ExcelMessages,provider);
            if (OnAfterExportExcel != null)
            {
                OnAfterExportExcel(this, xlsEptArgs);
                if (xlsEptArgs.Canceled)
                {
                    ExcelMessageFactory.PrepareEventMessage(result, "OnAfterExportExcel", xlsEptArgs.Canceled);
                    return result;
                }
            }
            #endregion

            
            string saveDirectory = excelFile.Substring(0, excelFile.LastIndexOf("\\"));
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            FileStream targetfs = new FileStream(excelFile, FileMode.Create, FileAccess.Write);
            hssfWorkbook.Write(targetfs);

            
            if (result.ExcelMessages.Count(p => p.IsException) == 0)
                result.Result = ExcelResultType.Succeed;
            else
                result.Result = ExcelResultType.CompletedWithException;

            sourcefs.Close();
            targetfs.Close();

            return result;
        }

        
        
        
        
        
        
        
        
        public virtual void ExportExcel(HttpResponse response, string saveAsFileName, DataSet container, string typeName, string templateFile)
        {
            ExcelExportProvider provider = null;
            if (ExcelExportConfiguration.IsActive)
            {
                provider = ExcelExportConfiguration.ExcelConfigsActive[typeName];
                ExcelExportConfiguration.IsActive = false;
            }
            else
                provider = ExcelExportConfiguration.Current.ExcelConfigs[typeName];

            
            ConfigExportValidate(provider, container, typeName);

            ExcelResult result = new ExcelResult();

            
            string fileName;
            if (!string.IsNullOrEmpty(templateFile))
                fileName = templateFile;
            else if (!string.IsNullOrEmpty(provider.TemplateFile))
                fileName = provider.TemplateFile;
            else
                throw new Exception("参数excelFile及预设的TemplateFile都为空，无法打开Excel文件");

            
            FileStream sourcefs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            IWorkbook hssfWorkbook = new HSSFWorkbook(sourcefs);

            provider.IWorkbook = hssfWorkbook;


            #region OnBeforeExportExcel 事件

            ExcelExportArgs xlsEptArgs = new ExcelExportArgs(fileName, container, result.ExcelMessages, provider);
            if (OnBeforeExportExcel != null)
            {
                OnBeforeExportExcel(this, xlsEptArgs);

                if (xlsEptArgs.Canceled)
                {
                    ExcelMessageFactory.PrepareEventMessage(result, "OnBeforeExportExcel", xlsEptArgs.Canceled);
                    return ;
                }
            }
            #endregion

            
            foreach (Export.Worksheet worksheet in provider.Worksheets)
            {
                ISheet hssfSheet = hssfWorkbook.GetSheet(worksheet.Name);

                
                if (hssfSheet == null)
                {
                    hssfSheet = hssfWorkbook.CreateSheet(worksheet.Name);
                    ISheet oldSheet = hssfWorkbook.GetSheet(worksheet.RefSheetName);
                    if (oldSheet == null)
                        throw new Exception(string.Format("不存在Sheet名称为{0}的工作表", worksheet.RefSheetName));

                    NpoiUtil.CopySheet(oldSheet, hssfWorkbook, hssfSheet);

                    
                }
                worksheet.ISheet = hssfSheet;

                #region OnBeforeWriteWorksheet 事件

                ExcelExportWorksheetArgs xlsEptSheetArgs = new ExcelExportWorksheetArgs(hssfSheet, worksheet, result.ExcelMessages, container);
                if (OnBeforeWriteWorksheet != null)
                {
                    OnBeforeWriteWorksheet(this, xlsEptSheetArgs);
                    string eventName = "OnBeforeReadWorksheet";
                    ExcelMessageFactory.PrepareSheetMessage(result, worksheet.Name, xlsEptSheetArgs.WorksheetStatus, eventName);
                    if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Skip)
                        continue;
                    else if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Canceled)
                        return ;
                }

                #endregion

                
                
                int totalRowSpan = worksheet.Row;
                foreach (Area area in worksheet.Areas)
                {
                    DataTable dataTable = container.Tables[area.DataTable];

                    #region OnBeforeWriteArea 事件

                    ExcelExportAreaArgs xlsAreaArgs = new ExcelExportAreaArgs(hssfSheet, area, dataTable, result.ExcelMessages);
                    if (OnBeforeWriteArea != null)
                    {
                        OnBeforeWriteArea(this, xlsAreaArgs);
                        string eventName = "OnBeforeWriteArea";
                        ExcelMessageFactory.PrepareAreaMessage(result, worksheet.Name, xlsAreaArgs.AreaStatus, eventName);
                        if (xlsAreaArgs.AreaStatus == AreaStatus.Skip)
                            continue;
                        else if (xlsAreaArgs.AreaStatus == AreaStatus.Canceled)
                            return ;
                    }
                    #endregion

                    totalRowSpan += area.Row; 
                    int originRowSpan = totalRowSpan;

                    if (area.IsRepeat)
                    {
                        
                        if (hssfSheet.GetRow(totalRowSpan) == null)
                            hssfSheet.CreateRow(totalRowSpan);

                        if (!area.IsStaticArea && dataTable.Rows.Count != 0)
                            hssfSheet.ShiftRows(totalRowSpan, hssfSheet.LastRowNum, dataTable.Rows.Count);

                        
                        RepeatArea repeatArea = area as RepeatArea;
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            IRow hssfRow = GetRow(hssfSheet, totalRowSpan);
                            bool breakFromIngoreRow = false;
                            #region OnBeforeWriteExcelRow 事件

                            ExcelExportRowArgs xlsRowArgs = new ExcelExportRowArgs(dataRow, hssfRow, hssfSheet, hssfWorkbook,
                                area, result.ExcelMessages);
                            if (OnBeforeWriteExcelRow != null)
                            {
                                OnBeforeWriteExcelRow(this, xlsRowArgs);
                                string eventName = "OnBeforeWriteExcelRow";
                                ExcelMessageFactory.PrepareRowMessage(result, hssfRow.RowNum, xlsRowArgs.RowStatus, eventName);
                                if (xlsRowArgs.RowStatus == RowStatus.Skip)
                                {
                                    SkipRow(hssfSheet, totalRowSpan, hssfRow);
                                    continue;
                                }
                                else if (xlsRowArgs.RowStatus == RowStatus.Canceled)
                                    return ;
                            }

                            #endregion

                            
                            foreach (Export.DataColumn dataColumn in (repeatArea).DataColumns)
                            {
                                ICell hssfCell = GetCell(hssfRow, worksheet.Column + area.Column + dataColumn.Offset);

                                
                                dataColumn.DataRow = dataRow;

                                object oldValue, newValue;
                                oldValue = newValue = dataColumn.GetCellPrepareValue(hssfCell, dataRow);

                                #region OnCellItemWriting
                                ExcelExportItemArgs xlsItemArgs = new ExcelExportItemArgs(hssfCell.RowIndex, hssfCell.ColumnIndex,
                                    oldValue, newValue, dataColumn, hssfCell, result.ExcelMessages);
                                if (OnCellItemWriting != null)
                                {
                                    OnCellItemWriting(this, xlsItemArgs);
                                    breakFromIngoreRow = ExcelMessageFactory.PrepareCellMessage(result, hssfCell.RowIndex, hssfCell.ColumnIndex, xlsItemArgs.CellStatus, "OnCellItemWriting");
                                    if (xlsItemArgs.CellStatus == CellStatus.IngoreRow)
                                    {
                                        SkipRow(hssfSheet, totalRowSpan, hssfRow);
                                        break;
                                    }
                                    else if (xlsItemArgs.CellStatus == CellStatus.Canceled)
                                        return ;
                                }
                                #endregion

                                dataColumn.SetCellValue(hssfCell, xlsItemArgs.NewValue, dataRow);
                            }

                            if (!breakFromIngoreRow)
                            {
                                #region OnAfterExcelRowWritten 事件

                                xlsRowArgs = new ExcelExportRowArgs(dataRow, hssfRow, hssfSheet, hssfWorkbook,
                                    area, result.ExcelMessages);
                                if (OnAfterExcelRowWritten != null)
                                {
                                    OnAfterExcelRowWritten(this, xlsRowArgs);
                                    string eventName = "OnAfterExcelRowWritten";
                                    ExcelMessageFactory.PrepareRowMessage(result, hssfRow.RowNum, xlsRowArgs.RowStatus, eventName);
                                    
                                    if (xlsRowArgs.RowStatus == RowStatus.Skip)
                                    {
                                        SkipRow(hssfSheet, totalRowSpan, hssfRow);
                                        continue;
                                    }
                                    else if (xlsRowArgs.RowStatus == RowStatus.Canceled)
                                        return ;
                                }
                                #endregion

                                

                                foreach (Export.DataColumn dataColumn in (repeatArea).DataColumns)
                                {
                                    
                                    if ((repeatArea.MergeTop.ContainsKey(dataColumn.ID) && repeatArea.MergeTop[dataColumn.ID]) || totalRowSpan != originRowSpan)
                                    {
                                        if (dataColumn.IsMergeColumn)
                                        {
                                            int columnIndex = worksheet.Column + area.Column + dataColumn.Offset;
                                            ICell hssfCell = GetCell(hssfRow, columnIndex);
                                            ICell hssfCell_1 = GetCell(GetRow(hssfSheet, hssfRow.RowNum - 1), columnIndex);
                                            if (hssfCell + "" == hssfCell_1 + "")
                                            {
                                                
                                                hssfSheet.AddMergedRegion(new CellRangeAddress(hssfRow.RowNum - 1,
                                                                                     hssfCell.ColumnIndex, hssfRow.RowNum,
                                                                                     hssfCell.ColumnIndex));

                                                
                                                
                                                
                                            }
                                        }
                                    }
                                }

                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                


                                totalRowSpan++; 
                            }
                        }
                        area.RowSpan = totalRowSpan - originRowSpan;

                        List<MergedConfig> mergedconfigs = worksheet.MergedConfigs.Where(config => config.AreaId == area.ID).ToList();
                        if (mergedconfigs != null && mergedconfigs.Count > 0)
                        {
                            
                            Merged merged = new Merged();
                            merged.CurrentSheet = hssfSheet;
                            merged.MergedConfigs = mergedconfigs;
                            merged.StartRow = originRowSpan;
                            merged.EndRow = totalRowSpan;
                            merged.DoMerged();
                        }
                    }
                    else
                    {
                        
                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow dataRow = dataTable.Rows[0];
                            
                            foreach (Export.DataCell dataCell in (area as StaticArea).DataCells)
                            {
                                ICell hssfCell = GetCell(GetRow(hssfSheet, totalRowSpan + dataCell.Row),
                                    worksheet.Column + area.Column + dataCell.Column);

                                
                                dataCell.DataRow = dataRow;

                                #region OnCellItemWriting

                                object oldValue, newValue;
                                oldValue = newValue = dataCell.GetCellPrepareValue(hssfCell, dataRow);
                                ExcelExportItemArgs xlsItemArgs = new ExcelExportItemArgs(hssfCell.RowIndex, hssfCell.ColumnIndex,
                                    oldValue, newValue, dataCell, hssfCell, result.ExcelMessages);
                                if (OnCellItemWriting != null)
                                {
                                    OnCellItemWriting(this, xlsItemArgs);
                                    ExcelMessageFactory.PrepareCellMessage(result, hssfCell.RowIndex, hssfCell.ColumnIndex, xlsItemArgs.CellStatus, "OnCellItemWriting");
                                    if (xlsItemArgs.CellStatus == CellStatus.IngoreRow) 
                                        continue;
                                    else if (xlsItemArgs.CellStatus == CellStatus.Canceled)
                                        return ;
                                }

                                #endregion

                                dataCell.SetCellValue(hssfCell, xlsItemArgs.NewValue, dataRow);
                            }
                        }
                        totalRowSpan += (area as StaticArea).RowSpan; 
                    }
                    #region OnAfterAreaWritten 事件

                    xlsAreaArgs = new ExcelExportAreaArgs(hssfSheet, area, dataTable, result.ExcelMessages);
                    if (OnAfterAreaWritten != null)
                    {
                        OnAfterAreaWritten(this, xlsAreaArgs);
                        string eventName = "OnAfterAreaWritten";
                        ExcelMessageFactory.PrepareAreaMessage(result, worksheet.Name, xlsAreaArgs.AreaStatus, eventName);
                        if (xlsAreaArgs.AreaStatus == AreaStatus.Skip)
                        {
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            continue;
                        }
                        else if (xlsAreaArgs.AreaStatus == AreaStatus.Canceled)
                            return ;
                    }

                    #endregion

                }

                #region OnAfterWorksheetWritten 事件

                xlsEptSheetArgs = new ExcelExportWorksheetArgs(hssfSheet, worksheet, result.ExcelMessages, container);
                if (OnAfterWorksheetWritten != null)
                {
                    OnAfterWorksheetWritten(this, xlsEptSheetArgs);
                    string eventName = "OnAfterWorksheetWritten";
                    ExcelMessageFactory.PrepareSheetMessage(result, worksheet.Name, xlsEptSheetArgs.WorksheetStatus, eventName);
                    if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Skip)
                        continue;
                    else if (xlsEptSheetArgs.WorksheetStatus == WorksheetStatus.Canceled)
                        return ;
                }

                #endregion

                
                worksheet.AutoSizeColumn(hssfSheet);
                
                hssfSheet.ForceFormulaRecalculation = true;
                
            }

            #region OnAfterExportExcel

            xlsEptArgs = new ExcelExportArgs(fileName, container, result.ExcelMessages, provider);
            if (OnAfterExportExcel != null)
            {
                OnAfterExportExcel(this, xlsEptArgs);
                if (xlsEptArgs.Canceled)
                {
                    ExcelMessageFactory.PrepareEventMessage(result, "OnAfterExportExcel", xlsEptArgs.Canceled);
                    return ;
                }
            }
            #endregion

            
            MemoryStream file = new MemoryStream();
            hssfWorkbook.Write(file);

            
            SetExcelResponse(response, file, saveAsFileName, null);
        }

        private void SetExcelResponse(HttpResponse response, MemoryStream file, string filename, Encoding encoding)
        {
            response.Clear();
            response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlPathEncode(filename + ".xls"));
            response.AddHeader("Content-Type", "application/force-download");
            response.AddHeader("Content-Type", "application/octet-stream");
            response.AddHeader("Content-Type", "application/download");
            response.AddHeader("Content-Transfer-Encoding", "binary");
            response.ContentType = "application/ms-excel";

            if (encoding != null)
            {
                response.ContentEncoding = encoding;
            }
            response.BinaryWrite(file.GetBuffer());
            response.End();
        }
        
        
        
        
        
        
        
        private void SkipRow(ISheet hssfSheet, int totalRowSpan, IRow hssfRow)
        {
            RemoveRow(hssfSheet, hssfRow);
            if (hssfSheet.GetRow(hssfRow.RowNum + 1) == null)
                hssfSheet.CreateRow(hssfRow.RowNum + 1);
            hssfSheet.ShiftRows(hssfRow.RowNum + 1, hssfSheet.LastRowNum, -1);
        }

        
        
        
        
        
        
        private IRow GetRow(ISheet hssfSheet, int rowIndex)
        {
            IRow hssfRow = hssfSheet.GetRow(rowIndex);
            if (hssfRow == null)
                hssfRow = hssfSheet.CreateRow(rowIndex);
            return hssfRow;
        }

        private void RemoveRow(ISheet hssfSheet, IRow hssfRow)
        {
            hssfSheet.RemoveRow(hssfRow);
        }

        
        
        
        
        
        
        private ICell GetCell(IRow hssfRow, int cellIndex)
        {
            ICell hssfCell = hssfRow.GetCell(cellIndex);
            if (hssfCell == null)
                hssfCell = hssfRow.CreateCell(cellIndex);
            return hssfCell;
        }

        protected void ConfigExportValidate(ExcelExportProvider provider, DataSet container, string typeName)
        {
            
            if (container.Tables.Count == 0) throw new Exception("DataSet对象容器中没有任何DataTable对象!");
            
            if (provider == null) throw new Exception(string.Format("配置中不存在类型为:{0}的导入配置项!", typeName));
            
            if (provider.Worksheets == null || provider.Worksheets.Count == 0) throw new Exception(string.Format("配置中类型为:{0}的配置节中不包含Worksheet的定义", typeName));
            
            foreach (Clover.Component.Excel.Export.Worksheet worksheet in provider.Worksheets)
            {
                foreach (Area area in worksheet.Areas)
                    if (!container.Tables.Contains(area.DataTable))
                        throw new Exception(string.Format("DataTable中不包含表 {0}", area.DataTable));
            }
        }

        #endregion

        #region fast export

        #endregion
    }
}
