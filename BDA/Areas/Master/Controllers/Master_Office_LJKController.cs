using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Master")]
    public class Master_Office_LJKController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public Master_Office_LJKController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        [HttpPost]
        public IActionResult LogExport()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Office LJK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_OfficeLJK", "Export Data", pageTitle);
                return Json(new { result = "Success" });
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

            db.CheckPermission("Master Office LJK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var dateSync = (from q in db.master_office_ljk
                            select q.sync_date).Max();
            if (dateSync == null)
            {
                ViewBag.lastsync = "";
            }
            else
            {
                ViewBag.lastsync = Convert.ToDateTime(dateSync).ToString("dd MMM yyyy HH:mm:ss");
            }

            ViewBag.Export = db.CheckPermission("Master Office LJK Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("OfficeLJK_Akses_Page", "Akses Page Master Office LJK", pageTitle);
            return View();
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = (from q1 in db.master_office_ljk
                         join a in db.master_ljk_type on q1.kode_jenis_ljk equals a.kode_jenis_ljk into temp1
                         from q2 in temp1.DefaultIfEmpty()
                         join b in db.master_ljk on new { MEMBER_CODE = q1.kode_ljk, MEMBER_TYPE_CODE = q1.kode_jenis_ljk } equals new { MEMBER_CODE = b.kode_ljk, MEMBER_TYPE_CODE = b.kode_jenis_ljk } into temp2
                         from q3 in temp2.DefaultIfEmpty()
                         where q1.status_aktif == "Y" && q1.status_delete == "T" &&
                         q2.status_aktif == "Y" && q2.status_delete == "T" &&
                         q3.status_aktif == "T" && q2.status_delete == "T"
                         select new
                         {
                             q1.kode_jenis_ljk,
                             q2.deskripsi_jenis_ljk,
                             q1.kode_ljk,
                             q3.nama_ljk,
                             q1.kode_kantor_cabang,
                             q1.kantor_cabang,
                             q1.parent_ljk,
                             q1.parent_jenis_ljk,
                             q1.parent_kantor_cabang
                         }).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        #endregion
    }
}
