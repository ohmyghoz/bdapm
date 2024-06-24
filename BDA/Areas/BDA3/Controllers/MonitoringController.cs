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
//namespace BDA.Helper {
//    public class ColorScheduler
//    {
//        public long id { get; set; }
//        public string text { get; set; }
//        public string color { get; set; }
//    }
    
//}
namespace BDA.Controllers
{
    [Area("BDA3")]
    public class MonitoringController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public MonitoringController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        [HttpPost]
        public IActionResult Antrian()
        {
            try
            {
                string reportId="log_monitoring_bda_slik_det";
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringPeriodes = null;
                string stringKantorCabangs = null;
                var obj = db.osida_master.Find(reportId);
                newq.rgq_nama = obj.menu_nama + " Export CSV";
                
                if (TempData.Peek("p") != null)
                {
                    stringPeriodes = TempData.Peek("p").ToString();
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

                db.InsertAuditTrail("ExportIndex_Monitoring_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        [HttpPost]
        public IActionResult ValueLabel(string periode)
        {
            try
            {
                //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
                DateTime? datePeriode = null;
                string ltb="";
                string lta="";
                if (periode != null) {
                    List<DateTime> lp = new List<DateTime>();
                    if (periode != null)
                    {
                        datePeriode = Convert.ToDateTime(periode);
                        lp.Add((DateTime)datePeriode);
                    }
                    string stringPeriode = null;
                    var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "log_monitoring_bda_slik_det");
                    if (periode != null)
                    {
                        TempData["periodeValue"] = periode;
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
                            stringPeriode = string.Join(", ", periode);
                        }
                        TempData["p"] = stringPeriode;
                    }
                    var result = Helper.WSQueryStore.GetMonitoringQueryNL(db,stringPeriode,"LTA",true);
                    foreach (DataRow row in result.data.Rows)
                    {
                        if (row["lta"] != DBNull.Value)
                        {
                            lta =Convert.ToDecimal(row["lta"]).ToString("n2");
                        }
                        if (row["ltb"] != DBNull.Value)
                        {
                            ltb = "Rp. " + Convert.ToDecimal(row["ltb"]).ToString("n2");
                        }
                    }
                }
               
                
                return Json(new { result = "Success", ltaa = lta, ltba = ltb });
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

            ViewBag.Export = true; // TODO ubah disini
            db.InsertAuditTrail("Monitoring_Akses_Page", "Akses Page Dashboard Monitoring Pengaliran Data", pageTitle);
            db.CheckPermission("Dashboard Monitoring Pengaliran Data SLIK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Dashboard Monitoring Pengaliran Data SLIK Export", DataEntities.PermissionMessageType.NoMessage);
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "log_monitoring_bda_slik_det");
            ViewBag.Hive = isHive;
            ViewBag.period = null;
            return View();
        }
        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";
                db.CheckPermission("Dashboard Monitoring Pengaliran Data SLIK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_Monitoring", "Export Data", pageTitle);
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

                db.CheckPermission( "Dashboard Monitoring Pengaliran Data SLIK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                db.InsertAuditTrail("ExportIndex_Monitoring", "Export Data", pageTitle);

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
                var fileName = "Monitoring_" + timeStamp + ".pdf";
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
            var fileName = "Monitoring_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions,string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            DateTime? datePeriode = null;
            TempData["memberTypeValue"] = null;
            TempData["memberValue"] = null;
            TempData["periodeValue"] = null;
            TempData["kcValue"] = null;
            if (periode !=null)
            {
                
                List<DateTime> lp = new List<DateTime>();
                if (periode != null)
                {
                    datePeriode = Convert.ToDateTime(periode);
                    lp.Add((DateTime)datePeriode);
                }
                string stringPeriode = null;
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "log_monitoring_bda_slik_det");
                if (periode != null)
                {
                    TempData["periodeValue"] = datePeriode;
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
                        stringPeriode = string.Join(", ", datePeriode);
                    }
                    TempData["p"] = stringPeriode;
                }
                var result = Helper.WSQueryStore.GetMonitoringQuery(db, loadOptions,stringPeriode,null,false);
                return JsonConvert.SerializeObject(result);
            }
            else {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }
        public object GetChartData(DataSourceLoadOptions loadOptions, string periode,string tipeChart)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            DateTime? datePeriode = null;
            TempData["periodeValue"] = null;
            if (periode!=null)
            {

                List<DateTime> lp = new List<DateTime>();
                if (periode != null)
                {
                    datePeriode = Convert.ToDateTime(periode);
                    lp.Add((DateTime)datePeriode);
                }

                string stringPeriode = null;
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "log_monitoring_bda_slik_sum");
                if (periode != null)
                {
                    TempData["periodeValue"] = periode;
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
                        stringPeriode = string.Join(", ", periode);
                    }
                    TempData["p"] = stringPeriode;
                }
                var result = Helper.WSQueryStore.GetMonitoringQuery(db, loadOptions,stringPeriode,tipeChart,true);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }
        public object GetScheduleData(DataSourceLoadOptions loadOptions, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            DateTime? datePeriode = null;
            TempData["periodeValue"] = null;
            if (periode !=null)
            {

                List<DateTime> lp = new List<DateTime>();
                if (periode != null)
                {
                    datePeriode = Convert.ToDateTime(periode);
                    lp.Add((DateTime)datePeriode);
                }

                string stringPeriode = null;
                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "log_monitoring_bda_slik_det");
                if (periode != null)
                {
                    TempData["periodeValue"] = periode;
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
                        stringPeriode = string.Join(", ", periode);
                    }
                    TempData["p"] = stringPeriode;
                }
                var result = Helper.WSQueryStore.GetScheduleQuery(db, loadOptions,stringPeriode);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }
        #endregion
    }
}
