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
using LumiSoft.Net.AUTH;
using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;
using LumiSoft.Net.SMTP.Client;
using System.Text;
using LumiSoft.Net.Mime;
namespace Clover.Net.Mail
{


    public class MailSmtpClient
    {
        #region Fields

        private string mMailFrom;
        private string mMailDisplyName;
        private string[] mMailTo;
        private string[] mMailCc;
        private string[] mMailBcc;
        private string mMailSubject;
        private string mMailBody;
        private Dictionary<string, string> mMailAttachments = new Dictionary<string, string>();

        private string mSMTPServer;
        private int mSMTPPort = 25;
        private string mSMTPUsername;
        private string mSMTPPassword;
        private bool mSMTPSSL;
        private MailPriority mPriority = MailPriority.Normal;
        private bool mIsBodyHtml = false;
        private MailMessage MailObject;
        public string mSMTPDomain;
        bool mailSent = false;
        public string ErrorMessage { get; set; }
        public string SMTPDomain { get { return mSMTPDomain; } set { mSMTPDomain = value; } }


        private int _timeount = 3 * 60 * 1000; 
        
        
        
        public int Timeout { get { return _timeount; } set { _timeount = value; } }
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


        
        
        
        public Dictionary<string, string> MailAttachments
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

        
        
        
        
        
        
        
        
        
        public MailSmtpClient(string smtpserver, string msmtpusername, string msmtppassword)
        {
            MailObject = new MailMessage();

            this.SMTPServer = smtpserver;
            this.SMTPUsername = msmtpusername;
            this.SMTPPassword = msmtppassword;
        }

        
        
        
        
        
        
        
        
        
        public MailSmtpClient(string[] mailTo, string mailSubject, string mailBody)
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

        
        
        
        
        
        
        
        
        
        
        public MailSmtpClient(string mailFrom, string[] mailTo, string mailSubject, string mailBody,
            string smtpServer, string userName, string password)
            : this(mailFrom, mailFrom, mailTo, mailSubject, mailBody, null, smtpServer, userName, password)
        {
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public MailSmtpClient(string mailFrom, string[] mailTo, string mailSubject, string mailBody,
            Dictionary<string, string> attachments, string smtpServer, string userName, string password)
            : this(mailFrom, mailFrom, mailTo, mailSubject, mailBody,
            attachments, smtpServer, userName, password)
        {
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public MailSmtpClient(string mailFrom, string displayName, string[] mailTo, string mailSubject, string mailBody,
            Dictionary<string, string> attachments, string smtpServer, string userName, string password)
            : this(mailFrom, displayName, mailTo, null, null, mailSubject, mailBody,
            attachments, smtpServer, 25, userName, password, false)
        {
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public MailSmtpClient(string mailFrom, string displayName, string[] mailTo, string[] mailCc, string[] mailBcc, string mailSubject, string mailBody,
            Dictionary<string, string> attachments, string smtpServer, int smtpPort, string userName, string password, bool smtpSsl)
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
        public bool Authenticate()
        {
            var musername = mSMTPUsername;
            if (mSMTPUsername != null && mSMTPUsername.IndexOf("@") >= 0)
            {
                musername = mSMTPUsername.Substring(0, mSMTPUsername.IndexOf("@"));
            }

            using (SMTP_Client client = new SMTP_Client())
            {
                try
                {
                    client.Connect(mSMTPServer, mSMTPPort);
                    client.EhloHelo(mSMTPServer);

                    client.Authenticate(musername, mSMTPPassword);

                    return client.IsAuthenticated;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }
        
        
        
        
        public Boolean Send()
        {
            return SendMail(false, null);
        }

        
        
        
        
        
        public void SendAsync(object userState)
        {
            SendMail(true, null);
        }

        
        
        
        
        
        
        private Boolean SendMail(bool isAsync, object userState)
        {
            string[] array = this.mMailTo;
            string[] array2 = this.mMailCc;
            string[] array3 = this.mMailBcc;
            System.Collections.Generic.Dictionary<string, string> dictionary = this.mMailAttachments;
            MailMessage mailMessage = new MailMessage();
            System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(this.mMailFrom, this.mMailDisplyName);
            mailMessage.From = from;
            if (array != null)
            {
                string[] array4 = array;
                for (int i = 0; i < array4.Length; i++)
                {
                    string text = array4[i];
                    if (!string.IsNullOrEmpty(text))
                    {
                        mailMessage.To.Add(text);
                    }
                }
            }
            if (array2 != null)
            {
                string[] array5 = array2;
                for (int j = 0; j < array5.Length; j++)
                {
                    string text2 = array5[j];
                    if (!string.IsNullOrEmpty(text2))
                    {
                        mailMessage.CC.Add(text2);
                    }
                }
            }
            if (array3 != null)
            {
                string[] array6 = array3;
                for (int k = 0; k < array6.Length; k++)
                {
                    string text3 = array6[k];
                    if (!string.IsNullOrEmpty(text3))
                    {
                        mailMessage.Bcc.Add(text3);
                    }
                }
            }
            if (dictionary != null)
            {
                foreach (string current in dictionary.Keys)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(current);
                    if (!string.IsNullOrEmpty(current) && fileInfo.Exists)
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(current);
                        string text4 = dictionary[current];
                        if (text4.Length > 30)
                        {
                            text4 = text4.Substring(0, 15) + "_略" + fileInfo.Extension;
                        }
                        Attachment item = new Attachment(streamReader.BaseStream, text4);
                        mailMessage.Attachments.Add(item);
                    }
                }
            }
            mailMessage.Subject = this.mMailSubject;
            mailMessage.Body = this.mMailBody;
            mailMessage.Priority = this.mPriority;
            mailMessage.IsBodyHtml = this.mIsBodyHtml;
            string userName = this.mSMTPUsername;
            if (this.mSMTPUsername != null && this.mSMTPUsername.IndexOf("@") >= 0)
            {
                userName = this.mSMTPUsername.Substring(0, this.mSMTPUsername.IndexOf("@"));
            }
            SmtpClient smtpClient = new SmtpClient(this.mSMTPServer, this.mSMTPPort);
            if (!string.IsNullOrEmpty(this.mSMTPDomain))
            {
                smtpClient.Credentials = new NetworkCredential(userName, this.mSMTPPassword, this.mSMTPDomain);
            }
            else
            {
                smtpClient.Credentials = new NetworkCredential(userName, this.mSMTPPassword);
            }
            smtpClient.Timeout = this.Timeout;
            smtpClient.EnableSsl = this.mSMTPSSL;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.SendCompleted += new SendCompletedEventHandler(this.SendCompletedCallback);
            try
            {
                if (!isAsync)
                {
                    smtpClient.Send(mailMessage);
                    this.mailSent = true;
                }
                else
                {
                    userState = ((userState == null) ? System.Guid.NewGuid() : userState);
                    smtpClient.SendAsync(mailMessage, userState);
                }
            }
            catch (SmtpException ex)
            {
                this.ErrorMessage = string.Concat(new object[]
		        {
			        ex.Message,
			        "<br/>error code:",
			        ex.StatusCode,
			        "<br/>",
			        ex.StackTrace
		        });
                this.mailSent = false;
            }
            catch (System.Exception ex2)
            {
                this.ErrorMessage = ex2.Message + "<br/>" + ex2.StackTrace;
                this.mailSent = false;
            }
            finally
            {
                foreach (Attachment current2 in mailMessage.Attachments)
                {
                    if (current2.ContentStream != null)
                    {
                        current2.ContentStream.Close();
                    }
                }
                mailMessage.Dispose();
                smtpClient.Dispose();
            }
            return this.mailSent;
        }


        public Boolean SendMailNew()
        {
            return SendMailNew(false, null);
        }

        private Boolean SendMailNew(bool isAsync, object userState)
        {

            string[] mailTos = mMailTo;
            string[] mailCcs = mMailCc;
            string[] mailBccs = mMailBcc;
            Dictionary<string, string> attachments = mMailAttachments;

            var toList = new Dictionary<string, string>();
            foreach (var to in MailTo)
            {
                toList.Add(to, to);
            }

            var musername = mSMTPUsername;
            if (mSMTPUsername != null && mSMTPUsername.IndexOf("@") >= 0)
            {
                musername = mSMTPUsername.Substring(0, mSMTPUsername.IndexOf("@"));
            }

            bool checkspmail = CheckSpecialMail(toList);

            if (!checkspmail && MailCc != null)
                checkspmail = CheckSpecialMail(MailCc);
            if (checkspmail)
            {
                foreach (string address in toList.Keys)
                {
                    using (SMTP_Client client = new SMTP_Client())
                    {
                        client.Timeout = this.Timeout;
                        client.Connect(mSMTPServer, mSMTPPort);
                        client.EhloHelo(mSMTPServer);

                        
                        client.Authenticate(musername, mSMTPPassword);
                        client.MailFrom(mMailFrom, -1);
                        client.RcptTo(address);

                        
                        Mail_Message m = Create_PlainText_Html_Attachment_Image(toList, new Dictionary<string, string>()
                            , mMailFrom,
                            mMailFrom, MailSubject, mMailBody, attachments, "", "", CheckSpecialMail(new string[] { address }));

                        try
                        {

                            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                            {
                                m.ToStream(stream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
                                stream.Position = 0;
                                client.SendMessage(stream);
                            }


                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        if (m != null)
                        {
                            m.Dispose();
                        }

                        client.Disconnect();
                    }
                } return true;
            }
            else
            {

                using (SMTP_Client client = new SMTP_Client())
                {
                    client.Timeout = this.Timeout;
                    client.Connect(mSMTPServer, mSMTPPort);
                    client.EhloHelo(mSMTPServer);
                    AUTH_SASL_Client authhh = client.AuthGetStrongestMethod(musername, mSMTPPassword);
                    client.Auth(authhh);
                    client.MailFrom(mMailFrom, -1);
                    foreach (string address in toList.Keys)
                    {
                        client.RcptTo(address);
                    }


                    
                    Mail_Message m = Create_PlainText_Html_Attachment_Image(toList, new Dictionary<string, string>()
                        , mMailFrom,
                        mMailFrom, MailSubject, mMailBody, attachments, "", "", false);

                    try
                    {

                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                        {
                            m.ToStream(stream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
                            stream.Position = 0;
                            client.SendMessage(stream);
                        }


                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (m != null)
                        {
                            m.Dispose();
                        }
                        client.Disconnect();
                    }
                }
            }
        }



        private Mail_Message Create_PlainText_Html_Attachment_Image(Dictionary<string, string> tomails, Dictionary<string, string> ccmails, string mailFrom, string mailFromDisplay,
          string subject, string body, Dictionary<string, string> attachments, string notifyEmail = "", string plaintTextTips = "", bool checkspmail = false)
        {
            Mail_Message msg = new Mail_Message();
            msg.MimeVersion = "1.0";
            msg.MessageID = MIME_Utils.CreateMessageID();
            msg.Date = DateTime.Now;
            msg.Subject = subject;
            msg.From = new Mail_t_MailboxList();
            msg.From.Add(new Mail_t_Mailbox(mailFromDisplay, mailFrom));
            msg.To = new Mail_t_AddressList();
            foreach (string address in tomails.Keys)
            {
                string displayName = tomails[address];
                msg.To.Add(new Mail_t_Mailbox(displayName, address));
            }
            msg.Cc = new Mail_t_AddressList();
            foreach (string address in ccmails.Keys)
            {
                string displayName = ccmails[address];
                msg.Cc.Add(new Mail_t_Mailbox(displayName, address));
            }

            
            if (!string.IsNullOrEmpty(notifyEmail))
            {
                msg.DispositionNotificationTo.Add(new Mail_t_Mailbox(notifyEmail, notifyEmail));
            }

            #region MyRegion

            
            MIME_h_ContentType contentType_multipartMixed = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed);
            contentType_multipartMixed.Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.');
            MIME_b_MultipartMixed multipartMixed = new MIME_b_MultipartMixed(contentType_multipartMixed);
            msg.Body = multipartMixed;

            
            MIME_Entity entity_multipartAlternative = new MIME_Entity();
            MIME_h_ContentType contentType_multipartAlternative = new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative);
            contentType_multipartAlternative.Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.');
            MIME_b_MultipartAlternative multipartAlternative = new MIME_b_MultipartAlternative(contentType_multipartAlternative);
            entity_multipartAlternative.Body = multipartAlternative;
            multipartMixed.BodyParts.Add(entity_multipartAlternative);

            
            MIME_Entity entity_text_plain = new MIME_Entity();
            MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
            entity_text_plain.Body = text_plain;

            
            string plainTextBody = "如果你邮件客户端不支持HTML格式，或者你切换到“普通文本”视图，将看到此内容";
            if (!string.IsNullOrEmpty(plaintTextTips))
            {
                plainTextBody = plaintTextTips;
            }

            text_plain.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, plainTextBody);
            multipartAlternative.BodyParts.Add(entity_text_plain);

            
            string htmlText = body;
            MIME_Entity entity_text_html = new MIME_Entity();
            MIME_b_Text text_html = new MIME_b_Text(MIME_MediaTypes.Text.html);
            entity_text_html.Body = text_html;
            text_html.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, htmlText);
            multipartAlternative.BodyParts.Add(entity_text_html);

            var i = 1;

            
            foreach (string attach in attachments.Keys)
            {

                string filename = string.Empty;
                FileInfo fino = new FileInfo(attach);
                if (mAttRename)
                {
                    filename = mAttachmentName + i.ToString() + fino.Extension;
                }
                else
                {
                    if (!string.IsNullOrEmpty(attachments[attach]))
                        filename = attachments[attach];
                    else
                    {

                        filename = fino.Name;
                    }
                }
                
                if (filename.Length > 30)
                {
                    FileInfo aainfo = new FileInfo(filename);
                    filename = filename.Substring(0, 15) + "_略" + aainfo.Extension;
                }

                
                
                filename = MimeUtils.EncodeWord(filename);
                

                using (var fs = File.OpenRead(attach))
                {
                    var ate = Mail_Message.CreateAttachment(fs, filename);
                    multipartAlternative.BodyParts.Add(ate);
                    fs.Close();
                }

            }
            #endregion

            return msg;
        }

        string mAttachmentName = "Attachment";
        public string AttachmentName { get { return mAttachmentName; } set { mAttachmentName = value; } }

        string mSpecialMailMark = ConfigurationManager.AppSettings["SpecialMailMark"] ?? "(qq)|(gtec.com.cn)|(intasect.com.cn)";
        public string SpecialMailMark { get { return mSpecialMailMark; } set { mSpecialMailMark = value; } }

        bool mAttRename = false;
        public bool AttachmentRename { get { return mAttRename; } set { mAttRename = value; } }

        bool mIsExchangeServer = ConfigurationManager.AppSettings["IsExchangeServer"] != null ? (ConfigurationManager.AppSettings["IsExchangeServer"] == "1") : false;
        
        

        
        public bool IsExchangeServer { get { return mIsExchangeServer; } set { mIsExchangeServer = value; } }

        
        
        
        
        
        public bool CheckSpecialMail(string[] toList)
        {
            foreach (var to in toList)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(to, mSpecialMailMark, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckSpecialMail(Dictionary<string, string> toList)
        {
            foreach (var to in toList.Keys)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(to, mSpecialMailMark, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
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
}

#region 附加内容

#region POP3 命令简介



#endregion

#endregion