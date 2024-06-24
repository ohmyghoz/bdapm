using BDA.DataModel;
using Ionic.Zip;
using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BDA.BusinessLayer
{
    public class QueueExportData
    {
        DataEntities db;
        public QueueExportData()
        {
            this.db = new DataEntities();
        }
        public void CreateCSV()
        {
            using (db = new DataEntities())
            {
                Console.WriteLine("Get Queue;");
                var objQueue = (from q in db.RptGrid_Queue
                                where q.stsrc == "A" && q.rgq_status == "Pending"
                                select q).FirstOrDefault();
                var timeStamp = DateTime.Now.ToString();
                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");

                if (objQueue != null)
                {
                    var nmMenu=objQueue.rgq_nama.Split(" ").First();
                    var csvPath = db.GetSetting("RGQPath") + objQueue.rgq_requestor + "_"+ nmMenu + "_" + timeStamp;
                    Console.WriteLine("RGQ:" + objQueue.rgq_id.ToString());
                    objQueue.rgq_start = DateTime.Now;
                    try
                    {
                        int? limitAntrian= Convert.ToInt32(db.GetSetting("LimitAntrian"));
                        bool isHive = IsPeriodInHive(db, objQueue.rgq_tablename);
                        var rowcount = DoQuery(db, objQueue.rgq_query, isHive, csvPath);
                        objQueue.rgq_result_rowcount = rowcount;

                        if (rowcount == limitAntrian) {
                            csvPath = csvPath + "_Truncate";
                        }
                        using (ZipFile zipFile = new ZipFile())
                        {
                            zipFile.UseZip64WhenSaving = Zip64Option.Always;

                            zipFile.AddFile(csvPath + ".csv", "");
                            //zipFile.AddEntry(objQueue.rgq_requestor + "_" + timeStamp + ".csv");

                            using (FileStream mstr = new FileStream(csvPath + ".zip", FileMode.CreateNew))
                            {
                                zipFile.Save(mstr);
                                File.Delete(csvPath + ".csv"); //hapus csv path
                            }
                            objQueue.rgq_status = "Selesai";
                            objQueue.rgq_end = DateTime.Now;
                            objQueue.rgq_result_filename = csvPath + ".zip";
                            objQueue.rgq_result_filesize = (int)(new System.IO.FileInfo(csvPath + ".zip")).Length;
                            //objQueue.rgq_result_rowcount = cnCount;
                        }

                    }
                    catch (Exception ex)
                    {
                        objQueue.rgq_error_message = ex.ToString();
                        objQueue.rgq_status = "Error";
                    }

                    db.SetStsrcFields(objQueue);
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("No Queue. Sleep 5 minutes;");
                    System.Threading.Thread.Sleep(300000); //sleep 5 menit
                    
                }

            }
        }
        public static bool IsPeriodInHive(DataEntities db, string tableName)
        {
            var dict = db.BDA2_Table.Find(tableName);
            if (dict == null) throw new InvalidOperationException("Cannot Find Table Dictionary : " + tableName);


            if (dict.StorageType == "SQL")
            {
                return false;
            }
            else if (dict.StorageType == "Hive")
            {
                return true;
            }
            else
            {
                return true;
            }

        }
        private static void PrepareProps(DataEntities db, WSQueryProperties props, string query1, bool forHive = false)
        {
            props.CountQuery = "SELECT count(*) from (" + Environment.NewLine + query1 + Environment.NewLine + ") cwhere";
            var limitAntrian=db.GetSetting("LimitAntrian");
            if (forHive == true)
            {
                props.Query = "SELECT * from (" + Environment.NewLine + query1 + Environment.NewLine + ") cwhere " + "LIMIT " + Convert.ToString(limitAntrian);
            }
            else {
                props.Query = "SELECT TOP "+ Convert.ToString(limitAntrian) + " * from (" + Environment.NewLine + query1 + Environment.NewLine + ") cwhere";
            }
            
        }

        private static int DataReaderToCsv(DbCommand cmd, string csvPath)
        {
            var i = 0;
            using (var write = new StreamWriter(csvPath))
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    var firstRow = true;
                    while (reader.Read())
                    {
                        if (firstRow)
                        {
                            firstRow = false;
                            // write schema
                            DataTable schemaTable = reader.GetSchemaTable();

                            var pertama = true;
                            foreach (DataRow row in schemaTable.Rows)
                            {

                                if (pertama) { pertama = false; } else { write.Write("|"); }
                                write.Write(String.Format("{0}",
                                   row["ColumnName"]));

                            }
                            write.WriteLine();

                        }
                        else
                        {
                            write.WriteLine(); //enter untuk baris sebelumnya
                        }

                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            var value = reader[j];
                            WriteCsv(write, value);
                            if (j < reader.FieldCount)
                            {
                                if (j == reader.FieldCount - 1)
                                {
                                    //write.WriteLine(); //enter untuk baris sebelumnya
                                }
                                else {
                                    write.Write("|");//tambah delimiter
                                }
                            }
                                 
                        }
                        i++;
                    }

                }
            }
            return i;
        }
        private static int ExecuteCommandSQLServer(SqlConnection conn, WSQueryProperties props, string csvPath)
        {
            var rowcount = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (var cmd = new SqlCommand(props.Query, conn))
            {
                cmd.CommandTimeout = 1800;

                cmd.CommandText = props.Query;

                var zipPath = csvPath + ".zip";
                csvPath = csvPath + ".csv";
                rowcount = DataReaderToCsv(cmd, csvPath);
            }
            conn.Close();
            return rowcount;
        }
        private static int ExecuteCommandHiveOdbc(string connString, WSQueryProperties props, string csvPath)
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
                var toReplace = "";
                props.CountQuery = props.CountQuery.Replace(mtc.Value, "'" + toReplace + "'");
                props.Query = props.Query.Replace(mtc.Value, "'" + toReplace + "'");
            }
            var rowcount = 0;
            using (var conn = new OdbcConnection(connString))
            {
               
                using (var cmd = new OdbcCommand(props.Query, conn))
                {
                    cmd.CommandTimeout = 1800;

                    cmd.CommandText = props.Query;

                    var zipPath = csvPath + ".zip";
                    csvPath = csvPath + ".csv";
                    rowcount = DataReaderToCsv(cmd, csvPath);
                }
                conn.Close();

            }

            return rowcount;
        }


        public static int DoQuery(DataEntities db, string query1, bool forHive = false, string csvPath = null)
        {
            var props = new WSQueryProperties();
            SqlConnection sc = (SqlConnection)db.Database.Connection;
            PrepareProps(db,props, query1, forHive);
            if (forHive)
            {
                return ExecuteCommandHiveOdbc("DSN=" + db.GetSetting("HiveDSN"), props, csvPath);
            }
            else
            {
                return ExecuteCommandSQLServer(sc, props, csvPath);
            }
        }
        private static void WriteCsv(StreamWriter sb, object obj)
        {
            var value = "";
            if (obj != null)
            {
                value = obj.ToString();
            }
            value = value.Replace("\n", "<newline>").Replace("\r", ""); //escape enter

            value = value.Replace("\"", "`"); //escape "
            value = value.Replace(" 00:00:00", "");
            //if (value.Contains(",")) //ada koma, kasih "
            //{
            //    value = $"\"{value}\"";
            //}
            sb.Write(value);
        }
    }
    public class WSQueryReturns
    {
        public DataTable data { get; set; }
        public int? totalCount { get; set; }
    }
    public class WSQueryProperties
    {
        public string Query { get; set; }
        public string CountQuery { get; set; }
    }
}
