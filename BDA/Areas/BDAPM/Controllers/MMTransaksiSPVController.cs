using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static BDA.Controllers.SampleGridController;
using System.Xml.Linq;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using Ionic.Zip;
using System.Web;
using System.Reflection;
using DevExpress.Xpo.DB;
using DevExpress.Charts.Native;
using static DevExpress.Data.ODataLinq.Helpers.ODataLinqHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MMTransaksiSPVController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MMTransaksiSPVController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            string monthpawal = null;
            string yearpawal = null;
            string monthpakhir = null;
            string yearpakhir = null;

            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            DateTime periodeAwal = DateTime.Now;
            monthpawal = Convert.ToDateTime(periodeAwal).ToString("MMM");
            yearpawal = Convert.ToDateTime(periodeAwal).ToString("yyyy");
            ViewData["monthyearawal"] = monthpawal + " " + yearpawal;
            ViewBag.monthyearawal = monthpawal + " " + yearpawal;
            TempData["monthyearawal"] = monthpawal + " " + yearpawal;

            DateTime periodeAkhir = DateTime.Now;
            monthpakhir = Convert.ToDateTime(periodeAkhir).ToString("MMM");
            yearpakhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy");
            ViewData["monthyearakhir"] = monthpakhir + " " + yearpakhir;
            ViewBag.monthyearakhir = monthpakhir + " " + yearpakhir;
            TempData["monthyearakhir"] = monthpakhir + " " + yearpakhir;

            db.CheckPermission("Pola 1 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Pola 1 Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Pola_1_Akses_Page", "Akses Page Pola 1", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("Pola_1_Akses_Page", "user " + userId + " mengakases halaman Pola 1", pageTitle);

            return View();
        }
    }
}
