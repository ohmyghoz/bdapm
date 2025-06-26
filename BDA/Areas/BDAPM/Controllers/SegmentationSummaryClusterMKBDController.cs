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
using Microsoft.AspNetCore.Components.Web;


namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class SegmentationSummaryClusterMKBDController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public SegmentationSummaryClusterMKBDController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public bool IsPengawasPM()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("Pengawas PM")) //cek jika role Pengawas PM
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
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Summary Cluster MKBD View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Akses Page Segmentation Summary Cluster MKBD", pageTitle); //simpan kedalam audit trail
            db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " mengakases halaman Segmentation Summary Cluster MKBD untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);

            return View();
        }
        public object GetGridData(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string status)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusPE = JsonConvert.DeserializeObject<string[]>(status);

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringStatus = null;
            string reportId = "pe_segmentation_sum_cluster_mkbd"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (StatusPE.Length > 0)
            {
                stringStatus = string.Join(", ", StatusPE);
                TempData["sts"] = stringStatus;
            }

            db.Database.CommandTimeout = 1200;
            if (periodeAwal.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQuery(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, stringStatus, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Segmentation Summary Cluster MKBD dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + ", status = " + stringStatus + " ", pageTitle);
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }
        public object GetChartClusterSearch(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string status)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusPE = JsonConvert.DeserializeObject<string[]>(status);

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringStatus = null;
            string reportId = "pe_segmentation_sum_cluster_mkbd"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (StatusPE.Length > 0)
            {
                stringStatus = string.Join(", ", StatusPE);
                TempData["sts"] = stringStatus;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryGetChartClusterSearch(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, stringStatus, cekHive);

            int sum = 0;
            foreach (DataRow dr in result.data.AsEnumerable())
            {
                dynamic value = dr["total"].ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    sum += Convert.ToInt32(value);
                }
            }
            ViewBag.TotalPie = sum;
            return JsonConvert.SerializeObject(result);
        }
        public object GetChartClusterBarSearch(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string status)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] StatusPE = JsonConvert.DeserializeObject<string[]>(status);

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringStatus = null;
            string reportId = "pe_segmentation_sum_cluster_mkbd"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                //string result = stringNamaPE.Replace("\",\"", "");
                TempData["pe"] = stringNamaPE;
            }

            if (StatusPE.Length > 0)
            {
                stringStatus = string.Join(", ", StatusPE);
                TempData["sts"] = stringStatus;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryGetChartClusterBarSearch(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, stringStatus, cekHive);
            var varDataList = (dynamic)null;

            if (cekHive == true)
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   cluster = bs.Field<string>("cluster").ToString(),
                                   total = Convert.ToInt64(bs.Field<Int64>("total").ToString()),
                                   urut = Convert.ToInt64(bs.Field<Int64>("urut").ToString()),
                               }).OrderBy(bs => bs.urut).ToList();
            }
            else
            {
                varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                               select new
                               {
                                   cluster = bs.Field<string>("cluster").ToString(),
                                   total = Convert.ToInt32(bs.Field<Int32>("total").ToString()),
                                   urut = Convert.ToInt64(bs.Field<Int64>("urut").ToString()),
                               }).OrderBy(bs => bs.urut).ToList();
            }

            return JsonConvert.SerializeObject(varDataList);
        }
        public object GetGridDataDetail(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_bridging_detail"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (stringPeriodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryDetail(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Detail Cluster MKBD dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryDetail(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Detail Cluster MKBD dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataDetailRincian(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_bridging_detail"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryDetailRincian(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryDetailRincian(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataRincianPortofolio(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_portofolio_saham"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryRincianPortofolio(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Rincian Portofolio dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryRincianPortofolio(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Rincian Portofolio dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataRincianPortofolioDetailSummary(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_portofolio_saham_sum"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryRincianPortofolioDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryRincianPortofolioDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataReksadana(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_reksa_dana"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReksadana(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Reksadana dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReksadana(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Reksadana dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataReksadanaDetailSummary(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_reksa_dana_sum"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReksadanaDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReksadanaDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataJaminanMargin(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_jaminan_margin"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryJaminanMargin(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Jaminan Margin dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryJaminanMargin(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Jaminan Margin dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataJaminanMarginDetailSummary(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_jaminan_margin_sum"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryJaminanMarginDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryJaminanMarginDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataReverseRepo(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var userId = HttpContext.User.Identity.Name;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_reverse_repo"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReverseRepo(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Reverse Repo dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReverseRepo(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " menampilkan dashboard Reverse Repo dengan filter tanggal = " + stringPeriodeAwal + ", nama PE = " + stringNamaPE + "", pageTitle);
                return JsonConvert.SerializeObject(result);
            }
        }
        public object GetGridDataReverseRepoDetailSummary(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string reportId = "pe_segmentation_det_reverse_repo_sum"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal != null) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReverseRepoDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQueryReverseRepoDetailSummary(db, loadOptions, reportId, stringPeriodeAwal, stringNamaPE, cekHive);
                return JsonConvert.SerializeObject(result);
            }
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
            db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "user " + userId + " mengakases halaman Segmentation Summary Cluster MKBD untuk digunakan sebagai " + Penggunaan_Data + "", pageTitle);

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
        public object GetNamaPEOnly(DataSourceLoadOptions loadOptions, string pName)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            string reportId = "dim_exchange_members"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMNamaPE(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           where bs.Field<string>("exchangemembercode") == pName
                           select new
                           {
                               exchangemembercode = bs.Field<string>("exchangemembercode").ToString().Trim(),
                               exchangemembername = bs.Field<string>("exchangemembername").ToString().Trim(),
                           }).OrderBy(bs => bs.exchangemembername).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);
            pName = dtList.Rows[0]["exchangemembercode"].ToString() + " - " + dtList.Rows[0]["exchangemembername"].ToString();
            return pName;
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
        [HttpGet]
        public object GetNamaSID(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            string reportId = "src_sid"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql
            var result = Helper.WSQueryStore.GetBDAPMSID(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;
            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               nama_sid = bs.Field<string>("nama_sid").ToString(),
                               sid = bs.Field<string>("sid").ToString(),
                           }).OrderBy(bs => bs.nama_sid).ToList();
            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string nama = dtList.Rows[i]["nama_sid"].ToString();
                    list.Add(new NamaPE() { value = dtList.Rows[i]["sid"].ToString(), text = nama });
                }
            }
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        [HttpGet]
        public object GetNamaPE_Old(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            using (SqlConnection conn = new SqlConnection(strSQL))
            {
                conn.Open();
                string strQuery = "Select exchangemembercode,exchangemembername from pasarmodal.dim_exchange_members where currentstatus='A' order by exchangemembername asc ";
                SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string namakode = dt.Rows[i]["exchangemembercode"].ToString() + " - " + dt.Rows[i]["exchangemembername"].ToString();
                        list.Add(new NamaPE() { value = dt.Rows[i]["exchangemembercode"].ToString(), text = namakode });
                    }

                    return Json(DataSourceLoader.Load(list, loadOptions));
                }
                conn.Close();
                conn.Dispose();
            }
            return DataSourceLoader.Load(list, loadOptions);
        }
        public class NamaPE
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        //-----------------------------detail-----------------------------------//
        public IActionResult Detail(DataSourceLoadOptions loadOptions, string id, string periodeAwal)
        {
            var userId = HttpContext.User.Identity.Name;
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Detail Cluster MKBD"; //menampilkan data menu

            string namaPE = id;
            string stringPeriodeAwal = null;
            string stringPeriodeAwalDate = null;
            string stringNamaPE = null;
            string strNamaPEOnly = null;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
                stringPeriodeAwalDate = Convert.ToDateTime(periodeAwal).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
                ViewBag.PeriodeAwalDateParam = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwalDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
                strNamaPEOnly = (string)GetNamaPEOnly(loadOptions, stringNamaPE);
                ViewBag.KodePE = namaPE;
                ViewBag.NamaPE = strNamaPEOnly;
            }
            else
            {
                ViewBag.NamaPE = "";
            }

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

            List<SelectListItem> entityTypelist = new List<SelectListItem>();
            if (dtList.Rows.Count > 0)
            {
                entityTypelist.Add(new SelectListItem() { Value = "", Text = "(ALL)" });
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    entityTypelist.Add(new SelectListItem() { Value = dtList.Rows[i]["exchangemembercode"].ToString(), Text = namakode });
                }
            }
            ViewBag.Jenis = entityTypelist;

            db.Database.CommandTimeout = 420;
            db.CheckPermission("Detail Cluster MKBD View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Detail Cluster MKBD Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("AksesPageDetailCluster_Akses_Page", "Akses Page Detail Cluster MKBD", pageTitle);
            db.InsertAuditTrail("AksesPageDetailCluster_Akses_Page", "user " + userId + " mengakases halaman Detail Cluster MKBD untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);

            return View();
        }

        //-----------------------------detail-----------------------------------//

        //-----------------------------Rincian Portofolio-----------------------------------//
        public IActionResult RincianPortofolio(DataSourceLoadOptions loadOptions, string id, string periodeAwal)
        {
            var userId = HttpContext.User.Identity.Name;
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Rincian Portofolio"; //menampilkan data menu

            string namaPE = id;
            string stringPeriodeAwal = null;
            string stringPeriodeAwalDate = null;
            string stringNamaPE = null;
            string strNamaPEOnly = null;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
                stringPeriodeAwalDate = Convert.ToDateTime(periodeAwal).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
                ViewBag.PeriodeAwalDateParam = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwalDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
                strNamaPEOnly = (string)GetNamaPEOnly(loadOptions, stringNamaPE);
                ViewBag.KodePE = namaPE;
                ViewBag.NamaPE = strNamaPEOnly;
            }
            else
            {
                ViewBag.NamaPE = "";
            }

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

            List<SelectListItem> entityTypelist = new List<SelectListItem>();
            if (dtList.Rows.Count > 0)
            {
                entityTypelist.Add(new SelectListItem() { Value = "", Text = "(ALL)" });
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    entityTypelist.Add(new SelectListItem() { Value = dtList.Rows[i]["exchangemembercode"].ToString(), Text = namakode });
                }
            }
            ViewBag.Jenis = entityTypelist;

            db.Database.CommandTimeout = 420;
            db.CheckPermission("Rincian Portofolio View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Rincian Portofolio Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("RincianPortofolio_Akses_Page", "Akses Page Rincian Portofolio", pageTitle);
            db.InsertAuditTrail("AksesPageDetailCluster_Akses_Page", "user " + userId + " mengakases halaman Rincian Portofolio untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);
            return View();
        }
        //-----------------------------Rincian Portofolio-----------------------------------//

        //-----------------------------Reksadana-----------------------------------//
        public IActionResult Reksadana(DataSourceLoadOptions loadOptions, string id, string periodeAwal)
        {
            var userId = HttpContext.User.Identity.Name;
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Reksadana"; //menampilkan data menu

            string namaPE = id;
            string stringPeriodeAwal = null;
            string stringPeriodeAwalDate = null;
            string stringNamaPE = null;
            string strNamaPEOnly = null;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
                stringPeriodeAwalDate = Convert.ToDateTime(periodeAwal).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
                ViewBag.PeriodeAwalDateParam = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwalDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
                strNamaPEOnly = (string)GetNamaPEOnly(loadOptions, stringNamaPE);
                ViewBag.KodePE = namaPE;
                ViewBag.NamaPE = strNamaPEOnly;
            }
            else
            {
                ViewBag.NamaPE = "";
            }

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

            List<SelectListItem> entityTypelist = new List<SelectListItem>();
            if (dtList.Rows.Count > 0)
            {
                entityTypelist.Add(new SelectListItem() { Value = "", Text = "(ALL)" });
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    entityTypelist.Add(new SelectListItem() { Value = dtList.Rows[i]["exchangemembercode"].ToString(), Text = namakode });
                }
            }
            ViewBag.Jenis = entityTypelist;

            db.Database.CommandTimeout = 420;
            db.CheckPermission("Reksadana View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Reksadana Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("Reksadana_Akses_Page", "Akses Page Reksadana", pageTitle);
            db.InsertAuditTrail("AksesPageDetailCluster_Akses_Page", "user " + userId + " mengakases halaman Reksadana untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);
            return View();
        }
        //-----------------------------Reksadana-----------------------------------//

        //-----------------------------JaminanMargin-----------------------------------//
        public IActionResult JaminanMargin(DataSourceLoadOptions loadOptions, string id, string periodeAwal)
        {
            var userId = HttpContext.User.Identity.Name;
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Jaminan Margin"; //menampilkan data menu

            string namaPE = id;
            string stringPeriodeAwal = null;
            string stringPeriodeAwalDate = null;
            string stringNamaPE = null;
            string strNamaPEOnly = null;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
                stringPeriodeAwalDate = Convert.ToDateTime(periodeAwal).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
                ViewBag.PeriodeAwalDateParam = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwalDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
                strNamaPEOnly = (string)GetNamaPEOnly(loadOptions, stringNamaPE);
                ViewBag.KodePE = namaPE;
                ViewBag.NamaPE = strNamaPEOnly;
            }
            else
            {
                ViewBag.NamaPE = "";
            }

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

            List<SelectListItem> entityTypelist = new List<SelectListItem>();
            if (dtList.Rows.Count > 0)
            {
                entityTypelist.Add(new SelectListItem() { Value = "", Text = "(ALL)" });
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    entityTypelist.Add(new SelectListItem() { Value = dtList.Rows[i]["exchangemembercode"].ToString(), Text = namakode });
                }
            }
            ViewBag.Jenis = entityTypelist;

            db.Database.CommandTimeout = 420;
            db.CheckPermission("Jaminan Margin View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Jaminan Margin Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("Jaminan_Margin_Akses_Page", "Akses Page Jaminan Margin", pageTitle);
            db.InsertAuditTrail("AksesPageDetailCluster_Akses_Page", "user " + userId + " mengakases halaman Jaminan Margin untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);
            return View();
        }
        //-----------------------------JaminanMargin-----------------------------------//


        //-----------------------------ReverseRepo-----------------------------------//
        public IActionResult ReverseRepo(DataSourceLoadOptions loadOptions, string id, string periodeAwal)
        {
            var userId = HttpContext.User.Identity.Name;
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Reverse Repo"; //menampilkan data menu

            string namaPE = id;
            string stringPeriodeAwal = null;
            string stringPeriodeAwalDate = null;
            string stringNamaPE = null;
            string strNamaPEOnly = null;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
                stringPeriodeAwalDate = Convert.ToDateTime(periodeAwal).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
                ViewBag.PeriodeAwalDateParam = stringPeriodeAwal;
            }
            else
            {
                stringPeriodeAwalDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy MMM dd");
                ViewBag.PeriodeAwalDate = stringPeriodeAwalDate;
            }
            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
                strNamaPEOnly = (string)GetNamaPEOnly(loadOptions, stringNamaPE);
                ViewBag.KodePE = namaPE;
                ViewBag.NamaPE = strNamaPEOnly;
            }
            else
            {
                ViewBag.NamaPE = "";
            }

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

            List<SelectListItem> entityTypelist = new List<SelectListItem>();
            if (dtList.Rows.Count > 0)
            {
                entityTypelist.Add(new SelectListItem() { Value = "", Text = "(ALL)" });
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    entityTypelist.Add(new SelectListItem() { Value = dtList.Rows[i]["exchangemembercode"].ToString(), Text = namakode });
                }
            }
            ViewBag.Jenis = entityTypelist;

            db.Database.CommandTimeout = 420;
            db.CheckPermission("Reverse Repo View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Reverse Repo Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("Reverse_Repo_Akses_Page", "Akses Page Reverse Repo", pageTitle);
            db.InsertAuditTrail("AksesPageDetailCluster_Akses_Page", "user " + userId + " mengakases halaman Reverse Repo untuk digunakan sebagai Pengawasan Perusahaan Efek", pageTitle);
            return View();
        }
        //-----------------------------ReverseRepo-----------------------------------//


        #region Export Index
        public FileResult FileIndex()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "SummaryClusterMKBD_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SummaryClusterMKBD_Akses_Page", "Export Data Excel Summary Cluster MKBD", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFIndex(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SummaryClusterMKBD_Akses_Page", "Export Data PDF Summary Cluster MKBD", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = TempData["pawal"].ToString(); 
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;


                    Style textStylesLeft = workbook.CreateStyle();
                    textStylesLeft.HorizontalAlignment = TextAlignmentType.Left;

                    Style textStylesRight = workbook.CreateStyle();
                    textStylesRight.HorizontalAlignment = TextAlignmentType.Right;

                    StyleFlag textStyleFlag = new StyleFlag();
                    textStyleFlag.HorizontalAlignment = true;


                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[0].Width = 8;
                    worksheet.Cells.Columns[1].Width = 8;
                    worksheet.Cells.Columns[2].Style.VerticalAlignment = TextAlignmentType.Center;
                    worksheet.Cells.Columns[2].ApplyStyle(textStylesLeft, textStyleFlag);

                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[5].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[6].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[7].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[8].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[9].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[10].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[8].Width = 20;
                    worksheet.Cells.Columns[8].ApplyStyle(textStyle, textFlag);
                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "SummaryClusterMKBD_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion


        #region Export Detail
        public FileResult FileDetail()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "DetailClusterMKBD_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexDetail()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Detail Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("DetailClusterMKBD_Akses_Page", "Export Data Excel Detail Cluster MKBD", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFDetail(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Detail Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("DetailClusterMKBD_Akses_Page", "Export Data PDF Detail Cluster MKBD", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[5].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[7].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "DetailClusterMKBD_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }


        public FileResult FileRincianPortofolioEfek()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "RincianPortofolioEfek_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexRincianPortofolioEfek()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Detail Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("DetailClusterMKBD_Akses_Page", "Export Data Excel Rincian Portofolio Efek", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFRincianPortofolioEfek(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Detail Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("DetailClusterMKBD_Akses_Page", "Export Data PDF Rincian Portofolio Efek", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "RincianPortofolioEfek_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion

        #region Export Rincian Portofolio
        public FileResult FileRincianPortofolio()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "RincianPortofolio_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexRincianPortofolio()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Rincian Portofolio Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("RincianPortofolio_Akses_Page", "Export Data Excel Rincian Portofolio", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFRincianPortofolio(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Rincian Portofolio Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("RincianPortofolio_Akses_Page", "Export Data PDF Rincian Portofolio", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[5].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[6].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[7].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[8].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[9].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[11].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[12].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "RincianPortofolio_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult FileRincianPortofolioDetailSummary()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "RincianPortofolioDetailSummary_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexRincianPortofolioDetailSummary()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Rincian Portofolio Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("RincianPortofolio_Akses_Page", "Export Data Excel Detail Summary Rincian Portofolio", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFRincianPortofolioDetailSummary(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Rincian Portofolio Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("RincianPortofolio_Akses_Page", "Export Data PDF Detail Summary Rincian Portofolio", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "RincianPortofolioDetailSummary_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion

        #region Export Reksadana
        public FileResult FileReksadana()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "Reksadana_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexReksadana()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reksadana Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Reksadana_Akses_Page", "Export Data Excel Reksadana", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFReksadana(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reksadana Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Reksadana_Akses_Page", "Export Data PDF Reksadana", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[5].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[6].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[7].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "Reksadana_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult FileReksadanaDetailSummary()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "ReksadanaDetailSummary_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexReksadanaDetailSummary()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reksadana Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Reksadana_Akses_Page", "Export Data Excel Detail Summary Reksadana", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFReksadanaDetailSummary(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reksadana Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("Reksadana_Akses_Page", "Export Data PDF Detail Summary Reksadana", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "ReksadanaDetailSummary_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion

        #region Export Jaminan Margin
        public FileResult FileJaminanMargin()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "JaminanMargin_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexJaminanMargin()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Jaminan Margin Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("JaminanMargin_Akses_Page", "Export Data Excel Jaminan Margin", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFJaminanMargin(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Jaminan Margin Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("JaminanMargin_Akses_Page", "Export Data PDF Jaminan Margin", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);
                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "JaminanMargin_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult FileJaminanMarginDetailSummary()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "JaminanMarginDetailSummary_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexJaminanMarginDetailSummary()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Jaminan Margin Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("JaminanMargin_Akses_Page", "Export Data Excel Detail Summary Jaminan Margin", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFJaminanMarginDetailSummary(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Jaminan Margin Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("JaminanMargin_Akses_Page", "Export Data Detail Summary Jaminan Margin", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "JaminanMarginDetailSummary_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion

        #region Export Reverse Repo
        public FileResult FileReverseRepo()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "ReverseRepo_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexReverseRepo()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reverse Repo Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ReverseRepo_Akses_Page", "Export Data Excel Reverse Repo", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFReverseRepo(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reverse Repo Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ReverseRepo_Akses_Page", "Export Data PDF Reverse Repo", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[5].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[6].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[7].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[8].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[9].ApplyStyle(textStyle, textFlag);
                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "ReverseRepo_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult FileReverseRepoDetailSummary()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "ReverseRepoDetailSummary_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }
        [HttpPost]
        public IActionResult LogExportIndexReverseRepoDetailSummary()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reverse Repo Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ReverseRepo_Akses_Page", "Export Data Excel Detail Summary Reverse Repo", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult LogExportPDFReverseRepoDetailSummary(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Reverse Repo Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ReverseRepo_Akses_Page", "Export Data PDF Detail Summary Reverse Repo", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
                Workbook workbook = new Workbook(file.OpenReadStream());
                Worksheet worksheet2 = workbook.Worksheets[0];
                var columns1 = worksheet2.Cells.Columns.Count;
                var rows1 = worksheet2.Cells.Rows.Count;
                var style = workbook.CreateStyle();
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thick, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thick, Color.Black);

                //Apply bottom borders from cell F4 till K4
                for (int r = 0; r <= rows1 - 1; r++)
                {
                    for (int col = 0; col <= columns1 - 1; col++)
                    {
                        Aspose.Cells.Cell cell = worksheet2.Cells[r, col];

                        cell.SetStyle(style);
                    }
                }

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;
                    worksheet.AutoFitRows(true);
                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[4].ApplyStyle(textStyle, textFlag);
                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, timeStamp);

                    inFile.Close();
                }

                timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
                TempData["timeStamp"] = timeStamp;
                var fileName = "ReverseRepoDetailSummary_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        #endregion
    }
}
