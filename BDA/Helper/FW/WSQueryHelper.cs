using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json.Linq;
using BDA.DataModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Odbc;
using System.Text.RegularExpressions;

namespace BDA.Helper.FW
{
    public class WSQueryHelper
    {
        private static void PrepareProps(WSQueryProperties props, string limitExport, bool isChart, DataSourceLoadOptions loadOptions, bool forHive = false)
        {

            if (props.SqlParameters == null)
            {
                props.SqlParameters = new List<SqlParameter>();
            }

            if (loadOptions.Filter != null && loadOptions.Filter.Count > 0)
            {

                //var insideWhereQuery = GetSqlExprByArray(loadOptions.Filter.Cast<object>().ToArray(), props.SqlParameters);
                JArray filterList = JArray.FromObject(loadOptions.Filter.Cast<object>().ToArray());
                var insideWhereQuery = GetSqlExprByArray(filterList.ToObject<object[]>(), props.SqlParameters);
                props.CountQuery = "SELECT count(*) from (" + Environment.NewLine + props.Query + Environment.NewLine + ") cwhere" +
                   Environment.NewLine + "WHERE " + insideWhereQuery;


                props.Query = "SELECT * from (" + Environment.NewLine + props.Query + Environment.NewLine + ") cwhere" +
                    Environment.NewLine + "WHERE " + insideWhereQuery;

            }
            else
            {
                //default = count(*) dari query select
                props.CountQuery = @" select count(*) from (
                    " +
                    props.Query + @"
                ) cntquery";
            }



            //generate order by query
            var ordByQuery = "1";
            if (loadOptions.Sort != null && loadOptions.Sort.Any())
            {
                ordByQuery = "";
                foreach (var srt in loadOptions.Sort)
                {
                    if (ordByQuery != "") { ordByQuery += ", "; }
                    ordByQuery += srt.Selector + " " + (srt.Desc == true ? "desc" : "asc");
                }
            }



            if (loadOptions.Take > 0 || loadOptions.Skip > 0)
            {
                var takeQuery = $"ORDER BY {ordByQuery} {Environment.NewLine} OFFSET {loadOptions.Skip} ROWS FETCH NEXT {loadOptions.Take} ROWS ONLY";
                if (forHive)
                {
                    takeQuery = $"ORDER BY {ordByQuery} {Environment.NewLine} LIMIT {loadOptions.Skip},{loadOptions.Take}";
                    takeQuery = takeQuery.Replace("ORDER BY 1", "");
                }

                props.Query = "SELECT * from (" + Environment.NewLine + props.Query + Environment.NewLine + ") cord" +
                                Environment.NewLine + takeQuery;
            }
            else if (loadOptions.Take == 0 && loadOptions.RequireTotalCount == false && isChart == false)
            {
                var takeQuery = $"ORDER BY {ordByQuery} {Environment.NewLine} OFFSET 0 ROWS FETCH NEXT " + limitExport + " ROWS ONLY";
                if (forHive)
                {
                    takeQuery = $"ORDER BY {ordByQuery} {Environment.NewLine} LIMIT 0," + limitExport;
                    takeQuery = takeQuery.Replace("ORDER BY 1", "");
                }

                props.Query = "SELECT * from (" + Environment.NewLine + props.Query + Environment.NewLine + ") cord" +
                                Environment.NewLine + takeQuery;
            }
            else if (ordByQuery != "1")
            {
                //ada sort disini ya, walaupun ngga ada take dan skip
                props.Query = "SELECT * from (" + Environment.NewLine + props.Query + Environment.NewLine + ") cord" +
                                Environment.NewLine + $"ORDER BY {ordByQuery}";

            }
        }
        private static void PreparePropsNL(WSQueryProperties props, string limitExport, bool isChart, bool forHive = false)
        {

            if (props.SqlParameters == null)
            {
                props.SqlParameters = new List<SqlParameter>();
            }

            props.CountQuery = @" select count(*) from (
                    " +
                    props.Query + @"
                ) cntquery";



            //generate order by query
            var ordByQuery = "1";


        }
        private static WSQueryReturns ExecuteCommandSQLServer(string connString, WSQueryProperties props, DataSourceLoadOptions loadOptions)
        {
            var result = new WSQueryReturns();
            using (var conn = new SqlConnection(connString))
            {
                if (props.CountQuery.Contains("@devx"))
                {
                    foreach (var prm in props.SqlParameters)
                    {
                        if (prm.ParameterName.Contains("devx"))
                        {
                            if (props.CountQuery.Contains(prm.ParameterName))
                            {
                                props.CountQuery = props.CountQuery.Replace(prm.ParameterName, prm.Value.ToString());

                            }
                            if (props.Query.Contains(prm.ParameterName))
                            {
                                props.Query = props.Query.Replace(prm.ParameterName, prm.Value.ToString());

                            }
                        }
                    }
                }
                using (var cmd = new SqlCommand(props.CountQuery, conn))
                {
                    cmd.CommandTimeout = 300;
                    //foreach (var prm in props.SqlParameters)
                    //{
                    //    cmd.Parameters.Add(prm);
                    //}
                    if (!props.CountQuery.Contains("@devx"))
                    {
                        foreach (var prm in props.SqlParameters)
                        {
                            cmd.Parameters.Add(prm);
                        }
                    }
                    conn.Open();

                    if (loadOptions.RequireTotalCount)
                    {
                        result.totalCount = (int)cmd.ExecuteScalar();

                    }

                    var dt = new DataTable();


                    if (result.totalCount == null || result.totalCount > 0) //hemat 1 kali call 
                    {
                        cmd.CommandText = props.Query;
                        var adap = new SqlDataAdapter(cmd);
                        adap.Fill(dt);
                    }
                    result.data = dt;
                }
            }
            return result;
        }
        private static WSQueryReturns ExecuteCommandSQLServerNL(string connString, WSQueryProperties props)
        {
            var result = new WSQueryReturns();
            foreach (var prm in props.SqlParameters)
            {
                if (props.CountQuery.Contains(prm.ParameterName))
                {
                    props.CountQuery = props.CountQuery.Replace(prm.ParameterName, prm.Value.ToString());
                }
                if (props.Query.Contains(prm.ParameterName))
                {
                    props.Query = props.Query.Replace(prm.ParameterName, prm.Value.ToString());
                }
            }
            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand(props.CountQuery, conn))
                {
                    cmd.CommandTimeout = 300;
                    //foreach (var prm in props.SqlParameters)
                    //{
                    //    cmd.Parameters.Add(prm);
                    //}

                    conn.Open();

                    result.totalCount = (int)cmd.ExecuteScalar();

                    var dt = new DataTable();


                    if (result.totalCount == null || result.totalCount > 0) //hemat 1 kali call 
                    {
                        cmd.CommandText = props.Query;
                        var adap = new SqlDataAdapter(cmd);
                        adap.Fill(dt);
                    }
                    result.data = dt;
                }
            }
            return result;
        }
        private static WSQueryReturns ExecuteCommandHiveOdbc(string connString, WSQueryProperties props, DataSourceLoadOptions loadOptions)
        {
            var result = new WSQueryReturns();
            //hive bedanya disini. schema = ojkdt, dm_periode jadi dm_pperiode (partisi)
            props.CountQuery = props.CountQuery.Replace("dbo.", "ojkdt.").Replace("dm_periode", "dm_pperiode");
            props.Query = props.Query.Replace("dbo.", "ojkdt.").Replace("dm_periode", "dm_pperiode");
            props.CountQuery = props.CountQuery.Replace("dbo.", "ojkdt.").Replace("dm_bulan_data", "dm_pperiode");
            props.Query = props.Query.Replace("dbo.", "ojkdt.").Replace("dm_bulan_data", "dm_pperiode");

            //simpan dulu urutan matches query disini
            MatchCollection matches_count = Regex.Matches(props.CountQuery, "\\@([\\w.$]+|\"[^\"]+\"|'[^']+')");
            MatchCollection matches_query = Regex.Matches(props.Query, "\\@([\\w.$]+|\"[^\"]+\"|'[^']+')");

            //odbc parameter ngga bisa dipassing?
            foreach (Match mtc in matches_query) //loop sesuai urutan
            {
                var prm = props.SqlParameters.Where(x => x.ParameterName.ToLower() == mtc.Value.ToLower()).FirstOrDefault();
                if (mtc.Value.Contains("devx"))
                {
                    prm = props.SqlParameters.Where(x => x.ParameterName.ToLower() == mtc.Value.ToLower()).FirstOrDefault();
                }
                else
                {
                    prm = props.SqlParameters.Where(x => x.ParameterName.ToLower() == mtc.Value.Replace("@", "").ToLower()).FirstOrDefault();
                }
                if (prm == null) throw new InvalidOperationException("No param for : @" + mtc.Value);


                var toReplace = "";
                //beda lain, untuk hive, periode = string yyyyMM
                if (prm.ParameterName == "periode")
                {
                    toReplace = string.Format("{0:yyyyMM}", prm.Value);
                }
                else
                {
                    toReplace = prm.Value.ToString();
                }

                props.CountQuery = props.CountQuery.Replace(mtc.Value.ToLower(), toReplace);
                props.Query = props.Query.Replace(mtc.Value.ToLower(), toReplace);
            }

            using (var conn = new OdbcConnection(connString))
            {

                using (var cmd = new OdbcCommand(props.CountQuery, conn))
                {
                    cmd.CommandTimeout = 300;

                    conn.Open();

                    if (loadOptions.RequireTotalCount)
                    {
                        Console.WriteLine(props.CountQuery);
                        result.totalCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (loadOptions.Take > 0 && result.totalCount <= loadOptions.Take) //test hardcode dulu 1000 sebenarnya harusnya <= Take
                        {
                            //ini count nya lebih < yang dibutuhkan, hapus LIMIT dari query (yang terakhir)
                            props.Query = props.Query.Substring(0, props.Query.LastIndexOf("LIMIT"));
                        }


                    }

                    var dt = new DataTable();


                    if (result.totalCount == null || result.totalCount > 0) //hemat 1 kali call 
                    {
                        cmd.CommandText = props.Query;
                        var adap = new OdbcDataAdapter(cmd);
                        adap.Fill(dt);
                    }
                    foreach (DataColumn column in dt.Columns)
                    {
                        column.ColumnName = column.ColumnName.Replace("cord.", "");
                        column.ColumnName = column.ColumnName.Replace("x.", "");
                    }

                    result.data = dt;
                }
            }

            return result;
        }
        private static WSQueryReturns ExecuteCommandHiveOdbcNL(string connString, WSQueryProperties props)
        {
            var result = new WSQueryReturns();
            //hive bedanya disini. schema = ojkdt, dm_periode jadi dm_pperiode (partisi)
            props.CountQuery = props.CountQuery.Replace("dbo.", "ojkdt.").Replace("dm_periode", "dm_pperiode");
            props.Query = props.Query.Replace("dbo.", "ojkdt.").Replace("dm_periode", "dm_pperiode");


            //simpan dulu urutan matches query disini
            MatchCollection matches_count = Regex.Matches(props.CountQuery, "\\@([\\w.$]+|\"[^\"]+\"|'[^']+')");
            MatchCollection matches_query = Regex.Matches(props.Query, "\\@([\\w.$]+|\"[^\"]+\"|'[^']+')");

            //odbc parameter ngga bisa dipassing?
            foreach (Match mtc in matches_query) //loop sesuai urutan
            {
                var prm = props.SqlParameters.Where(x => x.ParameterName.ToLower() == mtc.Value.ToLower()).FirstOrDefault();
                if (mtc.Value.Contains("devx"))
                {
                    prm = props.SqlParameters.Where(x => x.ParameterName.ToLower() == mtc.Value.ToLower()).FirstOrDefault();
                }
                else
                {
                    prm = props.SqlParameters.Where(x => x.ParameterName.ToLower() == mtc.Value.Replace("@", "").ToLower()).FirstOrDefault();
                }
                if (prm == null) throw new InvalidOperationException("No param for : @" + mtc.Value);


                var toReplace = "";
                //beda lain, untuk hive, periode = string yyyyMM
                if (prm.ParameterName == "periode")
                {
                    toReplace = string.Format("{0:yyyyMM}", prm.Value);
                }
                else
                {
                    toReplace = prm.Value.ToString();
                }

                props.CountQuery = props.CountQuery.Replace(mtc.Value.ToLower(), toReplace);
                props.Query = props.Query.Replace(mtc.Value.ToLower(), toReplace);
            }

            using (var conn = new OdbcConnection(connString))
            {

                using (var cmd = new OdbcCommand(props.CountQuery, conn))
                {
                    cmd.CommandTimeout = 300;

                    conn.Open();

                    var dt = new DataTable();

                    result.totalCount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (result.totalCount == null || result.totalCount > 0) //hemat 1 kali call 
                    {
                        cmd.CommandText = props.Query;
                        var adap = new OdbcDataAdapter(cmd);
                        adap.Fill(dt);
                    }
                    foreach (DataColumn column in dt.Columns)
                    {
                        column.ColumnName = column.ColumnName.Replace("cord.", "");
                        column.ColumnName = column.ColumnName.Replace("x.", "");
                    }

                    result.data = dt;
                }
            }

            return result;
        }

        public static WSQueryReturns DoQuery(DataEntities db, WSQueryProperties props, DataSourceLoadOptions loadOptions, bool isChart, bool forHive = false)
        {
            PrepareProps(props, db.GetSetting("LimitExportExcelPDF"), isChart, loadOptions, forHive);
            if (forHive)
            {
                return ExecuteCommandHiveOdbc("DSN=" + db.GetSetting("HiveDSN"), props, loadOptions);
            }
            else
            {
                return ExecuteCommandSQLServer(db.appSettings.DataConnString, props, loadOptions);
            }
        }
        public static WSQueryReturns DoQueryNL(DataEntities db, WSQueryProperties props, bool isChart, bool forHive = false)
        {
            PreparePropsNL(props, db.GetSetting("LimitExportExcelPDF"), isChart, forHive);
            if (forHive)
            {
                return ExecuteCommandHiveOdbcNL("DSN=" + db.GetSetting("HiveDSN"), props);
            }
            else
            {
                return ExecuteCommandSQLServerNL(db.appSettings.DataConnString, props);
            }
        }



        private static string _GetSimpleSqlExpr(object[] expression, List<SqlParameter> sqlParamList)
        {
            //TODO : ini masih bisa kena sql injection
            string result = "";
            int itemsCount = expression.Length;
            string fieldName = expression[0].ToString();
            if (itemsCount == 2)
            {
                var val = expression[1];
                result = String.Format("{0} = {1}", fieldName, val);
            }
            else if (itemsCount == 3)
            {
                string clause = expression[1].ToString();
                var val = expression[2];
                string pattern = "";

                val = WSQueryStore.CheckAndModifyFilterVal(expression[0].ToString(), val.ToString());

                switch (clause)
                {
                    case "=":
                    case "<>":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":
                        if (val.GetType().Name == "Int64")
                        {
                            pattern = "{0} {1} {2}";
                        }
                        else
                        {
                            pattern = "{0} {1} '{2}'";
                        }
                        break;
                    case "startswith":
                        pattern = "LOWER({0}) {1} '{2}%'";
                        clause = " LIKE ";
                        break;
                    case "endswith":
                        pattern = "LOWER({0}) {1} '%{2}'";
                        clause = " LIKE ";
                        break;
                    case "contains":
                        pattern = "LOWER({0}) {1} '%{2}%'";
                        clause = " LIKE ";
                        break;
                    case "notcontains":
                        pattern = "LOWER({0}) {1} '%{2}%'";
                        clause = String.Format("{0} {1}", " NOT ", " LIKE ");
                        break;
                    default:
                        clause = "";
                        break;
                }


                var namaParam = "@devx_" + sqlParamList.Count.ToString();

                //bikin param, lalu masukin ke sqlParamList
                result = string.Format(pattern, fieldName, clause, namaParam);
                if (clause == " LIKE ")
                {
                    sqlParamList.Add(new SqlParameter(namaParam, SqlDbType.VarChar) { Value = val.ToString().ToLower() });
                }
                else
                {
                    sqlParamList.Add(new SqlParameter(namaParam, SqlDbType.VarChar) { Value = val });
                }

            }
            return result;
        }

        private static string GetSqlExprByArray(object[] expression, List<SqlParameter> sqlParamList)
        {
            string result = "(";
            Boolean prevItemWasArray = false;
            int index = 0;
            foreach (var item in expression)
            {
                string nameObj = item.GetType().Name;
                //Console.WriteLine(nameObj);
                if (item.GetType().Name == "String")
                {
                    prevItemWasArray = false;
                    if (index == 0)
                    {
                        if (item.ToString() == "!")
                        {
                            result += " NOT ";
                            continue;
                        }
                        result += _GetSimpleSqlExpr(expression, sqlParamList);
                        break;
                    }
                    string strItem = item.ToString().ToUpper();
                    if (strItem == "AND" || strItem == "OR")
                    {
                        result += string.Format(" {0} ", strItem);
                    }
                    continue;
                }



                if (item.GetType().Name == "JArray")
                {
                    if (prevItemWasArray)
                    {
                        result += string.Format(" {0} ", "AND");
                    }
                    result += GetSqlExprByArray(((JArray)item).ToObject<object[]>(), sqlParamList);
                    prevItemWasArray = true;
                }

                index++;
            }
            result += ")";
            return result;
        }

    }

    public class WSQueryProperties
    {
        public string Query { get; set; }
        public string CountQuery { get; set; }
        public List<SqlParameter> SqlParameters { get; set; }
    }

    public class WSQueryReturns
    {
        public DataTable data { get; set; }
        public int? totalCount { get; set; }
    }
}
