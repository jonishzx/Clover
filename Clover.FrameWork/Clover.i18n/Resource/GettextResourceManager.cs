
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Configuration;
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Collections;

namespace Clover.I18n
{
    
    
    
    public class GettextResourceManager : FileBasedResourceManager
    {
        #region Defaults

        
        
        
        const string defaultFileFormat = "{{culture}}\\{{resource}}.po";
        const string defaultPath = "";

        #endregion

        #region 属性

        
        
        
        public override Type ResourceSetType
        {
            get { return typeof(GettextResourceSet); }
        }

        #endregion

        #region Constructos

        
        
        
        
        
        
        public GettextResourceManager(string name, string path, string fileformat)
            : base(name, path, fileformat)
        {
        }

        
        
        
        
        public GettextResourceManager(string name)
            : base(name, defaultPath, defaultFileFormat)
        {
        }

        #endregion

        #region 配置

        
        
        
        
        
        public bool LoadConfiguration(string section)
        {
            var config = ConfigurationManager.GetSection(section) as NameValueCollection;

            if (config == null) return false;
            
            this.FileFormat = config["fileformat"] ?? FileFormat;
            this.Path = config["path"] ?? Path;
            
            return true;
        }

        
        
        
        
        
        
        public static FileBasedResourceManager CreateFromConfiguration(string name, string section)
        {
            return CreateFromConfiguration(name, section, defaultFileFormat, defaultPath);
        }

        
        
        
        
        
        
        
        
        public static FileBasedResourceManager CreateFromConfiguration(string name, string section, string fallbackFileFormat, string fallbackPath)
        {
            var config = ConfigurationManager.GetSection(section) as NameValueCollection;

            string fileformat = null;
            string path = null;

            if (config == null)
            {
                fileformat = fallbackFileFormat;
                path = fallbackPath;
            }
            else
            {
                fileformat = config["fileformat"] ?? fallbackFileFormat;
                path = config["path"] ?? fallbackPath;
            }

            return new FileBasedResourceManager(name, path, fileformat);
        }

        #endregion

    }

    
}
