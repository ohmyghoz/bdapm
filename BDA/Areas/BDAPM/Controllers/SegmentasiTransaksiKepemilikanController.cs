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

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class SegmentasiTransaksiKepemilikanController
        : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public SegmentasiTransaksiKepemilikanController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public bool IsPengawasPM()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("PengawasPM")) //cek jika role Pengawas PM
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu
            ViewBag.Hive = false;

            db.CheckPermission("Segmentasi Transaksi Dan Kepemilikan View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            db.InsertAuditTrail("SegmentasiTransaksiKepemilikan_Akses_Page", "Akses Page Segmentasi Dan Transaksi MKBD", pageTitle); //simpan kedalam audit trail

            return View();
        }

        public object getPieliquidHC(DataSourceLoadOptions loadOptions, string periode, string pe, string jenisTransaksi)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            
            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMSegmentasiTransaksiLHC(db, loadOptions, periode, pe, jenisTransaksi);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        public object getPieliquidIM(DataSourceLoadOptions loadOptions, string periode, string pe, string jenisTransaksi)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMSegmentasiTransaksiLIM(db, loadOptions, periode, pe, jenisTransaksi);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        public object getPieliquidNK(DataSourceLoadOptions loadOptions, string periode, string pe, string jenisTransaksi)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMSegmentasiTransaksiLNK(db, loadOptions, periode, pe, jenisTransaksi);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        public object getGridSegmentasiTransaksiKepemilikan(DataSourceLoadOptions loadOptions, string periode, string pe, string jenisTransaksi)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMSegmentasiTransaksiGrid(db, loadOptions, periode, pe, jenisTransaksi);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
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
            db.InsertAuditTrail("SegmentasiTransaksi_Akses_Page", "user " + userId + " mengakases halaman Segmentasi Transaksi Dan Kepemilikan untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

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
                        list.Add(new NamaPE() { value = dt.Rows[i]["SecurityCompanyCode"].ToString(), text = namakode });
                    }

                    return Json(DataSourceLoader.Load(list, loadOptions));
                }
                conn.Close();
                conn.Dispose();
            }
            return DataSourceLoader.Load(list, loadOptions);
        }
        [HttpGet]
        public object GetJenisData(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            list.Add(new NamaPE { value = "1", text = "Transaksi" });
            list.Add(new NamaPE { value = "2", text = "Kepemilikan" });
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

                db.CheckPermission("Segmentasi Transaksi Dan Kepemilikan Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SegmentasiTransaksiKepemilikan_Akses_Page", "Export Data", pageTitle);
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

                db.CheckPermission("Segmentasi Transaksi Dan Kepemilikan", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.CheckPermission("Segmentasi Transaksi Kepemilikan", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SegmentasiTransaksiKepemilikan_Akses_Page", "Export Data", pageTitle);

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
                var fileName = "SegmentasiTransaksiKepemilikan_" + timeStamp + ".pdf";
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
