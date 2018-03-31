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
        /// 列id
        /// </summary>
        [XmlAttribute("id")]
        public String id = "";

        /// <summary>
        /// 第几列填充
        /// </summary>
        [XmlAttribute("offset")]
        public int offset = 0;
      
        /// <summary>
        /// 字段
        /// </summary>
        [XmlAttribute("field")]
        public String field;

        /// <summary>
        /// 数据类型(String,Numeric,DateTime)
        /// </summary>
        [XmlAttribute("datatype")]
        public String datatype;

        /// <summary>
        /// 默认值
        /// </summary>
        [XmlAttribute("defaultvalue")]
        public int defaultvalue;

        /// <summary>
        /// 列的标题
        /// </summary>
        [XmlAttribute("title")]
        public int title;

        /// <summary>
        /// 验证的正则表达式
        /// </summary>
        [XmlAttribute("validator")]
        public String validator;

    }
}
