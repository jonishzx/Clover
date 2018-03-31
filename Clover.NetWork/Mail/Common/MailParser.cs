using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;
using Clover.Net.Command;

namespace Clover.Net.Mail
{
    
    
    
    
    public class MailParser
    {
        private class RegexList
        {
            public static readonly Regex HexDecoder = new Regex("((\\=([0-9A-F][0-9A-F]))*)", RegexOptions.IgnoreCase);
            public static readonly Regex HexDecoder1 = new Regex("((%([0-9A-F][0-9A-F]))*)", RegexOptions.IgnoreCase);
            public static readonly Regex IsResponseOk = new Regex(@"^.*\+OK.*$", RegexOptions.IgnoreCase);
            public static readonly Regex DecodeByRfc2047 = new Regex(@"[\s]{0,1}[=][\?](?<Encoding>[^?]+)[\?](?<BorQ>[B|b|Q|q])[\?](?<Value>[^?]+)[\?][=][\s]{0,1}");
            public static readonly Regex DecodeByRfc2231 = new Regex(@"(?<Encoding>[^']+)[\'](?<Language>[a-zA-z\-]*)[\'](?<Value>[^\s]+)");
            public static readonly Regex IsReceiveCompleted = new Regex(String.Format(@"{0}\.{0}", MailParser.NewLine));
			public static readonly Regex AsciiCharOnly = new Regex("[^\x00-\x7F]");
            public static readonly ICollection<Regex> ContentTypeBoundary = new List<Regex>();
            public static readonly ICollection<Regex> ContentTypeName = new List<Regex>();
            public static readonly ICollection<Regex> ContentDispositionFileName = new List<Regex>();
            public static readonly String Rfc2231FormatText = @"[;\t\s]+{0}\*{1}=(?<Value>[^\n\r]+).*";
            public static readonly String Rfc2231FormatText1 = @"[;\t\s]+{0}\*{1}\*=(?<Value>[^\n\r]+).*";
            static RegexList()
            {
                InitializeRegexList();
            }
            private static void InitializeRegexList()
            {
                RegexList.ContentTypeBoundary.Add(new Regex(".*boundary=[\"]*(?<Value>[^\"]*).*", RegexOptions.IgnoreCase));
                RegexList.ContentTypeName.Add(new Regex(".*name=[\"]*(?<Value>[^\"]*)[;\n\r]", RegexOptions.IgnoreCase));
                RegexList.ContentTypeName.Add(new Regex(".*name=[\"]*(?<Value>[^\"]*).*", RegexOptions.IgnoreCase));
                RegexList.ContentTypeName.Add(new Regex(@"[;\t\s]+name\*=(?<Value>[^\n\r]+).*", RegexOptions.IgnoreCase));
                RegexList.ContentDispositionFileName.Add(new Regex("[;\t\\s]+filename=[\"]*(?<Value>[^\"]*)[;\n\r]", RegexOptions.IgnoreCase));
                RegexList.ContentDispositionFileName.Add(new Regex("[;\t\\s]+filename=[\"]*(?<Value>[^\"]*).*", RegexOptions.IgnoreCase));
                RegexList.ContentDispositionFileName.Add(new Regex(@"[;\t\s]+filename\*=(?<Value>[^\n\r]+).*", RegexOptions.IgnoreCase));
            }
        }
        private static TimeSpan _TimeZoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        private static String _DateTimeFormatString = "ddd, dd MMM yyyy HH:mm:ss +0000";
        
        
        
        
        public const String ThisIsMultiPartMessageInMIMEFormat = "This is multi-part message in MIME format.";
        
        
        
        
        public const String NewLine = "\r\n";
        
        
        
        
        public static String DateTimeFormatString
        {
            get { return MailParser._DateTimeFormatString; }
        }
        
        
        
        
        
        public static TimeSpan TimeZoneOffset
        {
            get { return MailParser._TimeZoneOffset; }
            set
            {
                MailParser._TimeZoneOffset = value;
                MailParser.SetDateTimeFormatString();
            }
        }
        
        
        
        
        public const Int32 MaxCharCountPerRow = 76;
        static MailParser()
        {
            MailParser.SetDateTimeFormatString();
        }
        private static void SetDateTimeFormatString()
        {
            MailParser._DateTimeFormatString = String.Format("ddd, dd MMM yyyy HH:mm:ss +{0:00}{1:00}"
                , MailParser._TimeZoneOffset.Hours, MailParser._TimeZoneOffset.Minutes);
        }
        
        
        
        
        
        
        public static Boolean IsResponseOk(String text)
        {
			return RegexList.IsResponseOk.IsMatch(text);
        }
        
        
        
        
        
        
        public static String MailAddress(String from)
        {
            Regex rg = new Regex("[<]{1}(?<MailAddress>[^>]+)[>]{1}");
            Match m = null;

            m = rg.Match(from);
            if (String.IsNullOrEmpty(m.Value) == true)
            {
                return from;
            }
            return m.Groups["MailAddress"].Value;
        }
        
        
        
        
        
        
        public static String Date(DateTime dateTime)
        {
            return dateTime.ToString(MailParser.DateTimeFormatString, new CultureInfo("en-US"));
        }
        
        
        
        
        
        
        public static TransferEncoding ToTransferEncoding(String text)
        {
            switch (text.ToLower())
            {
                case "7bit": return TransferEncoding.SevenBit;
                case "base64": return TransferEncoding.Base64;
                case "quoted-printable": return TransferEncoding.QuotedPrintable;
            }
            return TransferEncoding.SevenBit;
        }
        
        
        
        
        
        
        public static String ToTransferEncoding(TransferEncoding encoding)
        {
            switch (encoding)
            {
                case TransferEncoding.SevenBit: return "7bit";
                case TransferEncoding.Base64: return "Base64";
                case TransferEncoding.QuotedPrintable: return "Quoted-Printable";
            }
            return "7bit";
        }
        
        
        
        
        
        
        
        
        public static String EncodeToMailHeaderLine(String text, TransferEncoding encodeType, Encoding encoding)
        {
            return MailParser.EncodeToMailHeaderLine(text, encodeType, encoding, MailParser.MaxCharCountPerRow);
        }
        
        
        
        
        
        
        
        
        
        public static String EncodeToMailHeaderLine(String text, TransferEncoding encodeType, Encoding encoding, Int32 maxCharCount)
        {
            Byte[] bb = null;
            StringBuilder sb = new StringBuilder();
            Int32 StartIndex = 0;
            Int32 CharCountPerRow = 0;
            Int32 ByteCount = 0;

            if (maxCharCount > MailParser.MaxCharCountPerRow)
            { throw new ArgumentException("maxCharCount must less than MailParser.MaxCharCountPerRow."); }

            if (String.IsNullOrEmpty(text) == true)
            { return ""; }

            if (MailParser.AsciiCharOnly(text) == true)
            {
                StartIndex = 0;
                CharCountPerRow = maxCharCount;
                for (int i = 0; i < text.Length; i++)
                {
                    sb.Append(text[i]);
                    if (StartIndex == CharCountPerRow)
                    {
                        sb.Append(MailParser.NewLine);
                        StartIndex = 0;
                        CharCountPerRow = MailParser.MaxCharCountPerRow;
                        if (i < text.Length - 1)
                        {
                            sb.Append("\t");
                        }
                    }
                    else
                    {
                        StartIndex += 1;
                    }
                }
                return sb.ToString();
            }
            if (encodeType == TransferEncoding.Base64)
            {
                CharCountPerRow = (Int32)Math.Floor((maxCharCount - (encoding.WebName.Length + 10)) * 0.75);
                for (int i = 0; i < text.Length; i++)
                {
                    ByteCount = encoding.GetByteCount(text.Substring(StartIndex, (i + 1) - StartIndex));
                    if (ByteCount > CharCountPerRow)
                    {
                        bb = encoding.GetBytes(text.Substring(StartIndex, i - StartIndex));
                        sb.AppendFormat("=?{0}?B?{1}?={2}\t", encoding.WebName, Convert.ToBase64String(bb), MailParser.NewLine);
                        StartIndex = i;
                        CharCountPerRow = (Int32)Math.Floor((MailParser.MaxCharCountPerRow - (encoding.WebName.Length + 10)) * 0.75);
                    }
                }
                bb = encoding.GetBytes(text.Substring(StartIndex));
                sb.AppendFormat("=?{0}?B?{1}?=", encoding.WebName, Convert.ToBase64String(bb));

                return sb.ToString();
            }
            else if (encodeType == TransferEncoding.QuotedPrintable)
            {
                CharCountPerRow = (Int32)Math.Floor((maxCharCount - (Double)(encoding.WebName.Length + 10)) / 3);
                for (int i = 0; i < text.Length; i++)
                {
                    ByteCount = encoding.GetByteCount(text.Substring(StartIndex, (i + 1) - StartIndex));
                    if (ByteCount > CharCountPerRow)
                    {
                        bb = encoding.GetBytes(text.Substring(StartIndex, i - StartIndex));
                        sb.AppendFormat("=?{0}?Q?{1}?={2}\t", encoding.WebName, MailParser.ToQuotedPrintableOnHeader(Encoding.ASCII.GetString(bb)), MailParser.NewLine);
                        StartIndex = i;
                        CharCountPerRow = (Int32)Math.Floor((MailParser.MaxCharCountPerRow - (encoding.WebName.Length + 10)) * 0.75);
                    }
                }
                bb = encoding.GetBytes(text.Substring(StartIndex));
                sb.AppendFormat("=?{0}?Q?{1}?=", encoding.WebName, MailParser.ToQuotedPrintable(Encoding.ASCII.GetString(bb)));

                return sb.ToString();
            }
            else
            {
                return text;
            }
        }
        
        
        
        
        
        
        
        
        
        public static String EncodeToMailHeaderLineByRfc2231(String parameterName, String text, Encoding encoding, Int32 maxCharCount)
        {
            Byte[] bb = null;
            StringBuilder sb = new StringBuilder();
            Int32 StartIndex = 0;
            Int32 CharCountPerRow = 0;
            Int32 RowNo = 0;

            CharCountPerRow = MailParser.MaxCharCountPerRow - parameterName.Length - 3;
            bb = encoding.GetBytes(text);
            for (int i = 0; i < bb.Length; i++)
            {
                
                if (0x30 <= bb[i] && bb[i] <= 0x39)
                {
                    sb.Append((Char)bb[i]);
                }
                else if (0x41 <= bb[i] && bb[i] <= 0x5a)
                {
                    sb.Append((Char)bb[i]);
                }
                else if (0x61 <= bb[i] && bb[i] <= 0x7a)
                {
                    sb.Append((Char)bb[i]);
                }
                else
                {
                    sb.Append("%");
                    sb.Append(bb[i].ToString("X2"));
                }
            }

            if (sb.Length > CharCountPerRow)
            {
                String s = sb.ToString();
                sb.Length = 0;
                while (true)
                {
                    if (RowNo > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(parameterName);
                    sb.Append("*");
                    sb.Append(RowNo);
                    sb.Append("*=");
                    if (RowNo == 0)
                    {
                        sb.Append(encoding.WebName);
                        sb.Append("''");
                    }
                    if (StartIndex + CharCountPerRow < s.Length)
                    {
                        sb.Append(s.Substring(StartIndex, CharCountPerRow));
                        sb.Append(MailParser.NewLine);
                    }
                    else
                    {
                        sb.Append(s.Substring(StartIndex, s.Length - StartIndex));
                        sb.Append(";");
                        break;
                    }
                    RowNo += 1;
                    StartIndex += CharCountPerRow;
                }
                return sb.ToString();
            }
            else
            {
                return String.Format("{0}*={1}''{2}", parameterName, encoding.WebName, sb.ToString());
            }
        }
        
        
        
        
        
        
        public static String DecodeFromMailHeaderLine(String line)
        {
			Regex rg = RegexList.DecodeByRfc2047;
            MatchCollection mc = null;
            Match m = null;
            Byte[] bb = null;
			Encoding en = null;
			Int32 StartIndex = 0;
			StringBuilder sb = new StringBuilder();

            if (String.IsNullOrEmpty(line) == true) { return ""; }

            m = RegexList.DecodeByRfc2231.Match(line);
            mc = rg.Matches(line);
            if (m.Success == true && mc.Count == 0)
            {
                en = MailParser.GetEncoding(m.Groups["Encoding"].Value);
                sb.Append(MailParser.DecodeFromMailHeaderLineByRfc2231(m.Groups["Value"].Value, en));
            }
            else
            {
                for (int i = 0; i < mc.Count; i++)
                {
                    m = mc[i];
                    sb.Append(line.Substring(StartIndex, m.Index - StartIndex));
                    StartIndex = m.Index + m.Length;

                    if (m.Groups.Count < 3)
                    {
                        throw new InvalidDataException();
                    }
                    if (m.Groups["BorQ"].Value.ToUpper() == "B")
                    {
                        bb = Convert.FromBase64String(m.Groups["Value"].Value);
                    }
                    else if (m.Groups["BorQ"].Value.ToUpper() == "Q")
                    {
                        bb = MailParser.FromQuotedPrintableTextOnHeader(m.Groups["Value"].Value);
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }
                    en = MailParser.GetEncoding(m.Groups["Encoding"].Value);
                    sb.Append(en.GetString(bb));
                }
                sb.Append(line.Substring(StartIndex, line.Length - StartIndex));
            }
			return sb.ToString();
        }
        
        
        
        
        
        
        
        public static String DecodeFromMailHeaderLineByRfc2231(String text, Encoding encoding)
        {
            Int32 CurrentIndex = 0;
            Byte[] bb = new Byte[text.Length];
            Int32 ByteArrayIndex = 0;
            Boolean IsDigitChar = false;
            String HexChar = "";

            while (true)
            {
                
                if (CurrentIndex < text.Length - 3 &&
                    text[CurrentIndex] == '%')
                {
                    HexChar = text.Substring(CurrentIndex + 1, 2);
                    IsDigitChar = RegexList.HexDecoder1.IsMatch(HexChar);
                }
                else
                {
                    IsDigitChar = false;
                }

                if (IsDigitChar == true)
                {
                    bb[ByteArrayIndex] = Convert.ToByte(HexChar, 16);
                    CurrentIndex += 3;
                }
                else
                {
                    bb[ByteArrayIndex] = (Byte)Char.Parse(text.Substring(CurrentIndex, 1));
                    CurrentIndex += 1;
                }
                ByteArrayIndex += 1;
                if (CurrentIndex >= text.Length) { break; }
            }
            
            Byte[] bb2 = new Byte[ByteArrayIndex];
            Array.Copy(bb, 0, bb2, 0, ByteArrayIndex);
            return encoding.GetString(bb2);
        }
        
        
        
        
        
        
        
        public static void ParseContentType(ContentType contentType, String line)
        {
            Match m = null;

            
            foreach (Regex rx in MailParser.RegexList.ContentTypeName)
            {
                m = rx.Match(line);
                if (String.IsNullOrEmpty(m.Groups["Value"].Value) == false)
                {
                    contentType.Name = m.Groups["Value"].Value;
                    break;
                }
            }
            if (String.IsNullOrEmpty(contentType.Name) == true)
            {
                contentType.Name = MailParser.ParseHeaderParameterValue("name", line);
            }

            
            foreach (Regex rx in MailParser.RegexList.ContentTypeBoundary)
            {
                m = rx.Match(line);
                if (String.IsNullOrEmpty(m.Groups["Value"].Value) == false)
                {
                    contentType.Boundary = m.Groups["Value"].Value;
                    break;
                }
            }
            if (String.IsNullOrEmpty(contentType.Boundary) == true)
            {
                contentType.Boundary = MailParser.ParseHeaderParameterValue("boundary", line);
            }
        }
        
        
        
        
        
        
        
        public static void ParseContentDisposition(ContentDisposition contentDisposition, String line)
        {
            Match m = null;

            
            foreach (Regex rx in MailParser.RegexList.ContentDispositionFileName)
            {
                m = rx.Match(line);
                if (String.IsNullOrEmpty(m.Groups["Value"].Value) == false)
                {
                    contentDisposition.FileName = m.Groups["Value"].Value;
                    return;
                }
            }
            contentDisposition.FileName = MailParser.ParseHeaderParameterValue("filename", line);
        }
        private static String ParseHeaderParameterValue(String parameterName, String line)
        {
            Match m = null;
            Int32 RowNo = 0;
            StringBuilder sb = new StringBuilder();

            List<String> l = new List<String>();
            l.Add(MailParser.RegexList.Rfc2231FormatText);
            l.Add(MailParser.RegexList.Rfc2231FormatText1);

            for (int i = 0; i < l.Count; i++)
            {
                while (true)
                {
                    var rx = new Regex(String.Format(l[i], parameterName, RowNo), RegexOptions.IgnoreCase);
                    m = rx.Match(line);
                    if (String.IsNullOrEmpty(m.Groups["Value"].Value) == true)
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(m.Groups["Value"].Value);
                    }
                    RowNo += 1;
                }
            }
            return sb.ToString();
        }
        
        
        
        
        
        
        
        
        public static String EncodeToMailBody(String text, TransferEncoding encodeType, Encoding encoding)
        {
            Byte[] bb = encoding.GetBytes(text);
			if (encodeType == TransferEncoding.Base64)
            {
				return Convert.ToBase64String(bb);
            }
			else if (encodeType == TransferEncoding.QuotedPrintable)
			{
				return MailParser.ToQuotedPrintable(encoding.GetString(bb));
			}
			return encoding.GetString(bb);
        }
        
        
        
        
        
        
        
        
        public static String DecodeFromMailBody(String text, TransferEncoding encodeType, Encoding encoding)
        {
            Byte[] b = null;

            if (encodeType == TransferEncoding.Base64)
            {
                b = Convert.FromBase64String(text);
			}
			else if (encodeType == TransferEncoding.QuotedPrintable)
			{
				b = MailParser.FromQuotedPrintableText(text);
			}
			else
			{
				b = encoding.GetBytes(text);
			}
			return encoding.GetString(b);
		}
        
        
        
        
        
        public static string GenerateBoundary()
        {
            String s = String.Format("NextPart_{0}", Guid.NewGuid().ToString("D"));
            return s;
        }
		
		
		
		
		
		
		public static String ToQuotedPrintableOnHeader(String text)
		{
			StringReader sr = new StringReader(text);
			StringBuilder sb = new StringBuilder();
			Int32 i;

			while ((i = sr.Read()) > 0)
			{
				
				if (32 < i && i < 127)
				{
					
					if (i == 32 ||
						i == 61 ||
						i == 63 ||
						i == 95)
					{
						sb.AppendFormat("=", Convert.ToString(i, 16).ToUpper());
					}
					else
					{
						sb.Append(Convert.ToChar(i));
					}
				}
				else
				{
					sb.AppendFormat("=", Convert.ToString(i, 16).ToUpper());
				}
			}
			return sb.ToString();
		}
		
        
        
        
        
        
        public static String ToQuotedPrintable(String text)
        {
            StringReader sr = new StringReader(text);
            StringBuilder sb = new StringBuilder();
            Int32 i;

            while ((i = sr.Read()) > 0)
            {
				
                if (i == 61)
				{
					sb.AppendFormat("=", Convert.ToString(i, 16).ToUpper());
				}
				
				else if ((32 < i && i < 127) ||
                    i == 13 ||
                    i == 10 ||
                    i == 9 ||
                    i == 32)
                {
                    sb.Append(Convert.ToChar(i));
                }
                else
                {
					sb.AppendFormat("=", Convert.ToString(i, 16).ToUpper());
				}
            }
            return sb.ToString();
        }
		
		
		
		
		
		
		public static Byte[] FromQuotedPrintableTextOnHeader(String text)
		{
			if (text == null)
			{ throw new ArgumentNullException(); }

			MemoryStream ms = new MemoryStream();
			String line;
			Boolean AddNewLine = false;
			Int32 i = 0;

			using (StringReader sr = new StringReader(text))
			{
				while ((line = sr.ReadLine()) != null)
				{
					
					if (line.EndsWith("="))
					{
						
						line = line.Substring(0, line.Length - 1);
						AddNewLine = false;
					}
					else
					{
						AddNewLine = true;
					}
					i = 0;
					while (i < line.Length)
					{
						
						if (line.Substring(i, 1) == "=")
						{
							
							Int32 charLen = i == (line.Length - 2) ? 1 : 2;
							String target = line.Substring(i + 1, charLen);
							ms.WriteByte(Convert.ToByte(target, 16));
							i += 3;
						}
						
						else if (line.Substring(i, 1) == "_")
						{

							ms.WriteByte(Convert.ToByte(' '));
							i = i + 1;
						}
						
						else
						{
							String target = line.Substring(i, 1);
							ms.WriteByte(Convert.ToByte(Char.Parse(target)));
							i = i + 1;
						}
					}
					
					if (AddNewLine == true)
					{
						ms.WriteByte(13);
						ms.WriteByte(10);
					}
				}

			}
			return ms.ToArray();
		}
		
        
        
        
        
        
        public static Byte[] FromQuotedPrintableText(String text)
        {
            if (text == null)
            { throw new ArgumentNullException(); }

            MemoryStream ms = new MemoryStream();
            String line;
            Boolean AddNewLine = false;
            Int32 i = 0;

            using (StringReader sr = new StringReader(text))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    
                    if (line.EndsWith("="))
                    {
                        
                        line = line.Substring(0, line.Length - 1);
                        AddNewLine = false;
                    }
                    else
                    {
                        AddNewLine = true;
                    }
                    i = 0;
                    while (i < line.Length)
                    {
                        
                        if (line.Substring(i, 1) == "=")
                        {
                            
                            Int32 charLen = i == (line.Length - 2) ? 1 : 2; 
                            String target = line.Substring(i + 1, charLen);
                            ms.WriteByte(Convert.ToByte(target, 16));
                            i += 3;
                        }
                        
                        else
                        {
                            String target = line.Substring(i, 1);
                            ms.WriteByte(Convert.ToByte(Char.Parse(target)));
                            i = i + 1;
                        }
                    }
                    
                    if (AddNewLine == true)
                    {
                        ms.WriteByte(13);
                        ms.WriteByte(10);
                    }
                }

            }
            return ms.ToArray();
        }
        
        
        
        
        
        
        public static String ToBase64String(String text)
        {
            Byte[] b = null;
            b = Encoding.ASCII.GetBytes(text);
            return Convert.ToBase64String(b, 0, b.Length);
        }
		
		
		
		
		
		
		public static String FromBase64String(String text)
		{
			Byte[] b = null;
			b = Convert.FromBase64String(text);
			return Encoding.ASCII.GetString(b);
		}
		
        
        
        
        
        
        public static String ToMd5DigestString(String text)
        {
            Byte[] bb = null;
            StringBuilder sb = new StringBuilder();

            bb = Encoding.Default.GetBytes(text);
            MD5 md5 = new MD5CryptoServiceProvider();
            bb = md5.ComputeHash(bb);
            for (int i = 0; i < bb.Length; i++)
            {
                sb.Append(bb[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
		
        
        
        
        
        
        
        
        public static String ToCramMd5String(String challenge, String userName, String password)
        {
            StringBuilder sb = new StringBuilder();
            Byte[] bb = null;
            HMACMD5 md5 = new HMACMD5(Encoding.ASCII.GetBytes(password));
            
            bb = md5.ComputeHash(Convert.FromBase64String(challenge));
            
            for (int i = 0; i < bb.Length; i++)
            {
                sb.Append(bb[i].ToString("x02"));
            }
            
            bb = Encoding.ASCII.GetBytes(String.Format("{0} {1}", userName, sb.ToString()));
            return Convert.ToBase64String(bb);
        }
        
        
        
        
        
        
        public static Boolean AsciiCharOnly(String text)
        {
			return !RegexList.AsciiCharOnly.IsMatch(text);
        }
		private static Encoding GetEncoding(String encoding)
		{
			if (String.Equals(encoding, "UTF7", StringComparison.InvariantCultureIgnoreCase) == true)
			{
				return Encoding.UTF7;
			}
			else if (String.Equals(encoding, "UTF8", StringComparison.InvariantCultureIgnoreCase) == true)
			{
				return Encoding.UTF8;
			}
			else if (String.Equals(encoding, "UTF32", StringComparison.InvariantCultureIgnoreCase) == true)
			{
				return Encoding.UTF32;
			}
			return Encoding.GetEncoding(encoding);
		}
    }
}
