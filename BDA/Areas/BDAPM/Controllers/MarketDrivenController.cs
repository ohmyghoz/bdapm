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
using BDA.Helper;
using DevExpress.Xpo.DB;

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

        public IActionResult GainersVsLosers()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Gainers_vs_Lossers_Page", "Akses Page Gainers vs Lossers", pageTitle); //simpan kedalam audit trail

            return View();

        }

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
        public object GetMarketData(DataSourceLoadOptions loadOptions, string selectedDate)
        {
            // If no date is selected (initial load), return an empty result.
            if (string.IsNullOrEmpty(selectedDate))
            {
                return new { data = new object[0], totalCount = 0 };
            }

            // Convert the incoming "yyyy-MM-dd" string to "yyyymmdd" for the query
            string dateForQuery = selectedDate.Replace("-", "");

            // The SQL query with a parameter for the date
            string sqlQuery = $@"
        SELECT 
            CONVERT(DATE, CAST(tradedatesk AS VARCHAR(8))) AS TransactionDate,
            sid_ori,
            cpinvestorcode,
            securitycode,
            investorcode,
            CAST(quantity AS BIGINT) AS quantity,
            CAST(value AS BIGINT) AS value
        FROM pasarmodal.market_driven_validasi_data_tra
        WHERE tradedatesk = @tradedate";

            DataTable dt = new DataTable();
            string connString = db.appSettings.DataConnString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.CommandTimeout = 300;

                    // Add the date as a parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@tradedate", dateForQuery);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            // Use DataSourceLoader to apply grid options (like paging, sorting, etc.)
            var loadResult = DataSourceLoader.Load(dt.AsEnumerable(), loadOptions);

            // Return the processed data as a JsonResult
            return Json(loadResult);
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
            var result = WSQueryPS.GetMarketDrivenData(db, loadOptions, selectedDate);

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