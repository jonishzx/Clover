using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Mail
{
	
    
    
    public class AsynchronousContext : IDisposable
    {
		private static BufferManager _BufferManager = null;
		private DateTime _StartTime = DateTime.Now;
		private ArraySegment<Byte> _Buffer;
		private List<Byte> _Data = new List<byte>();
        private Exception _Exception = null;
		private Boolean _Timeout = false;
		private Encoding _Encoding = Encoding.ASCII;
		private Boolean _IsDisposed = false;
		
		
		
		public static BufferManager BufferManager
		{
			get
			{
				if (_BufferManager == null)
				{
					_BufferManager = new BufferManager(256, 8192);
				}
				return _BufferManager;
			}
			set { _BufferManager = value; }
		}
		internal DateTime StartTime
		{
			get { return _StartTime; }
			set { _StartTime = value; }
		}
		internal ArraySegment<Byte> Buffer
		{
			get { return _Buffer; }
		}
		
		
		
		protected Encoding Encoding
		{
			get { return _Encoding; }
			set { _Encoding = value; }
		}
        
        
        
		public List<Byte> Data
		{
			get { return _Data; }
		}
        
        
        
        public Exception Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }
        
        
        
		public Boolean Timeout
		{
			get { return _Timeout; }
			set { _Timeout = value; }
		}
		internal AsynchronousContext(Encoding encoding)
		{
			_Encoding = encoding;
			_Buffer = AsynchronousContext.BufferManager.CheckOut();
		}
		
		
		
		
		
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}
		
		
		
		
		protected void Dispose(Boolean disposing)
		{
			if (disposing)
			{
				if (this._IsDisposed == false &&
					this._Buffer != null)
				{
					AsynchronousContext.BufferManager.CheckIn(this._Buffer);
					this._IsDisposed = true;
				}
			}
		}
		
		
		
		~AsynchronousContext()
		{
			this.Dispose(false);
		}
    }
}
