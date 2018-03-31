namespace Clover.Core.Logging
{
	#region 引用的命名空间.
	

	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Xml;
	using Clover.Core.Properties;
    using StructureMap;

	
	#endregion

	
    [PluginFamily("Default")]
	
	
	
	public class LogCentral
    {
        public const string LOG_File_ErrorLog = "ErrorLogger";
        public const string LOG_DB_UserOpLog = "UserOPLogger";

        #region 
        public bool IsLoggingEnabled = false;

        public static ILogger Current {
            get {
                return ObjectFactory.GetInstance<ILogger>();
            }
        }

        #endregion

        #region Information provider.
      
		#endregion
	}

	
}