using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BDA.Controllers
{
    [Area("Master")]
    public class MappingUserLJKController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MappingUserLJKController(DataEntities db, IWebHostEnvironment env)
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

                db.CheckPermission("Pengawas LJK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_PengawasLJK", "Export Data", pageTitle);
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

            db.CheckPermission("Pengawas LJK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var user = HttpContext.User.Identity.Name;
            var dateSync = (from q in db.vw_PengawasLJK
                            select q.sync_date).Max();
            if (dateSync == null)
            {
                ViewBag.lastsync = "";
            }
            else
            {
                ViewBag.lastsync = Convert.ToDateTime( dateSync).ToString("dd MMM yyyy HH:mm:ss");
            }

            ViewBag.Export = db.CheckPermission("Pengawas LJK Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("PengawasLJK_Akses_Page", "Akses Page Pengawas LJK", pageTitle);
            return View();
        }

       
        #region "GetGridData"
        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.vw_PengawasLJK
                        select new {
                            q.rownum,
                            q.user_login_id,
                            q.orgn_name,
                            q.employee_id,
                            q.phone_number,
                            q.group_name,
                            q.active_flag,
                            q.member_type_code,
                            membertypecode = q.member_type_code + " - " + q.deskripsi_jenis_ljk,
                            membercode = q.member_code + " - " + q.nama_ljk,
                            q.member_code,
                            q.created_datetime,
                            q.created_by,
                            q.updated_datetime,
                            q.updated_by,
                            q.p_date,
                            q.deskripsi_jenis_ljk,
                            q.nama_ljk
                        };
            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion
    }
}