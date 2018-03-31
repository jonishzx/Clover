using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Clover.Core.Configuration;


namespace Clover.Config.FileUpload
{
    /// <summary>
    /// 解析用户查询配置
    /// </summary>
    public class FilesUploadInfos : IConfigInfo
    {

        private List<FilesUploadInfo> paramslist = new List<FilesUploadInfo>();
        /// <summary>
        /// 运行日程
        /// </summary>
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

       
        /// <summary>
        /// 更具url地址以及开始日期获取任务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="startdate"></param>
        /// <returns></returns>
        public FilesUploadInfo getFieldItem(string id)
        {
         
            foreach (FilesUploadInfo param in paramslist)
            {
                if (param.Id == id)
                {
                    return param;
                }
            }
            return null;
        }
    }
}
