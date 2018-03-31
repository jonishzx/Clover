

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;

namespace Clover.Core.Validate
{
    
    
    
    
    
    
    
    
    
    internal class LangCache
    {
        
        
        
        private static Dictionary<string, string> WordCache = new Dictionary<string, string>();

        
        
        
        private static Dictionary<ValidationLanguageEnum, bool> LoadedLanguagesCache = new Dictionary<ValidationLanguageEnum, bool>();

        
        
        
        private static ReaderWriterLock RWLock = new ReaderWriterLock();

        
        
        
        
        
        
        
        
        
        public static string FetchItem(ValidationLanguageEnum ValidationLanguage, string StringKey)
        {
            string CacheKey = ValidationLanguage.ToString() + ":" + StringKey;
            string CacheEntry;

            
            RWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (LoadedLanguagesCache.Keys.Contains(ValidationLanguage))
                {
                    
                    WordCache.TryGetValue(CacheKey, out CacheEntry);
                    return CacheEntry;
                }
            }
            finally
            {
                RWLock.ReleaseReaderLock();
            }

            
            
            RWLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                
                if (LoadedLanguagesCache.Keys.Contains(ValidationLanguage))
                {
                    
                    WordCache.TryGetValue(CacheKey, out CacheEntry);
                    return CacheEntry;
                }

                
                LoadLanguageDefinition(ValidationLanguage);

                
                WordCache.TryGetValue(CacheKey, out CacheEntry);
                return CacheEntry;
            }
            finally
            {
                RWLock.ReleaseLock();
            }
        }

        
        
        
        
        
        
        
        private static void LoadLanguageDefinition(ValidationLanguageEnum ValidationLanguage)
        {
            
            var Type = ValidationLanguage.GetType();
            var FieldInfo = Type.GetField(ValidationLanguage.ToString());
            var Attribs = FieldInfo.GetCustomAttributes(typeof(LanguageResourceFile), false) as LanguageResourceFile[];
            if (Attribs.Length != 1)
                throw new Exception("No language specified for Validation Language " + ValidationLanguage.ToString());
            var ResourceFileName = Attribs[0].ResourceFile;

            
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream LanguageStream = a.GetManifestResourceStream(ResourceFileName))
            {
                if (LanguageStream != null)
                {
                    StreamReader sr = new StreamReader(LanguageStream);
                    XDocument LangDefXMLDoc = XDocument.Load(sr);

                    
                    var Entries = (from E in LangDefXMLDoc.Elements().Elements()
                                   select E).ToList();

                    foreach (var entry in Entries)
                    {
                        string EntryName = entry.Attribute("name").Value.ToString().Trim();
                        string CacheKey = ValidationLanguage.ToString() + ":" + EntryName;
                        if (!WordCache.Keys.Contains(CacheKey))
                            WordCache.Add(CacheKey, entry.Value.Trim());
                        else
                            throw new ApplicationException("Duplicate '" + EntryName +
                                                           "' entry in the language definition xml file '" +
                                                           ResourceFileName + "'");
                    }

                    
                    LoadedLanguagesCache.Add(ValidationLanguage, true);
                }
            }
        }
    }
}
