using Aspose.Cells;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BDA.Controllers
{
    [Area("BDA2")]
    public class LAController : Controller
    {

        private DataEntities db;
        private IWebHostEnvironment _env;

        public LAController(DataEntities db, IWebHostEnvironment env)
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
                string stringVariable1 = null;
                string stringVariable2 = null;
                string stringPeriode = null;
                if (reportId == "la_inquiry_check")
                {
                    newq.rgq_nama = "LA01 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("k") != null)
                    {
                        stringVariable1 = TempData.Peek("k").ToString();
                    }
                    if (TempData.Peek("sp") != null)
                    {
                        stringVariable2 = TempData.Peek("sp").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetLA_InquiryCheckQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringVariable2, stringPeriode, isHive);
                    db.CheckPermission("LA01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "la_inquiry_pattern_recognition")
                {
                    newq.rgq_nama = "LA02 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("si") != null)
                    {
                        stringVariable1 = TempData.Peek("si").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetLA_InquiryPatternRecognitionQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("LA02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_LA_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult Index(string id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            ViewBag.id = id;
            ViewBag.Export = false; // TODO ubah permission disini
            List<string> listP = new List<string>();
            //listP.Add(string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)));
            TempData["periode"] = string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)).Split(",");
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, id);
            ViewBag.Hive = isHive;
            if (id == "la_inquiry_check")
            {
                db.InsertAuditTrail("LA_" + id + "_Akses_Page", "Akses Page Dashboard LA01", pageTitle);
                db.CheckPermission("LA01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("LA01 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("InquiryCheck");
            }
            else if (id == "la_inquiry_pattern_recognition")
            {
                db.InsertAuditTrail("LA_" + id + "_Akses_Page", "Akses Page Dashboard LA02", pageTitle);
                db.CheckPermission("LA02 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("LA02 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("InquiryPatternRecognition");
            }
            else
            {
                return View();
            }

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
                //db.CheckPermission("LA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "la_inquiry_check")
                {
                    db.CheckPermission("LA01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "la_inquiry_pattern_recognition")
                {
                    db.CheckPermission("LA02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_LA_" + reportId, "Export Data", pageTitle);
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
                //db.CheckPermission("Macro Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "la_inquiry_check")
                {
                    db.CheckPermission("LA01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "la_inquiry_pattern_recognition")
                {
                    db.CheckPermission("LA02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_LA_" + reportId, "Export Data", pageTitle);

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
                var fileName = "LA_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "LA_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridDataLA_InquiryCheck(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kolektibilitass, string statusPengecekans, string periode, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0) {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] Kolektibilitass = JsonConvert.DeserializeObject<string[]>(kolektibilitass);
                string[] StatusPengecekans = JsonConvert.DeserializeObject<string[]>(statusPengecekans);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKolektibilitass = null;
                string stringStatusPengecekans = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "la_inquiry_check");
                //cekHive = true;
                /*check pengawas LJK*/
                if (RefController.IsPengawasLJK(db))
                {
                    var filter = RefController.GetFilteredMemberTypes(db, login);
                    var filter2 = RefController.GetFilteredMembers(db, login);

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
                    TempData["memberTypeValue"] = memberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMemberTypes != "") stringMemberTypes += ", ";
                            stringMemberTypes += find.deskripsi_jenis_ljk;
                        }
                        TempData["mt"] = stringMemberTypes;
                    }

                }

                if (Members.Length > 0)
                {
                    TempData["memberValue"] = members;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }

                if (Kolektibilitass.Length > 0)
                {
                    stringKolektibilitass = string.Join(", ", Kolektibilitass);
                    TempData["k"] = stringKolektibilitass;
                }

                if (StatusPengecekans.Length > 0)
                {
                    stringStatusPengecekans = string.Join(", ", StatusPengecekans);
                    TempData["sp"] = stringStatusPengecekans;
                }

                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
                    if (cekHive == true)
                    {
                        foreach (var i in lp)
                        {
                            if (stringPeriode == null)
                            {
                                stringPeriode = string.Format("{0:yyyyMM}", i);
                            }
                            else
                            {
                                stringPeriode = stringPeriode + "," + string.Format("{0:yyyyMM}", i);
                            }

                        }
                    }
                    else
                    {
                        stringPeriode = string.Join(", ", periodes);
                    }
                    TempData["p"] = stringPeriode;
                }


                var result = Helper.WSQueryStore.GetLA_InquiryCheckQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKolektibilitass, stringStatusPengecekans, stringPeriode, cekHive, isChart);

                return JsonConvert.SerializeObject(result);
            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataLA_InquiryPatternRecognition(DataSourceLoadOptions loadOptions, string memberTypes, string members, string statusInquirys, string periode, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0) {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] StatusInquirys = JsonConvert.DeserializeObject<string[]>(statusInquirys);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                string stringStatusInquirys = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "la_inquiry_pattern_recognition");
                //cekHive = true;
                /*check pengawas LJK*/
                if (RefController.IsPengawasLJK(db))
                {
                    var filter = RefController.GetFilteredMemberTypes(db, login);
                    var filter2 = RefController.GetFilteredMembers(db, login);

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
                    TempData["memberTypeValue"] = memberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMemberTypes != "") stringMemberTypes += ", ";
                            stringMemberTypes += find.deskripsi_jenis_ljk;
                        }
                        TempData["mt"] = stringMemberTypes;
                    }

                }

                if (Members.Length > 0)
                {
                    TempData["memberValue"] = members;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }

                if (StatusInquirys.Length > 0)
                {
                    stringStatusInquirys = string.Join(", ", StatusInquirys);
                    TempData["si"] = stringStatusInquirys;
                }

                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
                    if (cekHive == true)
                    {
                        foreach (var i in lp)
                        {
                            if (stringPeriode == null)
                            {
                                stringPeriode = string.Format("{0:yyyyMM}", i);
                            }
                            else
                            {
                                stringPeriode = stringPeriode + "," + string.Format("{0:yyyyMM}", i);
                            }

                        }
                    }
                    else
                    {
                        stringPeriode = string.Join(", ", periodes);
                    }
                    TempData["p"] = stringPeriode;
                }


                var result = Helper.WSQueryStore.GetLA_InquiryPatternRecognitionQuery(db, loadOptions, stringMemberTypes, stringMembers, stringStatusInquirys, stringPeriode, cekHive, isChart);

                return JsonConvert.SerializeObject(result);
            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        #endregion
    }
}
