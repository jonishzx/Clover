using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Mail
{
	
    
    
    internal class AsynchronousSendContext : AsynchronousContext
    {
		private Int32 _Index = 0;
		private Byte[] _Bytes = null;
		private Int32 _SendBufferSize = 0;
		internal Int32 SendBufferSize
		{
			get { return _SendBufferSize; }
		}
		internal Boolean IsSendAllBytes
		{
			get { return _Index < _Bytes.Length; }
		}
		internal AsynchronousSendContext(Encoding encoding, Byte[] bytes) :
			base(encoding)
		{
			_Bytes = bytes;
			this._SendBufferSize = bytes.Length;
		}
		internal void FillBuffer()
		{
			Byte[] bb = Buffer.Array;

			if (_Index + bb.Length < _Bytes.Length)
			{
				for (int i = 0; i < bb.Length; i++)
				{
					bb[i] = _Bytes[_Index + i];
				}
                _SendBufferSize = bb.Length;
                _Index += bb.Length;
			}
			else
			{
				for (int i = 0; i < _Bytes.Length - _Index; i++)
				{
					bb[i] = _Bytes[_Index + i];
				}
				_SendBufferSize = _Bytes.Length - _Index;
				_Index = _Bytes.Length;	
			}
		}
    }
}
