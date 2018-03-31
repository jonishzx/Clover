using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Clover.Net.Mail;

namespace Clover.Net
{
    
    
    
    
    public class Pop3Content : MimeContent
    {
        private Pop3Message _Message;
        private Pop3Content _ParentContent = null;
        private String _Data;
        
        
        
        
        
        private String _BodyText;
        private Boolean _BodyTextCreated = false;
        private List<Pop3Content> _Contents = new List<Pop3Content>();
        
        
        
        
        
        public Pop3Content ParentContent
        {
            get { return this._ParentContent; }
            private set { this._ParentContent = value; }
        }
        
        
        
        
        
        public String Name
        {
            get { return this.ContentType.Name; }
        }
        
        
        
        
        
        public String FileName
        {
            get { return this.ContentDisposition.FileName; }
        }
        
        
        
        
        
        public String BodyText
        {
            get
            {
                this.EnsureBodyText();
                return this._BodyText;
            }
            set { this._BodyText = value; }
        }
        
        
        
        
        
        public String Data
        {
            get { return this._Data; }
        }
        
        
        
        
        
        public new List<Pop3Content> Contents
        {
            get { return this._Contents; }
        }
        
        
        
        
        
        protected Boolean BodyTextCreated
        {
            get { return this._BodyTextCreated; }
            set { this._BodyTextCreated = value; }
        }
		
		
		
		
		
        public Pop3Content(Pop3Message message, String text) : 
            base(text)
        {
            this.Initialize(message, text);
        }
        
        
        
        
        
        
        private void Initialize(Pop3Message message, String text)
        {
            Pop3Content ct = null;

            this._Message = message;
            this._Contents = new List<Pop3Content>();
            this._Data = text;
            this._BodyText = "";
            if (this.IsMultiPart == true)
            {
                List<String> l=  MimeContent.ParseToContentTextList(this.BodyData, this.MultiPartBoundary);
                for (int i = 0; i < l.Count; i++)
                {
                    ct = new Pop3Content(this._Message, l[i]);
                    ct.ParentContent = this;
                    this._Contents.Add(ct);
                }
            }
        }
        
        
        
        
        
        protected virtual void EnsureBodyText()
        {
            if (this.BodyTextCreated == false)
            {
                if (this.ContentType.Value.IndexOf("message/rfc822") > -1)
                {
                    this.BodyText = this.BodyData;
                }
                else if (this.IsText == true)
                {
                    this.BodyText = MailParser.DecodeFromMailBody(this.BodyData, this.ContentTransferEncoding, this.ContentEncoding);
                }
                else
                {
                    this.BodyText = this.BodyData;
                }
            }
            this.BodyTextCreated = true;
        }
        
        
        
        
        
        
        public Smtp.SmtpContent CreateSmtpContent()
        {
            Smtp.SmtpContent ct = new Clover.Net.Smtp.SmtpContent();
            Field f = null;

            for (int i = 0; i < this.Header.Count; i++)
            {
                f = this.Header[i];
                if (String.IsNullOrEmpty(f.Value) == true)
                { continue; }
                ct[f.Key] = MailParser.DecodeFromMailHeaderLine(f.Value);
            }
            for (int i = 0; i < this.ContentType.Fields.Count; i++)
            {
                f = this.ContentType.Fields[i];
                ct.ContentType.Fields.Add(new Field(f.Key, MailParser.DecodeFromMailHeaderLine(f.Value)));
            }
            for (int i = 0; i < this.ContentDisposition.Fields.Count; i++)
            {
                f = this.ContentDisposition.Fields[i];
                ct.ContentDisposition.Fields.Add(new Field(f.Key, MailParser.DecodeFromMailHeaderLine(f.Value)));
            }
            ct.BodyText = this.BodyText;
            for (int i = 0; i < this.Contents.Count; i++)
            {
                ct.Contents.Add(this.Contents[i].CreateSmtpContent());
            }
            return ct;
        }
    }
}
