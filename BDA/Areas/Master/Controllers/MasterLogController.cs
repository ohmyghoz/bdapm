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
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;

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
                        log_proc_id = result.data.Rows[i]["ProcID"].ToString(),
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

        public IActionResult LogExportTemplate(string msg)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master Log Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Export Master Log", msg, pageTitle);
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
            string templateFile = "Template_MasterLog_BDAPMv2.xlsx";
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
                            log_proc_id = resultDetail.data.Rows[i]["ProcID"].ToString(),
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
                LogExportTemplate("Export Template Master Log");
                return File(fileByte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

        }

        public IActionResult UploadTemplate(IFormFile file) 
        { 
            List<MasterData> data = new List<MasterData>();
            List<LogMasterDataDetail> dataDetail = new List<LogMasterDataDetail>();
            bool isSuccess = false;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);

                using (var workbook = new XLWorkbook(ms))
                {
                    var sheetData = workbook.Worksheet("Sheet1");
                    var sheetDetail = workbook.Worksheet("Sheet2");
                    var rowsData = sheetData.RowsUsed().Skip(1);
                    var rowsDetail = sheetDetail.RowsUsed().Skip(1);

                    foreach (var row in rowsData)
                    {
                        MasterData d = new MasterData
                        {
                            log_kode = row.Cell(1).GetText(),
                            log_nama = row.Cell(2).GetText(), 
                            log_waktu = row.Cell(3).GetText()
                        };

                        data.Add(d);
                    }

                    foreach (var row in rowsDetail)
                    {
                        LogMasterDataDetail d = new LogMasterDataDetail
                        {
                            log_kode = row.Cell(1).GetText(),
                            log_seq = row.Cell(2).GetText(),
                            log_proc_id = row.Cell(3).GetText(),
                            log_job = row.Cell(4).GetText(),
                            log_table_src = row.Cell(5).GetText(),
                            log_table_dst = row.Cell(6).GetText(),
                            log_script = row.Cell(7).GetText()
                        };

                        dataDetail.Add(d);
                    }
                }

                DataTable dtMasterJob = new DataTable();
                dtMasterJob.Columns.Add("job_id", typeof(string));
                dtMasterJob.Columns.Add("job_name", typeof(string));
                dtMasterJob.Columns.Add("scheduler", typeof(string));

                foreach (var item in data)
                {
                    var row = dtMasterJob.NewRow();

                    row["job_id"] = item.log_kode;
                    row["job_name"] = item.log_nama;
                    row["scheduler"] = item.log_waktu;

                    dtMasterJob.Rows.Add(row);
                }

                DataTable dtJobProc = new DataTable();
                dtJobProc.Columns.Add("proc_id", typeof(string));
                dtJobProc.Columns.Add("proc_name", typeof(string));
                dtJobProc.Columns.Add("job_id", typeof(string));
                dtJobProc.Columns.Add("seq_no", typeof(string));
                dtJobProc.Columns.Add("script_location", typeof(string));


                var jobProc = dataDetail.GroupBy(dd => new { dd.log_kode, dd.log_seq, dd.log_job, dd.log_proc_id, dd.log_script })
                              .Select(g => new 
                              {
                                  proc_id = g.Key.log_proc_id,
                                  proc_name = g.Key.log_job,
                                  job_id = g.Key.log_kode,
                                  seq_no = g.Key.log_seq,
                                  script_location = g.Key.log_script
                              }).ToList();

                foreach (var item in jobProc)
                {
                    var row = dtJobProc.NewRow();

                    row["proc_id"] = item.proc_id;
                    row["proc_name"] = item.proc_name;
                    row["job_id"] = item.job_id;
                    row["seq_no"] = item.seq_no;
                    row["script_location"] = item.script_location;

                    dtJobProc.Rows.Add(row);
                }

                DataTable dtIoProc = new DataTable();
                dtIoProc.Columns.Add("proc_id", typeof(string));
                dtIoProc.Columns.Add("data_id", typeof(string));
                dtIoProc.Columns.Add("io_status", typeof(string));

                var ioProc = dataDetail.Select(x => new
                             { 
                                proc_id = x.log_proc_id,
                                data_id = x.log_table_src.IsNullOrEmpty() ? x.log_table_dst : x.log_table_src,
                                io_status = x.log_table_src.IsNullOrEmpty() ? "output" : "input",
                             }).ToList();

                foreach (var item in ioProc) 
                {
                    var row = dtIoProc.NewRow();

                    row["proc_id"] = item.proc_id;
                    row["data_id"] = item.data_id;
                    row["io_status"] = item.io_status;

                    dtIoProc.Rows.Add(row);
                }

                var conn = (SqlConnection)db.Database.Connection;

                int dmj = uploadBulk(conn, "dim_master_job", dtMasterJob);
                int djp = uploadBulk(conn, "dim_job_proc", dtJobProc);
                int dip = uploadBulk(conn, "dim_io_proc", dtIoProc);

                isSuccess = (dmj == 200) && (djp == 200) && (dip == 200);
            }

            return isSuccess ? Ok() : StatusCode(500);
        }

        public int uploadBulk(SqlConnection conn, string dstTable, DataTable datas) 
        {
            int value = 500;
            bool mustClose = false;

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE " + dstTable);

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                mustClose = true;
            }

            try
            {
                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, null))
                {
                    bulkCopy.DestinationTableName = "[" + dstTable + "]";
                    bulkCopy.EnableStreaming = true;

                    bulkCopy.WriteToServer(datas);
                }

                value = 200; 
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Bulk Insert: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                value = 500; 
            }
            finally
            {
                if (mustClose)
                    conn.Close();
            }

            return value;
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
            public string log_proc_id { get; set; }
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

    }
}

