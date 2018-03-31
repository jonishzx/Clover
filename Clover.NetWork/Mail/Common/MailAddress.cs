using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Clover.Net.Smtp;

namespace Clover.Net.Mail
{
    
    
    
    
    public class MailAddress
    {
		private class RegexList
		{
			public static Regex DisplayName_MailAddress = new Regex("(?<DisplayName>.*)<(?<MailAddress>[^>]*)>");
			public static Regex MailAddressWithBracket = new Regex("<(?<MailAddress>[^>]*)>");
		}
        private String _Value = "";
        private String _DisplayName = "";
		private String _UserName = "";
		private String _DomainName = "";
        private Boolean _IsDoubleQuote = false;
        private Encoding _Encoding = Encoding.ASCII;
        private TransferEncoding _TransferEncoding = TransferEncoding.Base64;
        
        
        
        
        
        public String Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }
        
        
        
        
        public String DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }
		
		
		
		
		public String UserName
		{
			get { return this._UserName; }
			set { this._UserName = value; }
		}
		
		
		
		
		public String DomainName
		{
			get { return this._DomainName; }
			set { this._DomainName = value; }
		}
		
        
        
        
        public Boolean IsDoubleQuote
        {
            get { return this._IsDoubleQuote; }
            set { this._IsDoubleQuote = value; }
        }
        
        
        
        
        public Encoding Encoding
        {
            get { return this._Encoding; }
            set { this._Encoding = value; }
        }
        
        
        
        
        public TransferEncoding TransferEncoding
        {
            get { return this._TransferEncoding; }
            set { this._TransferEncoding = value; }
        }
		
		
		
		
        public MailAddress(String mailAddress)
        {
            if (String.IsNullOrEmpty(mailAddress) == true)
            { throw new FormatException(); }
			if (mailAddress.Contains("@") == false)
			{ throw new FormatException("Mail address must be contain @ char."); }

			Match m = RegexList.MailAddressWithBracket.Match(mailAddress);
			if (m.Success == true)
			{
				this._Value = m.Groups["MailAddress"].Value;
			}
			else
			{
				this._Value = mailAddress;
			}
			String[] ss = _Value.Split('@');
			this._UserName = ss[0];
			this._DomainName = ss[1];
            this.InitializeProperty();
        }
		
		
		
		
		
        public MailAddress(String mailAddress, String displayName) : 
			this(mailAddress)
        {
            this._DisplayName = displayName;
        }
        private void InitializeProperty()
        {
            if (CultureInfo.CurrentCulture.Name.StartsWith("ja") == true)
            {
                this.Encoding = Encoding.GetEncoding("iso-2022-jp");
                this.TransferEncoding = TransferEncoding.Base64;
            }
        }
		
		
		
		
        public override string ToString()
        {
            if (String.IsNullOrEmpty(this._DisplayName) == true)
            {
                return String.Format("<{0}>", this._Value);
            }
            if (this._IsDoubleQuote == true)
            {
                return String.Format("\"{0}\" <{1}>", this._DisplayName, this._Value);
            }
            else
            {
                return String.Format("{0} <{1}>", this._DisplayName, this._Value);
            }
        }
		
		
		
		
        public String ToEncodeString()
        {
            return MailAddress.ToMailAddressText(this._Encoding, this._TransferEncoding
                , this._Value, this._DisplayName, this._IsDoubleQuote);
        }
        
        
        
        
        
        
        
        
        
        public static String ToMailAddressText(String mailAddress, String displayName, Boolean doubleQuote)
        {
            if (CultureInfo.CurrentCulture.Name.StartsWith("ja") == true)
            {
                return MailAddress.ToMailAddressText(Encoding.GetEncoding("iso-2022-jp"), TransferEncoding.Base64
                    , mailAddress, displayName, doubleQuote);
            }
            return MailAddress.ToMailAddressText(Encoding.ASCII, TransferEncoding.Base64, mailAddress, displayName, doubleQuote);
        }
        
        
        
        
        
        
        
        
        
        
        
        public static String ToMailAddressText(Encoding encoding, TransferEncoding transferEncoding
            , String mailAddress, String displayName, Boolean doubleQuote)
        {
            if (String.IsNullOrEmpty(displayName) == true)
            {
                return mailAddress;
            }
            else
            {
                if (doubleQuote == true)
                {
                    return String.Format("\"{0}\" <{1}>", displayName, mailAddress);
                }
                else
                {
                    return String.Format("{0} <{1}>"
                        , MailParser.EncodeToMailHeaderLine(displayName, transferEncoding, encoding, MailParser.MaxCharCountPerRow - mailAddress.Length - 3)
                        , mailAddress);
                }
            }
        }
        
        
        
        
        
        
        
        
        
        public static MailAddress Create(String mailAddress)
        {
            Regex rx = RegexList.DisplayName_MailAddress;
            Match m = null;

            m = rx.Match(mailAddress);
            if (String.IsNullOrEmpty(m.Value) == true)
            {
                rx = RegexList.MailAddressWithBracket;
                m = rx.Match(mailAddress);
                if (String.IsNullOrEmpty(m.Value) == true)
                {
                    return new MailAddress(mailAddress);
                }
                else
                {
                    return new MailAddress(m.Groups["MailAddress"].Value);
                }
            }
            else
            {
				if (String.IsNullOrEmpty(m.Groups["DisplayName"].Value) == true)
				{
					return new MailAddress(mailAddress);
				}
				else
				{
					return new MailAddress(m.Groups["MailAddress"].Value, m.Groups["DisplayName"].Value.TrimEnd(' '));
				}
            }
        }
		
		
		
		
		
		
		
		
		public static MailAddress TryCreate(String mailAddress)
		{
			try
			{
				if (String.IsNullOrEmpty(mailAddress) == true)
				{ return null; }
				if (mailAddress.Contains("@") == false)
				{ return null; }
				return MailAddress.Create(mailAddress);
			}
			catch { }
			return null;
		}
		
        
        
        
        
        
        
        public static List<MailAddress> CreateMailAddressList(String mailAddressListText)
        {
            List<MailAddress> l = new List<MailAddress>();
			MailAddress m = null;
            String[] ss = null;

            ss = mailAddressListText.Split(',');
            for (int i = 0; i < ss.Length; i++)
            {
				m = MailAddress.TryCreate(ss[i].Trim());
				if (m == null)
				{ continue; }
                l.Add(m);
            }
            return l;
        }
    }
}
