using BDA.DataModel;
using BDA.Helper.FW;
using DevExpress.Data.Extensions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraRichEdit;
using DevExtreme.AspNet.Mvc;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace BDA.Helper
{
    public class WSQueryPS
    {
        #region Encrypt Decrypt PM
        private static string[] keySID = ["MFF", "FDD", "FDF", "CPD", "SCD", "IDD", "PFD", "OTF", "OTD", "IBD", "SCF", "IDF", "CPF", "MFD", "IBF", "PFF", "ISD", "ISF"];
        private static char[] keyName = ['M', 'R', 'S', 'D', 'N', 'F', 'H', 'Y', 'T', 'J'];
        private static char[] keyOne = ['X', 'A', 'Y', 'B', 'Z', 'C', 'L', 'R', 'H', 'S'];
        private static char[] keyTwo = ['D', 'N', 'J', 'P', 'E', 'I', 'O', 'K', 'Q', 'M'];

        public static WSQueryReturns DecryptResults(WSQueryReturns wqr)
        {
            DataTable dt = wqr.data;

            foreach(DataRow dr in dt.Rows)
            {
                dr["sid"] = DecryptSID(dr["sid"].ToString());
                dr["nama_sid"] = DecryptName(dr["nama_sid"].ToString());
                dr["full_name"] = DecryptName(dr["full_name"].ToString());
                dr["email"] = DecryptName(dr["email"].ToString());
                dr["nama_rekening"] = DecryptName(dr["nama_rekening"].ToString());
                dr["phone_number"] = DecryptNumber(dr["phone_number"].ToString());
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
            return (Int32.TryParse(s.Substring(0, 2), out val) ? keySID[(val-11)] : s.Substring(0, 2)) + s.Substring(2);
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
            return (Int32.TryParse(right.Substring(0,1), out val) ? keyOne[val] : right[0]) + (Int32.TryParse(right.Substring(1, 1), out val) ? keyTwo[val] : right[1])
                + s.Substring(0, s.Length - 2);
        }

        private static string EncryptSID(string s)
        {
            if (s.Length < 3) return s;
            int idx = Array.IndexOf(keySID, s.Substring(0, 3));
            if (idx >= 0) return (idx+11).ToString() + s.Substring(3);
            return s;
        }

        private static string EncryptName(string s)
        {
            if (s.Length < 2) return s;
            int idx = Array.IndexOf(keyName, s.Substring(0, 1));
            return ((idx >= 0 ? idx.ToString(): s.Substring(0, 1)) + s.Substring(1)).Replace("O", "IOII").Replace("A", "IOOI").Replace("U", "IIOO").Replace("E", "IOIO");            
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

        public static WSQueryReturns GetBDAPMMendeteksiHutangSubordinasiHSO(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";
            
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

            props.Query = @"
                    select securitycompanyname as NamaPE, calendardate as TanggalHSOT0, cast(mkbdaccountbalance as bigint) as HSO from pasarmodal.pe_segmentation_hutang_subordinasi_det
                    WHERE " + whereQuery;

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMMendeteksiHutangSubordinasiReverseRepo(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

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

            props.Query = @"
                    select sellername as LawanTransaksi, buyingdate as BuyingData, sum(cast(buyingamount as bigint)) as ReverseRepo from pasarmodal.pe_segmentation_hutang_subordinasi_det
                    WHERE " + whereQuery + @" group by sellername, buyingdate";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMPembiayaanVSJaminanSahamER(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

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

            props.Query = @"
                    select sellername as LawanTransaksi, buyingdate as BuyingData, sum(cast(buyingamount as bigint)) as ReverseRepo from pasarmodal.pe_segmentation_hutang_subordinasi_det
                    WHERE " + whereQuery + @" group by sellername, buyingdate";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMPembiayaanVSJaminanSahamJM(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

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

            props.Query = @"
                    select sellername as LawanTransaksi, buyingdate as BuyingData, sum(cast(buyingamount as bigint)) as ReverseRepo from pasarmodal.pe_segmentation_hutang_subordinasi_det
                    WHERE " + whereQuery + @" group by sellername, buyingdate";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

    }
}
