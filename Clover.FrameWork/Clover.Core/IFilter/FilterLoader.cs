using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Clover.Core.IFilter
{
  
  
  
  
  
  
  static class FilterLoader
  {
    #region CacheEntry
    private class CacheEntry
    {
      public string DllName;
      public string ClassName;

      public CacheEntry(string dllName, string className)
      {
        DllName=dllName;
        ClassName=className;
      }
    }
    #endregion

    static Dictionary<string, CacheEntry> _cache=new Dictionary<string, CacheEntry>();

    #region Registry Read String helper
    static string ReadStrFromHKLM(string key)
    {
      return ReadStrFromHKLM(key,null);
    }
    static string ReadStrFromHKLM(string key, string value)
    {
      RegistryKey rk=Registry.LocalMachine.OpenSubKey(key);
      if (rk==null)
        return null;

      using (rk)
      {
        return (string)rk.GetValue(value);
      }
    }
    #endregion

    
    
    
    
    
    private static IFilter LoadIFilter(string ext)
    {
      string dllName, filterPersistClass;

      
      if (GetFilterDllAndClass(ext, out dllName, out filterPersistClass))
      {
        
        return LoadFilterFromDll(dllName, filterPersistClass);
      }
      return null;
    }

    internal static IFilter LoadAndInitIFilter(string fileName)
    {
      return LoadAndInitIFilter(fileName,Path.GetExtension(fileName));
    }

    internal static IFilter LoadAndInitIFilter(string fileName, string extension)
    {
      IFilter filter=LoadIFilter(extension);
      if (filter==null)
        return null;

      IPersistFile persistFile=(filter as IPersistFile);
      if (persistFile!=null)
      {
        persistFile.Load(fileName, 0);
        IFILTER_FLAGS flags;
        IFILTER_INIT iflags =
					IFILTER_INIT.CANON_HYPHENS |
					IFILTER_INIT.CANON_PARAGRAPHS |
					IFILTER_INIT.CANON_SPACES |
					IFILTER_INIT.APPLY_INDEX_ATTRIBUTES |
					IFILTER_INIT.HARD_LINE_BREAKS |
					IFILTER_INIT.FILTER_OWNED_VALUE_OK;

        if (filter.Init(iflags, 0, IntPtr.Zero, out flags)==IFilterReturnCode.S_OK)
          return filter;
      }
      
      
      Marshal.ReleaseComObject(filter);
      return null;
    }

    private static IFilter LoadFilterFromDll(string dllName, string filterPersistClass)
    {
      
      IClassFactory classFactory=ComHelper.GetClassFactory(dllName, filterPersistClass);
      if (classFactory==null)
        return null;

      
      Guid IFilterGUID=new Guid("89BCB740-6119-101A-BCB7-00DD010655AF");
      Object obj;
      classFactory.CreateInstance(null, ref IFilterGUID, out obj);
      return (obj as IFilter);
    }

    private static bool GetFilterDllAndClass(string ext, out string dllName, out string filterPersistClass)
    {
      if (!GetFilterDllAndClassFromCache(ext, out dllName, out filterPersistClass))
      {
        string persistentHandlerClass;

        persistentHandlerClass=GetPersistentHandlerClass(ext,true);
        if (persistentHandlerClass!=null)
        {
          GetFilterDllAndClassFromPersistentHandler(persistentHandlerClass,
            out dllName, out filterPersistClass);
        }
        AddExtensionToCache(ext, dllName, filterPersistClass);
      }
      return (dllName!=null && filterPersistClass!=null); 
    }

    private static void AddExtensionToCache(string ext, string dllName, string filterPersistClass)
    {
      lock (_cache)
      {
        _cache.Add(ext.ToLower(), new CacheEntry(dllName, filterPersistClass));
      }
    }

    private static bool GetFilterDllAndClassFromPersistentHandler(string persistentHandlerClass, out string dllName, out string filterPersistClass)
    {
      dllName=null;
      filterPersistClass=null;

      
      filterPersistClass=ReadStrFromHKLM(@"Software\Classes\CLSID\" + persistentHandlerClass + 
        @"\PersistentAddinsRegistered\{89BCB740-6119-101A-BCB7-00DD010655AF}");
      if (String.IsNullOrEmpty(filterPersistClass))
          return false;

      
      dllName=ReadStrFromHKLM(@"Software\Classes\CLSID\" + filterPersistClass + 
        @"\InprocServer32");
      return (!String.IsNullOrEmpty(dllName));
    }

    private static string GetPersistentHandlerClass(string ext, bool searchContentType)
    {
      
      string persistentHandlerClass=GetPersistentHandlerClassFromExtension(ext);
      if (String.IsNullOrEmpty(persistentHandlerClass))
        
        persistentHandlerClass=GetPersistentHandlerClassFromDocumentType(ext);
      if (searchContentType && String.IsNullOrEmpty(persistentHandlerClass))
        
        persistentHandlerClass=GetPersistentHandlerClassFromContentType(ext);
      return persistentHandlerClass;
    }

    private static string GetPersistentHandlerClassFromContentType(string ext)
    {
      string contentType=ReadStrFromHKLM(@"Software\Classes\"+ext,"Content Type");
      if (String.IsNullOrEmpty(contentType))
        return null;
      
      string contentTypeExtension=ReadStrFromHKLM(@"Software\Classes\MIME\Database\Content Type\"+contentType,
          "Extension");
      if (ext.Equals(contentTypeExtension, StringComparison.CurrentCultureIgnoreCase))
        return null; 
    
      
      return GetPersistentHandlerClass(contentTypeExtension, false); 
    }

    private static string GetPersistentHandlerClassFromDocumentType(string ext)
    {
      
      string docType=ReadStrFromHKLM(@"Software\Classes\"+ext);
      if (String.IsNullOrEmpty(docType))
        return null;
      
      
      string docClass=ReadStrFromHKLM(@"Software\Classes\" + docType + @"\CLSID");
      if (String.IsNullOrEmpty(docType))
        return null;

      
      return ReadStrFromHKLM(@"Software\Classes\CLSID\" + docClass + @"\PersistentHandler");
    }

    private static string GetPersistentHandlerClassFromExtension(string ext)
    {
      return ReadStrFromHKLM(@"Software\Classes\"+ext+@"\PersistentHandler");
    }

    private static bool GetFilterDllAndClassFromCache(string ext, out string dllName, out string filterPersistClass)
    {
      string lowerExt=ext.ToLower();
      lock (_cache)
      {
        CacheEntry cacheEntry;
        if (_cache.TryGetValue(lowerExt, out cacheEntry))
        {
          dllName=cacheEntry.DllName;
          filterPersistClass=cacheEntry.ClassName;
          return true;
        }
      }
      dllName=null;
      filterPersistClass=null;
      return false;
    }
  }
}
