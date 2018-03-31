





using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Clover.Data
{
    
    
    
    
    public abstract class SqlHelper
    {
        
        public static readonly string ConnectionStringLocalTransaction =
            ConfigurationManager.AppSettings["SQLConnString1"];

        public static readonly string ConnectionStringInventoryDistributedTransaction =
            ConfigurationManager.AppSettings["SQLConnString2"];

        public static readonly string ConnectionStringOrderDistributedTransaction =
            ConfigurationManager.AppSettings["SQLConnString3"];

        public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["SQLProfileConnString"];

        
        private static readonly Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            var cmd = new SqlCommand();

            using (var conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            var cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            var cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            var cmd = new SqlCommand();
            var conn = new SqlConnection(connectionString);

            
            
            
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            var cmd = new SqlCommand();

            using (var connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            var cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        
        
        
        
        
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            var cachedParms = (SqlParameter[]) parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            var clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter) ((ICloneable) cachedParms[i]).Clone();

            return clonedParms;
        }

        
        
        
        
        
        
        
        
        
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType,
            string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}