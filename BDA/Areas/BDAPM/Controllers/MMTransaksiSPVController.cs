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
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static BDA.Controllers.SampleGridController;
using System.Xml.Linq;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using Ionic.Zip;
using System.Web;
using System.Reflection;
using DevExpress.Xpo.DB;
using DevExpress.Charts.Native;
using static DevExpress.Data.ODataLinq.Helpers.ODataLinqHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MMTransaksiSPVController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MMTransaksiSPVController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            DateTime periodeAwal = DateTime.Now;
            monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMM");
            yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
            ViewData["monthyearawal"] = monthpawal + " " + yearpawal;
            ViewBag.monthyearawal = monthpawal + " " + yearpawal;
            TempData["monthyearawal"] = monthpawal + " " + yearpawal;

            DateTime periodeAkhir = DateTime.Now;
            monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMM");
            yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
            ViewData["monthyearakhir"] = monthpakhir + " " + yearpakhir;
            ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
            TempData["monthyearakhir"] = monthpakhir + " " + yearpakhir;

            db.CheckPermission("Pola 1 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Pola 1 Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Pola_1_Akses_Page", "Akses Page Pola 1", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("Pola_1_Akses_Page", "user " + userId + " mengakases halaman Pola 1", pageTitle);

            return View();
        }
        public IActionResult GetTradeID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaTradeID>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterTradeId(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               sourcetradeid = bs.Field<string>("sourcetradeid").ToString().Trim()
                           }).OrderBy(bs => bs.sourcetradeid).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["sourcetradeid"].ToString();
                    list.Add(new NamaTradeID() { value = dtList.Rows[i]["sourcetradeid"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public IActionResult GetBondTypeCode(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaBondTypeCode>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterBondTypeCode(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               bondissuertypecode = bs.Field<string>("bondissuertypecode").ToString().Trim()
                           }).OrderBy(bs => bs.bondissuertypecode).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["bondissuertypecode"].ToString();
                    list.Add(new NamaBondTypeCode() { value = dtList.Rows[i]["bondissuertypecode"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public IActionResult GetSourceNameID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaSourceName>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterSourceName(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               sourcename = bs.Field<string>("sourcename").ToString().Trim()
                           }).OrderBy(bs => bs.sourcename).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["sourcename"].ToString();
                    list.Add(new NamaSourceName() { value = dtList.Rows[i]["sourcename"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public IActionResult GetTargetNameID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaTargetName>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterTargetName(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               targetname = bs.Field<string>("targetname").ToString().Trim()
                           }).OrderBy(bs => bs.targetname).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["targetname"].ToString();
                    list.Add(new NamaTargetName() { value = dtList.Rows[i]["targetname"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public IActionResult GetReportTypeID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaReportType>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterReportType(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               reporttypecode = bs.Field<string>("reporttypecode").ToString().Trim()
                           }).OrderBy(bs => bs.reporttypecode).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["reporttypecode"].ToString();
                    list.Add(new NamaReportType() { value = dtList.Rows[i]["reporttypecode"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public IActionResult GetBondLateID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaBondLate>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterBondLate(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               reporttypecode = bs.Field<string>("reporttypecode").ToString().Trim()
                           }).OrderBy(bs => bs.reporttypecode).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["reporttypecode"].ToString();
                    list.Add(new NamaBondLate() { value = dtList.Rows[i]["reporttypecode"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public IActionResult GetBondReportStatusID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaReportStatus>();

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMFilterReportStatus(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               bondreportstatuscode = bs.Field<string>("bondreportstatuscode").ToString().Trim()
                           }).OrderBy(bs => bs.bondreportstatuscode).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["bondreportstatuscode"].ToString();
                    list.Add(new NamaReportStatus() { value = dtList.Rows[i]["bondreportstatuscode"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public class NamaTradeID
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class NamaBondTypeCode
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class NamaSourceName
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class NamaTargetName
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class NamaReportType
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class NamaBondLate
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class NamaReportStatus
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public object GetGridData(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir, 
            string caseid, string tradeid, string bondtypecode, string sourcenameid, string targetnameid, string reporttypeid, string bondlateid, string bondreportid)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] strcaseid = JsonConvert.DeserializeObject<string[]>(caseid);
            string[] strtradeid = JsonConvert.DeserializeObject<string[]>(tradeid);
            string[] strbondtypecode = JsonConvert.DeserializeObject<string[]>(bondtypecode);
            string[] strsourcenameid = JsonConvert.DeserializeObject<string[]>(sourcenameid);
            string[] strtargetnameid = JsonConvert.DeserializeObject<string[]>(targetnameid);
            string[] strreporttypeid = JsonConvert.DeserializeObject<string[]>(reporttypeid);
            string[] strbondlateid = JsonConvert.DeserializeObject<string[]>(bondlateid);
            string[] strbondreportid = JsonConvert.DeserializeObject<string[]>(bondreportid);


            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;

            string stringcaseid= null;
            string stringtradeid = null;
            string stringbondtypecode = null;
            string stringsourcenameid = null;
            string stringtargetnameid = null;
            string stringreporttypeid = null;
            string stringbondlateid = null;
            string stringbondreportid = null; 

            string reportId = "mrkt_mnpltn_prtn_rcgntn"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                TempData["pakhir"] = stringPeriodeAwal;
            }

            if (strcaseid.Length > 0)
            {
                stringcaseid = string.Join(", ", strcaseid);
                TempData["caseid"] = stringcaseid;
            }

            if (strtradeid.Length > 0)
            {
                stringtradeid = string.Join(", ", strtradeid);
                TempData["tradeid"] = stringtradeid;
            }

            if (strbondtypecode.Length > 0)
            {
                stringbondtypecode = string.Join(", ", strbondtypecode);
                TempData["bondtypecode"] = stringbondtypecode;
            }

            if (strsourcenameid.Length > 0)
            {
                stringsourcenameid = string.Join(", ", strsourcenameid);
                TempData["sourcenameid"] = stringsourcenameid;
            }

            if (strtargetnameid.Length > 0)
            {
                stringtargetnameid = string.Join(", ", strtargetnameid);
                TempData["targetnameid"] = stringtargetnameid;
            }

            if (strreporttypeid.Length > 0)
            {
                stringreporttypeid = string.Join(", ", strreporttypeid);
                TempData["reporttypeid"] = stringreporttypeid;
            }

            if (strbondlateid.Length > 0)
            {
                stringbondlateid = string.Join(", ", strbondlateid);
                TempData["bondlateid"] = stringbondlateid;
            }

            if (strbondreportid.Length > 0)
            {
                stringbondlateid = string.Join(", ", strbondreportid);
                TempData["bondreportid"] = stringbondreportid;
            }

            db.Database.CommandTimeout = 1200;
            if (periodeAwal.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMMMTransaksiSPVQuery(db, loadOptions, reportId,  periodeAwal,  periodeAkhir, stringcaseid, stringtradeid,
                    stringbondtypecode, stringsourcenameid, stringtargetnameid, stringreporttypeid, stringbondlateid, stringbondreportid, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }


        #region Export Index
        public FileResult FileIndex()
        {
            var directory = _env.WebRootPath;
            var timeStampAwal = TempData.Peek("timeStampAwal").ToString();
            var timeStampAkhir = TempData.Peek("timeStampAkhir").ToString();
            var fileName = "Pola_1_" + timeStampAwal + " - " + timeStampAkhir + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Pola 1 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Pola_1_Akses_Page", "Export Data Excel Pola_1", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFIndex(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Pola 1 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Pola_1_Akses_Page", "Export Data PDF Pola_1", pageTitle);

                var directory = _env.WebRootPath;
                var timeStampAwal = TempData["pawal"].ToString();
                var timeStampAkhir = TempData["pakhir"].ToString();
                var timeStampFoot = timeStampAwal + " - " +  timeStampAkhir ;
                Workbook workbook = new Workbook(file.OpenReadStream());
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
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
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


                    Style textStylesLeft = workbook.CreateStyle();
                    textStylesLeft.HorizontalAlignment = TextAlignmentType.Left;

                    Style textStylesRight = workbook.CreateStyle();
                    textStylesRight.HorizontalAlignment = TextAlignmentType.Right;

                    StyleFlag textStyleFlag = new StyleFlag();
                    textStyleFlag.HorizontalAlignment = true;


                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[0].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[5].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[6].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[7].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[8].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[9].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[10].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[11].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[12].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[13].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[14].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[15].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[16].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[17].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[18].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[18].Width = 20;
                    worksheet.Cells.Columns[19].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[19].Width = 20;
                    worksheet.Cells.Columns[20].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[21].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[22].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[23].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[24].ApplyStyle(textStyle, textFlag);

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
                    pageSetup.SetFooter(0, timeStampFoot);

                    inFile.Close();
                }

                timeStampAwal = timeStampAwal.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                timeStampAkhir = timeStampAkhir.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStampAwal"] = timeStampAwal;
                TempData["timeStampAkhir"] = timeStampAkhir;
                var fileName = "Pola_1_" + timeStampAwal + " - " + timeStampAkhir+ ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion
    }
}
