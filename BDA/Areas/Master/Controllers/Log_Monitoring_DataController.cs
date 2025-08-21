using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
using BDA.Helper.FW;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static BDA.Areas.Master.Controllers.MasterLogController;

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

        public object GetGridData(DataSourceLoadOptions loadOptions, string paramLogDate, string paramStatus)
        {
            List<LogMonData> data = new List<LogMonData>();

            if (paramLogDate != null) {
                string logDate = DateTime.Parse(paramLogDate).ToString("yyyy-MM-dd");
                paramLogDate = logDate;
            }
            

            WSQueryReturns result = Helper.WSQueryStore.GetLogMonData(db, loadOptions, paramLogDate, paramStatus);

            if (result.data.Rows.Count > 0)
            {
                for (int i = 0; i < result.data.Rows.Count; i++)
                {
                    data.Add(new LogMonData()
                    {
                        log_date = result.data.Rows[i]["TglLog"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["TglLog"].ToString() : DateTime.Parse(result.data.Rows[i]["TglLog"].ToString()).ToString("dd MMMM yyyy"),
                        log_kode = result.data.Rows[i]["KodeJob"].ToString(),
                        log_nama = result.data.Rows[i]["NamaJob"].ToString(),
                        log_periode = result.data.Rows[i]["PeriodeData"].ToString(),
                        log_start = result.data.Rows[i]["Mulai"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["Mulai"].ToString() : DateTime.Parse(result.data.Rows[i]["Mulai"].ToString()).ToString("dd MMMM yyyy HH:mm:ss"),
                        log_end = result.data.Rows[i]["Selesai"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["Selesai"].ToString() : DateTime.Parse(result.data.Rows[i]["Selesai"].ToString()).ToString("dd MMMM yyyy HH:mm:ss"),
                        log_total_waktu = result.data.Rows[i]["TotalWaktu"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["TotalWaktu"].ToString() : DateTime.Parse(result.data.Rows[i]["TotalWaktu"].ToString()).ToString("HH:mm:ss"),
                        log_delete_cnt = result.data.Rows[i]["JumlahPenghapusan"].ToString(),
                        log_insert_cnt = result.data.Rows[i]["JumlahInsertData"].ToString(),
                        log_status = enumStatus.GetValueOrDefault(int.Parse(result.data.Rows[i]["Status"].ToString())),
                        log_percentage = result.data.Rows[i]["Persentase"].ToString(),
                    });
                }
            }

            return DataSourceLoader.Load(data, loadOptions);

        }

        public object GetGridDataDetails(DataSourceLoadOptions loadOptions, string paramLogDate, string paramPeriode, string paramJobId)
        {
            List<LogMonDataDetail> data = new List<LogMonDataDetail>();

            if (paramLogDate != null)
            {
                string logDate = DateTime.Parse(paramLogDate).ToString("yyyy-MM-dd");
                paramLogDate = logDate;
            }

            WSQueryReturns result = Helper.WSQueryStore.GetLogMonDataDetails(db, loadOptions, paramLogDate, paramPeriode, paramJobId);

            if (result.data.Rows.Count > 0)
            {
                for (int i = 0; i < result.data.Rows.Count; i++)
                {
                    data.Add(new LogMonDataDetail()
                    {
                        log_date = result.data.Rows[i]["TglLog"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["TglLog"].ToString() : DateTime.Parse(result.data.Rows[i]["TglLog"].ToString()).ToString("dd MMMM yyyy"),
                        log_kode = result.data.Rows[i]["KodeJob"].ToString(),
                        log_seq = result.data.Rows[i]["Urutan"].ToString(),
                        log_nama = result.data.Rows[i]["NamaJob"].ToString(),
                        log_start = result.data.Rows[i]["Mulai"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["Mulai"].ToString() : DateTime.Parse(result.data.Rows[i]["Mulai"].ToString()).ToString("dd MMMM yyyy HH:mm:ss"),
                        log_end = result.data.Rows[i]["Selesai"].ToString().IsNullOrEmpty() ? result.data.Rows[i]["Selesai"].ToString() : DateTime.Parse(result.data.Rows[i]["Selesai"].ToString()).ToString("dd MMMM yyyy HH:mm:ss"),
                        log_delete_cnt = result.data.Rows[i]["JumlahPenghapusan"].ToString(),
                        log_insert_cnt = result.data.Rows[i]["JumlahInsertData"].ToString(),
                        log_status = enumStatus.GetValueOrDefault(int.Parse(result.data.Rows[i]["Status"].ToString())),
                        log_desc = result.data.Rows[i]["Keterangan"].ToString()
                    });
                }
            }
            
            return DataSourceLoader.Load(data, loadOptions);
        }

        public class LogMonData
        {
            public string log_date { get; set; }
            public string log_kode { get; set; }
            public string log_nama { get; set; }
            public string log_periode { get; set; }
            public string log_start { get; set; }
            public string log_end { get; set; }
            public string log_total_waktu { get; set; }
            public string log_delete_cnt { get; set; }
            public string log_insert_cnt { get; set; }
            public string log_status { get; set; }
            public string log_percentage { get; set; }

        }

        public class LogMonDataDetail
        {
            public string log_date { get; set; }
            public string log_kode { get; set; }
            public string log_seq { get; set; }
            public string log_nama { get; set; }
            public string log_start { get; set; }
            public string log_end { get; set; }
            public string log_delete_cnt { get; set; }
            public string log_insert_cnt { get; set; }
            public string log_status { get; set; }
            public string log_desc { get; set; }
        }

        private static readonly Dictionary<int, string> enumStatus = new() 
        {
            { 1, "Initialization/Running" },
            { 2, "Finish - Success" },
            { 3, "Error" },
            { 4, "Others" }
        };

    }
}