using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Clover.Net.Command
{
    
    
    public class Stat : Pop3Command
    {
		
		
		
        public override String Name
        {
            get { return "Stat"; }
        }
		
		
		
		
        public override String GetCommandString()
        {
            return this.Name;
        }
        
        
        
        
        public static Int64 GetTotalMessageCount(String text)
        {
            return Int64.Parse(Regex.Replace(text.Replace("\r\n", "")
                , @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$", "$1"));
        }
		
		
        
        
        public static Int64 GetTotalSize(String text)
        {
            return Int64.Parse(Regex.Replace(text.Replace("\r\n", "")
                , @"^.*\+OK[ |	]+[0-9]+[ |	]+([0-9]+).*$", "$1"));
        }
		
		
        public class Result
        {
            private Int64 _TotalMessageCount = 0;
            private Int64 _TotalSize = 0;
			
			
			
            public Int64 TotalMessageCount
            {
                get { return this._TotalMessageCount; }
            }
			
			
			
            public Int64 TotalSize
            {
                get { return this._TotalSize; }
            }
			
			
			
			
            public Result(String text)
            {
                this._TotalMessageCount = Stat.GetTotalMessageCount(text);
                this._TotalSize = Stat.GetTotalSize(text);
            }
        }
    }
}
