using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Clover.Core.XCrypt;

namespace Clover.Config.Sys
{
	   
	
    
	
	public class ConnectionString
	{
       
        #region 属性及字段

        private string tckey;            
        private string tcType;           
        
        private string tcPassword;       
        private string tcDescription;    
        private string tcConnString;     

        private string m_DBBackupPath = "";

        [XmlAttribute("Key")]
        
        
        
        public string Key
        {
            get { return tckey; }
            set {
                tckey = value; 
            }
        }

        [XmlAttribute("DBType")]
        
        
        
        public string DBType
        {
            get { return tcType; }
            set {
                tcType = value; 
            }
        }


        [XmlAttribute("ConnString")]
        
        
        
        public string ConnString
        {
            get { return tcConnString; }
            set { tcConnString = value; }
        }

        [XmlAttribute("Password")]
        
        
        
        public string Password
        {
            get { return tcPassword; }
            set {

                if (!string.IsNullOrEmpty(value))
                {

                    tcPassword = XCryptEngine.Current().Decrypt(value);
                    if (tcPassword == null)
                    {
                        throw new Exception("解析Config.Connections.Item('" + tckey + "')密码时出错!");
                    }
                }
              
            }
        }


        [XmlAttribute("Description")]
        
        
        
        public string Description
        {
            get { return tcDescription; }
            set { tcDescription = value; }
        }


        [XmlElement]
        
        
        
        public string DBBackupPath
        {
            get { return m_DBBackupPath; }
            set { m_DBBackupPath = value; }
        }


        #endregion

        #region 自定义方法
        public string GetConnString()
        {
            
            return createConnString(tcType,tcConnString,tcPassword);
            
        }

        
        
        
        
        public string GetDBName()
        {
            string aa = tcConnString.Replace("\"", "");

            int sidx = aa.IndexOf("database=", StringComparison.OrdinalIgnoreCase);
            sidx = sidx + "database=".Length;

            int eidx = aa.IndexOf(";", sidx, System.StringComparison.Ordinal);

            return aa.Replace("\"", "").Substring(sidx, eidx - sidx);
        }

        private string createConnString(string aType, string aValue, string aPwd)
        {
            string rValue = "";
            if (System.String.Compare(aType, "sqlserver", System.StringComparison.OrdinalIgnoreCase) == 0 
                || System.String.Compare(aType, "ssas", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (aPwd == "")
                {
                    rValue = aValue;
                }
                else
                {
                    rValue = aValue + ";password=" + this.tcPassword + ";";
                }
            }
            else
            {
                rValue = aValue;
            }
            return rValue;
        }

        #endregion
		
	}
}
