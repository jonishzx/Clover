using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Clover.Net.Mail;

namespace Clover.Net
{
    
    
    
    
    public class MimeContent : InternetTextMessage
    {
        private List<MimeContent> _Contents = null;
        
        
        
        
        
        public List<MimeContent> Contents
        {
            get { return this._Contents; }
        }
		
		
		
        public MimeContent():
            base()
        {
            this.Initialize("");
        }
		
		
		
		
        public MimeContent(String text):
            base(text)
        {
            this.Initialize(text);
        }
        private void Initialize(String text)
        {
            this._Contents = new List<MimeContent>();
            if (this.IsMultiPart == true)
            {
                List<String> l = MimeContent.ParseToContentTextList(this.BodyData, this.MultiPartBoundary);
                for (int i = 0; i < l.Count; i++)
                {
                    this._Contents.Add(new MimeContent(l[i]));
                }
            }
        }
        
        
        
        
        
        
        
        public static List<String> ParseToContentTextList(String text, String multiPartBoundary) 
        {
            StringReader sr = null;
            StringBuilder sb = new StringBuilder();
            String CurrentLine = "";
            String StartOfBoundary = "--" + multiPartBoundary;
            String EndOfBoundary = "--" + multiPartBoundary + "--";
            List<String> l = new List<string>();
            Boolean IsBegin = false;

            using (sr = new StringReader(text))
            {
                while (true)
                {
                    CurrentLine = sr.ReadLine();
                    if (CurrentLine == null)
                    { break; }
                    if (IsBegin == false)
                    {
                        if (CurrentLine == StartOfBoundary)
                        {
                            IsBegin = true;
                            sb.Length = 0;
                        }
                        continue;
                    }
                    if (CurrentLine == StartOfBoundary ||
                        CurrentLine == EndOfBoundary)
                    {
                        if (sb.Length > 0)
                        {
                            l.Add(sb.ToString());
                        }
                        sb.Length = 0;
                        if (CurrentLine == EndOfBoundary)
                        { break; }
                    }
                    else
                    {
                        sb.Append(CurrentLine);
                        sb.Append(MailParser.NewLine);
                    }
                    if (sr.Peek() == -1)
                    {
                        if (IsBegin == true)
                        {
                            l.Add(sb.ToString());
                        }
                        break; 
                    }
                }
            }
            return l;
        }
    }
}
