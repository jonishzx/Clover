using System.Data;

namespace Clover.Data
{
    internal interface IDbHelper
    {
        string ConnectionString { get; set; }
        SafeDataReader ExecuteReader(string strSQL);
        SafeDataReader ExecuteReader(string SQLString, params IDbDataParameter[] cmdParms);
        int ExecuteSql(string SQLString, params IDbDataParameter[] cmdParms);
        int ExecuteSql(string SQLString);
        int ExecuteSql(string SQLString, string content);
        int ExecuteSqlInsertImg(string strSQL, byte[] fs);
        
        
        bool Exists(string strSql, params IDbDataParameter[] cmdParms);
        bool Exists(string strSql);
        int GetMaxID(string FieldName, string TableName);
        object GetSingle(string SQLString, params IDbDataParameter[] cmdParms);
        object GetSingle(string SQLString);
        DataSet Query(string SQLString, params IDbDataParameter[] cmdParms);
        DataSet Query(string SQLString);
        int RunProcedure(string storedProcName, IDbDataParameter[] parameters, out int rowsAffected);
        DataSet RunProcedure(string storedProcName, IDbDataParameter[] parameters, string tableName);
        SafeDataReader RunProcedure(string storedProcName, IDbDataParameter[] parameters);
    }
}