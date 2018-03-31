using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net
{
	
	
    public class Pop3SendException : Pop3Exception
    {
		
		
		
        public Pop3SendException()
        {
        }
		
		
		
		
		public Pop3SendException(Exception innerException)
			: base(innerException)
		{
		}
	}
}
