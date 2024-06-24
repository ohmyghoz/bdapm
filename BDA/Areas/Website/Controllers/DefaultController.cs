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
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using DevExpress.DataProcessing;
using DevExpress.Data.Helpers;
using DevExpress.DataAccess.Native.Web.DataContracts;

namespace BDA.Areas.Dashboard.Controllers
{
    [Area("Website")]
    public class DefaultController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public DefaultController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public ActionResult Index()
        {
            var user = HttpContext.User.Identity.Name;
            var usr = db.UserMaster.Find(user);

            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.InsertAuditTrail("Beranda_Akses_Page", "Akses Page Beranda", pageTitle);
            ViewBag.Edit = db.CheckPermission("Beranda Edit", DataEntities.PermissionMessageType.NoMessage);

            return View(usr);
        }

        [HttpPost]
        public IActionResult RedChange(bool redCheck) //Fungsi Untuk Escalate Claim
        {
            db.CheckPermission("Beranda Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var resp = "";
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var user = HttpContext.User.Identity.Name;
                var usr = db.UserMaster.Find(user);
                usr.user_is_notifredalert = redCheck;
                db.SaveChanges();
                db.InsertAuditTrail("UbahRedAlertEmail", "Ubah Red Alert Email", pageTitle, new object[] { usr }, new string[] { "UserId" }, usr.UserId.ToString());
                //var nott = new BusinessLayer.NoticeTemplateHelper(db, "Claim", obj);
                //nott.GenerateNotice(true);
                resp = "Success Saving data";
               
                return new JsonResult(resp);
                //return Ok();
            }
            catch (Exception ex)
            {
                resp = db.ProcessExceptionMessage(ex);
                return new JsonResult(resp);
            }
        }
        [HttpPost]
        public IActionResult YellowChange(bool yellowCheck) //Fungsi Untuk Escalate Claim
        {
            db.CheckPermission("Beranda Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            var resp = "";
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                var user = HttpContext.User.Identity.Name;
                var usr = db.UserMaster.Find(user);
                usr.user_is_notifyellowalert = yellowCheck;
                db.SaveChanges();
                db.InsertAuditTrail("UbahYellowAlertEmail", "Ubah Yellow Alert Email", pageTitle, new object[] { usr }, new string[] { "UserId" }, usr.UserId.ToString());
                //var nott = new BusinessLayer.NoticeTemplateHelper(db, "Claim", obj);
                //nott.GenerateNotice(true);
                resp = "Success Saving data";

                return new JsonResult(resp);
                //return Ok();
            }
            catch (Exception ex)
            {
                resp = db.ProcessExceptionMessage(ex);
                return new JsonResult(resp);
            }
        }
    }

}