using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Dashboard")]
    public class CollateralValueDifferenceController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public CollateralValueDifferenceController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Collateral Value Difference View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
            ViewBag.Export = db.CheckPermission("Collateral Value Difference Export", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("CollateralValueDifference_Akses_Page", "Akses Page Dashboard Collateral Value Difference", pageTitle);

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
        public IActionResult LogExportIndex()
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Collateral Value Difference Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_CollateralValueDifference", "Export Data", pageTitle);
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

                db.CheckPermission("Collateral Value Difference Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                db.InsertAuditTrail("ExportIndex_CollateralValueDifference", "Export Data", pageTitle);

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
                var fileName = "CollateralValueDifference_" + timeStamp + ".pdf";
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
            var fileName = "CollateralValueDifference_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        #region "GetGridData"
        //[HttpGet]
        public object GetGridData(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periode, string jenisDebitur, string jenisAgunan)
        {
            try
            {
                var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] JenisDebitur = JsonConvert.DeserializeObject<string[]>(jenisDebitur);
                string[] JenisAgunan = JsonConvert.DeserializeObject<string[]>(jenisAgunan);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                string q = "select  member_type_code, member_code,concat(substr(pperiode,0,4),'-', substr(pperiode,5,2) ,'-01')  as periode, pjenis_agunan, collateral_reg_no, status,desc_jenis_ljk_byreff, collateral_value_member, collateral_value_appr, collateral_value_tax,(collateral_value_member - collateral_value_appr) as selisih_member_appr,(collateral_value_member - collateral_value_tax) as selisih_member_tax from a01_agunandatamart  " + System.Environment.NewLine;
                q = q + "WHERE pperiode = '" + Convert.ToDateTime( periode).ToString("yyyyMM") + "' "  + System.Environment.NewLine;
                if (MemberTypes.Length > 0)
                {
                    string sMT = "";
                    foreach (var i in MemberTypes) {
                        if (sMT == "")
                        {
                            sMT = "'" + i + "'";
                        }
                        else {
                            sMT =sMT+ ",'" + i + "'";
                        }
                    }
                    q = q + "AND member_type_code IN ("+sMT+") " + System.Environment.NewLine;
                }
                else
                {
                    /*check pengawas LJK*/
                    if (IsPengawasLJK())
                    {
                        var filter = GetFilteredMemberTypes(login);

                        string sMT = "";
                        foreach (var i in filter)
                        {
                            if (sMT == "")
                            {
                                sMT = "'" + i + "'";
                            }
                            else
                            {
                                sMT = sMT + ",'" + i + "'";
                            }
                        }
                        q = q + "AND member_type_code IN (" + sMT + ") " + System.Environment.NewLine;
                    }
                }

                if (Members.Length > 0)
                {
                    string sMT = "";
                    foreach (var i in Members)
                    {
                        if (sMT == "")
                        {
                            sMT = "'" + i + "'";
                        }
                        else
                        {
                            sMT = sMT + ",'" + i + "'";
                        }
                    }
                    q = q + "AND member_code IN (" + sMT + ") " + System.Environment.NewLine;
                }
                else
                {
                    /*check pengawas LJK*/
                    if (IsPengawasLJK())
                    {
                        var filter2 = GetFilteredMembers(login);

                        string sMT = "";
                        foreach (var i in filter2)
                        {
                            if (sMT == "")
                            {
                                sMT = "'" + i + "'";
                            }
                            else
                            {
                                sMT = sMT + ",'" + i + "'";
                            }
                        }
                        q = q + "AND member_code IN (" + sMT + ") " + System.Environment.NewLine;
                    }
                }
                
                if (JenisDebitur.Length > 0)
                {
                    string sMT = "";
                    foreach (var i in JenisDebitur)
                    {
                        if (sMT == "")
                        {
                            sMT = "'" + i + "'";
                        }
                        else
                        {
                            sMT = sMT + ",'" + i + "'";
                        }
                    }
                    q = q + "AND status IN (" + sMT + ") " + System.Environment.NewLine;
                }
                if (JenisAgunan.Length > 0)
                {
                    string sMT = "";
                    foreach (var i in JenisAgunan)
                    {
                        if (sMT == "")
                        {
                            sMT = "'" + i + "'";
                        }
                        else
                        {
                            sMT = sMT + ",'" + i + "'";
                        }
                    }
                    q = q + "AND pjenis_agunan IN (" + sMT + ") " + System.Environment.NewLine;
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
                //var conn = new OdbcConnection("DSN=hive_secure_table");
                try
                {

                    conn.Open();
                    //conn.ConnectionTimeout = 37;
                    //var adp = new OdbcDataAdapter(q, conn);

                    //var ds = new DataSet();
                    //adp.Fill(ds);
                    //var dt = ds.Tables[0];


                    OdbcCommand myCommand = new OdbcCommand(q, conn);
                    myCommand.CommandTimeout = 1500;
                    DataTable table = new DataTable();
                    table.Load(myCommand.ExecuteReader());
                    var ds = new DataSet();
                    ds.Tables.Add(table);
                    var dt = ds.Tables[0];
                    var query = (from row in dt.AsEnumerable()
                                 select new
                                 {
                                     member_type_code = Convert.ToString(row["member_type_code"]),
                                     member_code = Convert.ToString(row["member_code"]),
                                     periode = Convert.ToString(row["periode"]),
                                     pjenis_agunan = Convert.ToString(row["pjenis_agunan"]),
                                     collateral_reg_no = Convert.ToString(row["collateral_reg_no"]),
                                     status = Convert.ToString(row["status"]),
                                     desc_jenis_ljk_byreff = Convert.ToString(row["desc_jenis_ljk_byreff"]),
                                     collateral_value_member = Convert.ToString(row["collateral_value_member"]),
                                     collateral_value_appr = Convert.ToString(row["collateral_value_appr"]),
                                     collateral_value_tax = Convert.ToString(row["collateral_value_tax"]),
                                     selisih_member_appr = Convert.ToString(row["selisih_member_appr"]),
                                     selisih_member_tax = Convert.ToString(row["selisih_member_tax"])
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

        #endregion

        #region "RefGetter"
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
                query = query.Where(x => filter.Contains(x.kode_ljk)).ToList();
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        public IActionResult GetCollateralTypes(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.Master_Agunan
                        select new { q.jenis_agunan, q.nama_agunan };
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }

        #endregion
    }
}
