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
using DevExpress.CodeParser;
using System.ServiceModel.Dispatcher;
using MathNet.Numerics.Statistics;
using System.Net;

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
        public IActionResult SumTxSID(string pe, string periode, string sid, string tradeid)
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
            ViewBag.tradeid = tradeid;

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
        public object GetDatasAll(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string invType, string invOrigin, string inRange, string market, int type)
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

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM");
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

            var result = Helper.WSQueryStore.GetPS07ALL(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvType, stringInvOrigin, stringInRange, stringMarket, cekHive);

            var data = result.data.AsEnumerable();

            int totalClients = data.Count(row => row.Field<string>("tradeid") != null);
            int activeClients = data.Count(row => { int itf; return int.TryParse(row.Field<string>("investortransactionfreq"), out itf) && itf > 30; });
            long trxFreq = data.Sum(row => Convert.ToInt64(row.Field<string>("investortransactionfreq")));
            long tradedValue = data.Sum(row => Convert.ToInt64(row.Field<string>("investortotalvalue")));
            long clientLiquidAmount = data.Sum(row => Convert.ToInt64(row.Field<string>("portofolio_amount")));

            double intrxfreq, ttlcli;
            intrxfreq = ttlcli = 0;

            List<string> segments = ["Champion", "Potential Loyalists", "Promising", "Recent Customer", "At Risk", "Churn"];
            List<double> itf = new List<double>();
            List<double> tc = new List<double>();

            foreach (var segment in segments)
            {
                intrxfreq = data.Where(row => row.Field<string>("basis_investor_1") == segment).Sum(row => Convert.ToInt64(row.Field<string>("investortotalvalue"))); ;
                ttlcli = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("basis_investor_1") == segment);

                itf.Add(intrxfreq);
                tc.Add(ttlcli);

                intrxfreq = ttlcli = 0;
            }

            double totalITF = itf.Sum();
            double totalTC = tc.Sum();

            itf = itf.Select(val => val/totalITF).ToList();
            tc = tc.Select(val => val / totalTC).ToList();


            double rh, rm, rl, rtotal, fh, fm, fl, ftotal, mh, mm, ml, mtotal, originAsing, originLokal, originTotal, client, newClient, totalClient, typeIndv, typeInst, typeTotal;
            rh = rm = rl = rtotal = fh = fm = fl = ftotal = mh = mm = ml = mtotal = originAsing = originLokal = originTotal = client = newClient = totalClient = typeIndv = typeInst = typeTotal = 0;

            if (intType == 1)
            {
                rh = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("r") == "H");
                rm = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("r") == "M");
                rl = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("r") == "L");
                rtotal = rh + rm + rl;
                rh /= rtotal; rm /= rtotal; rl /= rtotal;

                fh = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("f") == "H");
                fm = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("f") == "M");
                fl = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("f") == "L");
                ftotal = fh + fm + fl;
                fh /= rtotal; fm /= ftotal; fl /= ftotal;

                mh = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("m") == "H");
                mm = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("m") == "M");
                ml = data.Count(row => row.Field<string>("tradeid") != null & row.Field<string>("m") == "L");
                mtotal = mh + mm + ml;
                mh /= mtotal; mm /= mtotal; ml /= mtotal;
            }
            else
            {
                rh = data.Where(row => row.Field<string>("r") == "H").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                rm = data.Where(row => row.Field<string>("r") == "M").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                rl = data.Where(row => row.Field<string>("r") == "L").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                rtotal = rh + rm + rl;
                rh /= rtotal; rm /= rtotal; rl /= rtotal;

                fh = data.Where(row => row.Field<string>("f") == "H").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                fm = data.Where(row => row.Field<string>("f") == "M").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                fl = data.Where(row => row.Field<string>("f") == "L").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                ftotal = fh + fm + fl;
                fh /= rtotal; fm /= ftotal; fl /= ftotal;

                mh = data.Where(row => row.Field<string>("m") == "H").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                mm = data.Where(row => row.Field<string>("m") == "M").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                ml = data.Where(row => row.Field<string>("m") == "L").Sum(row => Convert.ToInt32(row.Field<string>("investortotalvalue")));
                mtotal = mh + mm + ml;
                mh /= mtotal; mm /= mtotal; ml /= mtotal;
            }

            switch(stringInvOrigin)
            {
                case "Asing":
                    originAsing = 1; originLokal = 0; break;

                case "Lokal":
                    originAsing = 0; originLokal = 1; break;

                case null:
                    originLokal = data.Count(row => row.Field<string>("investor_origin") == "Lokal");
                    originAsing = data.Count(row => row.Field<string>("investor_origin") == "Asing");
                    originTotal = originAsing + originLokal;
                    originAsing /= originTotal; originLokal /= originTotal;
                    break;
            }

            client = 1; newClient = 0;

            switch (stringInvType)
            {
                case "Individu":
                    typeIndv = 1; typeInst = 0; break;

                case "Institusi":
                    typeIndv = 0; typeInst = 1; break;

                case null:
                    typeIndv = data.Count(row => row.Field<string>("investor_type") == "Individu");
                    typeInst = data.Count(row => row.Field<string>("investor_type") == "Institusi");
                    typeTotal = typeIndv + typeInst;
                    typeIndv /= typeTotal; typeInst /= typeTotal;
                    break;
            }

            List<double> R = [rh, rm, rl];
            List<double> F = [fh, fm, fl];
            List<double> M = [mh, mm, ml];
            List<double> investorOrigin = [originAsing, originLokal];
            List<double> clientVNewClient = [client, newClient];
            List<double> investorType = [typeIndv, typeInst];
            List<string> segmentsSorted = new List<string>(segments);
            segmentsSorted.Sort();

            var scatterData = segmentsSorted
                              .Select(s =>
                              {
                                  var group = data.Where(item => item.Field<string>("basis_investor_1") == s).ToList();

                                  return new
                                  {
                                      basis_investor = s,
                                      log10_sid = group.Any() ? Math.Log10(group.Count(item => item.Field<string>("sid") != null)) : 0,
                                      med_inv_days = group.Any() ? Statistics.Median(group.Select(item => Convert.ToDouble(item.Field<string>("investorlasttransactionindays"))).ToArray()) : 0,
                                      med_inv_tot_val = group.Any() ? (Statistics.Median(group.Select(item => Convert.ToDouble(item.Field<string>("investortotalvalue"))).ToArray()) / 1000000) : 0
                                  };
                              })
                              .OrderBy(result => result.basis_investor)
                              .ToList();

            DataTable dtsd = Helper.WSQueryStore.LINQResultToDataTable(scatterData);


            DataBasisInvestor dbi = new DataBasisInvestor();

            dbi.totalClients = totalClients;
            dbi.activeClients = activeClients;
            dbi.trxFreq = trxFreq;
            dbi.tradedValue = tradedValue;
            dbi.clientLiquidAmount = clientLiquidAmount;

            dbi.segmentsITF = itf;
            dbi.segmentsTC = tc;

            dbi.R = R;
            dbi.F = F;
            dbi.M = M;
            dbi.investorOrigin = investorOrigin;
            dbi.clientVNewClient = clientVNewClient;
            dbi.investorType = investorType;

            dbi.scatterData = dtsd;

            return JsonConvert.SerializeObject(dbi);
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

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM");
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

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM");
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
            var result = Helper.WSQueryStore.GetPS07CPGTRX(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringInvCode, stringTrxSys, stringSecCode, cekHive);

            return JsonConvert.SerializeObject(result);
        }

        public object GetGridDetailSRE(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string tradeId, string trxSys, string secCode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering

            string stringPeriodeAwal = null;
            string stringNamaPE = null;
            string stringTradeId = null;
            string stringTrxSys = null;
            string stringSecCode = null;
            string reportId = "ps_basis_inv_pe"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            stringPeriodeAwal = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM");
            TempData["pawal"] = stringPeriodeAwal;

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM");
                TempData["pawal"] = stringPeriodeAwal;
            }

            if (namaPE != null)
            {
                stringNamaPE = namaPE;
                TempData["pe"] = stringNamaPE;
            }

            if (tradeId != null)
            {
                stringTradeId = tradeId;
                TempData["tradeId"] = stringTradeId;
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
            var result = Helper.WSQueryStore.GetPS07CGridSRE(db, loadOptions, stringPeriodeAwal, stringNamaPE, stringTradeId, stringTrxSys, stringSecCode, cekHive);
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
                    Style numericStyle = workbook.CreateStyle();
                    numericStyle.Custom = "#,##0.00";
                    numericStyle.HorizontalAlignment = TextAlignmentType.Right;
                    StyleFlag numericFlag = new StyleFlag();
                    numericFlag.NumberFormat = true;
                    numericFlag.HorizontalAlignment = true;

                    // Create header style with text wrapping
                    Style headerStyle = workbook.CreateStyle();
                    headerStyle.IsTextWrapped = true; // Enable text wrapping
                    headerStyle.HorizontalAlignment = TextAlignmentType.Center;
                    headerStyle.VerticalAlignment = TextAlignmentType.Center;
                    headerStyle.Font.IsBold = true;
                    headerStyle.Font.Size = 10;

                    StyleFlag headerFlag = new StyleFlag();
                    headerFlag.WrapText = true;
                    headerFlag.HorizontalAlignment = true;
                    headerFlag.VerticalAlignment = true;
                    headerFlag.FontBold = true;
                    headerFlag.FontSize = true;

                    // Apply header style to first row (assuming first row contains headers)
                    Aspose.Cells.Range headerRange = worksheet.Cells.CreateRange(0, 0, 1, worksheet.Cells.MaxDataColumn + 1);
                    headerRange.ApplyStyle(headerStyle, headerFlag);

                    // Alternative: Apply header style only to cells that contain data in first row
                    for (int col = 0; col <= worksheet.Cells.MaxDataColumn; col++)
                    {
                        Cell headerCell = worksheet.Cells[0, col];
                        if (!string.IsNullOrEmpty(headerCell.StringValue))
                        {
                            headerCell.SetStyle(headerStyle, headerFlag);
                        }
                    }

                    // Apply numeric formatting to data cells (excluding header row)
                    foreach (Cell cell in worksheet.Cells)
                    {
                        if (cell.Type == CellValueType.IsNumeric && cell.Row > 0) // Skip header row
                        {
                            cell.SetStyle(numericStyle);
                        }
                    }

                    // Auto-fit row height for header row to accommodate wrapped text
                    worksheet.AutoFitRow(0);

                    // Optional: Set minimum row height for header
                    worksheet.Cells.SetRowHeight(0, 30); // Set minimum height to 30 points

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

        [HttpGet]
        public object GetNamaNegara(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaNegara>();

            string reportId = "MasterNegara"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = false; //Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql


            var result = Helper.WSQueryStore.GetBDAPMNegara(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;

            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               NamaNegara = bs.Field<string>("NamaNegara").ToString(),
                           }).OrderBy(bs => bs.NamaNegara).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string var = dtList.Rows[i]["NamaNegara"].ToString();
                    list.Add(new NamaNegara() { value = var, text = var });
                }
            }

            var res = DataSourceLoader.Load(list, loadOptions);

            return Json(res);
        }

        [HttpGet]
        public object GetNamaProvinsi(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaProvinsi>();

            string reportId = "MasterPropinsi"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = false; //Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql


            var result = Helper.WSQueryStore.GetBDAPMProvinsi(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;

            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               RefPropinsiNama = bs.Field<string>("RefPropinsiNama").ToString(),
                           }).OrderBy(bs => bs.RefPropinsiNama).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string var = dtList.Rows[i]["RefPropinsiNama"].ToString();
                    list.Add(new NamaProvinsi() { value = var, text = var });
                }
            }

            var res = DataSourceLoader.Load(list, loadOptions);

            return Json(res);
        }

        [HttpGet]
        public object GetNamaKota(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;
            string strSQL = db.appSettings.DataConnString;
            var list = new List<NamaKota>();

            string reportId = "MasterKota2"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table
            var cekHive = false; //Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql


            var result = Helper.WSQueryStore.GetBDAPMKota(db, loadOptions, reportId, cekHive);
            var varDataList = (dynamic)null;

            varDataList = (from bs in result.data.AsEnumerable() //lempar jadi linq untuk bisa di order by no urut
                           select new
                           {
                               NamaKota = bs.Field<string>("NamaKota").ToString(),
                           }).OrderBy(bs => bs.NamaKota).ToList();

            DataTable dtList = new DataTable();
            dtList = Helper.WSQueryStore.LINQResultToDataTable(varDataList);

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    string var = dtList.Rows[i]["NamaKota"].ToString();
                    list.Add(new NamaKota() { value = var, text = var });
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

        public class DataBasisInvestor
        {
            public int totalClients { get; set; }
            public int activeClients { get; set; }
            public long trxFreq { get; set; }
            public long tradedValue { get; set; }
            public long clientLiquidAmount { get; set; }

            public List<double> segmentsITF { get; set; }
            public List<double> segmentsTC { get; set; }

            public List<double> R { get; set; }
            public List<double> F { get; set; }
            public List<double> M { get; set; }

            public List<double> investorOrigin { get; set; }
            public List<double> clientVNewClient { get; set; }
            public List<double> investorType { get; set; }

            public DataTable scatterData { get; set; }

        }

        public class NamaNegara
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public class NamaProvinsi 
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public class NamaKota
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
