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
using BDA.Models;

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
            db.InsertAuditTrail("Leader_vs_Laggard_Page", "Akses Page Leaders vs Laggards", pageTitle); //simpan kedalam audit trail

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
            db.InsertAuditTrail("Gainers_vs_Lossers_Page", "Akses Page Gainers vs Losers", pageTitle); //simpan kedalam audit trail

            
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
        // REWRITE: Add detailed System.Diagnostics.Debug logging
        // REWRITE (or ensure this version is in place): robust logging + fallback extraction from Request.Query
        // REWRITE: return a plain array so PivotGrid can consume it directly.
        // Also keep detailed debug already added in your previous version.
        [HttpGet]
        public object GetMarketData(
            DataSourceLoadOptions loadOptions,
            string periodType = null,
            string selectedDate = null,
            string selectedMonth = null,
            string startDate = null,
            string endDate = null,
            string startTime = null,
            string endTime = null,
            string[] confirmation = null,
            string[] lokalAsing = null,
            string[] countryInvestor = null,
            string[] typeInvestor = null,
            string[] market = null,
            string[] abCodes = null,
            int? topN = null
        )
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== MarketDrivenController.GetMarketData START ===");
                System.Diagnostics.Debug.WriteLine($"Route: {Request?.Path.Value}");
                System.Diagnostics.Debug.WriteLine($"Raw QueryString: {Request?.QueryString.Value}");
                foreach (var kv in Request.Query)
                    System.Diagnostics.Debug.WriteLine($"{kv.Key}=[{string.Join(",", kv.Value.ToArray())}]");

                // Fallback extraction (kept from your previous changes) ...
                string Q(string key) => Request.Query.TryGetValue(key, out var v) ? v.ToString() : null;
                string[] QA(string key)
                    => Request.Query.TryGetValue(key, out var v) && v.Count > 0 ? v.ToArray()
                     : (Request.Query.TryGetValue(key + "[]", out var v2) && v2.Count > 0 ? v2.ToArray() : null);

                periodType = string.IsNullOrWhiteSpace(periodType) ? Q("periodType") : periodType;
                selectedDate = string.IsNullOrWhiteSpace(selectedDate) ? Q("selectedDate") : selectedDate;
                selectedMonth = string.IsNullOrWhiteSpace(selectedMonth) ? Q("selectedMonth") : selectedMonth;
                startDate = string.IsNullOrWhiteSpace(startDate) ? Q("startDate") : startDate;
                endDate = string.IsNullOrWhiteSpace(endDate) ? Q("endDate") : endDate;
                startTime = string.IsNullOrWhiteSpace(startTime) ? Q("startTime") : startTime;
                endTime = string.IsNullOrWhiteSpace(endTime) ? Q("endTime") : endTime;
                confirmation ??= QA("confirmation");
                lokalAsing ??= QA("lokalAsing");
                countryInvestor ??= QA("countryInvestor");
                typeInvestor ??= QA("typeInvestor");
                market ??= QA("market");
                abCodes ??= QA("abCodes");
                if (!topN.HasValue && int.TryParse(Q("topN"), out var t)) topN = t;

                System.Diagnostics.Debug.WriteLine("--- Effective Parameters ---");
                System.Diagnostics.Debug.WriteLine($"periodType={periodType}, selectedDate={selectedDate}, selectedMonth={selectedMonth}, startDate={startDate}, endDate={endDate}");
                System.Diagnostics.Debug.WriteLine($"startTime={startTime}, endTime={endTime}, topN={topN}");
                System.Diagnostics.Debug.WriteLine($"confirmation=[{(confirmation == null ? "" : string.Join(",", confirmation))}]");
                System.Diagnostics.Debug.WriteLine($"lokalAsing=[{(lokalAsing == null ? "" : string.Join(",", lokalAsing))}]");
                System.Diagnostics.Debug.WriteLine($"countryInvestor=[{(countryInvestor == null ? "" : string.Join(",", countryInvestor))}]");
                System.Diagnostics.Debug.WriteLine($"typeInvestor=[{(typeInvestor == null ? "" : string.Join(",", typeInvestor))}]");
                System.Diagnostics.Debug.WriteLine($"market=[{(market == null ? "" : string.Join(",", market))}]");
                System.Diagnostics.Debug.WriteLine($"abCodes=[{(abCodes == null ? "" : string.Join(",", abCodes))}]");

                // IMPORTANT: return a flat array for PivotGrid
                var dataArray = Helper.WSQueryPS.GetMarketDrivenData(
                    db, loadOptions,
                    periodType, selectedDate, selectedMonth, startDate, endDate,
                    startTime, endTime, confirmation, lokalAsing, countryInvestor,
                    typeInvestor, market, abCodes, topN
                );

                // dataArray should be a List<Dictionary<...>>
                var count = (dataArray as System.Collections.ICollection)?.Count ?? -1;
                System.Diagnostics.Debug.WriteLine($"=== MarketDrivenController.GetMarketData RESULT: count={count} ===");
                System.Diagnostics.Debug.WriteLine("=== MarketDrivenController.GetMarketData END ===");

                return Json(dataArray); // plain array, no wrapper object
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== MarketDrivenController.GetMarketData ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack  : {ex.StackTrace}");
                return Json(Array.Empty<object>()); // return empty array to PivotGrid
            }
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

        [HttpPost]
        public JsonResult SimpanPenggunaanData(string id, string penggunaanData, string reportTitle)
        {
            string message = "";
            bool result = true;
            var userId = HttpContext.User.Identity.Name;

            try
            {
                // Use the parameters directly
                db.InsertAuditTrail(
                    "MarketDriven_Akses_Page",
                    $"user {userId} mengakses halaman Market Driven untuk digunakan sebagai {penggunaanData}",
                    reportTitle
                );
                result = true;
            }
            catch (Exception ex)
            {
                message = "Saving Failed! " + ex.Message;
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

                // Validate that we have proper date parameters
                bool hasSingleDate = !string.IsNullOrEmpty(singleDate);
                bool hasDateRange = !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate);

                if (!hasSingleDate && !hasDateRange)
                {
                    System.Diagnostics.Debug.WriteLine("Controller Chart - ERROR: No valid date parameters provided");
                    return Json(new { error = "No valid date parameters provided", data = new List<object>() });
                }

                // Call WSQueryPS method
                var result = Helper.WSQueryPS.GetMarketChartData(db, chartType, startDate, endDate, singleDate);

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

                    // Debug: Show first and last dates
                    if (chartData.Count > 0)
                    {
                        var firstPoint = chartData.First() as dynamic;
                        var lastPoint = chartData.Last() as dynamic;
                        System.Diagnostics.Debug.WriteLine($"Controller Chart - Date range: {firstPoint.date} to {lastPoint.date}");
                    }

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
                return Json(new { error = ex.Message, data = new List<object>() });
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

        // Export Gainers to Excel
        [HttpGet]
        public IActionResult ExportGainersToExcel(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Check export permission
              //  db.CheckPermission("Market Driven Export", BDA.DataModel.DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";
                db.InsertAuditTrail("Gainers_Export_Excel",
                    $"User {userId} exported Gainers data to Excel - Period: {periodType}, Date: {selectedDate}, Top: {topN}",
                    "GainersVsLosers");

                List<GainerLoserViewModel> gainers;

                // Get data based on period type
                if (periodType == "Custom Date")
                {
                    gainers = WSQueryPS.GetGainersOrLosersCustomDate(db, selectedDate, endDate, topN, true);
                }
                else
                {
                    gainers = WSQueryPS.GetGainersOrLosers(db, true, selectedDate, topN, periodType);
                }

                // Create Excel workbook
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Gainers";

                // Add headers
                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[0, i].PutValue(headers[i]);
                    // Optional: Set header style if you have a helper
                    // worksheet.Cells[0, i].SetStyle(GetHeaderStyle(workbook));
                }

                // Add data
                for (int i = 0; i < gainers.Count; i++)
                {
                    var gainer = gainers[i];
                    int row = i + 1;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(gainer.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(gainer.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)gainer.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)gainer.Volume);
                    worksheet.Cells[row, 5].PutValue((double)gainer.Turnover);
                    worksheet.Cells[row, 6].PutValue(gainer.Freq);
                    worksheet.Cells[row, 7].PutValue((double)gainer.Price);
                    worksheet.Cells[row, 8].PutValue((double)gainer.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)gainer.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)gainer.Point);
                    worksheet.Cells[row, 11].PutValue((double)gainer.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)gainer.MinPrice);
                }

                // Auto-fit columns
                worksheet.AutoFitColumns();

                // Generate filename
                string dateStr = selectedDate;
                if (periodType == "Monthly" && selectedDate.Length == 6)
                    dateStr = selectedDate;
                if ((periodType == "Daily" || periodType == "Custom Date") && selectedDate.Length == 8)
                    dateStr = selectedDate;

                string filename = $"TopGainers_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Save to memory stream
                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportGainersToExcel: {ex.Message}");
                return BadRequest($"Error exporting to Excel: {ex.Message}");
            }
        }

        // Export Gainers to PDF
        [HttpGet]
        public IActionResult ExportGainersToPDF(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Check export permission
               // db.CheckPermission("Market Driven Export", BDA.DataModel.DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";
                db.InsertAuditTrail("Gainers_Export_PDF",
                    $"User {userId} exported Gainers data to PDF - Period: {periodType}, Date: {selectedDate}, Top: {topN}",
                    "GainersVsLosers");

                List<GainerLoserViewModel> gainers;

                // Get data based on period type
                if (periodType == "Custom Date")
                {
                    gainers = WSQueryPS.GetGainersOrLosersCustomDate(db, selectedDate, endDate, topN, true);
                }
                else
                {
                    gainers = WSQueryPS.GetGainersOrLosers(db, true, selectedDate, topN, periodType);
                }

                // Create Excel workbook (will be converted to PDF)
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Gainers";

                // Add title
                worksheet.Cells.Merge(0, 0, 1, 13);
                worksheet.Cells[0, 0].PutValue($"TOP GAINERS - {selectedDate} ({periodType})");
                // Optional: Set title style if you have a helper

                // Add headers
                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i].PutValue(headers[i]);
                    // Optional: Set header style if you have a helper
                }

                // Add data
                for (int i = 0; i < gainers.Count; i++)
                {
                    var gainer = gainers[i];
                    int row = i + 2;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(gainer.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(gainer.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)gainer.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)gainer.Volume);
                    worksheet.Cells[row, 5].PutValue((double)gainer.Turnover);
                    worksheet.Cells[row, 6].PutValue(gainer.Freq);
                    worksheet.Cells[row, 7].PutValue((double)gainer.Price);
                    worksheet.Cells[row, 8].PutValue((double)gainer.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)gainer.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)gainer.Point);
                    worksheet.Cells[row, 11].PutValue((double)gainer.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)gainer.MinPrice);
                }

                // Auto-fit columns
                worksheet.AutoFitColumns();

                // Generate filename
                string dateStr = selectedDate;
                if (periodType == "Monthly" && selectedDate.Length == 6)
                    dateStr = selectedDate;
                if ((periodType == "Daily" || periodType == "Custom Date") && selectedDate.Length == 8)
                    dateStr = selectedDate;

                string filename = $"TopGainers_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                // Set PDF to landscape and narrow margins
                worksheet.PageSetup.Orientation = Aspose.Cells.PageOrientationType.Landscape;
                worksheet.PageSetup.LeftMargin = 0.25;
                worksheet.PageSetup.RightMargin = 0.25;
                worksheet.PageSetup.TopMargin = 0.25;
                worksheet.PageSetup.BottomMargin = 0.25;

                // Export to PDF
                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Pdf);
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", filename);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportGainersToPDF: {ex.Message}");
                return BadRequest($"Error exporting to PDF: {ex.Message}");
            }
        }

        // Export Losers to Excel
        [HttpGet]
        public IActionResult ExportLosersToExcel(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Check export permission
               // db.CheckPermission("Market Driven Export", BDA.DataModel.DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";
                db.InsertAuditTrail("Losers_Export_Excel",
                    $"User {userId} exported Losers data to Excel - Period: {periodType}, Date: {selectedDate}, Top: {topN}",
                    "GainersVsLosers");

                List<GainerLoserViewModel> losers;

                // Get data based on period type
                if (periodType == "Custom Date")
                {
                    losers = WSQueryPS.GetGainersOrLosersCustomDate(db, selectedDate, endDate, topN, false);
                }
                else
                {
                    losers = WSQueryPS.GetGainersOrLosers(db, false, selectedDate, topN, periodType);
                }

                // Create Excel workbook
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Losers";

                // Add headers
                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[0, i].PutValue(headers[i]);
                    // Optional: Set header style if you have a helper
                    // worksheet.Cells[0, i].SetStyle(GetHeaderStyle(workbook));
                }

                // Add data
                for (int i = 0; i < losers.Count; i++)
                {
                    var loser = losers[i];
                    int row = i + 1;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(loser.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(loser.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)loser.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)loser.Volume);
                    worksheet.Cells[row, 5].PutValue((double)loser.Turnover);
                    worksheet.Cells[row, 6].PutValue(loser.Freq);
                    worksheet.Cells[row, 7].PutValue((double)loser.Price);
                    worksheet.Cells[row, 8].PutValue((double)loser.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)loser.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)loser.Point);
                    worksheet.Cells[row, 11].PutValue((double)loser.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)loser.MinPrice);
                }

                // Auto-fit columns
                worksheet.AutoFitColumns();

                // Generate filename
                string dateStr = selectedDate;
                if (periodType == "Monthly" && selectedDate.Length == 6)
                    dateStr = selectedDate;
                if ((periodType == "Daily" || periodType == "Custom Date") && selectedDate.Length == 8)
                    dateStr = selectedDate;

                string filename = $"TopLosers_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Save to memory stream
                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportLosersToExcel: {ex.Message}");
                return BadRequest($"Error exporting to Excel: {ex.Message}");
            }
        }

        // Export Losers to PDF
        [HttpGet]
        public IActionResult ExportLosersToPDF(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Check export permission
               // db.CheckPermission("Market Driven Export", BDA.DataModel.DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";
                db.InsertAuditTrail("Losers_Export_PDF",
                    $"User {userId} exported Losers data to PDF - Period: {periodType}, Date: {selectedDate}, Top: {topN}",
                    "GainersVsLosers");

                List<GainerLoserViewModel> losers;

                // Get data based on period type
                if (periodType == "Custom Date")
                {
                    losers = WSQueryPS.GetGainersOrLosersCustomDate(db, selectedDate, endDate, topN, false);
                }
                else
                {
                    losers = WSQueryPS.GetGainersOrLosers(db, false, selectedDate, topN, periodType);
                }

                // Create Excel workbook (will be converted to PDF)
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Losers";

                // Add title
                worksheet.Cells.Merge(0, 0, 1, 13);
                worksheet.Cells[0, 0].PutValue($"TOP LOSERS - {selectedDate} ({periodType})");
                // Optional: Set title style if you have a helper

                // Add headers
                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i].PutValue(headers[i]);
                    // Optional: Set header style if you have a helper
                }

                // Add data
                for (int i = 0; i < losers.Count; i++)
                {
                    var loser = losers[i];
                    int row = i + 2;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(loser.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(loser.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)loser.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)loser.Volume);
                    worksheet.Cells[row, 5].PutValue((double)loser.Turnover);
                    worksheet.Cells[row, 6].PutValue(loser.Freq);
                    worksheet.Cells[row, 7].PutValue((double)loser.Price);
                    worksheet.Cells[row, 8].PutValue((double)loser.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)loser.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)loser.Point);
                    worksheet.Cells[row, 11].PutValue((double)loser.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)loser.MinPrice);
                }

                // Auto-fit columns
                worksheet.AutoFitColumns();

                // Generate filename
                string dateStr = selectedDate;
                if (periodType == "Monthly" && selectedDate.Length == 6)
                    dateStr = selectedDate;
                if ((periodType == "Daily" || periodType == "Custom Date") && selectedDate.Length == 8)
                    dateStr = selectedDate;

                string filename = $"TopLosers_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                // Set PDF to landscape and narrow margins
                worksheet.PageSetup.Orientation = Aspose.Cells.PageOrientationType.Landscape;
                worksheet.PageSetup.LeftMargin = 0.25;
                worksheet.PageSetup.RightMargin = 0.25;
                worksheet.PageSetup.TopMargin = 0.25;
                worksheet.PageSetup.BottomMargin = 0.25;

                // Export to PDF
                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Pdf);
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", filename);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ExportLosersToPDF: {ex.Message}");
                return BadRequest($"Error exporting to PDF: {ex.Message}");
            }
        }

        // Export Leaders to Excel
        [HttpGet]
        public IActionResult ExportLeadersToExcel(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Fetch data for leaders (isLeader: true)

                var userId = HttpContext.User.Identity.Name ?? "Anonymous";

                db.InsertAuditTrail("Leaders_Export_Excel",

                    $"User {userId} exported Leaders data to Excel - Period: {periodType}, Date: {selectedDate}, Top: {topN}",

                    "Leaders vs Laggards");

                List<LeaderLaggardViewModel> leaders;
                if (periodType == "Custom Date")
                {
                    leaders = WSQueryPS.GetLeadersOrLaggardsCustomDate(db, selectedDate, endDate, topN, true);
                }
                else
                {
                    leaders = WSQueryPS.GetLeadersOrLaggards(db, true, selectedDate, topN, periodType);
                }

                // Create Excel workbook
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Leaders";

                // Add headers
                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[0, i].PutValue(headers[i]);
                }

                // Add data
                for (int i = 0; i < leaders.Count; i++)
                {
                    var leader = leaders[i];
                    int row = i + 1;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(leader.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(leader.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)leader.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)leader.Volume);
                    worksheet.Cells[row, 5].PutValue((double)leader.Turnover);
                    worksheet.Cells[row, 6].PutValue(leader.Freq);
                    worksheet.Cells[row, 7].PutValue((double)leader.Price);
                    worksheet.Cells[row, 8].PutValue((double)leader.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)leader.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)leader.Point);
                    worksheet.Cells[row, 11].PutValue((double)leader.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)leader.MinPrice);
                }

                worksheet.AutoFitColumns();

                string dateStr = selectedDate;
                string filename = $"TopLeaders_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);
                stream.Position = 0;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting to Excel: {ex.Message}");
            }
        }

        // Export Leaders to PDF
        [HttpGet]
        public IActionResult ExportLeadersToPDF(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Fetch data for leaders (isLeader: true)
                var userId = HttpContext.User.Identity.Name ?? "Anonymous";

                db.InsertAuditTrail("Leaders_Export_PDF",

                   $"User {userId} exported Leaders data to PDF - Period: {periodType}, Date: {selectedDate}, Top: {topN}",

                   "Leaders vs Laggards");

                List<LeaderLaggardViewModel> leaders;
                if (periodType == "Custom Date")
                {
                    leaders = WSQueryPS.GetLeadersOrLaggardsCustomDate(db, selectedDate, endDate, topN, true);
                }
                else
                {
                    leaders = WSQueryPS.GetLeadersOrLaggards(db, true, selectedDate, topN, periodType);
                }

                // Create Excel workbook (will be converted to PDF)
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Leaders";

                worksheet.Cells.Merge(0, 0, 1, 13);
                worksheet.Cells[0, 0].PutValue($"TOP LEADERS - {selectedDate} ({periodType})");

                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i].PutValue(headers[i]);
                }

                for (int i = 0; i < leaders.Count; i++)
                {
                    var leader = leaders[i];
                    int row = i + 2;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(leader.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(leader.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)leader.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)leader.Volume);
                    worksheet.Cells[row, 5].PutValue((double)leader.Turnover);
                    worksheet.Cells[row, 6].PutValue(leader.Freq);
                    worksheet.Cells[row, 7].PutValue((double)leader.Price);
                    worksheet.Cells[row, 8].PutValue((double)leader.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)leader.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)leader.Point);
                    worksheet.Cells[row, 11].PutValue((double)leader.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)leader.MinPrice);
                }

                worksheet.AutoFitColumns();

                // Landscape and narrow margins
                worksheet.PageSetup.Orientation = Aspose.Cells.PageOrientationType.Landscape;
                worksheet.PageSetup.LeftMargin = 0.25;
                worksheet.PageSetup.RightMargin = 0.25;
                worksheet.PageSetup.TopMargin = 0.25;
                worksheet.PageSetup.BottomMargin = 0.25;

                string dateStr = selectedDate;
                string filename = $"TopLeaders_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Pdf);
                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", filename);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting to PDF: {ex.Message}");
            }
        }

        // Export Laggards to Excel
        [HttpGet]
        public IActionResult ExportLaggardsToExcel(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Fetch data for laggards (isLeader: false)
                var userId = HttpContext.User.Identity.Name ?? "Anonymous";

                db.InsertAuditTrail("Laggards_Export_Excel",

                    $"User {userId} exported Laggards data to Excel - Period: {periodType}, Date: {selectedDate}, Top: {topN}",

                    "Leaders vs Laggards");


                List<LeaderLaggardViewModel> laggards;
                if (periodType == "Custom Date")
                {
                    laggards = WSQueryPS.GetLeadersOrLaggardsCustomDate(db, selectedDate, endDate, topN, false);
                }
                else
                {
                    laggards = WSQueryPS.GetLeadersOrLaggards(db, false, selectedDate, topN, periodType);
                }

                // Create Excel workbook
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Laggards";

                // Add headers
                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[0, i].PutValue(headers[i]);
                }

                // Add data
                for (int i = 0; i < laggards.Count; i++)
                {
                    var laggard = laggards[i];
                    int row = i + 1;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(laggard.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(laggard.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)laggard.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)laggard.Volume);
                    worksheet.Cells[row, 5].PutValue((double)laggard.Turnover);
                    worksheet.Cells[row, 6].PutValue(laggard.Freq);
                    worksheet.Cells[row, 7].PutValue((double)laggard.Price);
                    worksheet.Cells[row, 8].PutValue((double)laggard.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)laggard.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)laggard.Point);
                    worksheet.Cells[row, 11].PutValue((double)laggard.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)laggard.MinPrice);
                }

                worksheet.AutoFitColumns();

                string dateStr = selectedDate;
                string filename = $"TopLaggards_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);
                stream.Position = 0;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting to Excel: {ex.Message}");
            }
        }

        // Export Laggards to PDF
        [HttpGet]
        public IActionResult ExportLaggardsToPDF(string selectedDate, int topN, string periodType, string endDate = null)
        {
            try
            {
                // Fetch data for laggards (isLeader: false)
                var userId = HttpContext.User.Identity.Name ?? "Anonymous";

                db.InsertAuditTrail("Laggards_Export_PDF",

                    $"User {userId} exported Laggards data to PDF - Period: {periodType}, Date: {selectedDate}, Top: {topN}",

                    "Leaders vs Laggards");

                List<LeaderLaggardViewModel> laggards;

                if (periodType == "Custom Date")
                {
                    laggards = WSQueryPS.GetLeadersOrLaggardsCustomDate(db, selectedDate, endDate, topN, false);
                }
                else
                {
                    laggards = WSQueryPS.GetLeadersOrLaggards(db, false, selectedDate, topN, periodType);
                }

                // Create Excel workbook (will be converted to PDF)
                var workbook = new Aspose.Cells.Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "Top Laggards";

                worksheet.Cells.Merge(0, 0, 1, 13);
                worksheet.Cells[0, 0].PutValue($"TOP LAGGARDS - {selectedDate} ({periodType})");

                var headers = new string[] {
            "Rank", "Security Code", "Security Name", "Change %", "Volume",
            "Turnover", "Frequency", "Price", "Net Value", "Net Volume",
            "Point", "Max Price", "Min Price"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i].PutValue(headers[i]);
                }

                for (int i = 0; i < laggards.Count; i++)
                {
                    var laggard = laggards[i];
                    int row = i + 2;

                    worksheet.Cells[row, 0].PutValue(i + 1);
                    worksheet.Cells[row, 1].PutValue(laggard.SecurityCode);
                    worksheet.Cells[row, 2].PutValue(laggard.SecurityName);
                    worksheet.Cells[row, 3].PutValue((double)laggard.ChangePercentage);
                    worksheet.Cells[row, 4].PutValue((long)laggard.Volume);
                    worksheet.Cells[row, 5].PutValue((double)laggard.Turnover);
                    worksheet.Cells[row, 6].PutValue(laggard.Freq);
                    worksheet.Cells[row, 7].PutValue((double)laggard.Price);
                    worksheet.Cells[row, 8].PutValue((double)laggard.NetValue);
                    worksheet.Cells[row, 9].PutValue((double)laggard.NetVolume);
                    worksheet.Cells[row, 10].PutValue((double)laggard.Point);
                    worksheet.Cells[row, 11].PutValue((double)laggard.MaxPrice);
                    worksheet.Cells[row, 12].PutValue((double)laggard.MinPrice);
                }

                worksheet.AutoFitColumns();

                // Landscape and narrow margins
                worksheet.PageSetup.Orientation = Aspose.Cells.PageOrientationType.Landscape;
                worksheet.PageSetup.LeftMargin = 0.25;
                worksheet.PageSetup.RightMargin = 0.25;
                worksheet.PageSetup.TopMargin = 0.25;
                worksheet.PageSetup.BottomMargin = 0.25;

                string dateStr = selectedDate;
                string filename = $"TopLaggards_{dateStr}_{topN}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                var stream = new MemoryStream();
                workbook.Save(stream, Aspose.Cells.SaveFormat.Pdf);
                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", filename);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting to PDF: {ex.Message}");
            }
        }

        [HttpGet]
        public object GetInvestorGridData(DataSourceLoadOptions loadOptions)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== INVESTOR GRID DATA DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"GetInvestorGridData called with loadOptions");

                // Get filter date from session or request parameters
                string filterDate = Request.Query["filterDate"].FirstOrDefault() ??
                                   HttpContext.Session.GetString("CurrentFilterDate");

                System.Diagnostics.Debug.WriteLine($"Filter date for investor grid: {filterDate}");

                if (string.IsNullOrEmpty(filterDate))
                {
                    // Return empty result if no filter date
                    return DataSourceLoader.Load(new List<object>(), loadOptions);
                }

                // Call WSQueryPS method to get data
                var result = Helper.WSQueryPS.GetInvestorGridData(db, filterDate, loadOptions);

                if (result?.data != null && result.data.Rows.Count > 0)
                {
                    // Convert DataTable to List for DevExtreme
                    var gridData = new List<object>();
                    int rowId = 1;

                    foreach (DataRow row in result.data.Rows)
                    {
                        gridData.Add(new
                        {
                            rowid = rowId++,
                            investorcode = row["investorcode"]?.ToString() ?? "",
                            cpinvestorcode = row["cpinvestorcode"]?.ToString() ?? "",
                            value = Convert.ToDecimal(row["value"] ?? 0),
                            quantity = Convert.ToDecimal(row["quantity"] ?? 0)
                        });
                    }

                    System.Diagnostics.Debug.WriteLine($"Returning {gridData.Count} investor records");

                    // Apply DevExtreme operations (sorting, paging, filtering)
                    return DataSourceLoader.Load(gridData, loadOptions);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No investor data found");
                    return DataSourceLoader.Load(new List<object>(), loadOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetInvestorGridData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                return DataSourceLoader.Load(new List<object>(), loadOptions);
            }
        }
        [HttpGet]
        public object GetSecurityGridData(DataSourceLoadOptions loadOptions)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== SECURITY GRID DATA DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"GetSecurityGridData called with loadOptions");

                // Get filter date from session or request parameters
                string filterDate = Request.Query["filterDate"].FirstOrDefault() ??
                                   HttpContext.Session.GetString("CurrentFilterDate");

                // Get transaction code from session if available
                string transactionCode = Request.Query["transactionCode"].FirstOrDefault() ??
                                        HttpContext.Session.GetString("CurrentTransactionCode");

                System.Diagnostics.Debug.WriteLine($"Filter date for security grid: {filterDate}");
                System.Diagnostics.Debug.WriteLine($"Transaction code for security grid: {transactionCode}");

                if (string.IsNullOrEmpty(filterDate))
                {
                    // Return empty result if no filter date
                    return DataSourceLoader.Load(new List<object>(), loadOptions);
                }

                // Call WSQueryPS method to get security data
                var result = Helper.WSQueryPS.GetSecurityGridData(db, filterDate, transactionCode, loadOptions);

                if (result?.data != null && result.data.Rows.Count > 0)
                {
                    // Convert DataTable to List for DevExtreme
                    var gridData = new List<object>();
                    int rowId = 1;

                    foreach (DataRow row in result.data.Rows)
                    {
                        gridData.Add(new
                        {
                            rowid = rowId++,
                            securitycode = row["securitycode"]?.ToString() ?? "",
                            value = Convert.ToDecimal(row["value"] ?? 0),
                            quantity = Convert.ToDecimal(row["quantity"] ?? 0),
                            frequency = Convert.ToInt32(row["frequency"] ?? 0)
                        });
                    }

                    System.Diagnostics.Debug.WriteLine($"Returning {gridData.Count} security records");

                    // Apply DevExtreme operations (sorting, paging, filtering)
                    return DataSourceLoader.Load(gridData, loadOptions);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No security data found");
                    return DataSourceLoader.Load(new List<object>(), loadOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetSecurityGridData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                return DataSourceLoader.Load(new List<object>(), loadOptions);
            }
        }

        // Helper method to set filter date for grids
        [HttpPost]
        public JsonResult SetGridFilterDate(string filterDate, string transactionCode = null)
        {
            try
            {
                // Store filter parameters in session for grid data methods
                HttpContext.Session.SetString("CurrentFilterDate", filterDate);
                if (!string.IsNullOrEmpty(transactionCode))
                {
                    HttpContext.Session.SetString("CurrentTransactionCode", transactionCode);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public object GetCPInvestorGridData(DataSourceLoadOptions loadOptions)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CP INVESTOR GRID DATA DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"GetCPInvestorGridData called with loadOptions");

                // Get filter date from session or request parameters
                string filterDate = Request.Query["filterDate"].FirstOrDefault() ??
                                   HttpContext.Session.GetString("CurrentFilterDate");

                // Get transaction code from session if available
                string transactionCode = Request.Query["transactionCode"].FirstOrDefault() ??
                                        HttpContext.Session.GetString("CurrentTransactionCode");

                System.Diagnostics.Debug.WriteLine($"Filter date for CP Investor grid: {filterDate}");
                System.Diagnostics.Debug.WriteLine($"Transaction code for CP Investor grid: {transactionCode}");

                if (string.IsNullOrEmpty(filterDate))
                {
                    // Return empty result if no filter date
                    return DataSourceLoader.Load(new List<object>(), loadOptions);
                }

                // Call WSQueryPS method to get CP Investor data
                var result = Helper.WSQueryPS.GetCPInvestorGridData(db, filterDate, transactionCode, loadOptions);

                if (result?.data != null && result.data.Rows.Count > 0)
                {
                    // Convert DataTable to List for DevExtreme
                    var gridData = new List<object>();
                    int rowId = 1;

                    foreach (DataRow row in result.data.Rows)
                    {
                        gridData.Add(new
                        {
                            rowid = rowId++,
                            cpinvestorcode = row["cpinvestorcode"]?.ToString() ?? "",
                            cptradeid = row["cpinvestorcode"]?.ToString() ?? "", // Same as cpinvestorcode as per your requirement
                            value = Convert.ToDecimal(row["value"] ?? 0),
                            quantity = Convert.ToDecimal(row["quantity"] ?? 0)
                        });
                    }

                    System.Diagnostics.Debug.WriteLine($"Returning {gridData.Count} CP Investor records");

                    // Apply DevExtreme operations (sorting, paging, filtering)
                    return DataSourceLoader.Load(gridData, loadOptions);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No CP Investor data found");
                    return DataSourceLoader.Load(new List<object>(), loadOptions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetCPInvestorGridData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                return DataSourceLoader.Load(new List<object>(), loadOptions);
            }
        }

        [HttpPost]
        public JsonResult LogExportAuditTrail(string gridName, string exportType, string filterSummary)
        {
            try
            {
                var userId = User.Identity.Name ?? "Unknown";
                string actionName = $"{gridName}_export_{exportType}";
                string logMessage = $"User {userId} exported {gridName} to {exportType.ToUpper()} with filter: {filterSummary}";
                db.InsertAuditTrail(actionName, logMessage, "Assessmen Pasar Equity");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Helper method for Excel header styling (place inside MarketDrivenController)
        private static Aspose.Cells.Style GetHeaderStyle(Aspose.Cells.Workbook workbook)
        {
            var style = workbook.CreateStyle();
            style.Font.IsBold = true;
            style.ForegroundColor = System.Drawing.Color.LightGray;
            style.Pattern = Aspose.Cells.BackgroundType.Solid;
            return style;
        }

        // Helper for PDF title style (optional, for PDF export)
        private static Aspose.Cells.Style GetTitleStyle(Aspose.Cells.Workbook workbook)
        {
            var style = workbook.CreateStyle();
            style.Font.IsBold = true;
            style.Font.Size = 16;
            style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            return style;
        }

        // Helper for filename formatting
        private string FormatDateForFilename(string dateString, string periodType)
        {
            if (periodType == "Monthly" && dateString.Length == 6)
                return dateString;
            if ((periodType == "Daily" || periodType == "Custom Date") && dateString.Length == 8)
                return dateString;
            return dateString;
        }



    }

 
}