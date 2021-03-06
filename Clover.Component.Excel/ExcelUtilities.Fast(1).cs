﻿using System.Data;
using System;
using System.IO;
using System.Linq;


using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Clover.Component.Excel
{
    public partial class ExcelUtilities
    {

        const int MAXRow = 65536; //记录数填充最大数
       
        const int columnRange = 26; //最大搜索填充标记的范围

        const string regDataSource = "\\{Table:(.*)\\}";

        const string regField = "\\{Field!(.*)\\}";

        const string regMark = "#(.*)#";


        public virtual void FillDataToExcelFile(DataTable data, string[] titles, string[] columns,
                                           string templatefile,
                                           string outputFilePath)
        {
            FillDataToExcelFile(data, 65536, "数据", 0, 0, titles, columns,
                                             null, templatefile, outputFilePath);

        }

        public virtual void FillDataToExcelFile(DataTable data, int maxrow, string sheetName, int startRowIndex,
                                            int startColIndex, string[] titles, string[] columns,
                                            Dictionary<string, string> keyValues,
                                            string templatefile,
                                            string outputFilePath)
        {
            IWorkbook book = FillDataToExcel(data, maxrow, sheetName, startRowIndex, startColIndex, titles, columns,
                                             keyValues, templatefile, outputFilePath);


            using (var file = new FileStream(outputFilePath, FileMode.Create))
            {
                book.Write(file);
            }
        }

        /// <summary>
        /// 导出Excel数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="sheetname"></param>
        /// <param name="titles"></param>
        /// <param name="columns"></param>
        /// <param name="templatefile"></param>
        /// <param name="outputFilePath"></param>
        public virtual IWorkbook FillDataToExcel(DataTable data, int maxrow, string sheetName, int startRowIndex, int startColIndex, 
                    string[] titles, string[] columns, Dictionary<string, string> keyValues, 
                    string templatefile, string outputFilePath)
        {
            int sheetCount = 1;
            int rowcount = data== null ? 0 : data.Rows.Count;
            if (string.IsNullOrEmpty(sheetName)) sheetName = "工作表";
           
            IWorkbook workbook;
            if (!string.IsNullOrEmpty(templatefile) && File.Exists(templatefile))
            {
                using (FileStream sourcefs = new FileStream(templatefile, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(sourcefs);
                }
            }
            else {
                using (FileStream sourcefs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    workbook = new HSSFWorkbook();
                }
            }
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");
            ISheet sheet;
            if (!string.IsNullOrEmpty(templatefile) && File.Exists(templatefile))
            {              
                sheet = workbook.GetSheetAt(0);                
            }
            else
            {
                sheet = workbook.GetSheet(sheetName) ?? workbook.CreateSheet(sheetName);
            }
            //获取填充设置标记范围
            List<FillSetting> settings = getTemplateSetting(sheet);
            //标记渲染
            renderDictFlag(data, maxrow, keyValues, settings, sheet, workbook);
            if (data != null)
            {
                if (settings.Where(x => x.Flag == FlagType.DataSource).Count() > 0)
                {
                    //模板方式渲染
                    renderByTemplate(data, maxrow, keyValues, settings, sheet, workbook);
                }
                else
                {
                    //没有任何填充标记,按照默认情况填充数据
                    renderByDefault(data, maxrow, startRowIndex, startColIndex, titles, columns, workbook, sheet);
                }
            }
            sheet.ForceFormulaRecalculation = true;
            return workbook;
        }

        private static List<FillSetting> getTemplateSetting(ISheet sheet)
        {
            var settings = new List<FillSetting>();
                 
            for (int x = 0; x <= sheet.LastRowNum; x ++)
            {
                IRow row = sheet.GetRow(x);
                if (row == null)
                    continue;
                
                for (int z = 0; z < row.Cells.Count; z ++)
                {
                    ICell cell = row.Cells[z];
                    if (cell.CellType != CellType.STRING)
                        continue;                       
                    
                    if (Regex.IsMatch(cell.StringCellValue, regMark)) //优先匹配标记
                    {
                        FillSetting s = new FillSetting();
                        s.Flag = FlagType.Mark;
                        s.TableName = cell.StringCellValue.Replace("#", "");
                        s.StartRowIndex = cell.RowIndex;
                        s.StartColumnIndex = z;
                        cell.SetCellValue("");
                        settings.Add(s);
                    }
                    else if (Regex.IsMatch(cell.StringCellValue, regField)) //匹配数据源类型
                    {
                        FillSetting s = new FillSetting();
                        s.Flag = FlagType.DataSource;
                        s.StartRowIndex = cell.RowIndex;
                        s.StartColumnIndex = cell.ColumnIndex;
                    
                        int y = 0;
                        for (y = z; y < row.Cells.Count; y ++)
                        {
                            var ccell = row.Cells[y];
                            if (ccell.CellType == CellType.FORMULA)
                            {
                                ColumnSetting cs = new ColumnSetting();
                                cs.ColumnIndex = ccell.ColumnIndex;
                                cs.RowIndex = x;
                                cs.ColumnType = ccell.CellType;

                                var ms = Regex.Matches(ccell.CellFormula, "[a-zA-Z]{1,3}(" + (cell.RowIndex + 1) + ")");
                                var formula = ccell.CellFormula;
                                foreach (Match m in ms)
                                {
                                    formula = formula.Replace(m.Value,
                                                              Regex.Replace(m.Value, "\\d{1,999999999}", "{0}"));
                                }
                                cs.FieldName = formula;

                                s.Columns.Add(cs);
                            }
                            else if (!string.IsNullOrEmpty(row.Cells[y].StringCellValue))
                            {
                             
                                ColumnSetting cs = new ColumnSetting();
                                cs.ColumnIndex = ccell.ColumnIndex;
                                cs.RowIndex = x;
                                cs.ColumnType = ccell.CellType;

                                if (Regex.IsMatch(ccell.StringCellValue, regField))
                                {
                                    cs.FieldName = ccell.StringCellValue.Replace("{Field!", "").Replace("}", "");
                                }
                               
                                s.Columns.Add(cs);
                            }
                            else
                            {
                                break;
                            }
                        }
                        z = y;
                        settings.Add(s);
                    }
                } //eend for cell
            } //end for row

            return settings;
        }
         private void renderDictFlag(DataTable data, int maxrow, Dictionary<string, string> keyValues,
                                       List<FillSetting> settings, ISheet sheet,
                                       IWorkbook workbook)
         {
             foreach (var s in settings)
             {
                 if (s.Flag == FlagType.Mark && keyValues.ContainsKey(s.TableName))
                 {
                     sheet.GetRow(s.StartRowIndex).Cells[s.StartColumnIndex].SetCellValue(keyValues[s.TableName]);
                     sheet.AutoSizeColumn(s.StartColumnIndex);
                 }
             }
         }

        private void renderByTemplate(DataTable data, int maxrow, Dictionary<string, string> keyValues,
                        List<FillSetting> settings, ISheet sheet,IWorkbook workbook)
        {
            foreach (var s in settings)
            {
                if (s.Flag == FlagType.DataSource)
                {
                    //填充数据行
                    //获取表格列
                    ICellStyle style = null;

                    IRow row = sheet.GetRow(s.StartRowIndex);
                    ISheet oldsheet = sheet;
                    int rdrowindex = s.StartRowIndex;

                    for (int p = 0; p < data.Rows.Count; p++, rdrowindex++)
                    {
                        DataRow dr = data.Rows[p];
                        if (p >= maxrow)
                        {
                            //new sheet
                            int idx = workbook.GetSheetIndex(oldsheet);
                            sheet = workbook.CloneSheet(idx);
                            for (int yy = 0; yy < sheet.PhysicalNumberOfRows; yy++)
                            {
                                sheet.RemoveRowBreak(yy);
                            }

                            rdrowindex = s.StartRowIndex + 1;
                        }
                        if (p == 0)
                        {
                            //直接替代原有的行
                            foreach (var col in s.Columns)
                            {
                                 var cell = row.GetCell(col.ColumnIndex);

                                if (col.ColumnType != CellType.FORMULA)
                                {
                                    if (!data.Columns.Contains(col.FieldName))
                                    {
                                        cell.SetCellValue(string.Empty);
                                    }
                                    else
                                    {
                                        SetCellValue(cell, data.Columns[col.FieldName], dr[col.FieldName].ToString());
                                    }
                                }
                            }
                        }
                        else
                        {
                            sheet.ShiftRows(rdrowindex, rdrowindex + 1, 1, true, false);
                            IRow newrow = sheet.CreateRow(rdrowindex);
                            foreach (var col in s.Columns)
                            {
                                if (col.ColumnType != CellType.FORMULA)
                                {
                                    var newcell = newrow.CreateCell(col.ColumnIndex);
                                    newcell.CellStyle = row.GetCell(col.ColumnIndex).CellStyle;
                                    if (!data.Columns.Contains(col.FieldName))
                                    {
                                        newcell.SetCellValue(string.Empty);
                                    }
                                    else
                                    {
                                        SetCellValue(newcell, data.Columns[col.FieldName], dr[col.FieldName].ToString());
                                    }
                                }
                                else
                                {
                                    var newcell = newrow.CreateCell(col.ColumnIndex);
                                    newcell.CellStyle = row.GetCell(col.ColumnIndex).CellStyle;

                                    newcell.SetCellFormula(col.FieldName.Replace("{0}", 
                                        (rdrowindex+1).ToString()));
                                }
                            }
                        }
                    } //foreach rows   
                    //autosize
                    for (int zz = 0; zz < s.Columns.Count; zz++)
                    {
                        sheet.AutoSizeColumn(s.Columns[zz].ColumnIndex);
                    }
                    sheet.ForceFormulaRecalculation = true;
                } //end for datasource                    
            }
        }

        private void renderByDefault(DataTable data, int maxrow, int startRowIndex, int startColIndex, string[] titles,
                                     string[] columns, IWorkbook workbook, ISheet sheet)
        {

            //填充头(粗体黑色)
            int i = 0;
            ICellStyle cellstyle = workbook.CreateCellStyle();

            if (titles != null && titles.Length > 0)
            {
                renderHeader(startRowIndex, startColIndex, titles, workbook, sheet);
            }

            if (columns == null || columns.Length == 0) {
                List<string> lcolumns = new List<string>();
                foreach (DataColumn dc in data.Columns)
                {
                    lcolumns.Add(dc.ColumnName);
                }
                columns = lcolumns.ToArray();
            }

            ISheet oldsheet = sheet;

            //单元格样式
            ICellStyle scellstyle = workbook.CreateCellStyle();
            scellstyle.BorderTop =
                scellstyle.BorderBottom = scellstyle.BorderLeft = scellstyle.BorderRight = BorderStyle.THIN;

            //填充数据
            int sheetcount = 1;
            int rdrowindex = startRowIndex + 1;
            string oldsheetname = sheet.SheetName;
            for (int p = 0; p < data.Rows.Count; p++, rdrowindex++)
            {
                DataRow dr = data.Rows[p];
                if ((rdrowindex + startRowIndex + 1) >= maxrow)
                {
                    for (int k = 0; k < data.Columns.Count; k++)
                    {
                        sheet.AutoSizeColumn(startColIndex + k);
                    }

                    //new sheet                    
                    sheet = workbook.CreateSheet(oldsheetname + "(" + sheetcount + ")");
                    sheetcount++;
                    for (int yy = 0; yy < sheet.PhysicalNumberOfRows; yy++)
                    {
                        sheet.RemoveRowBreak(yy);
                    }
                    renderHeader(startRowIndex, startColIndex, titles, workbook, sheet);
                    rdrowindex = startRowIndex + 1;
                }

                IRow newrow = sheet.CreateRow(rdrowindex);
                for (i = 0; i < columns.Length; i++)
                {
                    var colname = columns[i];
                    if (!data.Columns.Contains(colname)) break;

                    var newcell = newrow.CreateCell(startColIndex + i);
                    var datatype = data.Columns[colname].DataType.ToString();
                    switch (datatype)
                    {
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            newcell.CellStyle = GetDefaultDecimalCellStyle(workbook);
                            break;
                        default:
                            newcell.CellStyle = scellstyle;
                            break;
                    }
                   
                    SetCellValue(newcell, data.Columns[colname], dr[colname].ToString());
                }
            }
            for (int k = 0; k < data.Columns.Count; k++)
            {
                sheet.AutoSizeColumn(startColIndex + k);
            }
        }

        private void renderHeader(int startRowIndex, int startColIndex, string[] titles, IWorkbook workbook, ISheet sheet)
        {
            var header = sheet.CreateRow(startRowIndex);
            ICellStyle headerstyle = workbook.CreateCellStyle();
            headerstyle.Alignment = HorizontalAlignment.CENTER;
            IFont font = workbook.CreateFont();
            headerstyle.BorderTop =
                headerstyle.BorderBottom = headerstyle.BorderLeft = headerstyle.BorderRight = BorderStyle.THIN;
            font.Boldweight = (short) FontBoldWeight.BOLD;
            headerstyle.SetFont(font);
            int i = 0;
            foreach (var title in titles)
            {
                var cell = header.CreateCell(startColIndex + i);
                cell.CellStyle = headerstyle;
                cell.SetCellValue(title);
                i++;
            }
        }

        private void SetCellValue(ICell newCell, DataColumn dc, string drValue)
        {
            switch (dc.DataType.ToString())
            {
                case "System.String"://字符串类型
                    //辨别是否为公式行                    
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型

                    if (string.IsNullOrEmpty(drValue))
                    { newCell.SetCellValue(string.Empty); }
                    else
                    {
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);                        
                        newCell.SetCellValue(dateV);
                    }

                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    if (string.IsNullOrEmpty(drValue))
                    { newCell.SetCellValue(string.Empty); }
                    else
                    {
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell.SetCellValue(intV);
                    }
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    if (string.IsNullOrEmpty(drValue))
                    { newCell.SetCellValue(string.Empty); }
                    else
                    {
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell.SetCellValue(doubV);
                    }
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue(string.Empty);
                    break;
                default:
                    newCell.SetCellValue(string.Empty);
                    break;
            }
        }

        private ICellStyle GetDefaultDecimalCellStyle(IWorkbook workbook) {
            short i = (short)(workbook.NumCellStyles -1);
            var defaultDataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            while (i > 0)
            {
                if (workbook.GetCellStyleAt(i).DataFormat == defaultDataFormat)
                    return workbook.GetCellStyleAt(i);
                i--;
            }
            ICellStyle cellStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cellStyle.BorderTop =
              cellStyle.BorderBottom = cellStyle.BorderLeft = cellStyle.BorderRight = BorderStyle.THIN;

            return cellStyle;
        }
    }

    public static class NPOIHelper
    {
        #region - 由数字转换为Excel中的列字母 -

        public static int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+")) { throw new Exception("invalid parameter"); }
            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }

        public static string ToName(int index)
        {
            if (index < 0) { throw new Exception("invalid parameter"); }
            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);
            return String.Join(string.Empty, chars.ToArray());
        }
        #endregion


        public static void CopyRow(ISheet poiTo, ISheet poiFrom, int sourceRowNum, int destinationRowNum)
        {
            // Get the source / new row
            IRow newRow = poiTo.GetRow(destinationRowNum - 1);
            IRow sourceRow = poiFrom.GetRow(sourceRowNum - 1);

            // Loop through source columns to add to new row
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                ICell oldCell = sourceRow.GetCell(i);
                //ICell newCell = newRow.CreateCell(i);
                ICell newCell = newRow.GetCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }
                //模板cell有，目标cell没有
                if (newCell == null)
                {
                    newCell = newRow.CreateCell(i);
                }
                newCell.CellStyle = oldCell.CellStyle;
                //edit end by qiulc 2012/7/21

                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);
            }

            // If there are are any merged regions in the source row, copy to new row
            for (int i = 0; i < poiFrom.NumMergedRegions; i++)
            {
                CellRangeAddress cellRangeAddress = poiFrom.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    CellRangeAddress newCellRangeAddress =
                        new CellRangeAddress(newRow.RowNum,
                        (newRow.RowNum +
                            (cellRangeAddress.LastRow - cellRangeAddress.FirstRow)),
                        cellRangeAddress.FirstColumn,
                        cellRangeAddress.LastColumn);
                    poiTo.AddMergedRegion(newCellRangeAddress);
                }
            }
        }


        /// <summary>
        /// 在表格上渲染数据
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="startColIndex"></param>
        /// <param name="sheet"></param>
        /// <param name="rdrowindex"></param>
        /// <param name="rowOffset"></param>
        /// <param name="lastColumnIndex"></param>
        public static void RenderDataTableToExcel(DataTable data, int startRowIndex, int startColIndex, ISheet sheet, int rdrowindex, int rowOffset,
                                    int lastColumnIndex)
        {
            foreach (DataRow dr in data.Rows)
            {
                RenderDataRowToExcel(startRowIndex, startColIndex, sheet, dr, 0);
                startRowIndex += rowOffset;
            }
        }

        /// <summary>
        /// 渲染数据行到指定的区域
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="startColIndex"></param>
        /// <param name="sheet"></param>
        /// <param name="dr"></param>
        /// <param name="startColumnIndex"></param>
        public static void RenderDataRowToExcel(int startRowIndex, int startColIndex, ISheet sheet, DataRow dr, int startColumnIndex)
        {
            //获取拷贝的行
            IRow newrow = sheet.GetRow(startRowIndex);
            for (int i = startColumnIndex; i < dr.Table.Columns.Count; i++)
            {
                DataColumn col = dr.Table.Columns[i];
                var colname = col.ColumnName;
                var newcell = newrow.GetCell(startColIndex + i - startColumnIndex);
                var datatype = col.DataType.ToString();
                switch (datatype)
                {
                    case "System.Decimal": //浮点型
                    case "System.Double":
                        newcell.CellStyle = GetDefaultDecimalCellStyle(sheet.Workbook);
                        break;
                }

                SetCellValue(newcell, col, dr[colname].ToString());
            }
        }

        /// <summary>
        /// 拷贝行
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="startColIndex"></param>
        /// <param name="sheet"></param>
        /// <param name="rdrowindex"></param>
        /// <param name="copyRowCount"></param>
        /// <param name="copyColumnCount"></param>
        public static void CopyRows(int startRowIndex, int startColIndex, ISheet sheet, int rdrowindex, int copyRowCount,
                                    int copyColumnCount, bool makecellMerage)
        {
            sheet.ShiftRows(rdrowindex, sheet.LastRowNum, copyRowCount, true, false);

            for (int j = 0; j < copyRowCount; j++)
            {
                //获取首行（模板）
                IRow sourceRow = sheet.GetRow(startRowIndex + j);

                //目标行
                IRow targetrow = sheet.CreateRow(rdrowindex + j);

                for (int k = startColIndex; k < sourceRow.LastCellNum; k++)
                {
                    var sourceCell = sourceRow.Cells[k];
                    var targetCell = targetrow.CreateCell(k);
                    var cType = sourceCell.CellType;
                    targetCell.SetCellType(cType);
                    if (sourceCell.Hyperlink != null)
                        targetCell.Hyperlink = sourceCell.Hyperlink;
                    if (sourceCell.CellComment != null)
                        targetCell.CellComment = sourceCell.CellComment;
                    targetCell.CellStyle = sourceCell.CellStyle;

                    switch (cType)
                    {
                        case CellType.BOOLEAN:
                            targetCell.SetCellValue(sourceCell.BooleanCellValue);
                            break;
                        case CellType.ERROR:
                            targetCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                            break;
                        case CellType.FORMULA:
                            String s = sourceCell.CellFormula;
                            //s = s.Replace((j + 1) + "", (j + 1) + "");
                            targetCell.SetCellFormula(s);
                            break;
                        case CellType.NUMERIC:
                            targetCell.SetCellValue(sourceCell.NumericCellValue);
                            break;
                        case CellType.STRING:
                            targetCell.SetCellValue(sourceCell.StringCellValue);
                            break;
                    }

                    //合并单元格子
                    if (makecellMerage && j == 0 && k < copyColumnCount)
                        MakeRowMerageRange(targetCell, copyRowCount);
                }
            }
        }

        public static ICellStyle GetDefaultDecimalCellStyle(IWorkbook workbook)
        {
            short i = (short)(workbook.NumCellStyles - 1);
            var defaultDataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            while (i > 0)
            {
                if (workbook.GetCellStyleAt(i).DataFormat == defaultDataFormat)
                    return workbook.GetCellStyleAt(i);
                i--;
            }
            ICellStyle cellStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cellStyle.BorderTop =
              cellStyle.BorderBottom = cellStyle.BorderLeft = cellStyle.BorderRight = BorderStyle.THIN;

            return cellStyle;
        }

        public static void SetCellValue(ICell newCell, DataColumn dc, string drValue)
        {
            switch (dc.DataType.ToString())
            {
                case "System.String"://字符串类型
                    //辨别是否为公式行                    
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型

                    if (string.IsNullOrEmpty(drValue))
                    { newCell.SetCellValue(string.Empty); }
                    else
                    {
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(dateV);
                    }

                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    if (string.IsNullOrEmpty(drValue))
                    { newCell.SetCellValue(string.Empty); }
                    else
                    {
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell.SetCellValue(intV);
                    }
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    if (string.IsNullOrEmpty(drValue))
                    { newCell.SetCellValue(string.Empty); }
                    else
                    {
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell.SetCellValue(doubV);
                    }
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue(string.Empty);
                    break;
                default:
                    newCell.SetCellValue(string.Empty);
                    break;
            }
        }

        /// <summary>
        /// 获取隔行累计公式
        /// </summary>
        /// <param name="rollupCount">隔行数</param>
        /// <param name="count">循环次数</param>
        /// <returns></returns>
        public static string GetSumFormula(int rollupCount, int count)
        {
            if (count == 0 || rollupCount == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("Sum(");
            for (int i = 1; i <= count; i++)
            {
                sb.Append("INDIRECT(\"R[" + (-1 * rollupCount * i) + "]C\",FALSE),");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// 合并设置
        /// </summary>
        public class MerageSetting
        {
            public int StartRowIndex { get; set; }
            public int StartColumnIndex { get; set; }
            public int LastRowIndex { get; set; }
            public int LastColumnwIndex { get; set; }
        }

        /// <summary>
        /// 设置行的合并
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="merageCount">向下合并数</param>
        public static void MakeRowMerageRange(ICell cell, int merageCount)
        {
            cell.Sheet.AddMergedRegion(new CellRangeAddress(cell.RowIndex, (cell.RowIndex + merageCount - 1),
              cell.ColumnIndex, cell.ColumnIndex));
        }

        /// <summary>
        /// 设置单元格的合并区域
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="merageCount">向下合并数</param>
        public static void MakeMerageRange(ISheet sheet, MerageSetting merageSetting)
        {
            sheet.AddMergedRegion(new CellRangeAddress(merageSetting.StartRowIndex, merageSetting.LastRowIndex,
                merageSetting.StartColumnIndex, merageSetting.LastColumnwIndex));
        }
    }

    /// <summary>
    /// 填充选项
    /// </summary>
    public class FillSetting
    {
        public FlagType Flag = FlagType.DataSource;

        /// <summary>
        /// 开始填充的行号
        /// </summary>
        public int StartRowIndex;
        /// <summary>
        /// 开始填充列号
        /// </summary>
        public int StartColumnIndex;

        /// 结束填充的行号
        /// </summary>
        public int EndRowIndex;
        /// <summary>
        /// 结束填充列号
        /// </summary>
        public int EndColumnIndex;
    
        /// <summary>
        /// 目标读取的表格名称（表名$字段名）
        /// </summary>
        public string TableName;


        /// <summary>
        /// 嵌套的填充信息
        /// </summary>
        public List<FillSetting> InnerFillSettings = new List<FillSetting>();

        /// <summary>
        /// 具有的列
        /// </summary>
        public List<ColumnSetting> Columns = new List<ColumnSetting>(); 
    }
    public class ColumnSetting
    {
        /// <summary>
        /// 开始填充的行号
        /// </summary>
        public int RowIndex;

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName;
        /// <summary>
        /// 开始填充列号
        /// </summary>
        public int ColumnIndex;

        public CellType ColumnType = CellType.STRING;

        /// <summary>
        /// 样式
        /// </summary>
        public ICellStyle Style;
    }

    public enum FlagType
    {
        /// <summary>
        /// 单纯替代的标记
        /// </summary>
        Mark = 0,
        /// <summary>
        /// 数据源
        /// </summary>
        DataSource = 1
    }
}
