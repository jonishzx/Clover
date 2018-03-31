using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clover.Component.Excel.Import.EventArgument;

namespace Clover.Component.Excel
{
    public static class ExcelMessageFactory
    {
        
        
        
        
        
        public static ExcelMessage GenerateEvent(string msg, bool isException)
        {
            return new ExcelMessage
            {
                MessageType = MessageType.Event,
                LogTime = DateTime.Now,
                Message = msg,
                IsException = isException
            };
        }

        
        
        
        
        
        
        public static ExcelMessage GenerateWorksheet(string msg, bool isException)
        {
            return new ExcelMessage
            {
                MessageType = MessageType.Worksheet,
                LogTime = DateTime.Now,
                Message = msg,
                IsException = isException
            };
        }

        
        
        
        
        
        
        public static ExcelMessage GenerateArea(string msg, bool isException)
        {
            return new ExcelMessage
            {
                MessageType = MessageType.Area,
                LogTime = DateTime.Now,
                Message = msg,
                IsException = isException
            };
        }

        
        
        
        
        
        
        public static ExcelMessage GenerateRow(string msg, bool isException)
        {
            return new ExcelMessage
            {
                MessageType = MessageType.Row,
                LogTime = DateTime.Now,
                Message = msg,
                IsException = isException
            };
        }

        
        
        
        
        
        
        public static ExcelMessage GenerateCell(string msg, bool isException)
        {
            return new ExcelMessage
            {
                MessageType = MessageType.Cell,
                LogTime = DateTime.Now,
                Message = msg,
                IsException = isException
            };
        }

        
        
        
        
        
        
        public static void PrepareEventMessage(ExcelResult result, string eventName, bool isCancel)
        {
            if (isCancel)
            {
                result.ExcelMessages.Add(GenerateEvent(string.Format("在事件{0}中被取消", eventName), true));
                result.Result = ExcelResultType.Canceled;
            }
        }

        
        
        
        
        
        
        
        
        
        public static bool PrepareCellMessage(ExcelResult result, int iLoopRow, int jLoopCol, CellStatus cellStatus, string eventName)
        {
            bool breakFromIngore = false;
            switch (cellStatus)
            {
                case CellStatus.Canceled:
                    result.ExcelMessages.Add(GenerateEvent(
                    string.Format("第 {0} 行第 {1} 列在事件{2}中被取消", iLoopRow, jLoopCol, eventName), true));
                    result.Result = ExcelResultType.Canceled;
                    break;
                case CellStatus.IngoreRow:
                    result.ExcelMessages.Add(GenerateEvent(
                    string.Format("第 {0} 行第 {1} 列在事件{2}中被忽略", iLoopRow, jLoopCol, eventName), false));
                    breakFromIngore = true;
                    break;
            }
            return breakFromIngore;
        }

        
        
        
        
        
        
        
        public static void PrepareRowMessage(ExcelResult result, int iLoopRow, RowStatus rowStatus, string eventName)
        {
            switch (rowStatus)
            {
                case RowStatus.Canceled:
                    result.ExcelMessages.Add(GenerateEvent(
                    string.Format("第 {0} 行在事件{1}中被取消", iLoopRow, eventName), true));
                    result.Result = ExcelResultType.Canceled;
                    break;
                case RowStatus.Finish:
                    result.ExcelMessages.Add(GenerateEvent(
                    string.Format("第 {0} 行在事件{1}中被完成", iLoopRow, eventName), false));
                    break;
                case RowStatus.Skip:
                    result.ExcelMessages.Add(GenerateEvent(
                    string.Format("第 {0} 行在事件{1}中被跳过", iLoopRow, eventName), false));
                    break;
            }
        }

        
        
        
        
        
        
        
        public static void PrepareSheetMessage(ExcelResult result, string sheetName, WorksheetStatus worksheetStatus, string eventName)
        {
            switch (worksheetStatus)
            {
                case WorksheetStatus.Canceled:
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateEvent(
                    string.Format("工作表：{0}在事件{1}中被取消", sheetName, eventName), true));
                    result.Result = ExcelResultType.Canceled;
                    break;
                case WorksheetStatus.Skip:
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateEvent(
                    string.Format("工作表：{0}在事件{1}中被跳过", sheetName, eventName), false));
                    break;
            }
        }

        
        
        
        
        
        
        
        public static void PrepareAreaMessage(ExcelResult result, string sheetName, AreaStatus areaStatus, string eventName)
        {
            switch (areaStatus)
            {
                case AreaStatus.Canceled:
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateEvent(
                    string.Format("工作表：{0}在事件{1}中被取消", sheetName, eventName), true));
                    result.Result = ExcelResultType.Canceled;
                    break;
                case AreaStatus.Skip:
                    result.ExcelMessages.Add(ExcelMessageFactory.GenerateEvent(
                    string.Format("工作表：{0}在事件{1}中被跳过", sheetName, eventName), false));
                    break;
            }
        }
    }
}
