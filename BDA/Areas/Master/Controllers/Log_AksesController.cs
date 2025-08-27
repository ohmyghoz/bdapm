using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using BDA.Helper;
using DevExpress.Xpo.DB.Helpers;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BDA.Areas.Master.Controllers
{
    [Area("Master")]
    public class Log_AksesController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Log_AksesController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public FileResult FileIndex(string name)
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName =  name + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        public IActionResult ExportPDF(IFormFile file, string name)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Log Aktivitas Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_LogAktivitas", "Export Data", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
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
                        Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {

                    Style textStyle = workbook.CreateStyle();
                    textStyle.Copy(style);
                    textStyle.HorizontalAlignment = TextAlignmentType.Right;
                    textStyle.IsTextWrapped = true;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.WrapText = true;
                    textFlag.HorizontalAlignment = true;


                    // Create header style with text wrapping
                    Style headerStyle = workbook.CreateStyle();
                    headerStyle.IsTextWrapped = true; // Enable text wrapping
                    headerStyle.HorizontalAlignment = TextAlignmentType.Center;
                    headerStyle.VerticalAlignment = TextAlignmentType.Center;
                    headerStyle.Font.IsBold = true;
                    headerStyle.Font.Size = 10;

                    StyleFlag headerFlag = new StyleFlag();
                    headerFlag.WrapText = true;
                    headerFlag.HorizontalAlignment = true;
                    headerFlag.VerticalAlignment = true;
                    headerFlag.FontBold = true;
                    headerFlag.FontSize = true;

                    // Apply header style to first row (assuming first row contains headers)
                    Aspose.Cells.Range headerRange = worksheet.Cells.CreateRange(0, 0, 1, worksheet.Cells.MaxDataColumn + 1);
                    headerRange.ApplyStyle(headerStyle, headerFlag);

                    // Alternative: Apply header style only to cells that contain data in first row
                    for (int col = 0; col <= worksheet.Cells.MaxDataColumn; col++)
                    {
                        Cell headerCell = worksheet.Cells[0, col];
                        if (!string.IsNullOrEmpty(headerCell.StringValue))
                        {
                            headerCell.SetStyle(headerStyle, headerFlag);
                        }
                    }

                    foreach (Cell cell in worksheet.Cells)
                    {
                        if (cell.Type != CellValueType.IsNull && cell.Row > 0) // Skip header row
                        {
                            cell.SetStyle(textStyle, textFlag);
                        }
                    }

                    // Auto-fit row height for header row to accommodate wrapped text
                    for (int row = 0; row <= worksheet.Cells.MaxDataRow; row++)
                    {
                        worksheet.AutoFitRow(row);
                    }

                    // Optional: Set minimum row height for header
                    worksheet.Cells.SetRowHeight(0, 30); // Set minimum height to 30 points

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = name + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }


        [HttpPost]
        public IActionResult LogExport()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Log Aktivitas Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_LogAktivitas", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public ActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Log Aktivitas View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            //var user = HttpContext.User.Identity.Name;

            //var akses = (from a in db.UserMaster
            //             join b in db.FWUserRole on a.UserId equals b.UserId
            //             where a.Stsrc == "A" && a.UserId == user && b.RoleId == "AdminAplikasi"
            //             select new { b.RoleId }).ToList().Any();
            //ViewBag.Admin = akses;

            ViewBag.Export = db.CheckPermission("Log Aktivitas Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("LogAktivitas_Akses_Page", "Akses Page Log Aktivitas", pageTitle);
            return View();
        }

        public object GetGridData(DataSourceLoadOptions loadOptions, string paramStartDate, string paramEndDate, string paramMenu, string paramSatker, string paramUserID) 
        {
            DateTime startDate = Convert.ToDateTime(paramStartDate);
            DateTime endDate = Convert.ToDateTime(paramEndDate);

            if (paramEndDate == null)
            {
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            }
            else
            {
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            }

            List<AuditTrail> data = db.AuditTrail.Where(x => x.AuditDate >= startDate && x.AuditDate <= endDate).ToList();

            if (paramMenu != null)
            {
                data = data.Where(x => paramMenu.Contains(x.AuditMenu)).ToList();
            }

            if (paramUserID != null) 
            { 
                data = data.Where(x => x.AuditUser.Equals(paramUserID)).ToList();
            }

            if (paramSatker != null) 
            {
                data = data.Where(x => x.AuditSatker.Equals(paramSatker)).ToList();
            }

            List<LogAksesData> datas = (from x in data
                                       select new LogAksesData
                                       { 
                                           ID = x.AuditId.ToString(),
                                           AuditDate = x.AuditDate,
                                           AuditIpAddress = x.AuditIpAddress,
                                           AuditNIP = x.AuditNip,
                                           AuditUser = x.AuditUser,
                                           AuditSatker = x.AuditSatker,
                                           AuditCause = x.AuditCause,
                                           AuditMenu = x.AuditMenu,
                                           AuditUrl = x.AuditUrl,
                                           AuditIn = x.AuditDataIn,
                                           AuditOut = x.AuditDataOut,
                                           AuditDesc = x.AuditDesc
                                       }).ToList();

            return DataSourceLoader.Load(datas, loadOptions);
        }
        
        public IActionResult GetRefModul(DataSourceLoadOptions loadOptions)
        {
            var list = db.FWRefModul
                .Where(x => x.Stsrc == "A")
                .Select(x => new { x.ModId, x.ParentModId, x.ModKode, x.ModUrut })
                .OrderBy(x => x.ModUrut)
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }

        public class LogAksesData
        {
            public string ID { get; set; }
            public DateTime AuditDate { get; set; }
            public string AuditIpAddress { get; set; }
            public string AuditNIP { get; set; }
            public string AuditUser { get; set; }
            public string AuditSatker { get; set; }
            public string AuditCause { get; set; }
            public string AuditMenu { get; set; }
            public string AuditUrl { get; set; }
            public string AuditIn {  get; set; }
            public string AuditOut { get; set; }
            public string AuditDesc { get; set; }

        }

    }
}