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
    public class DXReportController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public DXReportController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Designer()
        {


            return View();

        }

        public IActionResult Viewer(string RepKode)
        {

            //var rpt = XtraReport.FromFile("E:\\Project\\EDWOJK3\\EDWOJK3\\DXReportsStore\\Report2.repx");
            //rpt.Parameters[0].Value = "100";
            //rpt.Parameters[0].Visible = false;
            //rpt.RequestParameters = false;
            //ViewBag.Report = rpt;
            ViewBag.url = RepKode;
            return View();
        }

        public IActionResult QueryBuilder(
            [FromServices] IQueryBuilderClientSideModelGenerator queryBuilderClientSideModelGenerator)
        {
            var newDataConnectionName = "EDWDATA";
            var queryBuilderModel = queryBuilderClientSideModelGenerator.GetModel(newDataConnectionName);
            return View(queryBuilderModel);
        }

        [HttpPost]
        public IActionResult SaveQuery([FromServices] IQueryBuilderInputSerializer queryBuilderInputSerializer,[FromForm] DevExpress.DataAccess.Web.QueryBuilder.DataContracts.SaveQueryRequest saveQueryRequest)
        {
            try
            {
                var queryBuilderInput = queryBuilderInputSerializer.DeserializeSaveQueryRequest(saveQueryRequest);
                SelectQuery resultingQuery = queryBuilderInput.ResultQuery;
                string sql = queryBuilderInput.SelectStatement;
                var newsql = Regex.Replace(sql, "\\\"[^\\\"]*\\\"", "[$&]").Replace("[\"", "[").Replace("\"]", "]");

                var resp = newsql;
                return new JsonResult(resp);

            }
            catch 
            {
                return null;
            }
        }

    }
}