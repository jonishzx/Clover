using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Command
{
    
    
    public class Top : Pop3Command
    {
        private Int64 _MailIndex = 1;
        private Int32 _LineCount = 0;
		
		
		
        public override String Name
        {
            get { return "Top"; }
        }
		
		
		
        public Int64 MailIndex
        {
            get { return this._MailIndex; }
        }
		
		
		
        public Int32 LineCount
        {
            get { return this._LineCount; }
        }
		
		
		
		
        public Top(Int64 mailIndex)
        {
            if (mailIndex < 1)
            { throw new ArgumentException(); }
            this._MailIndex = mailIndex;
        }
		
		
		
		
		
        public Top(Int64 mailIndex, Int32 lineCount)
        {
            if (mailIndex < 1)
            { throw new ArgumentException(); }
            this._MailIndex = mailIndex;
            this._LineCount = lineCount;
        }
		
		
		
		
        public override String GetCommandString()
        {
            return String.Format("{0} {1} {2}", this.Name, this._MailIndex, this._LineCount);
        }
    }
}
