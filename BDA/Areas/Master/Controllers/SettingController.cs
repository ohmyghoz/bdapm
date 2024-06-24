using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using BDA.DataModel;
using System;

namespace BDA.Controllers
{
    [Area("Master")]
    public class SettingController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public SettingController(DataEntities db, IWebHostEnvironment env)
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
            var obj = new FWRefSetting();
            return View("Edit", obj);
        }

        [HttpPost]
        //[TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "set_name" })]        
        public IActionResult Create([Bind("SetName", "SetValue", "SetType")] FWRefSetting obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.FWRefSetting.Add(obj);
                //db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Setting_CreateSetting", "Tambah Data Settingr", pageTitle, new object[] { obj }, new string[] { "SetName" });
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("Edit", obj);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return BadRequest();

            var obj = db.FWRefSetting.Find(id);
         
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        //[TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "set_name" })]        
        public IActionResult Edit([Bind("SetName", "SetValue", "SetType")] FWRefSetting obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var oldObj = db.FWRefSetting.Find(obj.SetName);
                WSMapper.CopyFieldValues(obj, oldObj, new[] { "SetName", "SetValue", "SetType" });
                //db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Master_Setting_EditSetting", "Edit Data Settingr", pageTitle, new object[] { obj }, new string[] { "SetName" });
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
        public IActionResult Delete(string param1)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var obj = db.FWRefSetting.First(o => o.SetName == param1);
            db.FWRefSetting.Remove(obj);
            db.SaveChanges();
            db.InsertAuditTrail("Master_Setting_DeleteSetting", "Hapus Data Settingr", pageTitle, new object[] { obj }, new string[] { "SetName" });
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);

        }
        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWRefSetting  select q;
            return DataSourceLoader.Load(query, loadOptions);
        }


        #region "RefGetter"
        [HttpGet]
        public IActionResult GetRefset_type(DataSourceLoadOptions loadOptions)
        {
            var list = db.MasterKota
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.RefKotaNama)
                .Select(x => new { RefKotaId = x.RefKotaId.ToString(), x.RefKotaNama })
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        #endregion
    }
}