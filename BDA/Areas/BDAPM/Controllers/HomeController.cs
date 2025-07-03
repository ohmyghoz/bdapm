using BDA.DataModel;
using BDA.Helper;
using DevExpress.Xpo.DB;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace BDA.Controllers
{
    [Area("BDAPM")] // Make sure the Area matches your project structure
    public class MarketDataController : Controller
    {
        private readonly DataEntities db;

        // Constructor to inject the database context
        public MarketDataController(DataEntities db)
        {
            this.db = db;
        }

        /// <summary>
        /// Displays the main view for the market data report.
        /// </summary>
        /// <returns>The Index view.</returns>
        public IActionResult Index()
        {
            // You can add permission checks and audit trails here if needed,
            // similar to the PembiayaanVSJaminanSahamController.
            ViewBag.Title = "Market Driven Validation Data";
            return View();
        }

        /// <summary>
        /// HTTP GET endpoint to fetch data for the DevExtreme DataGrid.
        /// </summary>
        /// <param name="loadOptions">DevExtreme data loading options.</param>
        /// <returns>Data in a format compatible with the DataGrid.</returns>
      
       
    }
}
