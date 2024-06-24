using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using BDA.DataModel;
using System;
using Microsoft.AspNetCore.Authorization;
using BDA.Helper;

namespace BDA.Controllers
{
    [AllowAnonymous]
    [Area("Master")]
    public class Master_PropinsiController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Master_PropinsiController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
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

        public IActionResult Create()
        {
            ViewBag.Tambah = "Tambah";
            var obj = new MasterPropinsi();
            return View("Edit", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "RefPropinsiId" })]
        public IActionResult Create([Bind("RefPropinsiId", "RefPropinsiNama", "RefPropinsiKode")] MasterPropinsi obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.MasterPropinsi.Where(x => x.Stsrc == "A" && x.RefPropinsiKode == obj.RefPropinsiKode && x.RefPropinsiKode != null).Any())
                {
                    throw new InvalidOperationException("Kode tidak boleh sama dengan data lain");
                }

                if (db.MasterPropinsi.Where(x => x.Stsrc == "A" && x.RefPropinsiNama == obj.RefPropinsiNama).Any())
                {
                    throw new InvalidOperationException("Nama tidak boleh sama dengan data lain");
                }

                obj.RefPropinsiNama = obj.RefPropinsiNama.Trim();
                if (obj.RefPropinsiKode != null)
                {
                    obj.RefPropinsiKode = obj.RefPropinsiKode.Trim();
                }
                var MasterProvinsi = "Data Provinsi";
                db.MasterPropinsi.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Propinsi_TambahPropinsi", "Tambah Data Propinsi", pageTitle, new object[] { obj }, new string[] { "RefPropinsiId" }, obj.RefPropinsiNama.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan" + " " + MasterProvinsi);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("Edit", obj);
        }

        public IActionResult Edit(long? id)
        {
            
            if (id == null) return BadRequest();

            var obj = db.MasterPropinsi.Find(id);
            if (obj.Stsrc != "A") return NotFound();
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "RefPropinsiId" })]
        public IActionResult Edit([Bind("RefPropinsiId", "RefPropinsiNama", "RefPropinsiKode")] MasterPropinsi obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.MasterPropinsi.Where(x => x.Stsrc == "A" && x.RefPropinsiKode == obj.RefPropinsiKode && x.RefPropinsiId != obj.RefPropinsiId && x.RefPropinsiKode != null).Any())
                {
                    throw new InvalidOperationException("Kode tidak boleh sama dengan data lain");
                }

                if (db.MasterPropinsi.Where(x => x.Stsrc == "A" && x.RefPropinsiNama == obj.RefPropinsiNama && x.RefPropinsiId != obj.RefPropinsiId).Any())
                {
                    throw new InvalidOperationException("Nama tidak boleh sama dengan data lain");
                }
                var oldObj = db.MasterPropinsi.Find(obj.RefPropinsiId);
                WSMapper.CopyFieldValues(obj, oldObj, "RefPropinsiNama,RefPropinsiKode");
                oldObj.RefPropinsiNama = oldObj.RefPropinsiNama.Trim();
                if (oldObj.RefPropinsiKode != null) {
                    oldObj.RefPropinsiKode = oldObj.RefPropinsiKode.Trim();
                }
                var MasterProvinsi = "Data Provinsi";
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Propinsi_UbahPropinsi", "Ubah Data Propinsi", pageTitle, new object[] { obj }, new string[] { "RefPropinsiId" }, oldObj.RefPropinsiNama.ToString());
                db.SetSessionString("sctext", "Berhasil Menyimpan" + " " + MasterProvinsi);
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
            
            var obj = db.MasterPropinsi.First(o => o.RefPropinsiId == param1);
            if (db.MasterKota.Where(x => x.Stsrc == "A" && x.RefPropinsiId == obj.RefPropinsiId).Any())
            {
                throw new InvalidOperationException("Provinsi telah termapping dengan kota sehingga tidak bisa dihapus");
            }
            db.DeleteStsrc(obj);
            db.SaveChanges();
            db.InsertAuditTrail("Master_Propinsi_HapusPropinsi", "Hapus Data Propinsi", pageTitle, new object[] { obj }, new string[] { "RefPropinsiId" }, obj.RefPropinsiNama.ToString());
            var resp = "Berhasil menghapus data Propinsi";
            return new JsonResult(resp);

        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.MasterPropinsi
                        where q.Stsrc == "A"
                        select new { q.RefPropinsiKode, q.Entrier, q.RefPropinsiNama, q.RefPropinsiId };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"

        #endregion

        #region "GridEdit"
        /*
        [HttpPost]
        public IActionResult InsertObject(string values)
        {
            var newObject = new DataModel.FW_Ref_Setting();
            JsonConvert.PopulateObject(values, newObject);

            if (!TryValidateModel(newObject))
                return BadRequest(ModelState.GetFullErrorMessage());

            db.FW_Ref_Setting.Add(newObject);
            db.SaveChanges();

            return Ok(newObject);
        }

        [HttpPut]
        public IActionResult UpdateObject(string key, string values)
        {
            var obj = db.FW_Ref_Setting.First(o => o.set_name == key);
            JsonConvert.PopulateObject(values, obj);

            if (!TryValidateModel(obj))
                return BadRequest(ModelState.GetFullErrorMessage());

            db.SaveChanges();

            return Ok(obj);
        }

        [HttpDelete]
        public void DeleteObject(string key)
        {
            var obj = db.FW_Ref_Setting.First(o => o.set_name == key);
            db.FW_Ref_Setting.Remove(obj);
            db.SaveChanges();
        }
		*/
        #endregion
    }
}