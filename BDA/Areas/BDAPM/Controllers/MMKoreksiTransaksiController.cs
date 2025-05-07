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

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class MMKoreksiTransaksiController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public MMKoreksiTransaksiController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Pola Koreksi Per Transaksi / Pola Transaksi View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Pola Koreksi Per Transaksi / Pola Transaksi Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Pola_Koreksi_Pola_Transaksi_Akses_Page", "Akses Page Pola Koreksi Per Transaksi / Pola Transaksi", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("Pola_Koreksi_Pola_Transaksi_Akses_Page", "user " + userId + " mengakases halaman Pola Koreksi Per Transaksi / Pola Transaksi", pageTitle);

            return View();
        }
        public IActionResult GetMemberTypes(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaAmandedTypeInfo>();

            string reportId = "mm_bond_trades_amended"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMAmandedTypeInfo(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               amended_info_type = bs.Field<string>("amended_info_type").ToString().Trim()
                           }).OrderBy(bs => bs.amended_info_type).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["amended_info_type"].ToString();
                    list.Add(new NamaAmandedTypeInfo() { value = dtList.Rows[i]["amended_info_type"].ToString(), text = namakode });
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        public class NamaAmandedTypeInfo
        {
            public string value { get; set; }
            public string text { get; set; }
        }
    }
}
