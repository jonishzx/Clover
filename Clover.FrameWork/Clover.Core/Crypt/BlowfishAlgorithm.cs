



using System;
using System.Security;
using System.Security.Cryptography;


namespace Blowfish_NET
{
	public class BlowfishAlgorithm : SymmetricAlgorithm, ICryptoTransform
	{
		
		

		Blowfish m_bf;
		BlowfishCBC m_bfc;
		bool m_blIsEncryptor;


		

		RNGCryptoServiceProvider m_rng;

		
		
		
		public BlowfishAlgorithm() : base()
		{
			m_bf = null;
			m_bfc = null;

			
			IVValue = null;
			KeyValue = null;
			KeySizeValue = Blowfish.MAXKEYLENGTH * 8;

			LegalBlockSizesValue = new KeySizes[1];
			LegalBlockSizesValue[0] = new KeySizes(BlockSize, BlockSize, 8);

			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(0, Blowfish.MAXKEYLENGTH * 8, 8);

			ModeValue = CipherMode.ECB;

			m_rng = null;
		}

		BlowfishAlgorithm
			(
			byte[] key,
			byte[] iv,
			bool blCBC,
			bool blIsEncryptor
			)
		{
			if (null == key) GenerateKey(); else Key = key;
 
			if (blCBC)
			{
			    if (null == iv) GenerateIV(); else IV = iv;

				m_bf = null;
				m_bfc = new BlowfishCBC(KeyValue, IVValue);
			}
			else
			{
				m_bf = new Blowfish(KeyValue);
				m_bfc = null;
			}
			
			m_blIsEncryptor = blIsEncryptor;
		}


		
		
		
		
		
		
		
		
		
		public static bool IsWeakKey
			(byte[] key)
		{
			BlowfishAlgorithm bfAlg = new BlowfishAlgorithm(key, null, false, true);
			
			return bfAlg.m_bf.IsWeakKey;
		}


		


		
		
		
		public override int BlockSize
		{
			get
			{
				return Blowfish.BLOCKSIZE * 8;
			}
			set
			{
				
				
				if (value != Blowfish.BLOCKSIZE * 8)
				{
					throw new CryptographicException("illegal blocksize");
				}
			}
		}

		
		
		
		public override byte[] IV
		{
			get
			{
				return IVValue;
			}
			set
			{
				if (null == value) 
				{
					throw new ArgumentNullException();
				}
				if (value.Length != Blowfish.BLOCKSIZE) 
				{
					throw new CryptographicException("illegal IV length");
				}
				IVValue = value;
			}
		}

		
		
		
		public override byte[] Key
		{
			get
			{
				return KeyValue;
			}
			set
			{
				if (null == value) 
				{
					throw new ArgumentNullException("key cannot be null");
				}
				
				
				
				
				KeyValue = value;
			}
		}

		
		
		
		public override int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				KeySizes ks = LegalKeySizes[0];
				if ((0 != (value % ks.SkipSize)) ||
					(value > ks.MaxSize) ||
					(value < ks.MinSize))
				{
					throw new CryptographicException("invalid key size");
				}
				KeySizeValue = value;
			}
		}

		public override KeySizes[] LegalBlockSizes
		{
			get
			{
				return LegalBlockSizesValue;
			}
		}
	
		public override KeySizes[] LegalKeySizes
		{
			get
			{
				return LegalKeySizesValue;
			}
		}

		public override CipherMode Mode
		{
			get
			{
				return ModeValue;
			}
			set
			{
				
				
				
				if (value != CipherMode.CBC &&
					value != CipherMode.ECB)
				{
					throw new CryptographicException("only ECB and CBC are supported");
				}
				ModeValue = value;
			}
		}

		

		public override ICryptoTransform CreateEncryptor
			(
			byte[] key,
			byte[] iv
			)
		{
			BlowfishAlgorithm result = new BlowfishAlgorithm(
				key, 
				iv, 
				(CipherMode.CBC == ModeValue),
				true);

			result.Padding = Padding;
			return result;
		}

		public override ICryptoTransform CreateDecryptor
			(
			byte[] key,
			byte[] iv
			)
		{
			BlowfishAlgorithm result = new BlowfishAlgorithm(
				key, 
				iv, 
				(CipherMode.CBC == ModeValue),
				false);

			result.Padding = Padding;
			return result;
		}

		
		

		public override void GenerateKey()
		{
			if (null == m_rng) m_rng = new RNGCryptoServiceProvider();
			
			KeyValue = new byte[KeySizeValue / 8];
			m_rng.GetBytes(KeyValue);		
		}

		public override void GenerateIV()
		{
			if (null == m_rng) m_rng = new RNGCryptoServiceProvider();
			
			IVValue = new byte[Blowfish.BLOCKSIZE];
			m_rng.GetBytes(IVValue);		
		}

		

		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return Blowfish.BLOCKSIZE;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return Blowfish.BLOCKSIZE;
			}
		}

		public int TransformBlock
			(
			byte[] bufIn,
			int nOfsIn,
			int nCount,
			byte[] bufOut,
			int nOfsOut
			)
		{
			
			
			

			int nResult = 0;

			if (null != m_bfc)
			{
				if (m_blIsEncryptor)
				{
					nResult = m_bfc.Encrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount);
				}
				else
				{
					nResult = m_bfc.Decrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount);
				}
			}
			else if (null != m_bf)
			{
				if (m_blIsEncryptor)
				{
					nResult = m_bf.Encrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount);
				}
				else
				{
					nResult = m_bf.Decrypt(bufIn, bufOut, nOfsIn, nOfsOut, nCount);
				}
			}
			else
			{
				nResult = 0;
			}

			return nResult * Blowfish.BLOCKSIZE;
		}

		public byte[] TransformFinalBlock
			(
			byte[] inBuf,
			int nOfs,
			int nCount
			)
		{
			byte[] result;

			if (m_blIsEncryptor)
			{
				
				

				int nRest = nCount % Blowfish.BLOCKSIZE;

				
				
				

				int nBufSize = nCount - nRest;
				int nFill;  

				if (PaddingMode.PKCS7 == PaddingValue)
				{
					nBufSize += Blowfish.BLOCKSIZE;
					nFill = Blowfish.BLOCKSIZE - nRest;
				}
				else
				{
					if (0 < nRest) nBufSize += Blowfish.BLOCKSIZE;
					nFill = 0;
				}

				result = new byte[nBufSize];
				Array.Copy(inBuf, nOfs, result, 0, nCount);
            
				for (int nI = nCount; nI < nBufSize; nI++)
				{
					result[nI] = (byte)nFill;
				}

				TransformBlock(result, 0, nBufSize, result, 0);
			}
			else
			{
				byte[] lastBlocks = new byte[nCount];
				if (0 < nCount)
				{
					TransformBlock(inBuf, nOfs, nCount, lastBlocks, 0);
					
					if (PaddingMode.PKCS7 == PaddingValue)
					{
					  nCount -= lastBlocks[nCount - 1];
					}
					
					result = new byte[nCount];
					Array.Copy(lastBlocks, 0, result, 0, nCount);
				}
				else
				{
					
					result = lastBlocks;
				}			
			}

			return result;
		}
	}
}
