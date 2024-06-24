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

namespace BDA.Areas.Master.Controllers
{
    [Area("Master")]
    public class Penarikan_DataController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Penarikan_DataController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        // GET: Ref_Kota
        public ActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Penarikan Data View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Add = db.CheckPermission("Penarikan Data Add", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Edit = db.CheckPermission("Penarikan Data Edit", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Delete = db.CheckPermission("Penarikan Data Delete", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("PenarikanData_Akses_Page", "Akses Page Penarikan Data", pageTitle);
            return View();
        }

        //GET
        public IActionResult Create()
        {
            db.CheckPermission("Penarikan Data Add", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            TempData["test"] = null;
            var obj = new Penarikan_Data();
            return View("Edit", obj);
        }

        //POST
        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "mpd_id" })]
        public IActionResult Create([Bind("mpd_id", "mpd_periode", "mpd_jenis")] Penarikan_Data obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.Penarikan_Data.Where(x => x.stsrc == "A" && x.mpd_periode == obj.mpd_periode && x.mpd_jenis==obj.mpd_jenis && x.mpd_status=="Pending").Any())
                {
                    throw new InvalidOperationException("Periode " + obj.mpd_periode + " sudah terdapat pada database ");
                }

                db.Penarikan_Data.Add(obj);
                obj.mpd_status = "Pending";
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("Penarikan_Data_Tambah", "Tambah Data Penarikan Data", pageTitle, new object[] { obj }, new string[] { "Penarikan_Data" }, obj.mpd_id.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("Edit", obj);
        }

        //GET
        public IActionResult Edit(long? id)
        {
            db.CheckPermission("Penarikan Data Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            if (id == null) return BadRequest();

            var obj = db.Penarikan_Data.Find(id);
            if (obj == null) return NotFound();
            TempData["test"] = String.Format("{0:yyyy-MM-dd}", obj.mpd_periode);
            return View(obj);
        }

        //POST
        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "mpd_id" })]
        public IActionResult Edit([Bind("mpd_id", "mpd_periode", "mpd_jenis", "mpd_status")] Penarikan_Data obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";
                
                if (db.Penarikan_Data.Where(x => x.stsrc == "A" && x.mpd_periode == obj.mpd_periode && x.mpd_jenis == obj.mpd_jenis && x.mpd_status == "Pending" && x.mpd_id != obj.mpd_id).Any())
                {
                    throw new InvalidOperationException("Periode " + obj.mpd_periode + " sudah terdapat pada database ");
                }

                var oldObj = db.Penarikan_Data.Find(obj.mpd_id);
                if (oldObj.mpd_status != "Pending")
                {
                    throw new InvalidOperationException("Data tidak boleh diedit");
                }
                oldObj.mpd_periode = obj.mpd_periode;
                oldObj.mpd_jenis = obj.mpd_jenis;
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Penarikan_Data_Ubah", "Ubah Data Penarikan Data", pageTitle, new object[] { obj }, new string[] { "Penarikan_Data" }, oldObj.mpd_id.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(long? param1)
        {
            db.CheckPermission("Penarikan Data Delete", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var obj = db.Penarikan_Data.First(o => o.mpd_id == param1);
            if (obj.mpd_status == "Success") {
                throw new InvalidOperationException("Data ini tidak boleh dihapus karena sudah selesai");
            }
            db.DeleteStsrc(obj);
            db.SaveChanges();
            db.InsertAuditTrail("Penarikan_Data_Hapus", "Hapus Data Penarikan Data", pageTitle, new object[] { obj }, new string[] { "Penarikan_Data" }, obj.mpd_id.ToString());
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);

        }

        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.Penarikan_Data
                        where q.stsrc == "A"
                        select new { q.mpd_id, q.mpd_status,q.mpd_periode,
                            mpd_jenis =
                            (
                                q.mpd_jenis == "hivebda_to_sql" ? "Penarikan Data BDA Tahun 2021-2022" :
                                q.mpd_jenis == "hiveews_to_sql" ? "Penarikan Data EWS Tahun 2021-2022" :
                                q.mpd_jenis == "hivemaster_to_sql" ? "Penarikan Data Master Tahun 2021-2022" :
                                q.mpd_jenis == "tarik1" ? "Penarikan Data BDA Tahun 2022-2023 Tahap 1" :
                                q.mpd_jenis == "tarik2" ? "Penarikan Data BDA Tahun 2022-2023 Tahap 2" :
                                q.mpd_jenis == "tarikan" ? "Penarikan Data BDA Tahun 2023-2024" : "Unknown"
                            )};
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"
        public IActionResult GetPeriode(DataSourceLoadOptions loadOptions)
        {
            var startDate = new DateTime(2019, 1, 1);
            var list = new List<PeriodeDto>();
            while (startDate < DateTime.Now)
            {
                list.Add(new PeriodeDto() { mpd_periode = startDate, text = String.Format("{0:yyyy-MM}", startDate) });
                startDate = startDate.AddMonths(1);
            }
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public class PeriodeDto
        {
            public DateTime mpd_periode { get; set; }
            public string text { get; set; }
        }
        #endregion
    }
}