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

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MarketDrivenController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MarketDrivenController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
            public IActionResult Index()
        {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();
                string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

                db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
                ViewBag.Export = db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
                db.InsertAuditTrail("MarketDriven_Akses_Page", "Akses Page Market Driven", pageTitle); //simpan kedalam audit trail

                return View();
        
        }
    }
}
