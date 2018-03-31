using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using Clover.Config.Sys;
using Clover.Core.Domain;
using Clover.Core.Logging;
using Dapper;
using MySql.Data.MySqlClient;
using StructureMap;

namespace Clover.Data
{
    
    
    
    public class BaseDAO
    {
        
        
        
        public static readonly string DEFAULT_DBNAME = "main";

        
        
        
        public static readonly string TRANSACTION_FLAG = "_tran";

        
        
        
        public static int CommandTimeout = 300;

        
        
        
        public static List<IDbConnection> ManualConnectionPool = new List<IDbConnection>();

        #region 根据字段名称拿取在SafeDataReader中的索引

        
        
        
        protected int GetReaderIndex(IDataReader reader, string colName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(colName))
                    return i;
            }
            return -1;
        }

        #endregion


        
        
        
        public static void ClearCachdQuery()
        {
            SqlMapper.PurgeQueryCache();
        }

        public static DataTable GetDataTable(string sql)
        {
            return GetDataTable("main", sql);
        }

        public static DataTable GetDataTable(string servicename, string sql)
        {
            string connStr = SysConfig.GetConnecting(servicename).GetConnString();

            DbHelperSQL.Current.ConnectionString = connStr;

            DataSet ds = DbHelperSQL.Current.Query(sql);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }

        
        
        
        
        
        
        public static DataSet QueryData(string sqlString, Dictionary<string, object> parameters)
        {
            return QueryData(DEFAULT_DBNAME, sqlString, parameters);
        }

        
        
        
        
        
        
        
        public static DataSet QueryData(string servicename, string sqlString, Dictionary<string, object> parameters)
        {
            string connStr = SysConfig.GetConnecting(servicename).GetConnString();

            DbHelperSQL.Current.ConnectionString = connStr;

            IDbHelper helper = GetDBHelper(servicename);

            var sqlparams = new List<IDbDataParameter>();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (string p in parameters.Keys)
                {
                    sqlparams.Add(new SqlParameter(p, parameters[p]));
                }
            }

            DataSet ds = helper.Query(sqlString, sqlparams.ToArray());

            return ds;
        }

        public static DataSet GetList(string objectname, string keyfield, int PageSize, int PageIndex, string strWhere,
            bool desc, string orderfield, out int rstcount)
        {
            return GetList(DEFAULT_DBNAME, objectname, keyfield, PageSize, PageIndex, strWhere, desc, orderfield, out rstcount);
        }

        public static DataSet GetList(string servicename, string objectname, string keyfield, int PageSize,
            int PageIndex, string strWhere, bool desc, string orderfield, out int rstcount)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@tblName", SqlDbType.VarChar, 1000),
                new SqlParameter("@keyName", SqlDbType.VarChar, 1000),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@PageIndex", SqlDbType.Int),
                new SqlParameter("@OrderType", SqlDbType.Bit),
                new SqlParameter("@strWhere", SqlDbType.VarChar, 1000),
                new SqlParameter("@OrderfldName", SqlDbType.VarChar, 500),
                new SqlParameter("@ResultCount", SqlDbType.Int)
            };
            parameters[0].Value = objectname;
            parameters[1].Value = keyfield;
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = desc ? 1 : 0;
            parameters[5].Value = strWhere;
            parameters[6].Value = orderfield;
            parameters[7].Direction = ParameterDirection.Output;

            IDbHelper helper = GetDBHelper(servicename);

            DataSet ds = helper.RunProcedure("UP_GetRecordByPage", parameters, "ds");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0] != DBNull.Value)
            {
                rstcount = int.Parse(parameters[7].Value.ToString());
            }
            else
                rstcount = 0;

            return ds;
        }

        
        
        
        
        
        private static IDbHelper GetDBHelper(string servicename)
        {
            string connStr = SysConfig.GetConnecting(servicename).GetConnString();

            ConnectionString conCfg = SysConfig.GetConnecting(servicename);

            IDbHelper helper = null;
            if (conCfg.DBType.Equals(DatabaseType.MySQL.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                helper = DbHelperMySql.Current;
            }
            else if (conCfg.DBType.Equals(DatabaseType.Oracle.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                helper = DbHelperOra.Current;
            }
            else if (conCfg.DBType.Equals(DatabaseType.SQLServer.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                helper = DbHelperSQL.Current;
            }
            else
            {
                helper = DbHelperOleDb.Current;
            }

            helper.ConnectionString = connStr;

            return helper;
        }


        
        
        
        
        
        
        public static bool CheckTableExists(string schema, string tableName)
        {
            string sql = string.Format(@"SELECT COUNT(*)
                        FROM information_schema.tables 
                        WHERE table_schema = '{0}' 
                        AND table_name = '{1}';", schema, tableName);

            IDbConnection conn = DbService();

            int count = new List<int>(conn.Query<int>(sql))[0];

            return count > 0;
        }

        public static Int16 OraBit(Boolean? value)
        {
            if (value != null && (Boolean)value)
                return 1;
            return 0;
        }

        static bool isdebug = true;
        
        
        
        public static bool IsDebug{get{return isdebug;}set{isdebug = value;}}

        
        
        
        
        public static void OnInit()
        {
            ObjectFactory.Configure(x =>
            {
                foreach (var pool in SysConfig.Config.ConnectionStrings.ConnectionString)
                {
                    if (pool.DBType == DatabaseType.MySQL.ToString())
                    {
                        
                        x.For<IDbConnection>().HybridHttpOrThreadLocalScoped().Use<MySqlConnection>().Named(pool.Key).Ctor<string>().Is(pool.GetConnString());

                        
                        x.For<IDbConnection>().HybridHttpOrThreadLocalScoped().Use<MySqlConnection>().Named(pool.Key + TRANSACTION_FLAG).Ctor<string>().Is(pool.GetConnString());

                    }
                    else if (pool.DBType == DatabaseType.Oracle.ToString())
                    {
                        
                        x.For<IDbConnection>().HybridHttpOrThreadLocalScoped().Use<OracleConnection>().Named(pool.Key).Ctor<string>().Is(pool.GetConnString());
                        
                        x.For<IDbConnection>().HybridHttpOrThreadLocalScoped().Use<OracleConnection>().Named(pool.Key + TRANSACTION_FLAG).Ctor<string>().Is(pool.GetConnString());
                    }
                    else
                    {
                        x.SelectConstructor<SqlConnection>(() => new SqlConnection(pool.GetConnString()));
                        
                        x.For<IDbConnection>().HybridHttpOrThreadLocalScoped().Use<SqlConnection>().Named(pool.Key).Ctor<string>().Is(pool.GetConnString());

                        
                        x.For<IDbConnection>().HybridHttpOrThreadLocalScoped().Use<SqlConnection>().Named(pool.Key + TRANSACTION_FLAG).Ctor<string>().Is(pool.GetConnString());
                    }
                }
                
            });

            
            SqlMapper.ErrorOccorEvent = delegate(string connection, string sql, object oparams, Exception exception)
            {
                string message = "DB(" + connection + ") Command Execute Occur Error \r\n" +
                                 sql + "\r\n" +
                                 (oparams != null ? ("Parameters:" + DumpDynamicParams(oparams)) : "") + "\r\n" +
                                 "Error:\r\n" + exception.Message + "\r\n";
                LogCentral.Current.Error(message);
            };

            if(IsDebug){
                SqlMapper.BeforeQueryRunEvent = delegate(string connection, string sql, object oparams)
                {
                    string qmessage = "DB(" + connection +
                                      ") Command Execute \r\n" +
                                      sql + "\r\n" +
                                      (oparams != null ? ("Parameters:" + DumpDynamicParams(oparams)) : "") + "\r\n";

                    LogCentral.Current.Debug(qmessage);
                };
            }
        }

        
        
        
        
        
        private static string DumpDynamicParams(object o) {
            if (o == null)
                return string.Empty;

            if (o is DynamicParameters)
            {
                StringBuilder sb = new StringBuilder(100);
                var ps = o as DynamicParameters;
                foreach (var p in ps.ParameterNames) {
                   
                    sb.AppendFormat("{0}='{1}',", p, ps.GetString(p));
                }
                var rst = sb.ToString();
                sb = null;
                return rst;
            }
            else {
                return Clover.Core.Reflection.DumpBuilder.Dump(o);
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        public static IDbConnection DbService()
        {
            return DbService(DEFAULT_DBNAME);
        }

        public static IDbConnection DbTranService()
        {
            return DbTranService(DEFAULT_DBNAME);
        }
        
        
        
        
        public static IDbConnection DbTranService(string servicename)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                servicename = DEFAULT_DBNAME;
            }
            ConnectionString conCfg = SysConfig.GetConnecting(servicename);

            var conn = ObjectFactory.GetNamedInstance<IDbConnection>(servicename + TRANSACTION_FLAG);

            if (conn == null)
            {
                if (ObjectFactory.Model.HasImplementationsFor<IDbConnection>())
                {
                    IList<IDbConnection> conns = ObjectFactory.GetAllInstances<IDbConnection>();
                    foreach (IDbConnection con in conns)
                    {
                        if (con.Database.Equals(conCfg.GetDBName(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            conn = con;
                            break;
                        }
                    }
                }
                
                if (conn == null)
                {
                    
                    conn = ManualDbService(servicename);
                    ObjectFactory.Inject(conn);
                }
            }

            if (conn.ConnectionString == string.Empty) 
            {
                conn.ConnectionString = SysConfig.GetConnecting(servicename).GetConnString();
            }
            if (conn != null && conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        public static IDbConnection DbService(string servicename)
        {
            return DbService(servicename, true);
        }
        
        
        
        
        
        
        
        
        
        
        
        public static IDbConnection DbService(string servicename, bool openConn = true)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                servicename = DEFAULT_DBNAME;
            }
            ConnectionString conCfg = SysConfig.GetConnecting(servicename);
            string connstring = conCfg.GetConnString();
            IDbConnection conn = ObjectFactory.GetNamedInstance<IDbConnection>(servicename);

            if (conn == null)
            {
                if (ObjectFactory.Model.HasImplementationsFor<IDbConnection>())
                {
                    IList<IDbConnection> conns = ObjectFactory.GetAllInstances<IDbConnection>();
                    foreach (IDbConnection con in conns)
                    {
                        if (con.Database.Equals(conCfg.GetDBName(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            conn = con;
                            break;
                        }
                    }
                }
                
                if (conn == null)
                {
                    
                    conn = ManualDbService(servicename);
                    ObjectFactory.Inject(conn);
                }
            }

            if (conn.ConnectionString == string.Empty) 
            {
                conn.ConnectionString = SysConfig.GetConnecting(servicename).GetConnString();
            }

            if (conn != null && openConn && conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        public static Type GetDBTypeByServiceName(string servicename)
        {
            ConnectionString conCfg = SysConfig.GetConnecting(servicename);

            if (conCfg.DBType.Equals(DatabaseType.MySQL.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                return typeof(MySqlConnection);
            }
            else if (conCfg.DBType.Equals(DatabaseType.Oracle.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                return typeof(OracleConnection);
            }
            else if (conCfg.DBType.Equals(DatabaseType.SQLServer.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                return typeof(SqlConnection);
            }
            else
            {
                return typeof(OleDbConnection);
            }
        }

        
        
        
        
        
        public static IDbConnection ManualDbService()
        {
            return ManualDbService(DEFAULT_DBNAME);
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public static IDbConnection ManualDbService(string servicename)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                servicename = DEFAULT_DBNAME;
            }

            ConnectionString conCfg = SysConfig.GetConnecting(servicename);

            string connstring = conCfg.GetConnString();

            IDbConnection conn = null;
            if (conCfg.DBType.Equals(DatabaseType.MySQL.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                conn = new MySqlConnection(connstring);
            }
            else if (conCfg.DBType.Equals(DatabaseType.Oracle.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                conn = new OracleConnection(connstring);
            }
            else if (conCfg.DBType.Equals(DatabaseType.SQLServer.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                conn = new SqlConnection(connstring);
            }
            else
            {
                conn = new OleDbConnection(connstring);
            }


            ManualConnectionPool.Add(conn);

            
            if (ManualConnectionPool.Count > 100)
                ManualConnectionPool.RemoveAt(0);

            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }

        #region 常用方法

        
        
        
        
        
        public delegate string GetDataRuleFilter(string dataruleCode);

        public delegate string GetMFDataRuleFilter(string modulecode, string functioncode);

        public static List<T> GetList<T>(string objectname, string keyfield, int PageSize, int PageIndex,
            string strWhere, bool desc, string orderfield, out int rstcount)
        {
            return GetPagedList<T>(null, objectname, keyfield, PageSize, PageIndex, strWhere, desc, orderfield,
                out rstcount);
        }


        public static List<T> GetPagedListBySQL<T>(string servicename, string fldName, string objectname,
            string keyfield, int PageSize, int PageIndex, string strWhere, bool desc, string orderfield,
            out int rstcount)
        {
            string select = "";
            int PageLowerBound = PageIndex <= 1 ? 1 : (PageIndex - 1) * PageSize;
            int PageUpperBound = (PageLowerBound + PageSize) < 0 ? PageSize : (PageLowerBound + PageSize);
            rstcount = 0;

            string sTemp = "";
            if (!string.IsNullOrEmpty(strWhere))
            {
                if (strWhere.IndexOf("WHERE", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    sTemp += " WHERE ";
                }

                sTemp += strWhere;
            }

            string stotalRecords = "SELECT COUNT(*) FROM {0} {1}";


            List<T> rst = null;
            using (IDbConnection conn = ManualDbService(servicename))
            {

                var counlist = new List<int>(conn.Query<int>(string.Format(stotalRecords, objectname, sTemp)));

                if (!string.IsNullOrEmpty(orderfield))
                {
                    sTemp += " ORDER BY " + orderfield;
                }

                if (PageIndex <= 1)
                {
                    select = string.Format("SELECT TOP {3} {0} FROM {1} {2}", fldName, objectname, sTemp, PageSize);
                }
                else
                {
                    string orderby = "ORDER BY " + orderfield;
                    select =
                        string.Format(
                            "WITH t AS(SELECT ROW_NUMBER() OVER({2}) AS RowNum,{0} FROM {1} {5}) \r\n SELECT * from t WHERE ROWNUM between {3} and {4}",
                            fldName, objectname, orderby, PageLowerBound, PageUpperBound, strWhere);
                }

                if (counlist != null && counlist.Count > 0)
                {
                    rstcount = counlist[0];
                }
                if (rstcount < 1 || (PageIndex > 1 && rstcount < (PageLowerBound + 1)))
                {
                    return null;
                }

                rst = conn.Query<T>(select).ToList();
            }
            return rst;
        }

        
        
        
        public static List<T> GetPagedList<T>(string servicename, string objectname, string keyfield, int PageSize,
            int PageIndex, string strWhere, bool desc, string orderfield, out int rstcount)
        {
            var p = new DynamicParameters();
            p.Add("tblName", objectname, null, null, null);
            p.Add("keyName", keyfield, null, null, null);
            p.Add("PageSize", PageSize, null, null, null);
            p.Add("PageIndex", PageIndex, null, null, null);
            p.Add("OrderType", desc ? 1 : 0, null, null, null);
            p.Add("strWhere", strWhere, null, null, null);
            p.Add("OrderfldName", orderfield, null, null, null);
            p.Add("@ResultCount", 0, DbType.Int32, ParameterDirection.Output, null);

            List<T> rst = null;

            using (IDbConnection conn = ManualDbService(servicename))
            {

                rst =
                    conn.Query<T>("UP_GetRecordByPage", p, null, true, CommandTimeout,
                        CommandType.StoredProcedure).ToList();
                rstcount = p.Get<int>("ResultCount");
            }

            return rst;
        }
        public static DataTable GetPagedDataTable(string objectname, string keyfield, int PageSize,
       int PageIndex, string strWhere, bool desc, string orderfield, out int rstcount)
        {
            return GetPagedDataTable("main", objectname, keyfield, PageSize, PageIndex, strWhere, desc, orderfield, out rstcount);
        }
        public static DataTable GetPagedDataTable(string servicename, string objectname, string keyfield, int PageSize,
         int PageIndex, string strWhere, bool desc, string orderfield, out int rstcount)
        {
            string connStr = SysConfig.GetConnecting(servicename).GetConnString();

            DbHelperSQL.Current.ConnectionString = connStr;

            IDbHelper helper = GetDBHelper(servicename);

            var sqlparams = new List<IDbDataParameter>();
            sqlparams.Add(new SqlParameter("tblName", objectname));
            sqlparams.Add(new SqlParameter("keyName", keyfield));
            sqlparams.Add(new SqlParameter("PageSize", PageSize));
            sqlparams.Add(new SqlParameter("PageIndex", PageIndex));
            sqlparams.Add(new SqlParameter("OrderType", desc ? 1 : 0));
            sqlparams.Add(new SqlParameter("strWhere", strWhere));
            sqlparams.Add(new SqlParameter("OrderfldName", orderfield));
            var countparam = new SqlParameter("@ResultCount", 0);
            countparam.DbType = DbType.Int32;
            countparam.Direction = ParameterDirection.Output;
            sqlparams.Add(countparam);

            DataSet ds = helper.RunProcedure("UP_GetRecordByPage", sqlparams.ToArray(), "data");
            rstcount = countparam.Value != null ? (int)countparam.Value : 0;
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        public static void Close(IDbConnection conn)
        {
            if (conn != null && !(conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed))
                conn.Close();
        }

        
        
        
        
        public static IDbConnection BeginTransaction()
        {
            var conn = DbService("main", false);
            ReOpen(conn);
            return conn;
        }

        
        
        
        
        public static void EndTransaction(IDbConnection conn)
        {
            Close(conn);
        }

        
        
        
        
        public static void ReOpen(IDbConnection conn)
        {
            lock (conn)
            {
                Close(conn);
                conn.Open();
            }
        }

        public static void CloseWithDispose(IDbConnection conn)
        {
            Close(conn);
            conn.Dispose();
        }

        public static void Close()
        {
            IDbConnection cnn = DbService();
            Close(cnn);
        }

        
        
        
        
        
        
        
        
        public static bool ExistsSameAttr(string table, string field, string value, string strWhere, string id,
            string idval)
        {
            bool hasrecord = false;

            var sb = new StringBuilder();
            string[] values = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] fields = field.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != fields.Length)
                throw new ArgumentException("传入的值参数和字段参数数量不一置");

            sb.AppendFormat("select count(*) from {0} where 1=1 ", table);

            for (int i = 0; i < fields.Length; i++)
            {
                sb.AppendFormat("and {0} = '{1}'", fields[i], values[i]);
            }

            string where = "";
            if (idval != string.Empty && idval != "0")
                where = " and " + id + " <> '" + idval + "'";
            sb.Append(where);

            if (strWhere != string.Empty)
            {
                sb.Append(" and " + strWhere);
            }
            using (IDbConnection conn = ManualDbService())
            {
                try
                {
                    var rst = conn.Query<int>(sb.ToString(), null).First();
                    hasrecord = rst > 0;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return hasrecord;
        }

        
        
        
        
        
        
        
        
        
        public static bool CheckTheRelation(string id, string targettable, string targetField, out DataSet dataRelations)
        {
            return CheckTheRelation(id, targettable, targetField, "", out dataRelations);
        }

        public static string ParseSQLCommand(IAppContext ctx, string sqlcmd)
        {
            return ParseSQLCommand(ctx, sqlcmd, null, null);
        }

        
        
        
        
        
        private static void ParseConstSQL(IAppContext ctx, ref string sqlcmd)
        {
            
            if (ctx == null || ctx.CurrentUser == null)
                return;

            sqlcmd = sqlcmd.Replace("#env.CurrentUser.UserId#", "'" + ctx.AccountID + "'");
            
            if (!string.IsNullOrEmpty(ctx.CurrentUser.CurrGroupId))
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.GroupId#", "'" + ctx.CurrentUser.CurrGroupId + "'");
            }
            else
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.GroupId#", "'Unkown!'");

            }
            
            if (!string.IsNullOrEmpty(ctx.CurrentUser.CurrPositionId))
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.PositionId#", "'" + ctx.CurrentUser.CurrPositionId + "'");
            }
            else
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.PositionId#", "'Unkown!'");
            }

            
            if (ctx.CurrentUser.GroupIds != null && ctx.CurrentUser.GroupIds.Count > 0)
            {
                string groupids = "'" + string.Join("','", ctx.CurrentUser.GroupIds.ToArray()) + "'";
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.GroupIds#", groupids);
            }
            else
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.GroupIds#", "'Unkown!'");
            }

            if (ctx.CurrentUser.VendorId.HasValue)
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.VendorId#", "'" + ctx.CurrentUser.VendorId.Value + "'");
            }
            else
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.VendorId#", "'Unkown!'");
            }

            if (!string.IsNullOrEmpty(ctx.CurrentUser.CarBrandCode))
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.CarBrandCode#", "'" + ctx.CurrentUser.CarBrandCode + "'");
            }
            else
            {
                sqlcmd = sqlcmd.Replace("#env.CurrentUser.CarBrandCode#", "'Unkown!'");
            }
        }

        public static string ParseSQLCommand(IAppContext ctx, string sqlcmd, Dictionary<string, string> dict)
        {
            if (ctx == null)
                return sqlcmd;


            
            ParseConstSQL(ctx, ref sqlcmd);

            
            var regex = new Regex(
                "\\{(\\?){0,2}(\\w|\\s|\\(|\\)|\\.|,)+(not)?(\\s)*([\\=,in,be" +
                "tween,like,is]|(>\\=|<=|<>))(\\s)*(not)?([\\(,',%])*((\\#(\\w)+" +
                "\\#)|(\\$(\\w)+\\$))(\\w|\\s)*((\\#(\\w)+\\#)|(\\$(\\w)+\\$)" +
                ")?([\\),',%])*\\}",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                );
            var pregex = new Regex("((\\#(\\w)+\\#)|(\\$(\\w)+\\$))");
            MatchCollection matches = regex.Matches(sqlcmd);
            Dictionary<string, string> p = ctx.GetContextParams();
            if (dict != null && dict.Count > 0)
            {
                
                foreach (string key in dict.Keys)
                {
                    var tkey = key.ToLower();
                    if (!p.ContainsKey(tkey))
                    {
                        p.Add(tkey, dict[key]);
                    }
                }
            }
            bool checknull = true;
            int nullcount = 0;

            
            foreach (Match m in matches)
            {
                string mv = m.Value;
                checknull = mv.IndexOf("??") == 1;

                MatchCollection pmatches = pregex.Matches(mv);
                if (pmatches.Count > 0)
                {
                    nullcount = 0;
                    foreach (Match m2 in pmatches)
                    {
                        string mv2 = m2.Value;

                        string paramname = mv2.Replace("#", "").Replace("$", "").ToLower();
                        if (p.ContainsKey(paramname) && !string.IsNullOrEmpty(paramname) &&
                            !string.IsNullOrEmpty(p[paramname]))
                        {
                            if (mv2.IndexOf("#") >= 0)
                            {
                                if (mv.IndexOf("like", StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    mv = mv.Replace(m2.Value, p[paramname].Replace("'", "''"));
                                }
                                else
                                {
                                    
                                    if (mv.IndexOf(" in ", StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {
                                        string newValue = p[paramname].Replace("'", "''");
                                        string[] array = p[paramname].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
                                        if (array.Length > 1)
                                        {
                                            newValue = "'" + string.Join("','", p[paramname].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries)) + "'";
                                        }
                                        else
                                        {
                                            newValue = "'" + p[paramname] + "'";
                                        }
                                        mv = mv.Replace(m2.Value, newValue);
                                    }
                                    else
                                    {
                                        mv = mv.Replace(m2.Value, "'" + p[paramname].Replace("'", "''") + "'");
                                    }
                                }
                            }
                            else
                            {
                                
                                mv = mv.Replace(m2.Value, p[paramname].Replace("'", "''"));
                            }
                        }
                        else
                        {
                            if (checknull) 
                            {
                                mv = mv.Replace(m2.Value, "''");
                            }
                            else
                            {
                                nullcount++;
                            }
                        }
                    }
                    if (nullcount > 0)
                    {
                        sqlcmd = sqlcmd.Replace(m.Value, "");
                    }
                    else
                    {
                        sqlcmd = sqlcmd.Replace(m.Value, mv.TrimStart('{', '?').TrimEnd('}'));
                    }
                } 
                else
                {
                    
                    sqlcmd = sqlcmd.Replace(m.Value, "");
                }
            }
            return sqlcmd;
        }

        public static string ParseSQLCommand(IAppContext ctx, string sqlcmd, GetDataRuleFilter datarule)
        {
            return ParseSQLCommand(ctx, sqlcmd, null, datarule, null);
        }

        public static string ParseSQLCommand(IAppContext ctx, string sqlcmd, Dictionary<string, string> dict, GetDataRuleFilter datarule, string dataruleString)
        {
            if (datarule != null)
            {
                
                var dataruleRegex = new Regex("\\{@security:(\\w)+\\}",
                    RegexOptions.IgnoreCase
                    | RegexOptions.CultureInvariant
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled);
                var drpregex = new Regex("(\\{@security:)|(\\})");
                MatchCollection drmatches = dataruleRegex.Matches(sqlcmd);
                foreach (Match m in drmatches)
                {
                    string mv = m.Value;
                    string datarulesql = datarule(drpregex.Replace(mv, ""));
                    if (!string.IsNullOrEmpty(datarulesql))
                    {
                        sqlcmd = sqlcmd.Replace(m.Value, " AND " + datarulesql);
                    }
                    else
                    {
                        sqlcmd = sqlcmd.Replace(m.Value, "");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(dataruleString))
            {
                var sdataruleRegex = new Regex("\\{@security\\}",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled);
                if (sdataruleRegex.IsMatch(sqlcmd))
                {
                    
                    sqlcmd = sdataruleRegex.Replace(sqlcmd, string.IsNullOrEmpty(dataruleString) ? "" : (" AND " + dataruleString));
                }
                else if (!string.IsNullOrEmpty(dataruleString))
                {
                    
                    sqlcmd = sqlcmd + " AND " + dataruleString;
                }
            }

            return ParseSQLCommand(ctx, sqlcmd, dict);
        }

        public static string ParseSQLCommand(IAppContext ctx, string sqlcmd, Dictionary<string, string> dict, string datarule)
        {
            return ParseSQLCommand(ctx, sqlcmd, dict, null, datarule);
        }

        
        
        
        
        
        
        
        
        
        
        public static bool CheckTheRelation(string id, string targettable, string targetField, string additionWhere,
            out DataSet dataRelations)
        {
            bool hasRecord = false;
            DataSet ds = null;

            var sb = new StringBuilder();
            string[] tables = targettable.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] fields = targetField.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (tables.Length != fields.Length)
                throw new ArgumentException("传入的表格参数和字段参数数量不一置");

            for (int i = 0; i < tables.Length; i++)
            {
                sb.AppendFormat("select top 1 * from {0} where {1} = '{2}'", tables[i], fields[i], id);
                if (!string.IsNullOrEmpty(additionWhere))
                    sb.Append(" and " + additionWhere);
                sb.Append(";");
            }
            try
            {
                var helper = GetDBHelper(DEFAULT_DBNAME);
                ds = helper.Query(sb.ToString());

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            hasRecord = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dataRelations = null;
                throw ex;
            }

            dataRelations = ds;
            return hasRecord;
        }

        
        
        
        
        
        
        
        
        
        
        public static bool SimpleCheckTheRelation(string id, string targettable, string targetField,
            string additionWhere)
        {
            bool hasrecord = false;

            var sb = new StringBuilder();
            string[] tables = targettable.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] fields = targetField.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (tables.Length != fields.Length)
                throw new ArgumentException("传入的表格参数和字段参数数量不一置");

            for (int i = 0; i < tables.Length; i++)
            {
                sb.AppendFormat("select count(*) from {0} where {1} = '{2}'", tables[i], fields[i], id);
                if (!string.IsNullOrEmpty(additionWhere))
                    sb.Append(" and " + additionWhere);
                sb.Append(";");
            }

            try
            {
                using (var conn = ManualDbService())
                {
                    hasrecord = conn.Query<int>(sb.ToString(), null).Any();
                    conn.Close();
                }
            }
            catch (DataException dex)
            {
                throw dex;
            }
            return hasrecord;
        }

        public string FilterErrChar(string input)
        {
            if (!string.IsNullOrEmpty(input))
                return input.Replace("'", "");
            return input;
        }

        #endregion
    }
}