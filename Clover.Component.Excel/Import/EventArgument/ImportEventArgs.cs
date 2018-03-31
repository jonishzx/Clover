









using System.Data;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using Clover.Component.Excel.Common.Validate;

namespace Clover.Component.Excel.Import.EventArgument
{
    public class ExcelImportArgs : EventArgs
    {
        #region 属性
        private string _filename;
        
        
        
        public string FileName
        {
            get { return _filename; }
            private set { _filename = value; }
        }
        
        
        
        public DataSet Container
        {
            get;
            set;
        }

        
        
        
        public bool Canceled
        {
            get;
            set;
        }

        
        
        
        public ExcelMessageCollection ExcelMessages 
        { get; set; }
        #endregion

        #region 初始化
        public ExcelImportArgs()  
        {
            Canceled = false;
        }

        public ExcelImportArgs(string fileName)
        {
            _filename = fileName;
            Canceled = false;
        }
        public ExcelImportArgs(string fileName, DataSet container, ExcelMessageCollection excelMessages)
        {
            _filename = fileName;
            Container = container;
            Canceled = false;
            ExcelMessages = excelMessages;
        }
        #endregion
    }

    public class ExcelImportWorksheetArgs : EventArgs
    {
        #region 属性

        
        
        
        public ISheet ISheet { get; protected set; }

        
        
        
        public Worksheet Worksheet { get; protected set; }

        
        
        
        public DataTable DataTable { get; set; }

        
        
        
        public WorksheetStatus WorksheetStatus { get; set; }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }
        #endregion

        #region 初始化

        public ExcelImportWorksheetArgs() { }

        public ExcelImportWorksheetArgs(ISheet hssfSheet, Worksheet worksheet, DataTable dataTable, ExcelMessageCollection excelMessages)
        {
            ISheet = hssfSheet;
            Worksheet = worksheet;
            DataTable = dataTable;
            WorksheetStatus = WorksheetStatus.Continue;
            ExcelMessages = excelMessages;
        }
        #endregion
    }

    public class ExcelImportRowArgs : EventArgs
    {
        #region 属性

        
        
        
        public DataRow DataRow { get; set; }

        
        
        
        public IRow IRow { get; protected set; }

        
        
        
        public ISheet ISheet { get; protected set; }

        
        
        
        public Worksheet Worksheet { get; protected set; }

        
        
        
        public RowStatus RowStatus { get; set; }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }
        #endregion

        #region ctor
        public ExcelImportRowArgs()
        {
            RowStatus = RowStatus.Continue;
        }

        public ExcelImportRowArgs(DataRow dataRow, IRow hssfRow, ISheet hssfSheet, Worksheet worksheet, ExcelMessageCollection excelMessages)
        {
            DataRow = dataRow;
            IRow = hssfRow;
            ISheet = hssfSheet;
            Worksheet = worksheet;
            RowStatus = RowStatus.Continue;
            ExcelMessages = excelMessages;
        }
        #endregion
    }

    public class ExcelImportItemArgs : EventArgs
    {
        #region 属性
        private int _rowIndex;
        
        
        
        public int RowIndex
        {
            get { return _rowIndex; }
            protected set { _rowIndex = value; }
        }

        private int _colIndex;
        
        
        
        public int ColumnIndex
        {
            get { return _colIndex; }
            protected set { _colIndex = value; }
        }

        
        
        
        private object _value;
        public object Value
        {
            get { return _value; }
            protected set { _value = value; }
        }

        
        
        
        public DataColumn DataColumn
        {
            get;
            protected set;
        }

        
        
        
        public OperationState OperationState { get; set; }

        public CellStatus CellStatus { get; set; }

        
        
        
        public ICell ICell { get; protected set; }

        
        
        
        public Dictionary<string, object>  DataRowValue { get; protected set; }

        #endregion

        #region 初始化
        public ExcelImportItemArgs() { }

        public ExcelImportItemArgs(int row, int col, object cellValue, DataColumn dataColumn, ICell hssfCell, OperationState OperationState, Dictionary<string, object> dataRowValue)
        {
            _rowIndex = row;
            _colIndex = col;
            _value = cellValue;
            CellStatus = CellStatus.Continue;
            DataColumn = dataColumn;
            ICell = hssfCell;
            OperationState = OperationState;
            DataRowValue = dataRowValue;
        }
        #endregion
    }

}
