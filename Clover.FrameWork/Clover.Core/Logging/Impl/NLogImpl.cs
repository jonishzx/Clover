using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using System.Diagnostics;
using Clover.Core.IO;

namespace Clover.Core.Logging
{
    
    
    
    public class NLogImpl : ILogger
    {

        NLog.Logger logger;

        
        
        
        public bool IsLoggingEnabled {
            get {
                return LogManager.IsLoggingEnabled();
                
            }
            set {
                if (value)
                    LogManager.EnableLogging();
                else
                    LogManager.DisableLogging();
            }
        }

        
        bool showsystemenvironment = false;

        
        
        
        public bool ShowSystemEnvironment
        {
            get
            {
                return showsystemenvironment;

            }
            set
            {
                showsystemenvironment = value;
            }
        }

        bool shownetworkenvironment = false;

        
        
        
        public bool ShowNetworkEnvironment
        {
            get
            {
                return shownetworkenvironment;

            }
            set
            {
                shownetworkenvironment = value;
            }
        }

        bool showloadedassemblies = false;

        
        
        
        public bool ShowLoadedAssemblies
        {
            get
            {
                return showloadedassemblies;

            }
            set
            {
                showloadedassemblies = value;
            }

        }
        
        
        
        public NLogImpl() : this("Config\\Nlog.config", "*") { }

        
        
        
        public NLogImpl(string configfilepath) : this(configfilepath, "*") { }

        
        
        
        
        public NLogImpl(string configfilepath, string loggerName)
        {
            var path = PathTool.getInstance().Map(configfilepath);

            LogManager.Configuration = new XmlLoggingConfiguration(path);

             if(!IsExistLogger(loggerName))
                throw new ArgumentException("初始化日志组件失败！日志配置文件里无此loggerName：" + loggerName, "loggerName");

             logger = LogManager.GetLogger(loggerName);

        }

      

        
        
        
        
        
        public ILogger this[
            string loggerName]
        {
            get
            {
                return GetLoggerByName(loggerName);
            }
        }

        
        
        
        
        
        public ILogger GetLoggerByName(
            string loggerName)
        {

            logger = LogManager.GetLogger(loggerName);

            return this;
        }

        public void WriteLog(string message, Dictionary<string,string> virables)
        {
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Info, this.logger.Name, AppendInfomation(message));

            foreach (string key in virables.Keys)
            {
                theEvent.Properties[key] = virables[key];
            }          
          
            logger.Log(theEvent);       
        }
        
        public void WriteUserLog(string message, string loginname, string username, string userip, string logOPName)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("LoginName", loginname);
            data.Add("UserName", username);
            data.Add("UserIP", userip);
            data.Add("LogOPName", logOPName);

            WriteLog(message, data);
        }

        public void WriteUserLog(string message, Clover.Core.Domain.IAccount user,string userip, string logOPName)
        {
        
            WriteUserLog(message, user.UniqueId,user.UserName,userip,logOPName);
        }

        
        
        
        
        
        private string AppendInfomation(string message) {
            return Clover.Core.Common.StringHelper.Join(
                    Environment.NewLine,
                    message,
                    showsystemenvironment ? LoggingInformation.SystemEnvironment : "",
                    shownetworkenvironment ? LoggingInformation.NetworkEnvironment : "",
                    showloadedassemblies ? LoggingInformation.Assemblies : ""
                );
            
        }

        public void WriteLog(string message)
        {
            

            Info(AppendInfomation(message));
        }

        public void Trace(string message)
        {
            logger.Trace(AppendInfomation(message));
        }
        
        public void Debug(string message)
        {
            logger.Debug(AppendInfomation(message));
        }

        public void Info(string message)
        {
            logger.Info(AppendInfomation(message));
        }
        public void Warn(string message)
        {
            logger.Warn(AppendInfomation(message));
        }

        public void Warn(string message, Exception ex)
        {
            logger.WarnException(AppendInfomation(message), ex);
        }

        public void Error(string message)
        {
            logger.Error(AppendInfomation(message));
        }

        public void Error(string message,Exception ex)
        {
            logger.ErrorException(AppendInfomation(message), ex);
        }

        public void Fatal(string message)
        {
            logger.Fatal(AppendInfomation(message));
        }

        public void Fatal(string message, Exception ex)
        {
            logger.FatalException(AppendInfomation(message), ex);
           
        }

        public ILogger GetLogger(string loggerName)
        {
            if (IsExistLogger(loggerName))
                return new NLogImpl(loggerName);
            else
                return this;
        }

        bool IsExistLogger(string loggerName)
        {
            bool isExist = false;
            for (int i = 0; i < LogManager.Configuration.LoggingRules.Count; i++)
            {
                if (LogManager.Configuration.LoggingRules[i].NameMatches(loggerName))
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }
    }
}
