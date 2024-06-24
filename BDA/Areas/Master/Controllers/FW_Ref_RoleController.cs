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

namespace BDA.Controllers
{
    [Area("Master")]
    public class FW_Ref_RoleController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public FW_Ref_RoleController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var user = HttpContext.User.Identity.Name;

            db.CheckPermission("Master Role View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Add = db.CheckPermission("Master Role Add", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Edit = db.CheckPermission("Master Role Edit", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Delete = db.CheckPermission("Master Role Delete", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("MasterRole_Akses_Page", "Akses Page Master Role", pageTitle);
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Mode = "Create";
            var obj = new FWRefRole();
            return View("Edit", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "RoleId" })]
        public IActionResult Create([Bind("RoleId", "RoleName", "RoleCatatan")] FWRefRole obj)
        {
            try
            {
                db.CheckPermission("Master Role Add", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var oldRefRole = db.FWRefRole.Where(x => x.RoleName == obj.RoleName).ToList();
                if (oldRefRole.Where(x => x.Stsrc == "A").Any()) /*check RoleName yang sama dengan status aktif*/
                {
                    throw new InvalidOperationException("Role sudah ada, masukan role yang lain");
                }
               
                if (string.IsNullOrEmpty(obj.RoleCatatan)) /*field RoleCatatan not null*/
                {
                    obj.RoleCatatan = "";
                }

                using (var dbTrans = db.Database.BeginTransaction())
                {
                    var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                    var currentNode = mdl.GetCurrentNode();

                    string pageTitle = currentNode != null ? currentNode.Title : "";

                    if (oldRefRole.Where(x => x.Stsrc == "D").Any()) /*check RoleName sudah pernah ada?*/
                    {
                        var oldobj = db.FWRefRole.FirstOrDefault(x => x.RoleName == obj.RoleName);
                        oldobj.Stsrc = "A";
                        oldobj.RoleCatatan = obj.RoleCatatan;
                        db.SetStsrcFields(oldobj);
                        db.SaveChanges();

                        /*kasih hak akses view ke beranda untuk role yang baru dibuat*/
                        var roleRight = new FWRoleRight();
                        db.FWRoleRight.Add(roleRight);
                        var home = db.FWRefModul.Where(x => x.ModKode == "Beranda").FirstOrDefault().ModId;
                        roleRight.RoleId = oldobj.RoleId;
                        roleRight.ModId = home;
                        roleRight.IsView = true;
                        db.SetStsrcFields(roleRight);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.FWRefRole.Add(obj);
                        db.SetStsrcFields(obj);
                        db.SaveChanges();

                        /*kasih hak akses view ke beranda untuk role yang baru dibuat*/
                        var roleRight = new FWRoleRight();
                        db.FWRoleRight.Add(roleRight);
                        var home = db.FWRefModul.Where(x => x.ModKode == "Beranda").FirstOrDefault().ModId;
                        roleRight.RoleId = db.FWRefRole.Where(x => x.RoleName == obj.RoleName).FirstOrDefault().RoleId;
                        roleRight.ModId = home;
                        roleRight.IsView = true;
                        db.SetStsrcFields(roleRight);
                        db.SaveChanges();
                    }

                    //db.SaveChanges();
                    db.InsertAuditTrail("FW_Ref_Role_TambahRefRole", "Tambah Ref Role", pageTitle, new object[] { obj }, new string[] { "RoleId" }, obj.RoleId.ToString());
                    db.SetSessionString("sctext", "Berhasil menyimpan data");

                    dbTrans.Commit();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("Edit", obj);
        }

        public IActionResult Edit(long? id)
        {
            ViewBag.Mode = "Edit";
            if (id == null) return BadRequest();

            var obj = db.FWRefRole.Find(id);
            if (obj == null) return NotFound();
            if (obj.Stsrc != "A") return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "RoleId" })]
        public IActionResult Edit([Bind("RoleId", "RoleName", "RoleCatatan")] FWRefRole obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Role Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var oldRefRole = db.FWRefRole.Where(x => x.RoleName == obj.RoleName && x.RoleId != obj.RoleId).ToList();
                if (oldRefRole.Where(x => x.Stsrc == "A").Any()) /*check RoleName yang sama dengan status aktif*/
                {
                    throw new InvalidOperationException("Role sudah ada, mohon masukan role yang lain");
                }

                if (string.IsNullOrEmpty(obj.RoleCatatan)) /*field RoleCatatan not null*/
                {
                    obj.RoleCatatan = "";
                }

                
                var oldObj = db.FWRefRole.Find(obj.RoleId);
                WSMapper.CopyFieldValues(obj, oldObj, "RoleId,RoleName,RoleCatatan");
                db.SetStsrcFields(oldObj);
               
                db.SaveChanges();
                db.InsertAuditTrail("FW_Ref_Role_UbahRefRole", "Edit Ref Role", pageTitle, new object[] { obj }, new string[] { "RoleId" }, obj.RoleId.ToString());
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
        public IActionResult Delete(long param1)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Master Role Delete", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            var obj = db.FWRefRole.FirstOrDefault(o => o.RoleId == param1); /*perlu check semua user dengan role ini?*/
            db.DeleteStsrc(obj);

            /*delete semua hak akses untuk role ini (FWRoleRight)*/
            var roleRight = db.FWRoleRight.Where(x => x.RoleId == obj.RoleId).ToList();
            foreach(var right in roleRight)
            {
                db.FWRoleRight.Remove(right);
            }

            db.SaveChanges();

            db.InsertAuditTrail("FW_Ref_Role_HapusRefRole", "Hapus Ref Role", pageTitle, new object[] { obj }, new string[] { "RoleId" }, obj.RoleId.ToString());
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);
        }

        #region "GetGridData"
        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWRefRole
                        where q.Stsrc == "A"
                        select new { q.RoleId, q.RoleName, q.RoleCatatan };
            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion
    }
}