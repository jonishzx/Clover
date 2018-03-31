using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace Clover.Data
{
    
    
    
    
    
    public sealed class DbHelperOleDb : IDbHelper
    {
        private static readonly object lockobj = new object();
        private static DbHelperOleDb obj;
        private string connectionString = PubConstant.ConnectionString;

        

        public DbHelperOleDb()
        {
        }

        public DbHelperOleDb(string connectionstring)
        {
            connectionString = connectionstring;
        }

        
        
        
        public static DbHelperOleDb Current
        {
            get
            {
                if (obj == null)
                {
                    lock (lockobj)
                    {
                        if (obj == null)
                            obj = new DbHelperOleDb();
                    }
                }
                return obj;
            }
        }

        #region  执行简单SQL语句

        
        
        
        
        
        public int ExecuteSql(string SQLString)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                using (var cmd = new OleDbCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OleDbException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        
        
        
        
        
        
        public int ExecuteSql(string SQLString, string content)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                var cmd = new OleDbCommand(SQLString, connection);
                var myParameter = new OleDbParameter("@content", OleDbType.VarChar);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OleDbException E)
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
            using (var connection = new OleDbConnection(connectionString))
            {
                var cmd = new OleDbCommand(strSQL, connection);
                var myParameter = new OleDbParameter("@fs", OleDbType.Binary);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OleDbException E)
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
            using (var connection = new OleDbConnection(connectionString))
            {
                using (var cmd = new OleDbCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        return obj;
                    }
                    catch (OleDbException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        
        
        
        
        
        public SafeDataReader ExecuteReader(string strSQL)
        {
            var connection = new OleDbConnection(connectionString);
            var cmd = new OleDbCommand(strSQL, connection);
            try
            {
                connection.Open();
                OleDbDataReader myReader = cmd.ExecuteReader();
                return new SafeDataReader(myReader);
            }
            catch (OleDbException e)
            {
                throw new Exception(e.Message);
            }
        }

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

        
        
        
        
        
        public DataSet Query(string SQLString)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                var ds = new DataSet();
                try
                {
                    connection.Open();
                    var command = new OleDbDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        
        
        
        
        public void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (var conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                var cmd = new OleDbCommand();
                cmd.Connection = conn;
                OleDbTransaction tx = conn.BeginTransaction();
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
                catch (OleDbException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        
        
        
        
        
        public int ExecuteSql(string SQLString, params IDbDataParameter[] cmdParms)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                using (var cmd = new OleDbCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (OleDbException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        
        
        
        
        
        public object GetSingle(string SQLString, params IDbDataParameter[] cmdParms)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                using (var cmd = new OleDbCommand())
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
                        return obj;
                    }
                    catch (OleDbException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        
        
        
        
        
        public SafeDataReader ExecuteReader(string SQLString, params IDbDataParameter[] cmdParms)
        {
            var connection = new OleDbConnection(connectionString);
            var cmd = new OleDbCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OleDbDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                new SafeDataReader(myReader);
            }
            catch (OleDbException e)
            {
                throw new Exception(e.Message);
            }
            return null;
        }

        
        
        
        
        
        public DataSet Query(string SQLString, params IDbDataParameter[] cmdParms)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                var cmd = new OleDbCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (var da = new OleDbDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OleDbException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        
        
        
        
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (var conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbTransaction trans = conn.BeginTransaction())
                {
                    var cmd = new OleDbCommand();
                    try
                    {
                        
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            var cmdParms = (OleDbParameter[]) myDE.Value;
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


        private void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, string cmdText,
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
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 存储过程操作

        
        
        
        
        
        
        public SafeDataReader RunProcedure(string storedProcName, IDbDataParameter[] parameters)
        {
            var connection = new OleDbConnection(connectionString);
            OleDbDataReader returnReader;
            connection.Open();
            OleDbCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            return new SafeDataReader(returnReader);
        }


        
        
        
        
        
        
        
        public DataSet RunProcedure(string storedProcName, IDbDataParameter[] parameters, string tableName)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                var dataSet = new DataSet();
                connection.Open();
                var sqlDA = new OleDbDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        
        
        
        
        
        
        
        public int RunProcedure(string storedProcName, IDbDataParameter[] parameters, out int rowsAffected)
        {
            using (var connection = new OleDbConnection(connectionString))
            {
                int result;
                connection.Open();
                OleDbCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int) command.Parameters["ReturnValue"].Value;
                
                return result;
            }
        }

        
        
        
        
        
        
        
        private OleDbCommand BuildQueryCommand(OleDbConnection connection, string storedProcName,
            IDbDataParameter[] parameters)
        {
            var command = new OleDbCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OleDbParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        
        
        
        
        
        
        private OleDbCommand BuildIntCommand(OleDbConnection connection, string storedProcName,
            IDbDataParameter[] parameters)
        {
            OleDbCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new OleDbParameter("ReturnValue",
                OleDbType.Integer, 4, ParameterDirection.ReturnValue,
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