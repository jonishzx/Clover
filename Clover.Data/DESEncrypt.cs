using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Clover.Data
{
    
    
    
    public class DESEncrypt
    {
        #region ========º”√‹======== 

        
        
        
        
        
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "litianping");
        }

        
        
        
        
        
        
        public static string Encrypt(string Text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key =
                Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5")
                    .Substring(0, 8));
            des.IV =
                Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5")
                    .Substring(0, 8));
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========Ω‚√‹======== 

        
        
        
        
        
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "litianping");
        }

        
        
        
        
        
        
        public static string Decrypt(string Text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length/2;
            var inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x*2, 2), 16);
                inputByteArray[x] = (byte) i;
            }
            des.Key =
                Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5")
                    .Substring(0, 8));
            des.IV =
                Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5")
                    .Substring(0, 8));
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion
    }
}