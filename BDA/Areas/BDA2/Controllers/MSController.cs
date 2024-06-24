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
    public class MSController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public MSController(DataEntities db, IWebHostEnvironment env)
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
                if (reportId == "ms_ketentuan_kolektibilitas")
                {
                    newq.rgq_nama = "MS02 Export CSV";
                    if(TempData.Peek("mt") != null) {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("ksk") != null)
                    {
                        stringVariable1 = TempData.Peek("ksk").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMS_KetentuanKolektibilitas(db,stringMemberTypes,stringMembers, stringVariable1, stringPeriode,isHive);
                    db.CheckPermission("MS02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_kolektibilitas_karyawan_ljk")
                {
                    newq.rgq_nama = "MS05 Export CSV";
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
                    newq.rgq_query = Helper.WSQueryExport.GetMS_KolektibilitasKaryawanLJK(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("MS05 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_pinjaman_baru_bersamaan_sum")
                {
                    newq.rgq_nama = "MS01 Export CSV";
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
                    newq.rgq_query = Helper.WSQueryExport.GetMS_PinjamanBaruBersamaan(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("MS01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_pemberian_pinjaman_karyawan_ljk")
                {
                    newq.rgq_nama = "MS04 Export CSV";
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
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMS_PemberianPinjamanKaryawanLJK(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("MS04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    newq.rgq_nama = "MS03 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("stb") != null)
                    {
                        stringVariable1 = TempData.Peek("stb").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMS_APSukuBungaQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("MS03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_MS_" + reportId, "Export Data", pageTitle);
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
            if (id == "ms_ketentuan_kolektibilitas")
            {
                db.InsertAuditTrail("MS_" + id + "_Akses_Page", "Akses Page Dashboard MS02", pageTitle);
                db.CheckPermission("MS02 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MS02 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("KetentuanKolektibilitas");
            }
            else if (id == "ms_kolektibilitas_karyawan_ljk")
            {
                db.InsertAuditTrail("MS_" + id + "_Akses_Page", "Akses Page Dashboard MS05", pageTitle);
                db.CheckPermission("MS05 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MS05 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("KolektibilitasKaryawanLJK");
            }
            else if (id == "ms_pinjaman_baru_bersamaan_sum")
            {
                db.InsertAuditTrail("MS_" + id + "_Akses_Page", "Akses Page Dashboard MS01", pageTitle);
                db.CheckPermission("MS01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MS01 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("PinjamanBaruBersamaan");
            }
            else if (id == "ms_pemberian_pinjaman_karyawan_ljk")
            {
                db.InsertAuditTrail("MS_" + id + "_Akses_Page", "Akses Page Dashboard MS04", pageTitle);
                db.CheckPermission("MS04 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MS04 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("PemberianPinjamanKaryawanLJK");
            }
            else
            {
                db.InsertAuditTrail("MS_" + id + "_Akses_Page", "Akses Page Dashboard MS03", pageTitle);
                db.CheckPermission("MS03 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("MS03 Export", DataEntities.PermissionMessageType.NoMessage);
                return View();
            }


        }
        public IActionResult PinjamanBaruBersamaanDetail(string id)
        {
            if (id == null) return BadRequest();
            ViewBag.id = "ms_pinjaman_baru_bersamaan_det";
            ViewBag.Export = true; // TODO ubah permission disini
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_pinjaman_baru_bersamaan_det");
            ViewBag.Hive = isHive;
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            //ViewBag.Export = db.CheckPermission("Size Bisnis Pelapor Export", DataEntities.PermissionMessageType.NoMessage);
            TempData["detailID"] = id;
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
                if (reportId == "ms_ketentuan_kolektibilitas")
                {
                    db.CheckPermission("MS02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_kolektibilitas_karyawan_ljk")
                {
                    db.CheckPermission("MS05 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_pinjaman_baru_bersamaan_sum")
                {
                    db.CheckPermission("MS01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_pemberian_pinjaman_karyawan_ljk")
                {
                    db.CheckPermission("MS04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    db.CheckPermission("MS03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_MS_" + reportId, "Export Data", pageTitle);
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
                if (reportId == "ms_ketentuan_kolektibilitas")
                {
                    db.CheckPermission("MS02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_kolektibilitas_karyawan_ljk")
                {
                    db.CheckPermission("MS05 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_pinjaman_baru_bersamaan_sum")
                {
                    db.CheckPermission("MS01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "ms_pemberian_pinjaman_karyawan_ljk")
                {
                    db.CheckPermission("MS04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    db.CheckPermission("MS03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_MS_" + reportId, "Export Data", pageTitle);

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
                var fileName = "MS_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "MS_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridDataMS_APSukuBunga(DataSourceLoadOptions loadOptions, string memberTypes, string members, string stb, string periode, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] stbs = JsonConvert.DeserializeObject<string[]>(stb);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;
            string stringSTB = null;
            string stringPeriode = null;
            //List<DateTime> lp = new List<DateTime>();
            //foreach (var i in periodes)
            //{
            //    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
            //}
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_perubahan_suku_bunga_no_restrukturisasi");
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
                stringMembers =members;
                TempData["m"] = stringMembers;
            }

            if (stbs.Length > 0)
            {
                stringSTB = string.Join(", ", stbs);
                TempData["stb"] = stringSTB;
            }

            if (periode!=null)
            {
                DateTime period1= Convert.ToDateTime(periode);
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

            var result = Helper.WSQueryStore.GetMS_APSukuBungaQuery(db, loadOptions, stringMemberTypes, stringMembers, stringSTB, stringPeriode, tipeChart, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        public object GetGridDataMS_KetentuanKolektibilitas(DataSourceLoadOptions loadOptions, string memberTypes, string members, string ksk, string periode, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] ksks = JsonConvert.DeserializeObject<string[]>(ksk);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;
            string stringKSK = null;
            string stringPeriode = null;

            //List<DateTime> lp = new List<DateTime>();
            //foreach (var i in periodes)
            //{
            //    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
            //}
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_ketentuan_kolektibilitas");
            //cekHive = true;
            /*check pengawas LJK*/
            if (RefController.IsPengawasLJK(db))
            {
                var filter = RefController.GetFilteredMemberTypes(db, login);
                var filter2 = RefController.GetFilteredMembers(db, login);

                if (memberTypes !=null)
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

            if (ksks.Length > 0)
            {
                stringKSK = string.Join(", ", ksks);
                TempData["ksk"] = stringKSK;
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


            var result = Helper.WSQueryStore.GetMS_KetentuanKolektibilitas(db, loadOptions, stringMemberTypes, stringMembers, stringKSK, stringPeriode, tipeChart, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        public object GetGridDataMS_KolektibilitasKaryawanLJK(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kolektibilitas, string periode, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] kolektibilitass = JsonConvert.DeserializeObject<string[]>(kolektibilitas);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;
            string stringKolektibilitas = null;
            string stringPeriode = null;
            //List<DateTime> lp = new List<DateTime>();
            //foreach (var i in periodes)
            //{
            //    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
            //}
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_kolektibilitas_karyawan_ljk");
            //cekHive = true;
            /*check pengawas LJK*/
            if (RefController.IsPengawasLJK(db))
            {
                var filter = RefController.GetFilteredMemberTypes(db, login);
                var filter2 = RefController.GetFilteredMembers(db, login);

                if (memberTypes !=null)
                {
                    stringMemberTypes = string.Join(", ", filter);
                }

                if (members !=null)
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

            if (members !=null)
            {
                stringMembers = members;
                TempData["m"] = stringMembers;
            }

            if (kolektibilitass.Length > 0)
            {
                stringKolektibilitas = string.Join(", ", kolektibilitass);
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
            var result = Helper.WSQueryStore.GetMS_KolektibilitasKaryawanLJK(db, loadOptions, stringMemberTypes, stringMembers, stringKolektibilitas, stringPeriode, tipeChart, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        public object GetGridDataMS_PemberianPinjamanKaryawanLJK(DataSourceLoadOptions loadOptions, string memberTypes, string members, string jp, string periode, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] jps = JsonConvert.DeserializeObject<string[]>(jp);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;
            string stringJP = null;
            string stringPeriode = null;
            //List<DateTime> lp = new List<DateTime>();
            //foreach (var i in periodes)
            //{
            //    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
            //}
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_pemberian_pinjaman_karyawan_ljk");
            //cekHive = true;
            /*check pengawas LJK*/
            if (RefController.IsPengawasLJK(db))
            {
                var filter = RefController.GetFilteredMemberTypes(db, login);
                var filter2 = RefController.GetFilteredMembers(db, login);

                if (memberTypes !=null)
                {
                    stringMemberTypes = string.Join(", ", filter);
                }

                if (members !=null)
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

            if (members !=null)
            {
                stringMembers = members;
                TempData["m"] = stringMembers;
            }

            if (jps.Length > 0)
            {
                stringJP = string.Join(", ", jps);
                TempData["jp"] = stringJP;
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


            var result = Helper.WSQueryStore.GetMS_PemberianPinjamanKaryawanLJK(db, loadOptions, stringMemberTypes, stringMembers, stringJP, stringPeriode, tipeChart, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        public object GetGridDataMS_PinjamanBaruBersamaan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kolektibilitas, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] kolektibilitass = JsonConvert.DeserializeObject<string[]>(kolektibilitas);
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            string members1 = members;
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringKolektibilitas = null;
            string stringPeriode = null;
            //List<DateTime> lp = new List<DateTime>();
            //foreach (var i in periodes)
            //{
            //    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
            //}
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_pinjaman_baru_bersamaan_sum");
            //cekHive = true;
            TempData["memberTypeValue"] = null;
            TempData["memberValue"] = null;
            TempData["kolektibilitasValue"] = null;
            TempData["periodeValue"] = null;
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
                TempData["memberTypeValue"] = memberTypes;
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

            if (members !=null)
            {
                TempData["memberValue"] = members1;
                stringMembers = members;
                TempData["m"] = stringMembers;
            }

            if (kolektibilitass.Length > 0)
            {
                TempData["kolektibilitasValue"] = kolektibilitass;
                stringKolektibilitas = string.Join(", ", kolektibilitass);
                TempData["k"] = stringKolektibilitas;
            }

            if (periode != null)
            {
                TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode); ;
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


            var result = Helper.WSQueryStore.GetMS_PinjamanBaruBersamaan(db, loadOptions, stringMemberTypes, stringMembers, stringKolektibilitas, stringPeriode, cekHive);

            return JsonConvert.SerializeObject(result);

        }
        public object GetGridDataMS_PinjamanBaruBersamaanDetail(DataSourceLoadOptions loadOptions)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var detailID = TempData.Peek("detailID").ToString().Split("~");
            //string[] Members = detailID[3].ToString().Split(",");
            //Members = Members.Select(x => x.Substring(x.IndexOf(" - ") -1, x.Length - (x.IndexOf(" - ") - 1))).ToArray();

            string stringMemberTypes = detailID[1].ToString().Replace(">", "/");
            //string stringMembers = detailID[2].ToString();
            string stringMembers = detailID[2].ToString().Replace(">", "/");
            string stringKolektibilitas = detailID[3].ToString();
            string stringPeriode = detailID[0].ToString();
            var dtPeriode = detailID[0].ToString();

            stringPeriode = dtPeriode.Substring(0, 4) + "-" + dtPeriode.Substring(4, 2) + "-01";
            List<DateTime> lp = new List<DateTime>();
            lp.Add(DateTime.Parse(stringPeriode));
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ms_pinjaman_baru_bersamaan_det");
            //cekHive = true;
            if (cekHive == true)
            {
                stringPeriode = detailID[0].ToString();
            }
            TempData["mt"] = stringMemberTypes;
            TempData["m"] = stringMembers;
            TempData["k"] = stringKolektibilitas;
            TempData["p"] = stringPeriode;
            /*check pengawas LJK*/
            //if (RefController.IsPengawasLJK(db))
            //{
            //    var filter = RefController.GetFilteredMemberTypes(db, login);
            //    var filter2 = RefController.GetFilteredMembers(db, login);

            //    if (MemberTypes.Length == 0)
            //    {
            //        stringMemberTypes = string.Join(", ", filter);
            //    }

            //    if (Members.Length == 0)
            //    {
            //        stringMembers = string.Join(", ", filter2);
            //    }
            //}

            //if (MemberTypes.Length > 0)
            //{
            //    TempData["memberTypeValue"] = MemberTypes;
            //    var listOfJenis = db.master_ljk_type.ToList();
            //    // nih gara2 si data processing kaga pake kode di output nya -_-;
            //    stringMemberTypes = "";
            //    foreach (var mem in MemberTypes)
            //    {
            //        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
            //        if (find != null)
            //        {
            //            if (stringMemberTypes != "") stringMemberTypes += ", ";
            //            stringMemberTypes += find.deskripsi_jenis_ljk;
            //        }
            //    }

            //}

            //if (Members.Length > 0)
            //{
            //    stringMembers = string.Join(", ", Members);
            //}

            //if (kolektibilitass.Length > 0)
            //{
            //    TempData["kolektibilitasValue"] = kolektibilitass;
            //    stringKolektibilitas = string.Join(", ", kolektibilitass);
            //}

            //if (periodes.Length > 0)
            //{
            //    TempData["periodeValue"] = periodes;
            //    stringPeriode = string.Join(", ", periodes);
            //}


            var result = Helper.WSQueryStore.GetMS_PinjamanBaruBersamaanDetail(db, loadOptions, stringMemberTypes, stringMembers, stringKolektibilitas, stringPeriode, cekHive);

            return JsonConvert.SerializeObject(result);

        }
        #endregion

    }
}
