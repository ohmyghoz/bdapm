using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BDA.Models;
using BDA.DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Components.Web;


namespace BDA.Areas.Website.Controllers
{
    [AllowAnonymous]
    [Area("Website")]
    public class PenggunaanDataController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public PenggunaanDataController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public object GetPenggunaanData(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<PenggunaData>();

            using (SqlConnection conn = new SqlConnection(strSQL))
            {
                conn.Open();
                string strQuery = "Select * from MasterPenggunaanData where Sts_Aktif=0 order by id asc ";
                SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        list.Add(new PenggunaData() { value = dt.Rows[i]["id"].ToString(), text = dt.Rows[i]["Penggunaan_Data"].ToString() });
                    }

                    return Json(DataSourceLoader.Load(list, loadOptions));
                }
                conn.Close();
                conn.Dispose();
            }
            return DataSourceLoader.Load(list, loadOptions);
        }
        public class PenggunaData
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public IActionResult PenggunaanData()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role) != null)
            {
                ViewBag.roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            }
            else
            {
                ViewBag.ClosePopup = "Session already finished";
            }
            return View("PenggunaanData");
        }
        [HttpPost]
        public ActionResult PenggunaanData(string id)
        {
            try
            {
                var userId = HttpContext.User.Identity.Name;
                string strSQL = db.appSettings.DataConnString;
                var query = (dynamic)null;
                using (SqlConnection conn = new SqlConnection(strSQL))
                {
                    conn.Open();
                    string strQuery = "Select * from MasterPenggunaanData where id=" + id + " order by id asc ";
                    SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        string Penggunaan_Data = dt.Rows[0]["Penggunaan_Data"].ToString();
                    }
                    conn.Close();
                    conn.Dispose();
                }

                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Akses Page Segmentation Summary Cluster MKBD", pageTitle);

                ViewBag.ClosePopup = "Success change role";
                return View();
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
                ViewBag.roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                return View();
            }
        }
    }
}
