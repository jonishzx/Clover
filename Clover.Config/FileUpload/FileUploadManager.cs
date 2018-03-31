using Clover.Core.Configuration;

namespace Clover.Config.FileUpload
{

    
    
    
    public sealed class FileUploadManager : DefaultConfigFileManager<FilesUploadInfos>
    {
         
        
        
        const string CONST_DEFAULTPATH = "~/config/FileUpload.config";

         
        
        
        public FileUploadManager()
            : this(CONST_DEFAULTPATH)
        {            
        }

        
        
        
        
        public FileUploadManager(string configfilepath)
            : base(configfilepath)
        {
          
        }     
    }
}
