using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Configuration;

namespace Clover.Core.NetWork
{
    #region 邮件接收类

    
    
    
    public class POP3
    {
        #region Fields

        string POPServer;
        string mPOPUserName;
        string mPOPPass;
        int mPOPPort;
        NetworkStream ns;
        StreamReader sr;

        #endregion

        #region Constructors

        
        
        
        
        
        
        public POP3(string server, string userName, string password)
            : this(server, 110, userName, password)
        {
        }

        
        
        
        
        
        
        
        public POP3(string server, int port, string userName, string password)
        {
            POPServer = server;
            mPOPUserName = userName;
            mPOPPass = password;
            mPOPPort = port;
        }

        #endregion
        
        #region Methods

        #region Public

        
        
        
        
        public int GetNumberOfNewMessages()
        {
            byte[] outbytes;
            string input;

            try
            {
                Connect();

                input = "stat" + "\r\n";
                outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                ns.Write(outbytes, 0, outbytes.Length);
                string resp = sr.ReadLine();
                string[] tokens = resp.Split(new Char[] { ' ' });

                Disconnect();

                return Convert.ToInt32(tokens[1]);
            }
            catch
            {
                return -1;
            }
        }

        
        
        
        
        
        
        public List<MailMessage> GetNewMessages(string subj, bool deleleAfterRtr)
        {

            int newcount;
            List<MailMessage> newmsgs = new List<MailMessage>();

            try
            {
                newcount = GetNumberOfNewMessages();
                Connect();

                for (int n = 1; n < newcount + 1; n++)
                {
                    List<string> msglines = GetRawMessage(n);
                    string msgsubj = GetMessageSubject(msglines);
                    if (msgsubj.CompareTo(subj) == 0)
                    {
                        MailMessage msg = new MailMessage();
                        msg.Subject = msgsubj;
                        msg.From = new MailAddress(GetMessageFrom(msglines));
                        msg.Body = GetMessageBody(msglines);
                        newmsgs.Add(msg);

                        if (deleleAfterRtr)
                            DeleteMessage(n);
                    }
                }

                Disconnect();
                return newmsgs;
            }
            catch 
            {
                return newmsgs;
            }
        }

        
        
        
        
        
        public MailMessage GetNewMessages(int nIndex)
        {
            int newcount;
            MailMessage msg = new MailMessage();

            try
            {
                newcount = GetNumberOfNewMessages();
                Connect();
                int n = nIndex + 1;

                if (n < newcount + 1)
                {
                    List<string> msglines = GetRawMessage(n);
                    string msgsubj = GetMessageSubject(msglines);


                    msg.Subject = msgsubj;
                    msg.From = new MailAddress(GetMessageFrom(msglines));
                    msg.Body = GetMessageBody(msglines);
                }

                Disconnect();
                return msg;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Private

        private bool Connect()
        {
            TcpClient sender = new TcpClient(POPServer, mPOPPort);
            byte[] outbytes;
            string input;

            try
            {
                ns = sender.GetStream();
                sr = new StreamReader(ns);

                sr.ReadLine();
                input = "user " + mPOPUserName + "\r\n";
                outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                ns.Write(outbytes, 0, outbytes.Length);
                sr.ReadLine();

                input = "pass " + mPOPPass + "\r\n";
                outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                ns.Write(outbytes, 0, outbytes.Length);
                sr.ReadLine();
                return true;

            }
            catch
            {
                return false;
            }
        }

        private void Disconnect()
        {
            string input = "quit" + "\r\n";
            Byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
            ns.Write(outbytes, 0, outbytes.Length);
            ns.Close();
        }

        private List<string> GetRawMessage(int messagenumber)
        {
            Byte[] outbytes;
            string input;
            string line = string.Empty;

            input = "retr " + messagenumber.ToString() + "\r\n";
            outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
            ns.Write(outbytes, 0, outbytes.Length);

            List<string> msglines = new List<string>();
            do
            {
                line = sr.ReadLine();
                msglines.Add(line);
            } while (line != ".");
            msglines.RemoveAt(msglines.Count - 1);

            return msglines;
        }

        private string GetMessageSubject(List<string> msglines)
        {
            string[] tokens;
            IEnumerator msgenum = msglines.GetEnumerator();
            while (msgenum.MoveNext())
            {
                string line = (string)msgenum.Current;
                if (line.StartsWith("Subject:"))
                {
                    tokens = line.Split(new Char[] { ' ' });
                    return tokens[1].Trim();
                }
            }
            return "None";
        }

        private string GetMessageFrom(List<string> msglines)
        {
            string[] tokens;
            IEnumerator msgenum = msglines.GetEnumerator();
            while (msgenum.MoveNext())
            {
                string line = (string)msgenum.Current;
                if (line.StartsWith("From:"))
                {
                    tokens = line.Split(new Char[] { '<' });
                    return tokens[1].Trim(new Char[] { '<', '>' });
                }
            }
            return "None";
        }

        private string GetMessageBody(List<string> msglines)
        {
            string body = string.Empty;
            string line = " ";
            IEnumerator msgenum = msglines.GetEnumerator();

            while (line.CompareTo(string.Empty) != 0)
            {
                msgenum.MoveNext();
                line = (string)msgenum.Current;
            }

            while (msgenum.MoveNext())
            {
                body = body + (string)msgenum.Current + "\r\n";
            }
            return body;
        }

        private void DeleteMessage(int messagenumber)
        {
            Byte[] outbytes;
            string input;

            try
            {
                input = "dele " + messagenumber.ToString() + "\r\n";
                outbytes = System.Text.Encoding.ASCII.GetBytes(input.ToCharArray());
                ns.Write(outbytes, 0, outbytes.Length);
            }
            catch 
            {
                return;
            }

        }

        #endregion

        #endregion
    }

    #endregion

    #region 邮件发送类

    public class SMTP
    {
        #region Fields

        private string mMailFrom;
        private string mMailDisplyName;
        private string[] mMailTo;
        private string[] mMailCc;
        private string[] mMailBcc;
        private string mMailSubject;
        private string mMailBody;
        private string[] mMailAttachments;
        private string mSMTPServer;
        private int mSMTPPort;
        private string mSMTPUsername;
        private string mSMTPPassword;
        private bool mSMTPSSL;
        private MailPriority mPriority = MailPriority.Normal;
        private bool mIsBodyHtml = false;
        private MailMessage MailObject;
        bool mailSent = false;

        #endregion

        #region Properties

        
        
        
        public string MailFrom
        {
            set { mMailFrom = value; }
            get { return mMailFrom; }
        }

        
        
        
        public string MailDisplyName
        {
            set { mMailDisplyName = value; }
            get { return mMailDisplyName; }
        }

        
        
        
        public string[] MailTo
        {
            set { mMailTo = value; }
            get { return mMailTo; }
        }

        
        
        
        public string[] MailCc
        {
            set { mMailCc = value; }
            get { return mMailCc; }
        }

        
        
        
        public string[] MailBcc
        {
            set { mMailBcc = value; }
            get { return mMailBcc; }
        }

        
        
        
        public string MailSubject
        {
            set { mMailSubject = value; }
            get { return mMailSubject; }
        }

        
        
        
        public string MailBody
        {
            set { mMailBody = value; }
            get { return mMailBody; }
        }

        
        
        
        public string[] MailAttachments
        {
            set { mMailAttachments = value; }
            get { return mMailAttachments; }
        }

        
        
        
        public string SMTPServer
        {
            set { mSMTPServer = value; }
            get { return mSMTPServer; }
        }

        
        
        
        public int SMTPPort
        {
            set { mSMTPPort = value; }
            get { return mSMTPPort; }
        }

        
        
        
        public string SMTPUsername
        {
            set { mSMTPUsername = value; }
            get { return mSMTPUsername; }
        }

        
        
        
        public string SMTPPassword
        {
            set { mSMTPPassword = value; }
            get { return mSMTPPassword; }
        }

        
        
        
        
        public Boolean SMTPSSL
        {
            set { mSMTPSSL = value; }
            get { return mSMTPSSL; }
        }

        
        
        
        public MailPriority Priority
        {
            get { return mPriority; }
            set { mPriority = value; }
        }

        
        
        
        public bool IsBodyHtml
        {
            get { return mIsBodyHtml; }
            set { mIsBodyHtml = value; }
        }

        #endregion

        #region Constructors

        
        
        
        
        
        
        
        
        
        public SMTP(string smtpserver, string msmtpusername, string msmtppassword)
        {
            MailObject = new MailMessage();

            this.SMTPServer = smtpserver;
            this.SMTPUsername = msmtpusername;
            this.SMTPPassword = msmtppassword;                
        }

        
        
        
        
        
        
        
        
        
        public SMTP(string[] mailTo, string mailSubject, string mailBody)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailSettings = NetSectionGroup.GetSectionGroup(config).MailSettings;

            MailObject = new MailMessage();
            mMailFrom = mailSettings.Smtp.From;
            mMailDisplyName = mailSettings.Smtp.From;
            mMailTo = mailTo;
            mMailCc = null;
            mMailBcc = null;
            mMailSubject = mailSubject;
            mMailBody = mailBody;
            mMailAttachments = null;
            mSMTPServer = mailSettings.Smtp.Network.Host;
            mSMTPPort = mailSettings.Smtp.Network.Port;
            mSMTPUsername = mailSettings.Smtp.Network.UserName;
            mSMTPPassword = mailSettings.Smtp.Network.Password;
            mSMTPSSL = false;
        }

        
        
        
        
        
        
        
        
        
        
        public SMTP(string mailFrom, string[] mailTo, string mailSubject, string mailBody,
            string smtpServer, string userName, string password)
            : this(mailFrom, mailFrom, mailTo, mailSubject, mailBody, null, smtpServer, userName, password)
        {
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public SMTP(string mailFrom, string[] mailTo, string mailSubject, string mailBody,
            string[] attachments, string smtpServer, string userName, string password)
            : this(mailFrom, mailFrom, mailTo, mailSubject, mailBody,
            attachments, smtpServer, userName, password)
        {
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public SMTP(string mailFrom, string displayName, string[] mailTo, string mailSubject, string mailBody,
            string[] attachments, string smtpServer, string userName, string password)
            : this(mailFrom, displayName, mailTo, null, null, mailSubject, mailBody,
            attachments, smtpServer, 25, userName, password, false)
        {
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public SMTP(string mailFrom, string displayName, string[] mailTo, string[] mailCc, string[] mailBcc, string mailSubject, string mailBody,
            string[] attachments, string smtpServer, int smtpPort, string userName, string password, bool smtpSsl)
        {
            MailObject = new MailMessage();
            mMailFrom = mailFrom;
            mMailDisplyName = displayName;
            mMailTo = mailTo;
            mMailCc = mailCc;
            mMailBcc = mailBcc;
            mMailSubject = mailSubject;
            mMailBody = mailBody;
            mMailAttachments = attachments;
            mSMTPServer = smtpServer;
            mSMTPPort = smtpPort;
            mSMTPUsername = userName;
            mSMTPPassword = password;
            mSMTPSSL = smtpSsl;
        }

        #endregion

        #region Methods

        
        
        
        
        public Boolean Send()
        {
            return SendMail(false, null);
        }

        
        
        
        
        
        public void SendAsync(object userState)
        {
            SendMail(true, userState);
        }

        
        
        
        
        
        
        private Boolean SendMail(bool isAsync, object userState)
        {
            #region 设置属性值

            string[] mailTos = mMailTo;
            string[] mailCcs = mMailCc;
            string[] mailBccs = mMailBcc;
            string[] attachments = mMailAttachments;

            
            MailMessage Email = new MailMessage();
            MailAddress MailFrom =
              new MailAddress(mMailFrom, mMailDisplyName);
            Email.From = MailFrom;

            if (mailTos != null)
            {
                foreach (string mailto in mailTos)
                {
                    if (!string.IsNullOrEmpty(mailto))
                    {
                        Email.To.Add(mailto);
                    }
                }
            }

            if (mailCcs != null)
            {
                foreach (string cc in mailCcs)
                {
                    if (!string.IsNullOrEmpty(cc))
                    {
                        Email.CC.Add(cc);
                    }
                }
            }

            if (mailBccs != null)
            {
                foreach (string bcc in mailBccs)
                {
                    if (!string.IsNullOrEmpty(bcc))
                    {
                        Email.Bcc.Add(bcc);
                    }
                }
            }

            if (attachments != null)
            {
                foreach (string file in attachments)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        Attachment att = new Attachment(file);
                        Email.Attachments.Add(att);
                    }
                }
            }

            Email.Subject = mMailSubject;
            Email.Body = mMailBody;
            Email.Priority = mPriority;
            Email.IsBodyHtml = mIsBodyHtml;

            
            SmtpClient SmtpMail =
             new SmtpClient(mSMTPServer, mSMTPPort);
            SmtpMail.Credentials =
             new NetworkCredential(mSMTPUsername, mSMTPPassword);
            SmtpMail.EnableSsl = mSMTPSSL;
            

            SmtpMail.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            #endregion

            try
            {
                if (!isAsync)
                {
                    SmtpMail.Send(Email);
                    mailSent = true;
                }
                else
                {
                    userState = (userState == null) ? Guid.NewGuid() : userState;
                    SmtpMail.SendAsync(Email, userState);
                }
            }
            
            
                
            
            
            catch 
            {
                
                mailSent = false;
            }

            return mailSent;
        }

        private void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
                mailSent = false;
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
                mailSent = false;
            }
            else
            {
                Console.WriteLine("Message sent.");
                mailSent = false;
            }

            mailSent = true;
        }

        #endregion
    }

    #endregion
}

#region 附加内容

#region POP3 命令简介



#endregion

#endregion