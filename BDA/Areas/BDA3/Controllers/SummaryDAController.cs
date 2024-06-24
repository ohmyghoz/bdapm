using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using BDA.Helper;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static BDA.Controllers.RefController;

namespace BDA.Areas.BDA3.Controllers
{
    [Area("BDA3")]
    public class SummaryDAController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public SummaryDAController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public class SumDto
        {
            public int menu_id { get; set; }
            public string menu_nama { get; set; }
            public string menu_link { get; set; }
        }

        [HttpPost]
        public IActionResult LogExport(string id)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (id == "summary_dqm_per_ljk")
                {
                    db.CheckPermission("DA00 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                    db.InsertAuditTrail("Export_DA_summary_dqm_per_ljk", "Export Data", pageTitle);
                    return Json(new { result = "Success" });
                }
                else
                {
                    db.CheckPermission("DA00 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                    db.InsertAuditTrail("Export_DA_summary_dqm_per_jenis_ljk", "Export Data", pageTitle);
                    return Json(new { result = "Success" });
                }
                
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult ExportPDF(string id, string menuName, string bulanData, string jenisLJK, string kodeLJK, IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("DA00 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_DA_" + id, "Export Data", pageTitle);
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
                    numericStyle.Custom = "#,##0";
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
                    worksheet.Cells.InsertRow(0);


                    if (id == "summary_dqm_per_ljk")
                    {
                        //apply style for additional info
                        Aspose.Cells.Range additionalInfoRange = worksheet.Cells.CreateRange(2, 0, 1, 3);
                        Style additionalInfoStyle = workbook.CreateStyle();
                        additionalInfoStyle.HorizontalAlignment = TextAlignmentType.Center;
                        additionalInfoStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.None;
                        additionalInfoStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                        additionalInfoStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        additionalInfoStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        additionalInfoRange.ApplyStyle(additionalInfoStyle, new StyleFlag() { Borders = true, HorizontalAlignment = true });

                        Aspose.Cells.Range mergeRange = worksheet.Cells.CreateRange(1, 0, 1, 3);
                        Style mergeStyle = workbook.CreateStyle();
                        mergeStyle.HorizontalAlignment = TextAlignmentType.Center;
                        mergeStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        mergeStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
                        mergeStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        mergeStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        mergeRange.ApplyStyle(mergeStyle, new StyleFlag() { Borders = true, HorizontalAlignment = true });

                        worksheet.Cells.UnMerge(1, 0, 2, 3);
                        worksheet.Cells.SetColumnWidth(1, 50);
                        worksheet.Cells.SetColumnWidth(2, 75);
                        pageSetup.TopMargin = 4;

                        worksheet.Cells["A2"].PutValue("Nama Skenario");
                        worksheet.Cells["B2"].PutValue("Jenis LJK");
                        worksheet.Cells["C2"].PutValue("LJK");
                    }
                    else
                    {
                        //apply style for additional info
                        Aspose.Cells.Range mergeRange = worksheet.Cells.CreateRange(1, 0, 1, 3);
                        Style mergeStyle = workbook.CreateStyle();
                        mergeStyle.HorizontalAlignment = TextAlignmentType.Center;
                        mergeStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        mergeStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
                        mergeStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        mergeStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        mergeRange.ApplyStyle(mergeStyle, new StyleFlag() { Borders = true, HorizontalAlignment = true });

                        worksheet.Cells.UnMerge(1, 0, 1, 3);
                        worksheet.Cells.SetColumnWidth(1, 50);
                        worksheet.Cells.SetColumnWidth(2, 75);
                        pageSetup.TopMargin = 4;

                        worksheet.Cells["A2"].PutValue("Bulan Data");
                        worksheet.Cells["B2"].PutValue("Jenis LJK");
                        worksheet.Cells["C2"].PutValue("LJK");
                    }
                    

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, "Export Date: " + timeStamp);

                    if (worksheet.Cells.MaxDataRow > 0)
                    {
                        pageSetup.PrintTitleRows = "$1:$1";
                        pageSetup.IsPercentScale = false;
                    }

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "DA_" + id + "_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult File(string id)
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "DA_" + id + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        public IActionResult Index(string id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";
            if (id == "summary_da")
            {
                db.CheckPermission("DA00 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA00 Export", DataEntities.PermissionMessageType.NoMessage);
                db.InsertAuditTrail("DA_summary_da_Akses_Page", "Akses Page DA00", pageTitle);
                return View("Summary");
            }
            else if (id == "summary_dqm_per_ljk")
            {
                db.CheckPermission("Dashboard Summary Data Assurance Per LJK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("Dashboard Summary Data Assurance Per LJK Export", DataEntities.PermissionMessageType.NoMessage);
                db.InsertAuditTrail("DA_summary_dqm_per_ljk_Akses_Page", "Akses Page Dashboard Summary Data Assurance Per LJK", pageTitle);
                return View("SummaryLJK");
            }
            else
            {
                db.CheckPermission("Dashboard Summary Data Assurance Per Jenis LJK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("Dashboard Summary Data Assurance Per Jenis LJK Export", DataEntities.PermissionMessageType.NoMessage);
                db.InsertAuditTrail("DA_summary_dqm_per_jenis_ljk_Akses_Page", "Akses Page Dashboard Summary Data Assurance Per Jenis LJK", pageTitle);
                return View("SummaryJenisLJK");
            }
        }

        public object GetGridDataSummary(DataSourceLoadOptions loadOptions)
        {
            var list = new List<SumDto>();
            list.Add(new SumDto() { menu_id = 1, menu_nama = "Dashboard Summary Data Assurance Per LJK", menu_link = "summary_dqm_per_ljk" });
            list.Add(new SumDto() { menu_id = 2, menu_nama = "Dashboard Summary Data Assurance Per Jenis LJK", menu_link = "summary_dqm_per_jenis_ljk" });
            return DataSourceLoader.Load(list, loadOptions);
        }

        public object GetGridDataSummaryLJK(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periode)
        {
            string[] Periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (Periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                string stringPeriode = null;

                if (MemberTypes.Length > 0)
                {
                    var listOfJenis = db.master_ljk_type.ToList();
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMemberTypes != "") stringMemberTypes += ", ";
                            stringMemberTypes += find.deskripsi_jenis_ljk;
                        }
                    }
                }
                if (Members.Length > 0)
                {
                    stringMembers = string.Join(", ", Members);
                }
                if (Periodes.Length > 0)
                {
                    stringPeriode = string.Join(", ", Periodes);
                }

                if (stringMemberTypes != null)
                {
                    stringMemberTypes = "'" + stringMemberTypes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'";
                }
                if (stringMembers != null)
                {
                    stringMembers = "'" + stringMembers.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'";
                }
                if (stringPeriode != null)
                {
                    stringPeriode = "'" + stringPeriode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'";
                }

                var query = db.getSummaryLJK(stringMemberTypes, stringMembers, stringPeriode).ToList();

                return JsonConvert.SerializeObject(query);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataSummaryJenisLJK(DataSourceLoadOptions loadOptions, string dqs, string memberTypes, string periode) 
        {
            string[] Periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (Periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] DQs = JsonConvert.DeserializeObject<string[]>(dqs);

                string stringMemberTypes = null;
                string stringDQs = null;
                string stringPeriode = null;

                if (MemberTypes.Length > 0)
                {
                    var listOfJenis = db.master_ljk_type.ToList();
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMemberTypes != "") stringMemberTypes += ", ";
                            stringMemberTypes += find.deskripsi_jenis_ljk;
                        }
                    }
                }
                if (DQs.Length > 0)
                {
                    stringDQs = string.Join(", ", DQs);
                }
                if (Periodes.Length > 0)
                {
                    stringPeriode = string.Join(", ", Periodes);
                }

                if (stringMemberTypes != null)
                {
                    stringMemberTypes = "'" + stringMemberTypes.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'";
                }
                if (stringDQs != null)
                {
                    stringDQs = "'" + stringDQs.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'";
                }
                if (stringPeriode != null)
                {
                    stringPeriode = "'" + stringPeriode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'";
                }

                var query = db.getSummaryJenisLJK(stringDQs, stringMemberTypes, stringPeriode).ToList();

                return DataSourceLoader.Load(query, loadOptions);
            }
            else
            {
                return DataSourceLoader.Load(new List<string>(), loadOptions);
            }
        }

    }
}