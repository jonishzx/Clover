using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net
{
	
	
    public class Pop3ConnectException : Pop3Exception
    {
		
		
		
        public Pop3ConnectException()
        {
        }
		
		
		
		
        public Pop3ConnectException(String message)
            : base(message)
        {
        }
    }
}
