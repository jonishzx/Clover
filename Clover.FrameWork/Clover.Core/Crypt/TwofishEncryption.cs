using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Twofish_NET
{
	
	
	
	internal class TwofishEncryption : TwofishBase, ICryptoTransform
	{
		public TwofishEncryption(int keyLen, ref byte[] key, ref byte[] iv, CipherMode cMode, EncryptionDirection direction)
		{
			
			for (int i=0;i<key.Length/4;i++)
			{
				Key[i] = (uint)( key[i*4+3]<<24) | (uint)(key[i*4+2] << 16) | (uint)(key[i*4+1] << 8) | (uint)(key[i*4+0]);
			}

			cipherMode = cMode;

			
			if (cipherMode == CipherMode.CBC)
			{
				for (int i=0;i<4;i++)
				{
					IV[i] = (uint)( iv[i*4+3]<<24) | (uint)(iv[i*4+2] << 16) | (uint)(iv[i*4+1] << 8) | (uint)(iv[i*4+0]);
				}
			}

			encryptionDirection = direction;
			reKey(keyLen,ref Key);
		}

		
		public void Dispose()
		{
		}


		
		
		
		
		
		
		
		
		
		public int TransformBlock(
			byte[] inputBuffer,
			int inputOffset,
			int inputCount,
			byte[] outputBuffer,
			int outputOffset
			)
		{			
			uint[] x=new uint[4];

			
			for (int i=0;i<4;i++)
			{
				x[i]= (uint)(inputBuffer[i*4+3+inputOffset]<<24) | (uint)(inputBuffer[i*4+2+inputOffset] << 16) | 
					(uint)(inputBuffer[i*4+1+inputOffset] << 8) | (uint)(inputBuffer[i*4+0+inputOffset]);

			}

			if (encryptionDirection == EncryptionDirection.Encrypting)
			{
				blockEncrypt(ref x);
			}
			else
			{
				blockDecrypt(ref x);
			}


			
			for (int i=0;i<4;i++)
			{
				outputBuffer[i*4+0+outputOffset] = b0(x[i]);
				outputBuffer[i*4+1+outputOffset] = b1(x[i]);
				outputBuffer[i*4+2+outputOffset] = b2(x[i]);
				outputBuffer[i*4+3+outputOffset] = b3(x[i]);
			}


			return inputCount;
		}

		public byte[] TransformFinalBlock(
			byte[] inputBuffer,
			int inputOffset,
			int inputCount
			)
		{
			byte[] result;
			
			if (inputCount>0)
			{
				int rest = inputCount % 16;
				int bufsize = inputCount - rest;
				if( rest > 0 ) bufsize += 16;
				result = new byte[bufsize];
				Array.Copy(inputBuffer,0,result,0,inputCount);

				for(int i = inputCount; i < bufsize; i++) 
					result[i] = (byte)0;
				TransformBlock(result,0,bufsize,result,0);

			}
			else
			{
				
				result = new byte[inputCount]; 
			}
			
			return result;
		}

		
		private bool canReuseTransform = true;
		public bool CanReuseTransform
		{
			get
			{
				return canReuseTransform;
			}
		}

		
		
		private bool canTransformMultipleBlocks = false;
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return canTransformMultipleBlocks;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return inputBlockSize;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return outputBlockSize;
			}
		}

		private EncryptionDirection encryptionDirection;
	}
}
