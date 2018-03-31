namespace Clover.Core.Logging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

	
	
	
	public interface ILogger
	{
        
        
        
        bool IsLoggingEnabled { get; set; }

        
        
        
        bool ShowSystemEnvironment { get; set; }

        
        
        
        bool ShowNetworkEnvironment { get; set; }

        
        
        
        bool ShowLoadedAssemblies { get; set; }

        
        
        
        ILogger this[string loggername]{ get; }

        
        
        
        ILogger GetLoggerByName(string loggername);

		
		
		void WriteLog(string message);

        
        
        
        
        
        void WriteLog(string message,Dictionary<string,string> virables);


        
        
        
        
        
        void WriteUserLog(string message, string loginname, string username, string userip, string logOPName);


        
        
        
        
        
        void WriteUserLog(string message, Clover.Core.Domain.IAccount user, string userip, string logOPName);

        
        
        void Trace(string message);

        
        
        void Debug(string message);

        
        
        void Info(string message);

        
        
        void Warn(string message);

        
        
        
        void Warn(string message, Exception ex);

        
        
        void Error(string message);


        
        
        void Error(string message, Exception ex);


        
        
        void Fatal(string message);


        
        
        
        void Fatal(string message, Exception ex);

        
        
        
        ILogger GetLogger(string loggerName);
	}
}
