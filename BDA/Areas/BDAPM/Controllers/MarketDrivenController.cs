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
using System.Configuration;
using BDA.Areas.BDAPM.Models;
using BDA.Helper;
using System.Xml.Linq;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MarketDrivenController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MarketDrivenController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            // Display the value of pageTitle
            Console.WriteLine(pageTitle);

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("MarketDriven_Akses_Page", "Akses Page Market Driven", pageTitle); //simpan kedalam audit trail

            return View();
        }

        public ActionResult AjaxRequest()
        {
            return View();
        }


        public IActionResult LeaderVsLaggard()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Leader_vs_Laggard_Page", "Akses Page Leader vs Laggard", pageTitle); //simpan kedalam audit trail

            return View();

        }

        [HttpPost]
        public PartialViewResult _LeadersAndLaggardsData(string selectedDate, int? topN)
        {
            var pageModel = new LeadersAndLaggardsPageViewModel();

            if (string.IsNullOrEmpty(selectedDate))
            {
                return PartialView("_LeadersAndLaggardsData", pageModel);
            }

            int topCount = topN ?? 10;

            // Call the new helper method from WSQueryPS for both lists
            pageModel.Leaders = WSQueryPS.GetLeadersOrLaggards(db, true, selectedDate, topCount); // true = Gainers
            pageModel.Laggards = WSQueryPS.GetLeadersOrLaggards(db, false, selectedDate, topCount); // false = Losers

            return PartialView("_LeadersAndLaggardsData", pageModel);
        }


        public IActionResult GainersVsLosers()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Gainers_vs_Lossers_Page", "Akses Page Gainers vs Lossers", pageTitle); //simpan kedalam audit trail

            
            // 5. Pass the single page model (containing both lists) to the view
            return View();
        }


        [HttpPost]
        public PartialViewResult _GetGainersAndLosersData(string selectedDate, int? topN)
        {
            var pageModel = new GainersAndLosersPageViewModel();

            if (string.IsNullOrEmpty(selectedDate))
            {
                return PartialView("_GainersAndLosersData", pageModel);
            }

            int topCount = topN ?? 10;

            // Call the new helper method from WSQueryPS for both lists
            pageModel.Gainers = WSQueryPS.GetGainersOrLosers(db, true, selectedDate, topCount); // true = Gainers
            pageModel.Losers = WSQueryPS.GetGainersOrLosers(db, false, selectedDate, topCount); // false = Losers

            return PartialView("_GainersAndLosersData", pageModel);
        }

        // I've created a private helper method to avoid duplicating the database logic
        

        public IActionResult PerkembanganTransaksiNG()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Perkembangan_Transaksi_Negosiasi", "Akses Page Perkembangan Transaksi Negosiasi", pageTitle); //simpan kedalam audit trail

            return View();

        }

        public IActionResult ValidasiDataTransaksi()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("ValidasiDataTransaksi_Akses_Page", "Akses Page Validasi Data Transaksi", pageTitle); //simpan kedalam audit trail

            return View();

        }

        public IActionResult AssessmentPasarEquity()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Assessment_Pasar_Equity", "Akses Page Assessment Pasar Equity", pageTitle); //simpan kedalam audit trail

            return View();

        }
        public IActionResult STPProcessing()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("STP_Processing", "Akses Page STP Processing", pageTitle); //simpan kedalam audit trail

            return View();

        }

        public IActionResult MDTest()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("MDTest", "Akses Page MDP Test", pageTitle); //simpan kedalam audit trail

            return View();

        }

        [HttpGet]
        public object GetMarketData(DataSourceLoadOptions loadOptions, string selectedDate) // Changed from DateTime?
        {
            // Pass the date STRING down to the data helper
            var result = Helper.WSQueryPS.GetMarketDrivenData(db, loadOptions, selectedDate);

            return Json(result);
        }

        public ActionResult SimpanPenggunaanData(string id)
        {
            string message = "";
            string Penggunaan_Data = "";
            bool result = true;
            var userId = HttpContext.User.Identity.Name;

            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "";
            db.InsertAuditTrail("MarketDriven_Akses_Page", "user " + userId + " mengakses halaman Market Driven untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

            try
            {
                string strSQL = db.appSettings.DataConnString;
                using (SqlConnection conn = new SqlConnection(strSQL))
                {
                    conn.Open();
                    string strQuery = "Select * from MasterPenggunaanData where id=" + id + " order by id asc ";
                    SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        Penggunaan_Data = dt.Rows[0]["Penggunaan_Data"].ToString();
                    }
                    conn.Close();
                    conn.Dispose();
                }
                result = true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                message = "Saving Failed !, " + " " + errMsg;
                result = false;
            }
            return Json(new { message, success = result }, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public ActionResult TopCompaniesByValue()
        {
            // Your permission checks can go here
            return View();
        }

        // This action is called by the DataGrid's MVC data source.
        // In MarketDrivenController.cs

        [HttpGet]
        public object _GetTopCompaniesData(DataSourceLoadOptions loadOptions, string selectedDate)
        {
            if (string.IsNullOrEmpty(selectedDate))
            {
                return DataSourceLoader.Load(new List<object>(), loadOptions);
            }

            // This query is from your WSQueryPS file
            string sqlQuery = @"
        SET ARITHABORT ON;
        SELECT TOP 10
            security_code,
            SUM(value) as total_value,
            SUM(volume) as total_volume,
            SUM(freq) as total_freq
        FROM BDAPM.pasarmodal.market_driven_rg_ng
        WHERE periode_lvl1 = @Periode
        GROUP BY security_code
        ORDER BY total_value DESC;";

            var dataList = new List<object>();
            string connString = db.appSettings.DataConnString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.Parameters.AddWithValue("@Periode", selectedDate);

                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);

                        dataList = dt.AsEnumerable().Select(row => new {
                            security_code = row.Field<string>("security_code"),
                            total_value = Convert.ToDecimal(row["total_value"]),
                            total_volume = Convert.ToInt64(row["total_volume"]),
                            total_freq = Convert.ToInt64(row["total_freq"])
                        }).ToList<object>();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DATABASE ERROR: " + ex.Message);
            }

            // This line guarantees the grid gets data in the correct format
            return DataSourceLoader.Load(dataList, loadOptions);
        }
        [HttpGet]
        public object _GetChartData(string selectedDate, string securityCode)
        {
            if (string.IsNullOrEmpty(selectedDate) || string.IsNullOrEmpty(securityCode))
            {
                return new List<object>();
            }

            string sqlQuery = @"
        SELECT 
            RIGHT(periode, 2) as periode,  -- Extract DD from YYYYMMDD
            security_code,
            ISNULL(SUM(CASE WHEN market = 'NG' THEN volume ELSE 0 END), 0) as volume_ng,
            ISNULL(MAX(CASE WHEN market = 'NG' THEN high END), 0) as high_ng,
            ISNULL(MIN(CASE WHEN market = 'NG' THEN low END), 0) as low_ng,
            ISNULL(MAX(CASE WHEN market = 'RG' THEN high END), 0) as high_rg,
            ISNULL(MIN(CASE WHEN market = 'RG' THEN low END), 0) as low_rg
        FROM BDAPM.pasarmodal.market_driven_rg_ng m 
        WHERE periode_lvl1 = @Periode AND m.history_type = 'Date' AND m.security_code = @security_code
        GROUP BY periode, security_code
        ORDER BY periode;";

            var dataList = new List<object>();
            string connString = db.appSettings.DataConnString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.Parameters.AddWithValue("@Periode", selectedDate);
                        cmd.Parameters.AddWithValue("@security_code", securityCode);

                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);

                        System.Diagnostics.Debug.WriteLine($"Chart query returned {dt.Rows.Count} rows for {securityCode}");

                        dataList = dt.AsEnumerable().Select(row => new {
                            periode = row.Field<string>("periode"),
                            security_code = row.Field<string>("security_code"),
                            volume_ng = Convert.ToInt64(row["volume_ng"] ?? 0),
                            high_ng = Convert.ToDecimal(row["high_ng"] ?? 0),
                            low_ng = Convert.ToDecimal(row["low_ng"] ?? 0),
                            high_rg = Convert.ToDecimal(row["high_rg"] ?? 0),
                            low_rg = Convert.ToDecimal(row["low_rg"] ?? 0)
                        }).ToList<object>();

                        // Log the data for debugging
                        System.Diagnostics.Debug.WriteLine($"Data for {securityCode}: {Newtonsoft.Json.JsonConvert.SerializeObject(dataList)}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Chart DATABASE ERROR for {securityCode}: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            return dataList;
        }
        public ActionResult SimpanPenggunaanDataVDT(string id)
        {
            string message = "";
            string Penggunaan_Data = "";
            bool result = true;
            var userId = HttpContext.User.Identity.Name;

            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "";


            db.InsertAuditTrail("ValidasiDataTransaksi_Akses_Page", "user " + userId + " mengakses halaman Validasi Data Transaksi untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

            try
            {
                string strSQL = db.appSettings.DataConnString;
                using (SqlConnection conn = new SqlConnection(strSQL))
                {
                    conn.Open();
                    string strQuery = "Select * from MasterPenggunaanData where id=" + id + " order by id asc ";
                    SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        Penggunaan_Data = dt.Rows[0]["Penggunaan_Data"].ToString();
                    }
                    conn.Close();
                    conn.Dispose();
                }
                result = true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                message = "Saving Failed !, " + " " + errMsg;
                result = false;
            }
            return Json(new { message, success = result }, new Newtonsoft.Json.JsonSerializerSettings());
        }



    }
}