using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml.Serialization;


namespace Clover.Config.FileUpload
{
    
    
    
    
    
    
    public class FilesUploadInfo
    {
        
        
        
        [XmlAttribute("Id")]
        public String Id = "";

        
        
        
        [XmlAttribute("MaxFileSize")]
        public int MaxFileSize = 0;
      
        
        
        
        [XmlElement("Path")]
        public String Path;

        
        
        
        [XmlElement("ThumbnailPath")]
        public String ThumbnailPath;

        
        
        
        [XmlElement("ThumbnailWidth")]
        public int ThumbnailWidth;

        
        
        
        [XmlElement("ThumbnailHeight")]
        public int ThumbnailHeight;

        
        
        
        [XmlElement("ExtAllowed")]
        public String ExtAllowed;

          
        
        
        [XmlElement("TempPath")]
        public String TempPath;
        
        
        
        
        [XmlIgnore]
        public List<string> ExtAllowedList
        {
            get {
                return new List<string>(ExtAllowed.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        
        
        
        [XmlElement("MaxFilesCountInDir")]
        public int MaxFilesCountInDir;

        
        
        
        [XmlElement("CurrBatchHead")]
        public String CurrBatchHead;


        
        
        
        
        public string getThumbUploadPath()
        {
            return CreateBatchDir(ThumbnailPath);
        }


        
        
        
        
        public string getUploadPath()
        {
            return CreateBatchDir(Path);    
        }

        private string CreateBatchDir(string basepath)
        {
            string rootpath = basepath;

            if (basepath.IndexOf("~", System.StringComparison.Ordinal) >= 0)
            {
                rootpath = HttpContext.Current.Server.MapPath(basepath);
            }

            
            string filepath = System.IO.Path.Combine(rootpath, CurrBatchHead);
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
            else
            {
                
                int count = 1;

                while ((Directory.Exists(filepath) && Directory.GetFiles(filepath).Length >= MaxFilesCountInDir)) 
                {
                    count++;
                    filepath = System.IO.Path.Combine(rootpath, CurrBatchHead + count);
                }

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                CurrBatchHead += count.ToString(CultureInfo.InvariantCulture);    
            }

            return filepath;
        }
    }
}
