using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Master")]
    public class Master_LJK_TypeController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public Master_LJK_TypeController(DataEntities db, IWebHostEnvironment env)
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

                db.CheckPermission("Master LJK Type Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_LJKType", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
            
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Master LJK Type View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            var dateSync = (from q in db.master_ljk_type
                            select q.sync_date).Max();
            if (dateSync == null)
            {
                ViewBag.lastsync = "";
            }
            else
            {
                ViewBag.lastsync = Convert.ToDateTime(dateSync).ToString("dd MMM yyyy HH:mm:ss");
            }

            ViewBag.Export = db.CheckPermission("Master LJK Type Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("LJKType_Akses_Page", "Akses Page Master LJK Type", pageTitle);
            return View();
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = db.master_ljk_type.Where(x => x.status_aktif == "Y" && x.status_delete == "T").ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        #endregion
    }
}
