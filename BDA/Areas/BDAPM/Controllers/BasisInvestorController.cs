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
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class BasisInvestorController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public BasisInvestorController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Basis Investor View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Basis Investor Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Basis_Investor_Page", "Akses Page Basis Investor", pageTitle); //simpan kedalam audit trail

            return View();
        }

        public IActionResult Detail(long? id) 
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Detail Basis Investor View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Basis Investor Detail Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Detail_Basis_Investor_Page", "Akses Page Detail Basis Investor", pageTitle); //simpan kedalam audit trail

            return View();
        }
        public IActionResult SumTxSID(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Summary Transaction SID View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Summary Transaction SID Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Summary_Transaction_SID_Page", "Akses Page Summary Transaction SID", pageTitle); //simpan kedalam audit trail

            return View();
        }

        public object GetCardTotalClients(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;

            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (invType != null) 
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null) 
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07TotalClientsQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardActiveClient(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;

            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07ActiveClientQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardTrxFreq(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07TrxFreqQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardtradedValue(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07TradedValueQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardClientLiquidAmt(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07ClientLiquidAmtQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }


        [HttpPost]
        public ActionResult SimpanPenggunaanData(string id)
        {
            string message = "";
            string Penggunaan_Data = "";
            bool result = true;
            var userId = HttpContext.User.Identity.Name;

            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "";
            db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " mengakases halaman Segmentation Summary Cluster MKBD untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

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

        [HttpGet]
        public object GetNamaPE(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            using (SqlConnection conn = new SqlConnection(strSQL))
            {
                conn.Open();
                string strQuery = "Select [SecurityCompanySK],[SecurityCompanyCode],[SecurityCompanyName] from PM_dimSecurityCompanies where CurrentStatus='A' order by SecurityCompanyName asc ";
                SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string namakode = dt.Rows[i]["SecurityCompanyCode"].ToString() + " - " + dt.Rows[i]["SecurityCompanyName"].ToString();
                        list.Add(new NamaPE() { value = dt.Rows[i]["SecurityCompanySK"].ToString(), text = namakode });
                    }

                    return Json(DataSourceLoader.Load(list, loadOptions));
                }
                conn.Close();
                conn.Dispose();
            }
            return DataSourceLoader.Load(list, loadOptions);
        }
        public class NamaPE
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult ExportPDF(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Export Data", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;

                    worksheet.Cells.Columns[9].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "SegmentationSummaryClusterMKBD_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

    }
}
