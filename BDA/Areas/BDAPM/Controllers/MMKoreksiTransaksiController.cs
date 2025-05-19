using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static BDA.Controllers.SampleGridController;
using System.Xml.Linq;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using Ionic.Zip;
using System.Web;
using System.Reflection;
using DevExpress.Xpo.DB;
using DevExpress.Charts.Native;
using static DevExpress.Data.ODataLinq.Helpers.ODataLinqHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MMKoreksiTransaksiController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MMKoreksiTransaksiController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu
            ViewBag.monthyearawal = "Mei 2025";
            db.CheckPermission("Pola Koreksi Per Transaksi / Pola Transaksi View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Pola Koreksi Per Transaksi / Pola Transaksi Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Pola_Koreksi_Pola_Transaksi_Akses_Page", "Akses Page Pola Koreksi Per Transaksi / Pola Transaksi", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("Pola_Koreksi_Pola_Transaksi_Akses_Page", "user " + userId + " mengakases halaman Pola Koreksi Per Transaksi / Pola Transaksi", pageTitle);

            return View();
        }
        public IActionResult GetMemberTypes(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaAmandedTypeInfo>();

            string reportId = "mm_bond_trades_amended"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterAmandedTypeInfo(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               amended_info_type = bs.Field<string>("amended_info_type").ToString().Trim()
                           }).OrderBy(bs => bs.amended_info_type).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["amended_info_type"].ToString();
                    list.Add(new NamaAmandedTypeInfo() { value = dtList.Rows[i]["amended_info_type"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public class NamaAmandedTypeInfo
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public object GetChartTypeInfo(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string amandedtypeinfo)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusAmandedTypeInfo = JsonConvert.DeserializeObject<string[]>(amandedtypeinfo);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringAmandedtypeinfo = null;
            string reportId = "mm_bond_trades_amended"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["StringPeriodeAwal"] = stringPeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                TempData["StringPeriodeAkhir"] = stringPeriodeAkhir;
            }

            if (StatusAmandedTypeInfo.Length > 0)
            {
                stringAmandedtypeinfo = string.Join(", ", StatusAmandedTypeInfo);
                TempData["StringAmandedTypeInfo"] = stringAmandedtypeinfo;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM08TypeInfo(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringAmandedtypeinfo, cekHive);
            return JsonConvert.SerializeObject(result);
        }
        public object GetChartTypeFirmID(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string amandedtypeinfo)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusAmandedTypeInfo = JsonConvert.DeserializeObject<string[]>(amandedtypeinfo);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringAmandedtypeinfo = null;
            string reportId = "mm_bond_trades_amended"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["StringPeriodeAwal"] = stringPeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                TempData["StringPeriodeAkhir"] = stringPeriodeAkhir;
            }

            if (StatusAmandedTypeInfo.Length > 0)
            {
                stringAmandedtypeinfo = string.Join(", ", StatusAmandedTypeInfo);
                TempData["StringAmandedTypeInfo"] = stringAmandedtypeinfo;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM08TypeFirmID(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringAmandedtypeinfo, cekHive);
            return JsonConvert.SerializeObject(result);
        }
        public object GetBarChartAmendMarket(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string amandedtypeinfo)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusAmandedTypeInfo = JsonConvert.DeserializeObject<string[]>(amandedtypeinfo);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringAmandedtypeinfo = null;
            string reportId = "mm_bond_trades_amended"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMM");
                yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
                ViewBag.monthyearawal = monthpawal + " " + yearpawal;
                TempData["StringPeriodeAwal"] = stringPeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMM");
                yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
                ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
                TempData["StringPeriodeAkhir"] = stringPeriodeAkhir;
            }

            if (StatusAmandedTypeInfo.Length > 0)
            {
                stringAmandedtypeinfo = string.Join(", ", StatusAmandedTypeInfo);
                TempData["StringAmandedTypeInfo"] = stringAmandedtypeinfo;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM08AmendMarket(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringAmandedtypeinfo, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_info = bs.Field<string>("amended_info").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_info = bs.Field<string>("amended_info").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetBarChartNonAmendMarket(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string amandedtypeinfo)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusAmandedTypeInfo = JsonConvert.DeserializeObject<string[]>(amandedtypeinfo);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringAmandedtypeinfo = null;
            string reportId = "mm_bond_trades_amended"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMMM");
                yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
                ViewBag.monthyearawal = monthpawal + " " + yearpawal;
                TempData["monthyearawal"] = monthpawal + " " + yearpawal;
                TempData["StringPeriodeAwal"] = stringPeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMMM");
                yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
                ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
                TempData["StringPeriodeAkhir"] = stringPeriodeAkhir;
            }

            if (StatusAmandedTypeInfo.Length > 0)
            {
                stringAmandedtypeinfo = string.Join(", ", StatusAmandedTypeInfo);
                TempData["StringAmandedTypeInfo"] = stringAmandedtypeinfo;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM08AmendNonMarket(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringAmandedtypeinfo, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_info = bs.Field<string>("amended_info").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_info = bs.Field<string>("amended_info").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetBarChartTop10AmendMarket(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string amandedtypeinfo)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusAmandedTypeInfo = JsonConvert.DeserializeObject<string[]>(amandedtypeinfo);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringAmandedtypeinfo = null;
            string reportId = "vw_GetBDAPMMM08Top10AmendMarket"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMMM");
                yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
                ViewBag.monthyearawal = monthpawal + " " + yearpawal;
                TempData["monthyearawal"] = monthpawal + " " + yearpawal;
                TempData["StringPeriodeAwal"] = stringPeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMMM");
                yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
                ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
                TempData["StringPeriodeAkhir"] = stringPeriodeAkhir;
            }

            if (StatusAmandedTypeInfo.Length > 0)
            {
                stringAmandedtypeinfo = string.Join(", ", StatusAmandedTypeInfo);
                TempData["StringAmandedTypeInfo"] = stringAmandedtypeinfo;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM08Top10AmendMarket(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringAmandedtypeinfo, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_firm_id = bs.Field<string>("amended_firm_id").ToString(),
                                   amended_info_type = bs.Field<string>("amended_info_type").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_info_type = bs.Field<string>("amended_info_type").ToString(),
                                   B_AGRO = bs.Field<Int32>("B-AGRO"),
                                   B_ANZP = bs.Field<Int32>("B-ANZP"),
                                   B_BALI = bs.Field<Int32>("B-BALI"),
                                   B_BBSI = bs.Field<Int32>("B-BBSI"),
                                   B_BCA = bs.Field<Int32>("B-BCA"),
                                   B_BDMN = bs.Field<Int32>("B-BDMN"),
                                   B_BMDR = bs.Field<Int32>("B-BMDR"),
                                   B_BNGA = bs.Field<Int32>("B-BNGA"),
                                   B_BNI = bs.Field<Int32>("B-BNI"),
                                   B_BOFA = bs.Field<Int32>("B-BOFA"),
                                   B_BRI = bs.Field<Int32>("B-BRI"),
                                   B_CBNA = bs.Field<Int32>("B-CBNA"),
                                   B_DBAG = bs.Field<Int32>("B-DBAG"),
                                   B_DBSB = bs.Field<Int32>("B-DBSB"),
                                   B_HNBN = bs.Field<Int32>("B-HNBN"),
                                   B_HSBC = bs.Field<Int32>("B-HSBC"),
                                   B_JPMB = bs.Field<Int32>("B-JPMB"),
                                   B_MEGA = bs.Field<Int32>("B-MEGA"),
                                   B_NISP = bs.Field<Int32>("B-NISP"),
                                   B_SCBI = bs.Field<Int32>("B-SCBI"),
                                   C_BNGA = bs.Field<Int32>("C-BNGA"),
                                   C_CBNA = bs.Field<Int32>("C-CBNA"),
                                   S_AH = bs.Field<Int32>("S-AH"),
                                   S_AI = bs.Field<Int32>("S-AI"),
                                   S_AO = bs.Field<Int32>("S-AO"),
                                   S_AR = bs.Field<Int32>("S-AR"),
                                   S_AZ = bs.Field<Int32>("S-AZ"),
                                   S_BZ = bs.Field<Int32>("S-BZ"),
                                   S_CC = bs.Field<Int32>("S-CC"),
                                   S_CD = bs.Field<Int32>("S-CD"),
                                   S_CP = bs.Field<Int32>("S-CP"),
                                   S_DH = bs.Field<Int32>("S-DH"),
                                   S_DR = bs.Field<Int32>("S-DR"),
                                   S_DX = bs.Field<Int32>("S-DX"),
                                   S_ID = bs.Field<Int32>("S-ID"),
                                   S_IF = bs.Field<Int32>("S-IF"),
                                   S_KI = bs.Field<Int32>("S-KI"),
                                   S_LG = bs.Field<Int32>("S-LG"),
                                   S_LH = bs.Field<Int32>("S-LH"),
                                   S_MG = bs.Field<Int32>("S-MG"),
                                   S_MI = bs.Field<Int32>("S-MI"),
                                   S_NI = bs.Field<Int32>("S-NI"),
                                   S_OD = bs.Field<Int32>("S-OD"),
                                   S_PD = bs.Field<Int32>("S-PD"),
                                   S_PP = bs.Field<Int32>("S-PP"),
                                   S_RO = bs.Field<Int32>("S-RO"),
                                   S_RS = bs.Field<Int32>("S-RS"),
                                   S_SQ = bs.Field<Int32>("S-SQ"),
                                   S_YP = bs.Field<Int32>("S-YP"),
                                   S_YU = bs.Field<Int32>("S-YU"),
                                   S_ZR = bs.Field<Int32>("S-ZR"),
                               }).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
    }
}
