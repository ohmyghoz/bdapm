using RestSharp;
using SyncWebService.DataModel;
using System;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading;
namespace SyncWebService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var mode = "Master";
            if (args.Length >= 1)
            {
                mode = args[0];
            }

           
            CookieContainer _cookieJar = new CookieContainer();
            var client = new RestClient(Settings1.Default.APIUrl);
            client.CookieContainer = _cookieJar;


            // selalu login dulu
            {
                var request = new RestRequest("login", Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                //request.AddHeader("Cookie", "JSESSIONID=881c2fb8-1875-4567-a5d2-0a4382b48dee");

                //var hasil = EncryptString("", "kuncirumahpakais");

                Console.WriteLine("Login");
                request.AddParameter("userName", Settings1.Default.APIUserId);
                request.AddParameter("password", DecryptString(Settings1.Default.APIEncPassword, "kuncirumahpakais"));
                var response = client.Post(request);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);

            }
            {
                //restart interpreter
                Console.WriteLine("Restart Interpreter");
                var request = new RestRequest("interpreter/setting/restart/spark2", Method.PUT);
                var response = client.Put(request);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
            }

            //test running notebook non-sync, no issue
            /*
            {
                Console.WriteLine($"Running Notebook");
                var request = new RestRequest("notebook/job/2KYJNFF4Y", Method.POST);
                var response = client.Post(request);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);

                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
            */

            
            if (mode == "Master")
            {
                int lastCnt = 0;

                using (var db = new DataEntities())
                {
                    ///============= test
                    Console.WriteLine($"Test Last Periode");
                    var lastPeriode = "19900101";
                    var lastQuery = from q in db.HiveSync
                                    where q.pprocess == "Master" && q.sync_status == "OK"
                                    orderby q.pperiode descending
                                    select q.pperiode;

                    if (lastQuery.Any())
                    {
                        lastPeriode = lastQuery.First();
                    }

                    Console.WriteLine($"Last Periode : {lastPeriode}");
                    Thread.Sleep(new TimeSpan(0, 0, 10));

                    //==================



                    /*
                    DateTime now = System.DateTime.Now.AddDays(-1);
                    var period = now.Year.ToString() + now.Month.ToString();

                    var lastQuery = from q in db.Log_ETL
                                    where q.log_tipe == "INSERT pm_master_sid" && q.log_periode == period
                                    orderby q.log_id descending
                                    select q.log_insert_cnt;

                    if (lastQuery.Any())
                    {
                        lastCnt = Int32.Parse(lastQuery.First().ToString());
                    }


                    Console.WriteLine($"SID Periode {period} Last Count : {lastCnt}");

                    
                    //if for testing may temporary be disabled
                    
                    var conn = new OdbcConnection
                    {
                        ConnectionString = @"DRIVER={Hortonworks Hive ODBC Driver};                                        
                                        Host=10.225.60.14;
                                        Port=10000;
                                        Schema=pasarmodal;
                                        HiveServerType=2;
                                        KrbHostFQDN={bgrdco-bddvmst1.ojk.go.id};
                                        KrbServiceName={hive};
                                        AuthMech=1;"
                    };
                    try
                    {

                        conn.Open();
                        
                        var q = "select count (*) as new_cnt from pasarmodal.src_sid_new where valid_from >= '" + period + "01'";
                        OdbcCommand myCommand = new OdbcCommand(q, conn);
                        myCommand.CommandTimeout = 300; // 5 menit

                        DataTable table = new DataTable();
                        table.Load(myCommand.ExecuteReader());
                        var ds = new DataSet();
                        ds.Tables.Add(table);
                        var dt = ds.Tables[0];


                        int hiveNewCnt = 0;
                        if (dt.Rows.Count > 0)
                        {
                            hiveNewCnt = Int32.Parse(dt.Rows[0][0].ToString());
                        }

                        Console.WriteLine($"Hive SID Periode {period} New Count: {hiveNewCnt}");



                        if (hiveNewCnt > lastCnt)
                        {
                            Console.WriteLine("Jumlah data di Hive lebih besar dari jumlah data di SQL");
                            //run sync master
                            Console.WriteLine("Sync Master");
                            var request = new RestRequest("notebook/job/2KYYBKUKN", Method.POST);
                            var response = client.Post(request);
                            Console.WriteLine(response.StatusCode);
                            Console.WriteLine(response.Content);


                            //insert hivesync
                            var newsync = new HiveSync();
                            newsync.pperiode = period;
                            newsync.pprocess = "Master";
                            if(response.StatusCode == HttpStatusCode.OK)
                            {
                                newsync.sync_status = "OK";
                            }
                            else
                            {
                                newsync.sync_status = response.StatusCode.ToString();
                            }
                            
                            db.HiveSync.Add(newsync);
                            db.SaveChanges();
                            Console.WriteLine("Sync Master");
                        }
                        else
                        {
                            Console.WriteLine("Periode Hive <= dari periode SQL");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return;
                    }
                    finally
                    {
                        conn.Close();
                    }
                    //end temporary disabled
*/

                }
               
            }
            
        }

        public static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }



}
