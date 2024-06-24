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
    [Area("BDA2")]
    public class ListMenuController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public ListMenuController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }


        public ActionResult Index(string id)
        {
            //db.CheckPermission("Log Aktivitas View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            //db.InsertAuditTrail("LogAktivitas_Akses_Page", "Akses Page Log Aktifitas");
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            TempData["tipe"] = id;

            if (id == "da")
            {
                db.InsertAuditTrail("List_Menu_DA_Akses_Page", "Akses Page List Menu DA", pageTitle);
            }
            else
            if (id == "osida")
            {
                db.InsertAuditTrail("List_Menu_Osida_Akses_Page", "Akses Page List Menu Osida", pageTitle);
            }

            
            return View();
        }

        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var tipe = TempData.Peek("tipe").ToString();

            var query = db.getListMenu(tipe).ToList();
            return DataSourceLoader.Load(query, loadOptions);
        }

     
    }
}