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
using DevExpress.Pdf;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using BDA.Helper.FW;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class BasisInvestorController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public BasisInvestorController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Basis Investor View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Basis Investor Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Basis_Investor_Page", "Akses Page Basis Investor", pageTitle); //simpan kedalam audit trail

            return View();
        }

        public IActionResult Detail(string pe, string periode)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Detail Basis Investor View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Basis Investor Detail Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Detail_Basis_Investor_Page", "Akses Page Detail Basis Investor", pageTitle); //simpan kedalam audit trail

            ViewBag.pe = pe;
            ViewBag.periode = periode;

            return View();
        }
        public IActionResult SumTxSID(string pe, string periode, string sid)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Summary Transaction SID View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Summary Transaction SID Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Summary_Transaction_SID_Page", "Akses Page Summary Transaction SID", pageTitle); //simpan kedalam audit trail

            ViewBag.pe = pe;
            ViewBag.periode = periode;
            ViewBag.sid = sid;

            return View();
        }

        public FileResult FileIndex(string name)
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "BasisInvestor_" + name + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        #region PS07
        public object GetCardTotalClients(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;

            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07TotalClientsQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardActiveClient(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;

            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07ActiveClientQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardTrxFreq(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07TrxFreqQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardtradedValue(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07TradedValueQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetCardClientLiquidAmt(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07ClientLiquidAmtQuery(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetBarChartSegments(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market, string segment)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string stringSegment = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            if (segment != null)
            {
                stringSegment = segment;
                TempData["segment"] = stringSegment;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07Segments(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, stringSegment, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetPieChartsRFM(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market, int type, string r, string f, string m)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            int intType = 1;
            string stringR = null;
            string stringF = null;
            string stringM = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            if (type != 1)
            {
                intType = type;
                TempData["type"] = intType;

            }

            if (r != null)
            {
                stringR = r;
                TempData["r"] = stringR;

            }

            if (f != null)
            {
                stringF = f;
                TempData["f"] = stringF;

            }

            if (m != null)
            {
                stringM = m;
                TempData["m"] = stringM;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07PieChartsRFM(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, intType, stringR, stringF, stringM, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetPieChartInv(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market, int type)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            int intType = 1;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            if (type != 1)
            {
                intType = type;
                TempData["type"] = intType;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07PieChartInv(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, intType, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetScatter(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvType = null;
            string stringInvOrigin = null;
            string stringInRange = null;
            string stringMarket = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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

            if (invType != null)
            {
                stringInvType = invType;
                TempData["invType"] = stringInvType;
            }

            if (invOrigin != null)
            {
                stringInvOrigin = invOrigin;
                TempData["invOrigin"] = stringInvOrigin;
            }

            if (inRange != null)
            {
                stringInRange = inRange;
                TempData["inRange"] = stringInRange;
            }

            if (market != null)
            {
                stringMarket = market;
                TempData["market"] = stringMarket;

            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07Scatter(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region PS07B
        public object GetGridDetail(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string negara, string provinsi, string kota, string jenisKelamin, string usia, string pendidikan, string pekerjaan, string penghasilan)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringNegara = null;
            string stringProvinsi = null;
            string stringKota = null;
            string stringJenisKelamin = null;
            string stringUsia = null;
            string stringPendidikan = null;
            string stringPekerjaan = null;
            string stringPenghasilan = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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
                TempData["pe"] = stringNamaPE;
            }

            if (negara != null)
            {
                stringNegara = negara;
                TempData["negara"] = stringNegara;
            }

            if (provinsi != null)
            {
                stringProvinsi = provinsi;
                TempData["provinsi"] = stringProvinsi;
            }

            if (kota != null)
            {
                stringKota = kota;
                TempData["kota"] = stringKota;
            }

            if (jenisKelamin != null)
            {
                stringJenisKelamin = jenisKelamin;
                TempData["jenisKelamin"] = stringJenisKelamin;

            }

            if (usia != null)
            {
                stringUsia = usia;
                TempData["usia"] = stringUsia;
            }

            if (pendidikan != null)
            {
                stringPendidikan = pendidikan;
                TempData["pendidikan"] = stringPendidikan;
            }

            if (pekerjaan != null)
            {
                stringPekerjaan = pekerjaan;
                TempData["pekerjaan"] = stringPekerjaan;
            }

            if (penghasilan != null)
            {
                stringPenghasilan = penghasilan;
                TempData["penghasilan"] = stringPenghasilan;
            }


            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07BGrid(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringNegara, stringProvinsi, stringKota, stringJenisKelamin, stringUsia, stringPendidikan, stringPekerjaan, stringPenghasilan, cekHive);

            foreach (DataRow row in result.data.Rows)
            {
                if (row["newsre"] != DBNull.Value)
                {
                    var val = row["newsre"].ToString();

                    if (val == "NO")
                    {
                        row["newsre"] = "Client";
                    }
                    else
                    {
                        row["newsre"] = "New Client";
                    }
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        public IActionResult LogExportBIDetail()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Detail Basis Investor Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("BasisInvestorDetail_Akses_Page", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

       

        public IActionResult ExportPDF(IFormFile file, string name)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Detail Basis Investor Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("BasisInvestorDetail_Akses_Page", "Export Data", pageTitle);

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
                var fileName = "BasisInvestor_" + name + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        #endregion

        #region PS07C
        public object GetGridDetailTRX(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invCode, string trxSys, string secCode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringInvCode = null;
            string stringTrxSys = null;
            string stringSecCode = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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
                TempData["pe"] = stringNamaPE;
            }

            if (invCode != null)
            {
                stringInvCode = invCode;
                TempData["invCode"] = stringInvCode;
            }

            if (trxSys != null)
            {
                stringTrxSys = trxSys;
                TempData["trxSys"] = stringTrxSys;
            }

            if (secCode != null)
            {
                stringSecCode = secCode;
                TempData["secCode"] = stringSecCode;
            }

            db.Database.CommandTimeout = 420;
            //var result = Helper.WSQueryStore.GetPS07CGridTRX(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvCode, stringTrxSys, stringSecCode, cekHive);
            var result = Helper.WSQueryStore.GetPS07CPGTRX(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvCode, stringTrxSys, stringSecCode, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetGridDetailSRE(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string sid, string trxSys, string secCode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringSID = null;
            string stringTrxSys = null;
            string stringSecCode = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

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
                TempData["pe"] = stringNamaPE;
            }

            if (sid != null)
            {
                stringSID = sid;
                TempData["sid"] = stringSID;
            }

            if (trxSys != null)
            {
                stringTrxSys = trxSys;
                TempData["trxSys"] = stringTrxSys;
            }

            if (secCode != null)
            {
                stringSecCode = secCode;
                TempData["secCode"] = stringSecCode;
            }

            db.Database.CommandTimeout = 420;
            var result = Helper.WSQueryStore.GetPS07CGridSRE(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringSID, stringTrxSys, stringSecCode, cekHive);
            //var processedData = (from row in result.data.AsEnumerable()
            //                       group row by new
            //                       {
            //                           pe = row.Field<string>("pe"),
            //                           sid = row.Field<string>("sid"),
            //                           secphytcode = row.Field<string>("secphytcode"),
            //                           stlactowntcode = row.Field<string>("stlactowntcode"),
            //                           stlacttcode = row.Field<string>("stlacttcode"),
            //                           actblcstscode = row.Field<string>("actblcstscode")
            //                       } into g
            //                       select new
            //                       {
            //                           pe = g.Key.pe,
            //                           sid = g.Key.sid,
            //                           secphytcode = g.Key.secphytcode,
            //                           stlactowntcode = g.Key.stlactowntcode,
            //                           stlacttcode = g.Key.stlacttcode,
            //                           actblcstscode = g.Key.actblcstscode,
            //                           portoamount = g.Sum(x => double.TryParse(x.Field<string>("portoamount"), out double valPA) ? valPA : 0d),
            //                           portoqty = g.Sum(x => double.TryParse(x.Field<string>("portoqty"), out double valPQ) ? valPQ : 0d)
            //                       });

            //DataTable dt = new DataTable();
            //dt = Helper.WSQueryStore.LINQResultToDataTable(processedData);

            //var processedResult = new WSQueryReturns { data = dt, totalCount = dt.Rows.Count };

            //return JsonConvert.SerializeObject(processedResult);
            return JsonConvert.SerializeObject(result);
        }

        public IActionResult LogExportTRXDetail()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Transaction SID Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SummaryTransactionSID_Akses_Page", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult LogExportSREDetail()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Transaction SID Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SummaryTransactionSID_Akses_Page", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult ExportPDFSumSID(IFormFile file, string name)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Transaction SID Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SummaryTransactionSID_Akses_Page", "Export Data", pageTitle);

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
                var fileName = "BasisInvestor_" + name + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        #endregion 

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

        [HttpGet]
        public object GetNamaPE(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaPE>();

            string reportId = "dim_exchange_members"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            
            var result = Helper.WSQueryStore.GetBDAPMNamaPEv2(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;

            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               exchangemembercode = bs.Field<string>("exchangemembercode").ToString(),
                               exchangemembername = bs.Field<string>("exchangemembername").ToString(),
                           }).OrderBy(bs => bs.exchangemembername).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["exchangemembercode"].ToString() + " - " + dtList.Rows[i]["exchangemembername"].ToString();
                    list.Add(new NamaPE() { value = dtList.Rows[i]["exchangemembercode"].ToString(), text = namakode });
                }
            }

            var res = DataSourceLoader.Load(list, loadOptions);

            return Json(res);
        }

        [HttpGet]
        public object GetNamaSecurities(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaSecurities>();

            string reportId = "dim_securities"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql


            var result = Helper.WSQueryStore.GetBDAPMSecurities(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;

            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               securitycode = bs.Field<string>("securitycode").ToString(),
                               securityname = bs.Field<string>("securityname").ToString(),
                           }).OrderBy(bs => bs.securityname).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string namakode = dtList.Rows[i]["securitycode"].ToString() + " | " + dtList.Rows[i]["securityname"].ToString();
                    list.Add(new NamaSecurities() { value = dtList.Rows[i]["securitycode"].ToString(), text = namakode });
                }
            }

            var res = DataSourceLoader.Load(list, loadOptions);

            return Json(res);
        }

        public class NamaPE
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public class NamaSecurities
        {
            public string value { get; set; }
            public string text { get; set; }
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
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
    }
}
