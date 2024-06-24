using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using BDA.DataModel;
using System;
using BDA.Helper;

namespace BDA.Controllers
{
    [Area("Master")]
    public class FW_Kode_DetailController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public FW_Kode_DetailController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var obj = new FWKodeDetail();
            return View("Edit", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "KodId" })]
        public IActionResult Create([Bind("KodId", "KofId", "KodTipe", "KodUrut", "KodLength", "KodCatatan", "KodChar", "KodParamKode", "KodParamAsCounter")] FWKodeDetail obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.FWKodeDetail.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("FW_Kode_Detail_TambahKodeDetail", "Tambah Kode Detail", pageTitle, new object[] { obj }, new string[] { "KodId" }, obj.KodTipe.ToString());
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

            var obj = db.FWKodeDetail.Find(id);
           
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "KodId" })]
        public IActionResult Edit([Bind("KodId", "KofId", "KodTipe", "KodUrut", "KodLength", "KodCatatan", "KodChar", "KodParamKode", "KodParamAsCounter")] FWKodeDetail obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var oldObj = db.FWKodeDetail.Find(obj.KodId);
                WSMapper.CopyFieldValues(obj, oldObj, "KofId,KodTipe,KodUrut,KodLength,KodCatatan,KodChar,KodParamKode,KodParamAsCounter");
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("FW_Kode_Detail_UbahKodeDetail", "Ubah Kode Detail", pageTitle, new object[] { obj }, new string[] { "KodId" }, oldObj.KodTipe.ToString());
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

            var obj = db.FWKodeDetail.First(o => o.KodId == param1);
            db.DeleteStsrc(obj);
            db.SaveChanges();
            db.InsertAuditTrail("FW_Kode_Detail_HapusKodeDetail", "Hapus Kode Detail", pageTitle, new object[] { obj }, new string[] { "KodId" }, obj.KodUrut.ToString());
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);

        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWKodeDetail                        
                        select new { q.KodParamAsCounter, q.KodParamKode, q.KodChar, q.KodCatatan, q.KodLength, q.KodUrut, q.KodTipe, q.KofId, q.KodId };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"
        [HttpGet]
        public IActionResult GetRefkof_id(DataSourceLoadOptions loadOptions)
        {
            var list = db.FWKodeFormat
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.KofId)
                .Select(x => new { x.KofId, KofNama = x.KofId })
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