using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clover.Component.Excel
{
    public class ExcelMessage
    {
        
        
        
        public string Message { get; set; }

        
        
        
        public DateTime LogTime { get; set; }

        
        
        
        public MessageType MessageType { get; set; }

        
        
        
        public bool IsException { get; set; }
    }
}
