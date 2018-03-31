using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;


namespace Clover.Config.Excel
{
    /// <summary>
    /// 导出配置信息
    /// </summary>
    public class ExportInfo
    {
        /// <summary>
        /// 每个上传设置的标记
        /// </summary>
        [XmlAttribute("Id")]
        public String Id = "";

        /// <summary>
        /// 文件最大上传大小
        /// </summary>
        [XmlAttribute("MaxFileSize")]
        public int MaxFileSize = 0;
      
        /// <summary>
        /// 接收路径
        /// </summary>
        [XmlElement("Path")]
        public String Path;

        /// <summary>
        /// 缩略图路径
        /// </summary>
        [XmlElement("ThumbnailPath")]
        public String ThumbnailPath;

        /// <summary>
        /// 缩略图宽度
        /// </summary>
        [XmlElement("ThumbnailWidth")]
        public int ThumbnailWidth;

        /// <summary>
        /// 缩略图路径长度
        /// </summary>
        [XmlElement("ThumbnailHeight")]
        public int ThumbnailHeight;

        /// <summary>
        /// 允许上传的格式
        /// </summary>
        [XmlElement("ExtAllowed")]
        public String ExtAllowed;

          /// <summary>
        /// 文件存放的临时目录
        /// </summary>
        [XmlElement("TempPath")]
        public String TempPath;
        
        /// <summary>
        /// 允许上传的文件格式列表
        /// </summary>
        [XmlIgnore]
        public List<string> ExtAllowedList
        {
            get {
                return new List<string>(ExtAllowed.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        /// <summary>
        /// 文件夹内允许的最大文件数量
        /// </summary>
        [XmlElement("MaxFilesCountInDir")]
        public int MaxFilesCountInDir;

        /// <summary>
        /// 当前文件批次头
        /// </summary>
        [XmlElement("CurrBatchHead")]
        public String CurrBatchHead;


        /// <summary>
        /// 获取可以上传文件缩略图文件夹路径
        /// </summary>
        /// <returns></returns>
        public string getThumbUploadPath()
        {
            return CreateBatchDir(ThumbnailPath);
        }


        /// <summary>
        /// 获取可以上传文件的文件夹路径
        /// </summary>
        /// <returns></returns>
        public string getUploadPath()
        {
            return CreateBatchDir(Path);    
        }

        private string CreateBatchDir(string basepath)
        {
            string rootpath = basepath;

            if (basepath.IndexOf("~") >= 0)
            {
                rootpath = HttpContext.Current.Server.MapPath(basepath);
            }

            //获取上传的文件路径
            string filepath = System.IO.Path.Combine(rootpath, CurrBatchHead);
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
            else
            {
                //判断文件数是否大于指定的文件数量
                int count = 1;

                while ((Directory.Exists(filepath) && Directory.GetFiles(filepath).Length >= MaxFilesCountInDir)) //存在文件夹，但文件数已经超过数量
                {
                    count++;
                    filepath = System.IO.Path.Combine(rootpath, CurrBatchHead + count.ToString());
                }

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                CurrBatchHead += count.ToString();    
            }

            return filepath;
        }
    }
}
