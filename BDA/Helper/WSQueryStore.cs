using BDA.DataModel;
using BDA.Helper.FW;
using DevExpress.Data.Extensions;
using DevExpress.XtraRichEdit;
using DevExtreme.AspNet.Mvc;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace BDA.Helper
{
    public class WSQueryStore
    {
        #region Encrypt Decrypt PM
        private static string[] keySID = ["MFF", "FDD", "FDF", "CPD", "SCD", "IDD", "PFD", "OTF", "OTD", "IBD", "SCF", "IDF", "CPF", "MFD", "IBF", "PFF", "ISD", "ISF"];
        private static char[] keyName = ['M', 'R', 'S', 'D', 'N', 'F', 'H', 'Y', 'T', 'J'];
        private static char[] keyOne = ['X', 'A', 'Y', 'B', 'Z', 'C', 'L', 'R', 'H', 'S'];
        private static char[] keyTwo = ['D', 'N', 'J', 'P', 'E', 'I', 'O', 'K', 'Q', 'M'];

        public static WSQueryReturns DecryptResults(WSQueryReturns wqr)
        {
            DataTable dt = wqr.data;
            dt.Columns.Add("no", typeof(System.String));
            int noRow = 1;
            foreach (DataRow dr in dt.Rows)
            {
                dr["no"] = noRow.ToString();                
                dr["sid"] = DecryptSID(dr["sid"].ToString());
                dr["lem"] = dr["lem"].ToString() + dr["sid"].ToString();
                dr["nama_sid"] = DecryptName(dr["nama_sid"].ToString());
                dr["full_name"] = DecryptName(dr["full_name"].ToString());
                dr["email"] = DecryptName(dr["email"].ToString());
                dr["nama_rekening"] = DecryptName(dr["nama_rekening"].ToString());
                dr["phone_number"] = DecryptNumber(dr["phone_number"].ToString());
                noRow++;
            }
            wqr.data = dt;

            return wqr;
        }

        public static WSQueryReturns DecryptResultsNamaSID(WSQueryReturns wqr)
        {
            DataTable dt = wqr.data;

            foreach (DataRow dr in dt.Rows)
            {
                dr["sid"] = DecryptSID(dr["sid"].ToString());
                dr["nama_sid"] = DecryptName(dr["nama_sid"].ToString());
            }

            wqr.data = dt;

            return wqr;
        }

        private static string DecryptSID(string s)
        {
            if (s.Length < 3) return s;
            int val = 0;
            return (Int32.TryParse(s.Substring(0, 2), out val) ? keySID[(val - 11)] : s.Substring(0, 2)) + s.Substring(2);
        }

        private static string DecryptName(string s)
        {
            if (s.Length == 0) return s;
            int val = 0;
            return ((Int32.TryParse(s.Substring(0, 1), out val) ? keyName[val] : s.Substring(0, 1))
                + s.Substring(1)).Replace("IOII", "O").Replace("IOOI", "A").Replace("IIOO", "U").Replace("IOIO", "E");
        }

        private static string DecryptNumber(string s)
        {
            if (s.Length < 2) return s;
            int val = 0;
            string right = s.Substring(s.Length - 2, 2);
            return (Int32.TryParse(right.Substring(0, 1), out val) ? keyOne[val] : right[0]) + (Int32.TryParse(right.Substring(1, 1), out val) ? keyTwo[val] : right[1])
                + s.Substring(0, s.Length - 2);
        }

        private static string EncryptSID(string s)
        {
            if (s.Length < 3) return s;
            int idx = Array.IndexOf(keySID, s.Substring(0, 3));
            if (idx >= 0) return (idx + 11).ToString() + s.Substring(3);
            return s;
        }

        private static string EncryptName(string s)
        {
            if (s.Length < 2) return s;
            int idx = Array.IndexOf(keyName, s.Substring(0, 1));
            return ((idx >= 0 ? idx.ToString() : s.Substring(0, 1)) + s.Substring(1)).Replace("O", "IOII").Replace("A", "IOOI").Replace("U", "IIOO").Replace("E", "IOIO");
        }
        #endregion

        public static string CheckAndModifyFilterVal(string colName, string colValue)
        {
            if (colName == "sid")
                return WSQueryStore.EncryptSID(colValue);

            return colValue;
        }

        #region SimpleQuery
        private static WSQueryReturns ExecuteSimpleSQL(string connString, string queryString)
        {
            var result = new WSQueryReturns();
            using (var conn = new SqlConnection(connString))
            {
                string countQuery = @"select count(*) from (" + queryString + @") cntquery";
                using (var cmd = new SqlCommand(countQuery, conn))
                {
                    cmd.CommandTimeout = 300;
                    conn.Open();
                    var dt = new DataTable();
                    if (result.totalCount == null || result.totalCount > 0) //hemat 1 kali call 
                    {
                        cmd.CommandText = queryString;
                        var adap = new SqlDataAdapter(cmd);
                        adap.Fill(dt);
                    }
                    result.data = dt;
                }
                return result;
            }
        }
        #endregion

        public static bool IsPeriodInHive(DataEntities db, string tableName)
        {
            var dict = db.BDA2_Table.Find(tableName);
            if (dict == null) throw new InvalidOperationException("Cannot Find Table Dictionary : " + tableName);


            if (dict.StorageType == "SQL")
            {
                //DateTime? maxPeriodAvailable = db.BDA2_Table_Period.Where(x => x.TableName == tableName).Max(x => (DateTime?)x.Period);
                //DateTime? minPeriodAvailable = db.BDA2_Table_Period.Where(x => x.TableName == tableName).Min(x => (DateTime?)x.Period);
                //if (maxPeriodAvailable == null) return true;
                //if (minPeriodAvailable == null) return true;
                //if (period.Min() >= minPeriodAvailable && period.Max() <= maxPeriodAvailable)
                //{
                //    return false;
                //}
                //else
                //{
                //    return true;
                //}
                return false;
            }
            else if (dict.StorageType == "Hive")
            {
                return true;
            }
            //else if (dict.StorageType == "Hybrid")
            //{
            //    DateTime? maxPeriodAvailable = db.BDA2_Table_Period.Where(x => x.TableName == tableName).Max(x => (DateTime?)x.Period);
            //    DateTime? minPeriodAvailable = db.BDA2_Table_Period.Where(x => x.TableName == tableName).Min(x => (DateTime?)x.Period);
            //    if (maxPeriodAvailable == null) return true; //ga nemu, artinya di hive semua
            //    if (minPeriodAvailable == null) return true;
            //    if (period.Min() >= minPeriodAvailable && period.Max() <= maxPeriodAvailable)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            else
            {
                return true;
            }
            //if (dict.StorageType == "Hive") return true;
            //if (dict.StorageType == "Hybrid")
            //{
            //    DateTime? maxPeriodAvailable = db.BDA2_Table_Period.Where(x => x.TableName == tableName).Max(x => (DateTime?)x.Period);
            //    if (maxPeriodAvailable == null) return true; //ga nemu, artinya di hive semua

            //    var earliestPeriodAvailable = maxPeriodAvailable.Value.AddMonths(1 - dict.SqlServerPeriodCount.Value);
            //    if ( earliestPeriodAvailable <= period && maxPeriodAvailable.Value >= period)
            //    {
            //        return false; //periode ini ada di SQL server
            //    }
            //    else
            //    {
            //        return true; // ud ngga ada di SQL Server
            //    }
            //}
            //return true;

        }

        public static WSQueryReturns GetOsidaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string tableName,
            string memberTypes, string members, DateTime periode, string tipeChart, bool isChart = true)
        {
            bool isC = false;
            var whereQuery = "x.dm_periode = @periode";
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)
            List<DateTime> lp = new List<DateTime>();
            lp.Add(periode);
            var isHive = IsPeriodInHive(db, tableName);
            //isHive = true;
            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }

            }
            var props = new WSQueryProperties();
            if (!isChart)
            {
                if (tableName == "osida_plafondering_umum_master")
                {
                    if (isHive == true)
                    {
                        props.Query = @"
                        select  concat_ws('~', cast(dm_periode as string), regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk) AS lem,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }
                    else
                    {
                        props.Query = @"
                        select  CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk AS lem,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }

                }
                //else if (tableName == "osida_kolektibilitas_hari_tunggakan")
                //{
                //    props.Query = @"
                //        SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, dm_no_rekening, dm_cif, dm_nama_debitur, dm_jenis_kredit,
                //        dm_jenis_penggunaan, dm_sifat_kredit, dm_tanggal_awal_pinjaman, dm_tanggal_mulai, dm_tanggal_jatuh_tempo,
                //        dm_baru_perpanjangan, dm_valuta, dm_plafon_awal, dm_plafon, dm_baki_debet, dm_jumlah_hari_tunggakan, 
                //        dm_kolektibilitas_dpd, dm_kolektibilitas, dm_tunggakan_pokok, dm_tunggakan_bunga, dm_denda, dm_kode_kantor_cabang,
                //        dm_nama_kantor_cabang, dm_no_akad_akhir, dm_status_tidak_dicover_agunan, dm_banyak_agunan_dalam_satu_rek, 
                //        dm_banyak_agunan_tunai_dalam_satu_rek, dm_banyak_agunan_paripasu_dalam_satu_rek, dm_nonmissing_persenparipasu_dalam_satu_rek,
                //        ROUND(dm_subtotal_nilai_agunan_dalam_satu_rek, 0) AS dm_subtotal_nilai_agunan_dalam_satu_rek, dm_subtotal_agunan_tunai_dalam_satu_rek,
                //        dm_nilai_agunan_dalam_satu_rek FROM dbo." + tableName + @" x
                //        WHERE " + whereQuery + @"	                
                //        ";
                //}
                else
                {
                    if (tableName == "osida_kolektibilitas_hari_tunggakan")
                    {
                        props.Query = @"
                        select  *,ROUND(dm_subtotal_nilai_agunan_dalam_satu_rek, 0) AS dm_subtotal_nilai_agunan_dalam_satu_rek1 from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }
                    else if (tableName == "osida_hapusbuku_satu_tahun")
                    {
                        props.Query = @"
                        select  *,ROUND(dm_jumlah_bulan_dari_akad_akhir, 0) AS dm_jumlah_bulan_dari_akad_akhir1 from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }
                    else
                    {
                        props.Query = @"
                        select * from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }

                }

            }
            else
            {
                isC = true;
                if (tableName == "osida_hapusbuku_satu_tahun" || tableName == "osida_kredit_takeover_tidak_bayar" || tableName == "osida_restrukturisasi_satu_tahun" || tableName == "osida_tidak_bayar_dari_fasilitas_dibentuk")
                {
                    if (tableName == "osida_hapusbuku_satu_tahun")
                    {
                        if (tipeChart == tableName + "_total_plafon")
                        {
                            props.Query = @"
                            SELECT  SUM(x.aggData) AS n, SUM(CASE WHEN COALESCE(p.dm_plafon_231_hari_terakhir,0)=0 THEN 0 ELSE p.dm_plafon_231_hari_terakhir - x.aggData END) AS y
                            FROM (
                            SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, SUM(COALESCE(dm_plafon_awal,0)) AS aggData
                            FROM dbo." + tableName + @"
                            GROUP BY dm_periode, dm_jenis_ljk, dm_kode_ljk
                            ) x
                            LEFT JOIN dbo.osida_pembanding_populasi_chart p ON p.dm_jenis_ljk = x.dm_jenis_ljk AND p.dm_kode_ljk = x.dm_kode_ljk AND p.dm_periode = x.dm_periode
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                        else if (tipeChart == tableName + "_jumlah_rekening")
                        {
                            props.Query = @"
                            SELECT  SUM(x.aggData) AS n, SUM(CASE WHEN COALESCE(p.dm_jumlah_rekening_231_hari_terakhir,0)=0 THEN 0 ELSE p.dm_jumlah_rekening_231_hari_terakhir - x.aggData END) AS y
                            FROM (
                            SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, COUNT(*) AS aggData
                            FROM dbo." + tableName + @"
                            GROUP BY dm_periode, dm_jenis_ljk, dm_kode_ljk
                            ) x
                            LEFT JOIN dbo.osida_pembanding_populasi_chart p ON p.dm_jenis_ljk = x.dm_jenis_ljk AND p.dm_kode_ljk = x.dm_kode_ljk AND p.dm_periode = x.dm_periode
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                    }
                    else if (tableName == "osida_kredit_takeover_tidak_bayar")
                    {
                        if (tipeChart == tableName + "_total_plafon")
                        {
                            props.Query = @"
                            SELECT  SUM(x.aggData) AS n, SUM(CASE WHEN COALESCE(p.dm_plafon_takeover_231_hari_terakhir,0)=0 THEN 0 ELSE p.dm_plafon_takeover_231_hari_terakhir - x.aggData END) AS y
                            FROM (
                            SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, SUM(COALESCE(dm_plafon_awal,0)) AS aggData
                            FROM dbo." + tableName + @"
                            GROUP BY dm_periode, dm_jenis_ljk, dm_kode_ljk
                            ) x
                            LEFT JOIN dbo.osida_pembanding_populasi_chart p ON p.dm_jenis_ljk = x.dm_jenis_ljk AND p.dm_kode_ljk = x.dm_kode_ljk AND p.dm_periode = x.dm_periode
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                        else if (tipeChart == tableName + "_jumlah_rekening")
                        {
                            props.Query = @"
                            SELECT  SUM(x.aggData) AS n, SUM(CASE WHEN COALESCE(p.dm_jumlah_rekening_takeover_231_hari_terakhir,0)=0 THEN 0 ELSE p.dm_jumlah_rekening_takeover_231_hari_terakhir - x.aggData END) AS y
                            FROM (
                            SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, COUNT(*) AS aggData
                            FROM dbo." + tableName + @"
                            GROUP BY dm_periode, dm_jenis_ljk, dm_kode_ljk
                            ) x
                            LEFT JOIN dbo.osida_pembanding_populasi_chart p ON p.dm_jenis_ljk = x.dm_jenis_ljk AND p.dm_kode_ljk = x.dm_kode_ljk AND p.dm_periode = x.dm_periode
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                    }
                    else if (tableName == "osida_restrukturisasi_satu_tahun" || tableName == "osida_tidak_bayar_dari_fasilitas_dibentuk")
                    {
                        if (tipeChart == tableName + "_total_plafon")
                        {
                            props.Query = @"
                            SELECT  SUM(x.aggData) AS n, SUM(CASE WHEN COALESCE(p.dm_plafon_1_tahun_terakhir,0)=0 THEN 0 ELSE p.dm_plafon_1_tahun_terakhir - x.aggData END) AS y
                            FROM (
                            SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, SUM(COALESCE(dm_plafon_awal,0)) AS aggData
                            FROM dbo." + tableName + @"
                            GROUP BY dm_periode, dm_jenis_ljk, dm_kode_ljk
                            ) x
                            LEFT JOIN dbo.osida_pembanding_populasi_chart p ON p.dm_jenis_ljk = x.dm_jenis_ljk AND p.dm_kode_ljk = x.dm_kode_ljk AND p.dm_periode = x.dm_periode
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                        else if (tipeChart == tableName + "_jumlah_rekening")
                        {
                            props.Query = @"
                            SELECT  SUM(x.aggData) AS n, SUM(CASE WHEN COALESCE(p.dm_jumlah_rekening_1_tahun_terakhir,0)=0 THEN 0 ELSE p.dm_jumlah_rekening_1_tahun_terakhir - x.aggData END) AS y
                            FROM (
                            SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, COUNT(*) AS aggData
                            FROM dbo." + tableName + @"
                            GROUP BY dm_periode, dm_jenis_ljk, dm_kode_ljk
                            ) x
                            LEFT JOIN dbo.osida_pembanding_populasi_chart p ON p.dm_jenis_ljk = x.dm_jenis_ljk AND p.dm_kode_ljk = x.dm_kode_ljk AND p.dm_periode = x.dm_periode
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                    }


                    if (tipeChart == tableName + "_jenis_kredit")
                    {
                        props.Query = @"
                        SELECT dm_jenis_kredit AS dm_base,COUNT(*) AS dm_count FROM dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	 
                        GROUP BY x.dm_jenis_kredit 	                
                        ";
                    }
                    else if (tipeChart == tableName + "_jenis_penggunaan")
                    {
                        props.Query = @"
                        SELECT dm_jenis_penggunaan AS dm_base,COUNT(*) AS dm_count FROM dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	 
                        GROUP BY x.dm_jenis_penggunaan 	                
                        ";
                    }

                }
                else
                {
                    props.Query = @"
                    SELECT 'n' AS dm_base,0.000000 AS dm_count
                    UNION
                    SELECT 'y' AS dm_base,0.000000 AS dm_count               
                    ";
                }

            }



            props.SqlParameters = new List<SqlParameter>();
            props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            //return WSQueryHelper.DoQuery(db, props, loadOptions, IsPeriodInHive(db, tableName, lp));
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMA_AMLQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string jenisAgunans, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisAgunans != null)
            {
                jenisAgunans = "'" + jenisAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_agunan in (" + jenisAgunans + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                select * from dbo.ma_aml_cft_analysis x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT dm_jenis_agunan, SUM(x.dm_agunan) AS dm_agunan, 
                    SUM(x.dm_plafon) AS dm_plafon, SUM(x.dm_outstanding) AS dm_outstanding
                    FROM dbo.ma_aml_cft_analysis  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_jenis_agunan
                    ORDER BY dm_outstanding DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 dm_jenis_agunan, SUM(x.dm_agunan) AS dm_agunan, 
                    SUM(x.dm_plafon) AS dm_plafon, SUM(x.dm_outstanding) AS dm_outstanding
                    FROM dbo.ma_aml_cft_analysis  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_jenis_agunan
                    ORDER BY dm_outstanding DESC
                    ";
                }

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }


        public static WSQueryReturns GetMA_ONAQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string jenisAgunans, string lokasiAgunans, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisAgunans != null)
            {
                jenisAgunans = "'" + jenisAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_agunan in (" + jenisAgunans + ")";
            }

            if (lokasiAgunans != null)
            {
                lokasiAgunans = "'" + lokasiAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_lokasi_agunan in (" + lokasiAgunans + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                select * from dbo.ma_outstanding_macet_no_agunan x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                props.Query = @"
                SELECT dm_jenis_penggunaan, Count(*) as cnt
                FROM dbo.ma_outstanding_macet_no_agunan  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_jenis_penggunaan                                               
            ";
                isC = true;
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMA_AnomaliPekerjaanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisPenggunaans, string pekerjaans, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisPenggunaans != null)
            {
                jenisPenggunaans = "'" + jenisPenggunaans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_penggunaan in (" + jenisPenggunaans + ")";
            }
            if (pekerjaans != null)
            {
                pekerjaans = "'" + pekerjaans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_profesi_debitur in (" + pekerjaans + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"

                select * from dbo.ma_anomali_plafon_by_pekerjaan_debitur x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT dm_profesi_debitur,
                    SUM(x.dm_plafon) AS dm_plafon
                    FROM dbo.ma_anomali_plafon_by_pekerjaan_debitur  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_profesi_debitur
                    ORDER BY dm_plafon DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 dm_profesi_debitur,
                    SUM(x.dm_plafon) AS dm_plafon
                    FROM dbo.ma_anomali_plafon_by_pekerjaan_debitur  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_profesi_debitur
                    ORDER BY dm_plafon DESC
                    ";
                }

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMA_RekBaruNPLQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisPinjamans, string npls, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisPinjamans != null)
            {
                jenisPinjamans = "'" + jenisPinjamans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_jenis_pinjaman in (" + jenisPinjamans + ")";
            }
            if (npls != null)
            {
                npls = "'" + npls.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_npl in (" + npls + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"

                select * from dbo.ma_rekening_baru_non_restrukturisasi_npl x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                props.Query = @"
                SELECT dm_npl, COUNT(*) AS cnt
                FROM dbo.ma_rekening_baru_non_restrukturisasi_npl  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_npl                                              
            ";
                isC = true;
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMA_OutstandingMacetQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisAgunans, string lokasiAgunans, string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisAgunans != null)
            {
                jenisAgunans = "'" + jenisAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_agunan in (" + jenisAgunans + ")";
            }
            if (lokasiAgunans != null)
            {
                lokasiAgunans = "'" + lokasiAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_lokasi_agunan in (" + lokasiAgunans + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                props.Query = @"

                SELECT dm_jenis_penggunaan, COUNT(*) AS dm_count
                FROM dbo.ma_outstanding_macet_no_agunan  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_jenis_penggunaan";

            }
            else if (isPieChart)
            {
                isC = true;
                props.Query = @"

                SELECT dm_base, COUNT(*) AS dm_count FROM (
                SELECT CASE WHEN dm_baki_debet_status = 'Normal' THEN 'Wajar' ELSE 'Tidak Wajar' END AS dm_base,* FROM dbo.ma_outstanding_macet_no_agunan
                ) x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_base";

            }
            else
            {
                props.Query = @"

                select * from dbo.ma_outstanding_macet_no_agunan x
                WHERE " + whereQuery + @"	                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMS_APSukuBungaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string stbs, string periode, string tipeChart, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (stbs != null)
            {
                stbs = "'" + stbs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_status_threshold_suku_bunga in (" + stbs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                SELECT * FROM dbo.ms_perubahan_suku_bunga_no_restrukturisasi x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT x.dm_status_threshold_suku_bunga,COUNT(*) AS dm_count
                FROM dbo.ms_perubahan_suku_bunga_no_restrukturisasi  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_status_threshold_suku_bunga                                               
                ";
                //if (tipeChart == "bar")
                //{
                //    if (isHive == true)
                //    {
                //        props.Query = @"
                //        SELECT x.dm_periode,SUM(x.kurangdari) AS kurangdari,SUM(x.lebihdari) AS lebihdari
                //        FROM (SELECT CASE WHEN dm_status_threshold_suku_bunga='Perubahan >=3.0%' THEN 1 ELSE 0 END AS lebihdari,CASE WHEN dm_status_threshold_suku_bunga='Perubahan <3.0%' THEN 1 ELSE 0 END AS kurangdari,* FROM dbo.ms_perubahan_suku_bunga_no_restrukturisasi)  x
                //        WHERE " + whereQuery + @"	 
                //        GROUP BY x.dm_periode                                              
                //        ";
                //    }
                //    else {
                //        props.Query = @"
                //        SELECT FORMAT (x.dm_periode, 'yyyyMM') AS dm_periode,SUM(x.kurangdari) AS kurangdari,SUM(x.lebihdari) AS lebihdari
                //        FROM (SELECT CASE WHEN dm_status_threshold_suku_bunga='Perubahan >=3.0%' THEN 1 ELSE 0 END AS lebihdari,CASE WHEN dm_status_threshold_suku_bunga='Perubahan <3.0%' THEN 1 ELSE 0 END AS kurangdari,* FROM dbo.ms_perubahan_suku_bunga_no_restrukturisasi)  x
                //        WHERE " + whereQuery + @"	 
                //        GROUP BY x.dm_periode                                              
                //        ";
                //    }

                //}
                //else
                //{
                //    props.Query = @"
                //    SELECT x.dm_status_threshold_suku_bunga,COUNT(*) AS dm_count
                //    FROM dbo.ms_perubahan_suku_bunga_no_restrukturisasi  x
                //    WHERE " + whereQuery + @"	 
                //    GROUP BY x.dm_status_threshold_suku_bunga                                               
                //    ";
                //}

            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMS_KetentuanKolektibilitas(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string ksks, string periode, string tipeChart, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (ksks != null)
            {
                ksks = "'" + ksks.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kategori_status_kolektibilitas in (" + ksks + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT  ROW_NUMBER() OVER (PARTITION BY 1 ORDER BY 1) as rowid,* FROM dbo.ms_ketentuan_kolektibilitas x
                    WHERE " + whereQuery + @"	                
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT  * FROM dbo.ms_ketentuan_kolektibilitas x
                    WHERE " + whereQuery + @"	                
                    ";
                }


            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT x.dm_kategori_status_kolektibilitas,COUNT(*) AS dm_count FROM dbo.ms_ketentuan_kolektibilitas x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kategori_status_kolektibilitas                                               
                ";

            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMS_PinjamanBaruBersamaan(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string kolektibilitass, string periode, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                //members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //whereQuery = whereQuery += " AND dm_kode_ljk_filter in (" + members + ")";
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk_filter LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk_filter in (" + members + ")";
                }
                //int icount = 1;
                //whereQuery = whereQuery += " AND (";
                //foreach (var i in members.Split(","))
                //{
                //    if (icount == 1)
                //    {
                //        whereQuery = whereQuery += " dm_list_ljk LIKE '%" + i + "%'";
                //    }
                //    else
                //    {
                //        whereQuery = whereQuery += " OR dm_list_ljk LIKE '%" + i + "%'";
                //    }
                //    icount = icount + 1;
                //}
                //whereQuery = whereQuery += " ) ";
            }

            if (kolektibilitass != null)
            {
                //kolektibilitass = "'" + kolektibilitass.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                int icount = 1;
                whereQuery = whereQuery += " AND (";
                foreach (var i in kolektibilitass.Split(","))
                {
                    if (icount == 1)
                    {
                        whereQuery = whereQuery += " dm_kolektibilitas LIKE '%" + i + "%'";
                    }
                    else
                    {
                        whereQuery = whereQuery += " OR dm_kolektibilitas LIKE '%" + i + "%'";
                    }
                    icount = icount + 1;
                }
                whereQuery = whereQuery += " ) ";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isHive == true)
            {
                props.Query = @"
                SELECT concat_ws('~',cast(dm_periode as string),dm_jenis_ljk,dm_kode_ljk_filter,dm_kolektibilitas) AS lem,* FROM dbo.ms_pinjaman_baru_bersamaan_sum x
                WHERE " + whereQuery + @"	                
                ";
            }
            else
            {
                props.Query = @"
                SELECT  FORMAT(x.dm_periode, 'yyyyMM') + '~' + REPLACE(dm_jenis_ljk,'/','>') + '~' + dm_kode_ljk_filter + '~' + dm_kolektibilitas AS lem,* FROM dbo.ms_pinjaman_baru_bersamaan_sum x
                WHERE " + whereQuery + @"	                
                ";
            }


            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMS_PinjamanBaruBersamaanDetail(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string kolektibilitass, string periode, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                //members = "'" + members.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //whereQuery = whereQuery += " AND member_code in (" + members + ")";
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk_filter LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk_filter in (" + members + ")";
                }
            }

            if (kolektibilitass != null)
            {
                kolektibilitass = "'" + kolektibilitass.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kolektibilitas in (" + kolektibilitass + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            props.Query = @"
            SELECT * FROM dbo.ms_pinjaman_baru_bersamaan_det x
            WHERE " + whereQuery + @"	                
            ";

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMS_KolektibilitasKaryawanLJK(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string kolektibilitas, string periode, string tipeChart, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kolektibilitas != null)
            {
                kolektibilitas = "'" + kolektibilitas.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kolektibilitas_sekarang in (" + kolektibilitas + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                SELECT * FROM dbo.ms_kolektibilitas_karyawan_ljk x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_pekerjaan AS desc_pekerjaan_byreff,COUNT(*) AS dm_count FROM dbo.ms_kolektibilitas_karyawan_ljk x                    
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_pekerjaan      
                    ORDER BY dm_count DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_pekerjaan AS desc_pekerjaan_byreff,COUNT(*) AS dm_count FROM dbo.ms_kolektibilitas_karyawan_ljk x                    
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_pekerjaan      
                    ORDER BY dm_count DESC
                    ";
                }


            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMS_PemberianPinjamanKaryawanLJK(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
           string members, string jps, string periode, string tipeChart, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jps != null)
            {
                jps = "'" + jps.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_jenis_pinjaman in (" + jps + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                SELECT * FROM dbo.ms_pemberian_pinjaman_karyawan_ljk x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT x.dm_kode_jenis_pinjaman,COUNT(*) AS dm_count FROM dbo.ms_pemberian_pinjaman_karyawan_ljk x   
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kode_jenis_pinjaman                                              
                ";

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMacro_ForecastingQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string tipeForecastings, DateTime periodeAwal, DateTime periodeAkhir, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "dm_pperiode >= @periodeAwal AND dm_pperiode <= @periodeAkhir";
            //var whereQuery2 = "dm_pperiode <= @periodeAkhir";
            List<DateTime> lp = new List<DateTime>();
            lp.Add(periodeAwal);
            lp.Add(periodeAkhir);
            var isHive = IsPeriodInHive(db, "macro_output_forecast_level_ljk");
            //isHive = true;
            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                //members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //whereQuery = whereQuery + " AND dm_kode_ljk in (" + members + ")";
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (tipeForecastings != null)
            {
                tipeForecastings = "'" + tipeForecastings.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_tipe_forecasting in (" + tipeForecastings + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"

                select * from dbo.macro_output_forecast_level_ljk x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_periode dm_tipe_forecasting,
                    SUM(x.dm_nilai) AS dm_nilai
                    FROM dbo.macro_output_forecast_level_ljk  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_tipe_forecasting, x.dm_periode
                    ORDER BY x.dm_periode
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT FORMAT(x.dm_periode, 'yyyyMM') AS dm_periode, dm_tipe_forecasting,
                    SUM(x.dm_nilai) AS dm_nilai
                    FROM dbo.macro_output_forecast_level_ljk  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_tipe_forecasting, x.dm_periode
                    ORDER BY x.dm_periode
                    ";
                }

            }

            props.SqlParameters = new List<SqlParameter>();
            props.SqlParameters.Add(new SqlParameter("periodeAwal", System.Data.SqlDbType.Date) { Value = periodeAwal });
            props.SqlParameters.Add(new SqlParameter("periodeAkhir", System.Data.SqlDbType.Date) { Value = periodeAkhir });

            //return WSQueryHelper.DoQuery(db, props, loadOptions, IsPeriodInHive(db, "macro_output_forecast_level_ljk", lp));
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMacro_AMPertumbuhanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
           string jenisPertumbuhans, DateTime periodeAwal, DateTime periodeAkhir, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "dm_periode >= @periodeAwal AND dm_periode <= @periodeAkhir";
            //var whereQuery2 = "dm_periode <= @periodeAkhir";
            List<DateTime> lp = new List<DateTime>();
            lp.Add(periodeAwal);
            lp.Add(periodeAkhir);
            var isHive = IsPeriodInHive(db, "macro_pertumbuhan_pinjaman_level_ljk");
            //isHive = true;
            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                //members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //whereQuery = whereQuery += " AND " + whereQuery2 + " AND dm_kode_ljk in (" + members + ")";
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisPertumbuhans != null)
            {
                jenisPertumbuhans = "'" + jenisPertumbuhans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_pertumbuhan in (" + jenisPertumbuhans + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                    select * from dbo.macro_pertumbuhan_pinjaman_level_ljk x
                    WHERE " + whereQuery + @"	                
                ";
                //props.Query = @"
                //SELECT dm_periode, dm_jenis_ljk, dm_kode_ljk, dm_outstanding_sekarang, dm_outstanding_sebelumnya, 
                //ROUND(dm_persen_pertumbuhan_outstanding, 3) AS dm_persen_pertumbuhan_outstanding, dm_jenis_pertumbuhan, 
                //dm_current_period, dm_prev_period FROM dbo.macro_pertumbuhan_pinjaman_level_ljk x
                //WHERE " + whereQuery + @"
                //";
            }
            else
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_periode, dm_jenis_pertumbuhan,
                    SUM(x.dm_persen_pertumbuhan_outstanding) AS dm_persen_pertumbuhan_outstanding
                    FROM dbo.macro_pertumbuhan_pinjaman_level_ljk  x
                    WHERE " + whereQuery + @"	 
                    GROUP BY x.dm_jenis_pertumbuhan, x.dm_periode
                    ORDER BY x.dm_periode
                    ";
                }
                else
                {
                    //props.Query = @"
                    //SELECT FORMAT(x.dm_periode, 'yyyyMM') AS dm_periode, dm_jenis_pertumbuhan,
                    //ROUND(SUM(x.dm_persen_pertumbuhan_outstanding), 3) AS dm_persen_pertumbuhan_outstanding
                    //FROM dbo.macro_pertumbuhan_pinjaman_level_ljk  x
                    //WHERE " + whereQuery + @"	 
                    //GROUP BY x.dm_jenis_pertumbuhan, x.dm_periode
                    //ORDER BY x.dm_periode
                    //";

                    props.Query = @"
                    SELECT FORMAT(x.dm_periode, 'yyyyMM') AS dm_periode, dm_jenis_pertumbuhan, ROUND(SUM(persen_pertumbuhan_outstanding), 3) AS dm_persen_pertumbuhan_outstanding FROM (
                    SELECT CASE WHEN dm_outstanding_sebelumnya = 0 THEN 0 ELSE ((dm_outstanding_sekarang - dm_outstanding_sebelumnya) / dm_outstanding_sebelumnya) * 100 END AS persen_pertumbuhan_outstanding,* FROM dbo.macro_pertumbuhan_pinjaman_level_ljk
                    ) x
                    WHERE " + whereQuery + @"	
                    GROUP BY x.dm_periode, x.dm_jenis_pertumbuhan
				    ORDER BY x.dm_periode
                    ";
                }

            }

            props.SqlParameters = new List<SqlParameter>();
            props.SqlParameters.Add(new SqlParameter("periodeAwal", System.Data.SqlDbType.Date) { Value = periodeAwal });
            props.SqlParameters.Add(new SqlParameter("periodeAkhir", System.Data.SqlDbType.Date) { Value = periodeAkhir });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMacro_AMPenetrasiQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisDebiturs, string kategori, string deskripsiKategoris, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_debitur in (" + jenisDebiturs + ")";
            }
            if (kategori != null)
            {
                kategori = "'" + kategori.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kategori = " + kategori;
            }
            if (deskripsiKategoris != null)
            {
                deskripsiKategoris = "'" + deskripsiKategoris.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(REPLACE(dm_deskripsi_kategori, '-', '~'), ',', '|') in (" + deskripsiKategoris + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"

                select * from dbo.macro_penetrasi_lending_ljk x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT TOP 10 dm_deskripsi_kategori, SUM(dm_outstanding) AS dm_outstanding
                FROM dbo.macro_penetrasi_lending_ljk  x
                WHERE x.dm_deskripsi_kategori != 'null' AND " + whereQuery + @"	 
                GROUP BY x.dm_deskripsi_kategori
                ORDER BY dm_outstanding DESC
            ";
            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMicro_LiquidityRiskQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kelasPlafons, string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kelasPlafons != null)
            {
                kelasPlafons = "'" + kelasPlafons.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_kelas_plafon, '-', '~') in (" + kelasPlafons + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }


            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                props.Query = @"
                SELECT dm_kelas_plafon, SUM(dm_plafon) AS dm_plafon
                FROM dbo.micro_plafond_usability_acc_detail  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kelas_plafon
            ";
            }
            else if (isPieChart)
            {
                isC = true;
                props.Query = @"
                SELECT dm_kelas_plafon, COUNT(*) AS dm_jumlah_debitur
                FROM dbo.micro_plafond_usability_acc_detail  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kelas_plafon
            ";
            }
            else
            {
                props.Query = @"

                select * from dbo.micro_plafond_usability_acc_detail x
                WHERE " + whereQuery + @"	                
            ";
            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periodeAwal", System.Data.SqlDbType.Date) { Value = periodeAwal });
            //props.SqlParameters.Add(new SqlParameter("periodeAkhir", System.Data.SqlDbType.Date) { Value = periodeAkhir });
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMacro_PolicyEAQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisDebiturs, string kolektibilitas, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(REPLACE(dm_jenis_debitur, '-', '~'), ',', '|') in (" + jenisDebiturs + ")";
            }
            if (kolektibilitas != null)
            {
                kolektibilitas = "'" + kolektibilitas.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kolektibilitas_bulan_sebelumnya + '~' + dm_kolektibilitas_sekarang in (" + kolektibilitas + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                select * from dbo.macro_policy_evaluation_analysis x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT dm_kolektibilitas_sekarang, SUM(dm_jumlah_hari_tunggakan) AS dm_jumlah_hari_tunggakan
                FROM dbo.macro_policy_evaluation_analysis  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kolektibilitas_sekarang                                               
            ";
            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMA_KKLQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kodeJenisPinjamans, string kolektibilitass, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kodeJenisPinjamans != null)
            {
                kodeJenisPinjamans = "'" + kodeJenisPinjamans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_jenis_pinjaman in (" + kodeJenisPinjamans + ")";
            }
            if (kolektibilitass != null)
            {
                kolektibilitass = "'" + kolektibilitass.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kolektibilitas in (" + kolektibilitass + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"

                select * from dbo.ma_kolektibilitas_kesalahan_ljk x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT dm_kolektibilitas, COUNT(dm_nama_debitur) AS dm_jumlah_debitur, SUM(dm_baki_debet) AS dm_baki_debet
                FROM dbo.ma_kolektibilitas_kesalahan_ljk  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kolektibilitas                                               
            ";
            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMicro_CreditRiskQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kolektabilitass, string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;
            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kolektabilitass != null)
            {
                kolektabilitass = "'" + kolektabilitass.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kolektibilitas in (" + kolektabilitass + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }


            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                whereQuery = whereQuery += " AND dm_plafon = dm_baki_debet";
                props.Query = @"
                SELECT x.dm_kode_jenis_pinjaman, COUNT(*) AS dm_count
                FROM dbo.micro_credit_risk_analysis  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_kode_jenis_pinjaman
                ORDER BY dm_count DESC
            ";
                isC = true;
            }
            else if (isPieChart)
            {
                props.Query = @"
                SELECT x.dm_base,COUNT(*) AS dm_count FROM (
                SELECT CASE WHEN dm_plafon = dm_baki_debet THEN 'Setara' ELSE 'Tidak Setara' END AS dm_base,* FROM dbo.micro_credit_risk_analysis
                ) x
                WHERE " + whereQuery + @"	
                GROUP BY x.dm_base
            ";
                isC = true;
            }
            else
            {
                props.Query = @"

                select * from dbo.micro_credit_risk_analysis x
                WHERE " + whereQuery + @"	                
            ";
            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periodeAwal", System.Data.SqlDbType.Date) { Value = periodeAwal });
            //props.SqlParameters.Add(new SqlParameter("periodeAkhir", System.Data.SqlDbType.Date) { Value = periodeAkhir });
            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMA_APIQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kategoriAsesmens, string similarityScores, string similarityResults, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kategoriAsesmens != null)
            {
                kategoriAsesmens = "'" + kategoriAsesmens.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kategori_asesmen in (" + kategoriAsesmens + ")";
            }
            if (similarityScores != null)
            {
                var start = similarityScores.Split(", ")[0];
                var end = similarityScores.Split(", ")[1];
                start = "'" + start.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                end = "'" + end.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_similarity_score >= " + start;
                whereQuery = whereQuery += " AND dm_similarity_score <= " + end;
            }
            if (similarityResults != null)
            {
                similarityResults = "'" + similarityResults.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_similarity_result in (" + similarityResults + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"

                select * from dbo.ma_analisis_pencurian_identitas x
                WHERE " + whereQuery + @"	                
            ";

            }
            else
            {
                isC = true;
                props.Query = @"
                SELECT x.dm_base,COUNT(*) AS dm_count FROM (
                SELECT CASE WHEN dm_similarity_result=0 THEN 'Tidak' ELSE 'Ya' END AS dm_base,* FROM dbo.ma_analisis_pencurian_identitas
                ) x
                WHERE " + whereQuery + @"	
                GROUP BY x.dm_base                                           
            ";
            }

            //props.SqlParameters = new List<SqlParameter>();
            //props.SqlParameters.Add(new SqlParameter("periode", System.Data.SqlDbType.Date) { Value = periode });

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetCoverageMapQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes,
            string members, string caks, string provs, string kotas, string periode, string tipeChart, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }
            if (caks != null)
            {
                caks = "'" + caks.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_cakupan in (" + caks + ")";
            }
            if (provs != null)
            {
                provs = "'" + provs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_dati1 in (" + provs + ")";
            }
            if (kotas != null)
            {
                kotas = "'" + kotas.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_dati2 in (" + kotas + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                SELECT * FROM dbo.coverage_map_ljkxcity x
                WHERE " + whereQuery + @"	                
                ";
            }
            else
            {
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT DISTINCT dm_dati1 as id,NULL AS parent_id,dm_dati1 AS name,NULL AS value FROM dbo.coverage_map_ljkxcity x
                    WHERE " + whereQuery + @"
                    UNION ALL
                    SELECT dm_dati2 AS id,dm_dati1 AS parent_id,concat(dm_dati2,' - ',CASE WHEN SUM(x.dm_nilai)>1000000 then concat(format_number(SUM(x.dm_nilai)/1000000,0),'Juta')ELSE SUM(x.dm_nilai) END)   AS name,SUM(dm_nilai) AS value FROM dbo.coverage_map_ljkxcity x
                    WHERE " + whereQuery + @"
                    GROUP BY dm_dati1,dm_dati2
                    ";
                }
                else
                {
                    props.Query = @"
                    (SELECT DISTINCT dm_dati1 as id,NULL AS parent_id,dm_dati1 AS name,NULL AS value FROM dbo.coverage_map_ljkxcity x
                    WHERE " + whereQuery + @"
                    )
                    UNION
                    (SELECT dm_dati2 AS id,dm_dati1 AS parent_id,dm_dati2 + ' - ' +CASE	WHEN SUM(dm_nilai)>1000000 THEN FORMAT(CONVERT(INTEGER,SUM(dm_nilai)/1000000), '#,#') + ' Juta'  ELSE FORMAT(CONVERT(INTEGER,SUM(dm_nilai)), '#,#')  END AS name,SUM(dm_nilai) AS value FROM dbo.coverage_map_ljkxcity x
                    WHERE " + whereQuery + @"
                    GROUP BY dm_dati1,dm_dati2
                    )";
                }

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isChart, isHive);
        }
        public static WSQueryReturns GetOsintQuery(DataEntities db, DataSourceLoadOptions loadOptions, string jns, string inq, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pcategory_scrapping in (" + jns + ")";
            }
            if (inq != null)
            {
                inq = "'%" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND (LOWER(dm_keyword) LIKE (" + inq.ToLower() + ") OR LOWER(dm_judul_berita) LIKE (" + inq + ")" + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode_scrapping in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT concat_ws('~',cast(dm_pperiode_scrapping as string),dm_pcategory_scrapping,dm_keyword) AS lem,concat_ws('~',cast(dm_pperiode_scrapping as string),dm_pcategory_scrapping,dm_keyword,dm_judul_berita) AS lem2,* FROM dbo.osint_scrapping_filtered x
                    WHERE " + whereQuery + @"	                
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT FORMAT(x.dm_pperiode_scrapping, 'yyyyMM') + '~' + dm_pcategory_scrapping + '~' + dm_keyword AS lem,FORMAT(x.dm_pperiode_scrapping, 'yyyyMM') + '~' + dm_pcategory_scrapping + '~' + dm_keyword + '~' + dm_judul_berita AS lem2,* FROM dbo.osint_scrapping_filtered x
                    WHERE " + whereQuery + @"	                
                    ";
                }

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetOsintQueryDetail(DataEntities db, DataSourceLoadOptions loadOptions, string jns, string inq, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pcategory_scrapping in (" + jns + ")";
            }
            if (inq != null)
            {
                if (isChart == true)
                {
                    whereQuery = whereQuery += " AND dm_key_word = '" + inq + "'";
                    //if (isHive == true)
                    //{
                    //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_key_word,'(?:[^0-9a-zA-Z._]|\\.(?!\\w+$))', '') = " + inq + "";
                    //}
                    //else
                    //{
                    //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    //    whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_key_word, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),' ','') = " + inq + "";
                    //}

                }
                else
                {
                    whereQuery = whereQuery += " AND dm_keyword = '" + inq + "'";
                    //if (isHive == true)
                    //{
                    //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_keyword,'(?:[^0-9a-zA-Z._]|\\.(?!\\w+$))', '') = " + inq + "";
                    //}
                    //else
                    //{
                    //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    //    whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_keyword, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),' ','') = " + inq + "";
                    //}

                }

            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode_scrapping in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT * FROM dbo.osint_scrapping_filtered x
                    WHERE " + whereQuery + @"	                
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT * FROM dbo.osint_scrapping_filtered x
                    WHERE " + whereQuery + @"	                
                    ";
                }

            }
            else
            {
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT dm_token,SUM(dm_freq) AS dm_count FROM dbo.osint_tfidf_wordcloud x
                    WHERE " + whereQuery + @"	
                    GROUP BY dm_token
                    ORDER BY SUM(dm_freq) DESC 
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 dm_token,SUM(dm_freq) AS dm_count FROM dbo.osint_tfidf_wordcloud x
                    WHERE " + whereQuery + @"	
                    GROUP BY dm_token
                    ORDER BY SUM(dm_freq) DESC 
                    ";
                }

            }


            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetOsintENQuery(DataEntities db, DataSourceLoadOptions loadOptions, string jns, string inq, string periode, string tipe, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pcategory_scrapping in (" + jns + ")";
            }
            if (inq != null)
            {
                whereQuery = whereQuery += " AND dm_key_word = '" + inq + "'";
                //if (isHive == true)
                //{
                //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_key_word,'(?:[^0-9a-zA-Z._]|\\.(?!\\w+$))', '') = " + inq + "";
                //}
                //else
                //{
                //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //    whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_key_word, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),' ','') = " + inq + "";
                //}

            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode_scrapping in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (tipe == "Node")
            {
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT dm_token AS id,cast(dm_score as float) as score,concat('',cast(cast(cast(dm_score as float) * 300 as integer) as string),'px') as wh FROM ojkdt.osint_node_wordpair x 
                    WHERE " + whereQuery + @"
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT dm_token AS id,CONVERT(FLOAT,dm_score) as score,CONVERT(VARCHAR(500),CONVERT(INTEGER,CONVERT(FLOAT,dm_score) * 300)) + 'px' as wh FROM dbo.osint_node_wordpair x 
                    WHERE " + whereQuery + @"
                    ";
                }

            }
            else
            {
                whereQuery = whereQuery += " AND x.dm_node1 IN (SELECT dm_token FROM dbo.osint_node_wordpair WHERE " + whereQuery + @" ) AND x.dm_node2 IN (SELECT dm_token FROM dbo.osint_node_wordpair WHERE " + whereQuery + @" ) ";
                props.Query = @"
                SELECT dm_node1 + dm_node2 AS id,dm_node1 AS source,dm_node2 AS target FROM dbo.osint_edge_wordpair x
                WHERE " + whereQuery + @"	                
                ";
            }

            return WSQueryHelper.DoQueryNL(db, props, isC, isHive);
        }
        public static WSQueryReturns GetOsintWCQuery(DataEntities db, DataSourceLoadOptions loadOptions, string jns, string inq, string periode, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pcategory_scrapping in (" + jns + ")";
            }
            if (inq != null)
            {
                whereQuery = whereQuery += " AND dm_key_word = '" + inq + "'";
                //if (isHive == true)
                //{
                //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_key_word,'(?:[^0-9a-zA-Z._]|\\.(?!\\w+$))', '') = " + inq + "";
                //}
                //else
                //{
                //    inq = "'" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //    whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_key_word, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),' ','') = " + inq + "";
                //}

            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode_scrapping in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            props.Query = @"
            SELECT dm_token  FROM dbo.osint_tfidf_wordcloud x 
            WHERE " + whereQuery + @"
            ";


            return WSQueryHelper.DoQueryNL(db, props, isC, isHive);
        }
        public static WSQueryReturns GetDNAQuery(DataEntities db, DataSourceLoadOptions loadOptions, string jns, string jas, string members, string periode, string tipeChart, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";
            bool ctAgunan = false;
            bool ctCif = false;
            bool ctLJK = false;
            bool ctDebitur = false;
            if (jns != null)
            {
                if (jns.Contains("Agunan"))
                {
                    ctAgunan = true;
                }
                if (jns.Contains("CIF"))
                {
                    ctCif = true;
                }
                if (jns.Contains("Kreditur"))
                {
                    ctLJK = true;
                }
                if (jns.Contains("Debitur"))
                {
                    ctDebitur = true;
                }
            }
            if (members != null)
            {
                ctAgunan = true;
            }
            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //stringJns = " (" + jns + ")";
                whereQuery = whereQuery += " AND dm_node_type LIKE (" + jns + ")";
                //whereQuery = whereQuery += "AND (";
                //int countI = 0;
                //foreach (var i in jns.Split(",")) {
                //    string i2 = "'" + i.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                //    if (countI == 0)
                //    {
                //        whereQuery = whereQuery += " dm_node_type LIKE (" + jns + ")";
                //    }
                //    else {
                //        whereQuery = whereQuery += " OR dm_node_type LIKE (" + jns + ")";
                //    }
                //    countI = countI + 1;
                //}
                //whereQuery = whereQuery += ")";
            }
            if (jas != null)
            {
                jas = "'" + jas.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //stringJas = " (" + jas + ")";
                whereQuery = whereQuery += " AND regexp_extract(dm_node_id, '^(.*?)( ID )') in (" + jas + ")";
                //whereQuery = whereQuery += "AND (";
                //int countI = 0;
                //foreach (var i in jas.Split(","))
                //{
                //    string i2 = "'" + i.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                //    if (countI == 0)
                //    {
                //        whereQuery = whereQuery += " dm_node_id LIKE (" + jns + ")";
                //    }
                //    else
                //    {
                //        whereQuery = whereQuery += " OR dm_node_id LIKE (" + jns + ")";
                //    }
                //    countI = countI + 1;
                //}
                //whereQuery = whereQuery += ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND (regexp_extract(dm_node_id, 'member:(.*$)') IN (" + members + ") OR dm_node_id IN (" + members + "))";
                //stringMembers = " (" + members + ")";
                //if (isHive == true)
                //{
                //    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                //    stringMembers= " (" + members + ")";
                //    whereQuery = whereQuery += " AND x.dm_kode_ljk LIKE (" + members + ")";
                //}
                //else
                //{
                //    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                //}
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                //stringPeriode = " (" + periode + ")";
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                string query0 = null;
                string query1 = null;
                string query2 = null;
                if (isHive == true)
                {
                    query1 = @"
                        select concat_ws('~',cast(dm_periode as string),dm_relationship,dm_first_node_id,dm_second_node_id) as lem,* from dbo.edge_collateral
                        where 
                        ";
                }
                else
                {
                    query1 = @"
                        select top 10000 FORMAT(dm_periode, 'yyyyMM') +'~' + dm_relationship + '~' + dm_first_node_id + '~' + dm_second_node_id AS lem ,* from dbo.edge_collateral
                        where 
                        ";
                }

                if (ctAgunan == true || ctLJK == true)
                {
                    query1 = query1 + " 1=1";
                }
                else
                {
                    query1 = query1 + " 1=0";
                }
                if (periode != null)
                {
                    query1 = query1 += " AND dm_periode in (" + periode + ")";
                }
                query1 = query1 + @" and dm_relationship = 'Agunan <> Kreditur' ";
                if (members != null)
                {
                    query1 = query1 + @" and dm_second_node_id in (" + members + ") ";

                }
                if (jas != null)
                {
                    if (isHive == true)
                    {
                        query1 = query1 + @" and regexp_extract(dm_first_node_id, '^(.*?)( ID )') in (" + jas + ")";
                    }
                    else
                    {
                        query1 = query1 + @" and substring(dm_first_node_id, 1, charindex('ID - ', dm_first_node_id)-2) in (" + jas + ")";
                    }

                }

                query1 = query1 + @" UNION ALL ";
                if (isHive == true)
                {
                    query1 = query1 + @"
                        select concat_ws('~',cast(dm_periode as string),dm_relationship,dm_first_node_id,dm_second_node_id) as lem,* from dbo.edge_collateral
                        where 
                        ";
                }
                else
                {
                    query1 = query1 + @"
                        select top 10000 FORMAT(dm_periode, 'yyyyMM') +'~' + dm_relationship + '~' + dm_first_node_id + '~' + dm_second_node_id AS lem ,* from dbo.edge_collateral
                        where 
                        ";
                }

                if (ctCif == true || ctAgunan == true)
                {
                    query1 = query1 + " 1=1";
                }
                else
                {
                    query1 = query1 + " 1=0";
                }
                if (periode != null)
                {
                    query1 = query1 += " AND dm_periode in (" + periode + ")";
                }
                query1 = query1 + @" and dm_relationship = 'CIF <> Agunan' ";
                if (members != null)
                {
                    if (isHive == true)
                    {
                        query1 = query1 + @" and regexp_extract(dm_first_node_id, 'member:(.*$)') in (" + members + ") ";
                    }
                    else
                    {
                        query1 = query1 + @" and substring(dm_first_node_id, charindex('member:', dm_first_node_id)+7,1000) in (" + members + ") ";
                    }

                }
                if (jas != null)
                {
                    if (isHive == true)
                    {
                        query1 = query1 + @" and regexp_extract(dm_second_node_id, '^(.*?)( ID )') in (" + jas + ") ";
                    }
                    else
                    {
                        query1 = query1 + @" and substring(dm_second_node_id, 1, charindex('ID - ', dm_second_node_id)-2) in (" + jas + ") ";
                    }

                }
                if (isHive == true)
                {
                    query2 = @"
                        select concat_ws('~',cast(dm_periode as string),dm_relationship,dm_first_node_id,dm_second_node_id) as lem,* from dbo.edge_collateral
                        where 
                        ";
                }
                else
                {
                    query2 = @"
                        select top 10000 FORMAT(dm_periode, 'yyyyMM') +'~' + dm_relationship + '~' + dm_first_node_id + '~' + dm_second_node_id AS lem ,* from dbo.edge_collateral
                        where 
                        ";
                }
                if (ctLJK == true || ctCif == true)
                {
                    query2 = query2 + " 1=1";
                }
                else
                {
                    query2 = query2 + " 1=0";
                }
                if (periode != null)
                {
                    query2 = query2 += " AND dm_periode in (" + periode + ")";
                }
                query2 = query2 + @" and dm_relationship = 'Kreditur <> CIF' ";
                if (members != null)
                {
                    query2 = query2 + @" and dm_first_node_id in (" + members + ") ";
                }
                query2 = query2 + @" UNION ALL";
                if (isHive == true)
                {
                    query2 = query2 + @"
                        select concat_ws('~',cast(dm_periode as string),dm_relationship,dm_first_node_id,dm_second_node_id) as lem,* from dbo.edge_collateral
                        where 
                        ";
                }
                else
                {
                    query2 = query2 + @"
                        select top 10000 FORMAT(dm_periode, 'yyyyMM') +'~' + dm_relationship + '~' + dm_first_node_id + '~' + dm_second_node_id AS lem ,* from dbo.edge_collateral
                        where 
                        ";
                }
                if (ctDebitur == true || ctCif == true)
                {
                    query2 = query2 + " 1=1";
                }
                else
                {
                    query2 = query2 + " 1=0";
                }
                if (periode != null)
                {
                    query2 = query2 += " AND dm_periode in (" + periode + ")";
                }
                query2 = query2 + @" and dm_relationship = 'ID <> CIF' ";
                if (members != null)
                {
                    if (isHive == true)
                    {
                        query2 = query2 + @" and regexp_extract(dm_second_node_id, 'member:(.*$)') in (" + members + ") ";
                    }
                    else
                    {
                        query2 = query2 + @" and substring(dm_second_node_id, charindex('member:', dm_second_node_id)+7,1000) in (" + members + ") ";
                    }

                }
                //if (jas != null || members != null)
                //{

                //}
                //if (members != null) {

                //}
                if (ctAgunan == false && ctCif == false && ctDebitur == false && ctLJK == false)
                {
                    if (isHive == true)
                    {
                        //query0 = @"
                        //select  concat_ws('~',dm_periode,REGEXP_REPLACE(dm_relationship,'(?:[^0-9a-zA-Z._]|\\.(?!<>\\w+$))', ''),REGEXP_REPLACE(dm_first_node_id,'(?:[^0-9a-zA-Z._]|\\.(?!<>\\w+$))', ''),REGEXP_REPLACE(dm_second_node_id,'(?:[^0-9a-zA-Z._]|\\.(?!<>\\w+$))', '')) AS lem,* from dbo.edge_collateral
                        //where 1=1
                        //";
                        query0 = @"
                        select  concat_ws('~',cast(dm_periode as string),dm_relationship,dm_first_node_id,dm_second_node_id) as lem,* from dbo.edge_collateral
                        where 1=1
                        ";
                    }
                    else
                    {
                        //query0 = @"
                        //select top 100 REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(FORMAT(dm_periode, 'yyyyMM') +'~' + dm_relationship + '~' + dm_first_node_id + '~' + dm_second_node_id, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),CHAR(58),''),CHAR(44),''),CHAR(46),''),CHAR(47),''),CHAR(60),''),CHAR(62),''),' ','') AS lem ,* from dbo.edge_collateral
                        //where 1=1
                        //";
                        query0 = @"
                        select top 10000 FORMAT(dm_periode, 'yyyyMM') +'~' + dm_relationship + '~' + dm_first_node_id + '~' + dm_second_node_id as lem,* from dbo.edge_collateral
                        where 1=1
                        ";
                    }
                    if (periode != null)
                    {
                        query0 = query0 += " AND dm_periode in (" + periode + ")";
                    }
                }
                else
                {
                    if (query1 != null)
                    {
                        query0 = query1;
                    }
                    if (query2 != null)
                    {
                        if (query0 == null)
                        {
                            query0 = query2;
                        }
                        else
                        {
                            query0 = query0 + " UNION ALL " + query2;
                        }
                    }
                }
                props.Query = query0;

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetDNADetailQuery(DataEntities db, DataSourceLoadOptions loadOptions, string rn, string fn, string sn, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";
            if (rn != null)
            {
                whereQuery = whereQuery += " AND dm_relationship in (" + rn + ") ";
                //if (isHive == true)
                //{
                //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_relationship,'(?:[^0-9a-zA-Z._]|\\.(?!<>\\w+$))', '') = " + rn + "";
                //}
                //else
                //{
                //    //whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_relationship, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),CHAR(58),''),CHAR(44),''),CHAR(46),''),CHAR(47),''),CHAR(60),''),CHAR(62),''),' ','') in (" + rn + ") ";
                //    whereQuery = whereQuery += " AND dm_relationship in (" + rn + ") ";
                //}
            }
            if (fn != null)
            {
                whereQuery = whereQuery += " AND dm_first_node_id in (" + fn + ") ";
                //if (isHive == true)
                //{
                //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_first_node_id,'(?:[^0-9a-zA-Z._]|\\.(?!<>\\w+$))', '') = " + fn + "";
                //}
                //else
                //{
                //    //whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_first_node_id, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),CHAR(58),''),CHAR(44),''),CHAR(46),''),CHAR(47),''),CHAR(60),''),CHAR(62),''),' ','') in (" + fn + ") ";
                //    whereQuery = whereQuery += " AND dm_first_node_id in (" + fn + ") ";
                //}
            }
            if (sn != null)
            {
                whereQuery = whereQuery += " AND dm_second_node_id in (" + sn + ") ";
                //if (isHive == true)
                //{
                //    whereQuery = whereQuery += " AND REGEXP_REPLACE(dm_second_node_id,'(?:[^0-9a-zA-Z._]|\\.(?!<>\\w+$))', '') = " + sn + "";
                //}
                //else
                //{
                //    //whereQuery = whereQuery += " AND REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dm_second_node_id, CHAR(34), ''), CHAR(36), ''),CHAR(38),''),CHAR(45),''),CHAR(58),''),CHAR(44),''),CHAR(46),''),CHAR(47),''),CHAR(60),''),CHAR(62),''),' ','') in (" + sn + ") ";
                //    whereQuery = whereQuery += " AND dm_second_node_id in (" + sn + ") ";
                //}
            }

            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (!isChart)
            {
                props.Query = @"
                SELECT *  FROM dbo.edge_collateral x 
                WHERE " + whereQuery + @"
                ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetDNAENQuery(DataEntities db, DataSourceLoadOptions loadOptions, string[] detail, string periode, string tipe, bool isHive = false)
        {
            bool isC = false;
            string stringPeriode = null;
            bool ctAgunan = false;
            bool ctCif = false;
            bool ctDebitur = false;
            bool ctLJK = false;
            string query1 = null;
            string query2 = null;
            string query3 = null;
            string query4 = null;
            string queryUnion = null;
            string whereQuery1 = @" where 1=1";
            string whereQuery2 = @" where 1=1";
            string whereQuery3 = @" where 1=1";
            string whereQuery4 = @" where 1=1";
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery1 = whereQuery1 += " AND dm_periode in (" + periode + ") ";
                whereQuery2 = whereQuery2 += " AND dm_periode in (" + periode + ") ";
                whereQuery3 = whereQuery3 += " AND dm_periode in (" + periode + ") ";
                whereQuery4 = whereQuery4 += " AND dm_periode in (" + periode + ") ";
            }

            if (detail != null)
            {
                string agu = null;
                string cif = null;
                string ljk = null;
                string deb = null;
                whereQuery1 = whereQuery1 += " AND dm_relationship='Agunan <> Kreditur' ";
                whereQuery2 = whereQuery2 += " AND dm_relationship='CIF <> Agunan' ";
                whereQuery3 = whereQuery3 += " AND dm_relationship='Kreditur <> CIF' ";
                whereQuery4 = whereQuery4 += " AND dm_relationship='ID <> CIF' ";
                foreach (var i in detail.Where(x => x.Contains("Agunan <> Kreditur") == true))
                {
                    var i2 = i.Split("~");
                    if (agu == null)
                    {
                        agu = "'" + i2[2] + "'";
                    }
                    else
                    {
                        agu = agu + ",'" + i2[2] + "'";
                    }
                    if (ljk == null)
                    {
                        ljk = "'" + i2[3] + "'";
                    }
                    else
                    {
                        ljk = ljk + ",'" + i2[3] + "'";
                    }
                    ctAgunan = true;
                    ctLJK = true;
                }
                foreach (var i in detail.Where(x => x.Contains("CIF <> Agunan") == true))
                {
                    var i2 = i.Split("~");
                    if (agu == null)
                    {
                        agu = "'" + i2[3] + "'";
                    }
                    else
                    {
                        if (agu.Contains(i2[3]) == false)
                        {
                            if (agu == null)
                            {
                                agu = "'" + i2[3] + "'";
                            }
                            else
                            {
                                agu = agu + ",'" + i2[3] + "'";
                            }
                        }
                    }
                    if (cif == null)
                    {
                        cif = "'" + i2[2] + "'";
                    }
                    else
                    {
                        if (cif.Contains(i2[2]) == false)
                        {
                            if (cif == null)
                            {
                                cif = "'" + i2[2] + "'";
                            }
                            else
                            {
                                cif = cif + ",'" + i2[2] + "'";
                            }
                        }
                    }

                    ctCif = true;
                    ctAgunan = true;
                }
                foreach (var i in detail.Where(x => x.Contains("Kreditur <> CIF") == true))
                {
                    var i2 = i.Split("~");
                    if (ljk == null)
                    {
                        ljk = "'" + i2[2] + "'";
                    }
                    else
                    {
                        if (ljk.Contains(i2[2]) == false)
                        {
                            if (ljk == null)
                            {
                                ljk = "'" + i2[2] + "'";
                            }
                            else
                            {
                                ljk = ljk + ",'" + i2[2] + "'";
                            }
                        }
                    }
                    if (cif == null)
                    {
                        cif = "'" + i2[3] + "'";
                    }
                    else
                    {
                        if (cif.Contains(i2[3]) == false)
                        {
                            if (cif == null)
                            {
                                cif = "'" + i2[3] + "'";
                            }
                            else
                            {
                                cif = cif + ",'" + i2[3] + "'";
                            }
                        }
                    }

                    ctCif = true;
                    ctLJK = true;
                }
                foreach (var i in detail.Where(x => x.Contains("ID <> CIF") == true))
                {
                    var i2 = i.Split("~");
                    if (deb == null)
                    {
                        deb = "'" + i2[2] + "'";
                    }
                    else
                    {
                        if (deb.Contains(i2[2]) == false)
                        {
                            if (deb == null)
                            {
                                deb = "'" + i2[2] + "'";
                            }
                            else
                            {
                                deb = deb + ",'" + i2[2] + "'";
                            }
                        }
                    }
                    if (cif == null)
                    {
                        cif = "'" + i2[3] + "'";
                    }
                    else
                    {
                        if (cif.Contains(i2[3]) == false)
                        {
                            if (cif == null)
                            {
                                cif = "'" + i2[3] + "'";
                            }
                            else
                            {
                                cif = cif + ",'" + i2[3] + "'";
                            }
                        }
                    }

                    ctCif = true;
                    ctDebitur = true;
                }
                if (ctAgunan == true || ctLJK == true)
                {
                    if (ljk != null)
                    {
                        whereQuery1 = whereQuery1 + " AND dm_second_node_id in (" + ljk + ") ";
                    }
                    if (agu != null)
                    {
                        whereQuery1 = whereQuery1 + " AND dm_first_node_id in (" + agu + ") ";
                    }
                    query1 = "select * from dbo.edge_collateral " + whereQuery1;
                }
                else
                {
                    query1 = "select * from dbo.edge_collateral " + whereQuery1 + " AND 1=0";
                }
                if (ctCif == true || ctAgunan == true)
                {
                    if (agu != null)
                    {
                        whereQuery2 = whereQuery2 + " AND dm_second_node_id in (" + agu + ") ";
                    }
                    if (cif != null)
                    {
                        whereQuery2 = whereQuery2 + " AND dm_first_node_id in (" + cif + ") ";
                    }
                    query2 = "select * from dbo.edge_collateral " + whereQuery2;
                }
                else
                {
                    query2 = "select * from dbo.edge_collateral " + whereQuery2 + " AND 1=0";
                }
                if (ctLJK == true || ctCif == true)
                {
                    if (cif != null)
                    {
                        whereQuery3 = whereQuery3 + " AND dm_second_node_id in (" + cif + ") ";
                    }
                    if (ljk != null)
                    {
                        whereQuery3 = whereQuery3 + " AND dm_first_node_id in (" + ljk + ") ";
                    }
                    query3 = "select * from dbo.edge_collateral " + whereQuery3;
                }
                else
                {
                    query3 = "select * from dbo.edge_collateral " + whereQuery3 + " AND 1=0";
                }
                if (ctDebitur == true || ctCif == true)
                {
                    if (cif != null)
                    {
                        whereQuery4 = whereQuery4 + " AND dm_second_node_id in (" + cif + ") ";
                    }
                    if (deb != null)
                    {
                        whereQuery4 = whereQuery4 + " AND dm_first_node_id in (" + deb + ") ";
                    }
                    query4 = "select * from dbo.edge_collateral " + whereQuery4;
                }
                else
                {
                    query4 = "select * from dbo.edge_collateral " + whereQuery4;
                }
                queryUnion = " with one as(" + query1 + " UNION ALL " + query2 + "),two as(" + query3 + "),tri as(" + query4 + ")";
                if (tipe == "Node")
                {
                    if (isHive == true)
                    {
                        queryUnion = queryUnion + "select dm_node_id as id,dm_node_size_factor as score,concat('',cast(cast(dm_node_size_factor * 30 as int) as string),'px') as wh,dm_node_color_type as nc from dbo.node_collateral x";
                    }
                    else
                    {
                        queryUnion = queryUnion + "select dm_node_id as id,dm_node_size_factor as score,CONVERT(VARCHAR(500),CONVERT(INTEGER,CONVERT(FLOAT,dm_node_size_factor) * 30)) + 'px' as wh,dm_node_color_type as nc from dbo.node_collateral x";
                    }
                    queryUnion = queryUnion + @" 
                    where dm_periode IN (" + periode + @" )
                    and dm_node_id in(
                    select dm_first_node_id  from one ";
                    if (ctLJK == false && ctAgunan == false)
                    {
                        queryUnion = queryUnion + @" union all 
                        select dm_second_node_id from one ";
                    }
                    queryUnion = queryUnion + @" union all
                    select dm_first_node_id  from two
                    where dm_second_node_id in (select dm_first_node_id from one)
                    UNION ALL
                    SELECT dm_first_node_id FROM tri
                    WHERE dm_second_node_id IN (select dm_first_node_id from one)
                    )
                    ";
                }
                else
                {
                    queryUnion = queryUnion + @" 
                    select dm_first_node_id+dm_second_node_id as id,dm_first_node_id as source,dm_second_node_id as target from one 
                    union all
                    select dm_first_node_id+dm_second_node_id as id,dm_first_node_id as source,dm_second_node_id as target from two
                    where dm_second_node_id in (select dm_first_node_id from one)
                    UNION ALL
                    SELECT dm_first_node_id+dm_second_node_id as id,dm_first_node_id as source,dm_second_node_id as target FROM tri
                    WHERE dm_second_node_id IN (select dm_first_node_id from one)
                    ";
                }
            }

            var props = new WSQueryProperties();



            props.Query = queryUnion;
            return WSQueryHelper.DoQueryNL(db, props, isC, isHive);
        }
        public static WSQueryReturns GetDA_AnomaliFormatKTPQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
        string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }

            }
            //if (jenisTA == "Anomali Penulisan ID KTP")
            //{
            //    jenisTA = "'" + jenisTA.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
            //    whereQuery = whereQuery += " AND x.dm_status_valid_id_debitur = 0";
            //}
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_kode_ljk AS nama_ljk, COUNT(x.dm_kode_ljk) AS dm_count
                    FROM dbo.debitur_anomali_format_ktp x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_kode_ljk
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_kode_ljk, x.dm_jenis_ljk, z.nama_LJK AS nama_ljk, COUNT(x.dm_kode_ljk) AS dm_count FROM dbo.debitur_anomali_format_ktp x
					LEFT JOIN dbo.master_ljk_type y ON y.deskripsi_jenis_ljk = x.dm_jenis_ljk
					LEFT JOIN dbo.master_ljk z ON z.kode_ljk = x.dm_kode_ljk AND z.kode_jenis_ljk = y.kode_jenis_ljk
					WHERE " + whereQuery + @"
					GROUP BY z.nama_ljk, x.dm_jenis_ljk, x.dm_kode_ljk
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    ";
                }

            }
            else if (isPieChart)
            {
                isC = true;
                props.Query = @"

                SELECT a.total, a.tipe FROM (
	                SELECT COUNT(*) AS total, 'Anomali' AS tipe FROM dbo.debitur_anomali_format_ktp x
	                WHERE " + whereQuery + @"
                    HAVING COUNT(*) != 0
                ) AS a
                UNION
                SELECT SUM(b.total), b.tipe FROM (
	                SELECT (y.dm_total - COUNT(*)) AS total, 'Tidak Anomali' AS tipe FROM dbo.debitur_anomali_format_ktp x
	                LEFT JOIN dbo.debitur_anomali_data_populasi y ON y.dm_periode = x.dm_periode AND y.dm_jenis_ljk = x.dm_jenis_ljk AND y.dm_kode_ljk = x.dm_kode_ljk AND y.dm_pjenis_data_populasi = 'Anomali Penulisan ID KTP'
	                WHERE " + whereQuery + @"
	                GROUP BY y.dm_total
                ) AS b
                GROUP BY b.tipe
            ";

            }
            else
            {
                props.Query = @"

                select * from dbo.debitur_anomali_format_ktp x
                WHERE " + whereQuery + @"	                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliGenderQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }
            //if (jenisTA == "Anomali Gender dengan KTP Valid")
            //{
            //    jenisTA = "'" + jenisTA.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
            //    whereQuery = whereQuery += " AND x.dm_status_valid_gender_based_kode = 0 AND x.dm_status_valid_id_debitur = 1";
            //}
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_kode_ljk AS nama_ljk, COUNT(x.dm_kode_ljk) AS dm_count
                    FROM dbo.debitur_anomali_gender x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_kode_ljk
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_kode_ljk, x.dm_jenis_ljk, z.nama_LJK AS nama_ljk, COUNT(x.dm_kode_ljk) AS dm_count FROM dbo.debitur_anomali_gender x
					LEFT JOIN dbo.master_ljk_type y ON y.deskripsi_jenis_ljk = x.dm_jenis_ljk
					LEFT JOIN dbo.master_ljk z ON z.kode_ljk = x.dm_kode_ljk AND z.kode_jenis_ljk = y.kode_jenis_ljk
					WHERE " + whereQuery + @"
					GROUP BY z.nama_ljk, x.dm_jenis_ljk, x.dm_kode_ljk
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    ";
                }

            }
            else if (isPieChart)
            {
                isC = true;
                props.Query = @"

                SELECT a.total, a.tipe FROM (
	                SELECT COUNT(*) AS total, 'Anomali' AS tipe FROM dbo.debitur_anomali_gender x
	                WHERE " + whereQuery + @"
                    HAVING COUNT(*) != 0
                ) AS a
                UNION
                SELECT SUM(b.total), b.tipe FROM (
	                SELECT (y.dm_total - COUNT(*)) AS total, 'Tidak Anomali' AS tipe FROM dbo.debitur_anomali_gender x
	                LEFT JOIN dbo.debitur_anomali_data_populasi y ON y.dm_periode = x.dm_periode AND y.dm_jenis_ljk = x.dm_jenis_ljk AND y.dm_kode_ljk = x.dm_kode_ljk AND y.dm_pjenis_data_populasi = 'Anomali Penulisan Gender dengan KTP Valid'
	                WHERE " + whereQuery + @"
	                GROUP BY y.dm_total
                ) AS b
                GROUP BY b.tipe
            ";

            }
            else
            {
                props.Query = @"

                select * from dbo.debitur_anomali_gender x
                WHERE " + whereQuery + @"              
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliDuplikasiNamaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }
            //if (jenisTA == "Anomali Suspek Duplikasi Nama Debitur")
            //{
            //    jenisTA = "'" + jenisTA.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
            //    whereQuery = whereQuery += " AND x.dm_similarity_result = 1";
            //}
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_kode_ljk AS nama_ljk, COUNT(x.dm_kode_ljk) AS dm_count
                    FROM dbo.debitur_anomali_duplikasi_nama_debitur x
                    WHERE x.dm_similarity_result = 0 AND " + whereQuery + @"
                    GROUP BY x.dm_kode_ljk
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_kode_ljk, x.dm_jenis_ljk, z.nama_LJK AS nama_ljk, COUNT(x.dm_kode_ljk) AS dm_count FROM dbo.debitur_anomali_duplikasi_nama_debitur x
					LEFT JOIN dbo.master_ljk_type y ON y.deskripsi_jenis_ljk = x.dm_jenis_ljk
					LEFT JOIN dbo.master_ljk z ON z.kode_ljk = x.dm_kode_ljk AND z.kode_jenis_ljk = y.kode_jenis_ljk
					WHERE x.dm_similarity_result = 0 AND " + whereQuery + @"
					GROUP BY z.nama_ljk, x.dm_jenis_ljk, x.dm_kode_ljk
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    ";
                }

            }
            else if (isPieChart)
            {
                isC = true;
                props.Query = @"

                SELECT a.total, a.tipe FROM (
	                SELECT COUNT(*) AS total, 'Anomali' AS tipe FROM dbo.debitur_anomali_duplikasi_nama_debitur x
	                WHERE x.dm_similarity_result = 0 AND " + whereQuery + @"
                    HAVING COUNT(*) != 0
                ) AS a
                UNION
                SELECT SUM(b.total), b.tipe FROM (
	                SELECT (y.dm_total - COUNT(*)) AS total, 'Tidak Anomali' AS tipe FROM dbo.debitur_anomali_duplikasi_nama_debitur x
	                LEFT JOIN dbo.debitur_anomali_data_populasi y ON y.dm_periode = x.dm_periode AND y.dm_jenis_ljk = x.dm_jenis_ljk AND y.dm_kode_ljk = x.dm_kode_ljk AND y.dm_pjenis_data_populasi = 'Anomali Dugaan Duplikasi Nama Debitur dengan ID Debitur yang Sama'
	                WHERE x.dm_similarity_result = 0 AND " + whereQuery + @"
	                GROUP BY y.dm_total
                ) AS b
                GROUP BY b.tipe
            ";

            }
            else
            {
                props.Query = @"

                select * from dbo.debitur_anomali_duplikasi_nama_debitur x
                WHERE x.dm_similarity_result = 0 AND " + whereQuery + @"                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliIDAgunanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisAgunans, string similarityScores, string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisAgunans != null)
            {
                jenisAgunans = "'" + jenisAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_agunan in (" + jenisAgunans + ")";
            }
            if (similarityScores != null)
            {
                var start = similarityScores.Split(", ")[0];
                var end = similarityScores.Split(", ")[1];
                start = "'" + start.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                end = "'" + end.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_similarity_score >= " + start;
                whereQuery = whereQuery += " AND dm_similarity_score <= " + end;
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_kode_ljk AS nama_ljk, x.dm_jenis_agunan, COUNT(x.dm_kode_ljk) AS dm_count
                    FROM dbo.agunan_id_anomali x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_kode_ljk, x.dm_jenis_agunan
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_kode_ljk, x.dm_jenis_ljk, z.nama_LJK AS nama_ljk, x.dm_jenis_agunan, COUNT(x.dm_kode_ljk) AS dm_count
                    FROM dbo.agunan_id_anomali x
                    LEFT JOIN dbo.master_ljk_type y ON y.deskripsi_jenis_ljk = x.dm_jenis_ljk
					LEFT JOIN dbo.master_ljk z ON z.kode_ljk = x.dm_kode_ljk AND z.kode_jenis_ljk = y.kode_jenis_ljk
                    WHERE " + whereQuery + @"
                    GROUP BY z.nama_ljk, x.dm_jenis_ljk, x.dm_kode_ljk, x.dm_jenis_agunan
                    ORDER BY COUNT(x.dm_kode_ljk) DESC
                    ";
                }

            }
            else if (isPieChart)
            {
                isC = true;
                props.Query = @"

                SELECT dm_base, COUNT(*) AS dm_count FROM (
                SELECT CASE WHEN dm_similarity_result = 1 THEN 'Anomali' ELSE 'Tidak Anomali' END AS dm_base,* FROM dbo.agunan_id_anomali
                ) x
                WHERE " + whereQuery + @"	
                GROUP BY x.dm_base
            ";
            }
            else
            {
                props.Query = @"

                select * from dbo.agunan_id_anomali x
                WHERE " + whereQuery + @"	                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliDokumenAgunanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string jenisAgunans, string ownershipDocuments, string periode, bool isHive = false, bool isChart = false, bool isPieChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (jenisAgunans != null)
            {
                jenisAgunans = "'" + jenisAgunans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_agunan in (" + jenisAgunans + ")";
            }
            if (ownershipDocuments != null)
            {
                ownershipDocuments = "'" + ownershipDocuments.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_dokumen_kepemilikan_agunan, '-', '~') in (" + ownershipDocuments + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_jenis_agunan, x.dm_dokumen_kepemilikan_agunan, COUNT(x.dm_dokumen_kepemilikan_agunan) AS dm_count
                    FROM dbo.agunan_dokumen_anomali  x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_jenis_agunan, x.dm_dokumen_kepemilikan_agunan
                    ORDER BY COUNT(x.dm_dokumen_kepemilikan_agunan) DESC
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_jenis_agunan, x.dm_dokumen_kepemilikan_agunan, COUNT(x.dm_dokumen_kepemilikan_agunan) AS dm_count
                    FROM dbo.agunan_dokumen_anomali  x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_jenis_agunan, x.dm_dokumen_kepemilikan_agunan
                    ORDER BY COUNT(x.dm_dokumen_kepemilikan_agunan) DESC
                    ";
                }

            }
            else if (isPieChart)
            {
                isC = true;
                if (isHive == true)
                {
                    props.Query = @"
                    SELECT x.dm_jenis_agunan, COUNT(x.dm_dokumen_kepemilikan_agunan) AS dm_count
                    FROM dbo.agunan_dokumen_anomali  x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_jenis_agunan
                    LIMIT 10
                    ";
                }
                else
                {
                    props.Query = @"
                    SELECT TOP 10 x.dm_jenis_agunan, COUNT(x.dm_dokumen_kepemilikan_agunan) AS dm_count
                    FROM dbo.agunan_dokumen_anomali  x
                    WHERE " + whereQuery + @"
                    GROUP BY x.dm_jenis_agunan
                    ";
                }

            }
            else
            {
                props.Query = @"

                select * from dbo.agunan_dokumen_anomali x
                WHERE " + whereQuery + @"	                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliNilaiAgunanDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string nilaiAgunan, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_jenis_debitur, '-', '~') in (" + jenisDebiturs + ")";
            }
            if (nilaiAgunan != null)
            {
                nilaiAgunan = "'" + nilaiAgunan.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_nilai_agunan_menurut_pelapor <= " + nilaiAgunan;
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_nilai_agunan_deb x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliNIKDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_nik_deb x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliGelarNamaDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_gelar_nama_deb x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliAlamatDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string segmens, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (segmens != null)
            {
                segmens = "'" + segmens.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_segmen, '-', '~') in (" + segmens + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_alamat_deb x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnalisisDebtorNomIdentitasSamaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_jenis_debitur, '-', '~') in (" + jenisDebiturs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_analisis_debtor_nom_identitas_sama x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnalisisDebtorNomIdentitasBedaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_jenis_debitur, '-', '~') in (" + jenisDebiturs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_analisis_debtor_nom_identitas_beda x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliNamaIbuKandungQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_nama_ibu_kandung x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliBentukBadanUsahaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_bentuk_badan_usaha x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliNilaiNJOPAgunanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string nilaiNJOP, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_jenis_debitur, '-', '~') in (" + jenisDebiturs + ")";
            }
            if (nilaiNJOP != null)
            {
                nilaiNJOP = "'" + nilaiNJOP.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_nilai_agunan_wajar <= " + nilaiNJOP;
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"
                SELECT
                    CASE 
                        WHEN dm_no_id_debitur = '' OR dm_no_id_debitur = 'null' THEN dm_no_identitas_badan_usaha
                        WHEN dm_no_identitas_badan_usaha = '' OR dm_no_identitas_badan_usaha = 'null' THEN dm_no_id_debitur
                        ELSE concat_ws(' - ', dm_no_id_debitur, dm_no_identitas_badan_usaha)
                    END AS nomor_identitas_nomor_identitas_badan_usaha,
                    CASE 
                        WHEN dm_nama_id_debitur = '' OR dm_nama_id_debitur = 'null' THEN dm_nama_badan_usaha
                        WHEN dm_nama_badan_usaha = '' OR dm_nama_badan_usaha = 'null' THEN dm_nama_id_debitur
                        ELSE concat_ws(' - ', dm_nama_id_debitur, dm_nama_badan_usaha)
                    END AS nama_sesuai_identitas_nama_badan_usaha,
                    CASE 
                        WHEN dm_jenis_badan_usaha = '' OR dm_jenis_badan_usaha = 'null' THEN dm_jenis_debitur
                        WHEN dm_jenis_debitur = '' OR dm_jenis_debitur = 'null' THEN dm_jenis_badan_usaha
                        ELSE concat_ws(' - ', dm_jenis_debitur, dm_jenis_badan_usaha)
                    END AS jenis_debitur_jenis_badan_usaha,
                    *
                FROM
                    dbo.da_anomali_nilai_njop_agunan x
                WHERE " + whereQuery + @"
            ";
            }
            else
            {
                props.Query = @"
                SELECT
                    CASE 
                        WHEN dm_no_id_debitur = '' OR dm_no_id_debitur IS NULL THEN dm_no_identitas_badan_usaha
                        WHEN dm_no_identitas_badan_usaha = '' OR dm_no_identitas_badan_usaha IS NULL THEN dm_no_id_debitur
                        ELSE dm_no_id_debitur + ' - ' + dm_no_identitas_badan_usaha
                    END AS nomor_identitas_nomor_identitas_badan_usaha,
                    CASE 
                        WHEN dm_nama_id_debitur = '' OR dm_nama_id_debitur IS NULL THEN dm_nama_badan_usaha
                        WHEN dm_nama_badan_usaha = '' OR dm_nama_badan_usaha IS NULL THEN dm_nama_id_debitur
                        ELSE dm_nama_id_debitur + ' - ' + dm_nama_badan_usaha
                    END AS nama_sesuai_identitas_nama_badan_usaha,
                    CASE 
                        WHEN dm_jenis_badan_usaha = '' OR dm_jenis_badan_usaha IS NULL THEN dm_jenis_debitur
                        WHEN dm_jenis_debitur = '' OR dm_jenis_debitur IS NULL THEN dm_jenis_badan_usaha
                        ELSE dm_jenis_debitur + ' - ' + dm_jenis_badan_usaha
                    END AS jenis_debitur_jenis_badan_usaha,
                    *
                FROM
                    dbo.da_anomali_nilai_njop_agunan x
                WHERE " + whereQuery + @"
            ";
            }


            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliPenghasilanPTQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string nilaiPenghasilan, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (nilaiPenghasilan != null)
            {
                nilaiPenghasilan = "'" + nilaiPenghasilan.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND (dm_penghasilan_kotor_per_tahun <= " + nilaiPenghasilan + " OR dm_penghasilan_kotor_per_tahun is null) ";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_penghasilan_per_tahun x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliNIKLahirDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_nik_lahir_debitur x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliFormatNPWPQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_format_npwp x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliTempatLahirDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_tempat_lahir_debitur x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliBakiDebetTWQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string segmens, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (segmens != null)
            {
                segmens = "'" + segmens.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_segmen, '-', '~') in (" + segmens + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_baki_debet_tidak_wajar x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliFormatTeleponDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_format_telepon_debitur x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliAlamatEmailDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_alamat_email_debitur x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliTempatBekerjaDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_tempat_bekerja_debitur x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliAlamatBekerjaDebQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_alamat_bekerja_debitur x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliTempatBUQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_tempat_badan_usaha x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliNomorAktaBUQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_nomor_akta_badan_usaha x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliFormatPeringkatAgunanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_jenis_debitur, '-', '~') in (" + jenisDebiturs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_format_peringkat_agunan x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliTingkatSukuBungaQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string sukuBungas, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (sukuBungas != null)
            {
                sukuBungas = sukuBungas.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'"); //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_suku_bunga_atau_imbalan " + sukuBungas;
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_bulan_data in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_tingkat_suku_bunga x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetDA_AnomaliBuktiKepemilikanAgunanQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            bool isC = false;

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (jenisDebiturs != null)
            {
                jenisDebiturs = "'" + jenisDebiturs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND REPLACE(dm_jenis_debitur, '-', '~') in (" + jenisDebiturs + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();

            props.Query = @"
            select * from dbo.da_anomali_bukti_kepemilikan_agunan x
                WHERE " + whereQuery + @"
            ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetLA_InquiryCheckQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string kolektibilitass, string statusPengecekans, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kolektibilitass != null)
            {
                kolektibilitass = "'" + kolektibilitass.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kolektibilitas in (" + kolektibilitass + ")";
            }
            if (statusPengecekans != null)
            {
                statusPengecekans = "'" + statusPengecekans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_status_pengecekan in (" + statusPengecekans + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }


            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                props.Query = @"
                SELECT dm_status_pengecekan, COUNT(*) AS dm_count
                FROM dbo.la_inquiry_check  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_status_pengecekan
            ";
            }
            else
            {
                props.Query = @"

                select * from dbo.la_inquiry_check x
                WHERE " + whereQuery + @"	                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetLA_InquiryPatternRecognitionQuery(DataEntities db, DataSourceLoadOptions loadOptions, string memberTypes, string members,
            string statusInquirys, string periode, bool isHive = false, bool isChart = false)
        {
            bool isC = false;
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                if (isHive == true)
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in  (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (statusInquirys != null)
            {
                statusInquirys = "'" + statusInquirys.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_status_inquiry in (" + statusInquirys + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }


            var props = new WSQueryProperties();

            //non chart ga ada group by, kalau sebaliknya ada
            if (isChart)
            {
                isC = true;
                props.Query = @"
                SELECT dm_status_inquiry, COUNT(*) AS dm_count
                FROM dbo.la_inquiry_pattern_recognition  x
                WHERE " + whereQuery + @"	 
                GROUP BY x.dm_status_inquiry
            ";
            }
            else
            {
                props.Query = @"

                select * from dbo.la_inquiry_pattern_recognition x
                WHERE " + whereQuery + @"	                
            ";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static string CekDataDA(DataEntities db, string periode, string reportID, bool isHive = false)
        {
            string rA = "";
            var props = new WSQueryProperties();
            periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
            if (reportID == "agunan_id_anomali" || reportID == "agunan_dokumen_anomali")
            {
                if (isHive == true)
                {
                    props.Query = @"
                    select * from dbo." + reportID + " where dm_periode in (" + periode + ") limit 1 ";
                }
                else
                {
                    props.Query = @"
                    select top 1 * from dbo." + reportID + " where dm_periode in (" + periode + ") ";
                }
                var res = WSQueryHelper.DoQueryNL(db, props, false, isHive);
                if (res.totalCount > 0)
                {
                    rA = "Data Tersedia";
                }
                else
                {
                    rA = "Tidak Ada Data Anomali";
                }
            }
            else
            //if (reportID == "debitur_anomali_format_ktp" || reportID == "debitur_anomali_gender" || reportID == "debitur_anomali_duplikasi_nama_debitur")
            {
                if (isHive == true)
                {
                    props.Query = @"
                    select * from dbo.debitur_anomali_data_populasi where dm_periode in (" + periode + ") limit 1 ";
                }
                else
                {
                    props.Query = @"
                    select top 1 * from dbo.debitur_anomali_data_populasi where dm_periode in (" + periode + ")  ";
                }
                var res = WSQueryHelper.DoQueryNL(db, props, false, isHive);
                if (res.totalCount > 0)
                {
                    if (isHive == true)
                    {
                        props.Query = @"
                        select * from dbo." + reportID + " where dm_periode in (" + periode + ") limit 1 ";
                    }
                    else
                    {
                        props.Query = @"
                        select top 1 * from dbo." + reportID + " where dm_periode in (" + periode + ")  ";
                    }
                    res = WSQueryHelper.DoQueryNL(db, props, false, isHive);
                    if (res.totalCount > 0)
                    {
                        rA = "Data Tersedia";
                    }
                    else
                    {
                        rA = "Tidak Ada Data Anomali";
                    }
                }
                else
                {
                    rA = "Data tidak tersedia";
                }

            }
            //else
            //{
            //    if (isHive == true)
            //    {
            //        props.Query = @"
            //        select * from dbo." + reportID + " where dm_bulan_data in (" + periode + ") limit 1 ";
            //    }
            //    else
            //    {
            //        props.Query = @"
            //        select top 1 * from dbo." + reportID + " where dm_bulan_data in (" + periode + ") ";
            //    }
            //    var res = WSQueryHelper.DoQueryNL(db, props, false, isHive);
            //    if (res.totalCount > 0)
            //    {
            //        rA = "Data Tersedia";
            //    }
            //    else
            //    {
            //        rA = "Tidak Ada Data Anomali";
            //    }
            //}




            return rA;

        }

        public static WSQueryReturns GetOsida2023Query(DataEntities db, DataSourceLoadOptions loadOptions, string tableName,
            string memberTypes, string members, string kantorCabangs, string periodes, bool chk100 = false, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;
            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }

            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periodes + ")";
            }
            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
                    props.Query = @"
                        select  concat_ws('~', cast(dm_periode as string), regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk,'',dm_cif) AS lem,concat_ws('~','n', cast(dm_periode as string), regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk,'',dm_cif) AS idcif,concat_ws('~','p', cast(dm_periode as string), regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk,'',dm_id_pengurus_pemilik) AS idpeng,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    if (chk100 == true)
                    {
                        props.Query = props.Query + @" order by dm_total_baki_debet desc 
                                      limit 100";
                    }
                }
                else
                {
                    if (tableName == "osida_potensi_konversi_kur_deb_noneligible_mst" || tableName == "osida_pemberian_kur_deb_noneligible_mst")
                    {
                        props.Query = @"
                        select  concat_ws('~', cast(dm_periode as string), regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk,dm_nik) AS lem,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                        if (chk100 == true)
                        {
                            if (tableName == "osida_potensi_konversi_kur_deb_noneligible_mst")
                            {
                                props.Query = props.Query + @" order by dm_total_baki_debet_kredit_baru desc 
                                      limit 100";
                            }
                            else
                            {
                                props.Query = props.Query + @" order by dm_baki_debet_kredit_baru desc 
                                      limit 100";
                            }

                        }
                    }
                    else
                    {
                        props.Query = @"
                        select  concat_ws('~', cast(dm_periode as string), regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk,dm_kode_kantor_cabang,dm_cif) AS lem,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }

                }

            }
            else
            {
                if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
                    if (chk100 == true)
                    {
                        props.Query = @"
                        select top 100 CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_cif AS lem,'n' + '~' + CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_cif  AS idcif,'p' + '~' + CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_id_pengurus_pemilik  AS idpeng,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        order by  dm_total_baki_debet desc ";
                    }
                    else
                    {
                        props.Query = @"
                        select CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_cif AS lem,'n' + '~' + CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_cif  AS idcif,'p' + '~' + CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_id_pengurus_pemilik  AS idpeng,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }
                    //props.Query = @"
                    //    select CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_cif AS lem,'n' + '~' + CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_cif  AS idcif,'p' + '~' + CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + '~' + dm_id_pengurus_pemilik  AS idpeng,* from dbo." + tableName + @" x
                    //    WHERE " + whereQuery + @"	                
                    //    ";
                }
                else
                {
                    if (tableName == "osida_potensi_konversi_kur_deb_noneligible_mst" || tableName == "osida_pemberian_kur_deb_noneligible_mst")
                    {
                        if (chk100 == true)
                        {
                            props.Query = @"
                            select top 100 CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + dm_nik AS lem,* from dbo." + tableName + @" x
                            WHERE " + whereQuery + @"	                
                            ";
                            if (tableName == "osida_potensi_konversi_kur_deb_noneligible_mst")
                            {
                                props.Query = props.Query + @" order by dm_total_baki_debet_kredit_baru desc ";
                            }
                            else
                            {
                                props.Query = props.Query + @" order by dm_baki_debet_kredit_baru desc ";
                            }
                        }
                        else
                        {
                            props.Query = @"
                            select CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + dm_nik AS lem,* from dbo." + tableName + @" x
                            WHERE " + whereQuery + @"	                
                            ";
                        }
                        //props.Query = @"
                        //    select CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + dm_nik AS lem,* from dbo." + tableName + @" x
                        //    WHERE " + whereQuery + @"	                
                        //    ";
                    }
                    else
                    {
                        props.Query = @"
                        select  CAST(dm_periode AS VARCHAR(20))+'~' + REPLACE(dm_jenis_ljk,'/','>') + '~' +dm_kode_ljk + '~' + dm_kode_kantor_cabang + '~' + dm_cif AS lem,* from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
                    }

                }

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetOsida2023DetailQuery(DataEntities db, DataSourceLoadOptions loadOptions, string tableName,
            string memberTypes, string members, string kantorCabangs, string Cifs, string periodes, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;
            if (memberTypes != null)
            {
                if (tableName != "osida_potensi_konversi_kur_deb_noneligible_det" && tableName != "osida_pemberian_kur_deb_noneligible_det" && tableName != "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus")
                {
                    memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
                }
            }
            if (members != null)
            {
                if (tableName != "osida_potensi_konversi_kur_deb_noneligible_det" && tableName != "osida_pemberian_kur_deb_noneligible_det" && tableName != "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus")
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }

            if (kantorCabangs != null && kantorCabangs != "")
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periodes + ")";
            }
            if (Cifs != null)
            {
                Cifs = "'" + Cifs.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                if (tableName != "osida_potensi_konversi_kur_deb_noneligible_det" && tableName != "osida_pemberian_kur_deb_noneligible_det")
                {
                    if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus")
                    {
                        whereQuery = whereQuery += " AND dm_id_pengurus in (" + Cifs + ")";
                    }
                    else
                    {
                        whereQuery = whereQuery += " AND dm_cif in (" + Cifs + ")";
                    }

                }
                else
                {
                    whereQuery = whereQuery += " AND dm_no_id_debitur in (" + Cifs + ")";
                }
            }
            var props = new WSQueryProperties();
            props.Query = @"
                        select  * from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetMonitoringQuery(DataEntities db, DataSourceLoadOptions loadOptions,
             string periodes, string tipeChart, bool isChart = true)
        {
            bool isC = false;
            var whereQuery = "1=1";
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)
            var isHive = false;
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND bulan_laporan in (" + periodes + ")";
            }
            var props = new WSQueryProperties();
            if (!isChart)
            {
                isHive = IsPeriodInHive(db, "log_monitoring_bda_slik_det");
                props.Query = @"
                        select * from dbo." + "log_monitoring_bda_slik_det" + @" x
                        WHERE " + whereQuery + @"	                
                        ";
            }
            else
            {
                isC = true;
                isHive = IsPeriodInHive(db, "log_monitoring_bda_slik_sum");
                if (tipeChart == "TA" || tipeChart == "TB")
                {
                    props.Query = @"
                        SELECT segmentasi,SUM(total_account) AS total_account ,SUM(baki_debet) AS baki_debet FROM dbo.log_monitoring_bda_slik_sum" + @" x
                        WHERE segmentasi IN ('F02','F03','F04','F05','F06') AND " + whereQuery + @"	                
                        GROUP BY segmentasi";
                }
                if (tipeChart == "LTA")
                {
                    props.Query = @"
                        SELECT SUM(total_account) AS lta  FROM dbo.log_monitoring_bda_slik_sum
                        WHERE segmentasi ='F01' AND " + whereQuery + @"	                
                        ";
                }
                else if (tipeChart == "LTB")
                {
                    props.Query = @"
                        SELECT SUM(baki_debet) AS ltb   FROM dbo.log_monitoring_bda_slik_sum
                        WHERE segmentasi ='F01' AND " + whereQuery + @"	                
                        ";
                }

            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetScheduleQuery(DataEntities db, DataSourceLoadOptions loadOptions, string periodes)
        {
            bool isC = false;
            var whereQuery = "1=1";
            var isHive = false;
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND bulan_laporan in (" + periodes + ")";
            }
            var props = new WSQueryProperties();
            isHive = IsPeriodInHive(db, "log_monitoring_bda_slik_det");
            props.Query = @"
                        SELECT DISTINCT date_add(x.tgl, 1) as tgl ,x.cat,x.col  FROM (
                        SELECT tanggal_cetak AS tgl,'File Tersedia' AS cat,'1' as col FROM dbo.log_monitoring_bda_slik_det
                        WHERE selisih_waktu<2 AND " + whereQuery + @"
                        UNION
                        SELECT tanggal_cetak AS tgl,'File Tidak Tersedia' AS cat,'2' as col FROM dbo.log_monitoring_bda_slik_det
                        WHERE (selisih_waktu>1 AND tanggal_terima IS NULL) AND " + whereQuery + @"
                        UNION
                        SELECT tanggal_terima AS tgl,'File Diterima' AS cat,'3' as col FROM dbo.log_monitoring_bda_slik_det
                        WHERE (tanggal_terima IS NOT NULL AND selisih_waktu>1) AND " + whereQuery + @"
                        ) x" + @"  ";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetMonitoringQueryNL(DataEntities db,
             string periodes, string tipeChart, bool isChart = true)
        {
            bool isC = false;
            var whereQuery = "1=1";
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)
            var isHive = false;
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND bulan_laporan in (" + periodes + ")";
            }
            var props = new WSQueryProperties();
            if (!isChart)
            {
                isHive = IsPeriodInHive(db, "log_monitoring_bda_slik_det");
                props.Query = @"
                        select * from dbo." + "log_monitoring_bda_slik_det" + @" x
                        WHERE " + whereQuery + @"	                
                        ";
            }
            else
            {
                isC = true;
                isHive = IsPeriodInHive(db, "log_monitoring_bda_slik_sum");
                if (tipeChart == "TA" || tipeChart == "TB")
                {
                    props.Query = @"
                        SELECT segmentasi,SUM(total_account) AS total_account ,SUM(baki_debet) AS baki_debet FROM dbo.log_monitoring_bda_slik_sum" + @" x
                        WHERE segmentasi IN ('F02','F03','F04','F05','F06') AND " + whereQuery + @"	                
                        GROUP BY segmentasi";
                }
                if (tipeChart == "LTA" || tipeChart == "LTB")
                {
                    props.Query = @"
                        SELECT SUM(total_account) AS lta,SUM(baki_debet) AS ltb  FROM dbo.log_monitoring_bda_slik_sum
                        WHERE segmentasi ='F01' AND " + whereQuery + @"	                
                        ";
                }

            }

            return WSQueryHelper.DoQueryNL(db, props, isC, isHive);
        }

        public static DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();

            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQuery(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, string stringStatus, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-","") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }
            if (stringStatus != null)
            {
                stringStatus = "'" + stringStatus.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND status in (" + stringStatus + ")";
            }
            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_sum_cluster_mkbd")
                {
                    props.Query = @"
                    SELECT row_number() over(order by securitycompanycode) as no,* from (
                    SELECT calendardate,securitycompanycode,securitycompanyname,
                        cast(cast(simpanangiro as BIGINT)  as string) as simpanangiro,
                        cast(cast(depositolt3bulan as BIGINT)  as string) as depositolt3bulan,
                        cast(cast(depositogt3bulandijaminlps as BIGINT)  as string) as depositogt3bulandijaminlps,
                        cast(cast(uangjaminanlkp as BIGINT)  as string) as uangjaminanlkp,
                        cast(cast(kasdansetarakas as BIGINT)  as string) as kasdansetarakas,
                        cast(cast(mkbd as BIGINT)  as string) as mkbd,
                        cast(cast(mkbdminimum as BIGINT)  as string) as mkbdminimum,
                        cast(cast(mkbdminimum as BIGINT)  as string) as mkbdpermkbdminimum,
                        CASE 
                            when cast(cast(kasdansetarakas as BIGINT)  as string) < cast(cast(mkbdminimum as BIGINT)  as string) then 'Alert'
                            when cast(cast(kasdansetarakas as BIGINT)  as string) > cast(cast(mkbdminimum as BIGINT)  as string) then 'Normal'
                        END AS status,periode
                        From pasarmodal." + tableName + @") as x
                WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_sum_cluster_mkbd")
                {
                    props.Query = @"
                    SELECT row_number() over(order by securitycompanycode) as no,* from (
                    SELECT calendardate,securitycompanycode,securitycompanyname,simpanangiro,depositolt3bulan,depositogt3bulandijaminlps,uangjaminanlkp,kasdansetarakas,mkbd,mkbdminimum,mkbdpermkbdminimum,
                        CASE 
                            when kasdansetarakas < mkbdminimum then 'Alert'
                            when kasdansetarakas > mkbdminimum then 'Normal'
                        END AS status,periode
                        From pasarmodal." + tableName + @") as x
                    WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryGetChartClusterSearch(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, string stringStatus, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }
            if (stringStatus != null)
            {
                stringStatus = "'" + stringStatus.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND status in (" + stringStatus + ")";
            }
            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_sum_cluster_mkbd")
                {
                    props.Query = @"
                    SELECT status,COUNT(status) total from (
                    SELECT calendardate,securitycompanycode,securitycompanyname,
                        cast(cast(simpanangiro as BIGINT)  as string) as simpanangiro,
                        cast(cast(depositolt3bulan as BIGINT)  as string) as depositolt3bulan,
                        cast(cast(depositogt3bulandijaminlps as BIGINT)  as string) as depositogt3bulandijaminlps,
                        cast(cast(uangjaminanlkp as BIGINT)  as string) as uangjaminanlkp,
                        cast(cast(kasdansetarakas as BIGINT)  as string) as kasdansetarakas,
                        cast(cast(mkbd as BIGINT)  as string) as mkbd,
                        cast(cast(mkbdminimum as BIGINT)  as string) as mkbdminimum,
                        cast(cast(mkbdminimum as BIGINT)  as string) as mkbdpermkbdminimum,
                        CASE 
                            when cast(cast(kasdansetarakas as BIGINT)  as string) < cast(cast(mkbdminimum as BIGINT)  as string) then 'Alert'
                            when cast(cast(kasdansetarakas as BIGINT)  as string) > cast(cast(mkbdminimum as BIGINT)  as string) then 'Normal'
                        END AS status,periode
                        From pasarmodal." + tableName + @") as x
                    WHERE " + whereQuery + @" group by status";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_sum_cluster_mkbd")
                {
                    props.Query = @"
                    SELECT status,COUNT(status) total from (
                    SELECT calendardate,securitycompanycode,securitycompanyname,simpanangiro,depositolt3bulan,depositogt3bulandijaminlps,uangjaminanlkp,kasdansetarakas,mkbd,mkbdminimum,mkbdpermkbdminimum,
                        CASE 
                            when kasdansetarakas < mkbdminimum then 'Alert'
                            when kasdansetarakas > mkbdminimum then 'Normal'
                        END AS status,periode
                        From pasarmodal." + tableName + @") as x
                    WHERE " + whereQuery + @" group by status";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryGetChartClusterBarSearch(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, string stringStatus, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }
            if (stringStatus != null)
            {
                stringStatus = "'" + stringStatus.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND status in (" + stringStatus + ")";
            }
            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_sum_cluster_mkbd")
                {
                    props.Query = @"
                    SELECT cluster,COUNT(status) total,urut from (
                    SELECT * FROM (                    
                    SELECT calendardate,securitycompanycode,
                        CASE 
                            when cast(cast(kasdansetarakas as BIGINT)  as string) < cast(cast(mkbdminimum as BIGINT)  as string) then 'Alert'
                            when cast(cast(kasdansetarakas as BIGINT)  as string) > cast(cast(mkbdminimum as BIGINT)  as string) then 'Normal'
                        END AS status,cluster,
                        CASE 
                           WHEN cluster ='<100%'  then '1'
                           WHEN cluster ='100% s.d. <120%'  then '2'
                           WHEN cluster ='120% s.d. <200%'  then '3'
                           WHEN cluster ='200% s.d. <500%'  then '4'
                           WHEN cluster ='>=500%'  then '5'
                        END AS urut,periode
                    FROM pasarmodal." + tableName + @") as x  
                    WHERE " + whereQuery + @") AS t 						
                    GROUP BY urut,cluster";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_sum_cluster_mkbd")
                {
                    props.Query = @"
                    SELECT cluster,COUNT(status) total,urut from (
                    SELECT * FROM (                    
                    SELECT calendardate,securitycompanycode,
                    	CASE 
                    		WHEN kasdansetarakas < mkbdminimum then 'Alert'
                    		WHEN kasdansetarakas > mkbdminimum then 'Normal'
                    	END AS status,cluster,
                        CASE 
                           WHEN cluster ='<100%'  then '1'
                           WHEN cluster ='100% s.d. <120%'  then '2'
                           WHEN cluster ='120% s.d. <200%'  then '3'
                           WHEN cluster ='200% s.d. <500%'  then '4'
                           WHEN cluster ='>=500%'  then '5'
                        END AS urut,periode
                    FROM pasarmodal." + tableName + @") as x  
                    WHERE " + whereQuery + @") AS t 						
                    GROUP BY urut,cluster";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryDetail(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_bridging_detail")
                {
                    props.Query = @"
                        SELECT calendardate,securitycompanysk,securitycompanycode,securitycompanyname,f_level_1,f_level_2,
                            cast(cast(total_balance as BIGINT)  as string) as total_balance,
                            cast(cast(total_aset_lancar as BIGINT)  as string) as total_aset_lancar,
                            cast(cast(persentase as BIGINT)  as string) as persentase,
                            CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,flag,periode
                        FROM pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_bridging_detail")
                {
                    props.Query = @"
                        select * 
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryRincianPortofolio(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_portofolio_saham")
                {
                    props.Query = @"
                        SELECT row_number() over(order by securitycompanycode) as no,
                            calendardate,securitycompanysk,securitycompanycode,securitycompanyname,securitysk,securitycode,securitytypename,
                            affiliated,nominalsheet,acquisitionprice,fairmarketprice,
                            CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,gainperloss,
                            cast(cast(fairmarketvaluepertotalporto as BIGINT)  as string) as fairmarketvaluepertotalporto,entitygroup,marketvaluepercentage,
                            CASE WHEN cast(cast(liabilitiesrankingvalue as BIGINT) as string) is null then '0' else cast(cast(liabilitiesrankingvalue as BIGINT) as string) END as liabilitiesrankingvalue,periode 
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_portofolio_saham")
                {
                    props.Query = @"
                        select row_number() over(order by securitycompanycode) as no, * 
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryRincianPortofolioDetailSummary(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_portofolio_saham_sum")
                {
                    props.Query = @"
                        SELECT 
                            calendardate,securitycompanysk,securitycompanycode,securitycompanyname,mkbdvd510accountsk,mkbdvd510accountcode,mkbdvd510description,
                            CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,
                            CASE WHEN cast(cast(liabilitiesrankingvalue as BIGINT) as string) is null then '0' else cast(cast(liabilitiesrankingvalue as BIGINT) as string) END as liabilitiesrankingvalue,periode 
                            from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_portofolio_saham_sum")
                {
                    props.Query = @"
                        select * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryReksadana(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_reksa_dana")
                {
                    props.Query = @"
                        SELECT calendardate,securitycompanysk,securitycompanycode,securitycompanyname,mutualfundtypesk,mutualfundtypecode,mutualfundtypename,mutualfundname,isaffiliated,
                        CASE WHEN cast(cast(netassetvalueunit as BIGINT) as string) is null then '0' else cast(cast(netassetvalueunit as BIGINT) as string) END as netassetvalueunit,
                        CASE WHEN cast(cast(netassetvalueunit as BIGINT) as string) is null then '0' else cast(cast(netassetvalueunit as BIGINT) as string) END as netassetvaluemutualfund,
                        CASE WHEN cast(cast(mkbdlimitationvalue as BIGINT) as string) is null then '0' else cast(cast(mkbdlimitationvalue as BIGINT) as string) END as mkbdlimitationvalue,liabilitiesrankingcal,
                        CASE WHEN cast(cast(mkbdlimitationexcessvalue as BIGINT) as string) is null then '0' else cast(cast(mkbdlimitationexcessvalue as BIGINT) as string) END as mkbdlimitationexcessvalue,
                        periode 
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }

               
            }
            else
            {
                if (tableName == "pe_segmentation_det_reksa_dana")
                {
                    props.Query = @"
                        select * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryReksadanaDetailSummary(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_reksa_dana_sum")
                {
                    props.Query = @"
                        SELECT calendardate,securitycompanysk,securitycompanycode,securitycompanyname, mkbdvd510accountsk,mkbdvd510accountcode,
                        CASE WHEN cast(cast(mkbdvd510description as BIGINT) as string) is null then '' else cast(cast(mkbdvd510description as BIGINT) as string) END as mkbdvd510description,
                        CASE WHEN cast(cast(mkbdlimitationexcessvalue as BIGINT) as string) is null then '0' else cast(cast(mkbdlimitationexcessvalue as BIGINT) as string) END as mkbdlimitationexcessvalue
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_reksa_dana_sum")
                {
                    props.Query = @"
                        select * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryJaminanMargin(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_jaminan_margin")
                {
                    props.Query = @"
                        Select row_number() over(order by securitycompanycode) as no,calendardate,securitycompanysk,securitycompanycode,securitycompanyname,securitysk,securitycode,securityname,volume,price,
                            CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,periode 
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_jaminan_margin")
                {
                    props.Query = @"
                        Select row_number() over(order by securitycompanycode) as no, * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryJaminanMarginDetailSummary(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_jaminan_margin_sum")
                {
                    props.Query = @"
                        select calendardate,securitycompanysk,securitycompanycode,securitycompanyname,mkbdvd510accountsk,mkbdvd510accountcode,mkbdvd510description,
                            CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,periode 
                        from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_jaminan_margin_sum")
                {
                    props.Query = @"
                        select * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryReverseRepo(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_reverse_repo_new")
                {
                    props.Query = @"
                        SELECT  row_number() over(order by securitycompanycode) as no,calendardate,securitycompanysk,securitycompanycode,securitycompanyname,securitycode,sellername,buyingdate,sellingdate,
                            CASE WHEN cast(cast(buyingamount as BIGINT) as string) is null then '0' else cast(cast(buyingamount as BIGINT) as string) END as buyingamount,
                            CASE WHEN cast(cast(sellingamount as BIGINT) as string) is null then '0' else cast(cast(sellingamount as BIGINT) as string) END as sellingamount,
                            collateralsecuritycode,
                            CASE WHEN cast(cast(collateralamount as BIGINT) as string) is null then '0' else cast(cast(collateralamount as BIGINT) as string) END as collateralamount,
                            CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,
                            CASE WHEN cast(cast(liabilitiesrankingvalue as BIGINT) as string) is null then '0' else cast(cast(liabilitiesrankingvalue as BIGINT) as string) END as liabilitiesrankingvalue,
                            cast(cast(rasio as BIGINT)  as string) as rasio,periode 
                        FROM pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_reverse_repo_new")
                {
                    props.Query = @"
                        Select row_number() over(order by securitycompanycode) as no, * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSegmentationSummaryClusterMKBDQueryReverseRepoDetailSummary(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string periodes, string stringPE, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            //isHive = true;

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode in (" + periodes.Replace("-", "") + ")";
            }
            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanycode in (" + stringPE + ")";
            }

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "pe_segmentation_det_reverse_repo_sum_new")
                {
                    props.Query = @"
                        SELECT calendardate,securitycompanysk,securitycompanycode,securitycompanyname,mkbdvd510accountsk,mkbdvd510accountcode,mkbdvd510description,
                        CASE WHEN cast(cast(buyingamount as BIGINT) as string) is null then '0' else cast(cast(buyingamount as BIGINT) as string) END as buyingamount,
                        CASE WHEN cast(cast(sellingamount as BIGINT) as string) is null then '0' else cast(cast(sellingamount as BIGINT) as string) END as sellingamount,
                        CASE WHEN cast(cast(fairmarketvalue as BIGINT) as string) is null then '0' else cast(cast(fairmarketvalue as BIGINT) as string) END as fairmarketvalue,
                        CASE WHEN cast(cast(liabilitiesrankingvalue as BIGINT) as string) is null then '0' else cast(cast(liabilitiesrankingvalue as BIGINT) as string) END as liabilitiesrankingvalue,periode 
                        FROM pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "pe_segmentation_det_reverse_repo_sum_new")
                {
                    props.Query = @"
                        select * from pasarmodal." + tableName + @" x
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMNamaPE(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, bool isHive = false)
        {
            var filter = (dynamic)null;
            filter = loadOptions.Filter;
            if (loadOptions.Filter != null)
            {
                filter = filter[0][2];
                filter = "AND exchangemembername LIKE '%" + filter + "%'";
            }
            else
            {
                filter = "";
            }
            bool isC = false;
            var whereQuery = "1=1 AND currentstatus='A' " + filter + "";

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                if (tableName == "dim_exchange_members")
                {
                    props.Query = @"
                        SELECT exchangemembercode,exchangemembername,currentstatus from pasarmodal." + tableName + @"
                        WHERE " + whereQuery + @"";
                }
            }
            else
            {
                if (tableName == "dim_exchange_members")
                {
                    props.Query = @"
                        SELECT exchangemembercode,exchangemembername,currentstatus from pasarmodal." + tableName + @"
                        WHERE " + whereQuery + @"";
                }
            }

            return WSQueryHelper.DoQueryNL(db, props, isC, isHive);
        }
        public static WSQueryReturns GetBDAPMSID(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, bool isHive = false)
        {
            bool isC = false;

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"
                        SELECT sid, nama_sid from pasarmodal.src_sid WHERE nama_sid like '%BIO%' AND is_active=1 LIMIT 100";
            }
            else
            {
                props.Query = @"
                        SELECT sid, nama_sid from pasarmodal.ip_sid";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetNamaSIDQuery(DataEntities db, DataSourceLoadOptions loadOptions, string namaSID, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "";
            //isHive = false;
            if (namaSID != null)
            {
                namaSID = namaSID.Replace("'", "").Replace(",", "','").Replace("' ", "'"); //cegah sql inject dikit
                namaSID = "'%" + namaSID.ToUpper() + "%'";
                whereQuery = "REPLACE(REPLACE(REPLACE(REPLACE(CONCAT(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(SUBSTRING(nama_sid, 1, 1), 0,'M'), 1,'R'), 2,'S'), 3,'D'), 4,'N'), 5,'F'), 6,'H'), 7,'Y'), 8,'T'), 9,'J'), SUBSTRING(nama_sid, 2, " + (isHive ? "LENGTH" : "LEN") + "(nama_sid))), 'IOII','O') , 'IOOI', 'A'), 'IIOO', 'U'),'IOIO' , 'E') LIKE " + namaSID + " ";

            }
            var props = new WSQueryProperties();
            props.Query = @"SELECT top 20 nama_sid, sid, len(nama_sid) len_nama FROM pasarmodal.master_sid x WHERE " + whereQuery + @" ORDER BY len_nama asc";
            if (isHive)
                props.Query = @"SELECT nama_sid, sid, length(nama_sid) len_nama FROM pasarmodal.src_sid x WHERE " + whereQuery + @" ORDER BY len_nama asc LIMIT 20";
            return DecryptResultsNamaSID(WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive));
        }

        public static WSQueryReturns GetPMIPQuery(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string SID, string tradeId, string namaSID, string nomorKTP, string nomorNPWP, string sistem, string businessReg, string startPeriod, string endPeriod, bool chk100 = false, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            var periodWhereQuery = "";
            //isHive = false;

            if (sistem != null) whereQuery = whereQuery += " AND UPPER(system) = '" + sistem.ToUpper() + "' ";

            if (SID != null) whereQuery = whereQuery += " AND sid = '" + EncryptSID(SID) + "' ";
            else if (tradeId != null) whereQuery = whereQuery += " AND SUBSTRING(sid, 7, 6) = '" + tradeId + "' ";
            else if (nomorKTP != null) whereQuery = whereQuery += " AND ktp = '" + nomorKTP + "' ";
            else if (nomorNPWP != null) whereQuery = whereQuery += " AND npwp = '" + nomorNPWP + "' ";
            //else if (namaSID != null) whereQuery = whereQuery += " AND nama_sid = " + EncryptName(namaSID);
            else if (businessReg != null) whereQuery = whereQuery += " AND business_registration_number = '" + businessReg + "' ";

            if (endPeriod != null)
            {
                string startperiodes = "'" + startPeriod.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                string endperiodes = "'" + endPeriod.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                periodWhereQuery = " AND periode between " + startperiodes + " and " + endperiodes;
            }
            else if (startPeriod != null)
            {
                string periodes = "'" + startPeriod.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                periodWhereQuery = " AND " + periodes + " between valid_from and valid_until"; //date_format(CURRENT_DATE(), 'YYYYMMdd')
            }

            string sqlGetQuery = "select table_" + (isHive ? "hive" : "sql") + @" as queryString from dbo.ref_query where table_id = '" + tableName + "'";
            //var propsQuery = new WSQueryProperties();
            //propsQuery.Query = sqlGetQuery;
            //DataRow dr = WSQueryHelper.DoQuery(db, propsQuery, loadOptions, false, false).data.Rows[0];

            DataRow dr = ExecuteSimpleSQL(db.appSettings.DataConnString, sqlGetQuery).data.Rows[0];
            string queryString = dr["queryString"].ToString();
            queryString = queryString.Replace("@wherefilter", whereQuery).Replace("@whereperiode", periodWhereQuery);

            var props = new WSQueryProperties();
            props.Query = queryString;



            #region OldQuery
            /*
            if (tableName == "ip_sid")
            {


                props.Query += @" SELECT 
                        CONCAT(CAST(valid_until AS VARCHAR(20)), '~', system, '~') AS lem, * from pasarmodal." + (isHive ? "src_sid" : tableName) + @" x WHERE " + whereQuery + periodWhereQuery;
                //props.Query += (chk100 == true) ? @" order by x.periode" : @"";
            }
            else if (tableName == "ip_transaction")
            {
                props.Query += @"
                        SELECT CONCAT(CAST(y.valid_until AS VARCHAR(20)), '~', y.system, '~') AS lem, trade_id, y.*, securitycode, x.buy_value, x.buy_quantity, x.buy_freq, 
                        x.sell_value, x.sell_quantity, x.sell_freq, 
                        (x.buy_value - x.sell_value) as net_value, 
                        (x.buy_quantity - x.sell_quantity) as net_quantity,
                        (x.buy_freq - x.sell_freq) as net_freq
                        FROM (
                            select sid, trade_id, securitycode, ";

                if (isHive)
                    props.Query += @"
                            IF( transactiontypecode = 'B', value, 0)  AS buy_value, 
                            IF( transactiontypecode = 'B', quantity, 0)  AS buy_quantity, 
                            IF( transactiontypecode = 'B', freq, 0)  AS buy_freq,
                            IF( transactiontypecode = 'S', value, 0)  AS sell_value, 
                            IF( transactiontypecode = 'S', quantity, 0)  AS sell_quantity, 
                            IF( transactiontypecode = 'S', freq, 0)  AS sell_freq ";
                else
                    props.Query += @"
                            buy_value, buy_quantity, buy_freq, sell_value, sell_quantity, sell_freq ";

                props.Query += @"
                            from pasarmodal." + (isHive ? "investor_profile_trans" : tableName) + @"
                            where " + whereQuery + periodWhereQuery 
                            //+ (isHive ? "group by sid, trade_id, securitycode" :"") 
                            + @" 
                        )x LEFT OUTER JOIN (
                            select valid_until, sid, nama_sid, tanggal_lahir, tanggal_pendirian, address1, ktp, npwp, status_sid, last_update_sid, system, email, phone_number, fax, passport, occupation, nationality, province, city, full_name, nama_rekening 
                            from pasarmodal." + (isHive ? "src_sid" : "ip_sid") + @" 
                            where " + whereQuery + @"
                        )y ON x.sid = y.sid";
            }
            else if (tableName == "ip_ownership")
            {
                props.Query += @"
                        SELECT CONCAT(CAST(y.valid_until AS VARCHAR(20)), '~', y.system, '~') AS lem, trade_id, y.*, 
                        securitycode, rekening_status, accountbalancestatuscode, volume, value 
                        FROM (
                            select sid, trade_id, securitycode, rekening_status, accountbalancestatuscode, volume, value  
                            from pasarmodal." + (isHive ? "investor_profile_kpm" : tableName) + @" WHERE " + whereQuery + periodWhereQuery + @"
                        )x LEFT OUTER JOIN (
                            select valid_until, sid, nama_sid, tanggal_lahir, tanggal_pendirian, address1, ktp, npwp, status_sid, last_update_sid, system, email, phone_number, fax, passport, occupation, nationality, province, city, full_name, nama_rekening 
                            from pasarmodal." + (isHive ? "src_sid" : "ip_sid") + @" 
                            where " + whereQuery + @"
                        )y ON x.sid = y.sid";
                //props.Query += (chk100 == true) ? @" order by x.periode" : @"";
            }
            else
            {
                props.Query += @" SELECT 
                        CAST(dm_periode AS VARCHAR(20)) + '~' + system + '~' + sid AS lem, * from pasarmodal." + tableName + @" x WHERE " + whereQuery;
                props.Query += (chk100 == true) ? @" order by x.is_direct" : @"";
            }
            */

            #endregion

            return DecryptResults(WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive));
        }

        public static WSQueryReturns GetPMMMQuery(DataEntities db, DataSourceLoadOptions loadOptions, string tableName, string startPeriod, string endPeriod, bool isHive = false)
        {
            bool isC = false;
            var whereQuery = "1=1";
            isHive = false;

            if (startPeriod != null)
            {
                string periodes = "'" + startPeriod.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periodes + ")";
            }
            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"";

            }
            else
            {
                if (tableName == "mm_spv")
                {
                    props.Query += @"
                        CAST(dm_periode AS VARCHAR(20)) + '~' + sid AS lem, * from pasarmodal." + tableName + @" x WHERE " + whereQuery;
                }


            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        #region query PS07, PS07B, PS07C

        public static string PS07Filters(string periode, string pe, string invType, string invOrigin, string inRange, string market)
        {
            string whereQuery = "";

            if (periode != null)
            {
                string periodes = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery += " AND pperiode in (" + periodes.Replace("-", "") + ")";
            }

            if (pe != null)
            {
                string namaPE = "'" + pe.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery += " AND exchangemembercode in (" + namaPE + ")";
            }

            if (invType != null)
            {
                string invt = "'" + invType.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery += " AND investor_type in (" + invt + ")";
            }

            if (invOrigin != null)
            {
                string invo = "'" + invOrigin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery += " AND investor_origin in (" + invo + ")";
            }
            if (inRange != null)
            {
                string range = "'" + inRange.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery += " AND inputrange in (" + range + ")";
            }

            if (market != null)
            {
                string mkt = "'" + market.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery += " AND market in (" + mkt + ")";
            }

            return whereQuery;
        }

        public static WSQueryReturns GetPS07TotalClientsQuery(DataEntities db, DataSourceLoadOptions loadOptions, string periode, string pe, string invType, string invOrigin, string inRange, string market, bool isHive = true)
        {
            bool isC = false;
            var whereQuery = "1=1";
            isHive = true;

            whereQuery += PS07Filters(periode, pe, invType, invOrigin, inRange, market);

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"
                SELECT COUNT(tradeid) AS total_clients FROM pasarmodal.basis_investor_pe WHERE " + whereQuery + @"";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetPS07ActiveClientQuery(DataEntities db, DataSourceLoadOptions loadOptions, string periode, string pe, string invType, string invOrigin, string inRange, string market, bool isHive = true)
        {
            bool isC = false;
            var whereQuery = "investortransactionfreq > 0";
            isHive = true;

            whereQuery += PS07Filters(periode, pe, invType, invOrigin, inRange, market);

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"
                SELECT COUNT(1) AS active_clients FROM pasarmodal.basis_investor_pe WHERE " + whereQuery + @"";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetPS07TrxFreqQuery(DataEntities db, DataSourceLoadOptions loadOptions, string periode, string pe, string invType, string invOrigin, string inRange, string market, bool isHive = true)
        {
            bool isC = false;
            var whereQuery = "1=1";
            isHive = true;

            whereQuery += PS07Filters(periode, pe, invType, invOrigin, inRange, market);

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = props.Query = @"
                SELECT SUM(investortransactionfreq) AS trx_freq FROM pasarmodal.basis_investor_pe WHERE " + whereQuery + @"";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetPS07TradedValueQuery(DataEntities db, DataSourceLoadOptions loadOptions, string periode, string pe, string invType, string invOrigin, string inRange, string market, bool isHive = true)
        {
            bool isC = false;
            var whereQuery = "1=1";
            isHive = true;

            whereQuery += PS07Filters(periode, pe, invType, invOrigin, inRange, market);

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"
                SELECT SUM(investortotalvalue) AS traded_value FROM pasarmodal.basis_investor_pe WHERE " + whereQuery + @"";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetPS07ClientLiquidAmtQuery(DataEntities db, DataSourceLoadOptions loadOptions, string periode, string pe, string invType, string invOrigin, string inRange, string market, bool isHive = true)
        {
            bool isC = false;
            var whereQuery = "1=1";
            isHive = true;

            whereQuery += PS07Filters(periode, pe, invType, invOrigin, inRange, market);

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"
                SELECT SUM(portofolio_amount) AS client_liquid_amt FROM pasarmodal.basis_investor_pe WHERE " + whereQuery + @"";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }

        public static WSQueryReturns GetPS07Segments(DataEntities db, DataSourceLoadOptions loadOptions, string periode, string pe, string invType, string invOrigin, string inRange, string market, string segment, bool isHive = true)
        {
            bool isC = false;
            var whereQuery = "basis_investor_1 = " + "'" + segment + "'";
            isHive = true;

            whereQuery += PS07Filters(periode, pe, invType, invOrigin, inRange, market);

            var props = new WSQueryProperties();
            if (isHive == true)
            {
                props.Query = @"SELECT SUM(investortransactionfreq) AS intrxfreq, COUNT(tradeid) AS ttlcli FROM pasarmodal.basis_investor_pe WHERE " + whereQuery + @"";
            }

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, isHive);
        }


        #endregion
    }
}
