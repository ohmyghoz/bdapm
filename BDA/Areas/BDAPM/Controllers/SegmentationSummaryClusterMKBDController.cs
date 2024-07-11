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
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class SegmentationSummaryClusterMKBDController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public IActionResult Index()
        {
            return View();
        }
    }
}
