using System.Runtime.Serialization;

namespace Clover.Component.Excel.Import
{
    
    
    
    public enum DataTypeEnums
    {
        
        
        
        [EnumMember (Value="String")]
        String = 0,
        
        
        
        [EnumMember (Value="Numeric")]
        Numeric,
        
        
        
        [EnumMember (Value="DateTime")]
        DateTime,
        
        
        
        [EnumMember (Value="Guid")]
        Guid
    }

    
    
    
    public enum DefaultValueSourceEnum
    {
        
        
        
        [EnumMember (Value="Constant")]
        Constant = 0,
        
        
        
        [EnumMember (Value="Property")]
        Property,
        
        
        
        [EnumMember (Value="Field")]
        Field,
        
        
        
        [EnumMember (Value="Method")]
        Method
    }
}