using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class OutliersFrekuensiAktivitasController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public OutliersFrekuensiAktivitasController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Outliers Frekuensi Aktivitas View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("Outliers Frekuensi Aktivitas Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("OutliersFrekuensiAktivitas_Akses_Page", "Akses Page Dashboard Outliers Frekuensi Aktivitas", pageTitle);

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

                db.CheckPermission("Outliers Frekuensi Aktivitas Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_OutliersFrekuensiAktivitas", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periodeAwal, string periodeAkhir)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            string[] Members = JsonConvert.DeserializeObject<string[]>(members);

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;

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
            }

            if (Members.Length > 0)
            {
                stringMembers = string.Join(", ", Members);
            }

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
            }

            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
            }

            db.Database.CommandTimeout = 420;
            var query = db.getLogOutliers(stringMemberTypes, stringMembers, stringPeriodeAwal, stringPeriodeAkhir).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        public object GetGridData2(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periodeAwal, string periodeAkhir)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            string[] Members = JsonConvert.DeserializeObject<string[]>(members);

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            string stringMemberTypes = null;
            string stringMembers = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;

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
            }

            if (Members.Length > 0)
            {
                stringMembers = string.Join(", ", Members);
            }

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
            }

            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
            }

            db.Database.CommandTimeout = 420;
            var query = db.getLogOutliers(stringMemberTypes, stringMembers, stringPeriodeAwal, stringPeriodeAkhir).ToList();

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
