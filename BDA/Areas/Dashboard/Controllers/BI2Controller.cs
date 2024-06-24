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
    [Area("Dashboard")]
    public class BI2Controller : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public BI2Controller(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        // GET: Ref_Kota
        public ActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            //db.CheckPermission("Log Monitoring Data View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            //var user = HttpContext.User.Identity.Name;

            //var akses = (from a in db.UserMaster
            //             join b in db.FWUserRole on a.UserId equals b.UserId
            //             where a.Stsrc == "A" && a.UserId == user && b.RoleId == "AdminAplikasi"
            //             select new { b.RoleId }).ToList().Any();
            //ViewBag.Admin = akses;
            db.InsertAuditTrail("BI_BDA_Akses_Page", "Akses Page BI BDA", pageTitle);
            return View();
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            //var lisobj = new List<biDTO>();
            //lisobj.Add(new biDTO { bi_id=1,bi_nama="HML MCDFA",bi_tgl=DateTime.Now,bi_link= "http://10.225.60.50/#/workbooks/11" });
            //lisobj.Add(new biDTO { bi_id = 2, bi_nama = "Clustering Predictive Agunan", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/12/views" });
            //lisobj.Add(new biDTO { bi_id = 3, bi_nama = "Kesesuaian Max-Min Overdue per Collectability", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/13/views" });
            //lisobj.Add(new biDTO { bi_id = 4, bi_nama = "Classification Predictive Actual Collectability", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/15/views" });
            //lisobj.Add(new biDTO { bi_id = 5, bi_nama = "Collateral Value Difference", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/16/views" });
            //lisobj.Add(new biDTO { bi_id = 6, bi_nama = "Log Outliers Aktivitas", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/17/views" });
            //lisobj.Add(new biDTO { bi_id = 7, bi_nama = "Log Outliers Diagnostic", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/18/views" });
            //lisobj.Add(new biDTO { bi_id = 8, bi_nama = "Log PerHour", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/19/views" });
            //lisobj.Add(new biDTO { bi_id = 11, bi_nama = "Log Daily Activity", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/20/views" });

            //return DataSourceLoader.Load(lisobj, loadOptions);

            var query = db.TableauLink.Where(x => x.stsrc == "A" && x.type == "bda").Select(x => new { x.id, x.name, x.link, x.date_update }).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion
    }
}