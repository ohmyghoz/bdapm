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

namespace BDA.Helper {
    public class biDTO {
        public long bi_id { get; set; }
        public string bi_nama { get; set; }
        public DateTime bi_tgl { get; set; }
        public string bi_link { get; set; }

    }
}
namespace BDA.Areas.Master.Controllers
{
    [Area("Dashboard")]
    public class BIController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public BIController(DataEntities db, IWebHostEnvironment env)
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
            db.InsertAuditTrail("BI_EWS_SLIK_Akses_Page", "Akses Page BI EWS SLIK", pageTitle);
            return View();
        }

        #region "GetGridData"
        [HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            //var lisobj = new List<biDTO>();
            //lisobj.Add(new biDTO { bi_id=1,bi_nama="Size Bisnis",bi_tgl=DateTime.Now,bi_link= "http://10.225.60.50/#/workbooks/6/views" });
            //lisobj.Add(new biDTO { bi_id = 2, bi_nama = "Rentang Waktu Permintaan", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/10/views" });
            //lisobj.Add(new biDTO { bi_id = 3, bi_nama = "Rata-Rata Jumlah Permintaan", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/9/views" });
            //lisobj.Add(new biDTO { bi_id = 4, bi_nama = "Kantor Cabang", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/7/views" });
            //lisobj.Add(new biDTO { bi_id = 5, bi_nama = "User Permintaan", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/8/views" });
            //lisobj.Add(new biDTO { bi_id = 6, bi_nama = "Kombinasi", bi_tgl = DateTime.Now, bi_link = "http://10.225.60.50/#/workbooks/4/views" });


            //return DataSourceLoader.Load(lisobj, loadOptions);

            var query = db.TableauLink.Where(x => x.stsrc == "A" && x.type == "ews").Select(x => new { x.id, x.name, x.link, x.date_update }).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion
    }
}