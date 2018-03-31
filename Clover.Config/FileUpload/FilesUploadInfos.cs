using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Clover.Core.Configuration;


namespace Clover.Config.FileUpload
{
    
    
    
    public class FilesUploadInfos : IConfigInfo
    {

        private List<FilesUploadInfo> paramslist = new List<FilesUploadInfo>();
        
        
        
        [XmlElement("FilesUploadInfo")]
        public FilesUploadInfo[] FilesUploadInfo
        {
            get
            {
                return paramslist.ToArray();
            }
            set
            {
                if (value == null)
                    return;

                paramslist.AddRange(value);
            }
        }


        
        
        
        
        public FilesUploadInfo getFieldItem(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            return paramslist.FirstOrDefault(param => param.Id == id);
        }
    }
}
