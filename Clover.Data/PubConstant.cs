using System.Configuration;

namespace Clover.Data
{
    public class PubConstant
    {
        
        
        
        public static string ConnectionString
        {
            get
            {
                string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
                if (ConStringEncrypt == "true")
                {
                    _connectionString = DESEncrypt.Decrypt(_connectionString);
                }
                return _connectionString;
            }
        }

        
        
        
        
        
        public static string GetConnectionString(string configName)
        {
            string connectionString = ConfigurationManager.AppSettings[configName];
            string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (ConStringEncrypt == "true")
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
            return connectionString;
        }


        
        
        
        
        public static string GetEncryConnectionString()
        {
            string str = ConfigurationManager.AppSettings["connString"];
            string str2 = ConfigurationManager.AppSettings["connPassword"];
            if (str == null)
            {
                return "";
            }
            if (str2.Length > 0)
            {
                str = str + ";Password=" + str2 + ";";
            }
            return str;
        }
    }
}