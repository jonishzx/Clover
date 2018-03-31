namespace Clover.Core.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Data;    
    using System.Text;
    using System.Reflection;
    
    
    
    public class DataTableHelper
    {
        
        
        
        
        
        
        public static void CheckColumnExists(DataTable dt, string[] fields,string msgfld) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if(i > 0)
                    sb.Remove(0, sb.Length - 1);

                for (int j = 0; j < fields.Length;j++ )
                {
                    if (dt.Rows[i][fields[j]] == DBNull.Value || dt.Rows[i][fields[j]].ToString() == string.Empty)
                    {
                        sb.AppendFormat("第{1}列数据不能为空|",(i+1),dt.Columns[fields[j]].Ordinal+1);
                    }
                }
                if (sb.Length > 0)
                    dt.Rows[i][msgfld] = sb.ToString();
            }
        }

        
        
        
        
        
        
        public static DataTable GetTop(DataTable dt, int top)
        {
            while (dt.Rows.Count > top)
            {
                dt.Rows.RemoveAt(dt.Rows.Count - 1);
            }
            return dt;
        }

        
        
        
        
        
        
        
        public static DataTable GetRange(DataTable dt,int start ,int end)
        {
            if (end <= start)
                return dt;

            if (dt.Rows.Count > start)
            {
                
                int maxcount = dt.Rows.Count - end;
                while (maxcount > 0)
                {
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    maxcount--;
                }

                while (start>0)
                {
                    dt.Rows.RemoveAt(0);
                    start--;
                }
            }
            return dt;
        }
        
        
        
        
        
        
        
        
        
        
        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize, string where, string sort, out int rowscount)
        {
            rowscount = 0;

            if (PageIndex == 0)
                return dt;
            DataTable newdt = dt.Clone();
            
            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            DataView dv = new DataView(dt);
            dv.Sort = sort;
            dv.RowFilter = where;

            
            rowscount = dv.Count;

            if (rowbegin >= rowscount)
                return newdt;

            if (rowend > rowscount)
                rowend = rowscount;

            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRowView drv = dv[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = drv[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }

            return newdt;
        }

        
        
        
        
        
        public static DataTable EntityListToDataTable<T>(List<T> entityList)
        {
            DataTable dt = new DataTable();

            
            Type entityType = typeof(T);
            PropertyInfo[] entityProperties = entityType.GetProperties();
            Type colType = null;
            foreach (PropertyInfo propInfo in entityProperties)
            {

                if (propInfo.PropertyType.IsGenericType)
                {
                    colType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                }
                else
                {
                    colType = propInfo.PropertyType;
                }

                if (colType.FullName.StartsWith("System"))
                {
                    dt.Columns.Add(propInfo.Name, colType);
                }
            }

            if (entityList != null && entityList.Count > 0)
            {
                foreach (T entity in entityList)
                {
                    DataRow newRow = dt.NewRow();
                    foreach (PropertyInfo propInfo in entityProperties)
                    {
                        if (dt.Columns.Contains(propInfo.Name))
                        {
                            object objValue = propInfo.GetValue(entity, null);
                            newRow[propInfo.Name] = objValue == null ? DBNull.Value : objValue;
                        }
                    }
                    dt.Rows.Add(newRow);
                }
            }

            return dt;
        }


        
        
        
        
        
        
        public static List<T> DataTableToEntityList<T>(DataTable dt)
        {
            List<T> entiyList = new List<T>();

            Type entityType = typeof(T);
            PropertyInfo[] entityProperties = entityType.GetProperties();

            foreach (DataRow row in dt.Rows)
            {
                T entity = Activator.CreateInstance<T>();

                foreach (PropertyInfo propInfo in entityProperties)
                {
                    if (dt.Columns.Contains(propInfo.Name))
                    {
                        if (!row.IsNull(propInfo.Name))
                        {
                            propInfo.SetValue(entity, row[propInfo.Name], null);
                        }
                    }
                }

                entiyList.Add(entity);
            }

            return entiyList;
        }

        
        
        
        
        
        
        
        private static bool DRColumnCompare(DataRow objA, DataRow objB, string[] arrFieldName)
        {
            foreach (string obj in arrFieldName)
            {
                if (objA[obj] == DBNull.Value && objB[obj] == DBNull.Value)
                    return false;
                else if (objA[obj].ToString() != objB[obj].ToString())
                    return false;
            }

            return true;
        }

        
        
        
        
        
        private static void RemoveEmptyRow(DataTable dt, string[] checkColumns)
        {
            foreach (DataRow dr in dt.Rows) {
                foreach (string col in checkColumns) {
                    if (dr[col] == DBNull.Value || string.IsNullOrEmpty(dr[col].ToString()))
                    {
                        dr.Delete();
                    }
                }
            }

            dt.AcceptChanges();
            
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public static DataTable unpivot(string strTableName, DataTable dtSourceTable, string strTargetColumn, string strComputeColumn, string[] strExchangeValues, string[] showColumns, bool merageRow)
        {
            DataTable dtTable = new DataTable(strTableName);

            
            

            
            dtTable.Columns.Add(strTargetColumn);
            
            foreach (string newcol in showColumns)
            {
                dtTable.Columns.Add(newcol);
            }
            
            dtTable.Columns.Add(strComputeColumn);

            
            DataView dv = new DataView(dtSourceTable);

            if (merageRow || showColumns.Length == 0)
            {

                foreach (string colValue in strExchangeValues)
                {
                    
                    DataRow newdr = dtTable.NewRow();
                    newdr[strTargetColumn] = colValue;
                    dtTable.Rows.Add(newdr);
                }


                
                foreach (DataRow dr in dtTable.Rows)
                {

                    List<string> distinctValues = DataSetSelectDistinct(dv, dr[strTargetColumn].ToString());
                    
                    dr[strComputeColumn] = string.Join("||", distinctValues.ToArray());
                }

            }
            else
            {
                foreach (string col in showColumns)
                {
                    
                    DataTable distincttable = DataSetSelectDistinct("Test", dtSourceTable, string.Join(",", showColumns));

                    foreach (DataRow dr in distincttable.Rows)
                    {

                        foreach (string colValue in strExchangeValues)
                        {
                            
                            DataRow newdr = dtTable.NewRow();
                            newdr[strTargetColumn] = colValue;

                            foreach (string editcol in showColumns)
                            {
                                newdr[editcol] = dr[editcol].ToString();
                            }
                            dtTable.Rows.Add(newdr);
                        }
                    }
                }

                
                foreach (DataRow dr in dtTable.Rows)
                {
                    List<string> distinctValues = DataSetSelectDistinct(dv, dr[strTargetColumn].ToString());
                    
                    dr[strComputeColumn] = string.Join("||", distinctValues.ToArray());
                }

            }

            return dtTable;
        }

        
        
        
        
        
        
        
        public static List<string> DataSetSelectDistinct(DataView dtSourceTable, string strFieldName)
        {
            List<string> rst = new List<string>();

            foreach (DataRowView dr in dtSourceTable)
            {
                if (!rst.Contains(dr[strFieldName].ToString()))
                    rst.Add(dr[strFieldName].ToString());
            }

            return rst;
        }

         
        
        
        
        
        
        
        public static List<string> DataSetSelectDistinct(DataTable dtSourceTable, string strFieldName)
        {
            List<string> rst = new List<string>();

            foreach (DataRow dr in dtSourceTable.Rows) {
                if (!rst.Contains(dr[strFieldName].ToString()))
                    rst.Add(dr[strFieldName].ToString());
            }

            return rst;
        }
        
        
        
        
        
        
        
        public static DataTable DataSetSelectDistinct(string strTableName, DataTable dtSourceTable, string strFieldNames)
        {

            DataRow objLastValue = null;


            DataTable dtTable = new DataTable(strTableName);

            string[] arrFieldName = null;

            try
            {
                if (strFieldNames == string.Empty)
                    return dtSourceTable;

                arrFieldName = strFieldNames.Split(',');

                
                foreach (string obj in arrFieldName)
                {
                    dtTable.Columns.Add(obj, dtSourceTable.Columns[obj].DataType);
                }

                
                foreach (DataRow drRow in dtSourceTable.Select("", strFieldNames))
                {

                    if (objLastValue == null || !DRColumnCompare(objLastValue, drRow, arrFieldName))
                    {
                        objLastValue = dtTable.NewRow();

                        foreach (DataColumn dr in dtTable.Columns)
                        {
                            objLastValue[dr.ColumnName] = drRow[dr.ColumnName];
                        }

                        dtTable.Rows.Add(objLastValue);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
            return dtTable;
        }
        

        
        
        
        
        
        
        
        
        
        public static DataTable DataSetSelectGroupBy(string strTableName, DataTable dtSourceTable, List<string> strGroupFieldNames, List<string> computeField)
        {
            
            DataTable groupdt = dtSourceTable.Clone();

            #region 定义公共变量

            
            bool needaddgroup = false;
            
            StringBuilder computefilter = new StringBuilder();
            
            StringBuilder groupfilter = new StringBuilder();
            
            DataRow[] selectrows = dtSourceTable.Select("", strGroupFieldNames[strGroupFieldNames.Count - 1]);

            
            int totallen = dtSourceTable.Select("", strGroupFieldNames[strGroupFieldNames.Count - 1]).Length;

            #endregion

            for (int i = 0; i < totallen; i++)
            {

                #region 计算小计

                if (i <= totallen - 1) 
                {

                    int filterindex = -1;

                    
                    for (int j = 0; j < strGroupFieldNames.Count; j++)
                    {
                        
                        
                        
                        
                        
                        string strGroupFieldNameString = strGroupFieldNames[j];


                        #region 分组过滤条件

                        
                        filterindex = strGroupFieldNameString.IndexOf("filter:");

                        
                        string filterString = null;

                        if (filterindex >= 0) 
                        {

                            filterString = strGroupFieldNameString.Substring(
                               (filterindex + "filter:".Length),
                               strGroupFieldNameString.IndexOf(",") - (filterindex + "filter:".Length));

                            
                            strGroupFieldNameString = strGroupFieldNameString.Replace("filter:" + filterString + ",", "");

                            
                            string[] tmp = filterString.Split('|');

                            
                            filterString = string.Empty;

                            foreach (string tmp2 in tmp)
                            {
                                filterString += tmp2;
                                filterString += ",";
                            }

                            filterString = filterString.Remove(filterString.Length - 1, 1);
                        }
                        #endregion

                        
                        string[] groupfields = strGroupFieldNameString.Split(',');
                        
                        string[] computefields = computeField[j].Split(',');

                        string filter = "";


                        if (filterString != null) 
                        {
                            
                            DataTable distinctdt = DataSetSelectDistinct("filterDT", dtSourceTable, filterString);


                            Dictionary<string, string> oldFilterColumnValue = new Dictionary<string, string>();
                            foreach (DataColumn dc in distinctdt.Columns) 
                            {
                                
                                oldFilterColumnValue.Add(dc.ColumnName, selectrows[i][dc.ColumnName].ToString());

                                filter += string.Format(" {0} = '{1}'", dc.ColumnName, selectrows[i][dc.ColumnName].ToString());

                                filter += " and ";
                            }

                            filter += "1=1";




                            DataRow[] filterdrs = distinctdt.Select(filter, "");
                            foreach (DataRow dr in filterdrs)
                            {
                                
                                foreach (string fieldname in groupfields)
                                {
                                    if (i == totallen - 1 || !(selectrows[i][fieldname.Trim()].ToString().Equals(selectrows[i + 1][fieldname.Trim()].ToString())))
                                    {

                                        needaddgroup = true;

                                    }
                                    
                                    computefilter.AppendFormat("{0}='{1}'", fieldname, selectrows[i][fieldname].ToString());
                                    computefilter.AppendFormat(" and ");
                                }


                                foreach (DataColumn dc in distinctdt.Columns) 
                                {
                                    if (i == totallen - 1 || !(selectrows[i][dc.ColumnName].ToString().Equals(selectrows[i + 1][dc.ColumnName].ToString())))
                                        needaddgroup = true;

                                    computefilter.AppendFormat(" {0} = '{1}'", dc.ColumnName, dr[dc.ColumnName].ToString());
                                    computefilter.AppendFormat(" and ");


                                }

                                
                                computefilter.Append("1=1");

                                if (needaddgroup) 
                                {
                                    
                                    DataRow computedr = groupdt.NewRow();


                                    foreach (string fieldname in groupfields)
                                    {
                                        computedr[fieldname] = selectrows[i][fieldname].ToString();
                                    }

                                    if (oldFilterColumnValue.Count >= 0) 
                                    {
                                        string[] keys = new string[oldFilterColumnValue.Keys.Count];
                                        oldFilterColumnValue.Keys.CopyTo(keys, 0);

                                        for (int jj = 0; jj < keys.Length; jj++)
                                        {

                                            computedr[keys[jj]] = oldFilterColumnValue[keys[jj]];
                                        }

                                    }

                                    foreach (string computefieldname in computefields) 
                                    {
                                        computedr[computefieldname.Trim()] =
                                            dtSourceTable.Compute(string.Format(" sum({0}) ", computefieldname), computefilter.ToString());
                                    }

                                    
                                    groupdt.Rows.InsertAt(computedr, groupdt.Rows.Count); 
                                }

                                
                                needaddgroup = false;
                                
                                computefilter.Remove(0, computefilter.Length);

                                filterindex = -1;
                            }

                        }
                        else 
                        {

                            #region 非过滤条件计算

                            
                            foreach (string fieldname in groupfields)
                            {
                                if (i == totallen - 1 || !selectrows[i][fieldname.Trim()].ToString().Equals(selectrows[i + 1][fieldname.Trim()].ToString()))
                                {

                                    needaddgroup = true;

                                }
                                
                                computefilter.AppendFormat("{0}='{1}'", fieldname, selectrows[i][fieldname].ToString());
                                computefilter.AppendFormat(" and ");
                            }

                            computefilter.Append("1=1");

                            if (needaddgroup) 
                            {
                                
                                DataRow computedr = groupdt.NewRow();

                                foreach (string fieldname in groupfields)
                                {
                                    computedr[fieldname] = selectrows[i][fieldname].ToString();                                    
                                }

                                foreach (string computefieldname in computefields)
                                {
                                    computedr[computefieldname.Trim()] =
                                        dtSourceTable.Compute(string.Format(" sum({0}) ", computefieldname), computefilter.ToString());
                                }

                                
                                groupdt.Rows.InsertAt(computedr, groupdt.Rows.Count); 
                            }

                            
                            needaddgroup = false;
                            
                            computefilter.Remove(0, computefilter.Length);

                            filterindex = -1;

                            #endregion

                        }

                    }
                }
                #endregion
            }

            return groupdt;
        }
    }

    
    
    
    public class DataTableRowsChecker
    {
        public delegate void PassRowHandler(DataTable comparedt,DataRow dr) ;
        public delegate void UnPassRowHandler(DataTable comparedt, DataRow dr);

        
        
        
        public event PassRowHandler PassRowEvent = null;
        
        
        
        public event UnPassRowHandler UnPassRowEvent = null;

        public void CheckRows(DataTable comparedt, DataTable srcdt, string compareColumns, out List<DataRow> passlist, out List<DataRow> unpasslist)
        {
            DataTable compareDistinctDt = DataTableHelper.DataSetSelectDistinct("", comparedt, compareColumns);
            DataTable srcDistinctDt = DataTableHelper.DataSetSelectDistinct("", srcdt, compareColumns);

            string[] comparecols = compareColumns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();

            passlist = new List<DataRow>();
            unpasslist = new List<DataRow>();

            foreach (DataRow dr in compareDistinctDt.Rows)
            {
                sb.Append("1=1 ");
                foreach (string col in comparecols)
                {
                    sb.AppendFormat(" and {0} = '{1}'",col,dr[col].ToString());
                }
                
                DataRow[] passrows = srcDistinctDt.Select(sb.ToString());
                if (passrows.Length > 0)
                {
                    passlist.Add(dr);
                    if (PassRowEvent != null)
                        PassRowEvent(comparedt,dr);
                }
                else                
                {
                    unpasslist.Add(dr);
                    
                    if (UnPassRowEvent != null)
                        UnPassRowEvent(comparedt,dr);
                }
                sb.Remove(0, sb.Length);
            }
        }
    }
}
