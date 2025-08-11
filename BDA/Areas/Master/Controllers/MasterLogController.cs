using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
using BDA.Helper.FW;
using DevExpress.Xpo.DB.Helpers;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.IO;

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
            List<LogMasterData> data = new List<LogMasterData>();

            WSQueryReturns result = Helper.WSQueryStore.GetMasterLogData(db, loadOptions);

            if (result.data.Rows.Count > 0)
            {
                for (int i = 0; i < result.data.Rows.Count; i++)
                {
                    data.Add(new LogMasterData() 
                    { 
                        log_no = i+1, 
                        log_kode = result.data.Rows[i]["job_id"].ToString(), 
                        log_nama = result.data.Rows[i]["job_name"].ToString(), 
                        log_waktu = result.data.Rows[i]["scheduler"].ToString() 
                    });
                }
            }

            return DataSourceLoader.Load(data, loadOptions);
        }

        public object GetGridDataDetails(DataSourceLoadOptions loadOptions, string paramID)
        {
            List<LogMasterDataDetail> data = new List<LogMasterDataDetail>();

            WSQueryReturns result = Helper.WSQueryStore.GetMasterLogDataDetail(db, loadOptions, paramID);

            if (result.data.Rows.Count > 0)
            {
                for (int i = 0; i < result.data.Rows.Count; i++)
                {
                    data.Add(new LogMasterDataDetail() 
                    { 
                        log_kode = result.data.Rows[i]["KodeJob"].ToString(), 
                        log_seq = result.data.Rows[i]["UrutanProses"].ToString(), 
                        log_job = result.data.Rows[i]["NamaJob"].ToString(),
                        log_table_src = result.data.Rows[i]["TblSrc"].ToString(),
                        log_table_dst = result.data.Rows[i]["TblDst"].ToString(),
                        log_script = result.data.Rows[i]["LokScript"].ToString(),
                    });
                }
            }

            return DataSourceLoader.Load(data, loadOptions);
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

        public IActionResult LogExportTemplate()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Log Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export Master Log", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult GetTemplate() 
        {
            DataSourceLoadOptions loadOptions = new DataSourceLoadOptions();
            List<MasterData> data = new List<MasterData>();
            List<LogMasterDataDetail> dataDetail = new List<LogMasterDataDetail>();
            string templateFile = "Template_MasterLog_BDAPM.xlsx";
            string templatePath = Path.Combine(_env.WebRootPath, "Template", templateFile);
            string outputPath = _env.WebRootPath;

            WSQueryReturns result = Helper.WSQueryStore.GetMasterLogData(db, loadOptions);

            if (result.data.Rows.Count > 0)
            {
                for (int i = 0; i < result.data.Rows.Count; i++)
                {
                    data.Add(new MasterData()
                    {
                        log_kode = result.data.Rows[i]["job_id"].ToString(),
                        log_nama = result.data.Rows[i]["job_name"].ToString(),
                        log_waktu = result.data.Rows[i]["scheduler"].ToString()
                    });
                }
            }

            foreach (var item in data)
            {
                WSQueryReturns resultDetail = Helper.WSQueryStore.GetMasterLogDataDetail(db, loadOptions, item.log_kode);

                if (resultDetail.data.Rows.Count > 0)
                {
                    for (int i = 0; i < resultDetail.data.Rows.Count; i++)
                    {
                        dataDetail.Add(new LogMasterDataDetail()
                        {
                            log_kode = resultDetail.data.Rows[i]["KodeJob"].ToString(),
                            log_seq = resultDetail.data.Rows[i]["UrutanProses"].ToString(),
                            log_job = resultDetail.data.Rows[i]["NamaJob"].ToString(),
                            log_table_src = resultDetail.data.Rows[i]["TblSrc"].ToString(),
                            log_table_dst = resultDetail.data.Rows[i]["TblDst"].ToString(),
                            log_script = resultDetail.data.Rows[i]["LokScript"].ToString(),
                        });
                    }
                }
            }

            using (var workbook = new XLWorkbook(templatePath))
            {
                var sheetData = workbook.Worksheet("Sheet1");
                var sheetDetail = workbook.Worksheet("Sheet2");

                sheetData.Cell("A2").InsertData(data);
                sheetDetail.Cell("A2").InsertData(dataDetail);

                string timestamp = DateTime.Now.ToString();
                timestamp = timestamp.Replace("/", "").Replace(" ", "_").Replace(":", "");
                var fileName = "MasterLog_" + timestamp + ".xlsx";
                outputPath = Path.Combine(outputPath, fileName);

                workbook.SaveAs(outputPath);
                var fileByte = System.IO.File.ReadAllBytes(outputPath);
                System.IO.File.Delete(outputPath);
                //Response.Headers.Append("Content-Disposition", "attachment; filename=" + fileName);
                return File(fileByte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

        }

        public IActionResult UploadTemplate() 
        { 
            throw new NotImplementedException(); 
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
            public string log_seq { get; set; }
            public string log_job { get; set; }
            public string log_table_src { get; set;}
            public string log_table_dst { get; set; }
            public string log_script { get; set; }

        }


        public class MasterData
        {
            public string log_kode { get; set; }
            public string log_nama { get; set; }
            public string log_waktu { get; set; }

        }

        //private static readonly Dictionary<string, string> mappingTemplate = new()
        //{
        //    { "Kode Job", "log_kode" },
        //    { "Nama Sequence", "log_nama" },
        //    { "Waktu Scheduler", "log_waktu" },
        //    { "Urutan Proses", "log_seq" },
        //    { "Nama Job", "log_job" },
        //    { "Nama Tabel Sumber", "log_table_src" },
        //    { "Nama Tabel Target", "log_table_dst" },
        //    { "Lokasi Script", "log_script" }
        //};

    }
}

