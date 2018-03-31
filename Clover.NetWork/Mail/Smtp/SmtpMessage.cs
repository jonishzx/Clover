using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Clover.Net.Mail;

namespace Clover.Net.Smtp
{
    
    
    
    
    public class SmtpMessage : InternetTextMessage
    {
        private List<SmtpContent> _Contents;
        private List<String> _EncodeHeaderKeys = new List<String>();
        private List<MailAddress> _To = new List<MailAddress>();
        private List<MailAddress> _Cc = new List<MailAddress>();
        private List<MailAddress> _Bcc = new List<MailAddress>();
        private String _BodyText = "";
        private Encoding _HeaderEncoding = Encoding.ASCII;
        private TransferEncoding _HeaderTransferEncoding = TransferEncoding.SevenBit;
        
        
        
        
        public List<MailAddress> To
        {
            get { return this._To; }
        }
        
        
        
        
        public List<MailAddress> Cc
        {
            get { return this._Cc; }
        }
        
        
        
        
        public List<MailAddress> Bcc
        {
            get { return this._Bcc; }
        }
        
        
        
        
        public Encoding HeaderEncoding
        {
            get { return this._HeaderEncoding; }
            set { this._HeaderEncoding = value; }
        }
        
        
        
        
        public TransferEncoding HeaderTransferEncoding
        {
            get { return this._HeaderTransferEncoding; }
            set { this._HeaderTransferEncoding = value; }
        }
        
        
        
        
        public String BodyText
        {
            get { return this._BodyText; }
            set { this._BodyText = value; }
        }
        
        
        
        
        public List<SmtpContent> Contents
        {
            get { return this._Contents; }
        }
		
		
		
        public SmtpMessage()
        {
            this.Initialize();
        }
		
		
		
		
		
		
		
		
        public SmtpMessage(String mailFrom, String to, String cc, String subject, String bodyText)
        {
            this.Initialize();
            this.From = mailFrom;
            this.To.Add(new MailAddress(to));
            this.Cc.Add(new MailAddress(cc));
            this.Subject = subject;
            this.BodyText = bodyText;
        }
        
        
        
        
        private void Initialize()
        {
            this._Contents = new List<SmtpContent>();
            if (CultureInfo.CurrentCulture.Name.StartsWith("ja") == true)
            {
                this.HeaderEncoding = Encoding.GetEncoding("iso-2022-jp");
                this.HeaderTransferEncoding = TransferEncoding.Base64;
                this.ContentEncoding = Encoding.GetEncoding("iso-2022-jp");
                this.ContentTransferEncoding = TransferEncoding.Base64;
            }
            this._EncodeHeaderKeys.Add("subject");
        }
        
        
        
        
        
        public String GetDataText()
        {
            StringBuilder sb = new StringBuilder();
            CultureInfo ci = CultureInfo.CurrentCulture;
            Field f = null;
            SmtpContent ct = null;
            String line = "";
            String bodyText = "";

            if (this.IsMultiPart == false &&
                this.Contents.Count > 0)
            {
                this.ContentType.Value = "multipart/mixed";
            }

            
            f = InternetTextMessage.Field.FindField(this.Header, "Content-Transfer-Encoding");
            if (f == null)
            {
                f = new Field("Content-Transfer-Encoding", MailParser.ToTransferEncoding(this.ContentTransferEncoding));
                this.Header.Add(f);
            }
            else
            {
                f.Value = MailParser.ToTransferEncoding(this.ContentTransferEncoding);
            }

            for (int i = 0; i < this.Header.Count; i++)
            {
                f = this.Header[i];
                if (this._EncodeHeaderKeys.Contains(f.Key.ToLower()) == true)
                {
                    sb.AppendFormat("{0}: {1}{2}", f.Key
                        , MailParser.EncodeToMailHeaderLine(f.Value, this.HeaderTransferEncoding, this.HeaderEncoding
                        , MailParser.MaxCharCountPerRow - f.Key.Length - 2), MailParser.NewLine);
                }
                else if(f.Key.ToLower() != "content-type")
                {
                    sb.AppendFormat("{0}: {1}{2}", f.Key, f.Value, MailParser.NewLine);
                }
            }
            
            f = Field.FindField(this.Header, "To");
            if (f == null)
            {
                line = this.CreateMailAddressListText(this._To);
                if (String.IsNullOrEmpty(line) == false)
                {
                    sb.Append("To: ");
                    sb.Append(line);
                }
            }
            
            f = Field.FindField(this.Header, "Cc");
            if (f == null)
            {
                line = this.CreateMailAddressListText(this._Cc);
                if (String.IsNullOrEmpty(line) == false)
                {
                    sb.Append("Cc: ");
                    sb.Append(line);
                }
            }

            if (this.IsMultiPart == true)
            {
                if (String.IsNullOrEmpty(this.MultiPartBoundary) == true)
                {
                    this.MultiPartBoundary = MailParser.GenerateBoundary();
                }
                
                if (String.IsNullOrEmpty(this.BodyText) == false)
                {
                    ct = new SmtpContent();
                    ct.BodyText = this.BodyText;
                    ct.ContentEncoding = this.ContentEncoding;
                    ct.ContentTransferEncoding = this.ContentTransferEncoding;
                    if (this.Contents.Exists(delegate(SmtpContent c) { return c.IsBody; }) == false)
                    {
                        this.Contents.Insert(0, ct);
                    }
                }

                
                sb.AppendFormat("Content-Type: {0}; boundary=\"{1}\"", this.ContentType.Value, this.MultiPartBoundary);
                sb.Append(MailParser.NewLine);
                sb.Append(MailParser.NewLine);

                
                sb.Append(MailParser.ThisIsMultiPartMessageInMIMEFormat);
                sb.Append(MailParser.NewLine);
                for (int i = 0; i < this._Contents.Count; i++)
                {
                    sb.Append("--");
                    sb.Append(this.MultiPartBoundary);
                    sb.Append(MailParser.NewLine);
                    sb.Append(this.Contents[i].GetDataText());
                    sb.Append(MailParser.NewLine);
                }
                sb.Append(MailParser.NewLine);
                sb.AppendFormat("--{0}--", this.MultiPartBoundary);
            }
            else
            {
                sb.AppendFormat("Content-Type: {0}; charset=\"{1}\"", this.ContentType.Value, this.ContentEncoding.WebName);
                sb.Append(MailParser.NewLine);
                sb.Append(MailParser.NewLine);
                bodyText = MailParser.EncodeToMailBody(this.BodyText, this.ContentTransferEncoding, this.ContentEncoding);
				if (this.ContentTransferEncoding == TransferEncoding.SevenBit)
				{
                    sb.Append(bodyText);
				}
				else
				{
                    for (int i = 0; i < bodyText.Length; i++)
					{
						if (i > 0 && i % 76 == 0)
						{
							sb.Append(MailParser.NewLine);
						}
                        
                        if (i == 0 || (i > 2 && bodyText[i - 2] == '\r' && bodyText[i - 1] == '\n'))
                        {
                            if (bodyText[i] == '.')
                            {
                                sb.Append(".");
                            }
                        }
                        sb.Append(bodyText[i]);
					}
				}
            }
            sb.Append(MailParser.NewLine);
            sb.Append(MailParser.NewLine);
            sb.Append(".");
            sb.Append(MailParser.NewLine);

            return sb.ToString();
        }
        
        
        
        
        
        
        public void SetFromMailAddress(String userName, String mailAddress)
        {
            this.From = SmtpMessage.CreateFromMailAddress(userName, mailAddress); ;
        }
        
        
        
        
        
        
        public static String CreateFromMailAddress(String userName, String mailAddress)
        {
            return String.Format("\"{0}\" <{1}>", userName, mailAddress);
        }
        
        
        
        
        
        
        private String CreateMailAddressListText(List<MailAddress> mailAddressList)
        {
            StringBuilder sb = new StringBuilder();
            List<MailAddress> l = mailAddressList;
            String s = "";

            for (int i = 0; i < l.Count; i++)
            {
                sb.AppendFormat("{0}{1}", s, l[i].ToEncodeString().Trim());
                sb.Append(MailParser.NewLine);

                s = "\t, ";
            }
            return sb.ToString();
        }
    }
}
