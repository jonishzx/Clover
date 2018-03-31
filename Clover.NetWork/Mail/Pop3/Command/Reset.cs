using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Command
{
    
    
    public class Rset : Pop3Command
    {
		
		
		
        public override String Name
        {
            get { return "Rset"; }
        }
		
		
		
		
        public override String GetCommandString()
        {
            return this.Name;
        }
    }
}
