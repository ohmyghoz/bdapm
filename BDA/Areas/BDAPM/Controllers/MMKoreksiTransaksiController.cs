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
            db.InsertAuditTrail("Pola_Koreksi_Transaksi_Akses_Page", "Akses Page Pola Koreksi Per Transaksi / Pola Transaksi", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("Pola_Koreksi_Transaksi_Akses_Page", "user " + userId + " mengakases halaman Pola Koreksi Per Transaksi / Pola Transaksi", pageTitle);

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
            var result = Helper.WSQueryStore.GetBDAPMMM08Top10AmendMarket(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringAmandedtypeinfo, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_firm_id = bs.Field<string>("amended_firm_id").ToString(),
                                   Market = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("Market").ToString()) ? bs.Field<Int32>("Market").ToString() : "0"),
                                   Non_Market = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("Non_Market").ToString()) ? bs.Field<Int32>("Non_Market").ToString() : "0"),
                               }).OrderByDescending(bs => bs.Non_Market).ToList().Take(10);
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   amended_firm_id = bs.Field<string>("amended_firm_id").ToString(),
                                   Market = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("Market").ToString()) ? bs.Field<Int32>("Market").ToString() : "0"),
                                   Non_Market = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("Non_Market").ToString()) ? bs.Field<Int32>("Non_Market").ToString() : "0"),
                               }).OrderByDescending(bs => bs.Non_Market).ToList().Take(10);
            }
            return JsonConvert.SerializeObject(varDataList);
        }
    }
}
