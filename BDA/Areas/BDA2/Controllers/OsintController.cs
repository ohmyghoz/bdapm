using System;
using System.Collections.Generic;
using System.Data;
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
using Sparc.TagCloud;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace BDA.Helper {
    public class VisualPopup
    {
        public string cleanText { get; set; }
    }
}
namespace BDA.Controllers
{
    [Area("BDA2")]
    public class OsintController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public OsintController(DataEntities db, IWebHostEnvironment env)
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
                string stringJns = null;
                string stringInq = null;
                string stringPeriode = null;
                if (reportId == "osint_scrapping_filtered")
                {
                    newq.rgq_nama = "Osint Export CSV";
                    if(TempData.Peek("jns1") != null) {
                        stringJns = TempData.Peek("jns1").ToString();
                    }
                    if (TempData.Peek("inq1") != null)
                    {
                        stringInq = TempData.Peek("inq1").ToString();
                    }
                    if (TempData.Peek("periode1") != null)
                    {
                        stringPeriode = TempData.Peek("periode1").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetOsintQuery(db,stringJns, stringInq, stringPeriode,isHive);
                    db.CheckPermission("Osint Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_Osint_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        [HttpPost]
        public IActionResult Antrian2(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringJns = null;
                string stringInq = null;
                string stringPeriode = null;
                if (reportId == "osint_scrapping_filtered")
                {
                    newq.rgq_nama = "Osint Detail Export CSV";
                    if (TempData.Peek("jns1") != null)
                    {
                        stringJns = TempData.Peek("jns1").ToString();
                    }
                    if (TempData.Peek("inq1") != null)
                    {
                        stringInq = TempData.Peek("inq1").ToString();
                    }
                    if (TempData.Peek("periode1") != null)
                    {
                        stringPeriode = TempData.Peek("periode1").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetOsintQueryDetail(db, stringJns, stringInq, stringPeriode);
                    db.CheckPermission("Osint Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_Osint_" + reportId, "Export Data", pageTitle);
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

            ViewBag.id = "osint_scrapping_filtered";
            ViewBag.Export = true; // TODO ubah permission disini
            List<string> listP = new List<string>();
            listP.Add(string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)));
            //DateTime a = new DateTime(2021, 1, 1);
            TempData["periode"] = listP.ToArray();
            TempData["periode1"] = string.Join(", ", listP.ToArray());
            
            //TempData["periode"] = string.Format("{0:yyyy-MM-01}", a);
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "osint_scrapping_filtered");
            //bool isHive = false;
            ViewBag.Hive = isHive;
            //var rn= Helper.WSQueryStore.GetOsintWCQuery(db, null, "Fasilitas Pinjaman", "Kredit Pinjaman Permodalan KPR", "2021-01-01", isHive);
            //var resourceNames = new List<string>();
            //foreach (DataRow row in rn.data.Rows) {
            //    resourceNames.Add(row["dm_token"].ToString());
            //}
            //var model = new TagCloudAnalyzer()
            //    .ComputeTagCloud(resourceNames)
            //    .Shuffle();
            db.InsertAuditTrail("Osint_Akses_Page", "Akses Page Dashboard Osint", pageTitle);
            db.CheckPermission("Osint View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Osint Export", DataEntities.PermissionMessageType.NoMessage);
            return View();
        }
        [HttpPost]
        public IActionResult redi(string id)
        {
            try
            {
                
                string[] detailID = JsonConvert.DeserializeObject<string[]>(id);
                TempData["id"] = detailID;
                return Json(new { result = "Redirect", url = Url.Action("Detail", "Osint") });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        [HttpPost]
        public IActionResult TempPopup(string id)
        {
            try
            {
                string[] detailID = JsonConvert.DeserializeObject<string[]>(id);
                TempData["ct"] = detailID;
                var resp = "Sukses";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        [HttpGet]
        public IActionResult PopupBody()
        {
            var osft = new BDA.Helper.VisualPopup();
            string ct = null;
            if (TempData.Peek("ct") != null) {
                ct = TempData.Peek("ct").ToString();
            }
            osft.cleanText = ct;
            return View(osft);
        }
        public IActionResult Detail()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            //if (id == null) return BadRequest();
            string[] detailID = null;
            if (TempData.Peek("id") != null) {
                detailID = (string[])TempData.Peek("id");
            }
            //TempData.Clear();
            var sID = detailID[0].Split("~");
            TempData["i"] = sID[2].ToString();
            TempData["j"] = sID[1].ToString().Split(",") ;
            var dtPeriode = sID[0].ToString();
            TempData["p"] = dtPeriode.Substring(0, 4) + "-" + dtPeriode.Substring(4, 2) + "-01";
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "osint_tfidf_wordcloud");
            string stringPeriode = null;
            DateTime period1 = Convert.ToDateTime(dtPeriode.Substring(0, 4) + "-" + dtPeriode.Substring(4, 2) + "-01");
            if (cekHive == true)
            {
                stringPeriode = string.Format("{0:yyyyMM}", period1);
            }
            else
            {
                stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
            }
            ViewBag.id = "osint_scrapping_filtered";
            ViewBag.Export = true; // TODO ubah permission disini
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "osint_scrapping_filtered");
            ViewBag.Hive = isHive;
            var rn = Helper.WSQueryStore.GetOsintWCQuery(db, null, sID[1].ToString(), sID[2].ToString(), stringPeriode, cekHive);
            var resourceNames = new List<string>();
            foreach (DataRow row in rn.data.Rows)
            {
                resourceNames.Add(row["dm_token"].ToString());
            }
            var model = new TagCloudAnalyzer()
                .ComputeTagCloud(resourceNames)
                .Shuffle();
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("Osint Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("OsintDetail_Akses_Page", "Akses Page Dashboard Osint Detail", pageTitle);
            TempData["detailID"] = detailID[0];
            return View(model);
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
                db.CheckPermission("Osint Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_Osint_" + reportId, "Export Data", pageTitle);
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
                if (reportId == "osint_scrapping_filtered")
                {
                    db.CheckPermission("Osint Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                
                db.InsertAuditTrail("ExportIndex_Osint_" + reportId, "Export Data", pageTitle);

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
                var fileName = "Osint_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "Osint_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions,string jns,string inq, string periode, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear();

            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0) {
                string[] JnsS = JsonConvert.DeserializeObject<string[]>(jns);

                //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                string stringJns = null;
                string stringPeriode = null;
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "osint_scrapping_filtered");
                //cekHive = false;
                //cekHive = true;
                if (JnsS.Length > 0)
                {
                    stringJns = string.Join(", ", JnsS);
                    TempData["jns"] = JnsS;
                    TempData["jns1"] = stringJns;
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
                    TempData["periode"] = periodes;
                    TempData["periode1"] = stringPeriode;
                }
                TempData["inq"] = inq;
                TempData["inq1"] = inq;
                var result = Helper.WSQueryStore.GetOsintQuery(db, loadOptions,  stringJns,inq,stringPeriode, cekHive, isChart);

                return JsonConvert.SerializeObject(result);
            }
            //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }
        public object GetGridDataDetail(DataSourceLoadOptions loadOptions, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var detailID = TempData.Peek("detailID").ToString().Split("~");
            string inq = detailID[2].ToString().Replace(">", "/").Replace("|", "-");
            string jns = detailID[1].ToString();
            string periode = detailID[0].ToString().Substring(0, 4) + "-" + detailID[0].ToString().Substring(4, 2) + "-01";
            TempData["jns1"] = jns;
            TempData["inq1"] = inq;
            string stringPeriode = null;
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "osint_scrapping_filtered");
            //cekHive = false;
            //cekHive = true;
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
                TempData["periode1"] = stringPeriode;
            }
            var result = Helper.WSQueryStore.GetOsintQueryDetail(db, loadOptions, jns, inq, stringPeriode, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        public object GetEdgesNodes()
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] JnsS = null;
            if (TempData.Peek("j") != null) {
                JnsS = (string[])TempData.Peek("j")  ;
            }
            string inq = null;
            string stringJns = null;
            string stringPeriode = null;
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "osint_node_wordpair");
            //cekHive = false;
            if (JnsS !=null)
            {
                stringJns = string.Join(", ", JnsS);
            }
            if (TempData.Peek("i") != null) {
                inq = TempData.Peek("i").ToString();
            }
            if (TempData.Peek("p") != null)
            {
                
                DateTime period1 = Convert.ToDateTime(TempData.Peek("p"));
                if (cekHive == true)
                {
                    stringPeriode = string.Format("{0:yyyyMM}", period1);
                }
                else
                {
                    stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
                }
            }
            
            //cekHive = true;
            //if (JnsS.Length > 0)
            //{
            //    stringJns = string.Join(", ", JnsS);
            //    TempData["jns"] = stringJns;
            //}
            //if (periode != null)
            //{
            //    DateTime period1 = Convert.ToDateTime(periode);
            //    if (cekHive == true)
            //    {
            //        stringPeriode = string.Format("{0:yyyyMM}", period1);
            //    }
            //    else
            //    {
            //        stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
            //    }
            //    TempData["p"] = stringPeriode;
            //}
            //stringPeriode = "2021-01-01";
            //var result1 = Helper.WSQueryStore.GetOsintENQuery(db, null, stringJns, inq, stringPeriode, "Node", Helper.WSQueryStore.IsPeriodInHive(db, "osint_node_wordpair"));
            //var result2 = Helper.WSQueryStore.GetOsintENQuery(db, null, stringJns, inq, stringPeriode, "Edge", Helper.WSQueryStore.IsPeriodInHive(db, "osint_edge_wordpair"));
            var result1 = Helper.WSQueryStore.GetOsintENQuery(db, null, stringJns, inq, stringPeriode, "Node", cekHive);
            var result2 = Helper.WSQueryStore.GetOsintENQuery(db, null, stringJns, inq, stringPeriode, "Edge", cekHive);
            return Json(new { result = "Success",Nodes=result1,Edges=result2 });

        }
        #endregion

    }
}
