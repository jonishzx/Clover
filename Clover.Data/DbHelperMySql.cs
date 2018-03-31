using System;
using System.Collections;
using System.Data;
using System.Data.OracleClient;
using MySql.Data.MySqlClient;

namespace Clover.Data
{
    
    
    
    
    
    public sealed class DbHelperMySql : IDbHelper
    {
        private static readonly object lockobj = new object();
        private static DbHelperMySql obj;
        private string connectionString = PubConstant.GetEncryConnectionString();

        

        public DbHelperMySql()
        {
        }

        public DbHelperMySql(string connectionstring)
        {
            connectionString = connectionstring;
        }

        
        
        
        public static DbHelperMySql Current
        {
            get
            {
                if (obj == null)
                {
                    lock (lockobj)
                    {
                        if (obj == null)
                            obj = new DbHelperMySql();
                    }
                }
                return obj;
            }
        }

        #region 公用方法

        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            return int.Parse(obj.ToString());
        }

        public bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
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

        public bool Exists(string strSql, params IDbDataParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
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

        #endregion

        #region  执行简单SQL语句

        
        
        
        
        
        public int ExecuteSql(string SQLString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySqlException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        
        
        
        
        
        
        public int ExecuteSql(string SQLString, string content)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var cmd = new MySqlCommand(SQLString, connection);
                var myParameter = new MySqlParameter("@content", OracleType.NVarChar);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        
        
        
        
        
        
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var cmd = new MySqlCommand(strSQL, connection);
                var myParameter = new MySqlParameter("@fs", OracleType.LongRaw);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        
        
        
        
        
        public object GetSingle(string SQLString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand(SQLString, connection))
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
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        
        
        
        
        
        public SafeDataReader ExecuteReader(string strSQL)
        {
            var connection = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return new SafeDataReader(myReader);
            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        
        
        
        
        
        public DataSet Query(string SQLString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var ds = new DataSet();
                try
                {
                    connection.Open();
                    var command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }

        
        
        
        
        public void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new MySqlCommand();
                cmd.Connection = connection;
                MySqlTransaction tx = connection.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (MySqlException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        
        
        
        
        
        public int ExecuteSql(string SQLString, params IDbDataParameter[] cmdParms)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        
        
        
        
        
        public object GetSingle(string SQLString, params IDbDataParameter[] cmdParms)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        
        
        
        
        
        public SafeDataReader ExecuteReader(string SQLString, params IDbDataParameter[] cmdParms)
        {
            var connection = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                new SafeDataReader(myReader);
            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }
            return null;
        }

        
        
        
        
        
        public DataSet Query(string SQLString, params IDbDataParameter[] cmdParms)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (var da = new MySqlDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }

        
        
        
        
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    var cmd = new MySqlCommand();
                    try
                    {
                        
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            var cmdParms = (MySqlParameter[]) myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText,
            IDbDataParameter[] cmdParms)
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
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 存储过程操作

        
        
        
        
        
        
        public SafeDataReader RunProcedure(string storedProcName, IDbDataParameter[] parameters)
        {
            var connection = new MySqlConnection(connectionString);
            MySqlDataReader returnReader;
            connection.Open();
            MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return new SafeDataReader(returnReader);
        }


        
        
        
        
        
        
        
        public DataSet RunProcedure(string storedProcName, IDbDataParameter[] parameters, string tableName)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var dataSet = new DataSet();
                connection.Open();
                var sqlDA = new MySqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        
        
        
        
        
        
        
        public int RunProcedure(string storedProcName, IDbDataParameter[] parameters, out int rowsAffected)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                int result;
                connection.Open();
                MySqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int) command.Parameters["ReturnValue"].Value;
                
                return result;
            }
        }

        
        
        
        
        
        
        
        private MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName,
            IDbDataParameter[] parameters)
        {
            var command = new MySqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (MySqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        
        
        
        
        
        
        private MySqlCommand BuildIntCommand(MySqlConnection connection, string storedProcName,
            IDbDataParameter[] parameters)
        {
            MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new MySqlParameter("ReturnValue",
                MySqlDbType.Int32, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        #endregion

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
    }
}