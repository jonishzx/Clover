using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net
{
	
	
    public class Pop3ReceiveException : Pop3Exception
    {
		
		
		
        public Pop3ReceiveException()
        {
        }
		
		
		
		
        public Pop3ReceiveException(String message)
            : base(message)
        {
        }
		
		
		
		
		public Pop3ReceiveException(Exception innerException)
			: base(innerException)
		{
		}
	}
}
