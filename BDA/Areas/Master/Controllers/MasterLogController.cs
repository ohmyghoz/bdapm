using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
using DevExpress.Xpo.DB.Helpers;
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

        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            List<LogMasterData> data = db.DimMasterJobs
                                        .Select((item, index) => new LogMasterData
                                        { 
                                            log_no = index + 1,
                                            log_kode = item.JobId,
                                            log_nama = item.JobName,
                                            log_waktu = item.Scheduler
                                        }).ToList();



            return DataSourceLoader.Load(data, loadOptions);
        }

        public object GetGridDataDetails(DataSourceLoadOptions loadOptions, string paramID)
        {
            string id = paramID;
            List<DimJobProc> datas = db.DimJobProcs.Where(x => x.JobId.Equals(id)).ToList();
            List<LogMasterDataDetail> data = datas
                                        .Select((item, index) => new LogMasterDataDetail
                                        {
                                            log_kode = item.JobId,
                                            log_seq = item.SeqNo,
                                            log_job = item.ProcName,
                                            log_table_src = "",
                                            log_table_dst = "",
                                            log_script = item.ScriptLocation
                                        }).ToList();



            return DataSourceLoader.Load(datas, loadOptions);
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

        public class LogMasterData
        {
            public int log_no { get; set; }
            public string log_kode { get; set; }
            public string log_nama { get; set; }
            public string log_waktu { get; set; }

        }

        public class LogMasterDataDetail
        {
            public string log_kode { get; set; }
            public int log_seq { get; set; }
            public string log_job { get; set; }
            public string log_table_src { get; set;}
            public string log_table_dst { get; set; }
            public string log_script { get; set; }

        }

    }
}

