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

namespace CoreTemplate.Controllers
{
    [Area("Master")]
    public class Master_SatkerController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Master_SatkerController(DataEntities db, IWebHostEnvironment env)
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
            var obj = new MasterSatker();
            return View("Edit", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "SatkerId" })]
        public IActionResult Create([Bind("SatkerId", "SatkerKode", "SatkerTipe", "SatkerParentId")] MasterSatker obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.MasterSatker.Where(x => x.Stsrc == "A" && x.SatkerKode == obj.SatkerKode && x.SatkerId != obj.SatkerId).Any())
                {
                    throw new InvalidOperationException("Kode tidak boleh sama dengan data lain");
                }
                db.MasterSatker.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Satker_TambahSatker", "Tambah Data Satker", pageTitle, new object[] { obj }, new string[] { "SatkerId" }, obj.SatkerNama.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
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

            var obj = db.MasterSatker.Find(id);
            if (obj.Stsrc != "A") return NotFound();
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "SatkerId" })]
        public IActionResult Edit([Bind("SatkerId", "SatkerKode", "SatkerTipe", "SatkerParentId")] MasterSatker obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (db.MasterSatker.Where(x => x.Stsrc == "A" && x.SatkerKode == obj.SatkerKode).Any())
                {
                    throw new InvalidOperationException("Kode tidak boleh sama dengan data lain");
                }
                var oldObj = db.MasterSatker.Find(obj.SatkerId);
                WSMapper.CopyFieldValues(obj, oldObj, "SatkerKode,SatkerTipe,SatkerParentId");
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Satker_UbahSatker", "Ubah Data Satker", pageTitle, new object[] { obj }, new string[] { "SatkerId" }, oldObj.SatkerNama.ToString());
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
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var obj = db.MasterSatker.First(o => o.SatkerId == param1);
            //hapus child
            foreach (var chld in db.MasterSatker.Where(x => x.SatkerParentId == param1))
            {
                db.DeleteStsrc(chld);
            }
            db.DeleteStsrc(obj);
            db.SaveChanges();
            db.InsertAuditTrail("Master_Satker_HapusSatker", "Hapus Data Satker", pageTitle, new object[] { obj }, new string[] { "satker_id" }, obj.SatkerNama.ToString());
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);

        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.MasterSatker
                        where q.Stsrc == "A"
                        select new { q.SatkerParentId, q.SatkerTipe, q.SatkerKode, q.SatkerId };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"
        [HttpGet]
        public IActionResult GetRefklas_parent_id(DataSourceLoadOptions loadOptions)
        {
            var list = db.MasterSatker
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.SatkerTipe)
                .Select(x => new { x.SatkerId, x.SatkerKode })
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }

        [HttpGet]
        public IActionResult GetRef_kota_id(DataSourceLoadOptions loadOptions)
        {
            var list = db.MasterKota
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.RefKotaNama)
                .Select(x => new { x.RefKotaId, x.RefKotaNama })
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }

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