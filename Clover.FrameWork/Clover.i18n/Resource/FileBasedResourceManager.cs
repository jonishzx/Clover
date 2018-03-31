
 
 
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Collections;

namespace Clover.I18n
{
    
    
    
    public class FileBasedResourceManager : System.Resources.ResourceManager
    {
        #region Properties

        string path;
        string fileformat;

        
        
        
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        
        
        
        public string FileFormat
        {
            get { return fileformat; }
            set { fileformat = value; }
        }

        #endregion

        #region Notification Events

        
        
        
        public class ResourceSetCreationEventArgs : EventArgs
        {
            
            
            
            public Exception Exception { get; set; }

            
            
            
            public String FileName { get; set; }

            
            
            
            public Type ResourceSetType { get; set; }

            
            
            
            public System.Resources.ResourceSet ResourceSet { get; set; }

            
            
            
            public bool Success { get; set; }
        }

        
        
        
        public event EventHandler<ResourceSetCreationEventArgs> CreatedResourceSet;

        
        
        
        public event EventHandler<ResourceSetCreationEventArgs> FailedResourceSet;

        protected void RaiseCreatedResourceSet(string filename, System.Resources.ResourceSet set)
        {
            var handler = CreatedResourceSet;
            if (handler != null)
            {
                handler(this, new ResourceSetCreationEventArgs
                {
                    FileName = filename,
                    ResourceSet = set,
                    ResourceSetType = this.ResourceSetType,
                    Success = true
                });
            }
        }

        protected void RaiseFailedResourceSet(string filename, Exception ex)
        {
            var handler = FailedResourceSet;
            if (handler != null)
            {
                handler(this, new ResourceSetCreationEventArgs
                {
                    FileName = filename,
                    ResourceSet = null,
                    ResourceSetType = this.ResourceSetType,
                    Success = false,
                    Exception = ex
                });
            }
        }

        #endregion

        
        
        
        
        
        
        public FileBasedResourceManager(string name, string path, string fileformat)
            : base()
        {
            this.path = path;
            this.fileformat = fileformat;
            this.BaseNameField = name;

            base.IgnoreCase = false;
            base.ResourceSets = new System.Collections.Hashtable();
        }

        protected override string GetResourceFileName(System.Globalization.CultureInfo culture)
        {
            return fileformat.Replace("{{culture}}", culture.Name).Replace("{{resource}}", BaseNameField);
        }

        protected new virtual System.Resources.ResourceSet InternalGetResourceSet(System.Globalization.CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            if (path == null && fileformat == null) return null;
            if (culture == null || culture.Equals(CultureInfo.InvariantCulture)) return null;

            System.Resources.ResourceSet rs = null;
            Hashtable resourceSets = this.ResourceSets;

            if (!TryFetchResourceSet(resourceSets, culture, out rs))
            {
                string resourceFileName = this.FindResourceFile(culture);
                if (resourceFileName == null)
                {
                    if (tryParents)
                    {
                        CultureInfo parent = culture.Parent;
                        rs = this.InternalGetResourceSet(parent, createIfNotExists, tryParents);
                        AddResourceSet(resourceSets, culture, ref rs);
                        return rs;
                    }
                }
                else
                {
                    rs = this.CreateResourceSet(resourceFileName);
                    AddResourceSet(resourceSets, culture, ref rs);
                    return rs;
                }
            }

            return rs;
        }

        protected virtual System.Resources.ResourceSet InternalCreateResourceSet(string resourceFileName)
        {
            object[] args = new object[] { resourceFileName };
            return (System.Resources.ResourceSet)Activator.CreateInstance(this.ResourceSetType, args);
        }

        protected System.Resources.ResourceSet CreateResourceSet(string resourceFileName)
        {
            System.Resources.ResourceSet set = null;

            try
            {
                set = InternalCreateResourceSet(resourceFileName);
                RaiseCreatedResourceSet(resourceFileName, set);
            }
            catch (Exception ex)
            {
                RaiseFailedResourceSet(resourceFileName, ex);
            }

            return set;
        }

        private string FindResourceFile(CultureInfo culture)
        {
            string resourceFileName = this.GetResourceFileName(culture);
            string path = this.path ?? String.Empty;

            
            string fullpath = System.IO.Path.Combine(path, resourceFileName);
            if (File.Exists(fullpath)) return fullpath;

            
            if (path == String.Empty || !System.IO.Path.IsPathRooted(path))
            {
                
                if (Assembly.GetEntryAssembly() != null)
                {
                    string dir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
                    fullpath = System.IO.Path.Combine(dir, resourceFileName);
                    if (File.Exists(fullpath)) return fullpath;
                }

                
                if (Assembly.GetExecutingAssembly() != null)
                {
                    if (Assembly.GetEntryAssembly() == null || System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) != System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    {
                        string dir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
                        fullpath = System.IO.Path.Combine(dir, resourceFileName);
                        if (File.Exists(fullpath)) return fullpath;
                    }
                }
            }

            return null;
        }

        protected void AddResourceSet(Hashtable localResourceSets, CultureInfo culture, ref System.Resources.ResourceSet rs)
        {
            lock (localResourceSets)
            {
                if (localResourceSets.Contains(culture))
                {
                    var existing = (System.Resources.ResourceSet)localResourceSets[culture];

                    if (existing != null && !object.Equals(existing, rs))
                    {
                        rs.Dispose();
                        rs = existing;
                        var a = (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationManager.GetSection("appSettings");
                    }
                }
                else
                {
                    localResourceSets.Add(culture, rs);
                }
            }
        }

        protected bool TryFetchResourceSet(Hashtable localResourceSets, CultureInfo culture, out System.Resources.ResourceSet set)
        {
            lock (localResourceSets)
            {
                if (ResourceSets.Contains(culture))
                {
                    set = (System.Resources.ResourceSet)ResourceSets[culture];
                    return true;
                }

                set = null;
                return false;
            }
        }

        private bool ValidateGetResourceSet(CultureInfo culture)
        {
            return !(culture == null || culture.Equals(CultureInfo.InvariantCulture) || String.IsNullOrEmpty(culture.Name));
        }

        #region override method

        
        
        
        
        
        
        
        public override String GetString(String msgid, CultureInfo culture)
        {
            GettextResourceSet rs = InternalGetResourceSet(culture, true, true) as GettextResourceSet;
            {
                String translation = rs.GetString(msgid);
                if (translation != null)
                    return translation;
            }
            
            return msgid;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public virtual String GetPluralString(String msgid, String msgidPlural, long n, CultureInfo culture)
        {
            GettextResourceSet rs = InternalGetResourceSet(culture, true, true) as GettextResourceSet;
            {
                String translation = rs.GetPluralString(msgid, msgidPlural, n);
                if (translation != null)
                    return translation;
            }
            
            return (n == 1 ? msgid : msgidPlural);
        }

        

        
        
        
        
        
        
        
        
        
        
        public virtual String GetParticularString(String msgctxt, String msgid, CultureInfo culture)
        {
            
            GettextResourceSet rs = InternalGetResourceSet(culture, true, true) as GettextResourceSet;
            {
                String translation = rs.GetString(msgid);
                if (translation != null)
                    return translation;
            }
            
            return msgid;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public virtual String GetParticularPluralString(String msgctxt, String msgid, String msgidPlural, long n, CultureInfo culture)
        {
            
            GettextResourceSet rs = InternalGetResourceSet(culture, true, true) as GettextResourceSet;
            {
                String translation = rs.GetPluralString(msgid, msgidPlural, n);
                if (translation != null)
                    return translation;
            }
            
            return (n == 1 ? msgid : msgidPlural);
        }

        
        
        
        
        
        
        
        
        public override String GetString(String msgid)
        {
            return GetString(msgid, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public virtual String GetPluralString(String msgid, String msgidPlural, long n)
        {
            return GetPluralString(msgid, msgidPlural, n, CultureInfo.CurrentUICulture);
        }

        
        
        
        
        
        
        
        
        
        
        public virtual String GetParticularString(String msgctxt, String msgid)
        {
            return GetParticularString(msgctxt, msgid, CultureInfo.CurrentUICulture);
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public virtual String GetParticularPluralString(String msgctxt, String msgid, String msgidPlural, long n)
        {
            return GetParticularPluralString(msgctxt, msgid, msgidPlural, n, CultureInfo.CurrentUICulture);
        }
        #endregion
    }
}
