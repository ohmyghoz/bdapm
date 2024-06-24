using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Web.QueryBuilder;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.QueryBuilder.Services;
using BDA.DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;

namespace BDA.Controllers
{
    [Area("FW")]
    public class SampleController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public SampleController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {

//            db.SetSessionString("sctext", "Sukses");
            return View();

        }

        public IActionResult Queue()
        {

            //            db.SetSessionString("sctext", "Sukses");
            return View();

        }

    }
}