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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("BDA3")]
    public class OSIDA2023Controller : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public OSIDA2023Controller(DataEntities db, IWebHostEnvironment env)
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
                pageTitle = TempData.Peek("pageTitle").ToString();
                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringPeriodes = null;
                string stringKantorCabangs = null;
                var obj = db.osida_master.Find(reportId);
                newq.rgq_nama = obj.menu_nama + " Export CSV";
                if (TempData.Peek("mt") != null)
                {
                    stringMemberTypes = TempData.Peek("mt").ToString();
                }
                if (TempData.Peek("m") != null)
                {
                    stringMembers = TempData.Peek("m").ToString();
                }
                if (TempData.Peek("p") != null)
                {
                    stringPeriodes = TempData.Peek("p").ToString();
                }
                if (TempData.Peek("kc") != null)
                {
                    stringKantorCabangs = TempData.Peek("kc").ToString();
                }
                //DateTime p = Convert.ToDateTime( TempData.Peek("p").ToString());
                newq.rgq_query = Helper.WSQueryExport.GetOsida2023Query(db, reportId, stringMemberTypes, stringMembers,stringKantorCabangs, stringPeriodes);

                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();

                db.InsertAuditTrail("ExportIndex_OSIDA_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        [HttpPost]
        public IActionResult AntrianDetail2(string reportId)
        {
            try
            {
                if (reportId != "osida_potensi_konversi_kur_deb_noneligible_mst" && reportId!= "osida_pemberian_kur_deb_noneligible_mst" && reportId!= "osida_nik_tidak_konsisten_mst" && reportId!= "osida_pengurus_pemilik_kredit_bermasalah_mst") {
                    throw new InvalidOperationException("Fungsi antrian ini bukan untuk dashboard ini");
                }
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();
                var rptId = "";
                if (reportId == "osida_potensi_konversi_kur_deb_noneligible_mst")
                {
                    rptId = "osida_potensi_konversi_kur_deb_noneligible_det";
                }
                else if (reportId == "osida_pemberian_kur_deb_noneligible_mst")
                {
                    rptId = "osida_pemberian_kur_deb_noneligible_det";
                }
                else if (reportId == "osida_nik_tidak_konsisten_mst")
                {
                    rptId = "osida_nik_tidak_konsisten_det";
                }
                else if (reportId == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
                    rptId = "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus";
                }
                string pageTitle = currentNode != null ? currentNode.Title : "";
                pageTitle = TempData.Peek("pageTitle").ToString();
                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, rptId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringPeriodes = null;
                string stringKantorCabangs = null;
                var obj = db.osida_master.Find(reportId);
                newq.rgq_nama = obj.menu_nama + " Export CSV";
                if (TempData.Peek("mt") != null)
                {
                    stringMemberTypes = TempData.Peek("mt").ToString();
                }
                if (TempData.Peek("m") != null)
                {
                    stringMembers = TempData.Peek("m").ToString();
                }
                if (TempData.Peek("p") != null)
                {
                    stringPeriodes = TempData.Peek("p").ToString();
                }
                if (TempData.Peek("kc") != null)
                {
                    stringKantorCabangs = TempData.Peek("kc").ToString();
                }
                //DateTime p = Convert.ToDateTime( TempData.Peek("p").ToString());
                newq.rgq_query = Helper.WSQueryExport.GetOsida2023Detail2Query(db, reportId, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriodes);

                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();

                db.InsertAuditTrail("ExportIndex_OSIDA_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        [HttpPost]
        public IActionResult AntrianDetail(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";
                pageTitle = TempData.Peek("pageTitle").ToString();
                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringPeriodes = null;
                string stringKantorCabangs = null;
                string stringCifs = null;
                var obj = db.osida_master.Find(reportId);
                newq.rgq_nama = obj.menu_nama + "Detail Export CSV";
                if (TempData.Peek("mtd") != null)
                {
                    stringMemberTypes = TempData.Peek("mtd").ToString();
                }
                if (TempData.Peek("md") != null)
                {
                    stringMembers = TempData.Peek("md").ToString();
                }
                if (TempData.Peek("pd") != null)
                {
                    stringPeriodes = TempData.Peek("pd").ToString();
                }
                if (TempData.Peek("kcd") != null)
                {
                    stringKantorCabangs = TempData.Peek("kcd").ToString();
                }
                if (TempData.Peek("cd") != null)
                {
                    stringCifs = TempData.Peek("cd").ToString();
                }
                //DateTime p = Convert.ToDateTime( TempData.Peek("p").ToString());
                newq.rgq_query = Helper.WSQueryExport.GetOsida2023DetailQuery(db, reportId, stringMemberTypes, stringMembers, stringKantorCabangs,stringCifs, stringPeriodes);

                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();

                db.InsertAuditTrail("ExportIndex_OSIDA_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult Index(string id,string period,string jljk,string ljk)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            
            string pageTitle = currentNode != null ? currentNode.Title : "";
            TempData["pageTitle"] = pageTitle;
            ViewBag.Export = true; // TODO ubah disini
            var obj = db.osida_master.Find(id);
            db.InsertAuditTrail("OSD_" + id + "_Akses_Page", "Akses Page Dashboard " + obj.menu_nama, pageTitle);
            db.CheckPermission(obj.menu_nama + " View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.NoMessage);
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, id);
            ViewBag.Hive = isHive;
            double sG = 0;
            double sD = 0;
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64 / 1024.0;
            TempData["SG"] = sG;
            TempData["SD"] = sD;
            TempData["PM"] = mem;
            if (obj == null) return NoContent();
            
            if (period == null)
            {
                //ViewBag.period = string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1));
                ViewBag.period = null;
            }
            else
            {
                DateTime p = Convert.ToDateTime(period);
                //ViewBag.period = string.Format("{0:yyyy-MM-01}", p);
                string[] p1= new string[]{ string.Format("{0:yyyy-MM-01}", p) };
                ViewBag.period = p1;
            }
            if (jljk == null)
            {
                ViewBag.jljk = null;
            }
            else
            {
                //ViewBag.jljk = jljk;
                string[] j1= new string[]{ jljk };
                ViewBag.jljk = j1;
            }
            if (ljk == null)
            {
                ViewBag.ljk = null;
            }
            else
            {
                //ViewBag.ljk = ljk;
                string[] l1= new string[]{ ljk };
                ViewBag.ljk = l1;
            }
            TempData["kodeOsida"] = obj.kode;
            //if (id == "osida_plafondering_umum_master")
            //{
            //    return View("Index2", obj);
            //}
            //else
            //{

            //}
            if (obj.kode == "osida_pengurus_pemilik_kredit_bermasalah_mst")
            {
                return View("index2",obj);
            }
            else {
                return View(obj);
            }
            
        }
        public IActionResult OsidaDetail(string id,string keys)
        {
            if (id == null) return BadRequest();
            var kodeOsida="";
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            
            string pageTitle = currentNode != null ? currentNode.Title : "";
            TempData["pageTitle"] = pageTitle;
            if (id == "osida_tanpa_info_keu_mst")
            {
                kodeOsida = "osida_tanpa_info_keu_det";
            }
            else if (id == "osida_info_keu_tdk_update_mst") {
                kodeOsida = "osida_info_keu_tdk_update_det";
            }
            else if (id == "osida_keu_buruk_kol_lancar_mst")
            {
                kodeOsida = "osida_keu_buruk_kol_lancar_det";
            }
            else if (id == "osida_agunan_tdk_dinilai_independen_mst")
            {
                kodeOsida = "osida_agunan_tdk_dinilai_independen_det";
            }
            else if (id == "osida_alih_deb_ke_bank_mst")
            {
                kodeOsida = "osida_alih_deb_ke_bank_det";
            }
            else if (id == "osida_kredit_macet_tdk_hb_mst")
            {
                kodeOsida = "osida_kredit_macet_tdk_hb_det";
            }
            else if (id == "osida_nik_tidak_konsisten_mst")
            {
                kodeOsida = "osida_nik_tidak_konsisten_det";
            }
            else if (id == "osida_potensi_konversi_kur_deb_noneligible_mst")
            {
                kodeOsida = "osida_potensi_konversi_kur_deb_noneligible_det";
            }
            else if (id == "osida_pemberian_kur_deb_noneligible_mst")
            {
                kodeOsida = "osida_pemberian_kur_deb_noneligible_det";
            }
            
                
                
            var obj = db.osida_master.Find(kodeOsida);
            ViewBag.id = kodeOsida;
            ViewBag.Export = true; // TODO ubah permission disini
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, kodeOsida);
            ViewBag.Hive = isHive;
            TempData["detailOsida"] = kodeOsida;
            db.InsertAuditTrail("OSD_" + kodeOsida + "_Detail_Akses_Page", "Akses Page Dashboard " + obj.menu_nama + " Detail", pageTitle + " Detail");
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            //ViewBag.Export = db.CheckPermission("Size Bisnis Pelapor Export", DataEntities.PermissionMessageType.NoMessage);
            TempData["detailID"] = keys;
            return View("Detail", obj);
        }
        public IActionResult OsidaDetail2(string id)
        {
            if (id == null) return BadRequest();
            var kodeOsida="";
            if (id.StartsWith("n") == true)
            {
                kodeOsida = "osida_pengurus_pemilik_kredit_bermasalah_det_bu";
                id = id.Replace("n~", "");
            }
            else {
                kodeOsida = "osida_pengurus_pemilik_kredit_bermasalah_det_pengurus";
                id = id.Replace("p~", "");
            }
            
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";
            TempData["pageTitle"] = pageTitle;
            var obj = db.osida_master.Find(kodeOsida);
            ViewBag.id = kodeOsida;
            ViewBag.Export = true; // TODO ubah permission disini
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, kodeOsida);
            ViewBag.Hive = isHive;
            TempData["detailOsida"] = kodeOsida;
            db.InsertAuditTrail("OSD_" + kodeOsida + "_Detail_Akses_Page", "Akses Page Dashboard " + obj.menu_nama + " Detail", pageTitle + " Detail");
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            //ViewBag.Export = db.CheckPermission("Size Bisnis Pelapor Export", DataEntities.PermissionMessageType.NoMessage);
            TempData["detailID"] = id;
            return View("Detail", obj);
        }
        [HttpPost]
        public IActionResult LogExportIndex(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";
                pageTitle = TempData.Peek("pageTitle").ToString();
                //TODO : tambah permission
                //db.CheckPermission("OSIDA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var obj = db.osida_master.Find(reportId);
                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_OSIDA_" + reportId, "Export Data", pageTitle);
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
                pageTitle = TempData.Peek("pageTitle").ToString();
                //TODO : tambah permission
                //db.CheckPermission("OSIDA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var obj = db.osida_master.Find(reportId);
                db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_OSIDA_" + reportId, "Export Data", pageTitle);

                var directory = _env.WebRootPath;

                var timeStamp = DateTime.Now.ToString();

                //Workbook workbook = new Workbook();
                //workbook.Open(file.OpenReadStream(), FileFormatType.Xlsx);
                Workbook workbook = new Workbook(file.OpenReadStream());

                if (reportId == "osida_pengurus_pemilik_kredit_bermasalah_mst")
                {
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
                var fileName = "OSIDA_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "OSIDA_" + reportId + "_" + timeStamp + ".pdf";
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
        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string reportId, string memberTypes, string members,string kantorCabangs, string periode,bool chk100)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            TempData["memberTypeValue"] = null;
            TempData["memberValue"] = null;
            TempData["periodeValue"] = null;
            TempData["kcValue"] = null;
            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members1 = JsonConvert.DeserializeObject<string[]>(members);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = null;
                string[] KantorCabangs1=null;
                if (kantorCabangs != null) {
                    KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                    KantorCabangs1 = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                    KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();
                }
                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }
                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                var timeNow = DateTime.Now;
                var timeAftter = DateTime.Now;

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
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
                    TempData["memberTypeValue"] = MemberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var m in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == m).FirstOrDefault();
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
                    TempData["memberValue"] = Members1;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }


                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = periodes;
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
                if (KantorCabangs != null) {
                    if (KantorCabangs.Length > 0)
                    {
                        TempData["kcValue"] = KantorCabangs1;
                        stringKantorCabangs = string.Join(", ", KantorCabangs);
                        TempData["kc"] = stringKantorCabangs;
                    }
                }
                var timeResultBefore = DateTime.Now;
                var result = Helper.WSQueryStore.GetOsida2023Query(db, loadOptions, reportId, stringMemberTypes, stringMembers, stringKantorCabangs,stringPeriode,chk100,cekHive);
                var timeResultAfter = DateTime.Now;
                timeAftter = DateTime.Now;
                var proc = Process.GetCurrentProcess();
                var mem = proc.WorkingSet64 / 1024.0;
                TempData["PM"] = mem;
                TempData["SG"] = (timeAftter - timeNow).TotalSeconds;
                TempData["SD"] = (timeResultAfter - timeResultBefore).TotalSeconds;
                return JsonConvert.SerializeObject(result);
            }
            else {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        public object GetGridDataDetail(DataSourceLoadOptions loadOptions)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var detailID = TempData.Peek("detailID").ToString().Split("~");
            var kodeOsida=TempData.Peek("detailOsida").ToString();
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, kodeOsida);
            string stringMemberTypes = detailID[1].ToString().Replace(">", "/");
            string stringMembers = detailID[2].ToString().Replace(">","/");
            string stringKantorCabangs = null;
            string stringCIFs = null;
            if (kodeOsida != "osida_potensi_konversi_kur_deb_noneligible_det" && kodeOsida != "osida_pemberian_kur_deb_noneligible_det")
            {
                if (detailID[3].ToString() != "") {
                    stringKantorCabangs = detailID[3].ToString().Replace(">", "/");
                }
                stringCIFs = detailID[4].ToString().Replace(">", "/");
            }
            else {
                stringCIFs = detailID[3].ToString().Replace(">", "/");
            }
            
            string stringPeriode = detailID[0].ToString();
            TempData["mtd"] = stringMemberTypes;
            TempData["md"] = stringMembers;
            TempData["kcd"] = stringKantorCabangs;
            TempData["cd"] = stringCIFs;
            TempData["pd"] = stringPeriode;
            
            var result = Helper.WSQueryStore.GetOsida2023DetailQuery(db, loadOptions, kodeOsida, stringMemberTypes, stringMembers,stringKantorCabangs,stringCIFs, stringPeriode, cekHive);
            return JsonConvert.SerializeObject(result);
        }
        #endregion
    }
}
