using DevExpress.XtraReports.UI;
using BDA.DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Controllers {
    [Area("FW")]
    public class DxDashboardController : Controller {


        private DataEntities db;
        private IWebHostEnvironment _env;
        public DxDashboardController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Designer() {
            ViewBag.IsReport = true;
            return View();            
        }

        public IActionResult Viewer(string RepKode) {
            ViewBag.IsReport = true;
            ViewBag.Url = RepKode;
            return View();
        }
    }
}