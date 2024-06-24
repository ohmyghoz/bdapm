using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
//using DevExpress.Spreadsheet;
using Aspose.Cells;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Dashboard")]
    public class SLIKController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public SLIKController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index(string lvl, string periode, string tipe, string tperiode, string uid, string jns)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Dashboard & Reporting Alert SLIK View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            db.InsertAuditTrail("EWSSLIK_Akses_Page", "Akses Page Dashboard EWS SLIK", pageTitle);

            TempData.Clear();
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.NoMessage);

            if (!string.IsNullOrWhiteSpace(lvl))
            {
                TempData["LevelAlert"] = lvl?.Split(",");
            }
            if (!string.IsNullOrWhiteSpace(tperiode))
            {
                TempData["tipePeriode"] = tperiode;
            }
            if (!string.IsNullOrWhiteSpace(jns))
            {
                var j = "";
                if (jns == "tp") {
                    j = "Total Permintaan";
                }
                else if (jns == "ind")
                {
                    j = "Individu";
                }
                else if (jns == "nind")
                {
                    j = "NonIndividu";
                }
                else if (jns == "bat")
                {
                    j = "Batch";
                }
                else if (jns == "inte")
                {
                    j = "Interactive";
                }
                TempData["PermintaanIdeb"] = j;
            }
            if (!string.IsNullOrWhiteSpace(uid))
            {
                var ui = Base64Decode(uid);
                var listJLJK = db.getLJKPengawas(ui).Select(x => x.member_type_code).Distinct();
                var listLJK = db.getLJKPengawas(ui).Select(x => x.member_code).Distinct();
                var singleStringJLJK = string.Join(",", listJLJK.ToArray());
                var singleStringLJK = string.Join(",", listLJK.ToArray());
                TempData["jljk"] = singleStringJLJK.Split(",");
                TempData["ljk"] = singleStringLJK.Split(",");
            }
            if (!string.IsNullOrWhiteSpace(periode))
            {
                TempData["PeriodeAwal"] = DateTime.ParseExact(periode, "yyyyMMdd", CultureInfo.InvariantCulture);
                TempData["PeriodeAkhir"] = DateTime.ParseExact(periode, "yyyyMMdd", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrWhiteSpace(tipe))
            {
                var t = "";
                if (tipe == "KC") {
                    t = "UNI - Kantor Cabang";
                }else if (tipe == "SB")
                {
                    t = "UNI - Size Bisnis Pelapor";
                }
                else if (tipe == "RWP")
                {
                    t = "UNI - Rentang Waktu";
                }
                else if (tipe == "RJP")
                {
                    t = "UNI - Rata-Rata Jumlah Permintaan";
                }
                else if (tipe == "UP")
                {
                    t = "UNI - User Permintaan";
                }
                TempData["JenisAlert"] = t?.Split(",");
            }

            return View();
        }

        public bool IsPengawasLJK()
        {
            var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("PengawasLJK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] GetFilteredMemberTypes(string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_type_code).Distinct().ToArray();
            return filter;
        }

        public string[] GetFilteredMembers(string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_code).ToArray();
            return filter;
        }

        [HttpPost]
        public IActionResult Detail(string memberType, string member, string kodeAlert)
        {

            //if (!string.IsNullOrWhiteSpace(memberType))
            //{
            //    TempData["DetailMemberType"] = memberType;
            //}

            //if (!string.IsNullOrWhiteSpace(member))
            //{
            //    TempData["DetailMember"] = member;
            //}

            //if (!string.IsNullOrWhiteSpace(kodeAlert))
            //{
            //    TempData["DetailKodeAlert"] = kodeAlert;
            //}

            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;

            //return Json(new { result = "Redirect", url = Url.Action("Detail", "SLIK") });

            string stringLevelAlert = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string dataMart = null;
            string permintaanIDeb = null;
            string stringMemberType = null;
            string stringMember = null;

            if (TempData.Peek("LevelAlert") != null)
            {
                stringLevelAlert = string.Join(",", (string[])TempData.Peek("LevelAlert"));
            }
            else
            {
                stringLevelAlert = "2,3";
            }

            if (TempData.Peek("StringPeriodeAwal") != null)
            {
                stringPeriodeAwal = TempData.Peek("StringPeriodeAwal").ToString().Replace("-", "");
            }

            if (TempData.Peek("StringPeriodeAkhir") != null)
            {
                stringPeriodeAkhir = TempData.Peek("StringPeriodeAkhir").ToString().Replace("-", "");
            }

            if (TempData.Peek("DataMart") != null)
            {
                dataMart = TempData.Peek("DataMart").ToString();
            }

            if (TempData.Peek("PermintaanIDeb") != null)
            {
                permintaanIDeb = TempData.Peek("PermintaanIDeb").ToString();
            }

            if (!string.IsNullOrWhiteSpace(memberType))
            {
                stringMemberType = memberType;
            }

            if (!string.IsNullOrWhiteSpace(member))
            {
                stringMember = member;
            }



            if (kodeAlert == "UNI - Kantor Cabang")
            {
                return Json(new { result = "Redirect", url = Url.Action("Index", "KantorCabang", new { lvl = stringLevelAlert, from = stringPeriodeAwal, until = stringPeriodeAkhir, type = stringMemberType, member = stringMember, datamart = dataMart, ideb = permintaanIDeb }) });
            }

            else if (kodeAlert == "UNI - User Permintaan")
            {
                return Json(new { result = "Redirect", url = Url.Action("Index", "UserPermintaan", new { lvl = stringLevelAlert, from = stringPeriodeAwal, until = stringPeriodeAkhir, type = stringMemberType, member = stringMember, datamart = dataMart, ideb = permintaanIDeb }) });
            }

            else if (kodeAlert == "UNI - Size Bisnis Pelapor")
            {
                return Json(new { result = "Redirect", url = Url.Action("Index", "SizeBisnisPelapor", new { lvl = stringLevelAlert, from = stringPeriodeAwal, until = stringPeriodeAkhir, type = stringMemberType, member = stringMember, datamart = dataMart, ideb = permintaanIDeb }) });
            }

            else if (kodeAlert == "UNI - Rentang Waktu")
            {
                return Json(new { result = "Redirect", url = Url.Action("Index", "RentangWaktuPermintaan", new { lvl = stringLevelAlert, from = stringPeriodeAwal, until = stringPeriodeAkhir, type = stringMemberType, member = stringMember, datamart = dataMart, ideb = permintaanIDeb }) });
            }

            else if (kodeAlert == "UNI - Rata-Rata Jumlah Permintaan")
            {
                return Json(new { result = "Redirect", url = Url.Action("Index", "RatarataJumlahPermintaan", new { lvl = stringLevelAlert, from = stringPeriodeAwal, until = stringPeriodeAkhir, type = stringMemberType, member = stringMember, datamart = dataMart, ideb = permintaanIDeb }) });
            }

            else
            {
                return Json(new { result = "Error" });
            }
        }

        [HttpPost]
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_EWSSLIK", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
            
        }

        [HttpPost]
        public IActionResult LogExportDetail()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportDetail_EWSSLIK", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        [HttpPost]
        public IActionResult LogExportInquiry()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportInquiry_EWSSLIK", "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });

            }
        }

        public IActionResult Detail()
        {
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.NoMessage);

            return View();
        }

        public IActionResult ExportPDF(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_EWSSLIK", "Export Data", pageTitle);

                var directory = _env.WebRootPath;

                var timeStamp = DateTime.Now.ToString();

                //Workbook workbook = new Workbook();
                //workbook.Open(file.OpenReadStream(), FileFormatType.Xlsx);
                Workbook workbook = new Workbook(file.OpenReadStream());

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    //prepare logo
                    string logo_url = Path.Combine(directory, "assets_m\\img\\OJK_Logo.png");
                    FileStream inFile;
                    byte[] binaryData;
                    inFile = new FileStream(logo_url, FileMode.Open, FileAccess.Read);
                    binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);

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
                var fileName = "AlertSLIK_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult File()
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "AlertSLIK_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        public IActionResult Record(long? id)
        {
            //var roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            //ViewBag.Akses = roleId;
            ViewBag.Export = db.CheckPermission("Dashboard & Reporting Alert SLIK Export", DataEntities.PermissionMessageType.NoMessage);

            if (id == null) return BadRequest();

            var obj = db.Alert_Summary.Find(id);

            TempData["rc"] = 0;
            string q = "select count(*) c from inquiry_enh_det ied LEFT OUTER JOIN master_user_ideb u ON(ied.user_id = u.user_id)  " + System.Environment.NewLine;
            q = q + "WHERE ied.p_priode like '" + obj.PERIODE.ToString("yyyyMM") + "' AND ied.member_type_code='" + obj.MEMBER_TYPE_CODE + "' AND ied.member_code='" + obj.MEMBER_CODE + "' " + System.Environment.NewLine;
            if (obj.KODE_ALERT == "UP")
            {
                q = q + "AND ied.user_id='" + obj.DIMENSI2 + "' ";
            }
            else if (obj.KODE_ALERT == "KC")
            {
                if (obj.OFFICE_CODE != null)
                {
                    string kc1 = obj.OFFICE_CODE.Split("-")[1];
                    q = q + "AND ied.byrf_office_code='" + kc1 + "' ";
                } 
            }

            if (obj.DIMENSI1 == "Individu" || obj.DIMENSI1 == "NonIndividu")
            {

                if (obj.DIMENSI1 == "Individu")
                {
                    q = q + "AND (ied.debtor_type_flag='I' OR ied.debtor_type_flag='i') ";
                }
                else
                {
                    q = q + "AND UPPER(ied.debtor_type_flag)<>'I' ";
                }
            }
            if (obj.DIMENSI1 == "Batch" || obj.DIMENSI1 == "Interactive")
            {
                if (obj.DIMENSI1 == "Batch")
                {
                    q = q + "AND ied.is_batch_flag='Y' ";
                }
                else
                {
                    q = q + "AND ied.is_batch_flag='T' ";
                }
            }

            //var conn = new OdbcConnection
            //{
            //    ConnectionString = @"DRIVER={Hortonworks Hive ODBC Driver};                                        
            //                            Host=10.225.60.14;
            //                            Port=10500;
            //                            Schema=ojk;
            //                            HiveServerType=2;
            //                            KrbHostFQDN={bgrdco-bddvmst1.ojk.go.id};
            //                            KrbServiceName={hive};
            //                            AuthMech=1;"
            //};
            var conn = new OdbcConnection("DSN=" + db.GetSetting("HiveDSN"));
            try
            {

                conn.Open();


                OdbcCommand myCommand = new OdbcCommand(q, conn);
                myCommand.CommandTimeout = 1000;
                DataTable table = new DataTable();
                table.Load(myCommand.ExecuteReader());
                var ds = new DataSet();
                ds.Tables.Add(table);
                var dt = ds.Tables[0];

                foreach (DataRow dtRow in dt.Rows)
                {
                    TempData["rc"] = dtRow["c"].ToString();
                }
            }
            catch (Exception ex)
            {
                //return null;
                throw new InvalidOperationException(ex.Message);
                // log.Info("Failed to connect to data source");
            }
            finally
            {
                conn.Close();
            }

            if (obj == null) return NotFound();

            return View(obj);
        }

        #region "GetGridData"
        public object GetGridData(DataSourceLoadOptions loadOptions, string levelAlert, string jenisAlert, string memberTypes, string members, string periodeAwal, string periodeAkhir, string dataMart, string permintaanIDeb)
        {
            string stringMemberTypes2 = null;
            string stringMembers2 = null;
            if (TempData.Peek("jljk") != null)
            {
                stringMemberTypes2 = string.Join(",", (string[])TempData.Peek("jljk"));
            }
            if (TempData.Peek("ljk") != null)
            {
                stringMembers2 = string.Join(",", (string[])TempData.Peek("ljk"));
            }
            TempData.Clear();

            var login = this.User.Identity.Name;

            string[] LevelAlert = JsonConvert.DeserializeObject<string[]>(levelAlert);
            string[] JenisAlert = JsonConvert.DeserializeObject<string[]>(jenisAlert);
            string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
            string[] Members = JsonConvert.DeserializeObject<string[]>(members);

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            string stringLevelAlert = null;
            string stringJenisAlert = null;
            string stringMemberTypes = null;
            string stringMembers = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;

            if (LevelAlert.Length > 0)
            {
                TempData["LevelAlert"] = LevelAlert;
                stringLevelAlert = string.Join(", ", LevelAlert);
            }
            else
            {
                stringLevelAlert = "2, 3";
            }

            if (JenisAlert.Length > 0)
            {
                TempData["JenisAlert"] = JenisAlert;
                stringJenisAlert = string.Join(", ", JenisAlert);
            }

            /*check pengawas LJK*/
            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                var filter2 = GetFilteredMembers(login);

                if (MemberTypes.Length == 0)
                {
                    stringMemberTypes = string.Join(", ", filter);
                }
                else
                {
                    if (!MemberTypes.Except(filter).Any() == false)
                    {
                        throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk Jenis LJK ini");
                    }
                }

                if (Members.Length == 0)
                {
                    stringMembers = string.Join(", ", filter2);
                }
                else
                {
                    if (!Members.Except(filter2).Any() == false)
                    {
                        throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk LJK ini");
                    }
                }
            }

            if (MemberTypes.Length > 0)
            {
                TempData["MemberTypes"] = MemberTypes;
                stringMemberTypes = string.Join(", ", MemberTypes);
            }

            if (Members.Length > 0)
            {
                //TempData["Members"] = Members;
                TempData["Members"] = JsonConvert.DeserializeObject<string[]>(members);
                stringMembers = string.Join(", ", Members);
            }

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["PeriodeAwal"] = Convert.ToDateTime(periodeAwal);
                TempData["StringPeriodeAwal"] = stringPeriodeAwal;
            }

            if (periodeAkhir != null)
            {
                stringPeriodeAkhir = Convert.ToDateTime(periodeAkhir).ToString("yyyy-MM-dd");
                TempData["PeriodeAkhir"] = Convert.ToDateTime(periodeAkhir);
                TempData["StringPeriodeAkhir"] = stringPeriodeAkhir;
            }

            if (dataMart != null)
            {
                TempData["DataMart"] = dataMart;
            }

            if (permintaanIDeb != null)
            {
                TempData["PermintaanIDeb"] = permintaanIDeb;
            }
            if (stringMemberTypes2 != null) {
                stringMemberTypes = stringMemberTypes2;
            }
            if (stringMembers2 != null)
            {
                stringMembers = stringMembers2;
            }
            db.Database.CommandTimeout = 420;
            //var query = db.getGroupedSLIKSummary(stringJenisAlert, stringLevelAlert, stringMemberTypes, stringMembers, stringPeriodeAwal, stringPeriodeAkhir, dataMart, permintaanIDeb, null, null, null, null).ToList();
            var query = db.getGroupedAlertSummary("UNI", stringJenisAlert, stringLevelAlert, stringMemberTypes, stringMembers, null, stringPeriodeAwal, stringPeriodeAkhir, dataMart, permintaanIDeb, null, null, null, null).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        public object GetChartData(DataSourceLoadOptions loadOptions, long id)
        {
            var login = this.User.Identity.Name;

            string stringLevelAlert = null;
            string stringPeriodeAwal = null;
            string stringPeriodeAkhir = null;
            string dataMart = null;
            string permintaanIDeb = null;

            string memberType = null;
            string member = null;
            string kodeAlert = null;

            if (TempData.Peek("LevelAlert") != null)
            {
                stringLevelAlert = string.Join(", ", (string[])TempData.Peek("LevelAlert"));
            }
            else
            {
                stringLevelAlert = "2, 3";
            }

            if (TempData.Peek("StringPeriodeAwal") != null)
            {
                stringPeriodeAwal = TempData.Peek("StringPeriodeAwal").ToString();
            }

            if (TempData.Peek("StringPeriodeAkhir") != null)
            {
                stringPeriodeAkhir = TempData.Peek("StringPeriodeAkhir").ToString();
            }

            if (TempData.Peek("DataMart") != null)
            {
                dataMart = TempData.Peek("DataMart").ToString();
            }

            if (TempData.Peek("PermintaanIDeb") != null)
            {
                permintaanIDeb = TempData.Peek("PermintaanIDeb").ToString();
            }

            //Detail Parameter
            if (TempData.Peek("DetailMemberType") != null)
            {
                memberType = TempData.Peek("DetailMemberType").ToString();
            }

            if (TempData.Peek("DetailMember") != null)
            {
                member = TempData.Peek("DetailMember").ToString();
            }

            if (TempData.Peek("DetailKodeAlert") != null)
            {
                kodeAlert = TempData.Peek("DetailKodeAlert").ToString();
            }

            /*check pengawas LJK*/
            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                var filter2 = GetFilteredMembers(login);

                if (!filter.Contains(memberType))
                {
                    throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk Jenis LJK ini");
                }

                if (!filter2.Contains(member))
                {
                    throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk LJK ini");
                }
            }
            db.Database.CommandTimeout = 420;
            //var query = db.getAlertSummary("SLIK", kodeAlert, stringLevelAlert, memberType, memberType + " - " + member, stringPeriodeAwal, stringPeriodeAkhir, dataMart, permintaanIDeb, null, null, null, null).ToList();
            var query = db.getAlertSummary("UNI", kodeAlert, stringLevelAlert, memberType, member, null, stringPeriodeAwal, stringPeriodeAkhir, dataMart, permintaanIDeb, null, null, null, null).ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

       // [HttpGet]
        public object GetGridRecordData(DataSourceLoadOptions loadOptions, long id)
        {
            try
            {
                var login = this.User.Identity.Name;

                var as1 = db.Alert_Summary.Find(id);

                /*check pengawas LJK*/
                if (IsPengawasLJK())
                {
                    var filter = GetFilteredMemberTypes(login);
                    var filter2 = GetFilteredMembers(login);

                    if (!filter.Contains(as1.MEMBER_TYPE_CODE))
                    {
                        throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk Jenis LJK ini");
                    }

                    if (!filter2.Contains(as1.MEMBER_CODE))
                    {
                        throw new InvalidOperationException("Pengawas LJK tidak mempunyai akses untuk LJK ini");
                    }
                }

                string q = db.GetSetting("QueryHiveEWS") + System.Environment.NewLine;
                //string q = "Select * from inquiry_enh_det ied" + System.Environment.NewLine;

                q = q + "WHERE ied.p_priode like '" + as1.PERIODE.ToString("yyyyMM") + "' AND ied.member_type_code='" + as1.MEMBER_TYPE_CODE + "' AND ied.member_code='" + as1.MEMBER_CODE + "' " + System.Environment.NewLine;

                if (as1.KODE_ALERT == "UP")
                {
                    q = q + "AND ied.user_id='" + as1.DIMENSI2 + "' ";
                }
                else if (as1.KODE_ALERT == "KC")
                {
                    if (as1.OFFICE_CODE != null) {
                        string kc1 = as1.OFFICE_CODE.Split("-")[1];
                        q = q + "AND ied.byrf_office_code='" + kc1 + "' ";
                    }
                }

                //if (as1.TIPE_PERIODE == "Harian")
                //{
                //    q = q + "AND ied.request_datetime like '" + as1.PERIODE.ToString("yyyyMMdd") + "%' ";
                //}
                //else if (as1.TIPE_PERIODE == "Bulanan")
                //{
                //    q = q + "AND ied.request_datetime like '" + as1.PERIODE.ToString("yyyyMM") + "%' ";
                //}
                //q = q + "AND ied.p_priode like '" + as1.PERIODE.ToString("yyyyMM") + "'";

                if (as1.DIMENSI1 == "Individu" || as1.DIMENSI1 == "NonIndividu")
                {

                    if (as1.DIMENSI1 == "Individu")
                    {
                        q = q + "AND (ied.debtor_type_flag='I' OR ied.debtor_type_flag='i') ";
                    }
                    else
                    {
                        q = q + "AND UPPER(ied.debtor_type_flag)<>'I' ";
                    }
                }
                if (as1.DIMENSI1 == "Batch" || as1.DIMENSI1 == "Interactive")
                {
                    if (as1.DIMENSI1 == "Batch")
                    {
                        q = q + "AND ied.is_batch_flag='Y' ";
                    }
                    else
                    {
                        q = q + "AND ied.is_batch_flag='T' ";
                    }
                }

                q = q + "LIMIT 100000 ";

                //var conn = new OdbcConnection
                //{
                //    ConnectionString = @"DRIVER={Hortonworks Hive ODBC Driver};                                        
                //                        Host=10.225.60.14;
                //                        Port=10500;
                //                        Schema=ojk;
                //                        HiveServerType=2;
                //                        KrbHostFQDN={bgrdco-bddvmst1.ojk.go.id};
                //                        KrbServiceName={hive};
                //                        AuthMech=1;"
                //};
                var conn = new OdbcConnection("DSN=" + db.GetSetting("HiveDSN"));
                
                try
                {

                    conn.Open();
                    //conn.ConnectionTimeout = 37;
                    //var adp = new OdbcDataAdapter(q, conn);

                    //var ds = new DataSet();
                    //adp.Fill(ds);
                    //var dt = ds.Tables[0];


                    OdbcCommand myCommand = new OdbcCommand(q, conn);
                    myCommand.CommandTimeout = 1000;
                    DataTable table = new DataTable();
                    table.Load(myCommand.ExecuteReader());
                    var ds = new DataSet();
                    ds.Tables.Add(table);
                    var dt = ds.Tables[0];
                    var query = (from row in dt.AsEnumerable()
                                 select new
                                 {
                                     inquiry_id = Convert.ToString(row["inquiry_id"]),
                                     user_id = Convert.ToString(row["user_id"]),
                                     user_login_id = Convert.ToString(row["user_login_id"]),
                                     user_name1 = Convert.ToString(row["user_name1"]),
                                     report_request_purpose_desc = Convert.ToString(row["report_request_purpose_desc"]),
                                     created_by = Convert.ToString(row["created_by"]),
                                     jenis_permintaan = Convert.ToString(row["jenis_permintaan"]),
                                     Jenis_Debitur = Convert.ToString(row["Jenis_Debitur"]),
                                     request_datetime = Convert.ToString(row["request_datetime"]),
                                     identity_number = Convert.ToString(row["identity_number"]),
                                     fullname = Convert.ToString(row["fullname"]),
                                     gender_code = Convert.ToString(row["gender_code"]),
                                     birth_date = Convert.ToString(row["birth_date"]),
                                     birth_place = Convert.ToString(row["birth_place"]),
                                     est_cert_date = Convert.ToString(row["est_cert_date"]),
                                     est_location = Convert.ToString(row["est_location"]),
                                     user_reference_code = Convert.ToString(row["user_reference_code"]),
                                     inquiry_status_flag = Convert.ToString(row["inquiry_status_flag"]),
                                     record_status_flag = Convert.ToString(row["record_status_flag"]),
                                     inquiry_report_number = Convert.ToString(row["inquiry_report_number"])
                                 }).ToList();

                    return DataSourceLoader.Load(query, loadOptions);
                }
                catch (Exception ex)
                {
                    //return null;
                    throw new InvalidOperationException(ex.Message);
                    // log.Info("Failed to connect to data source");
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region "RefGetter"
        public IActionResult GetUNIAlertTypes(DataSourceLoadOptions loadOptions)
        {
            var query = db.Alert_Master.Where(x => x.KODE_ALERT.StartsWith("UNI")).Select(x => new { x.KODE_ALERT, x.NAMA_ALERT }).ToList();

            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        public IActionResult GetMemberTypes(DataSourceLoadOptions loadOptions)
        {
            var login = this.User.Identity.Name;

            var query = db.master_ljk_type.Where(x => x.status_aktif == "Y" && x.status_delete == "T").Select(x => new { x.kode_jenis_ljk, Display = x.kode_jenis_ljk + " - " + x.deskripsi_jenis_ljk }).ToList();

            if (IsPengawasLJK())
            {
                var filter = GetFilteredMemberTypes(login);
                query = query.Where(x => filter.Contains(x.kode_jenis_ljk)).ToList();
            }

            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        public IActionResult GetMembers(DataSourceLoadOptions loadOptions, string memberTypes, bool reset)
        {
            var login = this.User.Identity.Name;

            if (reset)
            {
                loadOptions.Skip = 0;
                loadOptions.Take = 0;
            }

            string[] MemberTypes = string.IsNullOrEmpty(memberTypes) ? new string[] { } : memberTypes.Split(",");

            //var query = db.master_ljk.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, Display = x.kode_ljk + " - " + x.nama_ljk }).ToList();
            var query = db.vw_getMasterLJK.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, x.nama_ljk, x.CompositeKey, x.Display }).ToList();

            if (IsPengawasLJK())
            {
                var filter = GetFilteredMembers(login);
                //query = query.Where(x => filter.Contains(x.kode_jenis_ljk + " - " + x.kode_ljk)).ToList();
                query = query.Where(x => filter.Contains(x.kode_ljk)).ToList();
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        #endregion
    }
}
