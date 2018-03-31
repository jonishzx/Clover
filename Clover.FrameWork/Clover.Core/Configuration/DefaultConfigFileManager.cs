namespace Clover.Core.Configuration
{
    #region 引用的命名空间.

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Clover.Core.Common;
    using Clover.Core.IO;

    #endregion

    
    
    
    public abstract class DefaultConfigFileManager<T> : IConfigFileManager<T>  where T : IConfigInfo
    {

        #region member
        
        
        
        
        private string m_configfilepath;

        
        
        
        private T m_configinfo;

        
        
        
        private static object m_lockHelper = new object();


        
        
        
        private DateTime m_configoldchange = DateTime.Now;

        XMLSeaializeHelper<T> xmlhelper = new XMLSeaializeHelper<T>();

        
        
        
        private FileWatcher m_watcher;
        #endregion

        
        
        
        
        public DefaultConfigFileManager(string configfilepath)
        {
            ConfigFilePath = configfilepath;
            ConfigChangedTime = System.IO.File.GetLastWriteTime(configfilepath);
        }

        
        
        
        protected string ConfigFilePath
        {
            get { return m_configfilepath; }
            private set {
                if (value == null)
                    return;
          
                string filename = PathTool.getInstance().Map(value);
              
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException(Properties.Resources.Str_Configuration_PathNotExists + value);
                }

                m_configfilepath = filename;

                if (m_watcher == null)
                {
                    m_watcher = new FileWatcher(m_configfilepath);
                    m_watcher.FileChangeEvent += new Clover.Core.Base.TAction<FileSystemEventArgs>(m_watcher_FileChangeEvent);
                    m_watcher.Start();
                }
                else
                    m_watcher.Path = m_configfilepath;
               
            }
        }

        #region public 
        
        
        
        public T ConfigInfo
        {
            get { return m_configinfo; }
            set { m_configinfo = value; }
        }

        
        
        
        public T LoadRealConfig()
        {
            return LoadConfig(m_configoldchange, ConfigFilePath, false);
        }

        
        
        
        public T LoadConfig()
        {
            return LoadConfig(m_configoldchange, ConfigFilePath, true);
        }

        
        
        
        
        
        
        public bool SaveConfig()
        {
            try
            {
                m_watcher.Stop();
                lock (m_lockHelper)
                {
                    xmlhelper.Save(m_configinfo, ConfigFilePath);
                }
                m_watcher.Start();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        
        
        
        public void SetConfigFilePath(string path)
        {
            ConfigFilePath = path;
        }

        
        
        
        
        
        
        public bool SaveConfig(string configFilePath, T configinfo)
        {
            try
            {
                m_watcher.Stop();

                xmlhelper.Save(configinfo, configFilePath);

                m_watcher.Start();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        
        
        
        
        
        
        public T LoadConfig(DateTime fileoldchange, string configFilePath)
        {
            return LoadConfig(fileoldchange, configFilePath, true);
        }

        
        
        
        
        
        
        
        
        public T LoadConfig(DateTime fileoldchange, string configFilePath, bool checkTime)
        {

            if (!m_configfilepath.Equals(m_configfilepath, StringComparison.OrdinalIgnoreCase))
                ConfigFilePath = configFilePath;

            if (checkTime && m_configinfo != null)
            {
                DateTime m_filenewchange = System.IO.File.GetLastWriteTime(configFilePath);

                
                if (fileoldchange != m_filenewchange)
                {
                    fileoldchange = m_filenewchange;
                    lock (m_lockHelper)
                    {
                        m_configinfo = DeserializeInfo(configFilePath);
                    }
                }
            }
            else
            {
                lock (m_lockHelper)
                {
                    if (m_configinfo == null) 
                    {
                        m_configinfo = DeserializeInfo(configFilePath);
                    }
                }

            }

            return m_configinfo;
        }
        #endregion

        #region protected
        
        
        
        protected DateTime ConfigChangedTime
        {
            get { return m_configoldchange; }
            set { m_configoldchange = value; }
        }

        
        
        
        
        
        
        protected T DeserializeInfo(string configfilepath)
        {
            return xmlhelper.LoadFile(configfilepath);
        }  
        #endregion

        #region private 
        
        
        
        
        void m_watcher_FileChangeEvent(FileSystemEventArgs arg)
        {
            LoadConfig();

            
            if (ChangeConfigEvent!=null)
                ChangeConfigEvent(m_configinfo);
        }

        
        #endregion

        #region IConfigFileManager<T> 成员
        public Clover.Core.Base.TAction<T> changeconfig;

        public event Clover.Core.Base.TAction<T> ChangeConfigEvent;

        public Clover.Core.Base.TAction<T> ChangeConfig(T o)
        {
            return changeconfig;           
        }

        #endregion

        #region IConfigFileManager<T> 成员


     

        #endregion
    }
}
