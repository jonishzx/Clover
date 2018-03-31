using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Mail
{
	
    
    
    internal class AsynchronousSmtpResponseContext : AsynchronousContext
    {
		internal AsynchronousSmtpResponseContext(Encoding encoding):
			base(encoding)
		{
		}
		
		
		
		
		
		
		internal Boolean ReadBuffer(Int32 size)
		{
			Int32? NewLineStartIndex = null;
			Boolean isLastLine = false;
			List<Byte> l = this.Data;

			for (int i = 0; i < size; i++)
			{
				l.Add(this.Buffer.Array[i]);
				this.Buffer.Array[i] = 0;
				
				if (l.Count == 4)
				{
					if (l[3] == 32)
					{
						isLastLine = true;
					}
				}
				else if (l.Count > 4 &&
					l[l.Count - 2] == 13 &&
					l[l.Count - 1] == 10)
				{
					NewLineStartIndex = l.Count + 1;
				}
				if (NewLineStartIndex.HasValue == true &&
					NewLineStartIndex.Value + 3 == l.Count)
				{
					
					if (l[l.Count - 1] == 32)
					{
						isLastLine = true;
					}
				}
			}

			if (isLastLine == true)
			{
				if (size < this.Buffer.Array.Length)
				{
					return false;
				}
				
				if (l.Count > 1 &&
					l[l.Count - 2] == 13 &&
					l[l.Count - 1] == 10)
				{ return false; }
			}
			return true;
		}
    }
}
