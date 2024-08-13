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

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class PembiayaanVSJaminanSahamController
        : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public PembiayaanVSJaminanSahamController(DataEntities db, IWebHostEnvironment env)
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
            ViewBag.Hive = false;

            db.CheckPermission("Summary Cluster MKBD View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Akses Page Segmentation Summary Cluster MKBD", pageTitle); //simpan kedalam audit trail

            return View();
        }
        public object GetGridData(DataSourceLoadOptions loadOptions, string periodeAwal, string namaPE, string status)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            TempData.Clear(); //membersihkan data filtering
            string[] NamaPE = JsonConvert.DeserializeObject<string[]>(namaPE);

            string stringPeriodeAwal = null;
            string stringPE = null;
            string stringStatus = null;
            string reportId = "pe_segmentation_sum_cluster_mkbd"; //definisikan dengan table yg sudah disesuaikan pada table BDA2_Table

            var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId); //pengecekan apakah dipanggil dari hive/sql

            if (periodeAwal != null)
            {
                stringPeriodeAwal = Convert.ToDateTime(periodeAwal).ToString("yyyy-MM-dd");
                TempData["pawal"] = stringPeriodeAwal;
            }

            db.Database.CommandTimeout = 420;
            if (periodeAwal.Length > 0) //jika ada parameter nya
            {
                var result = Helper.WSQueryStore.GetBDAPMSegmentationSummaryClusterMKBDQuery(db, loadOptions, reportId, stringPeriodeAwal, stringPE, stringStatus, cekHive);
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

            using (SqlConnection conn = new SqlConnection(strSQL))
            {
                conn.Open();
                string strQuery = "Select [SecurityCompanySK],[SecurityCompanyCode],[SecurityCompanyName] from PM_dimSecurityCompanies where CurrentStatus='A' order by SecurityCompanyName asc ";
                SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string namakode = dt.Rows[i]["SecurityCompanyCode"].ToString() + " - " + dt.Rows[i]["SecurityCompanyName"].ToString();
                        list.Add(new NamaPE() { value = dt.Rows[i]["SecurityCompanySK"].ToString(), text = namakode });
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
        public IActionResult ExportPDF(IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Summary Cluster MKBD Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("SegmentationSummaryClusterMKBD_Akses_Page", "Export Data", pageTitle);

                var directory = _env.WebRootPath;
                var timeStamp = DateTime.Now.ToString();
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
                var fileName = "SegmentationSummaryClusterMKBD_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public object GetGridData(DataSourceLoadOptions loadOptions, string reportId, string memberTypes, string members, string kantorCabangs, string periode, bool chk100)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            TempData["memberTypeValue"] = null;
            TempData["memberValue"] = null;
            TempData["periodeValue"] = null;
            TempData["kcValue"] = null;
            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members1 = JsonConvert.DeserializeObject<string[]>(members);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = null;
                string[] KantorCabangs1 = null;
                if (kantorCabangs != null)
                {
                    KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                    KantorCabangs1 = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                    KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();
                }
                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }
                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                var timeNow = DateTime.Now;
                var timeAftter = DateTime.Now;

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                /*check pengawas LJK*/
                if (RefController.IsPengawasLJK(db))
                {
                    var filter = RefController.GetFilteredMemberTypes(db, login);
                    var filter2 = RefController.GetFilteredMembers(db, login);

                    if (MemberTypes.Length == 0)
                    {
                        stringMemberTypes = string.Join(", ", filter);
                    }

                    if (Members.Length == 0)
                    {
                        stringMembers = string.Join(", ", filter2);
                    }
                }

                if (MemberTypes.Length > 0)
                {
                    TempData["memberTypeValue"] = MemberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var m in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == m).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMemberTypes != "") stringMemberTypes += ", ";
                            stringMemberTypes += find.deskripsi_jenis_ljk;
                        }
                        TempData["mt"] = stringMemberTypes;
                    }

                }

                if (Members.Length > 0)
                {
                    TempData["memberValue"] = Members1;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }


                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = periodes;
                    if (cekHive == true)
                    {
                        foreach (var i in lp)
                        {
                            if (stringPeriode == null)
                            {
                                stringPeriode = string.Format("{0:yyyyMM}", i);
                            }
                            else
                            {
                                stringPeriode = stringPeriode + "," + string.Format("{0:yyyyMM}", i);
                            }

                        }
                    }
                    else
                    {
                        stringPeriode = string.Join(", ", periodes);
                    }
                    TempData["p"] = stringPeriode;
                }
                if (KantorCabangs != null)
                {
                    if (KantorCabangs.Length > 0)
                    {
                        TempData["kcValue"] = KantorCabangs1;
                        stringKantorCabangs = string.Join(", ", KantorCabangs);
                        TempData["kc"] = stringKantorCabangs;
                    }
                }
                var timeResultBefore = DateTime.Now;
                var result = Helper.WSQueryStore.GetOsida2023Query(db, loadOptions, reportId, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, chk100, cekHive);
                var timeResultAfter = DateTime.Now;
                timeAftter = DateTime.Now;
                var proc = Process.GetCurrentProcess();
                var mem = proc.WorkingSet64 / 1024.0;
                TempData["PM"] = mem;
                TempData["SG"] = (timeAftter - timeNow).TotalSeconds;
                TempData["SD"] = (timeResultAfter - timeResultBefore).TotalSeconds;
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);
        }

        //-----------------------------detail-----------------------------------//
        public IActionResult Detail(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Detil Cluster MKBD";

            db.CheckPermission("Detil Cluster MKBD View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Detil Cluster MKBD Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("AksesPageDetilCluster_Akses_Page", "Akses Page Detil Cluster MKBD", pageTitle);

            //if (id == null) return BadRequest(); //cek id itu menngarah ke mana

            if (id == null) {
                id = 1;
            }

            var obj = (dynamic)null;
            //var obj = db.BDA_F01_MaxMinOverdue.Find(id);
            //if (obj == null) return NotFound();

            return View(obj);
        }
        //-----------------------------detail-----------------------------------//

        //-----------------------------Rincian Portofolio-----------------------------------//
        public IActionResult RincianPortofolio(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Rincian Portofolio";

            db.CheckPermission("Rincian Portofolio View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Rincian Portofolio Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("RincianPortofolio_Akses_Page", "Akses Page Rincian Portofolio", pageTitle);

            //if (id == null) return BadRequest();

            if (id == null)
            {
                id = 1;
            }

            var obj = (dynamic)null;
            //var obj = db.BDA_F01_MaxMinOverdue.Find(id);
            //if (obj == null) return NotFound();

            return View(obj);
        }
        //-----------------------------Rincian Portofolio-----------------------------------//

        //-----------------------------Reksadana-----------------------------------//
        public IActionResult Reksadana(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Reksadana";

            db.CheckPermission("Reksadana View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Reksadana Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("Reksadana_Akses_Page", "Akses Page Reksadana", pageTitle);

            //if (id == null) return BadRequest();

            if (id == null)
            {
                id = 1;
            }

            var obj = (dynamic)null;
            //var obj = db.BDA_F01_MaxMinOverdue.Find(id);
            //if (obj == null) return NotFound();

            return View(obj);
        }
        //-----------------------------Reksadana-----------------------------------//

        //-----------------------------JaminanMargin-----------------------------------//
        public IActionResult JaminanMargin(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Jaminan Margin";

            db.CheckPermission("Jaminan Margin View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Jaminan Margin Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("Jaminan_Margin_Akses_Page", "Akses Page Jaminan Margin", pageTitle);

            //if (id == null) return BadRequest();

            if (id == null)
            {
                id = 1;
            }

            var obj = (dynamic)null;
            //var obj = db.BDA_F01_MaxMinOverdue.Find(id);
            //if (obj == null) return NotFound();

            return View(obj);
        }
        //-----------------------------JaminanMargin-----------------------------------//


        //-----------------------------ReverseRepo-----------------------------------//
        public IActionResult ReverseRepo(long? id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : "Jaminan Margin";

            db.CheckPermission("Reverse Repo View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Reverse Repo Export", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("Reverse_Repo_Akses_Page", "Akses Page Reverse Repo", pageTitle);

            //if (id == null) return BadRequest();

            if (id == null)
            {
                id = 1;
            }

            var obj = (dynamic)null;
            //var obj = db.BDA_F01_MaxMinOverdue.Find(id);
            //if (obj == null) return NotFound();

            return View(obj);
        }
        //-----------------------------ReverseRepo-----------------------------------//
    }
}
