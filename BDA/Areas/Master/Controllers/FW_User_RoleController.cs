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
    public class FW_User_RoleController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public FW_User_RoleController(DataEntities db, IWebHostEnvironment env)
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
            var obj = new FWUserRole();
            return View("Edit", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "UroleId" })]
        public IActionResult Create([Bind("UroleId", "UroleId", "UserId", "RoleId", "PostId")] FWUserRole obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.FWUserRole.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("FW_User_Role_TambahUserRole", "Tambah User Role", pageTitle, new object[] { obj }, new string[] { "UroleId" }, obj.RoleId.ToString());
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

            var obj = db.FWUserRole.Find(id);
            if (obj.Stsrc != "A") return NotFound();
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "UroleId" })]
        public IActionResult Edit([Bind("UroleId", "UserId", "RoleId", "PostId")] FWUserRole obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var oldObj = db.FWUserRole.Find(obj.UroleId);
                WSMapper.CopyFieldValues(obj, oldObj, "UserId,RoleId,PostId");
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("FW_User_Role_UbahUserRole", "Edit User Role", pageTitle, new object[] { obj }, new string[] { "UroleId" }, oldObj.RoleId.ToString());
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

            var obj = db.FWUserRole.FirstOrDefault(o => o.UroleId == param1);
            db.DeleteStsrc(obj);
            db.SaveChanges();
            db.InsertAuditTrail("FW_User_Role_HapusUserRole", "Hapus User Role", pageTitle, new object[] { obj }, new string[] { "UroleId" }, obj.RoleId.ToString());
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);

        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWUserRole
                        where q.Stsrc == "A"
                        select new { q.RoleId, q.UserId, q.UroleId };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"
        [HttpGet]
        public IActionResult GetRefuser_id(DataSourceLoadOptions loadOptions)
        {
            var list = db.UserMaster
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.UserNama)
                .Select(x => new { x.UserId, x.UserNama })
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        [HttpGet]
        public IActionResult GetRefrole_id(DataSourceLoadOptions loadOptions)
        {
            var list = db.FWRefRole
                .Where(x => x.Stsrc == "A")
                .OrderBy(x => x.RoleId)
                .Select(x => new { x.RoleId, role_nama = x.RoleId})
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