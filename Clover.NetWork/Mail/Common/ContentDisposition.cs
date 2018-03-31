using System;
using System.Collections.Generic;
using System.Text;
using Clover.Net.Mail;

namespace Clover.Net
{
	
	
	
	
	public class ContentDisposition : InternetTextMessage.Field
	{
		private List<InternetTextMessage.Field> _Fields = new List<InternetTextMessage.Field>();
		
		
		
		public List<InternetTextMessage.Field> Fields
		{
			get { return this._Fields; }
		}
		
		
		
		public String FileName
		{
			get
			{
				InternetTextMessage.Field f = InternetTextMessage.Field.FindField(this._Fields, "FileName");
				if (f == null)
				{
					return "";
				}
				return MailParser.DecodeFromMailHeaderLine(f.Value);
			}
			set
			{
				InternetTextMessage.Field f = InternetTextMessage.Field.FindField(this._Fields, "FileName");
				if (f == null)
				{
					f = new InternetTextMessage.Field("FileName", value);
					this._Fields.Add(f);
				}
				else
				{
					f.Value = value;
				}
				this.Value = "attachment";
			}
		}
		
		
		
		
		public ContentDisposition(String value) :
			base("Content-Disposition", value)
		{
			this.Value = value;
		}
		
		
		
		
		
		public ContentDisposition(String value, InternetTextMessage.Field[] fields) :
			base("Content-Disposition", value)
		{
			this.Value = value;
			for (int i = 0; i < fields.Length; i++)
			{
				this._Fields.Add(fields[i]);
			}
		}
	}
}
