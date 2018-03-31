using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NPOI.SS.UserModel;

namespace Clover.Component.Excel.Export.EventArgument
{
    public class ExcelExportArgs : EventArgs
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

        
        
        
        public ExcelExportProvider excelProvider
        {
            get;
            set;
        }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }
        #endregion

        #region 初始化
        public ExcelExportArgs()
        {
            Canceled = false;
        }

        public ExcelExportArgs(string fileName)
        {
            _filename = fileName;
            Canceled = false;
        }

        public ExcelExportArgs(string fileName, DataSet container, ExcelMessageCollection excelMessages, ExcelExportProvider provider)
        {
            _filename = fileName;
            Container = container;
            Canceled = false;
            ExcelMessages = excelMessages;
            excelProvider = provider;
        }
        #endregion
    }

    public class ExcelExportWorksheetArgs : EventArgs
    {
        #region 属性

        
        
        
        public ISheet ISheet { get; set; }

        
        
        
        public Worksheet Worksheet { get; protected set; }

        
        
        
        public WorksheetStatus WorksheetStatus { get; set; }

        
        
        
        public DataSet WorksheetData { get; set; }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }
        #endregion

        #region 初始化

        public ExcelExportWorksheetArgs() { }

        public ExcelExportWorksheetArgs(ISheet hssfSheet, Worksheet worksheet, ExcelMessageCollection excelMessages,DataSet datas)
        {
            ISheet = hssfSheet;
            WorksheetData = datas;
            Worksheet = worksheet;
            WorksheetStatus = WorksheetStatus.Continue;
            ExcelMessages = excelMessages;
        }
        #endregion
    }

    public class ExcelExportAreaArgs : EventArgs
    {
        #region 属性

        
        
        
        public ISheet ISheet { get; set; }

        
        
        
        public Area Area { get; protected set; }

        
        
        
        public AreaStatus AreaStatus { get; set; }

        public DataTable DataTable { get; set; }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }
        #endregion

        #region 初始化

        public ExcelExportAreaArgs() { }

        public ExcelExportAreaArgs(ISheet hssfSheet, Area area, DataTable dataTable, ExcelMessageCollection excelMessages)
        {
            ISheet = hssfSheet;
            Area = area;
            DataTable = dataTable;
            AreaStatus = AreaStatus.Continue;
            ExcelMessages = excelMessages;
        }
        #endregion
    }

    public class ExcelExportRowArgs : EventArgs
    {
        #region 属性

        
        
        
        public DataRow DataRow { get; set; }

        
        
        
        public IRow IRow { get; set; }

        
        
        
        public ISheet ISheet { get; protected set; }

        
        
        
        public IWorkbook HSSFWorkBook { get; private set; }

        
        
        
        public Area Area { get; protected set; }

        
        
        
        public RowStatus RowStatus { get; set; }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }
        #endregion

        #region ctor
        public ExcelExportRowArgs()
        {
            RowStatus = RowStatus.Continue;
        }

        
        
        
        
        
        
        
        
        

        public ExcelExportRowArgs(DataRow dataRow, IRow hssfRow, ISheet hssfSheet,IWorkbook hssfBook, Area area, ExcelMessageCollection excelMessages)
        {
            DataRow = dataRow;
            IRow = hssfRow;
            ISheet = hssfSheet;
            Area = area;
            RowStatus = RowStatus.Continue;
            HSSFWorkBook = hssfBook;
            ExcelMessages = excelMessages;
        }
        #endregion
    }

    public class ExcelExportItemArgs : EventArgs
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

        
        
        
        private object _oldValue;
        public object OldValue
        {
            get { return _oldValue; }
            protected set { _oldValue = value; }
        }

        
        
        
        private object _newValue;
        public object NewValue
        {
            get { return _newValue; }
            set { _newValue = value; }
        }

        
        
        
        public DataFieldBase DataFieldBase
        {
            get;
            protected set;
        }

        public bool IsRepeat
        {
            get
            {
                return DataFieldBase is DataColumn;
            }
        }

        
        
        
        public ExcelMessageCollection ExcelMessages
        { get; set; }

        public CellStatus CellStatus { get; set; }

        
        
        
        public ICell ICell { get; set; }

        #endregion

        #region 初始化
        public ExcelExportItemArgs() { }

        public ExcelExportItemArgs(int row, int col, object oldValue, object newValue, DataFieldBase dataFieldBase, ICell hssfCell, ExcelMessageCollection excelMessages)
        {
            _rowIndex = row;
            _colIndex = col;
            _oldValue = oldValue;
            _newValue = newValue;
            CellStatus = CellStatus.Continue;
            DataFieldBase = dataFieldBase;
            ICell = hssfCell;
            ExcelMessages = excelMessages;
        }
        #endregion
    }
}
