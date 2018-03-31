using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Clover.Component.Excel.Export
{
    
    public enum DataTypeEnums
    {
        
        
        
        [EnumMember (Value="String")]
        String = 0,
        
        
        
        [EnumMember (Value="Numeric")]
        Numeric,
        
        
        
        [EnumMember (Value="DateTime")]
        DateTime,
        
        
        
        [EnumMember(Value = "Boolean")]
        Boolean,
        
        
        
        [EnumMember(Value = "Formula")]
        Formula
    }

    public enum ValueSourceEnums
    {
        
        
        
        [EnumMember(Value = "Constant")]
        Constant = 0,
        
        
        
        [EnumMember(Value = "Property")]
        Property,
        
        
        
        [EnumMember(Value = "Field")]
        Field,
        
        
        
        [EnumMember(Value = "Method")]
        Method
    }

    public enum FormulaTypeEnums
    {
        [EnumMember(Value = "Column")]
        Cell = 0,
        [EnumMember(Value = "Column")]
        Column,
        [EnumMember(Value = "Row")]
        Row,
        [EnumMember(Value = "RowAndColumn")]
        RowAndColumn
    }

    public enum ArgumentTypeEnums
    {
        [EnumMember(Value = "String")]
        String = 0,
        [EnumMember(Value = "Short")]
        Short,
        [EnumMember(Value = "Byte")]
        Byte,
        [EnumMember(Value = "Boolean")]
        Boolean,
        [EnumMember(Value = "Other")]
        Reflect = 1000
    }

    public enum ColumnPriorityEnums
    {
        [EnumMember(Value = "Row")]
        Row = 0,
        [EnumMember(Value = "Column")]
        Column,
    }
}
