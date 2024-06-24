using BDA.DataModel;
using BDA.Helper.FW;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace BDA.Helper
{
    public class WSQueryExport
    {
        public static string GetOsidaQuery(DataEntities db, string tableName,
            string memberTypes, string members, DateTime periode)
        {
            var whereQuery = "1=1";
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)
            List<DateTime> lp = new List<DateTime>();
            lp.Add(periode);
            var isHive = BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName);
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
            if (periode != null)
            {
                string p;
                if (isHive == true)
                {
                    p = string.Format("{0:yyyyMM}", periode);
                }
                else
                {
                    p = string.Join(", ", string.Format("{0:yyyy-MM-dd}", periode));
                }
                p = "'" + p.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + p + ")";
            }
            var props = new WSQueryProperties();
            if (tableName == "osida_plafondering_umum_master")
            {
                if (isHive == true)
                {
                    props.Query = @"
                        select  concat_ws('~', dm_periode, regexp_replace(dm_jenis_ljk,'/','>'), dm_kode_ljk) AS lem,* from dbo." + tableName + @" x
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
            return props.Query;
            //return WSQueryHelper.DoQuery(db, props, loadOptions,BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName, lp));
            //return props.Query;
        }

        public static string GetMA_AMLQuery(DataEntities db, string memberTypes,
            string members, string jenisAgunans, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.ma_aml_cft_analysis x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }


        public static string GetMA_ONAQuery(DataEntities db, string memberTypes,
            string members, string jenisAgunans, string lokasiAgunans, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.ma_outstanding_macet_no_agunan x
                WHERE " + whereQuery + @"	                
                ";
            //non chart ga ada group by, kalau sebaliknya ada
            return props.Query;
        }

        public static string GetMA_AnomaliPekerjaanQuery(DataEntities db, string memberTypes, string members,
            string jenisPenggunaans, string pekerjaans, string periode, bool isHive = false)
        {
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

            props.Query = @"
            select * from dbo.ma_anomali_plafon_by_pekerjaan_debitur x
            WHERE " + whereQuery + @"	                
            ";

            return props.Query;
        }

        public static string GetMA_RekBaruNPLQuery(DataEntities db, string memberTypes, string members,
            string jenisPinjamans, string npls, string periode, bool isHive = false)
        {
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
            props.Query = @"
            select * from dbo.ma_rekening_baru_non_restrukturisasi_npl x
            WHERE " + whereQuery + @"	                
            ";
            return props.Query;
        }

        public static string GetMA_OutstandingMacetQuery(DataEntities db, string memberTypes, string members,
            string jenisAgunans, string lokasiAgunans, string periode, bool isHive = false)
        {
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

            props.Query = @"
                select * from dbo.ma_outstanding_macet_no_agunan x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetMS_APSukuBungaQuery(DataEntities db, string memberTypes,
            string members, string stbs, string periode, bool isHive = false)
        {
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
            props.Query = @"
                SELECT * FROM dbo.ms_perubahan_suku_bunga_no_restrukturisasi x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }
        public static string GetMS_KetentuanKolektibilitas(DataEntities db, string memberTypes,
            string members, string ksks, string periode, bool isHive = false)
        {
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
            props.Query = @"
                SELECT  * FROM dbo.ms_ketentuan_kolektibilitas x
                WHERE " + whereQuery + @"	                
                ";


            return props.Query;
        }
        public static string GetMS_PinjamanBaruBersamaan(DataEntities db, string memberTypes,
            string members, string kolektibilitass, string periode, bool isHive = false)
        {

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
            }

            if (kolektibilitass != null)
            {
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
            if (isHive == true)
            {
                props.Query = @"
                SELECT concat_ws('~',dm_periode,dm_jenis_ljk,dm_kode_ljk_filter,dm_kolektibilitas) AS lem,* FROM dbo.ms_pinjaman_baru_bersamaan_sum x
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


            return props.Query;
        }

        public static string GetMS_PinjamanBaruBersamaanDetail(DataEntities db, string memberTypes,
            string members, string kolektibilitass, string periode, bool isHive = false)
        {
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

            props.Query = @"
            SELECT * FROM dbo.ms_pinjaman_baru_bersamaan_det x
            WHERE " + whereQuery + @"	                
            ";

            return props.Query;
        }
        public static string GetMS_KolektibilitasKaryawanLJK(DataEntities db, string memberTypes,
            string members, string kolektibilitas, string periode, bool isHive = false)
        {

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
            props.Query = @"
                SELECT * FROM dbo.ms_kolektibilitas_karyawan_ljk x
                WHERE " + whereQuery + @"	                
                ";


            return props.Query;
        }
        public static string GetMS_PemberianPinjamanKaryawanLJK(DataEntities db, string memberTypes,
           string members, string jps, string periode, bool isHive = false)
        {
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
            props.Query = @"
                SELECT * FROM dbo.ms_pemberian_pinjaman_karyawan_ljk x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }
        public static string GetMacro_ForecastingQuery(DataEntities db, string memberTypes, string members,
            string tipeForecastings, DateTime periodeAwal, DateTime periodeAkhir)
        {
            var whereQuery = "1 = 1";
            var isHive = BDA.Helper.WSQueryStore.IsPeriodInHive(db, "macro_output_forecast_level_ljk");
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
            if (periodeAwal != null && periodeAkhir != null)
            {
                string pAwal, pAkhir;
                if (isHive == true)
                {
                    pAwal = string.Format("{0:yyyyMM}", periodeAwal);
                    pAkhir = string.Format("{0:yyyyMM}", periodeAkhir);
                }
                else
                {
                    pAwal = string.Join(", ", string.Format("{0:yyyy-MM-dd}", periodeAwal));
                    pAkhir = string.Join(", ", string.Format("{0:yyyy-MM-dd}", periodeAkhir));
                }
                pAwal = "'" + pAwal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                pAkhir = "'" + pAkhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode >= " + pAwal + " AND dm_pperiode <= " + pAkhir + " ";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                select * from dbo.macro_output_forecast_level_ljk x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }
        public static string GetMacro_AMPertumbuhanQuery(DataEntities db, string memberTypes, string members,
           string jenisPertumbuhans, DateTime periodeAwal, DateTime periodeAkhir)
        {
            var whereQuery = "1 = 1";
            //var whereQuery2 = "dm_periode <= @periodeAkhir";
            var isHive = BDA.Helper.WSQueryStore.IsPeriodInHive(db, "macro_pertumbuhan_pinjaman_level_ljk");
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
            if (periodeAwal != null && periodeAkhir != null)
            {
                string pAwal, pAkhir;
                if (isHive == true)
                {
                    pAwal = string.Format("{0:yyyyMM}", periodeAwal);
                    pAkhir = string.Format("{0:yyyyMM}", periodeAkhir);
                }
                else
                {
                    pAwal = string.Join(", ", string.Format("{0:yyyy-MM-dd}", periodeAwal));
                    pAkhir = string.Join(", ", string.Format("{0:yyyy-MM-dd}", periodeAkhir));
                }
                pAwal = "'" + pAwal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                pAkhir = "'" + pAkhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode >= " + pAwal + " AND dm_pperiode <= " + pAkhir + " ";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                    select * from dbo.macro_pertumbuhan_pinjaman_level_ljk x
                    WHERE " + whereQuery + @"	                
                ";
            //non chart ga ada group by, kalau sebaliknya ada

            return props.Query;
        }

        public static string GetMacro_AMPenetrasiQuery(DataEntities db, string memberTypes, string members,
            string jenisDebiturs, string kategori, string deskripsiKategoris, string periode, bool isHive = false)
        {
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
            props.Query = @"

                select * from dbo.macro_penetrasi_lending_ljk x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetMicro_LiquidityRiskQuery(DataEntities db, string memberTypes, string members,
            string kelasPlafons, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.micro_plafond_usability_acc_detail x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetMacro_PolicyEAQuery(DataEntities db, string memberTypes, string members,
            string jenisDebiturs, string kolektibilitas, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.macro_policy_evaluation_analysis x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetMA_KKLQuery(DataEntities db, string memberTypes, string members,
            string kodeJenisPinjamans, string kolektibilitass, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.ma_kolektibilitas_kesalahan_ljk x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetMicro_CreditRiskQuery(DataEntities db, string memberTypes, string members,
            string kolektabilitass, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.micro_credit_risk_analysis x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetMA_APIQuery(DataEntities db, string memberTypes, string members,
            string kategoriAsesmens, string similarityScores, string similarityResults, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.ma_analisis_pencurian_identitas x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetDA_AnomaliFormatKTPQuery(DataEntities db, string memberTypes, string members,
            string periode, bool isHive = false)
        {
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
                    whereQuery = whereQuery += " AND regexp_extract(x.dm_kode_ljk, '^(.*?)( - )') in (" + members + ")";
                }
                else
                {
                    members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                    whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
                }
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                select * from dbo.debitur_anomali_format_ktp x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetDA_AnomaliGenderQuery(DataEntities db, string memberTypes, string members,
            string periode, bool isHive = false)
        {
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
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                select * from dbo.debitur_anomali_gender x
                WHERE " + whereQuery + @"
                ";

            return props.Query;
        }

        public static string GetDA_AnomaliDuplikasiNamaQuery(DataEntities db, string memberTypes, string members,
            string periode, bool isHive = false)
        {
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
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                select * from dbo.debitur_anomali_duplikasi_nama_debitur x
                WHERE x.dm_similarity_result = 0 AND " + whereQuery + @"             
                ";

            return props.Query;
        }

        public static string GetDA_AnomaliIDAgunanQuery(DataEntities db, string memberTypes, string members,
            string jenisAgunans, string similarityScores, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.agunan_id_anomali x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetDA_AnomaliDokumenAgunanQuery(DataEntities db, string memberTypes, string members,
            string jenisAgunans, string ownershipDocuments, string periode, bool isHive = false)
        {
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
                whereQuery = whereQuery += " AND dm_dokumen_kepemilikan_agunan in (" + ownershipDocuments + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                select * from dbo.agunan_dokumen_anomali x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetDA_AnomaliNilaiAgunanDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string nilaiAgunan, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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
                whereQuery = whereQuery += " AND dm_baki_debet_nominal <= " + nilaiAgunan;
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

            return props.Query;
        }

        public static string GetDA_AnomaliNIKDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliGelarNamaDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliAlamatDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string segmens, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnalisisDebtorNomIdentitasSamaQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnalisisDebtorNomIdentitasBedaQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliNamaIbuKandungQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliBentukBadanUsahaQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliNilaiNJOPAgunanQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string nilaiNJOP, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            props.Query = @"
            select * from dbo.da_anomali_nilai_njop_agunan x
                WHERE " + whereQuery + @"
            ";

            return props.Query;
        }

        public static string GetDA_AnomaliPenghasilanPTQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string nilaiPenghasilan, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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
                whereQuery = whereQuery += " AND (dm_penghasilan_kotor_per_tahun <= " + nilaiPenghasilan + " OR dm_penghasilan_kotor_per_tahun is null ) ";
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

            return props.Query;
        }

        public static string GetDA_AnomaliNIKLahirDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliFormatNPWPQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliTempatLahirDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliBakiDebetTWQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string segmens, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliFormatTeleponDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliAlamatEmailDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliTempatBekerjaDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliAlamatBekerjaDebQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliTempatBUQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliNomorAktaBUQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliFormatPeringkatAgunanQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetDA_AnomaliTingkatSukuBungaQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string sukuBungas, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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
                whereQuery += " AND dm_suku_bunga_atau_imbalan " + sukuBungas;
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

            return props.Query;
        }

        public static string GetDA_AnomaliBuktiKepemilikanAgunanQuery(DataEntities db, string memberTypes, string members,
            string kantorCabangs, string jenisDebiturs, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

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

            return props.Query;
        }

        public static string GetLA_InquiryCheckQuery(DataEntities db, string memberTypes, string members,
            string kolektibilitass, string statusPengecekans, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.la_inquiry_check x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }

        public static string GetLA_InquiryPatternRecognitionQuery(DataEntities db, string memberTypes, string members,
            string statusInquirys, string periode, bool isHive = false)
        {
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
            props.Query = @"
                select * from dbo.la_inquiry_pattern_recognition x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }
        public static string GetOsintQuery(DataEntities db, string jns, string inq, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";

            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pcategory_scrapping in (" + jns + ")";
            }
            if (inq != null)
            {
                inq = "'%" + inq.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND (dm_keyword LIKE (" + inq + ") OR dm_judul_berita LIKE (" + inq + ")" + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode_scrapping in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            props.Query = @" SELECT * FROM dbo.osint_scrapping_filtered x
                    WHERE " + whereQuery + @"	                
                    ";
            return props.Query;
        }
        public static string GetOsintQueryDetail(DataEntities db, string jns, string inq, string periode)
        {
            var whereQuery = "1 = 1";

            if (jns != null)
            {
                jns = "'" + jns.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pcategory_scrapping in (" + jns + ")";
            }
            if (inq != null)
            {
                whereQuery = whereQuery += " AND dm_keyword = '" + inq + "'";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_pperiode_scrapping in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            props.Query = @"
                    SELECT * FROM dbo.osint_scrapping_filtered x
                    WHERE " + whereQuery + @"	                
                    ";

            return props.Query;
        }
        public static string GetCoverageMapQuery(DataEntities db, string memberTypes, string members, string caks, string provs, string kotas, string periode, bool isHive = false)
        {
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
            props.Query = @"
                SELECT * FROM dbo.coverage_map_ljkxcity x
                WHERE " + whereQuery + @"	                
                ";

            return props.Query;
        }
        public static string GetDNAQuery(DataEntities db, string jns, string jas, string members, string periode, bool isHive = false)
        {
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
                whereQuery = whereQuery += " AND dm_node_type LIKE (" + jns + ")";
            }
            if (jas != null)
            {
                jas = "'" + jas.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND regexp_extract(dm_node_id, '^(.*?)( ID )') in (" + jas + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND (regexp_extract(dm_node_id, 'member:(.*$)') IN (" + members + ") OR dm_node_id IN (" + members + "))";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

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
            if (ctAgunan == false && ctCif == false && ctDebitur == false && ctLJK == false)
            {
                if (isHive == true)
                {
                    query0 = @"
                        select  concat_ws('~',cast(dm_periode as string),dm_relationship,dm_first_node_id,dm_second_node_id) as lem,* from dbo.edge_collateral
                        where 1=1
                        ";
                }
                else
                {
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

            return props.Query;
        }
        public static string GetDNADetailQuery(DataEntities db, string rn, string fn, string sn, string periode, bool isHive = false)
        {
            var whereQuery = "1 = 1";
            if (rn != null)
            {
                whereQuery = whereQuery += " AND dm_relationship in (" + rn + ") ";
            }
            if (fn != null)
            {
                whereQuery = whereQuery += " AND dm_first_node_id in (" + fn + ") ";
            }
            if (sn != null)
            {
                whereQuery = whereQuery += " AND dm_second_node_id in (" + sn + ") ";
            }

            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }

            var props = new WSQueryProperties();

            props.Query = @"
                SELECT *  FROM dbo.edge_collateral x 
                WHERE " + whereQuery + @"
                ";

            return props.Query;
        }
        public static string GetUserMasterQuery(DataEntities db, bool isHive = false)
        {
            var whereQuery = "1 = 1";

            var props = new WSQueryProperties();

            props.Query = @"
                SELECT *  FROM dbo.UserMaster x 
                WHERE " + whereQuery + @"
                ";

            return props.Query;
        }
        public static string GetOsida2023Query(DataEntities db, string tableName, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var whereQuery = "1=1";
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)

            var isHive = BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName);
            //isHive = true;
            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_jenis_ljk in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND x.dm_kode_ljk in (" + members + ")";
            }
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                        select * from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
            return props.Query;
            //return WSQueryHelper.DoQuery(db, props, loadOptions,BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName, lp));
            //return props.Query;
        }
        public static string GetOsida2023DetailQuery(DataEntities db, string tableName, string memberTypes, string members, string kantorCabangs, string cifs, string periode)
        {
            var whereQuery = "1=1";
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)

            var isHive = BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName);
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
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
            }
            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            if (cifs != null)
            {
                cifs = "'" + cifs.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                if (tableName != "osida_potensi_konversi_kur_deb_noneligible_det" && tableName != "osida_pemberian_kur_deb_noneligible_det")
                {
                    if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus")
                    {
                        whereQuery = whereQuery += " AND dm_id_pengurus in (" + cifs + ")";
                    }
                    else
                    {
                        whereQuery = whereQuery += " AND dm_cif in (" + cifs + ")";
                    }

                }
                else
                {
                    whereQuery = whereQuery += " AND dm_no_id_debitur in (" + cifs + ")";
                }
            }
            var props = new WSQueryProperties();
            props.Query = @"
                        select * from dbo." + tableName + @" x
                        WHERE " + whereQuery + @"	                
                        ";
            return props.Query;
            //return WSQueryHelper.DoQuery(db, props, loadOptions,BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName, lp));
            //return props.Query;
        }
        public static string GetOsida2023Detail2Query(DataEntities db, string tableName, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var whereQuery = "1=1";
            var whereQuery2 = "1=1";
            var tableName2 = "";
            if (tableName == "osida_potensi_konversi_kur_deb_noneligible_mst")
            {
                tableName2 = "osida_potensi_konversi_kur_deb_noneligible_det";
            }
            else if (tableName == "osida_pemberian_kur_deb_noneligible_mst")
            {
                tableName2 = "osida_pemberian_kur_deb_noneligible_det";
            }
            else if (tableName == "osida_nik_tidak_konsisten_mst")
            {
                tableName2 = "osida_nik_tidak_konsisten_det";
            }
            else if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_mst")
            {
                tableName2 = "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus";
            }
            // concat_ws('~', dm_periode, dm_jenis_ljk, dm_kode_ljk_filter, dm_kolektibilitas)

            var isHive = BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName);
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
            if (periode != null)
            {
                periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
                whereQuery2 = whereQuery2 += " AND dm_periode in (" + periode + ")";
            }
            if (kantorCabangs != null)
            {
                kantorCabangs = "'" + kantorCabangs.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dm_kode_kantor_cabang in (" + kantorCabangs + ")";
            }
            var props = new WSQueryProperties();
            props.Query = @"
                        select * from dbo." + tableName2 + @" x
                        WHERE " + whereQuery2 + @" AND ";
            if (tableName != "osida_potensi_konversi_kur_deb_noneligible_mst" && tableName != "osida_pemberian_kur_deb_noneligible_mst")
            {
                if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
                    props.Query = props.Query + @" dm_id_pengurus in ( ";
                }
                else
                {
                    props.Query = props.Query + @" dm_cif in ( ";
                }

            }
            else
            {
                props.Query = props.Query + @" dm_no_id_debitur in ( ";
            }
            props.Query = props.Query + @"
                        select ";
            if (tableName != "osida_potensi_konversi_kur_deb_noneligible_mst" && tableName != "osida_pemberian_kur_deb_noneligible_mst")
            {
                if (tableName == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
                    props.Query = props.Query + @" dm_id_pengurus_pemilik  ";
                }
                else
                {
                    props.Query = props.Query + @" dm_cif ";
                }

            }
            else
            {
                props.Query = props.Query + @" dm_nik ";
            }
            props.Query = props.Query + @" from dbo." + tableName + @" x
                        WHERE " + whereQuery + @" )	                
                        ";
            return props.Query;
            //return WSQueryHelper.DoQuery(db, props, loadOptions,BDA.Helper.WSQueryStore.IsPeriodInHive(db, tableName, lp));
            //return props.Query;
        }
        public static string GetMaxMinOverdueQuery(DataEntities db, string memberTypes, string members,
            string jenisDebitur, string statusCollectability, string periodeAwal, string periodeAkhir, bool isHive = false)
        {
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.member_type_code in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.member_code in (" + members + ")";
            }
            if (jenisDebitur != null)
            {
                jenisDebitur = "'" + jenisDebitur.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.status in (" + jenisDebitur + ")";
            }
            if (statusCollectability != null)
            {
                statusCollectability = "'" + statusCollectability.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.collectibility_type_code in (" + statusCollectability + ")";
            }
            if (periodeAwal != null)
            {
                periodeAwal = "'" + periodeAwal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.periode >= " + periodeAwal;
            }
            if (periodeAkhir != null)
            {
                periodeAkhir = "'" + periodeAkhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.periode <= " + periodeAkhir;
            }
            var props = new WSQueryProperties();

            props.Query = @"
               SELECT 
                   periode,
                   b.deskripsi_jenis_ljk AS member_type_code,
                   member_code,
                   status,
                   cnt_distinct_cif,
                   cnt_acc,
                   min_overdue_days,
                   max_overdue_days,
	               collectibility_type_code,
	               sum_outstanding
	            FROM dbo.BDA_F01_MaxMinOverdue a
                LEFT JOIN dbo.master_ljk_type b ON a.member_type_code = b.kode_jenis_ljk
                WHERE " + whereQuery + @"
            ";

            return props.Query;
        }

        public static string GetClassificationPredictiveQuery(DataEntities db, string memberTypes, string members,
            string jenisDebitur, string statusCollectability, string dugaanCollectability, string periodeAwal, string periodeAkhir, bool isHive = false)
        {
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.member_type_code in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.member_code in (" + members + ")";
            }
            if (jenisDebitur != null)
            {
                jenisDebitur = "'" + jenisDebitur.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.status in (" + jenisDebitur + ")";
            }
            if (statusCollectability != null)
            {
                statusCollectability = "'" + statusCollectability.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.collectibility_type_code in (" + statusCollectability + ")";
            }
            if (dugaanCollectability != null)
            {
                dugaanCollectability = "'" + statusCollectability.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.dugaan_collectability in (" + dugaanCollectability + ")";
            }
            if (periodeAwal != null)
            {
                periodeAwal = "'" + periodeAwal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.periode >= " + periodeAwal;
            }
            if (periodeAkhir != null)
            {
                periodeAkhir = "'" + periodeAkhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.periode <= " + periodeAkhir;
            }
            var props = new WSQueryProperties();

            props.Query = @"
               SELECT 
                   periode,
                   b.deskripsi_jenis_ljk AS member_type_code,
                   member_code,
                   status,
                   cnt_distinct_cif,
                   cnt_acc,
                   min_overdue_days,
                   max_overdue_days,
	               collectibility_type_code,
	               dugaan_collectability,
                   sum_outstanding
	            FROM dbo.BDA_F01_Predictive a
				LEFT JOIN dbo.master_ljk_type b ON a.member_type_code = b.kode_jenis_ljk
                WHERE " + whereQuery + @"
            ";

            return props.Query;
        }

        public static string GetJarakAntarAktivitasQuery(DataEntities db, string memberTypes, string members,
            string status, string periodeAwal, string periodeAkhir, bool isHive = false)
        {
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.member_type_code in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.member_code in (" + members + ")";
            }
            if (status != null)
            {
                status = "'" + status.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND SUBSTRING(a.status_avg_activity,0,8) in (" + status + ")";
            }
            if (periodeAwal != null)
            {
                periodeAwal = "'" + periodeAwal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.periode >= " + periodeAwal;
            }
            if (periodeAkhir != null)
            {
                periodeAkhir = "'" + periodeAkhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND a.periode <= " + periodeAkhir;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                SELECT
	                a.periode,
	                a.member_code + ' - ' + c.nama_ljk AS LJK,
	                CAST(a.user_id AS REAL) AS user_id,
	                b.act_count,
	                a.mean_diff,
	                SUBSTRING(a.status_avg_activity,0,8) AS status 
                FROM dbo.BDA_LogOutliers_Avg a
                LEFT OUTER JOIN dbo.BDA_LogOutliers_Act b ON b.periode = a.periode AND b.member_type_code = a.member_type_code AND b.member_code = a.member_code AND b.user_id = a.user_id
                LEFT OUTER JOIN dbo.master_ljk c ON a.member_type_code = c.kode_jenis_ljk AND a.member_code = c.kode_ljk
                WHERE " + whereQuery + @"
            ";

            return props.Query;
        }

        public static string GetRasioMaxToAverageQuery(DataEntities db, string memberTypes, string members,
            string periodeAwal, string periodeAkhir, bool isHive = false)
        {
            var whereQuery = "1 = 1";

            if (memberTypes != null)
            {
                memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND lpm.member_type_code in (" + memberTypes + ")";
            }
            if (members != null)
            {
                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND lpm.member_code in (" + members + ")";
            }
            if (periodeAwal != null)
            {
                periodeAwal = "'" + periodeAwal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND lpm.periode >= " + periodeAwal;
            }
            if (periodeAkhir != null)
            {
                periodeAkhir = "'" + periodeAkhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND lpm.periode <= " + periodeAkhir;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                SELECT
                    lpm.periode,
                    lpm.member_type_code,
                    lpm.member_code,
                    lpm.user_id,
                    lpm.max_act_count,
                    lpm.min_act_count,
                    CAST(lpm.avg_act_count AS DECIMAL(34,2)) as avg_act_count,
                    CAST((CAST(lpm.max_act_count AS DECIMAL(34,2))/CAST(lpm.min_act_count AS DECIMAL(34,2))) AS DECIMAL(34,2)) AS max_to_min,
                    CAST((CAST(lpm.max_act_count AS DECIMAL(34,2))/CAST(lpm.avg_act_count AS DECIMAL(34,2))) AS DECIMAL(34,2)) AS max_to_avg
                FROM dbo.BDA_Log_PerMonth lpm
                WHERE " + whereQuery + @"
            ";

            return props.Query;
        }
    }
}
