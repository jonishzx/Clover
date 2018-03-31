using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;

namespace Clover.Data
{
    
    
    
    public abstract class OracleHelper
    {
        
        public static readonly string ConnectionStringLocalTransaction =
            ConfigurationManager.AppSettings["OraConnString1"];

        public static readonly string ConnectionStringInventoryDistributedTransaction =
            ConfigurationManager.AppSettings["OraConnString2"];

        public static readonly string ConnectionStringOrderDistributedTransaction =
            ConfigurationManager.AppSettings["OraConnString3"];

        public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["OraProfileConnString"];

        public static readonly string ConnectionStringMembership =
            ConfigurationManager.AppSettings["OraMembershipConnString"];

        
        private static readonly Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText,
            params OracleParameter[] commandParameters)
        {
            
            var cmd = new OracleCommand();

            
            using (var connection = new OracleConnection(connectionString))
            {
                
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                
                int val = cmd.ExecuteNonQuery();
                connection.Close();
                cmd.Parameters.Clear();
                return val;
            }
        }

        
        
        
        
        
        public static DataSet Query(string connectionString, string SQLString)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                var ds = new DataSet();
                try
                {
                    connection.Open();
                    var command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
                return ds;
            }
        }

        public static DataSet Query(string connectionString, string SQLString, params OracleParameter[] cmdParms)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                var cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (var da = new OracleDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                    return ds;
                }
            }
        }

        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans,
            string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text; 
            if (cmdParms != null)
            {
                foreach (OracleParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput ||
                         parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        
        
        
        
        
        public static object GetSingle(string connectionString, string SQLString)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                using (var cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        public static bool Exists(string connectionString, string strOracle)
        {
            object obj = GetSingle(connectionString, strOracle);
            int cmdresult;
            if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            return true;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText,
            params OracleParameter[] commandParameters)
        {
            var cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText,
            params OracleParameter[] commandParameters)
        {
            var cmd = new OracleCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public static int ExecuteNonQuery(string connectionString, string cmdText)
        {
            var cmd = new OracleCommand();
            var connection = new OracleConnection(connectionString);
            PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, null);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        
        
        
        public static OracleDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText,
            params OracleParameter[] commandParameters)
        {
            var cmd = new OracleCommand();
            var conn = new OracleConnection(connectionString);
            try
            {
                
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
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
            params OracleParameter[] commandParameters)
        {
            var cmd = new OracleCommand();

            using (var conn = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        
        
        
        
        
        
        
        
        
        public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText,
            params OracleParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException(
                    "The transaction was rollbacked	or commited, please	provide	an open	transaction.", "transaction");

            
            var cmd = new OracleCommand();

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            
            object retval = cmd.ExecuteScalar();

            
            cmd.Parameters.Clear();
            return retval;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public static object ExecuteScalar(OracleConnection connectionString, CommandType cmdType, string cmdText,
            params OracleParameter[] commandParameters)
        {
            var cmd = new OracleCommand();

            PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        
        
        
        
        
        public static void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        
        
        
        
        
        public static OracleParameter[] GetCachedParameters(string cacheKey)
        {
            var cachedParms = (OracleParameter[]) parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            
            var clonedParms = new OracleParameter[cachedParms.Length];

            
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OracleParameter) ((ICloneable) cachedParms[i]).Clone();

            return clonedParms;
        }

        
        
        
        
        
        
        
        
        
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans,
            CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {
            
            if (conn.State != ConnectionState.Open)
                conn.Open();

            
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            
            if (trans != null)
                cmd.Transaction = trans;

            
            if (commandParameters != null)
            {
                foreach (OracleParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }

        
        
        
        
        
        public static string OraBit(bool value)
        {
            if (value)
                return "Y";
            return "N";
        }

        
        
        
        
        
        public static bool OraBool(string value)
        {
            if (value.Equals("Y"))
                return true;
            return false;
        }

        
        
        
        
        public static bool ExecuteSqlTran(string conStr, List<CommandInfo> cmdList)
        {
            using (var conn = new OracleConnection(conStr))
            {
                conn.Open();
                var cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    foreach (CommandInfo c in cmdList)
                    {
                        if (!String.IsNullOrEmpty(c.CommandText))
                        {
                            PrepareCommand(cmd, conn, tx, CommandType.Text, c.CommandText,
                                (OracleParameter[]) c.Parameters);
                            if (c.EffentNextType == EffentNextType.WhenHaveContine ||
                                c.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (c.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    tx.Rollback();
                                    throw new Exception("Oracle:违背要求" + c.CommandText + "必须符合select count(..的格式");
                                    
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (c.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    tx.Rollback();
                                    throw new Exception("Oracle:违背要求" + c.CommandText + "返回值必须大于0");
                                    
                                }
                                if (c.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    tx.Rollback();
                                    throw new Exception("Oracle:违背要求" + c.CommandText + "返回值必须等于0");
                                    
                                }
                                continue;
                            }
                            int res = cmd.ExecuteNonQuery();
                            if (c.EffentNextType == EffentNextType.ExcuteEffectRows && res == 0)
                            {
                                tx.Rollback();
                                throw new Exception("Oracle:违背要求" + c.CommandText + "必须有影像行");
                                
                            }
                        }
                    }
                    tx.Commit();
                    return true;
                }
                catch (OracleException E)
                {
                    tx.Rollback();
                    throw E;
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }

        
        
        
        
        public static void ExecuteSqlTran(string conStr, List<String> SQLStringList)
        {
            using (var conn = new OracleConnection(conStr))
            {
                conn.Open();
                var cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    foreach (string sql in SQLStringList)
                    {
                        if (!String.IsNullOrEmpty(sql))
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}