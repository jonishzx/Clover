using System;
using System.Net;

namespace Clover.Net
{
	
	
	public class Pop3Exception : Exception
	{
		
		
		
		public Pop3Exception()
		{
		}
		
		
		
		
		public Pop3Exception(String message)
			: base(message)
		{
		}
		
		
		
		
		public Pop3Exception(Exception innerException)
			: base(innerException.Message, innerException)
		{
		}
	}
}