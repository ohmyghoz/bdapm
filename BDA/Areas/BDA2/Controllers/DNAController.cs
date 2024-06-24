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

namespace BDA.Controllers
{
    [Area("BDA2")]
    public class DNAController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public DNAController(DataEntities db, IWebHostEnvironment env)
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
                string stringMembers = null;
                string stringJas = null;
                string stringPeriode = null;
                if (reportId == "edge_collateral")
                {
                    newq.rgq_nama = "DNA Graph Network Export CSV";
                    if (TempData.Peek("jns1") != null)
                    {
                        stringJns = TempData.Peek("jns1").ToString();
                    }
                    if (TempData.Peek("member1") != null)
                    {
                        stringMembers = TempData.Peek("member1").ToString();
                    }
                    if (TempData.Peek("jas1") != null)
                    {
                        stringJas = TempData.Peek("jas1").ToString();
                    }
                    if (TempData.Peek("periode1") != null)
                    {
                        stringPeriode = TempData.Peek("periode1").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDNAQuery(db, stringJns, stringJas, stringMembers, stringPeriode, isHive);
                    //db.CheckPermission("MS02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_DNA_" + reportId, "Export Data", pageTitle);
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
                string stringRn = null;
                string stringFn = null;
                string stringSn = null;
                string stringPeriode = null;
                if (reportId == "edge_collateral")
                {
                    newq.rgq_nama = "DNA Graph Network Detail Export CSV";
                    if (TempData.Peek("rn") != null)
                    {
                        stringRn = TempData.Peek("rn").ToString();
                    }
                    if (TempData.Peek("fn") != null)
                    {
                        stringFn = TempData.Peek("fn").ToString();
                    }
                    if (TempData.Peek("sn") != null)
                    {
                        stringSn = TempData.Peek("sn").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDNADetailQuery(db, stringRn, stringFn, stringSn, stringPeriode, isHive);
                    //db.CheckPermission("MS02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_DNA_" + reportId, "Export Data", pageTitle);
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

            ViewBag.id = "edge_collateral";
            ViewBag.Export = true; // TODO ubah permission disini
            List<string> listP = new List<string>();
            listP.Add(string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)));
            //DateTime a = new DateTime(2021, 1, 1);
            TempData["periode"] = listP.ToArray();
            //TempData["periode"] = string.Format("{0:yyyy-MM-01}", a);
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "edge_collateral");
            ViewBag.Hive = isHive;
            //var rn= Helper.WSQueryStore.GetOsintWCQuery(db, null, "Fasilitas Pinjaman", "Kredit Pinjaman Permodalan KPR", "2021-01-01", isHive);
            //var resourceNames = new List<string>();
            //foreach (DataRow row in rn.data.Rows) {
            //    resourceNames.Add(row["dm_token"].ToString());
            //}
            //var model = new TagCloudAnalyzer()
            //    .ComputeTagCloud(resourceNames)
            //    .Shuffle();
            db.InsertAuditTrail("DGN_Akses_Page", "Akses Page Dashboard DNA Graph Network", pageTitle);
            //db.CheckPermission("CM01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            //ViewBag.Export = db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.NoMessage);
            return View();
        }
        public IActionResult Detail()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            ViewBag.id = "edge_collateral";
            ViewBag.Export = true; // TODO ubah permission disini
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "edge_collateral");
            ViewBag.Hive = isHive;
            db.InsertAuditTrail("DGNDetail_Akses_Page", "Akses Page Dashboard DNA Graph Network Detail", pageTitle);
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            //ViewBag.Export = db.CheckPermission("Size Bisnis Pelapor Export", DataEntities.PermissionMessageType.NoMessage);

            return View();
        }
        [HttpPost]
        public IActionResult redi(string id)
        {
            try
            {
                string[] detailID = JsonConvert.DeserializeObject<string[]>(id);
                TempData["detailID"] = detailID;
                return Json(new { result = "Redirect", url = Url.Action("Detail", "DNA") });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
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
                //db.CheckPermission("MA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "edge_collateral")
                {
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }

                db.InsertAuditTrail("ExportIndex_DGN_" + reportId, "Export Data", pageTitle);
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
                if (reportId == "edge_collateral")
                {
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }

                db.InsertAuditTrail("ExportIndex_DGN_" + reportId, "Export Data", pageTitle);

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
                var fileName = "DGN_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "DGN_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string jns, string ja, string periode, string members, string tipeChart, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear();
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0) {
                //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] JnsS = JsonConvert.DeserializeObject<string[]>(jns);
                string[] JaS = JsonConvert.DeserializeObject<string[]>(ja);


                //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                string stringJns = null;
                string stringJa = null;
                string stringPeriode = null;
                string stringMembers = null;
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "edge_collateral");
                //cekHive = false;
                //cekHive = true;
                if (JnsS.Length > 0)
                {
                    stringJns = string.Join(", ", JnsS);
                    TempData["jns"] = JnsS;
                    TempData["jns1"] = stringJns;
                }
                if (JaS.Length > 0)
                {
                    stringJa = string.Join(", ", JaS);
                    TempData["jas"] = JaS;
                    TempData["jas1"] = stringJa;
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
                if (Members.Length > 0)
                {
                    stringMembers = string.Join(", ", Members);
                    TempData["member"] = JsonConvert.DeserializeObject<string[]>(members);
                    TempData["member1"] = stringMembers;
                }
                var result = Helper.WSQueryStore.GetDNAQuery(db, loadOptions, stringJns, stringJa, stringMembers, stringPeriode, tipeChart, cekHive, isChart);

                return JsonConvert.SerializeObject(result);
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);


        }
        public object GetGridDataDetail(DataSourceLoadOptions loadOptions, bool isChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] detailID = null;
            string rn = null;
            string fn = null;
            string sn = null;
            string p = null;
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "edge_collateral");
            List<string> p1 = new List<string>();
            if (TempData.Peek("detailID") != null)
            {
                detailID = (string[])TempData.Peek("detailID");
                DateTime? period1 = null;
                foreach (var i in detailID)
                {
                    var i2 = i.Split("~");
                    if (p == null)
                    {
                        p = i2[0].ToString();
                        period1 = Convert.ToDateTime(i2[0].ToString().Substring(0, 4) + "-" + i2[0].ToString().Substring(4, 2) + "-01");
                        if (cekHive == true)
                        {
                            p1.Add(string.Format("{0:yyyyMM}", period1));
                        }
                        else
                        {
                            p1.Add(string.Format("{0:yyyy-MM-dd}", period1));
                        }
                    }
                    else
                    {
                        if (p.Contains(i2[0].ToString()) == false)
                        {
                            p = p + ",'" + i2[0].ToString() + "'";
                            period1 = Convert.ToDateTime(i2[0].ToString().Substring(0, 4) + "-" + i2[0].ToString().Substring(4, 2) + "-01");
                            if (cekHive == true)
                            {
                                p1.Add(string.Format("{0:yyyyMM}", period1));
                            }
                            else
                            {
                                p1.Add(string.Format("{0:yyyy-MM-dd}", period1));
                            }
                        }
                    }


                    if (rn == null)
                    {
                        rn = "'" + i2[1].ToString() + "'";
                    }
                    else
                    {
                        rn = rn + ",'" + i2[1].ToString() + "'";
                    }
                    if (fn == null)
                    {
                        fn = "'" + i2[2].ToString() + "'";
                    }
                    else
                    {
                        fn = fn + ",'" + i2[2].ToString() + "'";
                    }
                    if (sn == null)
                    {
                        sn = "'" + i2[3].ToString() + "'";
                    }
                    else
                    {
                        sn = sn + ",'" + i2[3].ToString() + "'";
                    }
                }
            }

            string stringPeriode = null;
            TempData["rn"] = rn;
            TempData["fn"] = fn;
            TempData["sn"] = sn;
            TempData["p"] = p;

            //cekHive = true;
            if (p1.Count != 0)
            {
                stringPeriode = string.Join(", ", p1.ToArray());
            }
            var result = Helper.WSQueryStore.GetDNADetailQuery(db, loadOptions, rn, fn, sn, stringPeriode, cekHive, isChart);

            return JsonConvert.SerializeObject(result);

        }
        public object GetEdgesNodes()
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] detail = null;
            string p = null;
            DateTime? period1 = null;
            string stringPeriode = null;
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "edge_collateral");
            List<string> p1 = new List<string>();
            if (TempData.Peek("detailID") != null)
            {
                detail = (string[])TempData.Peek("detailID");
                foreach (var i in detail)
                {
                    var i2 = i.Split("~");
                    if (p == null)
                    {
                        p = i2[0].ToString();
                        period1 = Convert.ToDateTime(i2[0].ToString().Substring(0, 4) + "-" + i2[0].ToString().Substring(4, 2) + "-01");
                        if (cekHive == true)
                        {
                            p1.Add(string.Format("{0:yyyyMM}", period1));
                        }
                        else
                        {
                            p1.Add(string.Format("{0:yyyy-MM-dd}", period1));
                        }
                    }
                    else
                    {
                        if (p.Contains(i2[0].ToString()) == false)
                        {
                            p = p + ",'" + i2[0].ToString() + "'";
                            period1 = Convert.ToDateTime(i2[0].ToString().Substring(0, 4) + "-" + i2[0].ToString().Substring(4, 2) + "-01");
                            if (cekHive == true)
                            {
                                p1.Add(string.Format("{0:yyyyMM}", period1));
                            }
                            else
                            {
                                p1.Add(string.Format("{0:yyyy-MM-dd}", period1));
                            }
                        }
                    }
                }
            }

            if (p1.Count != 0) {
                stringPeriode = string.Join(", ", p1.ToArray());
            }

            //if (TempData.Peek("periode") != null)
            //{

            //    DateTime period1 = Convert.ToDateTime(TempData.Peek("periode"));
            //    if (cekHive == true)
            //    {
            //        stringPeriode = string.Format("{0:yyyyMM}", period1);
            //    }
            //    else
            //    {
            //        stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
            //    }
            //}
            var result1 = Helper.WSQueryStore.GetDNAENQuery(db, null, detail, stringPeriode, "Node", cekHive);
            var result2 = Helper.WSQueryStore.GetDNAENQuery(db, null, detail, stringPeriode, "Edge", cekHive);
            return Json(new { result = "Success", Nodes = result1, Edges = result2 });

        }
        #endregion

    }
}
