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
    public class Master_KeteranganController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Master_KeteranganController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Master Keterangan View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Edit = db.CheckPermission("Master Keterangan Edit", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("MasterKeterangan_Akses_Page", "Akses Page Master Keterangan", pageTitle);
            return View();
        }

      

        public IActionResult Edit(long? id)
        {
            if (id == null) return BadRequest();

            var obj = db.Master_Keterangan.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "mk_id" })]
        public IActionResult Edit([Bind("mk_id", "mk_kode", "mk_keterangan", "mk_deskripsi_export")] Master_Keterangan obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Keterangan Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var oldObj = db.Master_Keterangan.Find(obj.mk_id);
                WSMapper.CopyFieldValues(obj, oldObj, "mk_id,mk_kode,mk_keterangan,mk_deskripsi_export");
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Keterangan_Master_Ubah", "Edit Master Keterangan", pageTitle, new object[] { obj }, new string[] { "mk_id" }, obj.mk_id.ToString());
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
            var query = from q in db.Master_Keterangan
                        where q.Stsrc == "A"
                        //orderby q.mk_menu.ToString()
                        select new { q.mk_id,q.mk_kode,q.mk_keterangan,q.mk_menu,q.mk_deskripsi_export };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"

        #endregion

    }
}