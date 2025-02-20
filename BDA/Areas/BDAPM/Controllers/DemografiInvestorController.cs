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
using RazorEngineCore;
using System.Globalization;
using Humanizer;
using System.Xml.Linq;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class DemografiInvestorController
        : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public DemografiInvestorController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public bool IsPengawasPM()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("PengawasPM")) //cek jika role Pengawas PM
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu
            CultureInfo culture = new CultureInfo("id-ID");
            ViewBag.Hive = false;
            //ViewBag.totalValueTraded = getTotalValueTraded();
            
            db.CheckPermission("Demografi Investor View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            db.InsertAuditTrail("DemografiInvestor_Akses_Page", "Akses Page Demografi Investor", pageTitle); //simpan kedalam audit trail

            var userId = HttpContext.User.Identity.Name;
            db.InsertAuditTrail("DemografiInvestor_Akses_Page", "user " + userId + " mengakases halaman Demografi Investor untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);

            return View();
        }

        [HttpGet]
        public object getTotalValueTraded(string periode, string pe, string origin, string tipeInvestor)
        {
            DataSourceLoadOptions loadOptions = new DataSourceLoadOptions();
            if (string.IsNullOrEmpty(pe)) {
                pe = null;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryPS.GetBDAPMDemografiInvestorTV(db, loadOptions, periode, pe, origin, tipeInvestor).data.Rows[0];
            return float.Parse(result["total"].ToString());
        }

        [HttpGet]
        public object getChartGender(DataSourceLoadOptions loadOptions, string periode, string pe, string origin, string tipeInvestor)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;

            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMDemografiInvestorCG(db, loadOptions, stringPeriodeAwal, stringNamaPE, origin, tipeInvestor);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        [HttpGet]
        public object getChartUsia(DataSourceLoadOptions loadOptions, string periode, string pe, string origin, string tipeInvestor)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;

            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMDemografiInvestorCU(db, loadOptions, stringPeriodeAwal, stringNamaPE, origin, tipeInvestor);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        [HttpGet]
        public object getChartPendidikan(DataSourceLoadOptions loadOptions, string periode, string pe, string origin, string tipeInvestor)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;

            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMDemografiInvestorCP(db, loadOptions, stringPeriodeAwal, stringNamaPE, origin, tipeInvestor);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        [HttpGet]
        public object getChartPekerjaan(DataSourceLoadOptions loadOptions, string periode, string pe, string origin, string tipeInvestor)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;

            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMDemografiInvestorCPk(db, loadOptions, stringPeriodeAwal, stringNamaPE, origin, tipeInvestor);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        [HttpGet]
        public object getChartPenghasilan(DataSourceLoadOptions loadOptions, string periode, string pe, string origin, string tipeInvestor)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string stringPeriodeAwal = null;
            string stringNamaPE = null;

            if (periode != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periode).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (pe != null && string.IsNullOrEmpty(pe))
            {
                stringNamaPE = pe;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periode.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryPS.GetBDAPMDemografiInvestorCPh(db, loadOptions, stringPeriodeAwal, stringNamaPE, origin, tipeInvestor);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }
               
        
        [HttpPost]
        public ActionResult SimpanPenggunaanData(string id)
        {
            string message = "";
            string Penggunaan_Data = "";
            bool result = true;
            var userId = HttpContext.User.Identity.Name;

            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "";
            db.InsertAuditTrail("DemografiInvestor_Akses_Page", "user " + userId + " mengakases halaman Demografi Investor untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

            try
            {
                string strSQL = db.appSettings.DataConnString;
                using (SqlConnection conn = new SqlConnection(strSQL))
                {
                    conn.Open();
                    string strQuery = "Select * from MasterPenggunaanData where id=" + id + " order by id asc ";
                    SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                         Penggunaan_Data = dt.Rows[0]["Penggunaan_Data"].ToString();
                    }
                    conn.Close();
                    conn.Dispose();
                }
                result = true;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                message = "Saving Failed !, " + " " + errMsg;
                result = false;
            }
            return Json(new { message, success = result }, new Newtonsoft.Json.JsonSerializerSettings());
        }
        public class NamaPE
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        [HttpGet]
        public object GetNamaPE(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            string reportId = "dim_exchange_members"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMNamaPE(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               exchangemembercode = bs.Field<string>("exchangemembercode").ToString().Trim(),
                               exchangemembername = bs.Field<string>("exchangemembername").ToString().Trim(),
                           }).OrderBy(bs => bs.exchangemembername).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                list.Add(new NamaPE() { value = "", text = "(ALL)" });
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    list.Add(new NamaPE() { value = dtList.Rows[i]["exchangemembercode"].ToString(), text = namakode });
                }
            }
            return Json(DataSourceLoader.Load(list, loadOptions));
        }

        public object GetOrigin(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<SelectionList>();

            list.Add(new SelectionList { text = "Lokal" , value= "Lokal"});
            list.Add(new SelectionList { text = "Asing", value = "Asing" });
            return DataSourceLoader.Load(list, loadOptions);
        }

        public object GetTipeInvestor(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<SelectionList>();

            list.Add(new SelectionList { text = "Individu", value = "Individu" });
            list.Add(new SelectionList { text = "Institusi", value = "Institusi" });
            return DataSourceLoader.Load(list, loadOptions);
        }
        public class SelectionList
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        
        
    }
}
