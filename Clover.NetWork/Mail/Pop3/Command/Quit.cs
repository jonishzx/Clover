using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Command
{
    
    
    public class Quit : Pop3Command
    {
		
		
		
        public override String Name
        {
            get { return "Quit"; }
        }
		
		
		
		
        public override String GetCommandString()
        {
            return this.Name;
        }
    }
}
