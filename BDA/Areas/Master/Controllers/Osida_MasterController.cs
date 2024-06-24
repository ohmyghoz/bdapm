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
    public class osida_masterController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public osida_masterController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Master Osida View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Edit = db.CheckPermission("Master Osida Edit", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("MasterOsida_Akses_Page", "Akses Page Master Osida", pageTitle);
            return View();
        }

      

        public IActionResult Edit(string id)
        {
            if (id == null) return BadRequest();

            var obj = db.osida_master.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "kode" })]
        public IActionResult Edit([Bind("kode", "judul", "skenario", "output", "output_empty", "tindaklanjut", "logic")] osida_master obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Osida Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var oldObj = db.osida_master.Find(obj.kode);
                WSMapper.CopyFieldValues(obj, oldObj, "judul,skenario,output,output_empty,tindaklanjut,logic");
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Osida_Master_UbahOsida", "Edit Master Osida", pageTitle, new object[] { obj }, new string[] { "kode" }, obj.kode.ToString());
                db.SetSessionString("sctext", "Sukses menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View(obj);
        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.osida_master
                        where q.Stsrc == "A"
                        select new { q.logic, q.tindaklanjut, q.output, q.skenario, q.judul, q.kode };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"

        #endregion

    }
}