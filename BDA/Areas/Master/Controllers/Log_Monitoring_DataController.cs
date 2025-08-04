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
    public class Log_Monitoring_DataController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public Log_Monitoring_DataController(DataEntities db, IWebHostEnvironment env)
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

                db.CheckPermission("Log Monitoring Data Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export_LogMonitoring", "Export Data", pageTitle);
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

            db.CheckPermission("Log Monitoring Data View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            //var user = HttpContext.User.Identity.Name;

            //var akses = (from a in db.UserMaster
            //             join b in db.FWUserRole on a.UserId equals b.UserId
            //             where a.Stsrc == "A" && a.UserId == user && b.RoleId == "AdminAplikasi"
            //             select new { b.RoleId }).ToList().Any();
            //ViewBag.Admin = akses;

            ViewBag.Export = db.CheckPermission("Log Monitoring Data Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("LogMonitoring_Akses_Page", "Akses Page Log Monitoring Data", pageTitle);
            return View();
        }

        public object GetGridData(DataSourceLoadOptions loadOptions, string paramStartDate, string paramEndDate)
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
            var query = from q in db.Log_ETL
                        where q.log_date >= startDate && q.log_date <= endDate
                        select new { q.log_date, q.log_delete_cnt, q.log_end, q.log_id, q.log_insert_cnt, q.log_periode, q.log_start, q.log_tipe, q.log_status, q.log_errmessage };
            return DataSourceLoader.Load(query, loadOptions);
        }

        public class LogMonData
        {
            public DateTime log_date { get; set; }
            public string log_kode { get; set; }
            public string log_nama { get; set; }
            public string log_periode { get; set; }
            public DateTime log_start { get; set; }
            public DateTime log_end { get; set; }
            public string log_total_waktu { get; set; }
            public int log_delete_cnt { get; set; }
            public int log_insert_cnt { get; set; }
            public string log_status { get; set; }
            public double log_percentage { get; set; }

        }

        public class LogMonDataDetail
        {
            public DateTime log_date { get; set; }
            public string log_kode { get; set; }
            public int log_seq { get; set; }
            public string log_nama { get; set; }
            public DateTime log_start { get; set; }
            public DateTime log_end { get; set; }
            public int log_delete_cnt { get; set; }
            public int log_insert_cnt { get; set; }
            public string log_status { get; set; }
            public string log_desc { get; set; }
        }

    }
}