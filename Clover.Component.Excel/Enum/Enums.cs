using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Clover.Component.Excel
{
    
    
    
    public enum MessageType
    {
        
        
        
        [EnumMember(Value = "Config")]
        Config = 0,
        
        
        
        [EnumMember(Value = "Event")]
        Event = 0,
        
        
        
        [EnumMember(Value = "Workbook")]
        Workbook,
        
        
        
        [EnumMember(Value = "Worksheet")]
        Worksheet,
        
        
        
        [EnumMember(Value = "Area")]
        Area,
        
        
        
        [EnumMember(Value = "Row")]
        Row,
        
        
        
        [EnumMember(Value = "Cell")]
        Cell
    }

    public enum ExcelResultType
    {
        
        
        
        [EnumMember(Value = "Failed")]
        Failed = 0,
        
        
        
        [EnumMember(Value = "Canceled")]
        Canceled,
        
        
        
        [EnumMember(Value = "CompletedWithException")]
        CompletedWithException,
        
        
        
        [EnumMember(Value = "Succeed")]
        Succeed
    }

    
    
    
    public enum WorksheetStatus
    {
        
        
        
        [EnumMember(Value = "Continue")]
        Continue = 0,
        
        
        
        [EnumMember(Value = "Skip")]
        Skip,
        
        
        
        [EnumMember(Value = "Canceled")]
        Canceled
    }

    
    
    
    public enum AreaStatus
    {
        
        
        
        [EnumMember(Value = "Continue")]
        Continue = 0,
        
        
        
        [EnumMember(Value = "Skip")]
        Skip,
        
        
        
        [EnumMember(Value = "Canceled")]
        Canceled
    }

    
    
    
    public enum RowStatus
    {
        
        
        
        [EnumMember(Value = "Continue")]
        Continue = 0,
        
        
        
        [EnumMember(Value = "Skip")]
        Skip,
        
        
        
        [EnumMember(Value = "Finish")]
        Finish,
        
        
        
        [EnumMember(Value = "Canceled")]
        Canceled
    }

    
    
    
    public enum CellStatus
    {
        
        
        
        [EnumMember(Value = "Continue")]
        Continue = 0,
        
        
        
        [EnumMember(Value = "Finish")]
        IngoreRow,
        
        
        
        [EnumMember(Value = "Canceled")]
        Canceled
    }
}
