using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Clover.Net.Mail;

namespace Clover.Net.Smtp
{
    
    
    
    
    public class SmtpContent : MimeContent
    {
        private FieldParameterEncoding _FieldParameterEncoding = FieldParameterEncoding.Rfc2047;
        private List<SmtpContent> _Contents;
        private static Dictionary<String, String> FileExtensionContentType = new Dictionary<String, String>();
        private String _BodyText = "";
        
        
        
        
        public FieldParameterEncoding FieldParameterEncoding
        {
            get { return this._FieldParameterEncoding; }
            set { this._FieldParameterEncoding = value; }
        }
        
        
        
        
        public String Name
        {
            get { return this.ContentType.Name; }
            set { this.ContentType.Name = value; }
        }
        
        
        
        
        public String FileName
        {
            get { return this.ContentDisposition.FileName; }
            set { this.ContentDisposition.FileName = value; }
        }
        
        
        
        
        public String BodyText
        {
            get { return this._BodyText; }
            set { this._BodyText = value; }
        }
        
        
        
        
        public new List<SmtpContent> Contents
        {
            get { return this._Contents; }
        }
        static SmtpContent()
        {
            SmtpContent.InitializeFileExtenstionContentType();
        }
		
		
		
		public SmtpContent() :
			base()
		{
			this._Contents = new List<SmtpContent>();
		}
        
        
        
        
        private static void InitializeFileExtenstionContentType()
        {
            SmtpContent.FileExtensionContentType.Add("txt", "text/plain");
            SmtpContent.FileExtensionContentType.Add("css", "text/css");
            SmtpContent.FileExtensionContentType.Add("htm", "text/html");
            SmtpContent.FileExtensionContentType.Add("html", "text/html");
            SmtpContent.FileExtensionContentType.Add("jpg", "Image/jpeg");
            SmtpContent.FileExtensionContentType.Add("gif", "Image/gif");
            SmtpContent.FileExtensionContentType.Add("bmp", "image/x-ms-bmp");
            SmtpContent.FileExtensionContentType.Add("png", "Image/png");
            SmtpContent.FileExtensionContentType.Add("wav", "Audio/wav");
            SmtpContent.FileExtensionContentType.Add("doc", "application/msword");
            SmtpContent.FileExtensionContentType.Add("mdb", "application/msaccess");
            SmtpContent.FileExtensionContentType.Add("xls", "application/vnd.ms-excel");
            SmtpContent.FileExtensionContentType.Add("ppt", "application/vnd.ms-powerpoint");
            SmtpContent.FileExtensionContentType.Add("mpeg", "video/mpeg");
            SmtpContent.FileExtensionContentType.Add("mpg", "video/mpeg");
            SmtpContent.FileExtensionContentType.Add("avi", "video/x-msvideo");
            SmtpContent.FileExtensionContentType.Add("zip", "application/zip");
        }
        
        
        
        
        
        
        private static String GetContentType(String extension)
        {
            String s = extension.Replace(".", "").ToLower();
            if (SmtpContent.FileExtensionContentType.ContainsKey(s.ToLower()) == true)
            {
                return SmtpContent.FileExtensionContentType[s.ToLower()];
            }
            return "application/octet-stream";
        }
        
        
        
        
        
        public void LoadData(String filePath)
        {
            FileInfo fi = null;
            Byte[] b = null;
            FileStream fsm = null;

            fi = new FileInfo(filePath);

            this.ContentType.Value = SmtpContent.GetContentType(Path.GetExtension(filePath).Replace(".", ""));
            this.ContentType.Name = fi.Name;
            this.ContentDisposition.FileName = fi.Name;
            this.ContentTransferEncoding = TransferEncoding.Base64;
            this.ContentDisposition.Value = "attachment";

            b = new Byte[fi.Length];
            using (fsm = new FileStream(filePath, FileMode.Open))
            {
                fsm.Read(b, 0, b.Length);
                this.BodyText = Convert.ToBase64String(b);
                fsm.Close();
            }
        }
        
        
        
        
        
        public void LoadData(Byte[] bytes)
        {
            Byte[] b = bytes;

            this.ContentTransferEncoding = TransferEncoding.Base64;
            this.ContentDisposition.Value = "attachment";
            this.BodyText = Convert.ToBase64String(b);
        }
        
        
        
        
        
        public String GetDataText()
        {
            StringBuilder sb = new StringBuilder();
            String bodyText = "";

            if (this.IsMultiPart == false &&
                this.Contents.Count > 0)
            {
                this.ContentType.Value = "multipart/mixed";
            }
            if (this.IsBody == true)
            {
                sb.AppendFormat("Content-Type: {0}; charset=\"{1}\"", this.ContentType.Value, this.ContentEncoding.WebName);
                sb.Append(MailParser.NewLine);
            }
            else
            {
                sb.AppendFormat("Content-Type: {0};", this.ContentType.Value);
                sb.Append(MailParser.NewLine);
                if (String.IsNullOrEmpty(this.ContentType.Name) == false)
                {
                    if (this._FieldParameterEncoding == Mail.FieldParameterEncoding.Rfc2047)
                    {
                        sb.AppendFormat(" name=\"{0}\"", MailParser.EncodeToMailHeaderLine(this.ContentType.Name
                            , this.ContentTransferEncoding, this.ContentEncoding, MailParser.MaxCharCountPerRow - 8));
                    }
                    else if (this._FieldParameterEncoding == Mail.FieldParameterEncoding.Rfc2231)
                    {
                        sb.AppendFormat(MailParser.EncodeToMailHeaderLineByRfc2231("name", this.ContentType.Name
                            , this.ContentEncoding, MailParser.MaxCharCountPerRow - 8));
                    }
                    sb.Append(MailParser.NewLine);
                }
            }
            sb.AppendFormat("Content-Transfer-Encoding: {0}", MailParser.ToTransferEncoding(this.ContentTransferEncoding));
            sb.Append(MailParser.NewLine);
            if (String.IsNullOrEmpty(this["Content-Disposition"]) == false)
            {
                sb.AppendFormat("Content-Disposition: {0};", this.ContentDisposition.Value);
                sb.Append(MailParser.NewLine);
                if (String.IsNullOrEmpty(this.ContentDisposition.FileName) == false)
                {
                    if (this._FieldParameterEncoding == Mail.FieldParameterEncoding.Rfc2047)
                    {
                        sb.AppendFormat(" filename=\"{0}\"", MailParser.EncodeToMailHeaderLine(this.ContentDisposition.FileName
                            , this.ContentTransferEncoding, this.ContentEncoding, MailParser.MaxCharCountPerRow - 12));
                    }
                    else if (this._FieldParameterEncoding == Mail.FieldParameterEncoding.Rfc2231)
                    {
                        sb.AppendFormat(MailParser.EncodeToMailHeaderLineByRfc2231("filename", this.ContentDisposition.FileName
                            , this.ContentEncoding, MailParser.MaxCharCountPerRow - 12));
                    }
                    sb.Append(MailParser.NewLine);
                }
            }
            if (String.IsNullOrEmpty(this["Content-Description"]) == false)
            {
                sb.AppendFormat("Content-Description: {0}", this["Content-Description"]);
                sb.Append(MailParser.NewLine);
            }

            if (this.IsMultiPart == true)
            {
                for (int i = 0; i < this._Contents.Count; i++)
                {
                    sb.Append(MailParser.NewLine);
                    sb.Append("--");
                    sb.Append(this.MultiPartBoundary);
                    sb.Append(MailParser.NewLine);
                    sb.Append(this._Contents[i].GetDataText());
                    sb.Append(MailParser.NewLine);
                }
                sb.Append(MailParser.NewLine);
                sb.AppendFormat("--{0}--", this.MultiPartBoundary);
            }
            else
            {
                sb.Append(MailParser.NewLine);
                if (this.IsAttachment == true)
                {
                    bodyText = this.BodyText;
                }
                else
                {
                    bodyText = MailParser.EncodeToMailBody(this.BodyText, this.ContentTransferEncoding, this.ContentEncoding);
                }
				if (this.ContentTransferEncoding == TransferEncoding.SevenBit)
				{
                    sb.Append(bodyText);
				}
				else
				{
                    for (int i = 0; i < bodyText.Length; i++)
					{
						if (i > 0 &&
							i % 76 == 0)
						{
							sb.Append(MailParser.NewLine);
						}
                        sb.Append(bodyText[i]);
					}
				}
            }
            return sb.ToString();
        }
    }
}
