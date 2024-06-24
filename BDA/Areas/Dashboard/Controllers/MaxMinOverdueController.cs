using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
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
    [Area("Dashboard")]
    public class MaxMinOverdueController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public MaxMinOverdueController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Kesesuaian Max-Min Overdue per Collectability View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Kesesuaian Max-Min Overdue per Collectability Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("MaxMinOverdue_Akses_Page", "Akses Page Dashboard Kesesuaian Max-Min Overdue per Collectability", pageTitle);

            return View();
        }

        public bool IsPengawasLJK()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("PengawasLJK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] GetFilteredMemberTypes(string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_type_code).Distinct().ToArray();
            return filter;
        }

        public string[] GetFilteredMembers(string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_code).ToArray();
            return filter;
        }

        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Kesesuaian Max-Min Overdue per Collectability Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_MaxMinOverdue", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        [HttpPost]
        public IActionResult LogExportDetail()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Kesesuaian Max-Min Overdue per Collectability Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportDetail_MaxMinOverdue", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult Detail(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            if (id == null) return BadRequest();

            var obj = db.BDA_F01_MaxMinOverdue.Find(id);
            if (obj == null) return NotFound();
            
            ViewBag.Export = db.CheckPermission("Kesesuaian Max-Min Overdue per Collectability Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("MaxMinOverdue_Akses_Detail", "Akses Detail Dashboard Kesesuaian Max-Min Overdue per Collectability", pageTitle);

            return View(obj);
        }

        public IActionResult ExportPDF(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Kesesuaian Max-Min Overdue per Collectability Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_MaxMinOverdue", "Export Data", pageTitle);

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

                    worksheet.Cells.Columns[9].ApplyStyle(textStyle, textFlag);

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
                var fileName = "MaxMinOverdue_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult File()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "MaxMinOverdue_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        public IActionResult Antrian(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var newq = new RptGrid_Queue();
                newq.rgq_tablename = "BDA_F01_MaxMinOverdue";
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "BDA_F01_MaxMinOverdue");
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringJD = null;
                string stringColle = null;
                string stringPeriodeAwal = null;
                string stringPeriodeAkhir = null;
                if (reportId == "max_min_overdue")
                {
                    newq.rgq_nama = "Kesesuaian Max-Min Overdue per Collectability Export CSV";
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
                        stringJD = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("c") != null)
                    {
                        stringColle = TempData.Peek("c").ToString();
                    }
                    if (TempData.Peek("pawal") != null)
                    {
                        stringPeriodeAwal = TempData.Peek("pawal").ToString();
                    }
                    if (TempData.Peek("pakhir") != null)
                    {
                        stringPeriodeAkhir = TempData.Peek("pakhir").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetMaxMinOverdueQuery(db, stringMemberTypes, stringMembers, stringJD, stringColle, stringPeriodeAwal, stringPeriodeAkhir, isHive);
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periodeAwal, string periodeAkhir, string jenisDebitur, string statusCollectability)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            TempData.Clear();
            string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] JenisDebitur = JsonConvert.DeserializeObject<string[]>(jenisDebitur);
            string[] StatusCollectability = JsonConvert.DeserializeObject<string[]>(statusCollectability);

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string stringJD = null;
            string stringColle = null;

            /*check pengawas LJK*/
            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                var filter2 = GetFilteredMembers(login);

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
                stringMemberTypes = string.Join(", ", MemberTypes);
                TempData["mt"] = stringMemberTypes;
            }

            if (Members.Length > 0)
            {
                stringMembers = string.Join(", ", Members);
                TempData["m"] = stringMembers;
            }

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                TempData["pakhir"] = stringPeriodeAkhir;
            }

            if (JenisDebitur.Length > 0)
            {
                stringJD = string.Join(", ", JenisDebitur);
                TempData["jd"] = stringJD;
            }

            if (StatusCollectability.Length > 0)
            {
                stringColle = string.Join(", ", StatusCollectability);
                TempData["c"] = stringColle;
            }

            db.Database.CommandTimeout = 420;

            var query = db.getDataMMOverdue(stringPeriodeAwal, stringPeriodeAkhir, stringMemberTypes, stringMembers, stringJD, stringColle).ToList();
            return DataSourceLoader.Load(query, loadOptions);
        }

        //[HttpGet]
        public object GetDetailData(DataSourceLoadOptions loadOptions, long? id)
        {
            try
            {
                var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                var as1 = db.BDA_F01_MaxMinOverdue.Find(id);

                /*check pengawas LJK*/
                if (IsPengawasLJK())
                {
                    var filter = GetFilteredMemberTypes(login);
                    var filter2 = GetFilteredMembers(login);

                    if (!filter.Contains(as1.member_type_code))
                    {
                        throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk Jenis LJK ini");
                    }

                    if (!filter2.Contains(as1.member_code))
                    {
                        throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk LJK ini");
                    }
                }

                string q = "SELECT account_no,cif,member_code,concat(substr(periode,0,4),'-', substr(periode,5,2) ,'-01') periode,collectibility_type_code,desc_jenis_ljk_byreff,qty_all,qty_distinct_acc_cif,min_overdue_days ,max_overdue_days,sum_outstanding FROM f01_predictive_detail  " + System.Environment.NewLine;
                //string q = "Select * from inquiry_enh_det ied" + System.Environment.NewLine;

                q = q + "WHERE pperiode = '" + as1.periode.ToString("yyyyMM") + "' AND member_type_code='" + as1.member_type_code + "' AND member_code='" + as1.member_code + "' and collectibility_type_code='" + as1.collectibility_type_code + "' and status='" + as1.status + "' " + System.Environment.NewLine;
                q = q + "LIMIT 100000 ";
                //var conn = new OdbcConnection
                //{
                //    ConnectionString = @"DRIVER={Hortonworks Hive ODBC Driver};                                        
                //                        Host=10.225.60.14;
                //                        Port=10500;
                //                        Schema=ojk;
                //                        HiveServerType=2;
                //                        KrbHostFQDN={bgrdco-bddvmst1.ojk.go.id};
                //                        KrbServiceName={hive};
                //                        AuthMech=1;"
                //};
                var conn = new OdbcConnection("DSN=" + db.GetSetting("HiveDSN"));
                //var conn = new OdbcConnection("DSN=hive_secure_table");
                try
                {

                    conn.Open();
                    //conn.ConnectionTimeout = 37;
                    //var adp = new OdbcDataAdapter(q, conn);

                    //var ds = new DataSet();
                    //adp.Fill(ds);
                    //var dt = ds.Tables[0];


                    OdbcCommand myCommand = new OdbcCommand(q, conn);
                    myCommand.CommandTimeout = 1500;
                    DataTable table = new DataTable();
                    table.Load(myCommand.ExecuteReader());
                    var ds = new DataSet();
                    ds.Tables.Add(table);
                    var dt = ds.Tables[0];
                    var query = (from row in dt.AsEnumerable()
                                 select new
                                 {
                                     account_no = Convert.ToString(row["account_no"]),
                                     cif = Convert.ToString(row["cif"]),
                                     member_code = Convert.ToString(row["member_code"]),
                                     periode = Convert.ToString(row["periode"]),
                                     collectibility_type_code = Convert.ToString(row["collectibility_type_code"]),
                                     desc_jenis_ljk_byreff = Convert.ToString(row["desc_jenis_ljk_byreff"]),
                                     qty_all = Convert.ToString(row["qty_all"]),
                                     qty_distinct_acc_cif = Convert.ToString(row["qty_distinct_acc_cif"]),
                                     min_overdue_days = Convert.ToString(row["min_overdue_days"]),
                                     max_overdue_days = Convert.ToString(row["max_overdue_days"]),
                                     sum_outstanding = Convert.ToString(row["sum_outstanding"])
                                 }).ToList();

                    return DataSourceLoader.Load(query, loadOptions);
                }
                catch (Exception ex)
                {
                    //return null;
                    throw new InvalidOperationException(ex.Message);
                    // log.Info("Failed to connect to data source");
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        #endregion

        #region "RefGetter"
        public IActionResult GetMemberTypes(DataSourceLoadOptions loadOptions)
        {
            var login = this.User.Identity.Name;

            var query = db.master_ljk_type.Where(x => x.status_aktif == "Y" && x.status_delete == "T").Select(x => new { x.kode_jenis_ljk, Display = x.kode_jenis_ljk + " - " + x.deskripsi_jenis_ljk }).ToList();

            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                query = query.Where(x => filter.Contains(x.kode_jenis_ljk)).ToList();
            }

            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        public IActionResult GetMembers(DataSourceLoadOptions loadOptions, string memberTypes, bool reset)
        {
            var login = this.User.Identity.Name;

            if (reset)
            {
                loadOptions.Skip = 0;
                loadOptions.Take = 0;
            }

            string[] MemberTypes = string.IsNullOrEmpty(memberTypes) ? new string[] { } : memberTypes.Split(",");

            //var query = db.master_ljk.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, Display = x.kode_ljk + " - " + x.nama_ljk }).ToList();
            var query = db.vw_getMasterLJK.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, x.nama_ljk, x.CompositeKey, x.Display }).ToList();

            if (IsPengawasLJK())
            {
                var filter = GetFilteredMembers(login);
                query = query.Where(x => filter.Contains(x.kode_ljk)).ToList();
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        #endregion


    }
}
