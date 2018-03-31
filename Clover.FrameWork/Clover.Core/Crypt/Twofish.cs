using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Twofish_NET
{
	
	
	
	
	
	public sealed class Twofish : SymmetricAlgorithm
	{
		
		
		
		public Twofish()
		{
			this.LegalKeySizesValue = new KeySizes[]{new KeySizes(128,256,64)}; 

			this.LegalBlockSizesValue = new KeySizes[]{new KeySizes(128,128,0)}; 

			this.BlockSize = 128; 
			this.KeySize = 128; 

			this.Padding = PaddingMode.Zeros; 

			this.Mode = CipherMode.ECB;

		}

		
		
		
		
		
		public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
		{
			Key = key; 

			if (Mode == CipherMode.CBC)
				IV = iv;
			
			return new TwofishEncryption(KeySize, ref KeyValue, ref IVValue, ModeValue, TwofishBase.EncryptionDirection.Encrypting);
		}

		
		
		
		
		
		public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
		{
			Key = key;

			if (Mode == CipherMode.CBC)
				IV = iv;

			return new TwofishEncryption(KeySize, ref KeyValue, ref IVValue, ModeValue, TwofishBase.EncryptionDirection.Decrypting);
		}

		
		
		
		public override void GenerateIV()
		{
			IV = new byte[16]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		}

		
		
		
		public override void GenerateKey()
		{
			Key = new byte[KeySize/8];

			
			for (int i=Key.GetLowerBound(0);i<Key.GetUpperBound(0);i++)
			{
				Key[i]=0;
			}
		}

		
		
		
		public override CipherMode Mode
		{
			set
			{
				switch (value)
				{
					case CipherMode.CBC:
						break;
					case CipherMode.ECB:
						break;
					default:
						throw (new CryptographicException("Specified CipherMode is not supported."));
				}
				this.ModeValue = value;
			}
		}

	}
}
