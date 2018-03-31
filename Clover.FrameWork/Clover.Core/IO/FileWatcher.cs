using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using Clover.Core.Base;

namespace Clover.Core.IO
{
    
    
    
    public class FileWatcher
    {
        
        
        
        FileSystemWatcher _fsWatcher;

        
        
        
        SingleRunMethod<string, FileSystemEventArgs> _executeChanged;

        public event TAction<FileSystemEventArgs> FileChangeEvent = null;


        #region ctor
        public FileWatcher(string path) : this(path, "*.*",
            NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName)
        {
           
        }

        
        
        
        
        
        
        
        public FileWatcher(string path, string filter, NotifyFilters notifyFilters)
        {
            if (File.Exists(path))
            {
                FileInfo finfo = new FileInfo(path);
                path = finfo.Directory.FullName;
                filter = finfo.Name; 
            }

            directory = path;

            _fsWatcher = new FileSystemWatcher(directory, filter);

            _fsWatcher.NotifyFilter = notifyFilters;

            
            _fsWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_Changed);

            
            _executeChanged = new SingleRunMethod<string, FileSystemEventArgs>(mySingleRunMethod, 1000);
        }

        #endregion


        #region  公共方法
        private string directory;
        
        
        
        public string Path {
            get {
                return directory;
            }
            set {
                if (string.Compare(this.directory, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    this.directory = value;
                  
                    this.Stop();
                    this.Start();
                }

            }
        }

        
        
        
        public void Start()
        {
            _fsWatcher.EnableRaisingEvents = true;
        }

        
        
        
        public void Stop()
        {
            _fsWatcher.EnableRaisingEvents = false;
        }
        #endregion

        
        
        
        
        
        void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _executeChanged.OnExecute(e.Name, e);
        }

        
        
        
        
        void mySingleRunMethod(object arg)
        {
            
            FileChangeEvent((FileSystemEventArgs)arg);
        }

    }
}
