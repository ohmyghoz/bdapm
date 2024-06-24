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
namespace BDA.Helper {
    public class VisualTipeDTO
    {
        public string vis_kode { get; set; }
        public string vis_nama { get; set; }
    }
    public class chartDTO
    {
        public string dm_base { get; set; }
        public decimal dm_count { get; set; }
    }
}
namespace BDA.Controllers
{
    [Area("BDA2")]
    public class OSIDAController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public OSIDAController(DataEntities db, IWebHostEnvironment env)
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
                DateTime p = Convert.ToDateTime( TempData.Peek("p").ToString());
                newq.rgq_query = Helper.WSQueryExport.GetOsidaQuery(db, reportId, stringMemberTypes, stringMembers, p);

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

            ViewBag.Export = true; // TODO ubah disini
            if (id == "osida_summary")
            {
                db.InsertAuditTrail("OSD_" + id + "_Akses_Page", "Akses Page Dashboard OSD00", pageTitle);
                db.CheckPermission("OSD00 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("OSD00 Export", DataEntities.PermissionMessageType.NoMessage);
                ViewBag.Hive = false;
                return View("Summary");
            }
            else {
                var obj = db.osida_master.Find(id);
                db.InsertAuditTrail("OSD_" + id + "_Akses_Page", "Akses Page Dashboard "+ obj.menu_nama, pageTitle);
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
                if (obj.kode == "osida_hapusbuku_satu_tahun" || obj.kode == "osida_kredit_takeover_tidak_bayar" || obj.kode == "osida_restrukturisasi_satu_tahun" || obj.kode == "osida_tidak_bayar_dari_fasilitas_dibentuk")
                {
                    ViewBag.isChart = true;
                    ViewBag.tipeVis = obj.kode + "_total_plafon";
                    TempData["judulChart"] = "Berdasarkan Total Plafon Awal";
                }
                else
                {
                    ViewBag.isChart = false;
                    ViewBag.tipeVis = null;
                    TempData["judulChart"] = "";
                }
                if (period == null)
                {
                    ViewBag.period = string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1));
                }
                else
                {
                    DateTime p = Convert.ToDateTime(period);
                    ViewBag.period = string.Format("{0:yyyy-MM-01}", p);
                }
                if (jljk == null)
                {
                    ViewBag.jljk = null;
                }
                else {
                    ViewBag.jljk = jljk;
                }
                if (ljk == null)
                {
                    ViewBag.ljk = null;
                }
                else
                {
                    ViewBag.ljk = ljk;
                }
                TempData["kodeOsida"] = obj.kode;
                if (id == "osida_plafondering_umum_master")
                {
                    return View("Index2", obj);
                }
                else
                {
                    return View(obj);
                }
            }
           
        }
        public IActionResult OsidaPlafonderingUmumDetail(string id)
        {
            if (id == null) return BadRequest();
            var obj = db.osida_master.Find("osida_plafondering_umum_detail");
            ViewBag.id = "osida_plafondering_umum_detail";
            ViewBag.Export = true; // TODO ubah permission disini
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "osida_plafondering_umum_detail");
            ViewBag.Hive = isHive;
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

                //TODO : tambah permission
                //db.CheckPermission("OSIDA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "osida_summary")
                {
                    db.CheckPermission("OSD00 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else 
                {
                    var obj = db.osida_master.Find(reportId);
                    db.CheckPermission(obj.menu_nama+" Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
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

                //TODO : tambah permission
                //db.CheckPermission("OSIDA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "osida_summary")
                {
                    db.CheckPermission("OSD00 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    var obj = db.osida_master.Find(reportId);
                    db.CheckPermission(obj.menu_nama + " Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_OSIDA_" + reportId, "Export Data", pageTitle);

                var directory = _env.WebRootPath;

                var timeStamp = DateTime.Now.ToString();

                //Workbook workbook = new Workbook();
                //workbook.Open(file.OpenReadStream(), FileFormatType.Xlsx);
                Workbook workbook = new Workbook(file.OpenReadStream());
                if (reportId == "osida_plafondering_umum_master") {
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
        public object GetGridData(DataSourceLoadOptions loadOptions, string reportId, string memberTypes, string members, string periode,string tipeChart,bool isChart=false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;          
            DateTime? datePeriode = null;
            var timeNow = DateTime.Now;
            var timeAftter = DateTime.Now;
            if (tipeChart != null)
            {
                if (tipeChart.Contains("_total_plafon"))
                {
                    TempData["judulChart"] = "Berdasarkan Total Plafon Awal";
                }
                else if (tipeChart.Contains("_jumlah_rekening"))
                {
                    TempData["judulChart"] = "Berdasarkan Jumlah Rekening/Fasilitas";
                }
                else if (tipeChart.Contains("_jenis_kredit"))
                {
                    TempData["judulChart"] = "Berdasarkan Jenis Kredit";
                }
                else {
                    TempData["judulChart"] = "Berdasarkan Jenis Penggunaan";
                }

            }
            else {
                TempData["judulChart"] = "";
            }
            
            /*check pengawas LJK*/
            if (RefController.IsPengawasLJK(db))
            {
                var filter = RefController.GetFilteredMemberTypes(db,login);
                var filter2 = RefController.GetFilteredMembers(db,login);
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

            if (members !=null)
            {
                stringMembers = members;
                TempData["m"] = stringMembers;
            }

            if (periode != null)
            {
                datePeriode = Convert.ToDateTime(periode);
                TempData["p"] = datePeriode;
            }
            var timeResultBefore = DateTime.Now;
            var result = Helper.WSQueryStore.GetOsidaQuery(db, loadOptions, reportId, stringMemberTypes, stringMembers, datePeriode.Value,tipeChart,isChart);
            var timeResultAfter = DateTime.Now;
            if (isChart == true)
            {
                if (tipeChart.Contains("_total_plafon") || tipeChart.Contains("_jumlah_rekening"))
                {
                    var listObj = new List<Helper.chartDTO>();
                    var yText = "Yes";
                    var nText="No";
                    if (tipeChart.Contains("osida_hapusbuku_satu_tahun"))
                    {
                        yText = "Selain Hapus Buku";
                        nText = "Hapus Buku";
                    }
                    else if (tipeChart.Contains("osida_kredit_takeover_tidak_bayar")) {
                        yText = "Kredit Takeover Tidak Macet";
                        nText = "Kredit Takeover Macet";
                    }
                    else if (tipeChart.Contains("osida_restrukturisasi_satu_tahun"))
                    {
                        yText = "Bukan Kredit Restrukturisasi";
                        nText = "Kredit Restrukturisasi";
                    }
                    else if (tipeChart.Contains("osida_tidak_bayar_dari_fasilitas_dibentuk"))
                    {
                        yText = "Kredit Tidak Macet";
                        nText = "Kredit Macet";
                    }
                    foreach (DataRow row in result.data.Rows)
                    {
                        if (row["y"]!=DBNull.Value) {
                            
                            listObj.Add(new Helper.chartDTO { dm_base = yText, dm_count = Convert.ToDecimal(row["y"]) });
                        }
                        if (row["n"] != DBNull.Value)
                        {
                            listObj.Add(new Helper.chartDTO { dm_base = nText, dm_count = Convert.ToDecimal(row["n"]) });
                        }
                    }
                    return JsonConvert.SerializeObject(listObj);
                }
                else {
                   
                    return JsonConvert.SerializeObject(result);
                }
            }
            else {
                timeAftter = DateTime.Now;
                var proc = Process.GetCurrentProcess();
                var mem = proc.WorkingSet64 / 1024.0;
                TempData["PM"] = mem;
                TempData["SG"] = (timeAftter - timeNow).TotalSeconds;
                TempData["SD"] = (timeResultAfter - timeResultBefore).TotalSeconds;
                return JsonConvert.SerializeObject(result);
            }
        }

        public object GetGridDataSummary(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            DateTime? datePeriode = null;
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;
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
            }

            if (members != null)
            {
                stringMembers = members;
            }
            if (periode != null)
            {
                datePeriode = Convert.ToDateTime(periode);
            }
            var query = db.getSummaryRowTable(datePeriode, "osida",stringMemberTypes,stringMembers).ToList();
            return DataSourceLoader.Load(query, loadOptions);
        }
        public object GetGridData2(DataSourceLoadOptions loadOptions, string reportId, string memberTypes, string members, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
            var members1 = members;
            if (members != null)
            {
                members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
            }
            string stringMemberTypes = null;
            string stringMembers = null;
            DateTime? datePeriode = null;
            TempData["memberTypeValue"] = null;
            TempData["memberValue"] = null;
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

            if (periode != null)
            {
                TempData["periodeValue"] = periode;
                datePeriode = Convert.ToDateTime(periode);
                TempData["p"] = datePeriode;
            }

            var result = Helper.WSQueryStore.GetOsidaQuery(db, loadOptions, reportId, stringMemberTypes, stringMembers, datePeriode.Value,null,false);
            return JsonConvert.SerializeObject(result);
        }
        public object GetGridDataDetail(DataSourceLoadOptions loadOptions)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var detailID = TempData.Peek("detailID").ToString().Split("~");

            string stringMemberTypes = detailID[1].ToString().Replace(">", "/");
            string stringMembers = detailID[2].ToString().Replace(">","/");
            DateTime? datePeriode =DateTime.Parse( detailID[0].ToString());
            TempData["mt"] = stringMemberTypes;
            TempData["m"] = stringMembers;
            TempData["p"] = datePeriode;
            var result = Helper.WSQueryStore.GetOsidaQuery(db, loadOptions, "osida_plafondering_umum_detail", stringMemberTypes, stringMembers, datePeriode.Value, null, false);
            return JsonConvert.SerializeObject(result);
        }
        #endregion
        public IActionResult GetVisualTypes(DataSourceLoadOptions loadOptions)
        {
            var obj = db.osida_master.Find(TempData.Peek("kodeOsida").ToString());
            var listTipe = new List<Helper.VisualTipeDTO>();
            if (obj.kode == "osida_hapusbuku_satu_tahun" || obj.kode == "osida_kredit_takeover_tidak_bayar" || obj.kode == "osida_restrukturisasi_satu_tahun" || obj.kode == "osida_tidak_bayar_dari_fasilitas_dibentuk")
            {
                listTipe.Add(new Helper.VisualTipeDTO { vis_kode = obj.kode + "_total_plafon", vis_nama ="Berdasarkan Jumlah Total Plafon Awal" });
                listTipe.Add(new Helper.VisualTipeDTO { vis_kode = obj.kode + "_jumlah_rekening", vis_nama = "Berdasarkan Jumlah Fasilitas/Rekening" });
                listTipe.Add(new Helper.VisualTipeDTO { vis_kode = obj.kode + "_jenis_kredit", vis_nama = "Berdasarkan Jenis Kredit" });
                listTipe.Add(new Helper.VisualTipeDTO { vis_kode = obj.kode + "_jenis_penggunaan", vis_nama = "Berdasarkan Jenis Penggunaan" });
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(listTipe.ToList(), loadOptions)), "application/json");
        }
    }
}
