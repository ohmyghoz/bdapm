using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using BDA.Helper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using static BDA.Controllers.RefController;
using System.Security.Policy;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using System.Text.RegularExpressions;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Xpo.DB.Helpers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class IPRelationController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public class RefDto
        {
            public string kode { get; set; }
            public string text { get; set; }
        }

        public IPRelationController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        
        public IActionResult Index(string id, string detailsid)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            
            string pageTitle = currentNode != null ? currentNode.Title : ""; 
            TempData["pageTitle"] = pageTitle;            
            ViewBag.Export = false; // TODO ubah disini
            var obj = db.osida_master.Find(id);
            db.InsertAuditTrail("PM_" + id + "_Akses_Page", "Akses Page Dashboard " + obj.menu_nama, pageTitle);
            db.CheckPermission(obj.menu_nama + " View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Export = true;
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, id);
            ViewBag.Hive = isHive;
            string roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value.ToString();
            ViewBag.FullFilter = false;
            ViewBag.IsExternal = false;
            if (roleId == "Pengawas TE" || roleId == "AdminAplikasi" || roleId == "Admin PM") ViewBag.FullFilter = true;
            if (roleId == "External") ViewBag.IsExternal = true;

            if (obj == null) return NoContent();
            
            
            if (detailsid == null)
            {
                ViewBag.period = System.DateTime.Now;
                //ViewBag.endperiod = null;
                ViewBag.sid = "";
                ViewBag.sistem = null;
            }
            else
            {
                var details = detailsid.ToString().Split("~");

                DateTime p = DateTime.ParseExact(details[0], "yyyyMMdd", CultureInfo.InvariantCulture);//Convert.ToDateTime(details[0]);
                ViewBag.period = p.Year == 9999? System.DateTime.Now: p;

                string sistem = details[1].ToUpper();//.Replace("Balance");
                if (sistem == "DTE") sistem = "CBEST";
                else if (sistem == "CBEST") sistem = "C-BEST";
                ViewBag.sistem = sistem;
                ViewBag.sid = details[2];
            }
            
            
            TempData["kodeReport"] = obj.kode;
            
            if (obj.kode == "HALT")
            {
                return View("index2",obj);
            }
            else {
                return View(obj);
            }
            
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
            db.InsertAuditTrail("IP_Akses_Page", "user " + userId + " mengakases halaman Investor Profile untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

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
        public IActionResult LogExportIndex(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = reportId; // currentNode != null ? currentNode.Title : "";
                //pageTitle = TempData.Peek("pageTitle").ToString();
                //TODO : tambah permission
                //db.CheckPermission("OSIDA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var obj = db.osida_master.Find(reportId);
                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_PM_" + reportId, "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult ExportPDF(string reportId,IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = reportId; //currentNode != null ? currentNode.Title : "";
                //pageTitle = TempData.Peek("pageTitle").ToString();
                //TODO : tambah permission
                //db.CheckPermission("OSIDA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var obj = db.osida_master.Find(reportId);
                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_PM_" + reportId, "Export Data", pageTitle);

                var directory = _env.WebRootPath;

                var timeStamp = DateTime.Now.ToString();

                //Workbook workbook = new Workbook();
                //workbook.Open(file.OpenReadStream(), FileFormatType.Xlsx);
                Workbook workbook = new Workbook(file.OpenReadStream());

                //if (reportId == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
                    Worksheet worksheet2 = workbook.Worksheets[0];
                    var columns1 = worksheet2.Cells.Columns.Count;
                    var rows1 = worksheet2.Cells.Rows.Count;
                    var style = workbook.CreateStyle();
                    style.SetBorder(BorderType.TopBorder, CellBorderType.Medium, Color.Black);
                    style.SetBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);
                    style.SetBorder(BorderType.LeftBorder, CellBorderType.Medium, Color.Black);
                    style.SetBorder(BorderType.RightBorder, CellBorderType.Medium, Color.Black);
                    //Apply bottom borders from cell F4 till K4
                    for (int r = 0; r <= rows1 - 1; r++)
                    {
                        for (int col = 0; col <= columns1 - 1; col++)
                        {
                            Cell cell = worksheet2.Cells[r, col];

                            cell.SetStyle(style);
                        }
                    }
                }
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
                    Style numericStyle = workbook.CreateStyle();
                    numericStyle.Custom = "#,##0.00";
                    numericStyle.HorizontalAlignment = TextAlignmentType.Right;

                    numericStyle.SetBorder(BorderType.TopBorder, CellBorderType.Medium, Color.Black);
                    numericStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);
                    numericStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Medium, Color.Black);
                    numericStyle.SetBorder(BorderType.RightBorder, CellBorderType.Medium, Color.Black);

                    StyleFlag numericFlag = new StyleFlag();
                    numericFlag.NumberFormat = true;
                    numericFlag.HorizontalAlignment = true;


                    foreach (Cell cell in worksheet.Cells)
                    {
                        if (cell.Type == CellValueType.IsNumeric)
                        {
                            cell.SetStyle(numericStyle);
                        }
                    }

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
                var fileName = "PM_" + reportId + "_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult File(string reportId)
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();            
            var fileName = "PM_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        public JsonResult getTime()
        {
            string se1 = "Waktu Proses : " + TempData.Peek("SG").ToString() + " detik \nWaktu tunggu Hive / SQL : " + TempData.Peek("SD").ToString() + " detik \nPenggunaan Memory Webserver: " + TempData.Peek("PM").ToString() + "KB";
            return Json(se1);
        }

        public IActionResult GetSistem(DataSourceLoadOptions loadOptions, string reportId, string tipe)
        {
            //var q = db.macro_penetrasi_lending_ljk.Select(x => new { text = x.dm_jenis_debitur, kode = x.dm_jenis_debitur }).Distinct();
            var list = new List<RefDto>();
            string query;
            if (tipe == "security")
            {
                query = @"Select securitycode as ItemValue, CONCAT(status, ' - ', securitycode, ' - ', securityname) as ItemText from dbo.pm_master_securities";
            }
            else if (tipe == "ExchangeMember")
            {
                query = @"Select exchangemembercode as ItemValue, CONCAT(exchangemembercode, ' - ', exchangemembername) as ItemText from dbo.pm_master_exchange_members";
            }
            else
            {
                query = @"Select ItemText, ItemValue from dbo.ref_items where TableName = '" + reportId + @"' and RefType = '" + tipe + @"'";
            }

            DataTable dt = Helper.WSQueryStore.GetSystemDropdown(db, query).data;
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new RefDto() { text = dr["ItemText"].ToString(), kode = dr["ItemValue"].ToString() });
            }

            return Json(DataSourceLoader.Load(list, loadOptions));
        }

        public object GetNamaSID(DataSourceLoadOptions loadOptions, string namaSID = "adi")
        {
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "sid_nama");
            var result = Helper.WSQueryStore.GetNamaSIDQuery(db, loadOptions, namaSID, cekHive);
            return JsonConvert.SerializeObject(result);            
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string reportId, string startPeriode, string endPeriode, 
            string SID, string tradeId, string namaSID, string namaLike, string kolomRel, float nilaiRel,
            string bSID, string bTradeId, string bExchangeCode, string sSID, string sTradeId, string sExchangeCode, string securityCode,
            string nomorKTP, string nomorNPWP, string exchangeCode, string invType, string invOrigin, string invClass,
            bool chk100)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "";
            string filterValue = "";
            bool valid = false;
            if (reportId == "ip_rel_sid") filterValue = "sid=" + SID + ";trade=" + tradeId + ";nama=" + namaSID + ";namamirip=" + namaLike + ";kolom=" + kolomRel + ";nilai=" + nilaiRel + ";periode=" + startPeriode + " - " + endPeriode + ";";
            else if (reportId == "ip_rel_transaction") filterValue = "bsid=" + bSID + ";btrade=" + bTradeId + ";bExchange=" + bExchangeCode + "ssid=" + sSID + ";strade=" + sTradeId + ";sExchange=" + sExchangeCode + ";security=" + securityCode + ";periode=" + startPeriode + " - " + endPeriode + ";";
            else filterValue = "sid=" + SID + ";trade=" + tradeId + ";nama=" + namaSID + ";namamirip=" + namaLike + ";ktp=" + nomorKTP + ";npwp=" + nomorNPWP + ";exchange=" + exchangeCode + ";security=" + securityCode + ";periode=" + startPeriode + " - " + endPeriode + ";";
            db.InsertAuditTrail("IP_Akses_Page", "user " + userId + " mengakases " + reportId + "; " + filterValue + "", pageTitle);

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear();
            
            string stringStartPeriode = null;
            string stringEndPeriode = null;

            if (startPeriode != null)
            {
                stringStartPeriode = Convert.ToDateTime(startPeriode).ToString("yyyyMMdd");
                TempData["sPeriod"] = stringStartPeriode;
            }

            if (endPeriode != null)
            {
                stringEndPeriode = Convert.ToDateTime(endPeriode).ToString("yyyyMMdd");
                TempData["ePeriod"] = stringEndPeriode;
            }

            if (reportId == "ip_rel_sid" && (SID != null || tradeId != null || namaSID != null || namaLike != null)) valid = true;
            else if (reportId == "ip_rel_transaction" && (bSID != null || bTradeId != null || sSID != null || sTradeId != null)) valid = true;
            else if (reportId == "ip_rel_ownership" && (SID != null || tradeId != null || nomorKTP != null || nomorNPWP != null || namaSID != null || namaLike != null)) valid = true;

            if (valid && startPeriode != null)
                {
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                cekHive = false;
                
                loadOptions.RequireTotalCount = false;
                var result = Helper.WSQueryStore.GetPMIPRelQuery(db, loadOptions, reportId, stringStartPeriode, stringEndPeriode, SID, tradeId, namaSID, kolomRel, nilaiRel, bSID, bTradeId, bExchangeCode, sSID, sTradeId, sExchangeCode, securityCode, nomorKTP, nomorNPWP, exchangeCode, invType, invOrigin, invClass, chk100, cekHive);
                loadOptions.RequireTotalCount = true;
                return JsonConvert.SerializeObject(result);

            }
            else {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        
        #endregion

        
    }
}
