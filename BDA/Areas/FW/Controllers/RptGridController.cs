using BDA.DataModel;
using BDA.Helper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BDA.Models;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Template;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;
using System.IO;

namespace BDA.Controllers
{
    [Area("FW")]
    public class RptGridController : Controller
    {
        private DataEntities db;
        private IHostingEnvironment _env;
        public RptGridController(DataEntities db, IHostingEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var user = HttpContext.User.Identity.Name;

            var akses = (from q in db.FWUserRole
                         where q.Stsrc == "A" && q.UserId == user
                         select q.RoleId).FirstOrDefault();
            ViewBag.Akses = akses;

            return View();
        }

        public IActionResult Create()
        {
            var obj = new RptGrid();

            string Kode = db.RptGrid.OrderByDescending(x => x.rg_id).First().rg_kode;
            int tempKode = int.Parse(new string(Kode.Where(char.IsDigit).ToArray()));
            obj.rg_kode = (tempKode + 1).ToString("00000");
            return View("Edit", obj);
        }

        private void RefreshRptGridParam(RptGrid rg)
        {
            var query = rg.rg_query;
            var lstParams = new List<string>();
            var paramMatches = Regex.Matches(query, "\\@([^=<>\\s\']+)");
            foreach (Match mtch in paramMatches)
            {
                var paramName = mtch.Value.ToLower();
                if (!lstParams.Contains(paramName))
                {
                    lstParams.Add(paramName);
                }
            }


            var curParams = rg.RptGrid_Param.Where(x => x.stsrc == "A").ToList();

            //hapus yang ga ada di query
            var deletedParams = (from q in curParams where !lstParams.Contains(q.rgpr_kode) select q);
            foreach (var prms in deletedParams)
            {
                db.DeleteStsrc(prms);
            }

            //insert yang ga ada di query
            var newParams = from q in lstParams where (!curParams.Select(x => x.rgpr_kode).ToList().Contains(q)) select q;
            foreach (var prm in newParams)
            {
                var newrgprm = new RptGrid_Param();
                newrgprm.rgpr_allow_null = true;
                newrgprm.rgpr_controltype = "TextBox";
                newrgprm.rgpr_datatype = "string";
                newrgprm.rgpr_kode = prm;
                newrgprm.rgpr_nama = prm;
                newrgprm.RptGrid = rg;
                db.SetStsrcFields(newrgprm);
                db.RptGrid_Param.Add(newrgprm);
            }

        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rg_id" })]
        public IActionResult Create([Bind("rg_id", "parent_id", "rg_nama", "rg_catatan", "rg_kode", "rg_db_name", "rg_query", "rg_entrier", "rg_rolesetting_inherited")] RptGrid obj)
        {
            try
            {
                obj.rg_tipe = "Report";
                obj.rg_entrier = User.Identity.Name;
                db.RptGrid.Add(obj);
                RefreshRptGridParam(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.SetSessionString("sctext", "Sukses menyimpan data");
                return RedirectToAction("IndexEdit", new { id = obj.rg_id });
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("Edit", obj);
        }

        public IActionResult Edit(long? id)
        {
            if (id == null) return BadRequest();

            var obj = db.RptGrid.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rg_id" })]
        public IActionResult Edit([Bind("rg_id", "parent_id", "rg_nama", "rg_catatan", "rg_kode", "rg_db_name", "rg_query", "rg_entrier", "rg_rolesetting_inherited")] RptGrid obj)
        {
            try
            {
                var oldObj = db.RptGrid.Find(obj.rg_id);

                WSMapper.CopyFieldValues(obj, oldObj, "parent_id,rg_nama,rg_catatan,rg_kode,rg_db_name,rg_query,rg_rolesetting_inherited");
                oldObj.rg_entrier = User.Identity.Name;
                RefreshRptGridParam(oldObj);
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.SetSessionString("sctext", "Sukses menyimpan data");
                return RedirectToAction("IndexEdit", new { id = obj.rg_id });
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
            var obj = db.RptGrid.First(o => o.rg_id == param1);
            db.DeleteStsrc(obj);
            db.SaveChanges();
            var resp = "Sukses menghapus data";
            return new JsonResult(resp);

        }

        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.RptGrid
                        where q.stsrc == "A"
                        select new { q.rg_rolesetting_inherited, q.rg_entrier, q.rg_query, q.rg_db_name, q.rg_tipe, q.rg_kode, q.rg_catatan, q.rg_nama, q.parent_id, q.rg_id };
            return DataSourceLoader.Load(query, loadOptions);
        }

        public IActionResult IndexEdit(long id)
        {
            var obj = db.RptGrid.Find(id);
            return View(obj);
        }

        public IActionResult CreateParam(long rg_id)
        {
            var obj = new RptGrid_Param();
            obj.rg_id = rg_id;
            return View("EditParam", obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rgpr_id" })]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rg_id" })]
        public IActionResult CreateParam([Bind("rgpr_id", "rg_id", "rgpr_kode", "rgpr_nama", "rgpr_catatan", "rgpr_datatype", "rgpr_controltype", "rgpr_allow_null", "rgpr_null_text", "rgpr_value_default", "rgpr_value_csv", "rgpr_value_query")] RptGrid_Param obj)
        {
            try
            {
                db.RptGrid_Param.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                ViewBag.ClosePopup = "Sukses menyimpan data";
                return View("EditParam", obj);
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("EditParam", obj);
        }

        public IActionResult EditParam(long? id)
        {
            if (id == null) return BadRequest();

            var obj = db.RptGrid_Param.Find(id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rgpr_id" })]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rg_id" })]
        public IActionResult EditParam([Bind("rgpr_id", "rg_id", "rgpr_kode", "rgpr_nama", "rgpr_catatan", "rgpr_datatype", "rgpr_controltype", "rgpr_allow_null", "rgpr_null_text", "rgpr_value_default", "rgpr_value_csv", "rgpr_value_query")] RptGrid_Param obj)
        {
            try
            {
                var oldObj = db.RptGrid_Param.Find(obj.rgpr_id);
                WSMapper.CopyFieldValues(obj, oldObj, "rg_id,rgpr_kode,rgpr_nama,rgpr_catatan,rgpr_datatype,rgpr_controltype,rgpr_allow_null,rgpr_null_text,rgpr_value_default,rgpr_value_csv,rgpr_value_query");
                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                ViewBag.ClosePopup = "Sukses menyimpan data";
                return View(oldObj);
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult DeleteParam(long? param1)
        {
            var obj = db.RptGrid_Param.First(o => o.rgpr_id == param1);
            db.DeleteStsrc(obj);
            db.SaveChanges();
            var resp = "Sukses menghapus data";
            return new JsonResult(resp);

        }
        [HttpGet]
        public object GetGridParamData(DataSourceLoadOptions loadOptions, long rg_id)
        {
            var query = from q in db.RptGrid_Param
                        where q.stsrc == "A" && q.rg_id == rg_id
                        select new { q.rgpr_value_query, q.rgpr_value_csv, q.rgpr_value_default, q.rgpr_null_text, q.rgpr_allow_null, q.rgpr_controltype, q.rgpr_datatype, q.rgpr_catatan, q.rgpr_nama, q.rgpr_kode, q.rg_id, q.rgpr_id };
            return DataSourceLoader.Load(query, loadOptions);
        }

        #region "RefGetter"
        [HttpGet]
        public object GetRefparent_id(DataSourceLoadOptions loadOptions)
        {
            var query = db.RptGrid
                .Where(x => x.stsrc == "A")
                .OrderBy(x => x.rg_id)
                .Select(x => new { x.rg_id, x.rg_nama });
            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpGet]
        public object GetRefRptGrid(DataSourceLoadOptions loadOptions)
        {
            var query = db.RptGrid
                .Where(x => x.stsrc == "A")
                .OrderBy(x => x.rg_id)
                .Select(x => new { x.rg_id, rg_nama = x.rg_kode + " - " + x.rg_nama });
            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion


        [HttpGet]
        public IActionResult Queue()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var user = HttpContext.User.Identity.Name;

            var akses = (from q in db.FWUserRole
                         where q.Stsrc == "A" && q.UserId == user
                         select q.RoleId).FirstOrDefault();
            ViewBag.Akses = akses;

            db.InsertAuditTrail("AntrianReport_Akses_Page", "Akses Page Antrian Report", pageTitle);

            return View();
        }

        [HttpGet]
        public IActionResult PreviewQueue()
        {
            var user = HttpContext.User.Identity.Name;

            var akses = (from q in db.FWUserRole
                         where q.Stsrc == "A" && q.UserId == user
                         select q.RoleId).FirstOrDefault();
            ViewBag.Akses = akses;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteQueue(long id)
        {
            var obj = db.RptGrid_Queue.First(o => o.rgq_id == id);
            if (obj.rgq_status == "Pending") {
                db.DeleteStsrc(obj);
            } 
            db.SaveChanges();
            var resp = "Sukses menghapus antrian";
            return new JsonResult(resp);
        }

        [HttpGet]
        public IActionResult Download(long id)
        {

            var obj = db.RptGrid_Queue.First(o => o.rgq_id == id);
            if (obj.rgq_requestor.ToLower() != User.Identity.Name.ToLower())
            {
                return Json(new { success = false, message = "Yang dapat mendownload file hanya user requestor [" + obj.rgq_requestor + "]" });
            }

            if (obj.rgq_status == "Selesai" && obj.rgq_result_filename != null)
            {
                var finfo = new FileInfo(obj.rgq_result_filename);
                Stream outputStream = new FileStream(obj.rgq_result_filename, FileMode.Open);
                string fileName = finfo.Name;
                if (outputStream != null)
                {
                    outputStream.Seek(0, 0);
                    return File(outputStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }

                return Json(new { success = true });

            }
            else
            {
                return Json(new { success = false, message = "Tidak menemukan file untuk didownload" });
            }
            //return Json(new { error = "Tidak menemukan file untuk didownload" });
        }


        [HttpGet]
        public IActionResult QueueAdd(long? id)
        {
            var obj = new RptGrid();
            var paramList = new List<RptGrid_Param>();
            if (id != null)
            {
                obj = db.RptGrid.Find(id);
                paramList = obj.RptGrid_Param.Where(x => x.stsrc == "A").ToList();
            }
            ViewBag.paramList = paramList;
            return View(obj);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rg_id" })]
        public IActionResult QueueAdd(long rg_id, string btnAction)
        {

            var obj = db.RptGrid.Find(rg_id);
            var paramList = obj.RptGrid_Param.Where(x => x.stsrc == "A").ToList();
            var paramValueList = new Dictionary<string, string>();

            foreach (var param in paramList)
            {
                if (Request.Form.ContainsKey(param.rgpr_kode))
                {
                    paramValueList.Add(param.rgpr_kode, Request.Form[param.rgpr_kode].ToString());
                }
                else
                {
                    paramValueList.Add(param.rgpr_kode, null);
                }
            }

            var paramString = Newtonsoft.Json.JsonConvert.SerializeObject(paramValueList);
            var newq = new RptGrid_Queue();
            newq.rgq_date = DateTime.Now;
            newq.rgq_nama = obj.rg_nama;
            newq.rgq_params = paramString;
            newq.rgq_priority = 1;
            newq.rgq_query = obj.rg_query;
            newq.rgq_requestor = User.Identity.Name;


            if (btnAction == "Antrikan Laporan")
            {
                newq.rgq_status = "Pending";
            }
            else
            {
                newq.rgq_status = "Preview";
            }

            newq.rgq_urut = 0;
            newq.RptGrid = obj;
            db.SetStsrcFields(newq);
            db.RptGrid_Queue.Add(newq);

            db.SaveChanges();

            if (btnAction == "Antrikan Laporan")
            {
                ViewBag.paramList = paramList;
                db.SetSessionString("sctext", "Sukses Mengantrikan Laporan");
                return View(obj);
            }
            else
            {
                return RedirectToAction("Preview", new { id = newq.rgq_id });
            }
        }


        [HttpPost]
        [TypeFilter(typeof(ValidateSecureHiddenInputsAttribute), Arguments = new object[] { "rgq_id" })]
        public IActionResult Queue([Bind("rg_id", "rgq_params", "rgq_priority", "rgq_urut")] RptGrid_Queue obj)
        {
            try
            {
                var rg = db.RptGrid.Find(obj.rg_id);
                obj.rgq_nama = rg.rg_nama;
                obj.rgq_query = rg.rg_query;
                obj.rgq_requestor = User.Identity.Name;
                obj.rgq_status = "Pending";
                obj.rgq_date = DateTime.Now;
                db.RptGrid_Queue.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.SetSessionString("sctext", "Sukses menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }

            return View(obj);
        }


        [HttpGet]
        public object GetGridQueueData(DataSourceLoadOptions loadOptions)
        {
            var user = HttpContext.User.Identity.Name;

            var role = (from q in db.FWUserRole
                         join p in db.FWRefRole
                         on q.RoleId equals p.RoleId
                         where q.Stsrc == "A" && p.Stsrc == "A" && q.UserId == user
                         select p.RoleName).FirstOrDefault();


            if (role == "Non Pengawas")
            {
                var querynp = (from p in db.getListQueue()
                         join q in db.FWUserRole on p.rgq_requestor equals q.UserId
                         join r in db.FWRefRole on q.RoleId equals r.RoleId
                         where p.stsrc == "A" && q.Stsrc == "A" && r.Stsrc == "A" && r.RoleName == "Non Pengawas"
                         select p).ToList();

                //var querynp = db.getListQueueNonPengawas().ToList();
                return DataSourceLoader.Load(querynp, loadOptions);
            }
            else
            {
                var query = db.getListQueue().ToList();
                return DataSourceLoader.Load(query, loadOptions);
            }
        }

        [HttpGet]
        public object GetGridPreviewQueueData(DataSourceLoadOptions loadOptions)
        {
            var query2 = new List<RptGrid_Queue>();
            var query1 = (from q in db.RptGrid_Queue
                          where q.stsrc == "A" && q.rgq_status == "Preview"
                          select new { q.rgq_result_rowcount, q.rgq_result_filename, q.rgq_result_filesize, q.rgq_error_message, q.rgq_urut, q.rgq_priority, q.rgq_status, q.rgq_end, q.rgq_start, q.rgq_date, q.rgq_requestor, q.rgq_nama, q.rgq_params, q.rgq_query, q.rg_id, q.rgq_id }).ToList();

            //foreach(var q in query1)
            //{
            //    query2.Add(new RptGrid_Queue { 
            //        rgq_result_rowcount = q.rgq_result_rowcount,
            //        rgq_result_filename = q.rgq_result_filename,
            //        rgq_result_filesize = q.rgq_result_filesize,
            //        rgq_error_message = q.rgq_error_message,
            //        rgq_urut = q.rgq_urut,
            //        rgq_priority = q.rgq_priority,
            //        rgq_status= q.rgq_status,
            //        rgq_end= q.rgq_end,
            //        rgq_start= q.rgq_start, 
            //        rgq_date = q.rgq_date,
            //        rgq_requestor= q.rgq_requestor,
            //        rgq_nama= q.rgq_nama,
            //        rgq_params= q.rgq_params.Replace("{", "").Replace("}", "").Replace("\"", " ").Replace(",", "<br/>"),
            //        rgq_query= q.rgq_query,
            //        rg_id = q.rg_id,
            //        rgq_id= q.rgq_id });

            //    //q.rgq_params;
            //}
            return DataSourceLoader.Load(query1, loadOptions);
        }

        [HttpGet]
        public IActionResult Preview(long? id)
        {
            var obj = db.RptGrid_Queue.Find(id);
            if (obj.rgq_status == "Preview")
            {
                ViewBag.NoPreview = false;
            }
            else
            {
                ViewBag.NoPreview = true;
            }
            return View(obj);
        }

        //[HttpGet]
        //public object GetGridPreviewData(DataSourceLoadOptions loadOptions, long id)
        //{
        //    try
        //    {
        //        var obj = db.RptGrid_Queue.Find(id);
        //        var paramValueList = JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.rgq_params);


        //        if (obj.rgq_status == "Preview")
        //        {
        //            var dbConn = db.DataConnection.Find(obj.RptGrid.rg_db_name);

        //            DataSet retVal = new DataSet();
        //            using (var sqlConn = new SqlConnection(dbConn.DecryptedConnString(db)))
        //            {


        //                //SqlConnection sqlConn = (SqlConnection)db.Database.Connection;
        //                var query = "SELECT TOP 100 * from (" + Environment.NewLine + obj.rgq_query + ") top100";
        //                SqlCommand cmdReport = new SqlCommand(query, sqlConn);
        //                cmdReport.CommandTimeout = 60; //1 menit dulu
        //                SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
        //                using (cmdReport)
        //                {
        //                    foreach (var prms in obj.RptGrid.RptGrid_Param.Where(x => x.stsrc == "A"))
        //                    {
        //                        cmdReport.Parameters.AddWithValue(prms.rgpr_kode, paramValueList[prms.rgpr_kode]);
        //                    }
        //                    cmdReport.CommandType = CommandType.Text;
        //                    daReport.Fill(retVal);
        //                }
        //                var dt = retVal.Tables[0];
        //                string JSONresult;
        //                JSONresult = JsonConvert.SerializeObject(dt);
        //                return JSONresult;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException(ex.Message);
        //    }

        //}
    }


}