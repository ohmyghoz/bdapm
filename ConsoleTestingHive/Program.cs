using BDA.DataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using System.Data.Odbc;

namespace ConsoleQueue
{
    class Program
    {
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
        static void Main(string[] args)
        {
            try
            {

                ExecuteCommandHiveOdbc();
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                System.Threading.Thread.Sleep(20000);
            }
           
        }
        private static void ExecuteCommandHiveOdbc()
        {
            using (var db = new DataEntities()) {
                var result = new WSQueryReturns();
                var props = new WSQueryProperties();
                //hive bedanya disini. schema = ojkdt, dm_periode jadi dm_pperiode (partisi)
                props.CountQuery = "SELECT count(*) from (select* from ojkdt.osida_kolektibilitas_nilai_tunggakan x WHERE x.dm_pperiode = '202101') cord ORDER BY 1 LIMIT 0,15";
                props.Query = "SELECT * from (select* from ojkdt.osida_kolektibilitas_nilai_tunggakan x WHERE x.dm_pperiode = '202101') cord ORDER BY 1 LIMIT 0,15";
                //props.CountQuery = props.CountQuery.Replace("dbo.", "ojkdt.").Replace("dm_periode", "dm_pperiode");
                //props.Query = props.Query.Replace("dbo.", "ojkdt.").Replace("dm_periode", "dm_pperiode");
             

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
                using (var conn = new OdbcConnection(db.GetSetting("HiveDSN")))
                {

                    using (var cmd = new OdbcCommand(props.Query, db.GetSetting("HiveDSN")))
                    {
                        cmd.CommandTimeout = 1800;

                        cmd.CommandText = props.Query;

                        
                    }
                    conn.Close();

                }
            }
            

        }
    }
}
