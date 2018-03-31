using System;
using System.Xml;

using System.Web;
using System.IO;
using System.Web.Caching;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Import
{
    public class ExcelImportConfiguration
    {
        #region Fields

        
        
        
        private const string ConfigFilePath = "config/ExcelImport.config";

        
        
        
        public static string CacheKey = "ExcelImportConfig";
        private XmlDocument xmlDoc;
        private ExcelImportProviderCollection _excels;
        #endregion

        #region 构造方法
        static ExcelImportConfiguration()
        {
        }

        private ExcelImportConfiguration(XmlDocument doc)
        {
            this.xmlDoc = doc;
            this._excels = new ExcelImportProviderCollection();
            this.Initialize();
        }
        #endregion

        #region 方法定义
        private void Initialize()
        {
            XmlNode rootNode = this.GetConfigSection("/ImportConfigs");
            foreach (XmlNode childNode in rootNode.ChildNodes)
            {
                switch (childNode.Name.ToLower())
                {
                    case "excel":
                        
                        if (!string.IsNullOrEmpty(XmlUtility.getNodeAttributeStringValue(childNode, "typename")))
                        {
                            ExcelImportProvider providerType = new ExcelImportProvider(childNode);
                            _excels.Add(providerType);
                        }
                        break;
                }
            }
        }

        
        
        
        
        
        public XmlNode GetConfigSection(String nodePath)
        {
            return this.xmlDoc.SelectSingleNode(nodePath);
        }
        #endregion

        #region 属性定义
        public ExcelImportProviderCollection ExcelConfigs
        {
            get { return _excels; }
        }

        private static ExcelImportConfiguration config = null;
        private static object lck = new object();

        public static void Clear()
        {
            config = null;
        }
        
        
        
        
        public static ExcelImportConfiguration Current
        {
            get
            {                
                if (config == null)
                {
                    lock (lck)
                    {
                        if (config == null)
                        {
                            String path = null;
                            XmlDocument doc = new XmlDocument();
                            HttpContext context = HttpContext.Current;
                            if (context != null)
                            {
                                path = context.Server.MapPath("~/" + ConfigFilePath);
                            }
                            else
                            {
                                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath);
                            }
                            doc.Load(path);
                            config = new ExcelImportConfiguration(doc);
                        }
                    }
                }
                return config;
            }
        }
        #endregion
    }
}
