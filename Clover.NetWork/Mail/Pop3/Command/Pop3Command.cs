using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Clover.Net.Mail;

namespace Clover.Net
{
    
	
    public abstract class Pop3Command
    {
		
		
		
        public abstract String Name { get;}
		
		
		
		
        public abstract String GetCommandString();
	}
}
