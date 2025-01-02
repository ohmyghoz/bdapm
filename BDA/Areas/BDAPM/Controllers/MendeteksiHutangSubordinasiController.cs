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
using Microsoft.IdentityModel.Tokens;
using static BDA.Controllers.GeospasialInvestorController;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MendeteksiHutangSubordinasiController
        : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MendeteksiHutangSubordinasiController(DataEntities db, IWebHostEnvironment env)
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

            db.CheckPermission("Hutang Subordinasi View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            db.InsertAuditTrail("MendeteksiHutangSubordinasi_Akses_Page", "Akses Page Hutang Subordinasi", pageTitle); //simpan kedalam audit trail

            return View();
        }

        [HttpGet]
        public object getGrid1(DataSourceLoadOptions loadOptions, string periode, string pe)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            
            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMMendeteksiHutangSubordinasiHSO(db, loadOptions, stringPeriodeAwal, stringNamaPE);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        [HttpGet]
        public object getGrid2(DataSourceLoadOptions loadOptions, string periode, string pe)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;

            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMMendeteksiHutangSubordinasiReverseRepo(db, loadOptions, stringPeriodeAwal, stringNamaPE);
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
            db.InsertAuditTrail("MendeteksiHutangSubordinasi_Akses_Page", "user " + userId + " mengakases halaman Mendeteksi Hutang Subordinasi untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

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
            list.Add(new NamaPE() { value = "", text = "(ALL)" });

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
        
        public FileResult FileIndex(string name)
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "MendeteksiHutangSubOrdinasi_" + name + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Hutang Subordinasi Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("MendeteksiHutangSubOrdinasi_Akses_Page", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult ExportPDF(IFormFile file, string name)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Hutang Subordinasi Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("MendeteksiHutangSubOrdinasi_Akses_Page", "Export Data", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
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
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;

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
                var fileName = "MendeteksiHutangSubOrdinasi_" + name + timeStamp + ".pdf";
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
