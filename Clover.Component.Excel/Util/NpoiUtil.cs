using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Clover.Component.Excel.Util
{
    
   public static class NpoiUtil
    {
       
       
       
       
       
       
       
       public static ISheet CopySheet(ISheet sourceSheet, IWorkbook sourceWorkook, ISheet targetSheet)
       {
           IRow sourceRow = null;
           IRow targetRow = null;
           
           
           CellRangeAddress region = null;
           int pEndRow = sourceSheet.LastRowNum;

           
           for (int i = 0; i < sourceSheet.NumMergedRegions; i++)
           {
               region = sourceSheet.GetMergedRegion(i);
               if ((region.FirstColumn >= 0) && (region.LastRow <= pEndRow))
               {
                   targetSheet.AddMergedRegion(region);
               }
           }

           
           for (int targetRowIndex = 0; targetRowIndex <= pEndRow; targetRowIndex++)
           {
               sourceRow = sourceSheet.GetRow(targetRowIndex);
               if (sourceRow == null)
               {
                   continue;
               }
               targetRow = targetSheet.CreateRow(targetRowIndex);
               CopyRow(sourceSheet, targetSheet, sourceWorkook, sourceRow, targetRowIndex, targetRow);
           }

           return targetSheet;

       }

       
       
       
       
       
       
       
       
       private static void CopyRow(ISheet sourceSheet, ISheet targetSheet, IWorkbook targetWorkook, IRow sourceRow, int targetRowIndex, IRow targetRow)
       {
           ICell sourceCell;
           ICell targetCell;
           targetRow.Height=sourceRow.Height;
           for (int cellIndex = sourceRow.FirstCellNum; cellIndex <= sourceRow.PhysicalNumberOfCells; cellIndex++)
           {
               sourceCell = sourceRow.GetCell(cellIndex);
               if (sourceCell == null)
               {
                   continue;
               }
               targetSheet.SetColumnWidth(cellIndex, sourceSheet.GetColumnWidth(cellIndex));
               targetSheet.IsActive = sourceSheet.IsActive;
               targetSheet.SetColumnHidden(cellIndex, sourceSheet.IsColumnHidden(cellIndex));

               targetCell = targetRow.CreateCell(cellIndex);
               var cType = sourceCell.CellType;
               targetCell.SetCellType(cType);
               if (sourceCell.Hyperlink!= null)
                   targetCell.Hyperlink = sourceCell.Hyperlink;
               if (sourceCell.CellComment != null)
                   targetCell.CellComment = sourceCell.CellComment;
               ICellStyle targetStyle = targetWorkook.CreateCellStyle();
               targetStyle.CloneStyleFrom(sourceCell.CellStyle);
               targetCell.CellStyle = targetStyle;
               string traceMsg = string.Format("{0}行{1}列的上下左右边框分别为:{2}{3}{4}{5}\r\n", targetRowIndex, cellIndex, sourceCell.CellStyle.BorderTop, sourceCell.CellStyle.BorderBottom, sourceCell.CellStyle.BorderLeft, sourceCell.CellStyle.BorderRight);
               Trace.Write(traceMsg);
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
                       s = s.Replace((targetRowIndex + 1) + "", (targetRowIndex + 1) + "");
                       targetCell.SetCellFormula(s);
                       break;
                   case CellType.NUMERIC:
                       targetCell.SetCellValue(sourceCell.NumericCellValue);
                       break;
                   case CellType.STRING:
                       targetCell.SetCellValue(sourceCell.StringCellValue);
                       break;
               }
           }
       }


      
      
      
      
       
      
       public static void removeColumn(ISheet sheet, int removeColumnIndex, int removeColumnTotal,int startRow,int endRow)
       {
           if (sheet == null)
           {
               return;
           }

           for (short n = (short)removeColumnIndex; n < (short)(removeColumnTotal + removeColumnIndex); n++)
           {
               sheet.SetColumnHidden(removeColumnIndex, true);
           }
          return;

           

           
       }
    }
}
