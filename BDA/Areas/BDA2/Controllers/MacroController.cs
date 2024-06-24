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
    public class MacroController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public MacroController(DataEntities db, IWebHostEnvironment env)
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
                string stringVariable3 = null;
                string stringPeriode = null;
                
                if (reportId == "macro_output_forecast_level_ljk")
                {
                    newq.rgq_nama = "MAP01 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("f") != null)
                    {
                        stringVariable1 = TempData.Peek("f").ToString();
                    }
                    DateTime pAwal = Convert.ToDateTime(TempData.Peek("p").ToString());
                    DateTime pAkhir = Convert.ToDateTime(TempData.Peek("p2").ToString());
                    newq.rgq_query = Helper.WSQueryExport.GetMacro_ForecastingQuery(db, stringMemberTypes, stringMembers, stringVariable1, pAwal,pAkhir);
                    db.CheckPermission("MAP01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_pertumbuhan_pinjaman_level_ljk")
                {
                    newq.rgq_nama = "MAP02 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("jp") != null)
                    {
                        stringVariable1 = TempData.Peek("jp").ToString();
                    }
                    DateTime pAwal = Convert.ToDateTime(TempData.Peek("p").ToString());
                    DateTime pAkhir = Convert.ToDateTime(TempData.Peek("p2").ToString());
                    newq.rgq_query = Helper.WSQueryExport.GetMacro_AMPertumbuhanQuery(db, stringMemberTypes, stringMembers, stringVariable1, pAwal, pAkhir);
                    db.CheckPermission("MAP02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_penetrasi_lending_ljk")
                {
                    newq.rgq_nama = "MAP03 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("k") != null)
                    {
                        stringVariable2 = TempData.Peek("k").ToString();
                    }
                    if (TempData.Peek("dk") != null)
                    {
                        stringVariable3 = TempData.Peek("dk").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMacro_AMPenetrasiQuery(db, stringMemberTypes, stringMembers, stringVariable1,stringVariable2,stringVariable3, stringPeriode, isHive);
                    db.CheckPermission("MAP03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_policy_evaluation_analysis")
                {
                    newq.rgq_nama = "MAP04 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("k") != null)
                    {
                        stringVariable2 = TempData.Peek("k").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMacro_PolicyEAQuery(db, stringMemberTypes, stringMembers, stringVariable1,stringVariable2, stringPeriode, isHive);
                    db.CheckPermission("MAP04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_Macro_" + reportId, "Export Data", pageTitle);
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
            if (id == "macro_output_forecast_level_ljk")
            {
                db.InsertAuditTrail("MAP_" + id + "_Akses_Page", "Akses Page Dashboard MAP01", pageTitle);
                db.CheckPermission("MAP01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MAP01 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("Forecasting");
            }
            else if (id == "macro_pertumbuhan_pinjaman_level_ljk")
            {
                db.InsertAuditTrail("MAP_" + id + "_Akses_Page", "Akses Page Dashboard MAP02", pageTitle);
                db.CheckPermission("MAP02 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MAP02 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("PertumbuhanPinjaman");
            }
            else if (id == "macro_penetrasi_lending_ljk")
            {
                db.InsertAuditTrail("MAP_" + id + "_Akses_Page", "Akses Page Dashboard MAP03", pageTitle);
                db.CheckPermission("MAP03 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MAP03 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("PenetrasiPinjaman");
            }
            else if (id == "macro_policy_evaluation_analysis")
            {
                db.InsertAuditTrail("MAP_" + id + "_Akses_Page", "Akses Page Dashboard MAP04", pageTitle);
                db.CheckPermission("MAP04 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MAP04 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("PolicyEvaluation");
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
                //db.CheckPermission("Macro Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "macro_output_forecast_level_ljk")
                {
                    db.CheckPermission("MAP01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_pertumbuhan_pinjaman_level_ljk")
                {
                    db.CheckPermission("MAP02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_penetrasi_lending_ljk")
                {
                    db.CheckPermission("MAP03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_policy_evaluation_analysis")
                {
                    db.CheckPermission("MAP04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_Macro_" + reportId, "Export Data", pageTitle);
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
                if (reportId == "macro_output_forecast_level_ljk")
                {
                    db.CheckPermission("MAP01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_pertumbuhan_pinjaman_level_ljk")
                {
                    db.CheckPermission("MAP02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_penetrasi_lending_ljk")
                {
                    db.CheckPermission("MAP03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "macro_policy_evaluation_analysis")
                {
                    db.CheckPermission("MAP04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_Macro_" + reportId, "Export Data", pageTitle);

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
                var fileName = "Macro_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "Macro_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridDataMacro_Forecasting(DataSourceLoadOptions loadOptions, string memberTypes, string members, string tipeForecastings, string periodeAwal, string periodeAkhir, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] TipeForecastings = JsonConvert.DeserializeObject<string[]>(tipeForecastings);

            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringTipeForecastings = null;
            DateTime? datePeriodeAwal = null;
            DateTime? datePeriodeAkhir = null;

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

            if (TipeForecastings.Length > 0)
            {
                stringTipeForecastings = string.Join(", ", TipeForecastings);
                TempData["f"] = stringTipeForecastings;
            }

            if (periodeAwal != null)
            {
                datePeriodeAwal = Convert.ToDateTime(periodeAwal);
                TempData["p"] = datePeriodeAwal;
            }

            if (periodeAkhir != null)
            {
                datePeriodeAkhir = Convert.ToDateTime(periodeAkhir);
                TempData["p2"] = datePeriodeAkhir;
            }


            var result = Helper.WSQueryStore.GetMacro_ForecastingQuery(db, loadOptions, stringMemberTypes, stringMembers, stringTipeForecastings, datePeriodeAwal.Value, datePeriodeAkhir.Value, isChart);

            return JsonConvert.SerializeObject(result);

        }

        public object GetGridDataMacro_AMPertumbuhan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string jenisPertumbuhans,
            string periodeAwal, string periodeAkhir, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] JenisPertumbuhans = JsonConvert.DeserializeObject<string[]>(jenisPertumbuhans);

            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringJenisPertumbuhans = null;
            DateTime? datePeriodeAwal = null;
            DateTime? datePeriodeAkhir = null;

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

            if (JenisPertumbuhans.Length > 0)
            {
                stringJenisPertumbuhans = string.Join(", ", JenisPertumbuhans);
                TempData["jp"] = stringJenisPertumbuhans;
            }
            if (periodeAwal != null)
            {
                datePeriodeAwal = Convert.ToDateTime(periodeAwal);
                TempData["p"] = datePeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                datePeriodeAkhir = Convert.ToDateTime(periodeAkhir);
                TempData["p2"] = datePeriodeAkhir;
            }


            var result = Helper.WSQueryStore.GetMacro_AMPertumbuhanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringJenisPertumbuhans, datePeriodeAwal.Value, datePeriodeAkhir.Value, isChart);

            return JsonConvert.SerializeObject(result);

        }

        public object GetGridDataMacro_AMPenetrasi(DataSourceLoadOptions loadOptions, string memberTypes, string members, string jenisDebiturs, string kategori,
            string deskripsiKategoris, string periode, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);
            string[] DeskripsiKategoris = JsonConvert.DeserializeObject<string[]>(deskripsiKategoris);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringJenisDebiturs = null;
            string stringKategori = null;
            string stringDeskripsiKategoris = null;
            string stringPeriode = null;
            
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "macro_penetrasi_lending_ljk");
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

            if (JenisDebiturs.Length > 0)
            {
                stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                TempData["jd"] = stringJenisDebiturs;
            }

            if (kategori != null)
            {
                stringKategori = kategori;
                TempData["k"] = stringKategori;
            }

            if (DeskripsiKategoris.Length > 0)
            {
                stringDeskripsiKategoris = string.Join(", ", DeskripsiKategoris);
                TempData["dk"] = stringDeskripsiKategoris;
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


            var result = Helper.WSQueryStore.GetMacro_AMPenetrasiQuery(db, loadOptions, stringMemberTypes, stringMembers, stringJenisDebiturs, stringKategori, stringDeskripsiKategoris, stringPeriode, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }

        public object GetGridDataMacro_PolicyEvaluation(DataSourceLoadOptions loadOptions, string memberTypes, string members, string jenisDebiturs, string kolektibilitas,
            string periode, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);
            string[] Kolektibilitas = JsonConvert.DeserializeObject<string[]>(kolektibilitas);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringJenisDebiturs = null;
            string stringKolektibilitas = null;
            string stringPeriode = null;

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "macro_policy_evaluation_analysis");
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

            if (JenisDebiturs.Length > 0)
            {
                stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                TempData["jd"] = stringJenisDebiturs;
            }
            if (Kolektibilitas.Length > 0)
            {
                stringKolektibilitas = string.Join(", ", Kolektibilitas);
                TempData["k"] = stringKolektibilitas;
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

            var result = Helper.WSQueryStore.GetMacro_PolicyEAQuery(db, loadOptions, stringMemberTypes, stringMembers, stringJenisDebiturs, stringKolektibilitas, stringPeriode, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        #endregion

    }
}
