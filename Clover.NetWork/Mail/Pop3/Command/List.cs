using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Clover.Net.Command
{
    
    
    public class List : Pop3Command
    {
        private Int64? _MailIndex = null;
		
		
		
        public override String Name
        {
            get { return "List"; }
        }
		
		
		
        public Int64? MailIndex
        {
            get { return this._MailIndex; }
            set { this._MailIndex = value; }
        }
		
		
		
        public List()
        {
        }
		
		
		
		
        public List(Int64 mailIndex)
        {
            if (mailIndex < 1)
            { throw new ArgumentException(); }
            this._MailIndex = mailIndex;
        }
		
		
		
		
        public override String GetCommandString()
        {
			if (_MailIndex.HasValue == true)
			{
				return String.Format("{0} {1}", this.Name, this._MailIndex);
			}
			return this.Name;
        }
        
        
        
        
        public static Int64 GetMessageIndex(String line)
        {
            return Int64.Parse(Regex.Replace(line.Replace("\r\n", "")
                , @"^([0-9]+)[ |	]+.*$", "$1"));
        }
		
		
        
        
        public static Int32 GetSize(String line)
        {
            return Int32.Parse(Regex.Replace(line.Replace("\r\n", "")
                , @"^[0-9]+[ |	]+([0-9]+).*$", "$1"));
        }
		
		
        public class Result
        {
            private Int64 _MailIndex = 0;
            private Int32 _Size = 0;
			
			
			
            public Int64 MailIndex
            {
                get { return this._MailIndex; }
            }
			
			
			
            public Int32 Size
            {
                get { return this._Size; }
            }
			
			
			
			
            public Result(String text)
            {
                this._MailIndex = List.GetMessageIndex(text);
                this._Size = List.GetSize(text);
            }
        }
    }
}
