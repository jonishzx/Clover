



using System;


namespace Blowfish_NET
{

	
	
	
	
	
	
	
	public class BlowfishCBC : Blowfish
	{

		uint m_unIvHi; 
		uint m_unIvLo;


		
		
		
		public byte[] Iv
		{
			set 
			{ 
				m_unIvHi = (((uint)value[0]) << 24) |
					(((uint)value[1]) << 16) |
					(((uint)value[2]) << 8) |
					((uint)value[3]);

				m_unIvLo = (((uint)value[4]) << 24) |
					(((uint)value[5]) << 16) |
					(((uint)value[6]) << 8) |
					((uint)value[7]);
			}

			get 
			{ 
				byte[] result = new byte[Blowfish.BLOCKSIZE];
 
				result[0] = (byte)(m_unIvHi >> 24);
				result[1] = (byte)(m_unIvHi >> 16);
				result[2] = (byte)(m_unIvHi >> 8);
				result[3] = (byte)m_unIvHi;
				result[4] = (byte)(m_unIvLo >> 24);
				result[5] = (byte)(m_unIvLo >> 16);
				result[6] = (byte)(m_unIvLo >> 8);
				result[7] = (byte)m_unIvLo;
  
				return result;
			}
		}

 
		
		
		
		
		
		
		
		
		
		
		public BlowfishCBC
			(byte[] key,
			byte[] iv) : base(key)
		{
			Iv = iv;
		}


		
		
		
		
		
		
		
		
		
		public override void Burn() 
		{
			base.Burn();
  
			m_unIvHi = m_unIvLo = 0;
		}


		
		
		
		
		
		
		
		
		
		
		
		
		
		
		public override void Encrypt
			(ref uint unHiRef,
			ref uint unLoRef)
		{
			unHiRef ^= m_unIvHi;
			unLoRef ^= m_unIvLo;

			BaseEncrypt(ref unHiRef, ref unLoRef);

			m_unIvHi = unHiRef;
			m_unIvLo = unLoRef;
		}


		
		
		
		
		
		
		
		
		
		
		
		
		
		
		public override void Decrypt
			(ref uint unHiRef,
			ref uint unLoRef)
		{
			uint unBakHi = unHiRef;  
			uint unBakLo = unLoRef;

			base.Decrypt(ref unHiRef, ref unLoRef);

			unHiRef ^= m_unIvHi;
			unLoRef ^= m_unIvLo;

			m_unIvHi = unBakHi;
			m_unIvLo = unBakLo;
		}
	}
}
