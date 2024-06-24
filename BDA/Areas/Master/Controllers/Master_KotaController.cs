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
    public class Master_KotaController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Master_KotaController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        // GET: Ref_Kota
        public ActionResult Index()
        {
            var user = HttpContext.User.Identity.Name;

            var adminAplikasi = db.FWRefRole.FirstOrDefault(x => x.RoleName == "AdminAplikasi").RoleId;

            var akses = (from a in db.UserMaster
                         join b in db.FWUserRole on a.UserId equals b.UserId
                         where a.Stsrc == "A" && a.UserId == user && b.RoleId == adminAplikasi
                         select new { b.RoleId }).ToList().Any();
            ViewBag.Admin = akses;
            return View();
        }

        // GET: Ref_Kota/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //GET
        public IActionResult Create()
        {
            var obj = new MasterKota();
            return View("Edit", obj);
        }

        //POST
        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "RefKotaId" })]
        public IActionResult Create([Bind("RefKotaId", "RefKotaNama", "RefKotaDomisili", "RefPropinsiId")] MasterKota obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.MasterKota.Where(x => x.Stsrc == "A" && x.RefKotaNama == obj.RefKotaNama).Any())
                {
                    throw new InvalidOperationException("Nama kota tidak boleh sama dengan data lain");
                }

                obj.RefKotaNama = obj.RefKotaNama.Trim();
                if (obj.RefKotaDomisili != null)
                {
                    obj.RefKotaDomisili = obj.RefKotaDomisili.Trim();
                }
                db.MasterKota.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Kota_TambahKota", "Tambah Data Kota", pageTitle, new object[] { obj }, new string[] { "RefKotaId" }, obj.RefKotaNama.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data" + " " + "kota");
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
            if (id == null) return BadRequest();

            var obj = db.MasterKota.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        //POST
        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "RefKotaId" })]
        public IActionResult Edit([Bind("RefKotaId", "RefKotaNama", "RefKotaDomisili", "RefPropinsiId")] MasterKota obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.MasterKota.Where(x => x.Stsrc == "A" && x.RefKotaNama == obj.RefKotaNama && x.RefKotaId != obj.RefKotaId).Any())
                {
                    throw new InvalidOperationException("Nama kota tidak boleh sama dengan data lain");
                }

                var oldObj = db.MasterKota.Find(obj.RefKotaId);
                WSMapper.CopyFieldValues(obj, oldObj, "RefKotaNama,RefKotaDomisili,RefPropinsiId");
                oldObj.RefKotaNama = oldObj.RefKotaNama.Trim();
                if (oldObj.RefKotaDomisili != null)
                {
                    oldObj.RefKotaDomisili = oldObj.RefKotaDomisili.Trim();
                }
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Kota_UbahKota", "Ubah Data Kota", pageTitle, new object[] { obj }, new string[] { "RefKotaId" }, oldObj.RefKotaNama.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data" + " " + "kota");
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
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var obj = db.MasterKota.First(o => o.RefKotaId == param1);
            db.DeleteStsrc(obj);
            db.SaveChanges();
            db.InsertAuditTrail("Master_Kota_HapusKota", "Hapus Data Kota", pageTitle, new object[] { obj }, new string[] { "RefKotaId" }, obj.RefKotaNama.ToString());
            var resp = "Berhasil menghapus data kota";
            return new JsonResult(resp);

        }

        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.MasterKota
                        where q.Stsrc == "A"
                        select new { q.RefKotaNama, q.RefKotaDomisili, q.RefKotaId, q.MasterPropinsi.RefPropinsiNama };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"
        [HttpGet]
        public IActionResult GetRefpropinsi_id(DataSourceLoadOptions loadOptions)
        {
            var list = db.MasterPropinsi
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.RefPropinsiNama)
                .Select(x => new { x.RefPropinsiId, x.RefPropinsiNama })
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }

        #endregion
    }
}