using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Areas.Master.Controllers
{
    [Area("Master")]
    public class AuditTrailController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public AuditTrailController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Audit Trail View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            db.InsertAuditTrail("LogAkses_Akses_Page", "Akses Page", pageTitle);
            return View();
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string periodeAwal, string periodeAkhir)
        {
            var query = db.AuditTrail.ToList();

            if (periodeAwal != null)
            {
                DateTime PeriodeAwal = Convert.ToDateTime(periodeAwal);
                query = query.Where(x => x.AuditDate >= PeriodeAwal).ToList();
            }

            if (periodeAkhir != null)
            {
                DateTime PeriodeAkhir = Convert.ToDateTime(periodeAkhir).AddHours(23).AddMinutes(59).AddSeconds(59);
                query = query.Where(x => x.AuditDate <= PeriodeAkhir).ToList();
            }

            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion
    }
}
