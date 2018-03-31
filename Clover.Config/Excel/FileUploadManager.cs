using System;
using System.Text;
using System.Web;
using System.IO;

using Clover.Core.Configuration;
using Clover.Config.FileUpload;

namespace Clover.Config.FileUpload
{

    /// <summary>
    /// 基本设置信息管理类
    /// </summary>
    public sealed class FileUploadManager : DefaultConfigFileManager<FilesUploadInfos>
    {
         /// <summary>
        /// 默认配置文件所在路径
        /// </summary>
        const string CONST_DEFAULTPATH = "~/config/FileUpload.config";

         /// <summary>
        /// 初始化文件修改时间和对象实例
        /// </summary>
        public FileUploadManager()
            : this(CONST_DEFAULTPATH)
        {            
        }

        
        /// <summary>
        /// 初始化文件修改时间和对象实例
        /// </summary>
        public FileUploadManager(string configfilepath)
            : base(configfilepath)
        {
          
        }     
    }
}
