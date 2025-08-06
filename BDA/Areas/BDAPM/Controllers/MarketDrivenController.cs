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
using System.Globalization;

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
        public PartialViewResult _LeadersAndLaggardsData(string selectedDate, int? topN, string periodType, string endDate = null)
        {
            var pageModel = new LeadersAndLaggardsPageViewModel();

            try
            {
                // Validasi input dasar
                if (string.IsNullOrEmpty(selectedDate))
                {
                    ViewBag.ErrorMessage = "Please select a valid date.";
                    return PartialView("_LeadersAndLaggardsData", pageModel);
                }

                if (string.IsNullOrEmpty(periodType))
                {
                    ViewBag.ErrorMessage = "Please select a period type.";
                    return PartialView("_LeadersAndLaggardsData", pageModel);
                }

                // Validasi format tanggal berdasarkan tipe periode
                bool isValidFormat = false;
                if (periodType == "Daily" || periodType == "Custom Date")
                {
                    isValidFormat = selectedDate.Length == 8 && int.TryParse(selectedDate, out _);
                    if (periodType == "Custom Date")
                    {
                        isValidFormat = isValidFormat && !string.IsNullOrEmpty(endDate) && endDate.Length == 8 && int.TryParse(endDate, out _);
                    }
                }
                else if (periodType == "Monthly")
                {
                    isValidFormat = selectedDate.Length == 6 && int.TryParse(selectedDate, out _);
                }

                if (!isValidFormat)
                {
                    ViewBag.ErrorMessage = $"Invalid date format for {periodType} period. Please select a valid date.";
                    return PartialView("_LeadersAndLaggardsData", pageModel);
                }

                int topCount = topN ?? 10;
                if (topCount <= 0 || topCount > 500)
                {
                    topCount = 10;
                }

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";
                db.InsertAuditTrail("Leaders_Laggards_Data_Request",
                    $"User {userId} requested leaders/laggards data for {periodType} period, date {selectedDate}" + (endDate != null ? $" to {endDate}" : "") + $", top {topCount}",
                    "LeadersAndLaggards");

                // Pemanggilan metode helper sesuai dengan tipe periode
                if (periodType == "Custom Date")
                {
                    pageModel.Leaders = WSQueryPS.GetLeadersOrLaggardsCustomDate(db, selectedDate, endDate, topCount, true);
                    pageModel.Laggards = WSQueryPS.GetLeadersOrLaggardsCustomDate(db, selectedDate, endDate, topCount, false);
                }
                else
                {
                    pageModel.Leaders = WSQueryPS.GetLeadersOrLaggards(db, true, selectedDate, topCount, periodType);
                    pageModel.Laggards = WSQueryPS.GetLeadersOrLaggards(db, false, selectedDate, topCount, periodType);
                }

                if (!pageModel.Leaders.Any() && !pageModel.Laggards.Any())
                {
                    ViewBag.InfoMessage = $"No data available for the selected {periodType.ToLower()}";
                }
                else
                {
                    for (int i = 0; i < pageModel.Leaders.Count; i++)
                    {
                        pageModel.Leaders[i].Sequence = i + 1;
                    }

                    for (int i = 0; i < pageModel.Laggards.Count; i++)
                    {
                        pageModel.Laggards[i].Sequence = i + 1;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error in _LeadersAndLaggardsData: {sqlEx.Message}");
                ViewBag.ErrorMessage = "Database connection error. Please try again later.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in _LeadersAndLaggardsData: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
            }

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
        public PartialViewResult _GetGainersAndLosersData(string selectedDate, int? topN, string periodType, string endDate = null)
        {
            var pageModel = new GainersAndLosersPageViewModel();

            try
            {
                // Input validation
                if (string.IsNullOrEmpty(selectedDate))
                {
                    ViewBag.ErrorMessage = "Please select a valid date.";
                    return PartialView("_GainersAndLosersData", pageModel);
                }

                if (string.IsNullOrEmpty(periodType))
                {
                    ViewBag.ErrorMessage = "Please select a period type.";
                    return PartialView("_GainersAndLosersData", pageModel);
                }

                // Validate date format based on period type
                bool isValidFormat = false;
                if (periodType == "Daily" || periodType == "Custom Date")
                {
                    // Should be YYYYMMDD (8 digits)
                    isValidFormat = selectedDate.Length == 8 && int.TryParse(selectedDate, out _);
                    if (periodType == "Custom Date")
                    {
                        // endDate must also be valid
                        isValidFormat = isValidFormat && !string.IsNullOrEmpty(endDate) && endDate.Length == 8 && int.TryParse(endDate, out _);
                    }
                }
                else if (periodType == "Monthly")
                {
                    // Should be YYYYMM (6 digits)
                    isValidFormat = selectedDate.Length == 6 && int.TryParse(selectedDate, out _);
                }

                if (!isValidFormat)
                {
                    ViewBag.ErrorMessage = $"Invalid date format for {periodType} period. Please select a valid date.";
                    return PartialView("_GainersAndLosersData", pageModel);
                }

                int topCount = topN ?? 10;
                if (topCount <= 0 || topCount > 500)
                {
                    topCount = 10; // Default fallback
                }

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";
                db.InsertAuditTrail("Gainers_Losers_Data_Request",
                    $"User {userId} requested gainers/losers data for {periodType} period, date {selectedDate}" + (endDate != null ? $" to {endDate}" : "") + $", top {topCount}",
                    "GainersVsLosers");

                // Call the helper methods
                if (periodType == "Custom Date")
                {
                    pageModel.Gainers = WSQueryPS.GetGainersOrLosersCustomDate(db, selectedDate, endDate, topCount, true);
                    pageModel.Losers = WSQueryPS.GetGainersOrLosersCustomDate(db, selectedDate, endDate, topCount, false);
                }
                else
                {
                    pageModel.Gainers = WSQueryPS.GetGainersOrLosers(db, true, selectedDate, topCount, periodType);
                    pageModel.Losers = WSQueryPS.GetGainersOrLosers(db, false, selectedDate, topCount, periodType);
                }

                if (!pageModel.Gainers.Any() && !pageModel.Losers.Any())
                {
                    ViewBag.InfoMessage = $"No data available for the selected {periodType.ToLower()}";
                }
                else
                {
                    for (int i = 0; i < pageModel.Gainers.Count; i++)
                    {
                        pageModel.Gainers[i].Sequence = i + 1;
                    }

                    for (int i = 0; i < pageModel.Losers.Count; i++)
                    {
                        pageModel.Losers[i].Sequence = i + 1;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error in _GetGainersAndLosersData: {sqlEx.Message}");
                ViewBag.ErrorMessage = "Database connection error. Please try again later.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in _GetGainersAndLosersData: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
            }

            return PartialView("_GainersAndLosersData", pageModel);
        }

      

        private string FormatDateForDisplay(string dateString, string periodType)
        {
            try
            {
                if (periodType == "Monthly" && dateString.Length == 6)
                {
                    // Format YYYYMM as MM/YYYY
                    if (DateTime.TryParseExact(dateString + "01", "yyyyMMdd", null, DateTimeStyles.None, out DateTime date))
                    {
                        return date.ToString("MM/yyyy");
                    }
                }
                else if ((periodType == "Daily" || periodType == "Custom Date") && dateString.Length == 8)
                {
                    // Format YYYYMMDD as DD/MM/YYYY
                    if (DateTime.TryParseExact(dateString, "yyyyMMdd", null, DateTimeStyles.None, out DateTime date))
                    {
                        return date.ToString("dd/MM/yyyy");
                    }
                }
            }
            catch
            {
                // If parsing fails, return original string
            }
            return dateString;
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

        [HttpGet]
        public object GetSTPBalanceData(DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            try
            {
                // Log the received parameters
                System.Diagnostics.Debug.WriteLine($"Received parameters - Start: {startDate}, End: {endDate}, SID: {SID}, Efek: {Efek}");

                // Call the WSQueryPS helper method directly with the YYYYMMDD formatted dates
                var result = Helper.WSQueryPS.GetSTPBalanceData(db, loadOptions,
                    startDate, endDate, SID, Efek);

                System.Diagnostics.Debug.WriteLine($"Query result count: {result?.data?.Rows?.Count ?? 0}");

                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetSTPBalanceData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        public ActionResult SimpanPenggunaanDataSTP(string id)
        {
            string message = "";
            string Penggunaan_Data = "";
            bool result = true;
            var userId = HttpContext.User.Identity.Name;

            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.InsertAuditTrail("STP_Processing_Akses_Page", "user " + userId + " mengakses halaman STP Processing untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

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

        [HttpGet]
        public object GetSTPSettlementData(DataSourceLoadOptions loadOptions,
            string startDate, string endDate, string SID, string Efek)
        {
            try
            {
                // Log the received parameters
                System.Diagnostics.Debug.WriteLine("=== SETTLEMENT CONTROLLER DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Settlement received parameters - Start: {startDate}, End: {endDate}, SID: {SID}, Efek: {Efek}");
                System.Diagnostics.Debug.WriteLine($"LoadOptions - Skip: {loadOptions.Skip}, Take: {loadOptions.Take}");
                System.Diagnostics.Debug.WriteLine($"LoadOptions - RequireTotalCount: {loadOptions.RequireTotalCount}");

                // Log before calling WSQueryPS
                System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryPS SETTLEMENT ===");
                System.Diagnostics.Debug.WriteLine($"About to call WSQueryPS.GetSTPSettlementData with:");
                System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}'");
                System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}'");
                System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}'");
                System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}'");

                // Call the WSQueryPS helper method
                var result = Helper.WSQueryPS.GetSTPSettlementData(db, loadOptions,
                    startDate, endDate, SID, Efek);

                // Log the result
                System.Diagnostics.Debug.WriteLine("=== WSQueryPS SETTLEMENT RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Query result count: {result?.data?.Rows?.Count ?? 0}");
               

                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== SETTLEMENT CONTROLLER ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in GetSTPSettlementData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
        [HttpGet]
        public object GetSTPTransactionData(DataSourceLoadOptions loadOptions,
        string startDate, string endDate, string SID, string Efek)
        {
            try
            {
                // Log the received parameters
                System.Diagnostics.Debug.WriteLine("=== TRANSACTION CONTROLLER DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Transaction received parameters - Start: {startDate}, End: {endDate}, SID: {SID}, Efek: {Efek}");
                System.Diagnostics.Debug.WriteLine($"LoadOptions - Skip: {loadOptions.Skip}, Take: {loadOptions.Take}");
                System.Diagnostics.Debug.WriteLine($"LoadOptions - RequireTotalCount: {loadOptions.RequireTotalCount}");

                // Log before calling WSQueryPS
                System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryPS TRANSACTION ===");
                System.Diagnostics.Debug.WriteLine($"About to call WSQueryPS.GetSTPTransactionData with:");
                System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}'");
                System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}'");
                System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}'");
                System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}'");

                // Call the WSQueryPS helper method
                var result = Helper.WSQueryPS.GetSTPTransactionData(db, loadOptions,
                    startDate, endDate, SID, Efek);

                // Log the result
                System.Diagnostics.Debug.WriteLine("=== WSQueryPS TRANSACTION RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Query result count: {result?.data?.Rows?.Count ?? 0}");


                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== TRANSACTION CONTROLLER ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in GetSTPTransactionData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
        [HttpGet]
        public object GetSTPClearingData(DataSourceLoadOptions loadOptions,
    string startDate, string endDate, string SID, string Efek)
        {
            try
            {
                // Log the received parameters
                System.Diagnostics.Debug.WriteLine("=== CLEARING CONTROLLER DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Clearing received parameters - Start: {startDate}, End: {endDate}, SID: {SID}, Efek: {Efek}");
                System.Diagnostics.Debug.WriteLine($"LoadOptions - Skip: {loadOptions.Skip}, Take: {loadOptions.Take}");
                System.Diagnostics.Debug.WriteLine($"LoadOptions - RequireTotalCount: {loadOptions.RequireTotalCount}");

                // Log before calling WSQueryPS
                System.Diagnostics.Debug.WriteLine("=== CALLING WSQueryPS CLEARING ===");
                System.Diagnostics.Debug.WriteLine($"About to call WSQueryPS.GetSTPClearingData with:");
                System.Diagnostics.Debug.WriteLine($"  - startDate: '{startDate}'");
                System.Diagnostics.Debug.WriteLine($"  - endDate: '{endDate}'");
                System.Diagnostics.Debug.WriteLine($"  - SID: '{SID}'");
                System.Diagnostics.Debug.WriteLine($"  - Efek: '{Efek}'");

                // Call the WSQueryPS helper method
                var result = Helper.WSQueryPS.GetSTPClearingData(db, loadOptions,
                    startDate, endDate, SID, Efek);

                // Log the result
                System.Diagnostics.Debug.WriteLine("=== WSQueryPS CLEARING RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Query result count: {result?.data?.Rows?.Count ?? 0}");

                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== CLEARING CONTROLLER ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Error in GetSTPClearingData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        public ActionResult ExportSTPPDFServerSide(IFormFile file, string name)
        {
            try
            {
                var userId = HttpContext.User.Identity.Name ?? "unknown";

                if (file != null && file.Length > 0)
                {
                    // Create a unique filename
                    string fileName = $"STP_{name}_{DateTime.Now:yyyyMMdd_HHmmss}_{userId}.pdf";
                    string tempPath = Path.Combine(_env.WebRootPath, "temp");

                    // Ensure temp directory exists
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }

                    string filePath = Path.Combine(tempPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Store file info in session or return file path
                    HttpContext.Session.SetString($"STP_PDF_{name}", fileName);

                    // Log the export
                    try
                    {
                        var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                        var currentNode = mdl.GetCurrentNode();
                        string pageTitle = currentNode != null ? currentNode.Title : "";

                        db.InsertAuditTrail("STP_Processing_Export_PDF", $"user {userId} mengekspor PDF {name}", pageTitle);
                    }
                    catch (Exception logEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Logging error: {logEx.Message}");
                    }

                    System.Diagnostics.Debug.WriteLine($"PDF file saved: {filePath}");
                }

                return Json(new { result = "Success", message = "File processed successfully" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportSTPPDFServerSide: {ex.Message}");
                return Json(new { result = "Error", message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult DownloadSTPPDF(string name)
        {
            try
            {
                string fileName = HttpContext.Session.GetString($"STP_PDF_{name}");

                if (string.IsNullOrEmpty(fileName))
                {
                    return NotFound("File not found");
                }

                string tempPath = Path.Combine(_env.WebRootPath, "temp");
                string filePath = Path.Combine(tempPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found on disk");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Clean up the file after reading
                try
                {
                    System.IO.File.Delete(filePath);
                    HttpContext.Session.Remove($"STP_PDF_{name}");
                }
                catch
                {
                    // Ignore cleanup errors
                }

                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }
        [HttpPost]
        public ActionResult ExportSTPPDFDirect(IFormFile file, string name)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var userId = HttpContext.User.Identity.Name ?? "unknown";
                    string fileName = $"STP_{name}_{DateTime.Now:yyyyMMdd_HHmmss}_{userId}.pdf";

                    // Read the file content
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        var fileBytes = memoryStream.ToArray();

                        // Log the export
                        try
                        {
                            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                            var currentNode = mdl.GetCurrentNode();
                            string pageTitle = currentNode != null ? currentNode.Title : "";

                            db.InsertAuditTrail("STP_Processing_Export_PDF", $"user {userId} mengekspor PDF {name}", pageTitle);
                        }
                        catch (Exception logEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Logging error: {logEx.Message}");
                        }

                        System.Diagnostics.Debug.WriteLine($"PDF direct download: {fileName}, Size: {fileBytes.Length} bytes");

                        // Set proper headers for PDF download
                        Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                        Response.Headers.Add("Content-Type", "application/pdf");
                        Response.Headers.Add("Content-Length", fileBytes.Length.ToString());

                        return File(fileBytes, "application/pdf", fileName);
                    }
                }

                return BadRequest("No file provided");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportSTPPDFDirect: {ex.Message}");
                return BadRequest($"Error processing file: {ex.Message}");
            }
        }

        [HttpGet]
        public JsonResult GetMarketChartData(string chartType = "Value", string startDate = null, string endDate = null, string singleDate = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CONTROLLER CHART DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Controller Chart - Type: {chartType}, Single: {singleDate}, Start: {startDate}, End: {endDate}");

                // Call WSQueryPS method (for first version - direct SQL)
                var result = Helper.WSQueryPS.GetMarketChartData(db, chartType, startDate, endDate, singleDate);

                // OR if using second version with loadOptions:
                // var result = Helper.WSQueryPS.GetMarketChartData(db, chartType, startDate, endDate, singleDate, null);

                // Check if we have valid data
                bool hasData = result?.data != null && result.data.Rows != null && result.data.Rows.Count > 0;

                if (hasData)
                {
                    // Convert DataTable to List for DevExtreme
                    var chartData = new List<object>();

                    foreach (DataRow row in result.data.Rows)
                    {
                        chartData.Add(new
                        {
                            date = row["date"],
                            value = row["value"] ?? 0,
                            calendarsk = row["calendarsk"]
                        });
                    }

                    System.Diagnostics.Debug.WriteLine($"Controller Chart - Returning {chartData.Count} data points");
                    return Json(chartData);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Controller Chart - No data found");
                    return Json(new List<object>());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Controller Chart error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public object GetMarketDrivenSummaryData(string filterDate)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== MARKET DRIVEN SUMMARY DEBUG START ===");
                System.Diagnostics.Debug.WriteLine($"GetMarketDrivenSummaryData received filterDate: '{filterDate}'");

                if (string.IsNullOrEmpty(filterDate))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Filter date is required",
                        data = new
                        {
                            closingvalue = 0,
                            marketcapitalizationamount = 0,
                            net_value = 0,
                            net_volume = 0,
                            calendarsk = ""
                        }
                    });
                }

                // Call the WSQueryPS helper method
                var result = Helper.WSQueryPS.GetMarketDrivenSummaryData(db, filterDate);

                System.Diagnostics.Debug.WriteLine("=== CONTROLLER RESULT ===");
                System.Diagnostics.Debug.WriteLine($"Result data is null: {result?.data == null}");
                System.Diagnostics.Debug.WriteLine($"Data rows count: {result?.data?.Rows?.Count ?? 0}");

                // Check if we have valid data (WSQueryReturns doesn't have 'success' property)
                bool hasData = result?.data != null && result.data.Rows != null && result.data.Rows.Count > 0;

                if (hasData)
                {
                    var row = result.data.Rows[0];
                    var summaryData = new
                    {
                        success = true,
                        data = new
                        {
                            closingvalue = row["closingvalue"] ?? 0,
                            marketcapitalizationamount = row["marketcapitalizationamount"] ?? 0,
                            net_value = row["net_value"] ?? 0,
                            net_volume = row["net_volume"] ?? 0,
                            calendarsk = row["calendarsk"]?.ToString() ?? filterDate
                        }
                    };

                    System.Diagnostics.Debug.WriteLine($"Returning summary data successfully");
                    System.Diagnostics.Debug.WriteLine($"closingvalue: {row["closingvalue"]}");
                    System.Diagnostics.Debug.WriteLine($"marketcapitalizationamount: {row["marketcapitalizationamount"]}");
                    System.Diagnostics.Debug.WriteLine($"net_value: {row["net_value"]}");
                    System.Diagnostics.Debug.WriteLine($"net_volume: {row["net_volume"]}");

                    return Json(summaryData);
                }
                else
                {
                    string errorMessage = "No data found for the specified date";
                  
                    System.Diagnostics.Debug.WriteLine($"No data found, returning error: {errorMessage}");
                    return Json(new
                    {
                        success = false,
                        message = errorMessage,
                        data = new
                        {
                            closingvalue = 0,
                            marketcapitalizationamount = 0,
                            net_value = 0,
                            net_volume = 0,
                            calendarsk = filterDate
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetMarketDrivenSummaryData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new
                    {
                        closingvalue = 0,
                        marketcapitalizationamount = 0,
                        net_value = 0,
                        net_volume = 0,
                        calendarsk = ""
                    }
                });
            }
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