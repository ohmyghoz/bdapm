using BDA.DataModel;
using BDA.Helper.FW;
using BDA.Models;
using DevExpress.Data.Extensions;
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
        public static object GetMarketDrivenData(DataEntities db, DataSourceLoadOptions loadOptions, string selectedDate)
        {
            // If no date is selected, return an empty result.
            if (string.IsNullOrEmpty(selectedDate))
            {
                return new { data = new object[0], totalCount = 0 };
            }

            string dateForQuery = selectedDate.Replace("-", "");

            string sqlQuery = $@"
        SELECT 
           CONVERT(VARCHAR(10), CONVERT(DATE, CAST(tradedatesk AS VARCHAR(8))), 105) AS TransactionDates,
            sid_ori, cpinvestorcode, securitycode, investorcode, quantity, value
        FROM pasarmodal.market_driven_validasi_data_tra
        WHERE tradedatesk = @tradedate";

            DataTable dt = new DataTable();
            string connString = db.appSettings.DataConnString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.CommandTimeout = 300;
                    int dateAsInt = Int32.Parse(dateForQuery);
                    cmd.Parameters.AddWithValue("@tradedate", dateAsInt);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            // --- THIS IS THE NEW PART ---

            // Manually convert the DataTable to a List of Dictionaries
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }

            // Return the raw data in a structure DevExtreme understands.
            // We are deliberately IGNORING the 'loadOptions' for this test.
            return new { data = list, totalCount = list.Count };
        }

        public static List<GainerLoserViewModel> GetGainersOrLosers(DataEntities db, bool isGainer, string selectedDate, int topN)
        {
            var list = new List<GainerLoserViewModel>();

            // Determine the sorting order based on the isGainer flag
            string orderByClause = isGainer ? "ORDER BY changeprice DESC" : "ORDER BY changeprice ASC";

            // Build the query
            string sqlQuery = $@"
        SET ARITHABORT ON;
        SELECT TOP (@TopN) * FROM pasarmodal.market_driven_ape_growth
        WHERE history_type = 'daily' AND periode = @Periode
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

                        conn.Open();
                        DataTable dt = new DataTable();
                        new SqlDataAdapter(cmd).Fill(dt);

                        // Map the results to the ViewModel list
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new GainerLoserViewModel
                            {
                                SecurityCode = row["security_code"] as string,
                                SecurityName = row["security_name"] as string,
                                Volume = row["volume"] != DBNull.Value ? Convert.ToInt64(row["volume"]) : 0,
                                Turnover = row["turnover"] != DBNull.Value ? Convert.ToDecimal(row["turnover"]) : 0,
                                Freq = row["freq"] != DBNull.Value ? Convert.ToInt32(row["freq"]) : 0,
                                NetValue = row["net_value"] != DBNull.Value ? Convert.ToDecimal(row["net_value"]) : 0,
                                NetVolume = row["net_volume"] != DBNull.Value ? Convert.ToDecimal(row["net_volume"]) : 0,
                                Point = row["point"] != DBNull.Value ? Convert.ToDecimal(row["point"]) : 0,
                                Price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : 0,
                                ChangePercentage = row["changeprice"] != DBNull.Value ? Convert.ToDecimal(row["changeprice"]) : 0,
                                MaxPrice = row["highvalue"] != DBNull.Value ? Convert.ToDecimal(row["highvalue"]) : 0,
                                MinPrice = row["lowvalue"] != DBNull.Value ? Convert.ToDecimal(row["lowvalue"]) : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the error
                System.Diagnostics.Debug.WriteLine("DATABASE ERROR: " + ex.Message);
            }
            return list;
        }

        public static List<LeaderLaggardViewModel> GetLeadersOrLaggards(DataEntities db, bool isLeader, string selectedDate, int topN)
        {
            var list = new List<LeaderLaggardViewModel>();

            // Determine the sorting order based on the isGainer flag
            string orderByClause = isLeader ? "ORDER BY point DESC" : "ORDER BY point ASC";

            // Build the query
            string sqlQuery = $@"
        SET ARITHABORT ON;
        SELECT TOP (@TopN) * FROM pasarmodal.market_driven_ape_growth
        WHERE history_type = 'daily' AND periode = @Periode
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

                        conn.Open();
                        DataTable dt = new DataTable();
                        new SqlDataAdapter(cmd).Fill(dt);

                        // Map the results to the ViewModel list
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new LeaderLaggardViewModel
                            {
                                SecurityCode = row["security_code"] as string,
                                SecurityName = row["security_name"] as string,
                                Volume = row["volume"] != DBNull.Value ? Convert.ToInt64(row["volume"]) : 0,
                                Turnover = row["turnover"] != DBNull.Value ? Convert.ToDecimal(row["turnover"]) : 0,
                                Freq = row["freq"] != DBNull.Value ? Convert.ToInt32(row["freq"]) : 0,
                                NetValue = row["net_value"] != DBNull.Value ? Convert.ToDecimal(row["net_value"]) : 0,
                                NetVolume = row["net_volume"] != DBNull.Value ? Convert.ToDecimal(row["net_volume"]) : 0,
                                Point = row["point"] != DBNull.Value ? Convert.ToDecimal(row["point"]) : 0,
                                Price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : 0,
                                ChangePercentage = row["changeprice"] != DBNull.Value ? Convert.ToDecimal(row["changeprice"]) : 0,
                                MaxPrice = row["highvalue"] != DBNull.Value ? Convert.ToDecimal(row["highvalue"]) : 0,
                                MinPrice = row["lowvalue"] != DBNull.Value ? Convert.ToDecimal(row["lowvalue"]) : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the error
                System.Diagnostics.Debug.WriteLine("DATABASE ERROR: " + ex.Message);
            }
            return list;
        }

        // Replace the GetSTPBalanceData method in your WSQueryPS class with this corrected version

        public static WSQueryReturns GetSTPBalanceData(DataEntities db, DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"WSQueryPS received parameters:");
            System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}' (Type: {startDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}' (Type: {endDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}' (Type: {SID?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}' (Type: {Efek?.GetType()})");

            bool isC = false;
            var whereQuery = "1=1";

            System.Diagnostics.Debug.WriteLine("=== BUILDING WHERE CLAUSE ===");
            System.Diagnostics.Debug.WriteLine($"Initial whereQuery: {whereQuery}");

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
                whereQuery += " AND efek = " + escapedEfek;
                System.Diagnostics.Debug.WriteLine($"Added Efek condition: {whereQuery}");
                System.Diagnostics.Debug.WriteLine($"Escaped Efek: {escapedEfek}");
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

            System.Diagnostics.Debug.WriteLine("=== FINAL SQL QUERY ===");
            System.Diagnostics.Debug.WriteLine($"Complete SQL Query:");
            System.Diagnostics.Debug.WriteLine(props.Query);
            System.Diagnostics.Debug.WriteLine("=== END SQL QUERY ===");

            System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryHelper.DoQuery ===");

            try
            {
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, isC, false);

                System.Diagnostics.Debug.WriteLine("=== WSQueryHelper.DoQuery RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Data rows count: {result?.data?.Rows?.Count ?? 0}");

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
                System.Diagnostics.Debug.WriteLine("=== WSQueryHelper.DoQuery ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in WSQueryHelper.DoQuery: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        // Add this method to your existing WSQueryPS class

        public static WSQueryReturns GetSTPSettlementData(DataEntities db, DataSourceLoadOptions loadOptions,
            string startDate, string endDate, string SID, string Efek)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS SETTLEMENT DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"WSQueryPS Settlement received parameters:");
            System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}' (Type: {startDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}' (Type: {endDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}' (Type: {SID?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}' (Type: {Efek?.GetType()})");

            bool isC = false;
            var whereQuery = "1=1";

            System.Diagnostics.Debug.WriteLine("=== BUILDING SETTLEMENT WHERE CLAUSE ===");
            System.Diagnostics.Debug.WriteLine($"Initial whereQuery: {whereQuery}");

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

            System.Diagnostics.Debug.WriteLine("=== FINAL SETTLEMENT SQL QUERY ===");
            System.Diagnostics.Debug.WriteLine($"Complete SQL Query:");
            System.Diagnostics.Debug.WriteLine(props.Query);
            System.Diagnostics.Debug.WriteLine("=== END SETTLEMENT SQL QUERY ===");

            System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryHelper.DoQuery FOR SETTLEMENT ===");

            try
            {
                var result = WSQueryHelper.DoQuery(db, props, loadOptions, isC, false);

                System.Diagnostics.Debug.WriteLine("=== SETTLEMENT WSQueryHelper.DoQuery RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Data rows count: {result?.data?.Rows?.Count ?? 0}");

                if (result?.data?.Rows?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("=== SETTLEMENT SAMPLE DATA (First Row) ===");
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
                System.Diagnostics.Debug.WriteLine("=== SETTLEMENT WSQueryHelper.DoQuery ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in Settlement WSQueryHelper.DoQuery: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public static WSQueryReturns GetSTPClearingData(DataEntities db, DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS CLEARING DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"WSQueryPS Clearing received parameters:");
            System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}' (Type: {startDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}' (Type: {endDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}' (Type: {SID?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}' (Type: {Efek?.GetType()})");

            bool isC = false;
            var whereQuery = "1=1";

            System.Diagnostics.Debug.WriteLine("=== BUILDING CLEARING WHERE CLAUSE ===");
            System.Diagnostics.Debug.WriteLine($"Initial whereQuery: {whereQuery}");

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

            System.Diagnostics.Debug.WriteLine("=== FINAL CLEARING SQL QUERY ===");
            System.Diagnostics.Debug.WriteLine($"Complete SQL Query:");
            System.Diagnostics.Debug.WriteLine(props.Query);
            System.Diagnostics.Debug.WriteLine("=== END CLEARING SQL QUERY ===");

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
            System.Diagnostics.Debug.WriteLine("=== WSQueryPS TRANSACTION DEBUG START ===");
            System.Diagnostics.Debug.WriteLine($"WSQueryPS Transaction received parameters:");
            System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}' (Type: {startDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}' (Type: {endDate?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}' (Type: {SID?.GetType()})");
            System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}' (Type: {Efek?.GetType()})");

            bool isC = false;
            var whereQuery = "1=1";

            System.Diagnostics.Debug.WriteLine("=== BUILDING TRANSACTION WHERE CLAUSE ===");
            System.Diagnostics.Debug.WriteLine($"Initial whereQuery: {whereQuery}");

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

                // Build WHERE clause based on date parameters
                if (!string.IsNullOrEmpty(singleDate))
                {
                    whereClause += " AND calendarsk = " + singleDate;
                }
                else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    whereClause += " AND calendarsk >= " + startDate + " AND calendarsk <= " + endDate;
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

                DataTable dt = new DataTable();
                string connString = db.appSettings.DataConnString;

                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;
                        var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Chart data rows: {dt.Rows.Count}");

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
