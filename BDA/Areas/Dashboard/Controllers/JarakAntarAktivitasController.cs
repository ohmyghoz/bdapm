using System;
using System.Collections.Generic;
using System.Data.Entity;
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

    public class JarakAntarAktivitasController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public JarakAntarAktivitasController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Jarak Antar Aktivitas View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("Jarak Antar Aktivitas Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("JarakAntarAktivitas_Akses_Page", "Akses Page Dashboard Jarak Antar Aktivitas", pageTitle);

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

                db.CheckPermission("Jarak Antar Aktivitas Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_JarakAntarAktivitas", "Export Data", pageTitle);
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

                db.CheckPermission("Jarak Antar Aktivitas Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_JarakAntarAktivitas", "Export Data", pageTitle);

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
                var fileName = "JarakAntarAktivitas_" + timeStamp + ".pdf";
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
            var fileName = "JarakAntarAktivitas_" + timeStamp + ".pdf";
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
                newq.rgq_tablename = "BDA_LogOutliers_Avg";
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, "BDA_LogOutliers_Avg");
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringStatus = null;
                string stringPeriodeAwal = null;
                string stringPeriodeAkhir = null;
                if (reportId == "jarak_antar_aktivitas")
                {
                    newq.rgq_nama = "Jarak Antar Aktivitas Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("s") != null)
                    {
                        stringStatus = TempData.Peek("s").ToString();
                    }
                    if (TempData.Peek("pawal") != null)
                    {
                        stringPeriodeAwal = TempData.Peek("pawal").ToString();
                    }
                    if (TempData.Peek("pakhir") != null)
                    {
                        stringPeriodeAkhir = TempData.Peek("pakhir").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetJarakAntarAktivitasQuery(db, stringMemberTypes, stringMembers, stringStatus, stringPeriodeAwal, stringPeriodeAkhir, isHive);
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
        //pakai SP
        //public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periodeAwal, string periodeAkhir, string status)
        //{
        //    var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

        //    string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
        //    string[] Members = JsonConvert.DeserializeObject<string[]>(members);
        //    string[] Status = JsonConvert.DeserializeObject<string[]>(status);

        //    //0101 - 011
        //    //0123456789

        //    Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

        //    string stringMemberTypes = null;
        //    string stringMembers = null;
        //    string stringStatus = null;
        //    string stringPeriodeAwal = null;
        //    string stringPeriodeAkhir = null;

        //    /*check pengawas LJK*/
        //    if (IsPengawasLJK())
        //    {
        //        var filter = GetFilteredMemberTypes(login);
        //        var filter2 = GetFilteredMembers(login);

        //        if (MemberTypes.Length == 0)
        //        {
        //            stringMemberTypes = string.Join(", ", filter);
        //        }

        //        if (Members.Length == 0)
        //        {
        //            stringMembers = string.Join(", ", filter2);
        //        }
        //    }

        //    if (MemberTypes.Length > 0)
        //    {
        //        stringMemberTypes = string.Join(", ", MemberTypes);
        //    }

        //    if (Members.Length > 0)
        //    {
        //        stringMembers = string.Join(", ", Members);
        //    }

        //    if (Status.Length > 0)
        //    {
        //        stringStatus = string.Join(", ", Status);
        //    }

        //    if (periodeAwal != null)
        //    {
        //        stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
        //    }

        //    if (periodeAkhir != null)
        //    {
        //        stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
        //    }

        //    db.Database.CommandTimeout = 600;
        //    var query = db.getLogOutliersAvg(stringMemberTypes, stringMembers, stringPeriodeAwal, stringPeriodeAkhir, stringStatus);

        //    return DataSourceLoader.Load(query, loadOptions);
        //}

        //pakai LINQ
        //public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periodeAwal, string periodeAkhir, string status)
        //{
        //    var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

        //    string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
        //    string[] Members = JsonConvert.DeserializeObject<string[]>(members);
        //    string[] Status = JsonConvert.DeserializeObject<string[]>(status);

        //    //0101 - 011
        //    //0123456789

        //    Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

        //    DateTime startDateVal = Convert.ToDateTime(periodeAwal);
        //    DateTime endDateVal = Convert.ToDateTime(periodeAkhir);

        //    db.Database.CommandTimeout = 600;
        //    var query = (from q1 in db.BDA_LogOutliers_Avg
        //                //join q2 in db.BDA_LogOutliers_Act on new { q1.periode, q1.member_type_code, q1.member_code, q1.user_id } equals new { q2.periode, q2.member_type_code, q2.member_code, q2.user_id } into q2Temp
        //                //from q2 in q2Temp.DefaultIfEmpty()
        //                //join q3 in db.master_ljk on new { q1.member_type_code, q1.member_code } equals new { q3.kode_jenis_ljk, q3.kode_ljk } into q3Temp
        //                //from q3 in q3Temp.DefaultIfEmpty()
        //                 from q2 in db.BDA_LogOutliers_Act.Where(x => q1.periode == x.periode && q1.member_type_code == x.member_type_code && q1.member_code == x.member_code && q1.user_id == x.user_id).DefaultIfEmpty()
        //                 from q3 in db.master_ljk.Where(x => q1.member_type_code == x.kode_jenis_ljk && q1.member_code == x.kode_ljk).DefaultIfEmpty()
        //                 where q1.periode >= startDateVal && q1.periode <= endDateVal
        //                 select new
        //                 {
        //                     q1.rowid,
        //                     q1.periode,
        //                     LJK = q1.member_code + " - " + q3.nama_ljk,
        //                     q1.member_type_code,
        //                     q1.member_code,
        //                     q1.user_id,
        //                     q2.act_count,
        //                     q1.mean_diff,
        //                     status = DbFunctions.Left(q1.status_avg_activity, 8)
        //                 });

        //    /*check pengawas LJK*/
        //    if (IsPengawasLJK())
        //    {
        //        var filter = GetFilteredMemberTypes(login);
        //        var filter2 = GetFilteredMembers(login);

        //        if (MemberTypes.Length == 0)
        //        {
        //            query = query.Where(x => filter.Contains(x.member_type_code));
        //        }

        //        if (Members.Length == 0)
        //        {
        //            query = query.Where(x => filter2.Contains(x.member_code));
        //        }
        //    }

        //    if (MemberTypes.Length > 0)
        //    {
        //        query = query.Where(x => MemberTypes.Contains(x.member_type_code));
        //    }

        //    if (Members.Length > 0)
        //    {
        //        query = query.Where(x => Members.Contains(x.member_code));
        //    }

        //    if (Status.Length > 0)
        //    {
        //        query = query.Where(x => Status.Contains(x.status));
        //    }

        //    return DataSourceLoader.Load(query, loadOptions);
        //}

        //pakai view
        public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periodeAwal, string periodeAkhir, string status)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            TempData.Clear();
            string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            string[] Members = JsonConvert.DeserializeObject<string[]>(members);
            string[] Status = JsonConvert.DeserializeObject<string[]>(status);

            //0101 - 011
            //0123456789

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringStatus = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;

            DateTime startDateVal = Convert.ToDateTime(periodeAwal);
            DateTime endDateVal = Convert.ToDateTime(periodeAkhir);

            db.Database.CommandTimeout = 600;
            var query = (from q in db.vw_JarakAntarAktivitas
                         where q.periode >= startDateVal && q.periode <= endDateVal
                         select new
                         {
                             q.rowid,
                             q.periode,
                             q.LJK,
                             q.member_type_code,
                             q.member_code,
                             q.user_id,
                             q.act_count,
                             q.mean_diff,
                             q.status
                         });

            /*check pengawas LJK*/
            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                var filter2 = GetFilteredMembers(login);

                if (MemberTypes.Length == 0)
                {
                    query = query.Where(x => filter.Contains(x.member_type_code));
                }

                if (Members.Length == 0)
                {
                    query = query.Where(x => filter2.Contains(x.member_code));
                }
            }

            if (MemberTypes.Length > 0)
            {
                query = query.Where(x => MemberTypes.Contains(x.member_type_code));
                stringMemberTypes = string.Join(", ", MemberTypes);
                TempData["mt"] = stringMemberTypes;
            }

            if (Members.Length > 0)
            {
                query = query.Where(x => Members.Contains(x.member_code));
                stringMembers = string.Join(", ", Members);
                TempData["m"] = stringMembers;
            }

            if (Status.Length > 0)
            {
                query = query.Where(x => Status.Contains(x.status));
                stringStatus = string.Join(", ", Status);
                TempData["s"] = stringStatus;
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

            return DataSourceLoader.Load(query, loadOptions);
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
