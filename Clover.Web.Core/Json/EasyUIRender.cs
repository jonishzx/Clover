using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Clover.Web.Json
{
    
    
    
    public class EasyUIRender
    {
        
        
        
        
        
        
        
        public static string GetDataGridJson(System.Data.DataTable dt, string tablename, int totalcount)
        {
            var sb = new StringBuilder();
            sb.Append("{\"total\":" + totalcount.ToString() + ",\"rows\":");
            dt.TableName = tablename;
            sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(dt, new Newtonsoft.Json.Converters.IsoDateTimeConverter()));
            sb.Append("}");
            return sb.ToString();
        }

        
        
        
        
        
        
        public static string GetDataGridJson(System.Data.DataTable dt, int totalcount)
        {
            return GetDataGridJson(dt, "Temp", totalcount);
        }

        
        
        
        
        
        
        public static string GetDataGridJson<T>(List<T> list, int totalcount)
        {

            string jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(list, new Newtonsoft.Json.Converters.IsoDateTimeConverter());

            var sb = new StringBuilder();
            sb.Append("{\"total\":" + totalcount.ToString() + ",\"rows\":");
            sb.Append(jsonstr != "null" ? jsonstr : "[]");
            sb.Append("}");
            return sb.ToString();
        }

        
        
        
        
        
        
        public string DataTableJsonStr(DataTable dt, int totalcount)
        {
            return GetDataGridJson(dt, totalcount, null);
        }


        
        
        
        
        
        
        
        public string GetDataGridJson(DataTable dt, int totalcount, string[] fields)
        {
            var builder = new StringBuilder();
            builder.Append("{");
            builder.AppendFormat("\"total\":\"{0}\",\"rows\":[", totalcount);
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                foreach (DataRow row in dt.Rows)
                {
                    int num;
                    builder.Append("{");
                    if (fields == null)
                    {
                        num = 0;
                        while (num < dt.Columns.Count)
                        {
                            builder.AppendFormat("\"{0}\":\"{1}\",", dt.Columns[num].ColumnName, row[dt.Columns[num].ColumnName].ToString().Replace(@"\", @"\\"));
                            num++;
                        }
                    }
                    else
                    {
                        for (num = 0; num < fields.Length; num++)
                        {
                            builder.AppendFormat("\"{0}\":\"{1}\",", fields[num], row[fields[num]].ToString().Replace(@"\", @"\\"));
                        }
                    }
                    builder.Remove(builder.Length - 1, 1);
                    builder.Append("},");
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("]}");
            return builder.ToString();
        }


    }
}
