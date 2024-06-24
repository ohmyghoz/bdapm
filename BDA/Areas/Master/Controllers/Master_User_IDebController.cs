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
    public class Master_User_IDebController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public Master_User_IDebController(DataEntities db, IWebHostEnvironment env)
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

                db.CheckPermission("Master User IDeb Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_UserIDeb", "Export Data", pageTitle);
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

            db.CheckPermission("Master User IDeb View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var dateSync = (from q in db.master_user_ideb
                            select q.sync_date).Max();
            if (dateSync == null)
            {
                ViewBag.lastsync = "";
            }
            else
            {
                ViewBag.lastsync = Convert.ToDateTime(dateSync).ToString("dd MMM yyyy HH:mm:ss");
            }

            ViewBag.Export = db.CheckPermission("Master User IDeb Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("UserIDeb_Akses_Page", "Akses Page Master User IDeb", pageTitle);
            return View();
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = (from q1 in db.master_user_ideb
                         join a in db.master_ljk_type on q1.member_type_code equals a.kode_jenis_ljk into temp1
                         from q2 in temp1.DefaultIfEmpty()
                         join b in db.master_ljk on new { MEMBER_CODE = q1.member_code, MEMBER_TYPE_CODE = q1.member_type_code } equals new { MEMBER_CODE = b.kode_ljk, MEMBER_TYPE_CODE = b.kode_jenis_ljk } into temp2
                         from q3 in temp2.DefaultIfEmpty()
                         join c in db.master_office_ljk on new { MEMBER_CODE = q1.member_code, MEMBER_TYPE_CODE = q1.member_type_code, OFFICE_CODE = q1.office_code } equals new { MEMBER_CODE = c.kode_ljk, MEMBER_TYPE_CODE = c.kode_jenis_ljk, OFFICE_CODE = c.kode_kantor_cabang } into temp3
                         from q4 in temp3.DefaultIfEmpty()
                         where q1.user_status_flag == "A" &&
                               q2.status_aktif == "Y" && q2.status_delete == "T" &&
                               q3.status_aktif == "Y" && q3.status_delete == "T" &&
                               q4.status_aktif == "Y" && q4.status_delete == "T"
                         select new
                         {
                             q1.user_id,
                             q1.user_login_id,
                             q1.user_name,
                             q1.phone_number,
                             q1.user_type_flag,
                             q1.member_type_code,
                             q2.deskripsi_jenis_ljk,
                             q1.member_code,
                             q3.nama_ljk,
                             q1.office_code,
                             q4.kantor_cabang,
                             q1.employee_id
                         }).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        #endregion
    }
}
