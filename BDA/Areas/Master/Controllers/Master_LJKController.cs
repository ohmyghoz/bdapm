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
    public class Master_LJKController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public Master_LJKController(DataEntities db, IWebHostEnvironment env)
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

                db.CheckPermission("Master LJK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_LJK", "Export Data", pageTitle);
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

            db.CheckPermission("Master LJK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var dateSync = (from q in db.master_ljk
                            select q.sync_date).Max();
            if (dateSync == null)
            {
                ViewBag.lastsync = "";
            }
            else
            {
                ViewBag.lastsync = Convert.ToDateTime(dateSync).ToString("dd MMM yyyy HH:mm:ss");
            }

            ViewBag.Export = db.CheckPermission("Master LJK Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("LJK_Akses_Page", "Akses Page Master LJK", pageTitle);
            return View();
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = (from q1 in db.master_ljk
                         join a in db.master_ljk_type on q1.kode_jenis_ljk equals a.kode_jenis_ljk into temp1
                         from q2 in temp1.DefaultIfEmpty()
                         where q1.status_aktif == "Y" && q1.status_delete == "T" &&
                         q2.status_aktif == "Y" && q2.status_delete == "T"
                         select new
                         {
                             q1.kode_jenis_ljk,
                             q2.deskripsi_jenis_ljk,
                             q1.kode_ljk,
                             q1.nama_ljk,
                             q1.nama_penanggung_jawab_ljk,
                             q1.nomor_telp_penanggung_jawab,
                             q1.alamat_email_penanggung_jawab,
                             q1.kode_kab,
                             q1.kecamatan,
                             q1.kelurahan,
                             q1.alamat,
                             q1.alamat_website,
                             q1.tahun_bulan_data_terakhir,
                             q1.status_submission_inisial,
                             q1.kode_kantor_cabang
                         }).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        #endregion
    }
}
