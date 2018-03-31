using System;
using System.Collections.Generic;
using System.Text;
using Clover.Net.Mail;

namespace Clover.Net
{
    
    
    public class Pop3CommandResult
    {
        private String _Text = "";
        private Boolean _Ok = true;
		
		
		
        public String Text
        {
            get { return this._Text; }
        }
		
		
		
        public Boolean Ok
        {
            get { return this._Ok; }
        }
		
		
		
		
        public Pop3CommandResult(String text)
        {
            this._Ok = MailParser.IsResponseOk(text);
            this._Text = text;
        }
    }
}
