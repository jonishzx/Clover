using Clover.Config.FileUpload;

namespace Clover.Config
{
    
    
    
    public class FileUploadConfig : ConfigCenter<FileUploadManager, FilesUploadInfos>
    {
        
        
        
        
        
        public static FilesUploadInfo GetFUConfig(string key)
        {
            return Config.getFieldItem(key);
        }

    }
}
