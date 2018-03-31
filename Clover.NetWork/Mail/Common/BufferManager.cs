using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Net.Mail
{
	
	
	
	
	
	
	
	
	
    
	
	
	
	
	
	
	public class BufferManager
	{
		private readonly Int32 _SegmentChunks;
		private readonly Int32 _ChunkSize;
		private readonly Int32 _SegmentSize;
		private readonly Stack<ArraySegment<Byte>> _Buffers;
		private readonly Object _LockObject = new Object();
		private readonly List<Byte[]> _Segments;
		
		
		
		public Int32 AvailableBuffers
		{
			get { return _Buffers.Count; } 
		}
		
		
		
		public Int32 TotalBufferSize
		{
			get { return _Segments.Count * _SegmentSize; } 
		}
		
		
		
		private void CreateNewSegment()
		{
			Byte[] Bytes = new Byte[_SegmentChunks * _ChunkSize];
			_Segments.Add(Bytes);
			for (Int32 i = 0; i < _SegmentChunks; i++)
			{
				ArraySegment<Byte> chunk = new ArraySegment<Byte>(Bytes, i * _ChunkSize, _ChunkSize);
				_Buffers.Push(chunk);
			}
		}
		
		
		
		
		
		
		
		
		public ArraySegment<Byte> CheckOut()
		{
			lock (_LockObject)
			{
				if (_Buffers.Count == 0)
				{
					CreateNewSegment();
				}
				return _Buffers.Pop();
			}
		}
		
		
		
		
		
		
		
		
		public void CheckIn(ArraySegment<Byte> buffer)
		{
			lock (_LockObject)
			{
				_Buffers.Push(buffer);
			}
		}
		
		
		
		
		
		public BufferManager(Int32 segmentChunks, Int32 chunkSize) :
			this(segmentChunks, chunkSize, 1) { }
		
		
		
		
		
		
		public BufferManager(Int32 segmentChunks, Int32 chunkSize, Int32 initialSegments)
		{
			_SegmentChunks = segmentChunks;
			_ChunkSize = chunkSize;
			_SegmentSize = _SegmentChunks * _ChunkSize;
			_Buffers = new Stack<ArraySegment<Byte>>(segmentChunks * initialSegments);
			_Segments = new List<Byte[]>();
			for (Int32 i = 0; i < initialSegments; i++)
			{
				this.CreateNewSegment();
			}
		}
	}
}