using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Clover.Component.Excel.Export.Merged
{
    
   public class Merged
    {
       
       
       
       
       public int StartRow { get; set; }

       
       
       
       public int EndRow { get; set; }

       
       
       
       public ISheet CurrentSheet { get; set; }

       
       
       
       public List<MergedConfig> MergedConfigs { get; set; }

       
       
       
       public bool DoMerged()
       {
           bool result = false;
           
           if (MergedConfigs==null||MergedConfigs.Count<1)
           {
               return true;
           }
           foreach(MergedConfig config in MergedConfigs)
           {
               if(config==null)
               {
                   continue;
               }
               result = DoSingleMerged(config);
               if(!result)
               {
                   break;
               }
           }

           return result;
           
       }

       
       
       
       
       
       private bool DoSingleMerged(MergedConfig config)
       {
           bool mergedResult = true;
           if (config.MergedType==MergedConfig.RowMergedType)
           {
               mergedResult = DoRowSingleMerged(config);
           }
           else if (config.MergedType == MergedConfig.ColumnMergedType)
           {
               mergedResult = DoColumnSingleMerged(config);
           }
           else if (config.MergedType == MergedConfig.AllMergedType)
           {
               mergedResult = DoAllSingleMerged(config);
           }


           return mergedResult;
       }


       
       
       
       
       
       private bool DoRowSingleMerged(MergedConfig config)
       {
           bool mergedResult = true;
           int fromColumn = -1;
           int toClumn = -1;
           if (!int.TryParse(config.FromColumn, out fromColumn))
           {
               throw new ArgumentException("Merged配置项中fromColumn参数无效");
           }

           if (!int.TryParse(config.ToColumn, out toClumn))
           {
               throw new ArgumentException("Merged配置项中toColumn参数无效");
           }
           if (fromColumn>toClumn)
           {
               throw new ArgumentException("Merged配置中from参数不能大于toColumn参数");
           }
           if (fromColumn == toClumn)
           {
               return true;
           }
           
           for (int index = StartRow; index <= EndRow;index++)
           {
               IRow hssRow = CurrentSheet.GetRow(index);
               bool identityFlag = true;
               string lastRowValue = string.Empty;
               
               for(int columnIndex=fromColumn;columnIndex<=toClumn;columnIndex++)
               {
                   string currentRowValue = string.Empty;
                   ICell currentCell = hssRow.GetCell(columnIndex);
                   currentRowValue = GetCellValue(currentCell);
                   
                   if(config.IgnoreEmpty)
                   {
                       if(string.IsNullOrEmpty(currentRowValue))
                       {
                           identityFlag = false;
                           break;
                       }
                   }
                   
                   if(columnIndex>fromColumn)
                   {
                       identityFlag = string.Compare(currentRowValue, lastRowValue) == 0;
                       if(!identityFlag)
                       {
                           break;
                       }
                   }
                   lastRowValue = currentRowValue;
               }
               
               if(identityFlag)
               {
                   CurrentSheet.AddMergedRegion(new CellRangeAddress(index, fromColumn, index, toClumn));
               }

           }
           return mergedResult;
       }


       
       
       
       
       
       private bool DoAllSingleMerged(MergedConfig config)
       {
           bool mergedResult=true;
           int fromColumn = -1;
           int fromRow = -1;
           int toClumn = -1;
           int toRow = -1;
           if (!int.TryParse(config.FromColumn, out fromColumn))
           {
               throw new ArgumentException("Merge配置中fromColumn参数不能为空或者只能为数字");
           }
           if (!int.TryParse(config.ToColumn, out toClumn))
           {
               throw new ArgumentException("Merge配置中ToColumn参数不能为空或者只能为数字");
           }
           if (fromColumn>toClumn)
           {
               throw new ArgumentException("Merge配置中fromColumn参数不能大于ToColumn");
           }
           
           if (!int.TryParse(config.FromRow, out fromRow))
           {
               fromRow = StartRow;
           }

           
           if (!int.TryParse(config.ToRow, out toRow))
           {
               toRow = EndRow;
           }
           int loopIndex = fromRow;
           while (loopIndex <= toRow)
           {
               int startRowIndex = loopIndex;
               IRow hssRow = CurrentSheet.GetRow(loopIndex);
               string standCurrentValue = GetCellValue(hssRow.GetCell(fromColumn));
               
               for (++loopIndex; loopIndex <= toRow; loopIndex++)
               {
                   IRow loopHssRow = CurrentSheet.GetRow(loopIndex);
                   string loopValue = GetCellValue(loopHssRow.GetCell(fromColumn));
                   
                   if(string.Compare(standCurrentValue,loopValue)!=0)
                   {
                       if (loopIndex - startRowIndex > 2)
                       {
                           RecursivelyMerged(startRowIndex, fromColumn, loopIndex-1, toClumn, config);
                       }
                       break;
                   }
                   else if (loopIndex == toRow)
                   {
                       RecursivelyMerged(startRowIndex, fromColumn, loopIndex, toClumn, config);
                   }
               }

           }

           return mergedResult;
       }

       
       
       
       
       
       
       
       
       private void RecursivelyMerged(int fromRow, int fromColumn, int toRow, int toColumn, MergedConfig orgianConfig)
       {
           bool isIndentity = false;
           int endMergedIndex = fromColumn;
           for (int loopIndex = toColumn; loopIndex >= fromColumn; loopIndex--)
           {
               isIndentity = ValidIdentity(fromRow, fromColumn, toRow, loopIndex);
               if (isIndentity)
               {
                   endMergedIndex = loopIndex;
                   break;
               }
           }

           CurrentSheet.AddMergedRegion(new CellRangeAddress(fromRow, fromColumn, toRow, endMergedIndex));

           if (toColumn - endMergedIndex > 0)
           {
               MergedConfig config = new MergedConfig();
               config.MergedType = orgianConfig.MergedType;
               config.MergedTypePriority = orgianConfig.MergedTypePriority;
               config.IgnoreEmpty = orgianConfig.IgnoreEmpty;
               config.NeedValid = orgianConfig.NeedValid;
               config.FromRow = fromRow + "";
               config.FromColumn = (endMergedIndex + 1).ToString();
               config.ToRow = toRow + "";
               config.ToColumn = toColumn + "";
               DoAllSingleMerged(config);

           }

       }

       
       
       
       
       
       
       
       
       private bool ValidIdentity(int fromRow,int fromColumn,int toRow,int toColumn)
       {
           bool isIdentyty = true;
           
           for (int loopIndex = fromRow; loopIndex <= toRow; loopIndex++)
           {
               IRow hssRow = CurrentSheet.GetRow(loopIndex);
               string fromColumnValue = GetCellValue(hssRow.GetCell(fromColumn));
               
               for (int columnIndex = fromColumn+1; columnIndex <= toColumn; columnIndex++)
               {
                   string currentColumnValue = GetCellValue(hssRow.GetCell(columnIndex));
                   isIdentyty = isIdentyty && string.Compare(fromColumnValue, currentColumnValue) == 0;
                   if(!isIdentyty)
                   {
                       break;
                   }
               }
               if (!isIdentyty)
               {
                   break;
               }
           }
           return isIdentyty;

       }

       
       
       
       
       
       private bool DoColumnSingleMerged(MergedConfig config)
       {
           bool mergedResult = true;
           int fromColumn = -1;
           int toClumn = -1;
           if (!int.TryParse(config.FromColumn, out fromColumn))
           {
               throw new ArgumentException("Merge配置中fromColumn参数不能为空");
           }
           if (!int.TryParse(config.ToColumn, out toClumn))
           {
               throw new ArgumentException("Merge配置中ToColumn参数不能为空或者只能为数字");
           }
           if (fromColumn > toClumn)
           {
               throw new ArgumentException("Merge配置中fromColumn参数不能大于ToColumn");
           }

           for (int i = fromColumn; i <= toClumn; i++)
           {
               this.DoColumnSingleMerged(config, i);
           }

           return mergedResult;
       }

       private void DoColumnSingleMerged(MergedConfig config, int indexColumn)
       {
           
           int fromRow = -1;
           if (!int.TryParse(config.FromRow, out fromRow))
           {
               fromRow = StartRow;
           }
           int loopIndex = fromRow;
           while (loopIndex <= EndRow)
           {
               int startRowIndex = loopIndex;
               IRow hssRow = CurrentSheet.GetRow(loopIndex);
               string standCurrentValue = GetCellValue(hssRow.GetCell(indexColumn));
               for (++loopIndex; loopIndex <= EndRow; loopIndex++)
               {
                   IRow loopHssRow = CurrentSheet.GetRow(loopIndex);
                   string loopValue = GetCellValue(loopHssRow.GetCell(indexColumn));
                   
                   if (string.Compare(standCurrentValue, loopValue) != 0)
                   {
                       if (loopIndex - fromRow > 2)
                       {
                           CurrentSheet.AddMergedRegion(new CellRangeAddress(startRowIndex, indexColumn, loopIndex - 1, indexColumn));
                       }
                       break;
                   }
                   
                   else if (loopIndex == EndRow)
                   {
                       CurrentSheet.AddMergedRegion(new CellRangeAddress(startRowIndex, indexColumn, loopIndex, indexColumn));
                   }
               }

           }
       }


       
       
       
       
       
       private string GetCellValue(ICell currentCell)
       {
           string currentRowValue = string.Empty;
           if(currentCell==null)
           {
               return currentRowValue;
           }
           CellType cType = currentCell.CellType;
           switch (cType)
           {
               case CellType.BOOLEAN:
                   currentRowValue = currentCell.BooleanCellValue + "";
                   break;
               case CellType.ERROR:
                   currentRowValue = currentCell.ErrorCellValue + "";
                   break;
               case CellType.FORMULA:
                   currentRowValue = currentCell.NumericCellValue + "";
                   break;
               case CellType.NUMERIC:
                   currentRowValue = currentCell.NumericCellValue + "";
                   break;
               case CellType.STRING:
                   currentRowValue = currentCell.StringCellValue + "";
                   break;
               default:
                   currentRowValue = currentCell.StringCellValue + "";
                   break;
           }

           return currentRowValue;
       }
    }
}
