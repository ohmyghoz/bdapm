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
using System.Data.SqlClient;
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
            var query = (dynamic)null;
            using (SqlConnection conn = new SqlConnection(strSQL))
            {
                conn.Open();
                string strQuery = "Select * from MasterPenggunaanData where Sts_Aktif=0 order by id asc ";
                SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    query = from q in dt.AsEnumerable()
                                where q.Field<Int32>("Sts_Aktif") == 0
                                select new {
                                    id = q.Field<Int32>("id"),
                                    Penggunaan_Data = q.Field<string>("Penggunaan_Data") };
                }
                conn.Close();
                conn.Dispose();
            }
            return DataSourceLoader.Load(query, loadOptions);
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
                //var usr = db.UserMaster.Find(userId);
                ////var satkerId = HttpContext.User.FindFirst("satker_id");
                //var claims = new List<System.Security.Claims.Claim>
                //    {
                //        new System.Security.Claims.Claim(ClaimTypes.Name, userId),
                //        new System.Security.Claims.Claim(ClaimTypes.Role, role_id),
                //        new System.Security.Claims.Claim("FullName", usr.UserNama),
                //        new System.Security.Claims.Claim(ClaimTypes.Email , usr.UserEmail),
                //        new System.Security.Claims.Claim("CurrentRoleName", role_id),
                //    };

                ////claims.Add(new Claim("satker_id", satkerId.Value));
                //var claimsIdentity = new ClaimsIdentity(
                //                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //var authProperties = new AuthenticationProperties
                //{

                //};

                //HttpContext.SignInAsync(
                //    CookieAuthenticationDefaults.AuthenticationScheme,
                //    new ClaimsPrincipal(claimsIdentity),
                //    authProperties);

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
