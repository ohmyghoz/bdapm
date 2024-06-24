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
    [Area("BDA2")]
    public class CoverageMapController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public CoverageMapController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        [HttpPost]
        public IActionResult Antrian(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringCak = null;
                string stringProv = null;
                string stringKota = null;
                string stringPeriode = null;
                if (reportId == "coverage_map_ljkxcity")
                {
                    newq.rgq_nama = "Coverage Map Export CSV";
                    if(TempData.Peek("mt") != null) {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("cak") != null)
                    {
                        stringCak = TempData.Peek("cak").ToString();
                    }
                    if (TempData.Peek("prov") != null)
                    {
                        stringProv = TempData.Peek("prov").ToString();
                    }
                    if (TempData.Peek("kota") != null)
                    {
                        stringKota = TempData.Peek("kota").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetCoverageMapQuery(db,stringMemberTypes,stringMembers, stringCak,stringProv,stringKota, stringPeriode,isHive);
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_CM_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            ViewBag.id = "coverage_map_ljkxcity";
            ViewBag.Export = true; // TODO ubah permission disini
            List<string> listP = new List<string>();
            listP.Add(string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)));
            //DateTime a = new DateTime(2021, 1, 1);
            TempData["periode"] = listP.ToArray();
            //TempData["periode"] = string.Format("{0:yyyy-MM-01}", a);
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "coverage_map_ljkxcity");
            //bool isHive = false;
            ViewBag.Hive = isHive;
            db.InsertAuditTrail("CM_coverage_map_ljkxcity_Akses_Page", "Akses Page Dashboard CM01", pageTitle);
            //db.CheckPermission("CM01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            //ViewBag.Export = db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.NoMessage);
            return View();


        }
        
        [HttpPost]
        public IActionResult LogExportIndex(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                //db.CheckPermission("MA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "coverage_map_ljkxcity")
                {
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                
                db.InsertAuditTrail("ExportIndex_CM_" + reportId, "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult ExportPDF(string reportId, IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                if (reportId == "coverage_map_ljkxcity")
                {
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                
                db.InsertAuditTrail("ExportIndex_CM_" + reportId, "Export Data", pageTitle);

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
                var fileName = "CM_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "CM_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string cak,string prov,string kota, string periode, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0) {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] caks = JsonConvert.DeserializeObject<string[]>(cak);
                string[] provs = JsonConvert.DeserializeObject<string[]>(prov);
                string[] kotas = JsonConvert.DeserializeObject<string[]>(kota);
                
                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                //if (members != null)
                //{
                //    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                //}
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringCak = null;
                string stringProv = null;
                string stringKota = null;
                string stringPeriode = null;
                //List<DateTime> lp = new List<DateTime>();
                //foreach (var i in periodes)
                //{
                //    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                //}
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "coverage_map_ljkxcity");
                //cekHive = false;
                //cekHive = true;
                /*check pengawas LJK*/
                if (RefController.IsPengawasLJK(db))
                {
                    var filter = RefController.GetFilteredMemberTypes(db, login);
                    var filter2 = RefController.GetFilteredMembers(db, login);

                    if (MemberTypes != null)
                    {
                        stringMemberTypes = string.Join(", ", filter);
                    }

                    if (Members != null)
                    {
                        stringMembers = string.Join(", ", filter2);
                    }
                }

                if (MemberTypes.Length > 0)
                {
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    var find = listOfJenis.Where(x => MemberTypes.Contains(x.kode_jenis_ljk));
                    foreach (var i in find)
                    {
                        if (stringMemberTypes != "") stringMemberTypes += ", ";
                        stringMemberTypes += i.deskripsi_jenis_ljk;
                    }
                    TempData["mt"] = stringMemberTypes;
                }

                if (Members.Length > 0)
                {
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }

                if (caks.Length > 0)
                {
                    stringCak = string.Join(", ", caks);
                    TempData["cak"] = stringCak;
                }
                if (kotas.Length > 0)
                {
                    stringKota = string.Join(", ", kotas);
                    TempData["kota"] = stringKota;
                }
                if (provs.Length > 0)
                {
                    stringProv = string.Join(", ", provs);
                    TempData["prov"] = stringProv;
                }
                if (periodes.Length > 0)
                {
                    List<string> p1 = new List<string>();
                    DateTime? period1 = null;
                    foreach (var i in periodes)
                    {
                        period1 = Convert.ToDateTime(i);
                        if (cekHive == true)
                        {
                            p1.Add(string.Format("{0:yyyyMM}", period1));
                        }
                        else
                        {
                            p1.Add(string.Format("{0:yyyy-MM-dd}", period1));
                        }
                    }
                    stringPeriode = string.Join(", ", p1.ToArray());
                    TempData["p"] = stringPeriode;
                }
                var result2 = Helper.WSQueryStore.GetCoverageMapQuery(db, loadOptions, stringMemberTypes, stringMembers, stringCak,stringProv,stringKota, stringPeriode, tipeChart, cekHive, isChart);

                return JsonConvert.SerializeObject(result2);

            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }
        #endregion

    }
}
