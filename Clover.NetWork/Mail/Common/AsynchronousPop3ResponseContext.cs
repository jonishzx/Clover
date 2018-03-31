using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Mail
{
    
    
    
    internal class AsynchronousPop3ResponseContext: AsynchronousContext
    {
		private Boolean _IsMultiLine = false;
		private EndGetResponse _EndGetResponseCallback = null;
		internal AsynchronousPop3ResponseContext(Encoding encoding, Boolean isMultiLine) :
			base(encoding)
		{
			_IsMultiLine = isMultiLine;
		}
		internal AsynchronousPop3ResponseContext(Encoding encoding, Boolean isMultiLine, EndGetResponse callbackFunction):
			base(encoding)
		{
			_IsMultiLine = isMultiLine;
			_EndGetResponseCallback = callbackFunction;
		}
		
		
		
		
		
		
		internal Boolean ReadBuffer(Int32 size)
		{
			List<Byte> l = this.Data;
			Byte[] bb = this.Buffer.Array;

			for (int i = 0; i < size; i++)
			{
				l.Add(bb[i]);
				bb[i] = 0;
			}

			if (this._IsMultiLine == false)
			{
				if (size < bb.Length)
				{
					return false;
				}
				
				if (l.Count > 1 &&
					l[l.Count - 2] == 13 &&
					l[l.Count - 1] == 10)
				{ return false; }
			}
			else 
			{
				
				if (l.Count > 4 &&
					l[l.Count - 5] == 13 &&
					l[l.Count - 4] == 10 &&
					l[l.Count - 3] == 46 &&
					l[l.Count - 2] == 13 &&
					l[l.Count - 1] == 10)
				{ return false; }
			}
			return true;
		}
		internal void OnEndGetResponse()
		{
			var eh = _EndGetResponseCallback;
			if (eh != null)
			{
				eh(this.Encoding.GetString(this.Data.ToArray()));
			}
		}
	}
}
