using BDA.DataModel;
using BDA.Helper.FW;
using BDA.Models;
using DevExpress.Data.Extensions;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraRichEdit;
using DevExtreme.AspNet.Data;
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
using System.Text;
using System.Text.RegularExpressions;

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

            foreach (DataRow dr in dt.Rows)
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
                whereQuery = whereQuery += " AND periode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanysk = " + stringPE;
            }
            var props = new WSQueryProperties();

            props.Query = @"select securitycompanyname as namape, calendardate as tanggalhsot0, cast(mkbdaccountbalance as bigint) as hso, sellername as lawantransaksi, buyingdate as buyingdate, cast(buyingamount as bigint) as reverserepo from pasarmodal.pe_segmentation_hutang_subordinasi_det WHERE " + whereQuery;

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMMendeteksiHutangSubordinasiReverseRepo(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanysk = " + stringPE;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select sellername as lawantransaksi, buyingdate as buyingdate, sum(cast(buyingamount as bigint)) as reverserepo from pasarmodal.pe_segmentation_hutang_subordinasi_det
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
                whereQuery = whereQuery += " AND periode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanysk = " + stringPE;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select kodeefek, groupemiten, cast(sum(volume) as bigint) as volume, cast(sum(value) as bigint) as value from pasarmodal.pe_segmentation_pembiayaan_vs_jaminan_saham
                    WHERE " + whereQuery + @" AND flag = 1 group by kodeefek, groupemiten";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMPembiayaanVSJaminanSahamJM(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanysk = " + stringPE;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select kodeefek, groupemiten, cast(sum(volume) as bigint) as volume, cast(sum(value) as bigint) as value from pasarmodal.pe_segmentation_pembiayaan_vs_jaminan_saham
                    WHERE " + whereQuery + @" AND flag = 2 group by kodeefek, groupemiten";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMPendanaanMKBDPPE(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanysk = " + stringPE;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select UPPER(mkbdaccountname) as Akun, cast(mkbdaccountbalance as bigint) as Nilai From pasarmodal.pe_segmentation_pendanaan_per_mkbd 
                    WHERE " + whereQuery + "  and mkbdaccountname <> '' order by sort_no asc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMPendanaanMKBDSPPE(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND periode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND securitycompanysk = " + stringPE;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select UPPER(mkbdaccountname_mkbd) as Akun, cast(mkbdaccountbalance_mkbd as bigint) as Nilai From pasarmodal.pe_segmentation_pendanaan_per_mkbd 
                    WHERE " + whereQuery + "  and mkbdaccountname_mkbd <> '' order by sort_no asc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMDemografiInvestorCG(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string origin, string tipeInvestor)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (origin != null)
            {
                origin = "'" + origin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + origin;
            }

            if (tipeInvestor != null)
            {
                stringPE = "'" + tipeInvestor.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_type = " + tipeInvestor;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select jenis_kelamin gender, cast(sum(investortotalvalue) as bigint) as total from pasarmodal.basis_investor_pe
                    WHERE " + whereQuery + @" group by jenis_kelamin";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMDemografiInvestorCU(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string origin, string tipeInvestor)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (origin != null)
            {
                origin = "'" + origin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + origin;
            }

            if (tipeInvestor != null)
            {
                stringPE = "'" + tipeInvestor.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_type = " + tipeInvestor;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select golonganusia agerange, cast(sum(investortotalvalue) as bigint) as total from pasarmodal.basis_investor_pe
                    WHERE " + whereQuery + @" group by golonganusia order by count(*) desc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMDemografiInvestorCP(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string origin, string tipeInvestor)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (origin != null)
            {
                origin = "'" + origin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + origin;
            }

            if (tipeInvestor != null)
            {
                stringPE = "'" + tipeInvestor.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_type = " + tipeInvestor;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select pendidikan, cast(sum(investortotalvalue) as bigint) as total from pasarmodal.basis_investor_pe
                    WHERE " + whereQuery + @" group by pendidikan order by count(*) desc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMDemografiInvestorCPk(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string origin, string tipeInvestor)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (origin != null)
            {
                origin = "'" + origin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + origin;
            }

            if (tipeInvestor != null)
            {
                stringPE = "'" + tipeInvestor.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_type = " + tipeInvestor;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select pekerjaan occupation, cast(sum(investortotalvalue) as bigint) as total from pasarmodal.basis_investor_pe
                    WHERE " + whereQuery + @" group by pekerjaan order by count(*) desc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMDemografiInvestorCPh(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string origin, string tipeInvestor)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (origin != null)
            {
                origin = "'" + origin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + origin;
            }

            if (tipeInvestor != null)
            {
                stringPE = "'" + tipeInvestor.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_type = " + tipeInvestor;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select pinghasilan_individu name, cast(sum(investortotalvalue) as bigint) value from pasarmodal.basis_investor_pe
                    WHERE " + whereQuery + @" group by pinghasilan_individu order by count(*) desc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMDemografiInvestorTV(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string origin, string tipeInvestor)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (origin != null)
            {
                origin = "'" + origin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + origin;
            }

            if (tipeInvestor != null)
            {
                stringPE = "'" + tipeInvestor.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_type = " + tipeInvestor;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select cast(sum(investortotalvalue) as bigint) as total from pasarmodal.basis_investor_pe
                    WHERE " + whereQuery;

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPGeospasialInvestorMaps(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string growthtype, string dimension, string investorOrigin)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pe = " + stringPE;
            }

            if (growthtype != null)
            {
                growthtype = "'" + growthtype.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND growth_type = " + growthtype;
            }

            if (dimension != null)
            {
                dimension = "'" + dimension.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dimension = " + dimension;
            }

            if (investorOrigin != null)
            {
                investorOrigin = "'" + investorOrigin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + investorOrigin;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select provinsi as lokasi, cast(sum(current_value) as bigint) current_value From pasarmodal.pe_segmentation_geo where "
                    + whereQuery + " group by provinsi";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPGeospasialInvestorLB(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string growthtype, string dimension, string investorOrigin, string LBType, string province)
        {
            bool isC = false;
            var whereQuery = "1=1";
            var order = " asc";

            if (LBType == "Lg")
            {
                order = "desc";
            }

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pe = " + stringPE;
            }

            if (growthtype != null)
            {
                growthtype = "'" + growthtype.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND growth_type = " + growthtype;
            }

            if (dimension != null)
            {
                dimension = "'" + dimension.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dimension = " + dimension;
            }

            if (investorOrigin != null)
            {
                investorOrigin = "'" + investorOrigin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + investorOrigin;
            }

            if (province != null)
            {
                province = "'" + province.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND provinsi = " + province;
            }
            var props = new WSQueryProperties();


            if (investorOrigin != null)
            {
                if (investorOrigin == "'Lokal'")
                {
                    if (province != null)
                    {
                        props.Query = @"
                    select city as lokasi, investor_type investortype, cast(sum(current_value) as bigint) value From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by city, investor_type order by sum(current_value) " + order + " limit 16";
                    }
                    else
                    {
                        props.Query = @"
                    select provinsi as lokasi, investor_type investortype, cast(sum(current_value) as bigint) value From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by provinsi, investor_type order by sum(current_value) " + order + " limit 16";
                    }
                }
                else
                {
                    props.Query = @"
                    select country as lokasi, investor_type investortype, cast(sum(current_value) as bigint) value From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by country, investor_type order by sum(current_value) " + order + " limit 16";
                }
            }
            else
            {
                props.Query = @"
                    select country as lokasi, investor_type investortype, cast(sum(current_value) as bigint) value From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by country, investor_type order by sum(current_value) " + order + " limit 16";
            }


            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPGeospasialInvestorCG(DataEntities db, DataSourceLoadOptions loadOptions, string periodeawal, string periodeakhir, string stringPE, string growthtype, string dimension, string investorOrigin, string province)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodeawal != null)
            {
                periodeawal = "'" + periodeawal.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                periodeakhir = "'" + periodeakhir.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode between " + periodeawal.Replace("-", "") + " and " + periodeakhir.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pe = " + stringPE;
            }

            if (growthtype != null)
            {
                growthtype = "'" + growthtype.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND growth_type = " + growthtype;
            }

            if (dimension != null)
            {
                dimension = "'" + dimension.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dimension = " + dimension;
            }

            if (investorOrigin != null)
            {
                investorOrigin = "'" + investorOrigin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + investorOrigin;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                         SELECT row_number() over(order by periode) as no,* from (
                            select cast(cast(pperiode as BIGINT)  as BIGINT) as periode, cast(sum(current_value) as bigint) as currentvalue, cast(sum(prev_value - current_value) as bigint) as growth From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by pperiode order by pperiode) as t";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPGeospasialInvestorIT(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string growthtype, string dimension, string investorOrigin, string province)
        {
            bool isC = false;
            var whereQuery = "1=1";
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pe = " + stringPE;
            }

            if (growthtype != null)
            {
                growthtype = "'" + growthtype.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND growth_type = " + growthtype;
            }

            if (dimension != null)
            {
                dimension = "'" + dimension.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dimension = " + dimension;
            }

            if (investorOrigin != null)
            {
                investorOrigin = "'" + investorOrigin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + investorOrigin;
            }
            var props = new WSQueryProperties();


            props.Query = @"
                        select investor_type investortype, cast(sum(current_value) as bigint) currentvalue From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by investor_type";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPGeospasialInvestorIO(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string growthtype, string dimension, string investorOrigin, string province)
        {
            bool isC = false;
            var whereQuery = "1=1";
            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pperiode = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND pe = " + stringPE;
            }

            if (growthtype != null)
            {
                growthtype = "'" + growthtype.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND growth_type = " + growthtype;
            }

            if (dimension != null)
            {
                dimension = "'" + dimension.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND dimension = " + dimension;
            }

            if (investorOrigin != null)
            {
                investorOrigin = "'" + investorOrigin.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND investor_origin = " + investorOrigin;
            }
            var props = new WSQueryProperties();


            props.Query = @"
                        select investor_origin investororigin, cast(sum(current_value) as bigint) currentvalue From pasarmodal.pe_segmentation_geo where "
                            + whereQuery + " group by investor_origin";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMSegmentasiTransaksiLHC(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string jenisTransaksi)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND substring(p_periode,0,6) = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (jenisTransaksi != null)
            {
                jenisTransaksi = "'" + jenisTransaksi.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND datatype = " + jenisTransaksi;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select liquidhc as Type, cast(sum(total_value) as bigint) as Val from pasarmodal.segmentasi_transaksi_kepemilikan
                    WHERE " + whereQuery + " group by liquidhc";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMSegmentasiTransaksiLIM(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string jenisTransaksi)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND substring(p_periode,0,6) = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (jenisTransaksi != null)
            {
                jenisTransaksi = "'" + jenisTransaksi.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND datatype = " + jenisTransaksi;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select liquidim as Type, cast(sum(total_value) as bigint) as Val from pasarmodal.segmentasi_transaksi_kepemilikan
                    WHERE " + whereQuery + " group by liquidim";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        public static WSQueryReturns GetBDAPMSegmentasiTransaksiLNK(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string jenisTransaksi)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND substring(p_periode,0,6) = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (jenisTransaksi != null)
            {
                jenisTransaksi = "'" + jenisTransaksi.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND datatype = " + jenisTransaksi;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select liquidnk as Type, cast(sum(total_value) as bigint) as Val from pasarmodal.segmentasi_transaksi_kepemilikan
                    WHERE " + whereQuery + " group by liquidnk";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }
        // REWRITE: Support Daily, Monthly, and Custom Date, plus all filters from the view
        // REWRITE: fix invalid table name + keep detailed SQL and parameter logging
        // REWRITE: fix table name; return a plain array the PivotGrid can consume; keep detailed debug
        public static List<Dictionary<string, object>> GetMarketDrivenData(
            DataEntities db,
            DataSourceLoadOptions loadOptions,
            string periodType,
            string selectedDate,
            string selectedMonth,
            string startDate,
            string endDate,
            string startTime,
            string endTime,
            string[] confirmation,
            string[] lokalAsing,
            string[] countryInvestor,
            string[] typeInvestor,
            string[] market,
            string[] abCodes,
            int? topN
        )
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS.GetMarketDrivenData START ===");
            System.Diagnostics.Debug.WriteLine($"periodType={periodType}, selectedDate={selectedDate}, selectedMonth={selectedMonth}, startDate={startDate}, endDate={endDate}");
            System.Diagnostics.Debug.WriteLine($"startTime={startTime}, endTime={endTime}, topN={topN}");
            System.Diagnostics.Debug.WriteLine($"confirmation=[{(confirmation == null ? "" : string.Join(",", confirmation))}]");
            System.Diagnostics.Debug.WriteLine($"lokalAsing=[{(lokalAsing == null ? "" : string.Join(",", lokalAsing))}]");
            System.Diagnostics.Debug.WriteLine($"countryInvestor=[{(countryInvestor == null ? "" : string.Join(",", countryInvestor))}]");
            System.Diagnostics.Debug.WriteLine($"typeInvestor=[{(typeInvestor == null ? "" : string.Join(",", typeInvestor))}]");
            System.Diagnostics.Debug.WriteLine($"market=[{(market == null ? "" : string.Join(",", market))}]");
            System.Diagnostics.Debug.WriteLine($"abCodes=[{(abCodes == null ? "" : string.Join(",", abCodes))}]");

            var list = new List<Dictionary<string, object>>();

            try
            {
                var tableName = "BDAPM.pasarmodal.market_driven_validasi_data_tra"; // FIXED
                System.Diagnostics.Debug.WriteLine($"Target table: {tableName}");

                if (string.IsNullOrWhiteSpace(periodType))
                {
                    System.Diagnostics.Debug.WriteLine("[WS] Empty periodType -> return empty.");
                    return list;
                }

                var useTop = topN.HasValue && topN.Value > 0;
                var topClause = useTop ? $"TOP {topN.Value}" : string.Empty;

                var sql = new System.Text.StringBuilder();
                sql.AppendLine($@"
            SELECT {topClause}
                CONVERT(VARCHAR(10), CONVERT(DATE, CAST(tradedatesk AS VARCHAR(8))), 103) AS TransactionDates,
                sid_ori,
                cpinvestorcode,
                securitycode,
                investorcode,
                CAST(quantity AS BIGINT) AS quantity,
                CAST(value  AS BIGINT) AS value
            FROM {tableName} WITH (NOLOCK)
            WHERE 1=1");

                var parameters = new List<SqlParameter>();

                // Period filter branches
                if (string.Equals(periodType, "Daily", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(selectedDate, out var dly))
                    {
                        sql.AppendLine("  AND tradedatesk = @tradedate");
                        parameters.Add(new SqlParameter("@tradedate", dly));
                        System.Diagnostics.Debug.WriteLine($"[WS] Daily: tradedatesk = {dly}");

                        int? sh = null, eh = null;
                        if (!string.IsNullOrWhiteSpace(startTime) && int.TryParse(startTime.Split(':')[0], out var shh)) sh = shh;
                        if (!string.IsNullOrWhiteSpace(endTime) && int.TryParse(endTime.Split(':')[0], out var ehh)) eh = ehh;
                        if (sh.HasValue && !eh.HasValue) eh = sh;
                        if (!sh.HasValue && eh.HasValue) sh = eh;

                        if (sh.HasValue && eh.HasValue)
                        {
                            sql.AppendLine("  AND tradinghourcode BETWEEN @startHour AND @endHour");
                            parameters.Add(new SqlParameter("@startHour", sh.Value));
                            parameters.Add(new SqlParameter("@endHour", eh.Value));
                            System.Diagnostics.Debug.WriteLine($"[WS] Time window: tradinghourcode BETWEEN {sh.Value} AND {eh.Value}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[WS] Invalid Daily selectedDate -> empty.");
                        return list;
                    }
                }
                else if (string.Equals(periodType, "Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(selectedMonth, out var mon))
                    {
                        sql.AppendLine("  AND p_month = @pmonth");
                        parameters.Add(new SqlParameter("@pmonth", mon));
                        System.Diagnostics.Debug.WriteLine($"[WS] Monthly: p_month = {mon}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[WS] Invalid Monthly selectedMonth -> empty.");
                        return list;
                    }
                }
                else if (string.Equals(periodType, "Custom Date", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(startDate, out var sdt) && int.TryParse(endDate, out var edt))
                    {
                        sql.AppendLine("  AND tradedatesk BETWEEN @startdate AND @enddate");
                        parameters.Add(new SqlParameter("@startdate", sdt));
                        parameters.Add(new SqlParameter("@enddate", edt));
                        System.Diagnostics.Debug.WriteLine($"[WS] Custom: tradedatesk BETWEEN {sdt} AND {edt}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[WS] Invalid Custom Date range -> empty.");
                        return list;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[WS] Unknown periodType '{periodType}' -> empty.");
                    return list;
                }

                // Helper to add IN clauses
                void AddInClause(string caption, string column, IEnumerable<string> values, string paramPrefix)
                {
                    var vals = values?.Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                    System.Diagnostics.Debug.WriteLine($"[WS] Filter '{caption}': {(vals == null ? "null" : string.Join(",", vals))}");
                    if (vals == null || vals.Count == 0) return;

                    var paramNames = new List<string>();
                    for (int i = 0; i < vals.Count; i++)
                    {
                        var pname = $"@{paramPrefix}{i}";
                        paramNames.Add(pname);
                        parameters.Add(new SqlParameter(pname, vals[i]));
                    }
                    sql.AppendLine($"  AND {column} IN ({string.Join(",", paramNames)})");
                    System.Diagnostics.Debug.WriteLine($"[WS] Applied IN on {column} ({vals.Count} values)");
                }

                AddInClause("Confirmation", "transactiontypecode", confirmation, "conf");
                AddInClause("Lokal/Asing", "lokal_asing_sid", lokalAsing, "lasg");
                AddInClause("Country Investor", "countryinvestor", countryInvestor, "ctry");
                AddInClause("Type Investor", "typeinvestor", typeInvestor, "tinv");
                AddInClause("Market", "transactionboardcode", market, "mkt");
                AddInClause("AB Codes", "exchangemembercode", abCodes, "ab");

                sql.AppendLine("ORDER BY tradedatesk, tradeno");

                System.Diagnostics.Debug.WriteLine("=== WS FINAL SQL (Validasi Data) ===");
                System.Diagnostics.Debug.WriteLine(sql.ToString());
                if (parameters.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("--- WS SQL Parameters ---");
                    foreach (var p in parameters)
                        System.Diagnostics.Debug.WriteLine($"{p.ParameterName} = {p.Value} ({p.Value?.GetType().Name ?? "null"})");
                    System.Diagnostics.Debug.WriteLine("-------------------------");
                }

                var dt = new DataTable();
                using (var conn = new SqlConnection(db.appSettings.DataConnString))
                using (var cmd = new SqlCommand(sql.ToString(), conn))
                using (var adp = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 300;
                    foreach (var p in parameters) cmd.Parameters.Add(p);

                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("[WS] Executing SQL...");
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    adp.Fill(dt);
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"[WS] SQL done in {sw.ElapsedMilliseconds} ms. Rows: {dt.Rows.Count}");
                }

                if (dt.Rows.Count > 0)
                {
                    var firstRow = dt.Rows[0];
                    System.Diagnostics.Debug.WriteLine("=== WS SAMPLE ROW (first) ===");
                    foreach (DataColumn c in dt.Columns)
                        System.Diagnostics.Debug.WriteLine($"  {c.ColumnName}: {firstRow[c]}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[WS] No data returned for the given filters/period.");
                }

                // Convert to plain array for PivotGrid consumption
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>(dt.Columns.Count, StringComparer.OrdinalIgnoreCase);
                    foreach (DataColumn col in dt.Columns)
                        dict[col.ColumnName] = row[col];
                    list.Add(dict);
                }

                System.Diagnostics.Debug.WriteLine($"=== WSQueryPS.GetMarketDrivenData END (count={list.Count}) ===");
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== WSQueryPS.GetMarketDrivenData ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack  : {ex.StackTrace}");
                return list;
            }
        }

        public static WSQueryReturns GetInvestorGridData(DataEntities db, string filterDate, string periodType = "Daily", string transactionCode = null, object loadOptions = null)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS INVESTOR GRID DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"GetInvestorGridData received filterDate: '{filterDate}', periodType: '{periodType}', transactionCode: '{transactionCode}'");

            try
            {
                // Determine which date column to use based on period type
                string dateColumn = "tradedatesk";
                string dateValue = filterDate;

                if (string.Equals(periodType, "Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    dateColumn = "p_month";
                    // For monthly, we expect YYYYMM format
                    if (filterDate.Length == 8) // Convert YYYYMMDD to YYYYMM
                    {
                        dateValue = filterDate.Substring(0, 6);
                    }
                    else if (filterDate.Length == 6) // Already in YYYYMM format
                    {
                        dateValue = filterDate;
                    }
                    System.Diagnostics.Debug.WriteLine($"Using monthly filter: p_month = {dateValue}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Using daily filter: tradedatesk = {dateValue}");
                }

                // Build WHERE clause
                string whereClause = $"{dateColumn} = @filterDate";

                if (!string.IsNullOrEmpty(transactionCode))
                {
                    whereClause += " AND transactiontypecode = @transactionCode";
                }

                string sqlQuery = $@"
            SELECT 
                ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(value, 0)) DESC) as rowid,
                investorcode,
                cpinvestorcode,
                SUM(ISNULL(value, 0)) as value,
                SUM(ISNULL(quantity, 0)) as quantity
            FROM BDAPM.pasarmodal.market_driven_validasi_data_tra
            WHERE {whereClause}
            GROUP BY investorcode, cpinvestorcode
            ORDER BY value DESC, quantity DESC";

                System.Diagnostics.Debug.WriteLine($"Investor Grid SQL Query: {sqlQuery}");
                System.Diagnostics.Debug.WriteLine($"Parameters: filterDate={dateValue}, transactionCode={transactionCode}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add parameters
                        if (int.TryParse(dateValue, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {dateValue}");
                        }

                        if (!string.IsNullOrEmpty(transactionCode))
                        {
                            cmd.Parameters.AddWithValue("@transactionCode", transactionCode);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Investor grid result rows: {dt.Rows.Count}");

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Investor Grid WSQuery Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new WSQueryReturns
                {
                    data = new DataTable(),
                };
            }
        }

        // Additional method with transaction type filtering
        public static WSQueryReturns GetInvestorGridDataWithTransactionType(DataEntities db, string filterDate, string transactionCode = null, object loadOptions = null)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS INVESTOR GRID WITH TRANSACTION DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"GetInvestorGridDataWithTransactionType received filterDate: '{filterDate}', transactionCode: '{transactionCode}'");

            try
            {
                string whereClause = "tradedatesk = @filterDate";

                // Add transaction type filter if provided
                if (!string.IsNullOrEmpty(transactionCode))
                {
                    whereClause += " AND transactiontypecode = @transactionCode";
                }

                string sqlQuery = $@"
            SELECT 
                investorcode,
                cpinvestorcode,
                SUM(ISNULL(value, 0)) as value,
                SUM(ISNULL(quantity, 0)) as quantity
            FROM BDAPM.pasarmodal.market_driven_validasi_data_tra
            WHERE {whereClause}
            GROUP BY investorcode, cpinvestorcode
            ORDER BY value DESC, quantity DESC";

                System.Diagnostics.Debug.WriteLine($"Investor Grid SQL Query with Transaction: {sqlQuery}");
                System.Diagnostics.Debug.WriteLine($"Parameters: filterDate={filterDate}, transactionCode={transactionCode}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add parameters
                        if (int.TryParse(filterDate, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {filterDate}");
                        }

                        if (!string.IsNullOrEmpty(transactionCode))
                        {
                            cmd.Parameters.AddWithValue("@transactionCode", transactionCode);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Investor grid with transaction result rows: {dt.Rows.Count}");

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Investor Grid with Transaction WSQuery Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new WSQueryReturns
                {
                    data = new DataTable(),
                };
            }
        }

        public static WSQueryReturns GetSecurityGridData(DataEntities db, string filterDate, string periodType = "Daily", string transactionCode = null, object loadOptions = null)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS SECURITY GRID DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"GetSecurityGridData received filterDate: '{filterDate}', periodType: '{periodType}', transactionCode: '{transactionCode}'");

            try
            {
                // Determine which date column to use based on period type
                string dateColumn = "tradedatesk";
                string dateValue = filterDate;

                if (string.Equals(periodType, "Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    dateColumn = "p_month";
                    // For monthly, we expect YYYYMM format
                    if (filterDate.Length == 8) // Convert YYYYMMDD to YYYYMM
                    {
                        dateValue = filterDate.Substring(0, 6);
                    }
                    else if (filterDate.Length == 6) // Already in YYYYMM format
                    {
                        dateValue = filterDate;
                    }
                    System.Diagnostics.Debug.WriteLine($"Using monthly filter: p_month = {dateValue}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Using daily filter: tradedatesk = {dateValue}");
                }

                // Build WHERE clause
                string whereClause = $"{dateColumn} = @filterDate";

                if (!string.IsNullOrEmpty(transactionCode))
                {
                    whereClause += " AND transactiontypecode = @transactionCode";
                }

                string sqlQuery = $@"
            SELECT 
                ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(value, 0)) DESC) as rowid,
                securitycode,
                SUM(ISNULL(value, 0)) as value,
                SUM(ISNULL(quantity, 0)) as quantity,
                COUNT(securitycode) as frequency
            FROM BDAPM.pasarmodal.market_driven_validasi_data_tra
            WHERE {whereClause}
            GROUP BY securitycode
            ORDER BY value DESC, quantity DESC, frequency DESC";

                System.Diagnostics.Debug.WriteLine($"Security Grid SQL Query: {sqlQuery}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add parameters
                        if (int.TryParse(dateValue, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {dateValue}");
                        }

                        if (!string.IsNullOrEmpty(transactionCode))
                        {
                            cmd.Parameters.AddWithValue("@transactionCode", transactionCode);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Security grid result rows: {dt.Rows.Count}");

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Security Grid WSQuery Error: {ex.Message}");
                return new WSQueryReturns { data = new DataTable() };
            }
        }

        // Additional method for more detailed security analysis
        public static WSQueryReturns GetSecurityGridDataDetailed(DataEntities db, string filterDate, string transactionCode = null, int topCount = 0)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS SECURITY GRID DETAILED DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"GetSecurityGridDataDetailed received filterDate: '{filterDate}', transactionCode: '{transactionCode}', topCount: {topCount}");

            try
            {
                // Build WHERE clause
                string whereClause = "tradedatesk = @filterDate";

                if (!string.IsNullOrEmpty(transactionCode))
                {
                    whereClause += " AND transactiontypecode = @transactionCode";
                }

                // Add TOP clause if specified
                string topClause = topCount > 0 ? $"TOP {topCount}" : "";

                // More detailed SQL query with additional metrics
                string sqlQuery = $@"
            SELECT {topClause}
                securitycode,
                SUM(ISNULL(value, 0)) as value,
                SUM(ISNULL(quantity, 0)) as quantity,
                COUNT(securitycode) as frequency,
                AVG(CAST(ISNULL(value, 0) AS DECIMAL(18,2))) as avg_value,
                MAX(ISNULL(value, 0)) as max_value,
                MIN(ISNULL(value, 0)) as min_value
            FROM BDAPM.pasarmodal.market_driven_validasi_data_tra
            WHERE {whereClause}
            GROUP BY securitycode
            ORDER BY value DESC, quantity DESC, frequency DESC";

                System.Diagnostics.Debug.WriteLine($"Security Grid Detailed SQL Query: {sqlQuery}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add parameters
                        if (int.TryParse(filterDate, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {filterDate}");
                        }

                        if (!string.IsNullOrEmpty(transactionCode))
                        {
                            cmd.Parameters.AddWithValue("@transactionCode", transactionCode);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Security grid detailed result rows: {dt.Rows.Count}");

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Security Grid Detailed WSQuery Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new WSQueryReturns
                {
                    data = new DataTable(),
                };
            }
        }

        public static WSQueryReturns GetCPInvestorGridData(DataEntities db, string filterDate, string periodType = "Daily", string transactionCode = null, object loadOptions = null)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS CP INVESTOR GRID DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"GetCPInvestorGridData received filterDate: '{filterDate}', periodType: '{periodType}', transactionCode: '{transactionCode}'");

            try
            {
                // Determine which date column to use based on period type
                string dateColumn = "tradedatesk";
                string dateValue = filterDate;

                if (string.Equals(periodType, "Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    dateColumn = "p_month";
                    // For monthly, we expect YYYYMM format
                    if (filterDate.Length == 8) // Convert YYYYMMDD to YYYYMM
                    {
                        dateValue = filterDate.Substring(0, 6);
                    }
                    else if (filterDate.Length == 6) // Already in YYYYMM format
                    {
                        dateValue = filterDate;
                    }
                    System.Diagnostics.Debug.WriteLine($"Using monthly filter: p_month = {dateValue}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Using daily filter: tradedatesk = {dateValue}");
                }

                // Build WHERE clause
                string whereClause = $"{dateColumn} = @filterDate";

                if (!string.IsNullOrEmpty(transactionCode))
                {
                    whereClause += " AND transactiontypecode = @transactionCode";
                }

                string sqlQuery = $@"
            SELECT 
                ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(value, 0)) DESC) as rowid,
                cpinvestorcode,
                cpinvestorcode as cptradeid,
                SUM(ISNULL(value, 0)) as value,
                SUM(ISNULL(quantity, 0)) as quantity
            FROM BDAPM.pasarmodal.market_driven_validasi_data_tra
            WHERE {whereClause}
            GROUP BY cpinvestorcode
            ORDER BY value DESC, quantity DESC";

                System.Diagnostics.Debug.WriteLine($"CP Investor Grid SQL Query: {sqlQuery}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add parameters
                        if (int.TryParse(dateValue, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {dateValue}");
                        }

                        if (!string.IsNullOrEmpty(transactionCode))
                        {
                            cmd.Parameters.AddWithValue("@transactionCode", transactionCode);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"CP Investor grid result rows: {dt.Rows.Count}");

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CP Investor Grid WSQuery Error: {ex.Message}");
                return new WSQueryReturns { data = new DataTable() };
            }
        }

        // Additional method with frequency count and detailed analysis
        public static WSQueryReturns GetCPInvestorGridDataWithFrequency(DataEntities db, string filterDate, string transactionCode = null, object loadOptions = null)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS CP INVESTOR GRID WITH FREQUENCY DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"GetCPInvestorGridDataWithFrequency received filterDate: '{filterDate}', transactionCode: '{transactionCode}'");

            try
            {
                // Build WHERE clause with optional transaction type filter
                string whereClause = "tradedatesk = @filterDate";

                if (!string.IsNullOrEmpty(transactionCode))
                {
                    whereClause += " AND transactiontypecode = @transactionCode";
                }

                // SQL query for CP Investor data with additional metrics
                string sqlQuery = $@"
            SELECT 
                cpinvestorcode,
                SUM(ISNULL(value, 0)) as value,
                SUM(ISNULL(quantity, 0)) as quantity,
                COUNT(cpinvestorcode) as frequency,
                AVG(CAST(ISNULL(value, 0) AS DECIMAL(18,2))) as avg_value_per_transaction,
                COUNT(DISTINCT securitycode) as unique_securities_traded
            FROM BDAPM.pasarmodal.market_driven_validasi_data_tra
            WHERE {whereClause}
            GROUP BY cpinvestorcode
            ORDER BY value DESC, quantity DESC, frequency DESC";

                System.Diagnostics.Debug.WriteLine($"CP Investor Grid with Frequency SQL Query: {sqlQuery}");
                System.Diagnostics.Debug.WriteLine($"Parameters: filterDate={filterDate}, transactionCode={transactionCode}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add filter date parameter
                        if (int.TryParse(filterDate, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {filterDate}");
                        }

                        // Add transaction code parameter if provided
                        if (!string.IsNullOrEmpty(transactionCode))
                        {
                            cmd.Parameters.AddWithValue("@transactionCode", transactionCode);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"CP Investor grid with frequency result rows: {dt.Rows.Count}");

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CP Investor Grid with Frequency WSQuery Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new WSQueryReturns
                {
                    data = new DataTable(),
                };
            }
        }

     public static List<GainerLoserViewModel> GetGainersOrLosers(
     DataEntities db,
     bool isGainer,
     string selectedDate,
     int topN,
     string periodType = "Daily",
     bool forExport = false) // default = false, kalau untuk Export Excel/PDF set true
        {
            var list = new List<GainerLoserViewModel>();

            if (string.IsNullOrEmpty(selectedDate) || topN <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid parameters: selectedDate='{selectedDate}', topN={topN}");
                return list;
            }

            string historyType = periodType == "Monthly" ? "monthly" : "daily";
            string dateColumn = historyType == "monthly" ? "periode_lvl0" : "periode";
            string orderByClause = isGainer ? "ORDER BY changeprice DESC" : "ORDER BY changeprice ASC";
            string queryType = isGainer ? "Gainers" : "Losers";
            string tableName = "market_driven_ape_growth"; // tanpa schema

            bool isHive = Helper.WSQueryStore.IsPeriodInHive(db, tableName);
            System.Diagnostics.Debug.WriteLine($"IsPeriodInHive({tableName}) = {isHive}");

            // =============================
            //  Build SQL
            // =============================
            string sqlQuery;

            if (isHive)
            {
                                // Hive tidak pakai parameter binding → inline values
                                sqlQuery = $@"
                SELECT 
                    security_code,
                    security_name,
                    CAST(volume AS BIGINT) as volume,
                    CAST(turnover AS DECIMAL(18,2)) as turnover,
                    CAST(freq AS INT) as freq,
                    CAST(net_value AS DECIMAL(18,2)) as net_value,
                    CAST(net_volume AS DECIMAL(18,2)) as net_volume,
                    CAST(point AS DECIMAL(18,2)) as point,
                    CAST(price AS DECIMAL(18,2)) as price,
                    CAST(changeprice AS DECIMAL(18,2)) as changeprice,
                    CAST(highvalue AS DECIMAL(18,2)) as highvalue,
                    CAST(lowvalue AS DECIMAL(18,2)) as lowvalue
                FROM pasarmodal.{tableName}
                WHERE history_type = '{historyType}'
                  AND {dateColumn} = '{selectedDate}'
                {orderByClause}";

                // Tambahkan LIMIT hanya kalau untuk Export (ambil N teratas)
                if (forExport)
                    sqlQuery += $" LIMIT {topN}";
            }
            else
            {
                // SQL Server mode
                // SQL Server mode - ALWAYS include TOP to avoid ORDER BY issues
                string topClause = forExport ? "TOP (@TopN)" : "TOP (1000)"; // Default limit when not exporting
                sqlQuery = $@"
                SELECT {topClause}
                    security_code,
                    security_name,
                    CAST(ISNULL(volume, 0) AS BIGINT) as volume,
                    CAST(ISNULL(turnover, 0) AS DECIMAL(18,2)) as turnover,
                    CAST(ISNULL(freq, 0) AS INT) as freq,
                    CAST(ISNULL(net_value, 0) AS DECIMAL(18,2)) as net_value,
                    CAST(ISNULL(net_volume, 0) AS DECIMAL(18,2)) as net_volume,
                    CAST(ISNULL(point, 0) AS DECIMAL(18,2)) as point,
                    CAST(ISNULL(price, 0) AS DECIMAL(18,2)) as price,
                    CAST(ISNULL(changeprice, 0) AS DECIMAL(18,2)) as changeprice,
                    CAST(ISNULL(highvalue, 0) AS DECIMAL(18,2)) as highvalue,
                    CAST(ISNULL(lowvalue, 0) AS DECIMAL(18,2)) as lowvalue
                FROM pasarmodal.{tableName}
                WHERE history_type = @HistoryType
                  AND {dateColumn} = @Periode
                {orderByClause}";
            }

            System.Diagnostics.Debug.WriteLine("Generated SQL:");
            System.Diagnostics.Debug.WriteLine(sqlQuery);

            try
            {
                // =============================
                //  Prepare props
                // =============================
                var props = new WSQueryProperties
                {
                    Query = sqlQuery,
                    SqlParameters = new List<SqlParameter>()
                };

                if (!isHive)
                {
                    if (forExport)
                        props.SqlParameters.Add(new SqlParameter("@TopN", topN));
                    else
                        props.SqlParameters.Add(new SqlParameter("@TopN", 1000)); // Default limit

                    props.SqlParameters.Add(new SqlParameter("@HistoryType", historyType));
                    props.SqlParameters.Add(new SqlParameter("@Periode", selectedDate));
                }

                var loadOptions = new DataSourceLoadOptions { RequireTotalCount = false };

                // =============================
                //  Execute
                // =============================
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, false, isHive);

                if (result?.data != null)
                {
                    foreach (DataRow row in result.data.Rows)
                    {
                        list.Add(new GainerLoserViewModel
                        {
                            SecurityCode = row["security_code"]?.ToString() ?? "",
                            SecurityName = row["security_name"]?.ToString() ?? "",
                            Volume = Convert.ToInt64(row["volume"] ?? 0),
                            Turnover = Convert.ToDecimal(row["turnover"] ?? 0),
                            Freq = Convert.ToInt32(row["freq"] ?? 0),
                            NetValue = Convert.ToDecimal(row["net_value"] ?? 0),
                            NetVolume = Convert.ToDecimal(row["net_volume"] ?? 0),
                            Point = Convert.ToDecimal(row["point"] ?? 0),
                            Price = Convert.ToDecimal(row["price"] ?? 0),
                            ChangePercentage = Convert.ToDecimal(row["changeprice"] ?? 0),
                            MaxPrice = Convert.ToDecimal(row["highvalue"] ?? 0),
                            MinPrice = Convert.ToDecimal(row["lowvalue"] ?? 0)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Get{queryType}: {ex.Message}");
                throw;
            }

            System.Diagnostics.Debug.WriteLine("=== WSQueryPS.GetGainersOrLosers END ===");
            return list;
        }



        public static List<GainerLoserViewModel> GetGainersOrLosersCustomDate(
    DataEntities db, string startDate, string endDate, int topN, bool isGainer)
        {
            var list = new List<GainerLoserViewModel>();
            System.Diagnostics.Debug.WriteLine($"[WS G/L SINGLE] isGainer={isGainer} selectedDate={startDate} endDate={endDate}len={startDate?.Length} topN={topN}");
            string orderByClause = isGainer ? "ChangePercentage DESC" : "ChangePercentage ASC";
            string sqlQuery = $@"
        WITH Aggregated AS (
            SELECT
                security_code,
                MAX(security_name) as security_name,
                SUM(volume) as volume,
                AVG(turnover) as turnover,
                SUM(freq) as freq,
                SUM(net_value) as net_value,
                SUM(net_volume) as net_volume,
                AVG(point) as point,
                MAX(CASE WHEN periode = @EndDate THEN price ELSE NULL END) as price,
                MAX(CASE WHEN periode = @EndDate THEN price ELSE NULL END) as price_end,
                MIN(CASE WHEN periode = @StartDate THEN price ELSE NULL END) as price_start,
                ((MAX(CASE WHEN periode = @EndDate THEN price ELSE NULL END) - MIN(CASE WHEN periode = @StartDate THEN price ELSE NULL END)) * 100.0 / NULLIF(MIN(CASE WHEN periode = @StartDate THEN price ELSE NULL END),0)) as ChangePercentage,
                MAX(highvalue) as MaxPrice,
                MIN(lowvalue) as MinPrice
            FROM BDAPM.pasarmodal.market_driven_ape_growth
            WHERE history_type = 'daily'
              AND periode BETWEEN @StartDate AND @EndDate
            GROUP BY security_code
        )
        SELECT TOP (@TopN)
            security_code,
            security_name,
            volume,
            turnover,
            freq,
            net_value,
            net_volume,
            point,
            price,
            ChangePercentage,
            MaxPrice,
            MinPrice
        FROM Aggregated
        ORDER BY {orderByClause};
    ";
      
            using (SqlConnection conn = new SqlConnection(db.appSettings.DataConnString))
            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
            {
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@TopN", topN);

                conn.Open();
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
           

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new GainerLoserViewModel
                    {
                        SecurityCode = row["security_code"]?.ToString() ?? "",
                        SecurityName = row["security_name"]?.ToString() ?? "",
                        Volume = row["volume"] == DBNull.Value ? 0 : Convert.ToInt64(row["volume"]),
                        Turnover = row["turnover"] == DBNull.Value ? 0 : Convert.ToDecimal(row["turnover"]),
                        Freq = row["freq"] == DBNull.Value ? 0 : Convert.ToInt32(row["freq"]),
                        NetValue = row["net_value"] == DBNull.Value ? 0 : Convert.ToDecimal(row["net_value"]),
                        NetVolume = row["net_volume"] == DBNull.Value ? 0 : Convert.ToDecimal(row["net_volume"]),
                        Point = row["point"] == DBNull.Value ? 0 : Convert.ToDecimal(row["point"]),
                        Price = row["price"] == DBNull.Value ? 0 : Convert.ToDecimal(row["price"]),
                        ChangePercentage = row["ChangePercentage"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ChangePercentage"]),
                        MaxPrice = row["MaxPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MaxPrice"]),
                        MinPrice = row["MinPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MinPrice"])
                    });
                }
            }
            return list;
        }
        public static List<LeaderLaggardViewModel> GetLeadersOrLaggards(DataEntities db, bool isLeader, string selectedDate, int topN, string periodType = "Daily")
        {
            var list = new List<LeaderLaggardViewModel>();

            // Input validation
            if (string.IsNullOrEmpty(selectedDate) || topN <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid parameters: selectedDate='{selectedDate}', topN={topN}");
                return list;
            }

            // Determine history_type based on periodType
            string historyType;
            switch (periodType)
            {
                case "Monthly":
                    historyType = "monthly";
                    break;
                case "Daily":
                case "Custom Date":
                default:
                    historyType = "daily";
                    break;
            }

            // Determine the sorting order based on the isLeader flag
            string orderByClause = isLeader ? "ORDER BY point DESC" : "ORDER BY point ASC";
            string queryType = isLeader ? "Leaders" : "Laggards";

            // Build the query with dynamic history_type
            string sqlQuery = $@"
SET ARITHABORT ON;
SELECT TOP (@TopN) 
    security_code,
    security_name,
    ISNULL(volume, 0) as volume,
    ISNULL(turnover, 0) as turnover,
    ISNULL(freq, 0) as freq,
    ISNULL(net_value, 0) as net_value,
    ISNULL(net_volume, 0) as net_volume,
    ISNULL(point, 0) as point,
    ISNULL(price, 0) as price,
    ISNULL(changeprice, 0) as changeprice,
    ISNULL(highvalue, 0) as highvalue,
    ISNULL(lowvalue, 0) as lowvalue
FROM BDAPM.pasarmodal.market_driven_ape_growth
WHERE history_type = @HistoryType AND periode = @Periode
{orderByClause}";

            string connString = db.appSettings.DataConnString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.Parameters.AddWithValue("@Periode", selectedDate);
                        cmd.Parameters.AddWithValue("@TopN", topN);
                        cmd.Parameters.AddWithValue("@HistoryType", historyType);

                        conn.Open();
                        DataTable dt = new DataTable();
                        new SqlDataAdapter(cmd).Fill(dt);

                        // Map the results to the ViewModel list with null checks
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new LeaderLaggardViewModel
                            {
                                SecurityCode = row["security_code"]?.ToString() ?? "",
                                SecurityName = row["security_name"]?.ToString() ?? "",
                                Volume = Convert.ToInt64(row["volume"] ?? 0),
                                Turnover = Convert.ToDecimal(row["turnover"] ?? 0),
                                Freq = Convert.ToInt32(row["freq"] ?? 0),
                                NetValue = Convert.ToDecimal(row["net_value"] ?? 0),
                                NetVolume = Convert.ToDecimal(row["net_volume"] ?? 0),
                                Point = Convert.ToDecimal(row["point"] ?? 0),
                                Price = Convert.ToDecimal(row["price"] ?? 0),
                                ChangePercentage = Convert.ToDecimal(row["changeprice"] ?? 0),
                                MaxPrice = Convert.ToDecimal(row["highvalue"] ?? 0),
                                MinPrice = Convert.ToDecimal(row["lowvalue"] ?? 0)
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error in Get{queryType}: {sqlEx.Message}");
                System.Diagnostics.Debug.WriteLine($"SQL Query: {sqlQuery}");
                throw new ApplicationException($"Database error while retrieving {queryType.ToLower()} data for {periodType} period", sqlEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error in Get{queryType}: {ex.Message}");
                throw new ApplicationException($"Error while retrieving {queryType.ToLower()} data for {periodType} period", ex);
            }

            return list;
        }

        public static List<LeaderLaggardViewModel> GetLeadersOrLaggardsCustomDate(
    DataEntities db, string startDate, string endDate, int topN, bool isLeader)
        {
            var list = new List<LeaderLaggardViewModel>();
            string orderByClause = isLeader ? "point DESC" : "point ASC";
            string sqlQuery = $@"
WITH Aggregated AS (
    SELECT
        security_code,
        MAX(security_name) as security_name,
        SUM(volume) as volume,
        AVG(turnover) as turnover,
        SUM(freq) as freq,
        SUM(net_value) as net_value,
        SUM(net_volume) as net_volume,
        AVG(point) as point,
        MAX(CASE WHEN periode = @EndDate THEN price ELSE NULL END) as price,
        MAX(CASE WHEN periode = @EndDate THEN price ELSE NULL END) as price_end,
        MIN(CASE WHEN periode = @StartDate THEN price ELSE NULL END) as price_start,
        ((MAX(CASE WHEN periode = @EndDate THEN price ELSE NULL END) - MIN(CASE WHEN periode = @StartDate THEN price ELSE NULL END)) * 100.0 / NULLIF(MIN(CASE WHEN periode = @StartDate THEN price ELSE NULL END),0)) as ChangePercentage,
        MAX(highvalue) as MaxPrice,
        MIN(lowvalue) as MinPrice
    FROM BDAPM.pasarmodal.market_driven_ape_growth
    WHERE history_type = 'daily'
      AND periode BETWEEN @StartDate AND @EndDate
    GROUP BY security_code
)
SELECT TOP (@TopN)
    security_code,
    security_name,
    volume,
    turnover,
    freq,
    net_value,
    net_volume,
    point,
    price,
    ChangePercentage,
    MaxPrice,
    MinPrice
FROM Aggregated
ORDER BY {orderByClause};
";

            using (SqlConnection conn = new SqlConnection(db.appSettings.DataConnString))
            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
            {
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@TopN", topN);

                conn.Open();
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new LeaderLaggardViewModel
                    {
                        SecurityCode = row["security_code"]?.ToString() ?? "",
                        SecurityName = row["security_name"]?.ToString() ?? "",
                        Volume = row["volume"] == DBNull.Value ? 0 : Convert.ToInt64(row["volume"]),
                        Turnover = row["turnover"] == DBNull.Value ? 0 : Convert.ToDecimal(row["turnover"]),
                        Freq = row["freq"] == DBNull.Value ? 0 : Convert.ToInt32(row["freq"]),
                        NetValue = row["net_value"] == DBNull.Value ? 0 : Convert.ToDecimal(row["net_value"]),
                        NetVolume = row["net_volume"] == DBNull.Value ? 0 : Convert.ToDecimal(row["net_volume"]),
                        Point = row["point"] == DBNull.Value ? 0 : Convert.ToDecimal(row["point"]),
                        Price = row["price"] == DBNull.Value ? 0 : Convert.ToDecimal(row["price"]),
                        ChangePercentage = row["ChangePercentage"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ChangePercentage"]),
                        MaxPrice = row["MaxPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MaxPrice"]),
                        MinPrice = row["MinPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MinPrice"])
                    });
                }
            }
            return list;
        }
        public static WSQueryReturns GetSTPBalanceData(DataEntities db, DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            

            bool isC = false;
            var whereQuery = "1=1";

          

            // Build the WHERE clause based on provided filters
            if (!string.IsNullOrEmpty(startDate))
            {
                whereQuery += " AND tanggal_balance >= " + startDate;
                System.Diagnostics.Debug.WriteLine($"Added startDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("StartDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                whereQuery += " AND tanggal_balance <= " + endDate;
                System.Diagnostics.Debug.WriteLine($"Added endDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("EndDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(SID))
            {
                string escapedSID = "'" + SID.Replace("'", "''") + "'"; // Prevent SQL injection
                whereQuery += " AND sid = " + escapedSID;
   
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SID is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(Efek))
            {
                string escapedEfek = "'" + Efek.Replace("'", "''") + "'"; // Prevent SQL injection
                whereQuery += " AND efek = " + escapedEfek;
  
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Efek is null or empty - skipping");
            }

            var props = new WSQueryProperties();

            // Build the complete query
            string baseQuery = @"
        SELECT 
            ROW_NUMBER() OVER (ORDER BY tanggal_balance, sid) as rowid,
            tanggal_balance,
            sid,
            trading_id,
            efek,
            subrekening_efek,
            CAST(quantity as bigint) as quantity,
            CAST(price as bigint) as price
        FROM BDAPM.pasarmodal.market_driven_stp_balance
        WHERE " + whereQuery;

            props.Query = baseQuery;

            

            System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryHelper.DoQuery ===");

            try
            {
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, isC, false);

               

                if (result?.data?.Rows?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("=== SAMPLE DATA (First Row) ===");
                    var firstRow = result.data.Rows[0];
                    foreach (DataColumn column in result.data.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {column.ColumnName}: {firstRow[column.ColumnName]}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
             
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        // Add this method to your existing WSQueryPS class

        public static WSQueryReturns GetSTPSettlementData(DataEntities db, DataSourceLoadOptions loadOptions,
            string startDate, string endDate, string SID, string Efek)
        {
           

            bool isC = false;
            var whereQuery = "1=1";

        

            // Build the WHERE clause based on provided filters
            if (!string.IsNullOrEmpty(startDate))
            {
                whereQuery += " AND settlementtransactiondatesk >= " + startDate;
                System.Diagnostics.Debug.WriteLine($"Added startDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("StartDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                whereQuery += " AND settlementtransactiondatesk <= " + endDate;
                System.Diagnostics.Debug.WriteLine($"Added endDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("EndDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(SID))
            {
                string escapedSID = "'" + SID.Replace("'", "''") + "'"; // Prevent SQL injection
                whereQuery += " AND fpinvestorid = " + escapedSID;
                System.Diagnostics.Debug.WriteLine($"Added SID condition: {whereQuery}");
                System.Diagnostics.Debug.WriteLine($"Escaped SID: {escapedSID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SID is null or empty - skipping");
            }

            // Note: For Settlement, we're not filtering by Efek since it's not mentioned in the requirements
            // If you need to filter by a security code field, you can add it here

            var props = new WSQueryProperties();

            // Build the complete query for Settlement
            string baseQuery = @"
        SELECT 
            ROW_NUMBER() OVER (ORDER BY settlementtransactiondatesk, fpinvestorid) as rowid,
            settlementtransactiondatesk,
            tradingdatesk,
            fpinvestorid,
            cpinvestorid,
            fpsettlementaccountcode,
            cpsettlementaccountcode
        FROM BDAPM.pasarmodal.market_driven_stp_settlement_new
        WHERE " + whereQuery;

            props.Query = baseQuery;

           

         

            try
            {
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, isC, false);

        

                if (result?.data?.Rows?.Count > 0)
                {
                    
                    var firstRow = result.data.Rows[0];
                    foreach (DataColumn column in result.data.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {column.ColumnName}: {firstRow[column.ColumnName]}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
              
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public static WSQueryReturns GetSTPClearingData(DataEntities db, DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            

            bool isC = false;
            var whereQuery = "1=1";

     
            // Build the WHERE clause based on provided filters
            if (!string.IsNullOrEmpty(startDate))
            {
                whereQuery += " AND settlementdatesk >= " + startDate;
                System.Diagnostics.Debug.WriteLine($"Added startDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("StartDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                whereQuery += " AND settlementdatesk <= " + endDate;
                System.Diagnostics.Debug.WriteLine($"Added endDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("EndDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(SID))
            {
                string escapedSID = "'" + SID.Replace("'", "''") + "'"; // Prevent SQL injection
                whereQuery += " AND investorid = " + escapedSID;
                System.Diagnostics.Debug.WriteLine($"Added SID condition: {whereQuery}");
                System.Diagnostics.Debug.WriteLine($"Escaped SID: {escapedSID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SID is null or empty - skipping");
            }

            // Note: For Clearing, we're not filtering by Efek since it's not mentioned in the requirements
            // If you need to filter by a security code field, you can add it here

            var props = new WSQueryProperties();

            // Build the complete query for Clearing
            string baseQuery = @"
      SELECT 
            ROW_NUMBER() OVER (ORDER BY settlementdatesk, investorid) as rowid,
            settlementdatesk,
            clearingdatesk,
            investorid,
            securitycompanycode,
            ISNULL(clearingobligationquantity, '0') as clearingobligationquantity
        FROM BDAPM.pasarmodal.market_driven_stp_clearing
        WHERE " + whereQuery;


            props.Query = baseQuery;

           

            System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryHelper.DoQuery FOR CLEARING ===");

            try
            {
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, isC, false);

                System.Diagnostics.Debug.WriteLine("=== CLEARING WSQueryHelper.DoQuery RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Data rows count: {result?.data?.Rows?.Count ?? 0}");

                if (result?.data?.Rows?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("=== CLEARING SAMPLE DATA (First Row) ===");
                    var firstRow = result.data.Rows[0];
                    foreach (DataColumn column in result.data.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {column.ColumnName}: {firstRow[column.ColumnName]}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== CLEARING WSQueryHelper.DoQuery ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in Clearing WSQueryHelper.DoQuery: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
        public static WSQueryReturns GetSTPTransactionData(DataEntities db, DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            

            bool isC = false;
            var whereQuery = "1=1";

        

            // Build the WHERE clause based on provided filters
            if (!string.IsNullOrEmpty(startDate))
            {
                whereQuery += " AND settledate >= " + startDate;
                System.Diagnostics.Debug.WriteLine($"Added startDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("StartDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                whereQuery += " AND settledate <= " + endDate;
                System.Diagnostics.Debug.WriteLine($"Added endDate condition: {whereQuery}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("EndDate is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(SID))
            {
                string escapedSID = "'" + SID.Replace("'", "''") + "'"; // Prevent SQL injection
                whereQuery += " AND sid = " + escapedSID;
                System.Diagnostics.Debug.WriteLine($"Added SID condition: {whereQuery}");
                System.Diagnostics.Debug.WriteLine($"Escaped SID: {escapedSID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SID is null or empty - skipping");
            }

            if (!string.IsNullOrEmpty(Efek))
            {
                string escapedEfek = "'" + Efek.Replace("'", "''") + "'"; // Prevent SQL injection
                whereQuery += " AND securitycode = " + escapedEfek;
                System.Diagnostics.Debug.WriteLine($"Added Efek condition: {whereQuery}");
                System.Diagnostics.Debug.WriteLine($"Escaped Efek: {escapedEfek}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Efek is null or empty - skipping");
            }

            var props = new WSQueryProperties();

            // Build the complete query for Transaction with transaction type translation
            string baseQuery = @"
        SELECT 
            ROW_NUMBER() OVER (ORDER BY settledate, sid) as rowid,
            settledate,
            tradedatesk,
            sid,
            investorcode,
            cpinvestorcode,
            securitycode,
            transactiontypecode,
            CASE 
                WHEN transactiontypecode = 'B' THEN 'Buy'
                WHEN transactiontypecode = 'S' THEN 'Sell'
                ELSE transactiontypecode
            END as transactiontypedesc,
            volume
        FROM BDAPM.pasarmodal.market_driven_stp_tra
        WHERE " + whereQuery;

            props.Query = baseQuery;

            System.Diagnostics.Debug.WriteLine("=== FINAL TRANSACTION SQL QUERY ===");
            System.Diagnostics.Debug.WriteLine($"Complete SQL Query:");
            System.Diagnostics.Debug.WriteLine(props.Query);
            System.Diagnostics.Debug.WriteLine("=== END TRANSACTION SQL QUERY ===");

            System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryHelper.DoQuery FOR TRANSACTION ===");

            try
            {
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, isC, false);

                System.Diagnostics.Debug.WriteLine("=== TRANSACTION WSQueryHelper.DoQuery RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Data rows count: {result?.data?.Rows?.Count ?? 0}");

                if (result?.data?.Rows?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("=== TRANSACTION SAMPLE DATA (First Row) ===");
                    var firstRow = result.data.Rows[0];
                    foreach (DataColumn column in result.data.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {column.ColumnName}: {firstRow[column.ColumnName]}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== TRANSACTION WSQueryHelper.DoQuery ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in Transaction WSQueryHelper.DoQuery: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public static WSQueryReturns GetMarketDrivenSummaryData(DataEntities db, string filterDate)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS SUMMARY DIRECT SQL DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"WSQueryPS Summary received filterDate: '{filterDate}'");

            try
            {
                string sqlQuery = @"
            SELECT TOP 1
                calendarsk,
                ISNULL(closingvalue, 0) as closingvalue,
                ISNULL(marketcapitalizationamount, 0) as marketcapitalizationamount,
                ISNULL(net_value, 0) as net_value,
                ISNULL(net_volume, 0) as net_volume
            FROM BDAPM.pasarmodal.market_driven_sum_trades
            WHERE calendarsk = @filterDate
            ORDER BY calendarsk DESC";

                System.Diagnostics.Debug.WriteLine($"Direct SQL Query: {sqlQuery}");
                System.Diagnostics.Debug.WriteLine($"Filter Date Parameter: {filterDate}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Convert string to int for the parameter
                        if (int.TryParse(filterDate, out int dateAsInt))
                        {
                            cmd.Parameters.AddWithValue("@filterDate", dateAsInt);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format: {filterDate}");
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Direct SQL result rows: {dt.Rows.Count}");

                if (dt.Rows.Count > 0)
                {
                    var firstRow = dt.Rows[0];
                    System.Diagnostics.Debug.WriteLine("=== DIRECT SQL SAMPLE DATA ===");
                    foreach (DataColumn column in dt.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {column.ColumnName}: {firstRow[column.ColumnName]}");
                    }
                }

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Direct SQL Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return new WSQueryReturns
                {
                    data = new DataTable(),
            
                };
            }
        }

        public static WSQueryReturns GetMarketChartData(DataEntities db, string chartType, string startDate = null, string endDate = null, string singleDate = null)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS CHART DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"Chart Type: {chartType}, Single: {singleDate}, Start: {startDate}, End: {endDate}");

            try
            {
                string whereClause = "1=1";
                List<SqlParameter> parameters = new List<SqlParameter>();

                // Build WHERE clause based on date parameters - FIX THE PRIORITY ORDER
                if (!string.IsNullOrEmpty(singleDate))
                {
                    whereClause += " AND calendarsk = @singleDate";
                    parameters.Add(new SqlParameter("@singleDate", int.Parse(singleDate)));
                    System.Diagnostics.Debug.WriteLine($"Using single date filter: {singleDate}");
                }
                else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    whereClause += " AND calendarsk >= @startDate AND calendarsk <= @endDate";
                    parameters.Add(new SqlParameter("@startDate", int.Parse(startDate)));
                    parameters.Add(new SqlParameter("@endDate", int.Parse(endDate)));
                    System.Diagnostics.Debug.WriteLine($"Using date range filter: {startDate} to {endDate}");
                }
                else
                {
                    // FIX: Don't return default data if no proper dates provided
                    System.Diagnostics.Debug.WriteLine("ERROR: No valid date parameters provided");
                    return new WSQueryReturns
                    {
                        data = new DataTable(),
                    };
                }

                // Determine value field based on chart type
                string valueField;
                switch (chartType?.ToLower())
                {
                    case "volume":
                        valueField = "tradingvolume";
                        break;
                    case "market cap":
                        valueField = "marketcapitalizationamount";
                        break;
                    default: // "value"
                        valueField = "closingvalue";
                        break;
                }

                string sqlQuery = $@"
            SELECT 
                calendarsk,
                CAST(CONCAT(
                    SUBSTRING(CAST(calendarsk AS VARCHAR), 1, 4), '-',
                    SUBSTRING(CAST(calendarsk AS VARCHAR), 5, 2), '-', 
                    SUBSTRING(CAST(calendarsk AS VARCHAR), 7, 2)
                ) AS DATE) as date,
                ISNULL({valueField}, 0) as value
            FROM BDAPM.pasarmodal.market_driven_sum_trades
            WHERE {whereClause}
            ORDER BY calendarsk ASC";

                System.Diagnostics.Debug.WriteLine("=== CHART SQL QUERY ===");
                System.Diagnostics.Debug.WriteLine(sqlQuery);
                System.Diagnostics.Debug.WriteLine($"Parameters: {string.Join(", ", parameters.Select(p => $"{p.ParameterName}={p.Value}"))}");

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;

                        // Add parameters to command
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Chart data rows returned: {dt.Rows.Count}");

                // Debug: Show date range of returned data
                if (dt.Rows.Count > 0)
                {
                    var firstDate = dt.Rows[0]["calendarsk"];
                    var lastDate = dt.Rows[dt.Rows.Count - 1]["calendarsk"];
                    System.Diagnostics.Debug.WriteLine($"Date range in results: {firstDate} to {lastDate}");
                }

                return new WSQueryReturns
                {
                    data = dt,
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Chart WSQuery error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                return new WSQueryReturns
                {
                    data = new DataTable(),
                };
            }
        }
        public static WSQueryReturns GetBDAPMSegmentasiTransaksiGrid(DataEntities db, DataSourceLoadOptions loadOptions, string periodes, string stringPE, string jenisTransaksi)
        {
            bool isC = false;
            var whereQuery = "1=1";

            if (periodes != null)
            {
                periodes = "'" + periodes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND substring(p_periode,0,6) = " + periodes.Replace("-", "");
            }

            if (stringPE != null)
            {
                stringPE = "'" + stringPE.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND exchangemembercode = " + stringPE;
            }

            if (jenisTransaksi != null)
            {
                jenisTransaksi = "'" + jenisTransaksi.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
                whereQuery = whereQuery += " AND datatype = " + jenisTransaksi;
            }
            var props = new WSQueryProperties();

            props.Query = @"
                    select exchangemembercode as KodeEfek, liquidhc as LiquidHC, liquidim as LiquidIM, liquidnk as LiquidNK, cast(sum(total_volume) as bigint) as TotalVolume, cast(sum(total_value) as bigint) as TotalValue, cast(sum(total_freq) as bigint) as TotalFreq from pasarmodal.segmentasi_transaksi_kepemilikan
                    WHERE " + whereQuery + " group by exchangemembercode, liquidhc, liquidim, liquidnk";

            return WSQueryHelper.DoQuery(db, props, loadOptions, isC, true);
        }

        internal static object GetTopCompaniesByValue(object loadOptions, string selectedDate)
        {
            throw new NotImplementedException();
        }
    }
}
