using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Cells;
using BDA.DataModel;
using BDA.Helper.FW;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("BDA2")]
    public class DAController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public DAController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        [HttpPost]
        public IActionResult CekNoData(string periodes,string reportId)
        {
            try
            {
                if (periodes != null)
                {
                    string[] periodes1 = JsonConvert.DeserializeObject<string[]>(periodes);
                    string resp1="No Data";
                    if (periodes1.Length > 0)
                    {
                        var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                        string stringPeriode = null;
                        List<DateTime> lp = new List<DateTime>();
                        foreach (var i in periodes1)
                        {
                            lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                        }
                        if (periodes1.Length > 0)
                        {
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
                                stringPeriode = string.Join(", ", periodes1);
                            }
                        }
                        resp1 = Helper.WSQueryStore.CekDataDA(db, stringPeriode, reportId, cekHive);
                    }
                    //return Json(resp1);
                    return Json(new { result = resp1 });
                }
                else {
                    return Json(new { result = "No Data" });
                }
                
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        [HttpPost]
        public IActionResult Antrian(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                //db.CheckPermission("DA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringVariable1 = null;
                string stringVariable2 = null;
                string stringPeriode = null;
                if (reportId == "debitur_anomali_format_ktp")
                {
                    newq.rgq_nama = "DA01 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    //if (TempData.Peek("jt") != null)
                    //{
                    //    stringVariable1 = TempData.Peek("jt").ToString();
                    //}
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliFormatKTPQuery(db, stringMemberTypes, stringMembers, stringPeriode, isHive);
                    db.CheckPermission("DA01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "debitur_anomali_gender")
                {
                    newq.rgq_nama = "DA02 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    //if (TempData.Peek("jt") != null)
                    //{
                    //    stringVariable1 = TempData.Peek("jt").ToString();
                    //}
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliGenderQuery(db, stringMemberTypes, stringMembers, stringPeriode, isHive);
                    db.CheckPermission("DA02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "debitur_anomali_duplikasi_nama_debitur")
                {
                    newq.rgq_nama = "DA03 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    //if (TempData.Peek("jt") != null)
                    //{
                    //    stringVariable1 = TempData.Peek("jt").ToString();
                    //}
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliDuplikasiNamaQuery(db, stringMemberTypes, stringMembers, stringPeriode, isHive);
                    db.CheckPermission("DA03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "agunan_id_anomali")
                {
                    newq.rgq_nama = "DA04 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("ja") != null)
                    {
                        stringVariable1 = TempData.Peek("ja").ToString();
                    }
                    if (TempData.Peek("ss") != null)
                    {
                        stringVariable2 = TempData.Peek("ss").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliIDAgunanQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringVariable2, stringPeriode, isHive);
                    db.CheckPermission("DA04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "agunan_dokumen_anomali")
                {
                    newq.rgq_nama = "DA05 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("ja") != null)
                    {
                        stringVariable1 = TempData.Peek("ja").ToString();
                    }
                    if (TempData.Peek("od") != null)
                    {
                        stringVariable2 = TempData.Peek("od").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliDokumenAgunanQuery(db, stringMemberTypes, stringMembers, stringVariable1, stringVariable2, stringPeriode, isHive);
                    db.CheckPermission("DA05 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                //var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ma_rekening_baru_non_restrukturisasi_npl");
                //ExportExcel(reportId, memberTypes, members, jenisPinjamans, npls, periode, cekHive);
                newq.rgq_date = DateTime.Now;
                newq.rgq_priority = 1;
                newq.rgq_requestor = User.Identity.Name;
                newq.rgq_urut = 0;
                newq.rgq_status = "Pending";
                db.SetStsrcFields(newq);
                db.RptGrid_Queue.Add(newq);
                db.SaveChanges();
                db.InsertAuditTrail("ExportIndex_DA_" + reportId, "Export Data", pageTitle);
                var resp = "Sukses mengantrikan";
                return Json(resp);
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }
        public IActionResult Index(string id)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            //TempData.Clear();
            ViewBag.id = id;
            ViewBag.Export = false; // TODO ubah permission disini
            List<string> listP = new List<string>();
            //listP.Add(string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)));

            //if (periode != null)
            //{
            //    TempData["periode"] = periode.Split(",");
            //}
            //else
            //{
            //    TempData["periode"] = string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)).Split(",");
            //}
            //if (memberTypes != null) {
            //    TempData["mt"] = memberTypes.Split(",");
            //}
            //if (members != null)
            //{
            //    TempData["m"] = members.Split(",");
            //}
            //if (jenisTA != null)
            //{
            //    TempData["jt"] = jenisTA;
            //}
            TempData["periode"] = string.Format("{0:yyyy-MM-01}", DateTime.Now.AddMonths(-1)).Split(",");
            var isHive = Helper.WSQueryStore.IsPeriodInHive(db, id);
            ViewBag.Hive = isHive;
            if (id == "debitur_anomali_format_ktp")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA01", pageTitle);
                db.CheckPermission("DA01 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA01 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliFormatKTP");
            }
            else if (id == "debitur_anomali_gender")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA02", pageTitle);
                db.CheckPermission("DA02 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA02 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliGender");
            }
            else if (id == "debitur_anomali_duplikasi_nama_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA03", pageTitle);
                db.CheckPermission("DA03 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA03 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliDuplikasiNama");
            }
            else if (id == "agunan_id_anomali")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA04", pageTitle);
                db.CheckPermission("DA04 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA04 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliIDAgunan");
            }
            else if (id == "agunan_dokumen_anomali")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA05", pageTitle);
                db.CheckPermission("DA05 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA05 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliDokumenAgunan");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public IActionResult LogExportIndex(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                //db.CheckPermission("DA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "debitur_anomali_format_ktp")
                {
                    db.CheckPermission("DA01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "debitur_anomali_gender")
                {
                    db.CheckPermission("DA02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "debitur_anomali_duplikasi_nama_debitur")
                {
                    db.CheckPermission("DA03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "agunan_id_anomali")
                {
                    db.CheckPermission("DA04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    db.CheckPermission("DA05 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                //var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ma_rekening_baru_non_restrukturisasi_npl");
                //ExportExcel(reportId, memberTypes, members, jenisPinjamans, npls, periode, cekHive);
                db.InsertAuditTrail("ExportIndex_DA_" + reportId, "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        //private DataTable GetData(String queryString)
        //{
        //    DataTable dt = new DataTable();
        //    using (var conn = new SqlConnection(db.appSettings.DataConnString))
        //    {
        //        using (var cmd = new SqlCommand(queryString, conn))
        //        {
        //            conn.Open();
        //            cmd.CommandText = queryString;
        //            var adap = new SqlDataAdapter(cmd);
        //            adap.Fill(dt);
        //        }
        //    }
        //    return dt;


        //    //DataTable dt = new DataTable();
        //    //var conn = new SqlConnection(db.appSettings.DataConnString);
        //    //SqlDataAdapter sda = new SqlDataAdapter();
        //    //cmd.CommandType = CommandType.Text;
        //    //cmd.Connection = conn.Connection;
        //    //try
        //    //{
        //    //    sda.SelectCommand = cmd;
        //    //    sda.Fill(dt);
        //    //    return dt;
        //    //}
        //    //catch
        //    //{
        //    //    return null;
        //    //}
        //    //finally
        //    //{
        //    //    con.connection.Close();
        //    //    sda.Dispose();
        //    //    con.connection.Dispose();
        //    //}
        //}

        //public IActionResult ExportExcel(string reportId, string memberTypes, string members, string jenisPinjamans, string npls, string periode, bool isHive)
        //{
        //    var strQuery = "";
        //    if (reportId == "ma_anomali_plafon_by_pekerjaan_debitur")
        //    {
        //        strQuery = "SELECT * FROM dbo.ma_anomali_plafon_by_pekerjaan_debitur";
        //    }
        //    else if (reportId == "ma_aml_cft_analysis")
        //    {
        //        strQuery = "SELECT * FROM dbo.ma_aml_cft_analysis";
        //    }
        //    else if (reportId == "ma_kolektibilitas_kesalahan_ljk")
        //    {
        //        strQuery = "SELECT * FROM dbo.ma_kolektibilitas_kesalahan_ljk";
        //    }
        //    else if (reportId == "ma_outstanding_macet_no_agunan")
        //    {
        //        strQuery = "SELECT * FROM dbo.ma_outstanding_macet_no_agunan";
        //    }
        //    else if (reportId == "ma_analisis_pencurian_identitas")
        //    {
        //        strQuery = "SELECT * FROM dbo.ma_analisis_pencurian_identitas";
        //    }
        //    else
        //    {
        //        var whereQuery = "1 = 1";

        //        if (memberTypes != null)
        //        {
        //            memberTypes = "'" + memberTypes.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
        //            whereQuery = whereQuery += " AND dm_jenis_ljk in (" + memberTypes + ")";
        //        }
        //        if (members != null)
        //        {
        //            if (isHive == true)
        //            {
        //                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "%'"; //cegah sql inject dikit
        //                whereQuery = whereQuery += " AND dm_kode_ljk LIKE (" + members + ")";
        //            }
        //            else
        //            {
        //                members = "'" + members.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
        //                whereQuery = whereQuery += " AND dm_kode_ljk in (" + members + ")";
        //            }
        //        }

        //        if (jenisPinjamans != null)
        //        {
        //            jenisPinjamans = "'" + jenisPinjamans.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
        //            whereQuery = whereQuery += " AND dm_kode_jenis_pinjaman in (" + jenisPinjamans + ")";
        //        }
        //        if (npls != null)
        //        {
        //            npls = "'" + npls.Replace("'", "").Replace("-", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
        //            whereQuery = whereQuery += " AND dm_npl in (" + npls + ")";
        //        }
        //        if (periode != null)
        //        {
        //            periode = "'" + periode.Replace("'", "").Replace(",", "','").Replace("' ", "'") + "'"; //cegah sql inject dikit
        //            whereQuery = whereQuery += " AND dm_periode in (" + periode + ")";
        //        }

        //        strQuery = "SELECT TOP 100000 * FROM dbo.ma_rekening_baru_non_restrukturisasi_npl";
        //        //pake filter masih gagal karena yang valuenya null tetap masuk ke query
        //        //strQuery = @"SELECT TOP 100000 * FROM dbo.ma_rekening_baru_non_restrukturisasi_npl WHERE " + whereQuery + @"";
        //    }

        //    //SqlCommand cmd = new SqlCommand(strQuery);
        //    DataTable dt = GetData(strQuery);

        //    if (dt != null)
        //    {
        //        Workbook wb = new Workbook();

        //        var sheet = wb.Worksheets[0];
        //        var cell = sheet.Cells;

        //        //set stlyenya
        //        Aspose.Cells.Style style1 = new Aspose.Cells.Style();
        //        style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //        style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //        style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //        style1.Borders.SetColor(System.Drawing.Color.Gray);

        //        Aspose.Cells.Style style2 = new Aspose.Cells.Style();
        //        style2.HorizontalAlignment = TextAlignmentType.Left;
        //        style2.Pattern = BackgroundType.Solid;
        //        style2.ForegroundColor = System.Drawing.Color.FromArgb(110, 158, 202);
        //        style2.Font.Color = System.Drawing.Color.White;
        //        style2.Font.IsBold = true;
        //        style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //        style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //        style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //        style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //        style2.Borders.SetColor(System.Drawing.Color.White);

        //        Aspose.Cells.Style style3 = new Aspose.Cells.Style();
        //        style3.HorizontalAlignment = TextAlignmentType.Center;
        //        style3.Pattern = BackgroundType.Solid;
        //        style3.ForegroundColor = System.Drawing.Color.FromArgb(110, 158, 202);
        //        style3.Font.Color = System.Drawing.Color.White;
        //        style3.Font.IsBold = true;
        //        style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //        style3.Borders.SetColor(System.Drawing.Color.White);

        //        sheet.FreezePanes(3, 2, 3, 2);

        //        ////merge kolom2nya
        //        sheet.Cells.Merge(1, 0, 2, 1);
        //        sheet.Cells.Merge(1, 1, 2, 1);
        //        sheet.Cells.Merge(1, 2, 2, 1);
        //        sheet.Cells.Merge(1, 3, 1, 5);
        //        sheet.Cells.Merge(2, 3, 1, 2);
        //        sheet.Cells.Merge(1, 8, 1, 8);
        //        sheet.Cells.Merge(1, 16, 2, 1);
        //        sheet.Cells.Merge(1, 17, 2, 1);
        //        sheet.Cells.Merge(1, 18, 2, 1);
        //        sheet.Cells.Merge(1, 19, 2, 1);
        //        sheet.Cells.Merge(1, 20, 2, 1);
        //        sheet.Cells.Merge(1, 21, 2, 1);
        //        sheet.Cells.Merge(1, 22, 2, 1);
        //        sheet.Cells.Merge(1, 23, 2, 1);
        //        sheet.Cells.Merge(1, 24, 2, 1);
        //        sheet.Cells.Merge(1, 25, 1, 89);


        //        int rows = 3;

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            cell[rows, 0].Value = row["dm_periode"].ToString();
        //            cell[rows, 1].Value = row["dm_jenis_ljk"];
        //            cell[rows, 2].Value = row["dm_kode_ljk"];
        //            cell[rows, 3].Value = row["dm_kode_jenis_pinjaman"];
        //            cell[rows, 4].Value = row["dm_cif"];
        //            cell[rows, 5].Value = row["dm_no_rekening"];
        //            cell[rows, 6].Value = row["dm_nama_debitur"];
        //            cell[rows, 7].Value = row["dm_outstanding"];
        //            cell[rows, 8].Value = row["dm_baki_debet"];
        //            cell[rows, 9].Value = row["dm_plafon"];
        //            cell[rows, 10].Value = row["dm_jumlah_hari_tunggakan"];
        //            cell[rows, 11].Value = row["dm_npl"];
        //            cell[rows, 12].Value = row["dm_tanggal_mulai"];
        //            cell[rows, 13].Value = row["dm_tanggal_awal_pinjaman"];
        //            cell[rows, 14].Value = row["dm_tanggal_jatuh_tempo"];
        //            cell[rows, 15].Value = row["dm_selisih_awalvsperiode"];

        //            //set style detailnya
        //            for (int i = 0; i < 15; i++)
        //            {
        //                cell[rows, i].SetStyle(style1);
        //            }

        //            rows += 1;
        //        }

        //        var directory = _env.WebRootPath;

        //        var timeStamp = DateTime.Now.ToString();

        //        timeStamp = timeStamp.Replace('/', '-').Replace(" ", "_").Replace(":", "-");
        //        TempData["timeStamp"] = timeStamp;
        //        var fileName = "DA_" + reportId + "_" + timeStamp + ".xlsx";
        //        wb.Save(Path.Combine(directory, fileName), SaveFormat.Xlsx);
        //        return new EmptyResult();

        //    }
        //    return null;
        //    //    Helper.WSQueryStore.GetDA_AnomaliPekerjaanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringJenisPenggunaans, stringPekerjaans, stringPeriode, cekHive, isChart);
        //}

        public IActionResult ExportPDF(string reportId, IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                //db.CheckPermission("DA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "debitur_anomali_format_ktp")
                {
                    db.CheckPermission("DA01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "debitur_anomali_gender")
                {
                    db.CheckPermission("DA02 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "debitur_anomali_duplikasi_nama_debitur")
                {
                    db.CheckPermission("DA03 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "agunan_id_anomali")
                {
                    db.CheckPermission("DA04 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    db.CheckPermission("DA05 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                db.InsertAuditTrail("ExportIndex_DA" + reportId, "Export Data", pageTitle);
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

                    //apply format number
                    Style textStyle = workbook.CreateStyle();
                    textStyle.Number = 3;
                    StyleFlag textFlag = new StyleFlag();
                    textFlag.NumberFormat = true;

                    worksheet.Cells.Columns[1].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[2].ApplyStyle(textStyle, textFlag);
                    worksheet.Cells.Columns[3].ApplyStyle(textStyle, textFlag);

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
                var fileName = "DA_" + reportId + "_" + timeStamp + ".pdf";
                workbook.Save(Path.Combine(directory, fileName), SaveFormat.Pdf);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public FileResult File(string reportId)
        {
            var directory = _env.WebRootPath;
            var timeStamp = TempData.Peek("timeStamp").ToString();
            var fileName = "DA_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }


        #region "GetGridData"
        public object GetGridDataDA_AnomaliFormatKTP(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                //string[] JenisTAs = JsonConvert.DeserializeObject<string[]>(jenisTAs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                //string stringJenisTA = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "debitur_anomali_format_ktp");
                TempData["memberTypeValue"] = null;
                TempData["memberValue"] = null;
                TempData["periodeValue"] = null;
                //cekHive = true;
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
                    TempData["memberTypeValue"] = memberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
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
                    TempData["memberValue"] = members;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }

                //if (jenisTA != null)
                //{
                //    TempData["jenisTAValue"] = jenisTA;
                //    stringJenisTA = jenisTA;
                //    TempData["jt"] = stringJenisTA;
                //}

                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
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

                var result = Helper.WSQueryStore.GetDA_AnomaliFormatKTPQuery(db, loadOptions, stringMemberTypes, stringMembers, stringPeriode, cekHive, isChart, isPieChart);

                return JsonConvert.SerializeObject(result);
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliGender(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                //string[] JenisTAs = JsonConvert.DeserializeObject<string[]>(jenisTAs);\
                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                //string stringJenisTA = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "debitur_anomali_gender");
                TempData["memberTypeValue"] = null;
                TempData["memberValue"] = null;
                TempData["periodeValue"] = null;
                //cekHive = true;
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
                    TempData["memberTypeValue"] = memberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
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
                    TempData["memberValue"] = members;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }
                //if (jenisTA != null)
                //{
                //    TempData["jenisTAValue"] = jenisTA;
                //    stringJenisTA = jenisTA;
                //    TempData["jt"] = stringJenisTA;
                //}
                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
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
                var result = Helper.WSQueryStore.GetDA_AnomaliGenderQuery(db, loadOptions, stringMemberTypes, stringMembers, stringPeriode, cekHive, isChart, isPieChart);

                return JsonConvert.SerializeObject(result);
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliDuplikasiNama(DataSourceLoadOptions loadOptions, string memberTypes, string members, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                //string[] JenisTAs = JsonConvert.DeserializeObject<string[]>(jenisTAs);
                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                //string stringJenisTA = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "debitur_anomali_duplikasi_nama_debitur");
                TempData["memberTypeValue"] = null;
                TempData["memberValue"] = null;
                TempData["periodeValue"] = null;
                //cekHive = true;
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
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
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
                    TempData["memberValue"] = members;
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }
                //if (jenisTA != null)
                //{
                //    TempData["jenisTAValue"] = jenisTA;
                //    stringJenisTA = jenisTA;
                //    TempData["jt"] = stringJenisTA;
                //}
                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
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
                var result = Helper.WSQueryStore.GetDA_AnomaliDuplikasiNamaQuery(db, loadOptions, stringMemberTypes, stringMembers, stringPeriode, cekHive, isChart, isPieChart);

                return JsonConvert.SerializeObject(result);
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliIDAgunan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string jenisAgunans, string similarityScores, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] JenisAgunans = JsonConvert.DeserializeObject<string[]>(jenisAgunans);
                string[] SimilarityScores = JsonConvert.DeserializeObject<string[]>(similarityScores);
                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringJenisAgunans = null;
                string stringSimilarityScores = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "agunan_id_anomali");
                //cekHive = true;

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
                    TempData["memberTypeValue"] = memberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
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
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }

                if (JenisAgunans.Length > 0)
                {
                    stringJenisAgunans = string.Join(", ", JenisAgunans);
                    TempData["ja"] = stringJenisAgunans;
                }
                if (SimilarityScores.Length > 0)
                {
                    stringSimilarityScores = string.Join(", ", SimilarityScores);
                    TempData["ss"] = stringSimilarityScores;
                }
                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
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

                var result = Helper.WSQueryStore.GetDA_AnomaliIDAgunanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringJenisAgunans, stringSimilarityScores, stringPeriode, cekHive, isChart, isPieChart);

                return JsonConvert.SerializeObject(result);
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliDokumenAgunan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string jenisAgunans, string ownershipDocuments, string periode, bool isChart = false, bool isPieChart = false)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] JenisAgunans = JsonConvert.DeserializeObject<string[]>(jenisAgunans);
                string[] OwnershipDocuments = JsonConvert.DeserializeObject<string[]>(ownershipDocuments);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringJenisAgunans = null;
                string stringOwnershipDocuments = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "agunan_dokumen_anomali");
                //cekHive = true;

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
                    TempData["memberTypeValue"] = memberTypes;
                    var listOfJenis = db.master_ljk_type.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMemberTypes = "";
                    foreach (var mem in MemberTypes)
                    {
                        var find = listOfJenis.Where(x => x.kode_jenis_ljk == mem).FirstOrDefault();
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
                    stringMembers = string.Join(", ", Members);
                    TempData["m"] = stringMembers;
                }

                if (JenisAgunans.Length > 0)
                {
                    stringJenisAgunans = string.Join(", ", JenisAgunans);
                    TempData["ja"] = stringJenisAgunans;
                }
                if (OwnershipDocuments.Length > 0)
                {
                    stringOwnershipDocuments = string.Join(", ", OwnershipDocuments);
                    TempData["od"] = stringOwnershipDocuments;
                }
                if (periodes.Length > 0)
                {
                    TempData["periodeValue"] = string.Format("{0:yyyy-MM-01}", periode);
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

                var result = Helper.WSQueryStore.GetDA_AnomaliDokumenAgunanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringJenisAgunans, stringOwnershipDocuments, stringPeriode, cekHive, isChart, isPieChart);

                return JsonConvert.SerializeObject(result);
            }
            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        //public object GetGridDataDA_API(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kategoriAsesmens, string similarityScores, string similarityResults, string periode, bool isChart = false)
        //{
        //    var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

        //    //string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
        //    //string[] Members = JsonConvert.DeserializeObject<string[]>(members);
        //    string[] KategoriAsesmens = JsonConvert.DeserializeObject<string[]>(kategoriAsesmens);
        //    string[] SimilarityScores = JsonConvert.DeserializeObject<string[]>(similarityScores);
        //    string[] SimilarityResults = JsonConvert.DeserializeObject<string[]>(similarityResults);
        //    //string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);
        //    //Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
        //    if (members != null)
        //    {
        //        members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
        //    }

        //    string stringMemberTypes = null;
        //    string stringMembers = null;
        //    string stringKategoriAsesmens = null;
        //    string stringSimilarityScores = null;
        //    string stringSimilarityResults = null;
        //    string stringPeriode = null;

        //    var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "ma_analisis_pencurian_identitas");
        //    //cekHive = true;
        //    /*check pengawas LJK*/
        //    if (RefController.IsPengawasLJK(db))
        //    {
        //        var filter = RefController.GetFilteredMemberTypes(db, login);
        //        var filter2 = RefController.GetFilteredMembers(db, login);

        //        if (memberTypes != null)
        //        {
        //            stringMemberTypes = string.Join(", ", filter);
        //        }

        //        if (members != null)
        //        {
        //            stringMembers = string.Join(", ", filter2);
        //        }
        //    }

        //    if (memberTypes != null)
        //    {
        //        var listOfJenis = db.master_ljk_type.ToList();
        //        // nih gara2 si data processing kaga pake kode di output nya -_-;
        //        stringMemberTypes = "";
        //        var find = listOfJenis.Where(x => x.kode_jenis_ljk == memberTypes).FirstOrDefault();
        //        if (find != null)
        //        {
        //            if (stringMemberTypes != "") stringMemberTypes += ", ";
        //            stringMemberTypes += find.deskripsi_jenis_ljk;
        //        }
        //        TempData["mt"] = stringMemberTypes;
        //    }

        //    if (members != null)
        //    {
        //        stringMembers = members;
        //        TempData["m"] = stringMembers;
        //    }

        //    if (KategoriAsesmens.Length > 0)
        //    {
        //        stringKategoriAsesmens = string.Join(", ", KategoriAsesmens);
        //        TempData["ka"] = stringKategoriAsesmens;
        //    }

        //    if (SimilarityScores.Length > 0)
        //    {
        //        stringSimilarityScores = string.Join(", ", SimilarityScores);
        //        TempData["ss"] = stringSimilarityScores;
        //    }

        //    if (SimilarityResults.Length > 0)
        //    {
        //        stringSimilarityResults = string.Join(", ", SimilarityResults);
        //        TempData["sr"] = stringSimilarityResults;
        //    }

        //    if (periode != null)
        //    {
        //        DateTime period1 = Convert.ToDateTime(periode);
        //        if (cekHive == true)
        //        {
        //            stringPeriode = string.Format("{0:yyyyMM}", period1);
        //        }
        //        else
        //        {
        //            stringPeriode = string.Join(", ", string.Format("{0:yyyy-MM-dd}", period1));
        //        }
        //        TempData["p"] = stringPeriode;
        //    }


        //    var result = Helper.WSQueryStore.GetDA_APIQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKategoriAsesmens, stringSimilarityScores, stringSimilarityResults, stringPeriode, cekHive, isChart);

        //    return JsonConvert.SerializeObject(result);

        //}

        #endregion

    }
}
