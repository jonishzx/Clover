





























using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Clover.I18n
{
    public class Strings
    {
		private static Object resourceManLock = new Object();
        private static System.Resources.ResourceManager resourceMan;
        private static System.Globalization.CultureInfo resourceCulture;

        public const string ResourceName = "Strings";

        private static string resourcesDir = GetSetting("ResourcesDir", "Po");
        private static string fileFormat = GetSetting("ResourcesFileFormat", "{{culture}}/{{resource}}.po");
        
        private static string GetSetting(string setting, string defaultValue)
        {
			var section = (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationManager.GetSection("appSettings");
			if (section == null) return defaultValue;
			else return section[setting] ?? defaultValue;
        }

        
        
        
        
        public static string ResourcesDirectory
        {
            get { return resourcesDir; }
            set { resourcesDir = value; }
        }

        
        
        
        public static string FileFormat
        {
            get { return fileFormat; }
            set { fileFormat = value; }
        }


        
        
        
        public static System.Resources.ResourceManager ResourceManager
        {
            get
            {
            
                if (object.ReferenceEquals(resourceMan, null))
                {
					lock (resourceManLock) 
					{
					    if (object.ReferenceEquals(resourceMan, null))
		                {
							var directory = resourcesDir;
								var mgr = new global::Clover.I18n.GettextResourceManager(ResourceName, directory, fileFormat);
								resourceMan = mgr;
						}
					}
                }
                
                return resourceMan;
            }
        }

        
        
        
        
        public static System.Globalization.CultureInfo Culture
        {
            get { return resourceCulture; }
            set { resourceCulture = value; }
        }

        
        
        
        public static string T(string t)
        {
            return T(null, t);
        }

		
		
        
        
        public static string TP(CultureInfo info, string t, long n)
        {
            if (String.IsNullOrEmpty(t)) return t;
            var mgr = ResourceManager as Clover.I18n.FileBasedResourceManager;
            var translated = mgr.GetPluralString(t, null, n, info ?? resourceCulture);
            return String.IsNullOrEmpty(translated) ? t : translated;
        }   
        
          
        
        
        public static string TP(CultureInfo info, string t, long n, params object[] parameters)
        {
            if (String.IsNullOrEmpty(t)) return t;
            return String.Format(TP(info, t, n), parameters);
        }     	
                
        
        
        
        public static string T(CultureInfo info, string t)
        {
            if (String.IsNullOrEmpty(t)) return t;
            var translated = ResourceManager.GetString(t, info ?? resourceCulture);
            return String.IsNullOrEmpty(translated) ? t : translated;
        }

        
        
        
        public static string T(string t, params object[] parameters)
        {
            return T(null, t, parameters);
        }

        
        
        
        public static string T(CultureInfo info, string t, params object[] parameters)
        {
            if (String.IsNullOrEmpty(t)) return t;
            return String.Format(T(info, t), parameters);
        }

        
        
        
        public static string M(string t)
        {
            return t;
        }
        
        
        
        
        public static System.Resources.ResourceSet GetResourceSet(CultureInfo culture)
        {
            return ResourceManager.GetResourceSet(culture, true, true);
        }
    }
}
 
