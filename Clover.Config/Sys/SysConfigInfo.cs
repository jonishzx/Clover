using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Clover.Core.Configuration;

namespace Clover.Config.Sys
{
	
	
	
	[Serializable]
	public class SysConfigInfo : IConfigInfo
    {
        #region 私有字段

		private string m_tableprefix = "bf_";		
		private string m_VodPath = "/";			
        private ConnectionStrings m_connectionstrings;
        private string m_AdminAuthType = "Simple";			
        #endregion

        #region 属性
        [XmlElement("DBConnections")]
        public ConnectionStrings ConnectionStrings
        {
            get { return m_connectionstrings; }
            set { m_connectionstrings = value; }
        }

        
        
        
        [XmlElement]
		public string Tableprefix
		{
			get { return m_tableprefix;}
			set { m_tableprefix = value;}
		}

        
        
        
        [XmlElement]
		public string VodPath
		{
			get { return m_VodPath;}
			set { m_VodPath = value;}
		}

        
        
        
        [XmlElement]
        public string AdminAuthType
		{
            get { return m_AdminAuthType; }
            set { m_AdminAuthType = value; }
		}

       
        #endregion
    }
}
