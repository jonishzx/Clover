using System;
using System.Collections.Generic;
using System.Text;
using Clover.Net.Mail;

namespace Clover.Net
{
    
    
    
    
    public class ContentType : InternetTextMessage.Field
    {
        private List<InternetTextMessage.Field> _Fields = new List<InternetTextMessage.Field>();
        
        
        
        public List<InternetTextMessage.Field> Fields
        {
            get { return this._Fields; }
        }
        
        
        
        public String Name
        {
            get
            {
                InternetTextMessage.Field f = InternetTextMessage.Field.FindField(this._Fields, "Name");
                if (f == null)
                {
                    return "";
                }
				return MailParser.DecodeFromMailHeaderLine(f.Value);
			}
            set
            {
                InternetTextMessage.Field f = InternetTextMessage.Field.FindField(this._Fields, "Name");
                if (f == null)
                {
                    f = new InternetTextMessage.Field("Name", value);
                    this._Fields.Add(f);
                }
                else
                {
                    f.Value = value;
                }
            }
        }
        
        
        
        public String Boundary
        {
            get
            {
                InternetTextMessage.Field f = InternetTextMessage.Field.FindField(this._Fields, "Boundary");
                if (f == null)
                {
                    return "";
                }
                return f.Value;
            }
            set
            {
                InternetTextMessage.Field f = InternetTextMessage.Field.FindField(this._Fields, "Boundary");
                if (f == null)
                {
                    f = new InternetTextMessage.Field("Boundary", value);
                    this._Fields.Add(f);
                }
                else
                {
                    f.Value = value;
                }
            }
        }
		
		
		
		
        public ContentType(String value) : 
            base("Content-Type", value)
        {
            this.Value = value;
        }
		
		
		
		
		
        public ContentType(String value, InternetTextMessage.Field[] fields) :
            base("Content-Type", value)
        {
            this.Value = value;
            for (int i = 0; i < fields.Length; i++)
            {
                this._Fields.Add(fields[i]);
            }
        }
    }
}
