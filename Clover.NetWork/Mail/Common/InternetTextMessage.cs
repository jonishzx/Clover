using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using Clover.Net.Mail;

namespace Clover.Net
{
    
    
    
    
	public class InternetTextMessage 
	{
		private static readonly ICollection<Regex> ContentEncodingCharset = new List<Regex>();
		private static readonly Regex HeaderParseRegex = new Regex("^(?<key>[^:]*):[\\s]*(?<value>.*)");
        private static readonly Regex HeaderParseRegex1 = new Regex("(?<value>[^;]*)[;]*");
        private List<Field> _Header;
		
		
		
		
		
		private String _HeaderData = "";
		
        
        
        
        
        private String _BodyData = "";
        private Boolean _DecodeHeaderText = true;
        private Encoding _ContentEncoding = Encoding.Default;
		
		
		
		
		
		public String this[String key]
		{
			get 
			{
				Field f = InternetTextMessage.Field.FindField(this._Header, key);
				if (f == null)
				{
					return "";
				}
				else
				{
                    if (this._DecodeHeaderText == true)
                    {
                        return MailParser.DecodeFromMailHeaderLine(f.Value);
                    }
                    else
                    {
                        return f.Value;
                    }
				}
			}
			set 
			{
				Field f = InternetTextMessage.Field.FindField(this._Header, key);
				if (f == null)
				{
					f = new Field(key, value);
					this._Header.Add(f);
				}
				else
				{
					f.Value = value;
				}
			}
		}
        
        
        
        
        
        public String From
        {
            get { return this["From"]; }
            set { this["From"] = value; }
        }
        
        
        
        
        public String ReplyTo
        {
            get { return this["Reply-To"]; }
            set { this["Reply-To"] = value; }
        }
        
        
        
        
        public String InReplyTo
        {
            get { return this["In-Reply-To"]; }
            set { this["In-Reply-To"] = value; }
        }
        
        
        
        
        public String Subject
        {
            get { return this["Subject"]; }
            set { this["Subject"] = value; }
        }
        
        
        
        
        public DateTime Date
        {
            get
            {
                DateTime dtime = DateTime.Now;
                String dateAsString = this["Date"];
                if (DateTime.TryParse(dateAsString, out dtime) == false)
                {
                    dtime = RFC2822DateTime.Parse(dateAsString);
                }
                return dtime;
            }
            set { this["Date"] = MailParser.Date(value); }
        }
        
        
        
        
        public String MessageID
        {
            get { return this["Message-ID"]; }
            set { this["Message-ID"] = value; }
        }
        
        
        
        
        public String References
        {
            get { return this["References"]; }
            set { this["References"] = value; }
        }
        
        
        
        
        public ContentType ContentType
        {
            get 
            {
                ContentType ff = null;
                ff = InternetTextMessage.Field.FindField(this._Header, "content-type") as ContentType;
                if (ff == null)
                {
                    ff = new ContentType("text/plain");
                    this._Header.Add(ff);
                }
                return ff;
            }
        }
        
        
        
        
        public Encoding ContentEncoding
        {
            get { return this._ContentEncoding; }
            set { this._ContentEncoding = value; }
        }
        
        
        
        
        public ContentDisposition ContentDisposition
        {
            get
            {
                ContentDisposition ff = null;
                ff = InternetTextMessage.Field.FindField(this._Header, "content-disposition") as ContentDisposition;
                if (ff == null)
                {
                    ff = new ContentDisposition("");
                    this._Header.Add(ff);
                }
                return ff;
            }
        }
        
        
        
        
        public String MultiPartBoundary
        {
            get { return this.ContentType.Boundary; }
            set { this.ContentType.Boundary = value; }
        }
        
        
        
        
        public Boolean IsMultiPart
        {
            get { return Regex.IsMatch(this.ContentType.Value, ".*multipart/.*", RegexOptions.IgnoreCase); }
        }
        
        
        
        
        public Boolean IsBody
        {
            get
            {
                return (this.ContentType.Value.StartsWith("text/", StringComparison.OrdinalIgnoreCase) == true);
            }
        }
        
        
        
        
        public Boolean IsText
        {
            get
            {
                return (this.ContentType.Value.StartsWith("text/", StringComparison.OrdinalIgnoreCase) == true) ||
                    (this.ContentType.Value.Equals("application/xml", StringComparison.OrdinalIgnoreCase) == true) ||
                    (this.ContentType.Value.Equals("application/json", StringComparison.OrdinalIgnoreCase) == true);
            }
        }
        
        
        
        
        public Boolean IsHtml
        {
            get
            {
                return (this.ContentType.Value.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) == true);
            }
        }
        
        
        
        
        public Boolean IsAttachment
        {
            get
            {
                if (String.IsNullOrEmpty(this.ContentDisposition.Value) == false)
                {
                    return Regex.Match(this.ContentDisposition.Value, "^attachment.*$").Success;
                }
                return false;
            }
        }
        
        
        
        
        public String ContentDescription
        {
            get { return this["Content-Description"]; }
            set { this["Content-Description"] = value; }
        }
        
        
        
        
        public TransferEncoding ContentTransferEncoding
        {
            get { return MailParser.ToTransferEncoding(this["Content-Transfer-Encoding"]); }
            set { this["Content-Transfer-Encoding"] = MailParser.ToTransferEncoding(value); }
        }
        
        
        
        
        public String CharSet
        {
            get { return this.ContentEncoding.HeaderName; }
        }
        
        
        
        
        public List<Field> Header
        {
            get { return this._Header; }
        }
        
        
        
        
        public Boolean DecodeHeaderText
        {
            get { return this._DecodeHeaderText; }
            set { this._DecodeHeaderText = value; }
        }
		
		
		
		
		protected String HeaderData
		{
			get { return this._HeaderData; }
			set { this._HeaderData = value; }
		}
		
        
        
        
        protected String BodyData
        {
            get { return this._BodyData; }
            set { this._BodyData = value; }
        }
		static InternetTextMessage()
		{
			InternetTextMessage.Initialize();
		}
		private static void Initialize()
		{
			InternetTextMessage.ContentEncodingCharset.Add(new Regex(".*charset ?= ?[\"]*(?<Value>[^\";]*)[;\n\r]", RegexOptions.IgnoreCase));
			InternetTextMessage.ContentEncodingCharset.Add(new Regex(".*charset ?= ?[\"]*(?<Value>[^\"]*).*", RegexOptions.IgnoreCase));
		}
		
		
		
        public InternetTextMessage()
        {
            this.Initialize("");
        }
		
		
		
		
        public InternetTextMessage(String text)
        {
            this.Initialize(text);
        }
        
        
        
        
        
        private void Initialize(String text)
		{
			this._Header = new List<Field>();
            
            this.Date = DateTime.Now;
            this._Header.Add(new Field("From", ""));
            this._Header.Add(new Field("Subject", ""));
            this.ContentType.Value = "text/plain";
            this.ContentTransferEncoding = TransferEncoding.SevenBit;
            this.ContentDisposition.Value = "inline";
            this.SetDefaultContentEncoding();

            this.Parse(text);
        }
        
        
        
        
        private void SetDefaultContentEncoding()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja")
            {
                this.ContentEncoding = Encoding.GetEncoding("iso-2022-jp");
            }
        }
        
        
        
        
        
        protected void Parse(String text)
        {
            StringReader sr = null;
            
            List<String> l = new List<String>();
            String CurrentLine = "";
            String FirstLine = "";
            Boolean IsConcating = false;
            Int32 c = 0;
			StringBuilder sb = new StringBuilder(512);

            using (sr = new StringReader(text))
            {
                while (true)
                {
                    CurrentLine = sr.ReadLine();
					sb.Append(CurrentLine);
					sb.Append(MailParser.NewLine);
					if (IsConcating == true)
                    {
                        l.Add(CurrentLine);
                    }
                    else
                    {
                        l.Clear();
                        FirstLine = CurrentLine;
                        
                        if (FirstLine == "")
                        {
							_HeaderData = sb.ToString();
							
							sb = new StringBuilder(text.Length - _HeaderData.Length);
                            while (true)
                            {
                                CurrentLine = sr.ReadLine();
                                if (CurrentLine == null) { break; }
                                if (CurrentLine == ".") { break; }
                                if (CurrentLine.StartsWith("..") == true)
                                { CurrentLine = CurrentLine.Substring(1, CurrentLine.Length - 1); }
                                sb.Append(CurrentLine);
                                if (sr.Peek() == -1) { break; }
                                
                                
                                sb.Append(MailParser.NewLine);
                            }
                            this.BodyData = sb.ToString();
                            return;
                        }
                    }
                    
                    c = sr.Peek();
                    
					if (c == -1)
					{
						_HeaderData = sb.ToString();
						break;
					}
                    
                    if (c == 9 || c == 32)
                    {
                        IsConcating = true;
                        continue;
                    }
                    else
                    {
                        IsConcating = false;
                        this.ParseHeaderField(FirstLine, l);
                        l.Clear();
                        IsConcating = false;
                    }
                }
            }
        }
        
        
        
        
        
        
        
        private void ParseHeaderField(String line, List<String> lines)
        {
            Match m = HeaderParseRegex.Match(line);
            Match m1 = null;
            Regex rx = HeaderParseRegex1;
            Field f = null;
            List<String> l = lines;
			Int32 size = 0;
			for (int i = 0; i < lines.Count; i++)
			{
				size += line.Length;
			}
            StringBuilder sb = new StringBuilder(size);

            if (String.IsNullOrEmpty(m.Groups["key"].Value) == false)
            {
                m1 = rx.Match(m.Groups["value"].Value);
                if (m.Groups["key"].Value.ToLower() == "content-type" ||
                    m.Groups["key"].Value.ToLower() == "content-disposition")
                {
                    sb.Append(line);
                    for (int i = 0; i < l.Count; i++)
                    {
                        sb.Append(l[i].TrimStart('\t'));
                    }
                    this.ParseContentEncoding(sb.ToString());

                    if (m.Groups["key"].Value.ToLower() == "content-type")
                    {
                        MailParser.ParseContentType(this.ContentType, sb.ToString());
                        this.ContentType.Value = m1.Groups["value"].Value;
                    }
                    else if (m.Groups["key"].Value.ToLower() == "content-disposition")
                    {
                        MailParser.ParseContentDisposition(this.ContentDisposition, sb.ToString());
                        this.ContentDisposition.Value = m1.Groups["value"].Value;
                    }
                }
                else
                {
                    f = Field.FindField(this._Header, m.Groups["key"].Value);
                    if (f == null)
                    {
                        f = new Field(m.Groups["key"].Value, m.Groups["value"].Value);
                        this.Header.Add(f);
                    }
                    else
                    {
                        f.Value = m.Groups["value"].Value;
                    }
                    for (int i = 0; i < l.Count; i++)
                    {
                        f.Value += l[i].TrimStart('\t');
                    }
                }
            }
        }
        
        
        
        
        
        private void ParseContentEncoding(String line)
        {
            Match m = null;

            
            foreach (Regex rx in InternetTextMessage.ContentEncodingCharset)
            {
                m = rx.Match(line);
                if (String.IsNullOrEmpty(m.Groups["Value"].Value) == false)
                {
                    this._ContentEncoding = this.GetEncoding(m.Groups["Value"].Value, this.ContentEncoding);
                    break;
                }
            }
        }
        
        
        
        
        
        
        
        private Encoding GetEncoding(String name, Encoding defaultEncoding)
        {
            Encoding en = null;
            try
            {
                en = Encoding.GetEncoding(name);
            }
            catch
            {
                en = defaultEncoding;
            }
            return en;
        }
        
        
        
        
        
        
        public void DecodeData(String filePath)
        {
            Byte[] bb = null;

            if (String.IsNullOrEmpty(this.ContentDisposition.Value) == true)
            { return; }

            if (this.ContentTransferEncoding == TransferEncoding.Base64)
            {
                bb = Convert.FromBase64String(this.BodyData.Replace("\n", "").Replace("\r", ""));
                using (BinaryWriter sw = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
                {
                    sw.Write(bb);
                    sw.Flush();
                    sw.Close();
                }
            }
            else if (this.ContentTransferEncoding == TransferEncoding.QuotedPrintable)
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(this.ContentEncoding.GetString(MailParser.FromQuotedPrintableText(this.BodyData)));
                    sw.Flush();
                    sw.Close();
                }
            }
            else if (this.ContentTransferEncoding == TransferEncoding.SevenBit)
            {
                bb = Encoding.ASCII.GetBytes(this.BodyData);
                using (BinaryWriter sw = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
                {
                    sw.Write(bb);
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        
        
        
        
        
        
        
        public void DecodeData(Stream stream, Boolean isClose)
        {
            Byte[] bb = null;

            if (this.ContentTransferEncoding == TransferEncoding.Base64)
            {
                bb = Convert.FromBase64String(this.BodyData.Replace("\n", ""));
                BinaryWriter sw = null;
                try
                {
                    sw = new BinaryWriter(stream);
                    sw.Write(bb);
                    sw.Flush();
                }
                finally
                {
                    if (isClose == true)
                    {
                        sw.Close();
                    }
                }
            }
            else if (this.ContentTransferEncoding == TransferEncoding.QuotedPrintable)
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(stream);
                    sw.Write(this.ContentEncoding.GetString(MailParser.FromQuotedPrintableText(this.BodyData)));
                    sw.Flush();
                }
                finally
                {
                    if (isClose == true)
                    {
                        sw.Close();
                    }
                }
            }
            else if (this.ContentTransferEncoding == TransferEncoding.SevenBit)
            {
                bb = Encoding.ASCII.GetBytes(this.BodyData);
                BinaryWriter sw = null;
                try
                {
                    sw = new BinaryWriter(stream);
                    sw.Write(bb);
                    sw.Flush();
                }
                finally
                {
                    if (isClose == true)
                    {
                        sw.Close();
                    }
                }
            }
        }
        
        
        
        
        
        public class Field
        {
            private String _Key = "";
            private String _Value = "";
			
			
			
            public String Key
            {
                get { return this._Key ?? ""; }
                set { this._Key = value; }
            }
			
			
			
            public String Value
            {
                get { return this._Value ?? ""; }
                set { this._Value = value; }
            }
			
			
			
			
			
            public Field(String key, String value)
            {
                this._Key = key;
                this._Value = value;
            }
			
			
			
			
			
			
            public static Field FindField(List<Field> fields, String key)
            {
                List<Field> l = fields.FindAll(delegate(Field f) { return String.Equals(f.Key, key, StringComparison.InvariantCultureIgnoreCase); });
                if (l.Count > 0)
                {
                    return l[l.Count - 1];
                }
                return null;
            }
			
			
			
			
            public override string ToString()
            {
                return String.Format("{0}: {1}", this.Key, this.Value);
            }
        }
	}
}
