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
    public class Master_MenuController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Master_MenuController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public ActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var user = HttpContext.User.Identity.Name;

            db.CheckPermission("Master Menu View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Add = db.CheckPermission("Master Menu Add", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Edit = db.CheckPermission("Master Menu Edit", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Delete = db.CheckPermission("Master Menu Delete", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("MasterMenu_Akses_Page", "Akses Page Master Menu", pageTitle);
            return View();
        }

        public IActionResult CreateModul()
        {
            ViewBag.Mode = "Create";
            var obj = new FWRefModul();
            return View("EditModul", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "ModId" })]
        public IActionResult CreateModul([Bind("ModId", "ParentModId", "ModKode", "ModCatatan", "ModUrut", "ModIsPublic", "ModIsHidden", "ModTooltip")] FWRefModul obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var oldRefRole = db.FWRefModul.Where(x => x.ModKode == obj.ModKode).ToList();

                if (oldRefRole.Where(x => x.Stsrc == "A").Any()) /*check ModNama yang sama dengan status aktif*/
                {
                    throw new InvalidOperationException("Nama modul sudah digunakan, masukan nama modul lain");
                }

                if (string.IsNullOrEmpty(obj.ModCatatan)) /*field ModCatatan not null*/
                {
                    obj.ModCatatan = "";
                }

                if (obj.ModIsPublic == null)
                {
                    obj.ModIsPublic = false;
                }

                if (obj.ModIsHidden == null)
                {
                    obj.ModIsHidden = false;
                }

                db.FWRefModul.Add(obj);
                obj.ModNama = obj.ModKode;

                db.SetStsrcFields(obj);

                db.SaveChanges();
                db.InsertAuditTrail("Master_Menu_TambahRefModul", "Tambah Ref Modul", pageTitle, new object[] { obj }, new string[] { "ModId" }, obj.ModId.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("EditModul", obj);
        }

        public IActionResult EditModul(long? id)
        {
            ViewBag.Mode = "Edit";
            if (id == null) return BadRequest();

            var obj = db.FWRefModul.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "ModId" })]
        public IActionResult EditModul([Bind("ModId", "ParentModId", "ModKode", "ModCatatan", "ModUrut", "ModIsPublic", "ModIsHidden", "ModTooltip")] FWRefModul obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var oldRefRole = db.FWRefModul.Where(x => x.ModKode == obj.ModKode && x.ModId != obj.ModId).ToList();

                if (oldRefRole.Where(x => x.Stsrc == "A").Any()) /*check ModNama yang sama dengan status aktif*/
                {
                    throw new InvalidOperationException("Nama modul sudah digunakan, masukan nama modul lain");
                }

                if (string.IsNullOrEmpty(obj.ModCatatan)) /*field ModCatatan not null*/
                {
                    obj.ModCatatan = "";
                }

                if (obj.ModIsPublic == null)
                {
                    obj.ModIsPublic = false;
                }

                if (obj.ModIsHidden == null)
                {
                    obj.ModIsHidden = false;
                }

                var oldObj = db.FWRefModul.Find(obj.ModId);
                obj.ModNama = obj.ModKode;
                WSMapper.CopyFieldValues(obj, oldObj, "ModId,ParentModId,ModKode,ModNama,ModCatatan,ModUrut,ModIsPublic,ModIsHidden,ModTooltip");
                db.SetStsrcFields(oldObj);

                db.SaveChanges();
                db.InsertAuditTrail("Master_Menu_UbahRefModul", "Edit Ref Modul", pageTitle, new object[] { obj }, new string[] { "ModId" }, obj.ModId.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View(obj);
        }

        public IActionResult CreateMenu()
        {
            ViewBag.Mode = "Create";
            var obj = new FWRefModulMenu();
            return View("EditMenu", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "ModMenuId" })]
        public IActionResult CreateMenu([Bind("ModMenuId", "ModId", "ModMenuAksi", "ModMenuUrl", "ModMenuIsHidden")] FWRefModulMenu obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                if (obj.ModMenuIsHidden == null)
                {
                    obj.ModMenuIsHidden = false;
                }

                db.FWRefModulMenu.Add(obj);
                db.SetStsrcFields(obj);

                db.SaveChanges();
                db.InsertAuditTrail("Master_Menu_TambahRefMenu", "Tambah Ref Menu", pageTitle, new object[] { obj }, new string[] { "ModMenuId" }, obj.ModMenuId.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("EditMenu", obj);
        }

        public IActionResult EditMenu(long? id)
        {
            ViewBag.Mode = "Edit";
            if (id == null) return BadRequest();

            var obj = db.FWRefModulMenu.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "ModMenuId" })]
        public IActionResult EditMenu([Bind("ModMenuId", "ModId", "ModMenuAksi", "ModMenuUrl", "ModMenuIsHidden")] FWRefModulMenu obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var oldObj = db.FWRefModulMenu.Find(obj.ModMenuId);

                if (obj.ModMenuIsHidden == null)
                {
                    obj.ModMenuIsHidden = false;
                }

                WSMapper.CopyFieldValues(obj, oldObj, "ModMenuId,ModId,ModMenuAksi,ModMenuUrl,ModMenuIsHidden");
                db.SetStsrcFields(oldObj);

                db.SaveChanges();
                db.InsertAuditTrail("Master_Menu_UbahRefMenu", "Edit Ref Menu", pageTitle, new object[] { obj }, new string[] { "ModMenuId" }, obj.ModMenuId.ToString());
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

            if (param1 < 0)
            {
                var obj = db.FWRefModulMenu.FirstOrDefault(o => o.ModMenuId == (param1 * (-1)));
                db.DeleteStsrc(obj);
                db.SaveChanges();

                db.InsertAuditTrail("Master_Menu_HapusRefModulMenu", "Hapus Ref Modul Menu", pageTitle, new object[] { obj }, new string[] { "ModMenuId" }, obj.ModMenuId.ToString());
            }
            else
            {
                var obj = db.FWRefModul.FirstOrDefault(o => o.ModId == param1);

                var objChild = db.FWRefModul.Where(x => x.ParentModId == obj.ModId).ToList();

                if (objChild.Any())
                {
                    foreach (var child in objChild)
                    {
                        db.DeleteStsrc(child);
                    }
                }

                var objMenu = db.FWRefModulMenu.Where(x => x.ModId == obj.ModId).ToList();

                if (objMenu.Any())
                {
                    foreach (var menu in objMenu)
                    {
                        db.DeleteStsrc(menu);
                    }
                }

                db.DeleteStsrc(obj);
                db.SaveChanges();

                db.InsertAuditTrail("Master_Menu_HapusRefModul", "Hapus Ref Modul", pageTitle, new object[] { obj }, new string[] { "ModId" }, obj.ModId.ToString());
            }
           
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.vw_Modul
                        orderby q.ModUrut
                        select new
                        {
                            q.ID,
                            q.ModId,
                            q.ParentModId,
                            q.ModMenuId,
                            q.ModKode,
                            q.ModMenuUrl,
                            ModMenuAksi = (
                                q.ModMenuAksi == "View" ? "Lihat" :
                                q.ModMenuAksi == "Add" ? "Tambah" :
                                q.ModMenuAksi == "Edit" ? "Ubah" :
                                q.ModMenuAksi == "Delete" ? "Hapus" :
                                q.ModMenuAksi == "Review" ? "Review" :
                                q.ModMenuAksi == "Print" ? "Cetak" : null
                            )
                        };

            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion

        #region "RefGetter"
        public IActionResult GetRefModul(DataSourceLoadOptions loadOptions)
        {
            var list = db.FWRefModul
                .Where(x => x.Stsrc == "A")
                .Select(x => new { x.ModId, x.ParentModId, x.ModKode, x.ModUrut })
                .OrderBy(x => x.ModUrut)
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }

        #endregion
    }
}