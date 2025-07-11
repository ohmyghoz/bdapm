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
using System.Globalization;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MMCancelTransaksiController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MMCancelTransaksiController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            DateTime periodeAwal = DateTime.Now;
            monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMM");
            yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
            ViewData["monthyearawal"] = monthpawal + " " + yearpawal;
            ViewBag.monthyearawal = monthpawal + " " + yearpawal;
            TempData["monthyearawal"] = monthpawal + " " + yearpawal;

            DateTime periodeAkhir = DateTime.Now;
            monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMM");
            yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
            ViewData["monthyearakhir"] = monthpakhir + " " + yearpakhir;
            ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
            TempData["monthyearakhir"] = monthpakhir + " " + yearpakhir;

            db.CheckPermission("Pola 9 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Pola 9 Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Pola_9_Akses_Page", "Akses Page Pola 9", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("Pola_9_Akses_Page", "user " + userId + " mengakases halaman Pola 9", pageTitle);

            return View();
        }
        public IActionResult GetBondTypes(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<Namabondissuertypecode>();

            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterBondTypeInfo(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               bondissuertypecode = bs.Field<string>("bondissuertypecode").ToString().Trim()
                           }).OrderBy(bs => bs.bondissuertypecode).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["bondissuertypecode"].ToString();
                    list.Add(new Namabondissuertypecode() { value = dtList.Rows[i]["bondissuertypecode"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public class Namabondissuertypecode
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public object GetBarChartNumberCancellation(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM09NumberCancellation(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   nobulan = Convert.ToInt32(bs.Field<Int32>("nobulan").ToString()),
                                   bulan = bs.Field<string>("bulan").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderBy(bs => bs.nobulan).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   nobulan = Convert.ToInt32(bs.Field<Int32>("nobulan").ToString()),
                                   bulan = bs.Field<string>("bulan").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                               }).OrderBy(bs => bs.nobulan).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetChartCancellationStatus(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM09BondType(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            return JsonConvert.SerializeObject(result);
        }
        public object GetChartBondType(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM09BondType(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            return JsonConvert.SerializeObject(result);
        }
        public object GetBarChartTop10CancelMarket(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            ViewData["monthyearawal"] = null;
            ViewData["monthyearakhir"] = null;
            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMM");
                yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
                ViewData["monthyearawal"] = monthpawal + " " + yearpawal;
                ViewBag.monthyearawal = monthpawal + " " + yearpawal;
                TempData["monthyearawal"] = monthpawal + " " + yearpawal; 
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMM");
                yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
                ViewData["monthyearakhir"] = monthpakhir + " " + yearpakhir;
                ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
                TempData["monthyearakhir"] = monthpakhir + " " + yearpakhir;
            }

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM08Top10CancelMarket(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   buyerfirmcode = bs.Field<string>("buyerfirmcode").ToString(),
                                   OTHERS = Convert.ToInt64(!string.IsNullOrEmpty(bs.Field<Int64>("OTHERS").ToString()) ? bs.Field<Int64>("OTHERS").ToString() : "0"),
                                   TRADE_CANCEL = Convert.ToInt64(!string.IsNullOrEmpty(bs.Field<Int64>("TRADE_CANCEL").ToString()) ? bs.Field<Int64>("TRADE_CANCEL").ToString() : "0"),
                                   WRONG_INPUT = Convert.ToInt64(!string.IsNullOrEmpty(bs.Field<Int64>("WRONG_INPUT").ToString()) ? bs.Field<Int64>("WRONG_INPUT").ToString() : "0"),
                                   DOUBLE_REPORT = Convert.ToInt64(!string.IsNullOrEmpty(bs.Field<Int64>("DOUBLE_REPORT").ToString()) ? bs.Field<Int64>("DOUBLE_REPORT").ToString() : "0"),
                                   NETWORK_CONNECTION = Convert.ToInt64(!string.IsNullOrEmpty(bs.Field<Int64>("NETWORK_CONNECTION").ToString()) ? bs.Field<Int64>("NETWORK_CONNECTION").ToString() : "0"),
                                   Total = Convert.ToInt64(!string.IsNullOrEmpty(bs.Field<Int64>("Total").ToString()) ? bs.Field<Int64>("Total").ToString() : "0"),
                               }).OrderByDescending(bs => bs.Total).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   buyerfirmcode = bs.Field<string>("buyerfirmcode").ToString(),
                                   OTHERS = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("OTHERS").ToString()) ? bs.Field<Int32>("OTHERS").ToString() : "0"),
                                   TRADE_CANCEL = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("TRADE_CANCEL").ToString()) ? bs.Field<Int32>("TRADE_CANCEL").ToString() : "0"),
                                   WRONG_INPUT = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("WRONG_INPUT").ToString()) ? bs.Field<Int32>("WRONG_INPUT").ToString() : "0"),
                                   DOUBLE_REPORT = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("DOUBLE_REPORT").ToString()) ? bs.Field<Int32>("DOUBLE_REPORT").ToString() : "0"),
                                   NETWORK_CONNECTION = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("NETWORK_CONNECTION").ToString()) ? bs.Field<Int32>("NETWORK_CONNECTION").ToString() : "0"),
                                   Total = Convert.ToInt32(!string.IsNullOrEmpty(bs.Field<Int32>("Total").ToString()) ? bs.Field<Int32>("Total").ToString() : "0"),
                               }).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetBarChartCanceledBonds(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

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

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM09CanceledBonds(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   bondcode = bs.Field<string>("bondcode").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   bondcode = bs.Field<string>("bondcode").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetBarChartReasonCanceledBonds(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

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

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM09ReasonCanceledBonds(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   tradereason = bs.Field<string>("tradereason").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   tradereason = bs.Field<string>("tradereason").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                               }).OrderByDescending(bs => bs.total).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetBarChartDateCanceledBonds(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, string bondissuertypecode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] Statusbondissuertypecode = JsonConvert.DeserializeObject<string[]>(bondissuertypecode);

            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringbondissuertypecode = null;
            string reportId = "mm_bond_trades_cancel"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

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

            if (Statusbondissuertypecode.Length > 0)
            {
                stringbondissuertypecode = string.Join(", ", Statusbondissuertypecode);
                TempData["stringbondissuertypecode"] = stringbondissuertypecode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMMM09DateCanceledBonds(db, loadOptions, reportId, stringPeriodeAwal, stringPeriodeAkhir, stringbondissuertypecode, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   tgl = Convert.ToInt32(bs.Field<string>("tgl").ToString()),
                                   nobulan = Convert.ToInt32(bs.Field<Int32>("nobulan").ToString()),
                                   entrydate = bs.Field<string>("entrydate").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                               }).OrderBy(bs => bs.tgl).OrderBy(bs => bs.nobulan).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   tgl = Convert.ToInt32(bs.Field<string>("tgl").ToString()),
                                   nobulan = Convert.ToInt32(bs.Field<Int32>("nobulan").ToString()),
                                   entrydate = bs.Field<string>("entrydate").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                               }).OrderBy(bs => bs.tgl).OrderBy(bs => bs.nobulan).ToList();
            }
            return JsonConvert.SerializeObject(varDataList);
        }
    }
}
