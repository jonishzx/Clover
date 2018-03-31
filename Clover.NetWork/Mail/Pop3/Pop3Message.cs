using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Clover.Net.Mail;

namespace Clover.Net
{
	
	
	
	
    public class Pop3Message : InternetTextMessage
    {
        private Boolean _InvalidFormat = false;
        private String _Data;
        
        
        
        
        
        private String _BodyText;
        private Boolean _BodyTextCreated = false;
        private Pop3Content _BodyContent = null;
        private List<Pop3Content> _Contents = new List<Pop3Content>();
        private Int64? _Index = 0;
        private Int32 _Size = 0;
        
        
        
        
        
        public Int64? Index
        {
            get { return this._Index; }
        }
        
        
        
        
        
        public String Data
        {
            get { return this._Data; }
        }
        
        
        
        
        
        public String To
        {
            get { return this["To"]; }
        }
        
        
        
        
        
        public String Cc
        {
            get { return this["Cc"]; }
        }
        
        
        
        
        
        public String Bcc
        {
            get { return this["Bcc"]; }
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
		
		
		
		
		
		public new String HeaderData
		{
			get { return base.HeaderData; }
		}
		
		
		
		
		
		public new String BodyData
		{
			get { return base.BodyData; }
		}
        
        
        
        
        
        public Int32 Size
        {
            get { return this._Size; }
            set { this._Size = value; }
        }
        
        
        
        
        
        public Pop3Content BodyContent
        {
            get
            {
                this.EnsureBodyContent(this._Contents);
                return this._BodyContent;
            }
        }
        
        
        
        
        
        public List<Pop3Content> Contents
        {
            get { return this._Contents; }
        }
        
        
        
        
        
        public Boolean InvalidFormat
        {
            get { return this._InvalidFormat; }
        }
        
        
        
        
        
        protected Boolean BodyTextCreated
        {
            get { return this._BodyTextCreated; }
            set { this._BodyTextCreated = value; }
        }
		
		
		
		
        public Pop3Message(String text) : 
            base(text)
        {
            this.Initialize(text);
        }
		
		
		
		
		
        public Pop3Message(String text, Int64 index) :
            base(text)
        {
            this._Index = index;
            this.Initialize(text);
        }
        private void Initialize(String text)
        {
            this._Data = text;
            this._Size = text.Length;
            if (this.IsMultiPart == true)
            {
                List<String> l = MimeContent.ParseToContentTextList(this.BodyData, this.MultiPartBoundary);
                for (int i = 0; i < l.Count; i++)
                {
                    this._Contents.Add(new Pop3Content(this, l[i]));
                }
            }
        }
        
        
        
        
        
        
        
        private Boolean EnsureBodyContent(List<Pop3Content> contents)
        {
            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i].IsBody == true)
                {
                    this._BodyContent = contents[i];
                    return true;
                }
                if (this.EnsureBodyContent(contents[i].Contents) == true)
                { return true; }
            }
            return false;
        }
        
        
        
        
        
        
        public static List<Pop3Content> GetAllContents(Pop3Message pop3Message)
        {
            if (pop3Message == null)
            { throw new ArgumentNullException("pop3Message"); }
            List<Pop3Content> l = new List<Pop3Content>();
            l = Pop3Message.GetAttachedContents(pop3Message.Contents, delegate(Pop3Content c) { return true; });
            return l;
        }
        
        
        
        
        
        
        public static List<Pop3Content> GetAttachedContents(Pop3Message pop3Message)
        {
            if (pop3Message == null)
            { throw new ArgumentNullException("pop3Message"); }
            List<Pop3Content> l = new List<Pop3Content>();
            l = Pop3Message.GetAttachedContents(pop3Message.Contents, delegate(Pop3Content c) { return c.IsAttachment; });
            return l;
        }
        
        
        
        
        
        
        
        
        public static List<Pop3Content> GetAttachedContents(List<Pop3Content> contents, Predicate<Pop3Content> predicate)
        {
            List<Pop3Content> l = new List<Pop3Content>();
            for (int i = 0; i < contents.Count; i++)
            {
                if (predicate(contents[i]) == true)
                {
                    l.Add(contents[i]);
                }
                l.AddRange(Pop3Message.GetAttachedContents(contents[i].Contents, predicate).ToArray());
            }
            return l;
        }
        
        
        
        
        
        
        protected virtual void EnsureBodyText()
        {
            if (this.BodyTextCreated == false)
            {
                if (this.ContentType.Value.IndexOf("message/rfc822") > -1)
                {
                    this.BodyText = this.BodyData;
                }
                else if (this.IsMultiPart == true)
                {
                    if (this.BodyContent == null)
                    {
                        this.BodyText = "";
                    }
                    else
                    {
                        this.BodyText = this.BodyContent.BodyText;
                    }
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
        
        
        
        
        
        
        public Smtp.SmtpMessage CreateSmtpMessage()
        {
            Smtp.SmtpMessage mg = new Clover.Net.Smtp.SmtpMessage();
            Field f = null;

            mg.To.AddRange(MailAddress.CreateMailAddressList(this.To));
			mg.Cc.AddRange(MailAddress.CreateMailAddressList(this.Cc));
            for (int i = 0; i < this.Header.Count; i++)
            {
                f = this.Header[i];
                if (String.IsNullOrEmpty(f.Value) == true)
                { continue; }
                if (f.Key.ToLower() == "to" ||
                    f.Key.ToLower() == "cc")
                { continue; }
                mg[f.Key] = MailParser.DecodeFromMailHeaderLine(f.Value);
            }
            for (int i = 0; i < this.ContentType.Fields.Count; i++)
            {
                f = this.ContentType.Fields[i];
                mg.ContentType.Fields.Add(new Field(f.Key, MailParser.DecodeFromMailHeaderLine(f.Value)));
            }
            for (int i = 0; i < this.ContentDisposition.Fields.Count; i++)
            {
                f = this.ContentDisposition.Fields[i];
                mg.ContentDisposition.Fields.Add(new Field(f.Key, MailParser.DecodeFromMailHeaderLine(f.Value)));
            }
            mg.BodyText = this.BodyText;
            for (int i = 0; i < this.Contents.Count; i++)
            {
                mg.Contents.Add(this.Contents[i].CreateSmtpContent());
            }
            return mg;
        }
    }
}
