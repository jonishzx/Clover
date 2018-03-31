using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net
{
	
	
	
	public class SocketMessageEventArgs : EventArgs
	{
		
		
		
		public String Text { get; set; }
		
		
		
		
		public SocketMessageEventArgs(String text)
		{
			this.Text = text;
		}
	}
}
