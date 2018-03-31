using System.Data;
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

          public virtual void WageListToFile(DataTable data, int maxrow, string sheetName,
                                            Dictionary<string, string> keyValues,
                                            string templatefile, string outputFilePath)
          {
              IWorkbook book = WageList(data, maxrow, sheetName, -1, -1, keyValues, templatefile, outputFilePath);
              using (var file = new FileStream(outputFilePath, FileMode.Create))
              {
                  book.Write(file);
              }
          }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public virtual IWorkbook WageList(DataTable data, int maxrow, string sheetName, 
                                          int colHeaderRowIndex,int splitterRowIdx, 
                                          Dictionary<string, string> keyValues, 
                                          string templatefile, string outputFilePath)
        {
            int sheetCount = 1;
            int rowcount = data.Rows.Count;
            if (string.IsNullOrEmpty(sheetName)) sheetName = "工作表";

            IWorkbook workbook;

            using (FileStream sourcefs = new FileStream(templatefile, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(sourcefs);
            }
            
            ISheet sheet;
            sheet = workbook.GetSheetAt(0);

            
            List<FillSetting> settings = getTemplateSetting(sheet);
            
            renderDictFlag(data, maxrow, keyValues, settings, sheet, workbook);

            
            renderWageList(data, maxrow, settings, sheet, workbook, colHeaderRowIndex, splitterRowIdx);

            sheet.ForceFormulaRecalculation = true;
            return workbook;
        }
        
        private void renderWageList(DataTable data, int maxrow,
                        List<FillSetting> settings, ISheet sheet, IWorkbook workbook,
                        int colHeaderRowIndex,int splitterRowIdx)
        {
            IRow repeatHrow = colHeaderRowIndex >= 0 ? sheet.GetRow(colHeaderRowIndex) : null;
            IRow repeatFrow = splitterRowIdx >= 0 ? sheet.GetRow(splitterRowIdx) : null;

            foreach (var s in settings)
            {
                if (s.Flag == FlagType.DataSource)
                {
                    
                    
                    ICellStyle style = null;

                    IRow row = sheet.GetRow(s.StartRowIndex);
                    ISheet oldsheet = sheet;
                    int rdrowindex = s.StartRowIndex;

                    for (int p = 0; p < data.Rows.Count; p++, rdrowindex++)
                    {
                        if (repeatHrow != null)
                        {
                            sheet.ShiftRows(rdrowindex, sheet.LastRowNum, 1, true, false);
                            NPOIHelper.CopyRow(sheet, sheet, colHeaderRowIndex,  rdrowindex);
               
                            rdrowindex++;
                        }

                        DataRow dr = data.Rows[p];
                        if (p >= maxrow)
                        {
                            
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
                            
                            foreach (var col in s.Columns)
                            {
                                var cell = row.GetCell(col.ColumnIndex);

                                if (col.ColumnType != CellType.FORMULA)
                                {
                                    if (!data.Columns.Contains(col.FieldName)) break;

                                    SetCellValue(cell, data.Columns[col.FieldName], dr[col.FieldName].ToString());
                                }
                            }
                        }
                        else
                        {
                            sheet.ShiftRows(rdrowindex, sheet.LastRowNum, 1, true, false);
                            IRow newrow = sheet.CreateRow(rdrowindex);
                            foreach (var col in s.Columns)
                            {
                                if (col.ColumnType != CellType.FORMULA)
                                {
                                    if (!data.Columns.Contains(col.FieldName)) break;

                                    var newcell = newrow.CreateCell(col.ColumnIndex);
                                    newcell.CellStyle = row.GetCell(col.ColumnIndex).CellStyle;
                                    SetCellValue(newcell, data.Columns[col.FieldName], dr[col.FieldName].ToString());
                                }
                                else
                                {
                                    var newcell = newrow.CreateCell(col.ColumnIndex);
                                    newcell.CellStyle = row.GetCell(col.ColumnIndex).CellStyle;

                                    newcell.SetCellFormula(col.FieldName.Replace("{0}",
                                        (rdrowindex + 1).ToString()));
                                }
                            }

                        }

                        if (repeatFrow != null)
                        {
                            
                            sheet.ShiftRows(rdrowindex, sheet.LastRowNum, 1, true, false);
                            NPOIHelper.CopyRow(sheet, sheet, splitterRowIdx, rdrowindex);

                            rdrowindex++;
                        }


                    } 

                    
                    for (int zz = 0; zz < s.Columns.Count; zz++)
                    {
                        sheet.AutoSizeColumn(s.Columns[zz].ColumnIndex);
                    }
                    sheet.ForceFormulaRecalculation = true;
                } 
            }
        }
    }
}
