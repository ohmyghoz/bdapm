using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class BIviewController : Controller
    {
        private readonly DataEntities _db;
        private readonly IWebHostEnvironment _env;

        public BIviewController(DataEntities db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            // OPTIONAL: Integrate with your existing menu structure (same pattern as other pages)
            try
            {
                var mdl = new BDA.Models.MenuDbModels(_db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(_db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();
                string pageTitle = currentNode != null ? currentNode.Title : "Tableau Links";

                // Permission (adjust to your permission scheme if needed)
                _db.CheckPermission("Market Driven View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = _db.CheckPermission("Market Driven Export", DataEntities.PermissionMessageType.NoMessage);

                _db.InsertAuditTrail("BIview_Akses_Page", "Akses Page BI View", pageTitle);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Menu/Audit error (non fatal): " + ex.Message);
            }

            return View();
        }

        [HttpGet]
        public object GetTableauLinks(DataSourceLoadOptions loadOptions)
        {
            try
            {
                // Adjust column names below to match actual table structure if different.
                // Expected table: BDAPM.pasarmodal.tableaulink
                // Sample columns (assumed): id (int), namadashboard (string), link (string)
                var sql = @"
SELECT 
    ROW_NUMBER() OVER (ORDER BY namadashboard) AS RowNo,
    ISNULL(namadashboard,'') AS NamaDashboardTableau,
    ISNULL(link,'') AS Link
FROM BDAPM.pasarmodal.tableaulink WITH (NOLOCK);";

                var dt = new DataTable();
                using (var conn = new SqlConnection(_db.appSettings.DataConnString))
                using (var cmd = new SqlCommand(sql, conn))
                using (var adp = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 120;
                    conn.Open();
                    adp.Fill(dt);
                }

                var list = dt.AsEnumerable()
                    .Select(r => new TableauLinkViewModel
                    {
                        RowNo = r.Field<long>("RowNo"),
                        NamaDashboardTableau = r.Field<string>("NamaDashboardTableau"),
                        Link = r.Field<string>("Link")
                    })
                    .ToList();

                return DataSourceLoader.Load(list, loadOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTableauLinks ERROR: " + ex.Message);
                return DataSourceLoader.Load(new List<TableauLinkViewModel>(), loadOptions);
            }
        }

        // Simple ViewModel (can be moved to a separate file if preferred)
        public class TableauLinkViewModel
        {
            public long RowNo { get; set; }
            public string NamaDashboardTableau { get; set; }
            public string Link { get; set; }
        }
    }
}