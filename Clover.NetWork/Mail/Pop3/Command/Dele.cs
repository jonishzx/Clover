using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Command
{
    
    
    public class Dele : Pop3Command
    {
        private Int64 _MailIndex = 1;
		
		
		
        public override String Name
        {
            get { return "Dele"; }
        }
		
		
		
        public Int64 MailIndex
        {
            get { return this._MailIndex; }
            set { this._MailIndex = value; }
        }
		
		
		
		
        public Dele(Int64 mailIndex)
        {
            if (mailIndex < 1)
            { throw new ArgumentException(); }
            this._MailIndex = mailIndex;
        }
		
		
		
		
        public override String GetCommandString()
        {
            return String.Format("{0} {1}", this.Name, this._MailIndex);
        }
    }
}
