using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Clover.Component.Excel.Import;
using System.Web;
using System.IO;
using System.Web.Caching;
using Clover.Component.Excel.Common;
namespace Clover.Component.Excel.Export
{
    public class ExcelExportConfiguration
    {
        #region 字段

        
        
        
        private const string ConfigFilePath = "config/ExcelExport.config";

        
        
        
        public static string CacheKey = "ExcelExportConfig";
        private XmlDocument xmlDoc;
        private ExcelExportProviderCollection _excels;

        
        
        
        public static bool _isActive = false;
        
        
        
        private static ExcelExportProviderCollection _excelsActive = new ExcelExportProviderCollection(); 

        #endregion

        #region 属性

        public ExcelExportProviderCollection ExcelConfigs
        {
            get {
                _isActive = false;
                return _excels; 
            }
        }

        public static bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public static ExcelExportProviderCollection ExcelConfigsActive
        {
            get {
                _isActive = true;
                return _excelsActive; 
            }
        }

        #endregion

        #region 构造方法

        public ExcelExportConfiguration() { }

        public ExcelExportConfiguration(XmlDocument xdoc)
        {
            xmlDoc = xdoc;
            _excels = new ExcelExportProviderCollection();
            Initialize();
        }

        private static ExcelExportConfiguration config = null;
        private static object lck = new object();

        public static void Clear()
        {
            config = null;
        }

        
        
        
        
        public static ExcelExportConfiguration Current
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
                            config = new ExcelExportConfiguration(doc);
                        }
                    }
                }
                return config;
            }
        }

        #endregion

        #region 方法

        private void Initialize()
        {
            XmlNode rootNode = GetConfigSection("/ExportConfigs");
            foreach (XmlNode childNode in rootNode.ChildNodes)
            {
                switch (childNode.Name.ToLower())
                {
                    case "excel":
                        if (!string.IsNullOrEmpty(XmlUtility.getNodeAttributeStringValue(childNode, "typename")))
                        {
                            ExcelExportProvider providerType = new ExcelExportProvider(childNode);
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
    }
}
