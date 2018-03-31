using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Clover.Net.Mail;

namespace Clover.Net.Command
{
    
    
    
    
    public class Uidl : Pop3Command
    {
		private class RegexList
		{
			public static Regex CheckFormat = new Regex(@"^([0-9]+)[ |	]+([\x21-\x7e]+).*$", RegexOptions.None);
		}
        private Int64? _MailIndex = null;
		
		
		
        public override String Name
        {
            get { return "Uidl"; }
        }
		
		
		
        public Int64? MailIndex
        {
            get { return this._MailIndex; }
        }
		
		
		
        public Uidl()
        {
        }
		
		
		
		
        public Uidl(Int64 mailIndex)
        {
            if (mailIndex < 1)
            { throw new ArgumentException(); }
            this._MailIndex = mailIndex;
        }
		
		
		
		
        public override String GetCommandString()
        {
			if (this._MailIndex.HasValue == true)
			{
				return String.Format("{0} {1}", this.Name, this._MailIndex);
			}
			return this.Name;
        }
        
        
        
        
        
        
        public static String GetMessageIndex(String line)
        {
            return Regex.Replace(line.Replace(MailParser.NewLine, ""), @"^([0-9]+)[ |	]+.*$", "$1");
        }
        
        
        
        
        
        
        
        public static String GetUid(String line)
        {
			return Regex.Replace(line.Replace(MailParser.NewLine, ""), @"^[0-9]+[ |	]+([\x21-\x7E]+)$", "$1");
        }
		
		
		
        public class Result
        {
            private Int64 _MailIndex = 0;
            private String _Uid = "";
			
			
			
            public Int64 MailIndex
            {
                get { return this._MailIndex; }
            }
			
			
			
            public String Uid
            {
                get { return this._Uid; }
            }
			
			
			
			
            public Result(String text)
            {
				if (text == null)
				{ throw new ArgumentNullException("text"); }

                String mailIndexString = Uidl.GetMessageIndex(text);

                this._MailIndex = Convert.ToInt64(mailIndexString);
                this._Uid = text.Substring(mailIndexString.Length).Trim();
            }
			
			
			
			
			
            public Result(Int64 mailIndex, String uid)
            {
                this._MailIndex = mailIndex;
                this._Uid = uid;
            }
            
            
            
            
            
            
            public static Boolean CheckFormat(String line)
            {
				return RegexList.CheckFormat.IsMatch(line.Replace(MailParser.NewLine, ""));
            }
        }
    }
}
