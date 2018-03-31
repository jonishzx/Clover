using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Command
{
    
    
    public class Noop : Pop3Command
    {
		
		
		
        public override String Name
        {
            get { return "Noop"; }
        }
		
		
		
		
        public override String GetCommandString()
        {
            return this.Name;
        }
    }
}
