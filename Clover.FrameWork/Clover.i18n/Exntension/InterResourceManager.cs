
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Collections;

namespace Clover.I18n
{
    
    
    
    public class InterResourceManager : FileBasedResourceManager
    {
       
        #region Notification Events

        
        
        
        public new class ResourceSetCreationEventArgs : EventArgs
        {
            
            
            
            public Exception Exception { get; set; }

            
            
            
            public Stream ResourceStream { get; set; }

            
            
            
            public Type ResourceSetType { get; set; }

            
            
            
            public System.Resources.ResourceSet ResourceSet { get; set; }

            
            
            
            public bool Success { get; set; }
        }

        
        
        
        public new event EventHandler<ResourceSetCreationEventArgs> CreatedResourceSet;

        
        
        
        public new event EventHandler<ResourceSetCreationEventArgs> FailedResourceSet;

        protected void RaiseCreatedResourceSet(Stream resourcestream, System.Resources.ResourceSet set)
        {
            var handler = CreatedResourceSet;
            if (handler != null)
            {
                handler(this, new ResourceSetCreationEventArgs 
                {
                    ResourceStream = resourcestream, 
                    ResourceSet = set, 
                    ResourceSetType = this.ResourceSetType, 
                    Success = true 
                });
            }
        }

        protected void RaiseFailedResourceSet(Stream resourcestream, Exception ex)
        {
            var handler = FailedResourceSet;
            if (handler != null)
            {
                handler(this, new ResourceSetCreationEventArgs 
                {
                    ResourceStream = resourcestream, 
                    ResourceSet = null, 
                    ResourceSetType = this.ResourceSetType, 
                    Success = false,
                    Exception = ex
                });
            }
        }

        #endregion


        
        
        
        
        
        
        public InterResourceManager(string name, string assemblyPath, string fileformat) : base(name, assemblyPath ,fileformat)
        {
        }

        protected override string GetResourceFileName(System.Globalization.CultureInfo culture)
        {
            return FileFormat.Replace("{{culture}}", culture.Name).Replace("{{resource}}", BaseNameField);
        }

        protected override System.Resources.ResourceSet InternalGetResourceSet(System.Globalization.CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            if (Path == null && FileFormat == null) return null;
            if (culture == null || culture.Equals(CultureInfo.InvariantCulture)) return null;

            System.Resources.ResourceSet rs = null;
            Hashtable resourceSets = this.ResourceSets;

            if (!TryFetchResourceSet(resourceSets, culture, out rs))
            {
                Stream resourceFullPath = this.GetAssemblyResource(culture);

                if (resourceFullPath == null)
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
                    rs = this.CreateResourceSet(resourceFullPath);
                    AddResourceSet(resourceSets, culture, ref rs);
                    return rs;
                }
            }

            return rs;
        }

        private System.Resources.ResourceSet CreateResourceSet(Stream resourcePath)
        {
            System.Resources.ResourceSet set = null;

            try
            {
                set = InternalCreateResourceSet(resourcePath);
                RaiseCreatedResourceSet(resourcePath, set);
            }
            catch (Exception ex)
            {
                RaiseFailedResourceSet(resourcePath, ex);
            }

            return set;
        }

        private Stream GetAssemblyResource(CultureInfo culture)
        {
            string resourceFileName = this.GetResourceFileName(culture);
            string path = this.Path ?? String.Empty;

            
            string fullpath = path;
            if (File.Exists(fullpath)) {
                FileInfo f = new FileInfo(fullpath);
                return LoadResourceStream(resourceFileName, f.FullName);
            };

            
            if (path == String.Empty || !System.IO.Path.IsPathRooted(path))
            {
                
                if (Assembly.GetEntryAssembly() != null)
                {
                    string dir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
                    fullpath = System.IO.Path.Combine(dir, path);
                    if (File.Exists(fullpath))
                    {
                        return LoadResourceStream(resourceFileName, fullpath);
                    }
                }

                
                if (Assembly.GetExecutingAssembly() != null)
                {
                    if (Assembly.GetEntryAssembly() == null || System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) != System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    {
                        string dir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
                        fullpath = System.IO.Path.Combine(dir, path);
                        if (File.Exists(fullpath)) {
                            return LoadResourceStream(resourceFileName, fullpath);
                        }
                    }
                }
            }

            return null;
        }

		protected virtual System.Resources.ResourceSet InternalCreateResourceSet(Stream resourcePath)
        {
            object[] args = new object[] { resourcePath };
            GettextResourceSet rs = new GettextResourceSet(resourcePath);
            return (System.Resources.ResourceSet)rs;
        }
		
        private static Stream LoadResourceStream(string resourceFileName, string fullpath)
        {
            Assembly a = Assembly.LoadFrom(fullpath);
            Stream ms = a.GetManifestResourceStream(resourceFileName);
            if (ms != null)
                return ms;
            else
                return null;
        }

    }
}
