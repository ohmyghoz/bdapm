using System;
using System.Collections.Generic;
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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Dashboard")]
    public class HMLPeroranganController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public HMLPeroranganController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("HML Perorangan View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("HML Perorangan Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("HMLPerorangan_Akses_Page", "Akses Page Dashboard HML Perorangan", pageTitle);

            return View();
        }

        public bool IsPengawasLJK()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("PengawasLJK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] GetFilteredMemberTypes(string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_type_code).Distinct().ToArray();
            return filter;
        }

        public string[] GetFilteredMembers(string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_code).ToArray();
            return filter;
        }

        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("HML Perorangan Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_HMLPerorangan", "Export Data", pageTitle);
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

                db.CheckPermission("HML Perorangan Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_HMLPerorangan", "Export Data", pageTitle);

                var directory = _env.WebRootPath;

                var timeStamp = DateTime.Now.ToString();

                //Workbook workbook = new Workbook();
                //workbook.Open(file.OpenReadStream(), FileFormatType.Xlsx);
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

                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);

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
                var fileName = "HMLPerorangan_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult File()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "HMLPerorangan_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string hml, /*string mcdfa,*/ string periodeAwal, string periodeAkhir, string jenisDebitur)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] HML = JsonConvert.DeserializeObject<string[]>(hml);
            //string[] MCDFA = JsonConvert.DeserializeObject<string[]>(mcdfa);

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringHML = null;
            string stringMCDFA = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;

            /*check pengawas LJK*/
            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                var filter2 = GetFilteredMembers(login);

                if (MemberTypes.Length == 0)
                {
                    stringMemberTypes = string.Join(", ", filter);
                }

                if (Members.Length == 0)
                {
                    stringMembers = string.Join(", ", filter2);
                }
            }

            if (MemberTypes.Length > 0)
            {
                stringMemberTypes = string.Join(", ", MemberTypes);
            }

            if (Members.Length > 0)
            {
                stringMembers = string.Join(", ", Members);
            }

            if (HML.Length > 0)
            {
                stringHML = string.Join(", ", HML);
            }

            //if (MCDFA.Length > 0)
            //{
            //    stringMCDFA = string.Join(", ", MCDFA);
            //}

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
            }

            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
            }

            db.Database.CommandTimeout = 420;
            var query = db.getHmlMcdfa("hml", "D01", stringMemberTypes, stringMembers, stringHML, stringMCDFA, stringPeriodeAwal, stringPeriodeAkhir, jenisDebitur).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion

        #region "RefGetter"
        public IActionResult GetMemberTypes(DataSourceLoadOptions loadOptions)
        {
            var login = this.User.Identity.Name;

            var query = db.master_ljk_type.Where(x => x.status_aktif == "Y" && x.status_delete == "T").Select(x => new { x.kode_jenis_ljk, Display = x.kode_jenis_ljk + " - " + x.deskripsi_jenis_ljk }).ToList();

            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                query = query.Where(x => filter.Contains(x.kode_jenis_ljk)).ToList();
            }

            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        public IActionResult GetMembers(DataSourceLoadOptions loadOptions, string memberTypes, bool reset)
        {
            var login = this.User.Identity.Name;

            if (reset)
            {
                loadOptions.Skip = 0;
                loadOptions.Take = 0;
            }

            string[] MemberTypes = string.IsNullOrEmpty(memberTypes) ? new string[] { } : memberTypes.Split(",");

            //var query = db.master_ljk.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, Display = x.kode_ljk + " - " + x.nama_ljk }).ToList();
            var query = db.vw_getMasterLJK.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, x.nama_ljk, x.CompositeKey, x.Display }).ToList();

            if (IsPengawasLJK())
            {
                var filter = GetFilteredMembers(login);
                query = query.Where(x => filter.Contains(x.kode_ljk)).ToList();
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }
        #endregion
    }
}
