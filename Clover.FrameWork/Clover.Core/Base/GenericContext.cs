using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Clover.Core.Common;

namespace Clover.Core
{
    
    
    
    public class GenericContext
    {
        private static GenericContext m_obj;

        private string m_applicationPath;
        private string m_rootPath;
        private string m_host;

        private static object m_lockhelper = new object();

        public static GenericContext Current
        {
            get{
                if(m_obj == null)
                {
                    lock (m_lockhelper)
                    {
                        if (m_obj == null)
                        {
                            m_obj = init();
                        }
                    }
                }

                return m_obj;
            }
        }

        GenericContext(){
            
            
        }
        
        
        
        
        public String RootPath { get { return m_applicationPath; } }
        
        
        
        public String ApplicationPath { get { return m_rootPath; } }
        
        
        
        public String Host { get { return m_host; } }

        
        
        
        public static Boolean IsWeb { get { return HttpContext.Current != null; } }

        
        
        
        public static Boolean IsWindows
        {
            get { return Environment.OSVersion.VersionString.ToLower().IndexOf("windows") >= 0; }
        }      
        
        
        
        
        
        private static GenericContext init()
        {
            GenericContext obj = new GenericContext();

            if (IsWeb)
            {
                obj.m_applicationPath = HttpContext.Current.Request.ApplicationPath;
                obj.m_rootPath = StringHelper.addEndSlash(obj.m_applicationPath);
                obj.m_host = HttpContext.Current.Request.Url.Host;
            }
            else
            {
                obj.m_applicationPath = "/";
                obj.m_rootPath = "/";
                obj.m_host = "localhost";

                
                AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(UnhandledException);
            }

            return obj;
        }

        
        
        
        
        protected virtual void HandleApplicationError(
            Exception e)
        {
            
            
            
            throw new NotImplementedException("未实现");
        }


        
        
        
        
        
        
        private static void UnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception x = e.ExceptionObject as Exception;
                
            }
            catch (Exception)
            {
                
            }
        }
    }
}
