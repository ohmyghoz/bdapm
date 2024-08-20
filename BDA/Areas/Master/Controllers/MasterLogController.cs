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
    public class MasterLogController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MasterLogController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        [HttpPost]
        public IActionResult LogExport()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Log Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_MasterLog", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public ActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Master Log View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            //var user = HttpContext.User.Identity.Name;

            //var akses = (from a in db.UserMaster
            //             join b in db.FWUserRole on a.UserId equals b.UserId
            //             where a.Stsrc == "A" && a.UserId == user && b.RoleId == "AdminAplikasi"
            //             select new { b.RoleId }).ToList().Any();
            //ViewBag.Admin = akses;

            ViewBag.Export = db.CheckPermission("Master Log Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("MasterLog_Akses_Page", "Akses Page Master Log", pageTitle);
            return View();
        }

        public object GetGridData(DataSourceLoadOptions loadOptions, string paramStartDate, string paramEndDate, string paramMenu)
        {
            DateTime startDate = Convert.ToDateTime(paramStartDate);
            DateTime endDate = Convert.ToDateTime(paramEndDate);

            if (paramEndDate == null)
            {
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            }
            else
            {
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            }

            if (paramMenu != null)
            {
                var query = from q in db.AuditTrail
                            where q.AuditDate >= startDate && q.AuditDate <= endDate && paramMenu.Contains(q.AuditMenu)
                            select new { q.AuditCause, q.AuditMenu, q.AuditDate, q.AuditDebtorName, q.AuditErrMsg, q.AuditId, q.AuditIpAddress, q.AuditUser, q.AuditPrevUrl, q.AuditUrl, q.AuditTipe };
                return DataSourceLoader.Load(query.ToList(), loadOptions);
            }
            else
            {
                var query = from q in db.AuditTrail
                            where q.AuditDate >= startDate && q.AuditDate <= endDate
                            select new { q.AuditCause, q.AuditMenu, q.AuditDate, q.AuditDebtorName, q.AuditErrMsg, q.AuditId, q.AuditIpAddress, q.AuditUser, q.AuditPrevUrl, q.AuditUrl, q.AuditTipe };
                return DataSourceLoader.Load(query.ToList(), loadOptions);
            }

        }

        public IActionResult GetRefModul(DataSourceLoadOptions loadOptions)
        {
            var list = db.FWRefModul
                .Where(x => x.Stsrc == "A")
                .Select(x => new { x.ModId, x.ParentModId, x.ModKode, x.ModUrut })
                .OrderBy(x => x.ModUrut)
                .ToList();
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }

    }
}

