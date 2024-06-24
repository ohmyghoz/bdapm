using Aspose.Cells;
using BDA.DataModel;
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
    public class MicroController : Controller
    {

        private DataEntities db;
        private IWebHostEnvironment _env;

        public MicroController(DataEntities db, IWebHostEnvironment env)
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
                string stringPeriode = null;
                if (reportId == "micro_credit_risk_analysis")
                {
                    newq.rgq_nama = "MIP01 Export CSV";
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
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMicro_CreditRiskQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("MIP01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "micro_plafond_usability_acc_detail")
                {
                    newq.rgq_nama = "MIP02 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kp") != null)
                    {
                        stringVariable1 = TempData.Peek("kp").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMicro_LiquidityRiskQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("MIP02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_Micro_" + reportId, "Export Data", pageTitle);
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
            TempData["periode"] = string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1));
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, id);
            ViewBag.Hive = isHive;
            if (id == "micro_credit_risk_analysis")
            {
                db.InsertAuditTrail("MIP_" + id + "_Akses_Page", "Akses Page Dashboard MIP01", pageTitle);
                db.CheckPermission("MIP01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MIP01 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("CreditRisk");
            }
            else if (id == "micro_plafond_usability_acc_detail")
            {
                db.InsertAuditTrail("MIP_" + id + "_Akses_Page", "Akses Page Dashboard MIP02", pageTitle);
                db.CheckPermission("MIP02 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MIP02 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("LiquidityRisk");
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
                //db.CheckPermission("Micro Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "micro_credit_risk_analysis")
                {
                    db.CheckPermission("MIP01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "micro_plafond_usability_acc_detail")
                {
                    db.CheckPermission("MIP02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_Micro_" + reportId, "Export Data", pageTitle);
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

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                //db.CheckPermission("Macro Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "micro_credit_risk_analysis")
                {
                    db.CheckPermission("MIP01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "micro_plafond_usability_acc_detail")
                {
                    db.CheckPermission("MIP02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_Micro_" + reportId, "Export Data", pageTitle);

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
                var fileName = "Micro_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "Micro_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridDataMicro_LiquidityRisk(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kelasPlafons, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] KelasPlafons = JsonConvert.DeserializeObject<string[]>(kelasPlafons);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringKelasPlafons = null;
            string stringPeriode = null;

            //List<DateTime> lp = new List<DateTime>();
            //foreach (var i in periode)
            //{
            //    var rep = i.ToString();
            //    lp.Add(DateTime.Parse(rep.Replace("'", "").Replace("[", "").Replace("]", "")));
            //}
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "micro_plafond_usability_acc_detail");
            //cekHive = true;
            /*check pengawas LJK*/
            if (RefController.IsPengawasLJK(db))
            {
                var filter = RefController.GetFilteredMemberTypes(db, login);
                var filter2 = RefController.GetFilteredMembers(db, login);

                if (memberTypes != null)
                {
                    stringMemberTypes = string.Join(", ", filter);
                }

                if (members != null)
                {
                    stringMembers = string.Join(", ", filter2);
                }
            }

            if (memberTypes != null)
            {
                var listOfJenis = db.master_ljk_type.ToList();
                // nih gara2 si data processing kaga pake kode di output nya -_-;
                stringMemberTypes = "";
                var find = listOfJenis.Where(x => x.kode_jenis_ljk == memberTypes).FirstOrDefault();
                if (find != null)
                {
                    if (stringMemberTypes != "") stringMemberTypes += ", ";
                    stringMemberTypes += find.deskripsi_jenis_ljk;
                }
                TempData["mt"] = stringMemberTypes;
            }

            if (members != null)
            {
                stringMembers = members;
                TempData["m"] = stringMembers;
            }

            if (KelasPlafons.Length > 0)
            {
                stringKelasPlafons = string.Join(", ", KelasPlafons);
                TempData["kp"] = stringKelasPlafons;
            }

            if(periode != null)
            {
                DateTime period1 = Convert.ToDateTime(periode);
                if (cekHive == true)
                {
                    stringPeriode = string.Format("{0:yyyyMM}", period1);
                }
                else
                {
                    stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
                }
                TempData["p"] = stringPeriode;
            }


            var result = Helper.WSQueryStore.GetMicro_LiquidityRiskQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKelasPlafons, stringPeriode, cekHive, isChart, isPieChart);

            return JsonConvert.SerializeObject(result);

        }

        public object GetGridDataMicro_CreditRisk(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kolektibilitass, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] Kolektibilitass = JsonConvert.DeserializeObject<string[]>(kolektibilitass);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringKolektibilitass = null;
            string stringPeriode = null;
            
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "micro_credit_risk_analysis");
            //cekHive = true;
            /*check pengawas LJK*/
            if (RefController.IsPengawasLJK(db))
            {
                var filter = RefController.GetFilteredMemberTypes(db, login);
                var filter2 = RefController.GetFilteredMembers(db, login);

                if (memberTypes != null)
                {
                    stringMemberTypes = string.Join(", ", filter);
                }

                if (members != null)
                {
                    stringMembers = string.Join(", ", filter2);
                }
            }

            if (memberTypes != null)
            {
                var listOfJenis = db.master_ljk_type.ToList();
                // nih gara2 si data processing kaga pake kode di output nya -_-;
                stringMemberTypes = "";
                var find = listOfJenis.Where(x => x.kode_jenis_ljk == memberTypes).FirstOrDefault();
                if (find != null)
                {
                    if (stringMemberTypes != "") stringMemberTypes += ", ";
                    stringMemberTypes += find.deskripsi_jenis_ljk;
                }
                TempData["mt"] = stringMemberTypes;
            }

            if (members != null)
            {
                stringMembers = members;
                TempData["m"] = stringMembers;
            }

            if (Kolektibilitass.Length > 0)
            {
                stringKolektibilitass = string.Join(", ", Kolektibilitass);
                TempData["k"] = stringKolektibilitass;
            }

            if (periode != null)
            {
                DateTime period1 = Convert.ToDateTime(periode);
                if (cekHive == true)
                {
                    stringPeriode = string.Format("{0:yyyyMM}", period1);
                }
                else
                {
                    stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
                }
                TempData["p"] = stringPeriode;
            }


            var result = Helper.WSQueryStore.GetMicro_CreditRiskQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKolektibilitass, stringPeriode,cekHive, isChart, isPieChart);

            return JsonConvert.SerializeObject(result);

        }

        #endregion
    }
}
