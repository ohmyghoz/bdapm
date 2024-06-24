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
using Range = Aspose.Cells.Range;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("BDA3")]
    public class DA2023Controller : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public DA2023Controller(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        //[HttpPost]
        //public IActionResult CekNoData(string periodes, string reportId)
        //{
        //    try
        //    {
        //        if (periodes != null)
        //        {
        //            string[] periodes1 = JsonConvert.DeserializeObject<string[]>(periodes);
        //            string resp1 = "No Data";
        //            if (periodes1.Length > 0)
        //            {
        //                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
        //                string stringPeriode = null;
        //                List<DateTime> lp = new List<DateTime>();
        //                foreach (var i in periodes1)
        //                {
        //                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
        //                }
        //                if (periodes1.Length > 0)
        //                {
        //                    if (cekHive == true)
        //                    {
        //                        foreach (var i in lp)
        //                        {
        //                            if (stringPeriode == null)
        //                            {
        //                                stringPeriode = string.Format("{0:yyyyMM}", i);
        //                            }
        //                            else
        //                            {
        //                                stringPeriode = stringPeriode + "," + string.Format("{0:yyyyMM}", i);
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        stringPeriode = string.Join(", ", periodes1);
        //                    }
        //                }
        //                resp1 = Helper.WSQueryStore.CekDataDA(db, stringPeriode, reportId, cekHive);
        //            }
        //            //return Json(resp1);
        //            return Json(new { result = resp1 });
        //        }
        //        else
        //        {
        //            return Json(new { result = "No Data" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { result = db.ProcessExceptionMessage(ex) });
        //    }
        //}
        [HttpPost]
        public IActionResult Antrian(string reportId)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";
                var chk=from q in db.Master_Keterangan
                        where q.Stsrc=="A" && q.mk_kode==reportId
                        select q.mk_menu;
                if (chk.Count() != 0)
                {
                    pageTitle = chk.FirstOrDefault();
                }
                //TODO : tambah permission
                //db.CheckPermission("DA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                var newq = new RptGrid_Queue();
                newq.rgq_tablename = reportId;
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, reportId);
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringVariable1 = null;
                string stringVariable2 = null;
                string stringPeriode = null;
                if (reportId == "da_anomali_nilai_agunan_deb")
                {
                    newq.rgq_nama = "DA06 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("na") != null)
                    {
                        stringVariable2 = TempData.Peek("na").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliNilaiAgunanDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringVariable2, stringPeriode, isHive);
                    db.CheckPermission("DA06 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nik_deb")
                {
                    newq.rgq_nama = "DA07 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliNIKDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA07 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_gelar_nama_deb")
                {
                    newq.rgq_nama = "DA08 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliGelarNamaDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA08 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_deb")
                {
                    newq.rgq_nama = "DA09 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("s") != null)
                    {
                        stringVariable1 = TempData.Peek("s").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliAlamatDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA09 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_analisis_debtor_nom_identitas_sama")
                {
                    newq.rgq_nama = "DA10 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnalisisDebtorNomIdentitasSamaQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA10 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_analisis_debtor_nom_identitas_beda")
                {
                    newq.rgq_nama = "DA11 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnalisisDebtorNomIdentitasBedaQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA11 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nama_ibu_kandung")
                {
                    newq.rgq_nama = "DA12 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliNamaIbuKandungQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA12 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_bentuk_badan_usaha")
                {
                    newq.rgq_nama = "DA13 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliBentukBadanUsahaQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA13 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nilai_njop_agunan")
                {
                    newq.rgq_nama = "DA14 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("nn") != null)
                    {
                        stringVariable2 = TempData.Peek("nn").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliNilaiNJOPAgunanQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringVariable2, stringPeriode, isHive);
                    db.CheckPermission("DA14 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_penghasilan_per_tahun")
                {
                    newq.rgq_nama = "DA15 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("np") != null)
                    {
                        stringVariable1 = TempData.Peek("np").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliPenghasilanPTQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA15 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nik_lahir_debitur")
                {
                    newq.rgq_nama = "DA16 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliNIKLahirDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA16 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_npwp")
                {
                    newq.rgq_nama = "DA17 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliFormatNPWPQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA17 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_lahir_debitur")
                {
                    newq.rgq_nama = "DA18 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliTempatLahirDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA18 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_baki_debet_tidak_wajar")
                {
                    newq.rgq_nama = "DA19 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("s") != null)
                    {
                        stringVariable1 = TempData.Peek("s").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliBakiDebetTWQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA19 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_telepon_debitur")
                {
                    newq.rgq_nama = "DA20 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliFormatTeleponDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA20 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_email_debitur")
                {
                    newq.rgq_nama = "DA21 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliAlamatEmailDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA21 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_bekerja_debitur")
                {
                    newq.rgq_nama = "DA22 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliTempatBekerjaDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA22 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_bekerja_debitur")
                {
                    newq.rgq_nama = "DA23 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliAlamatBekerjaDebQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA23 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_badan_usaha")
                {
                    newq.rgq_nama = "DA24 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliTempatBUQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA24 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nomor_akta_badan_usaha")
                {
                    newq.rgq_nama = "DA25 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliNomorAktaBUQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, isHive);
                    db.CheckPermission("DA25 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_peringkat_agunan")
                {
                    newq.rgq_nama = "DA26 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliFormatPeringkatAgunanQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA26 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tingkat_suku_bunga")
                {
                    newq.rgq_nama = "DA27 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("sb") != null)
                    {
                        stringVariable1 = TempData.Peek("sb").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliTingkatSukuBungaQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA27 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_bukti_kepemilikan_agunan")
                {
                    newq.rgq_nama = "DA28 Export CSV";
                    if (TempData.Peek("mt") != null)
                    {
                        stringMemberTypes = TempData.Peek("mt").ToString();
                    }
                    if (TempData.Peek("m") != null)
                    {
                        stringMembers = TempData.Peek("m").ToString();
                    }
                    if (TempData.Peek("kc") != null)
                    {
                        stringKantorCabangs = TempData.Peek("kc").ToString();
                    }
                    if (TempData.Peek("jd") != null)
                    {
                        stringVariable1 = TempData.Peek("jd").ToString();
                    }
                    if (TempData.Peek("p") != null)
                    {
                        stringPeriode = TempData.Peek("p").ToString();
                    }
                    newq.rgq_query = Helper.WSQueryExport.GetDA_AnomaliBuktiKepemilikanAgunanQuery(db, stringMemberTypes, stringMembers, stringKantorCabangs, stringVariable1, stringPeriode, isHive);
                    db.CheckPermission("DA28 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
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
            if (pageTitle == "DA29" || pageTitle == "DA30")
            {
                var isHive = false;
                ViewBag.Hive = isHive;
            }
            else
            {
                var isHive = Helper.WSQueryStore.IsPeriodInHive(db, id);
                ViewBag.Hive = isHive;
            }
            
            if (id == "da_anomali_nilai_agunan_deb")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA06", pageTitle);
                db.CheckPermission("DA06 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA06 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliNilaiAgunanDeb");
            }
            else if (id == "da_anomali_nik_deb")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA07", pageTitle);
                db.CheckPermission("DA07 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA07 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliNIKDeb");
            }
            else if (id == "da_anomali_gelar_nama_deb")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA08", pageTitle);
                db.CheckPermission("DA08 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA08 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliGelarNamaDeb");
            }
            else if (id == "da_anomali_alamat_deb")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA09", pageTitle);
                db.CheckPermission("DA09 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA09 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliAlamatDeb");
            }
            else if (id == "da_analisis_debtor_nom_identitas_sama")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA10", pageTitle);
                db.CheckPermission("DA10 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA10 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnalisisDebtorNomIdentitasSama");
            }
            else if (id == "da_analisis_debtor_nom_identitas_beda")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA11", pageTitle);
                db.CheckPermission("DA11 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA11 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnalisisDebtorNomIdentitasBeda");
            }
            else if (id == "da_anomali_nama_ibu_kandung")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA12", pageTitle);
                db.CheckPermission("DA12 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA12 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliNamaIbuKandung");
            }
            else if (id == "da_anomali_bentuk_badan_usaha")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA13", pageTitle);
                db.CheckPermission("DA13 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA13 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliBentukBadanUsaha");
            }
            else if (id == "da_anomali_nilai_njop_agunan")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA14", pageTitle);
                db.CheckPermission("DA14 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA14 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliNilaiNJOPAgunan");
            }
            else if (id == "da_anomali_penghasilan_per_tahun")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA15", pageTitle);
                db.CheckPermission("DA15 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA15 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliPenghasilanPT");
            }
            else if (id == "da_anomali_nik_lahir_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA16", pageTitle);
                db.CheckPermission("DA16 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA16 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliNIKLahirDeb");
            }
            else if (id == "da_anomali_format_npwp")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA17", pageTitle);
                db.CheckPermission("DA17 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA17 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliFormatNPWP");
            }
            else if (id == "da_anomali_tempat_lahir_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA18", pageTitle);
                db.CheckPermission("DA18 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA18 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliTempatLahirDeb");
            }
            else if (id == "da_anomali_baki_debet_tidak_wajar")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA19", pageTitle);
                db.CheckPermission("DA19 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA19 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliBakiDebetTW");
            }
            else if (id == "da_anomali_format_telepon_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA20", pageTitle);
                db.CheckPermission("DA20 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA20 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliFormatTeleponDeb");
            }
            else if (id == "da_anomali_alamat_email_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA21", pageTitle);
                db.CheckPermission("DA21 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA21 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliAlamatEmailDeb");
            }
            else if (id == "da_anomali_tempat_bekerja_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA22", pageTitle);
                db.CheckPermission("DA22 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA22 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliTempatBekerjaDeb");
            }
            else if (id == "da_anomali_alamat_bekerja_debitur")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA23", pageTitle);
                db.CheckPermission("DA23 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA23 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliAlamatBekerjaDeb");
            }
            else if (id == "da_anomali_tempat_badan_usaha")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA24", pageTitle);
                db.CheckPermission("DA24 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA24 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliTempatBU");
            }
            else if (id == "da_anomali_nomor_akta_badan_usaha")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA25", pageTitle);
                db.CheckPermission("DA25 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA25 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliNomorAktaBU");
            }
            else if (id == "da_anomali_format_peringkat_agunan")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA26", pageTitle);
                db.CheckPermission("DA26 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA26 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliFormatPeringkatAgunan");
            }
            else if (id == "da_anomali_tingkat_suku_bunga")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA27", pageTitle);
                db.CheckPermission("DA27 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA27 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliTingkatSukuBunga");
            }
            else if (id == "da_anomali_bukti_kepemilikan_agunan")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA28", pageTitle);
                db.CheckPermission("DA28 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                ViewBag.Export = db.CheckPermission("DA28 Export", DataEntities.PermissionMessageType.NoMessage);
                return View("AnomaliBuktiKepemilikanAgunan");
            }
            else if (id == "summary_dqm_per_ljk")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA29", pageTitle);
                db.CheckPermission("DA29 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
/*                ViewBag.Export = db.CheckPermission("DA28 Export", DataEntities.PermissionMessageType.NoMessage */
                return View("SummaryLJK");
            }
            else if (id == "summary_dqm_per_jenis_ljk")
            {
                db.InsertAuditTrail("DA_" + id + "_Akses_Page", "Akses Page Dashboard DA30", pageTitle);
                db.CheckPermission("DA30 View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                /*                ViewBag.Export = db.CheckPermission("DA28 Export", DataEntities.PermissionMessageType.NoMessage; */
                return View("SummaryJenisLJK");
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
                var chk=from q in db.Master_Keterangan
                         where q.Stsrc=="A" && q.mk_kode==reportId
                         select q.mk_menu;
                if (chk.Count() != 0) {
                    pageTitle = chk.FirstOrDefault();
                }
                //TODO : tambah permission
                //db.CheckPermission("DA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "da_anomali_nilai_agunan_deb")
                {
                    db.CheckPermission("DA06 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nik_deb")
                {
                    db.CheckPermission("DA07 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_gelar_nama_deb")
                {
                    db.CheckPermission("DA08 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_deb")
                {
                    db.CheckPermission("DA09 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_analisis_debtor_nom_identitas_sama")
                {
                    db.CheckPermission("DA10 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_analisis_debtor_nom_identitas_beda")
                {
                    db.CheckPermission("DA11 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nama_ibu_kandung")
                {
                    db.CheckPermission("DA12 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_bentuk_badan_usaha")
                {
                    db.CheckPermission("DA13 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nilai_njop_agunan")
                {
                    db.CheckPermission("DA14 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_penghasilan_per_tahun")
                {
                    db.CheckPermission("DA15 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nik_lahir_debitur")
                {
                    db.CheckPermission("DA16 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_npwp")
                {
                    db.CheckPermission("DA17 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_lahir_debitur")
                {
                    db.CheckPermission("DA18 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_baki_debet_tidak_wajar")
                {
                    db.CheckPermission("DA19 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_telepon_debitur")
                {
                    db.CheckPermission("DA20 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_email_debitur")
                {
                    db.CheckPermission("DA21 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_bekerja_debitur")
                {
                    db.CheckPermission("DA22 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_bekerja_debitur")
                {
                    db.CheckPermission("DA23 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_badan_usaha")
                {
                    db.CheckPermission("DA24 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nomor_akta_badan_usaha")
                {
                    db.CheckPermission("DA25 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_peringkat_agunan")
                {
                    db.CheckPermission("DA26 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tingkat_suku_bunga")
                {
                    db.CheckPermission("DA27 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    db.CheckPermission("DA28 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
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

        public IActionResult ExportPDF(string reportId, string menuName, string deskripsiExport, string bulanData, string jenisLJK, string kodeLJK, string kantorCabang, string totalRows, string nilaiAgunan, string segmen, string jenisDebitur, IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";
                var chk=from q in db.Master_Keterangan
                        where q.Stsrc=="A" && q.mk_kode==reportId
                        select q.mk_menu;
                if (chk.Count() != 0)
                {
                    pageTitle = chk.FirstOrDefault();
                }
                //TODO : tambah permission
                //db.CheckPermission("DA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "da_anomali_nilai_agunan_deb")
                {
                    db.CheckPermission("DA06 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nik_deb")
                {
                    db.CheckPermission("DA07 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_gelar_nama_deb")
                {
                    db.CheckPermission("DA08 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_deb")
                {
                    db.CheckPermission("DA09 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_analisis_debtor_nom_identitas_sama")
                {
                    db.CheckPermission("DA10 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_analisis_debtor_nom_identitas_beda")
                {
                    db.CheckPermission("DA11 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nama_ibu_kandung")
                {
                    db.CheckPermission("DA12 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_bentuk_badan_usaha")
                {
                    db.CheckPermission("DA13 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nilai_njop_agunan")
                {
                    db.CheckPermission("DA14 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_penghasilan_per_tahun")
                {
                    db.CheckPermission("DA15 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nik_lahir_debitur")
                {
                    db.CheckPermission("DA16 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_npwp")
                {
                    db.CheckPermission("DA17 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_lahir_debitur")
                {
                    db.CheckPermission("DA18 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_baki_debet_tidak_wajar")
                {
                    db.CheckPermission("DA19 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_telepon_debitur")
                {
                    db.CheckPermission("DA20 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_email_debitur")
                {
                    db.CheckPermission("DA21 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_bekerja_debitur")
                {
                    db.CheckPermission("DA22 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_alamat_bekerja_debitur")
                {
                    db.CheckPermission("DA23 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tempat_badan_usaha")
                {
                    db.CheckPermission("DA24 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_nomor_akta_badan_usaha")
                {
                    db.CheckPermission("DA25 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_format_peringkat_agunan")
                {
                    db.CheckPermission("DA26 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else if (reportId == "da_anomali_tingkat_suku_bunga")
                {
                    db.CheckPermission("DA27 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }
                else
                {
                    db.CheckPermission("DA28 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }

                db.InsertAuditTrail("ExportIndex_DA_" + reportId, "Export Data", pageTitle);
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
                    Style numericStyle = workbook.CreateStyle();
                    numericStyle.Custom = "#,##0.00";
                    numericStyle.HorizontalAlignment = TextAlignmentType.Right;
                    StyleFlag numericFlag = new StyleFlag();
                    numericFlag.NumberFormat = true;
                    numericFlag.HorizontalAlignment = true;

                    foreach (Cell cell in worksheet.Cells)
                    {
                        if (cell.Type == CellValueType.IsNumeric)
                        {
                            cell.SetStyle(numericStyle);
                        }
                    }

                    //page setup
                    PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.Orientation = PageOrientationType.Landscape;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.FitToPagesTall = 0;

                    if (reportId == "da_anomali_nilai_agunan_deb" || reportId == "da_anomali_nik_deb" || reportId == "da_anomali_gelar_nama_deb" || reportId == "da_anomali_alamat_deb" || reportId == "da_analisis_debtor_nom_identitas_sama" || reportId == "da_analisis_debtor_nom_identitas_beda" || reportId == "da_anomali_nama_ibu_kandung" || reportId == "da_anomali_bentuk_badan_usaha")
                    {
                        worksheet.Cells.InsertRow(0);

                        //apply style for additional info
                        Range additionalInfoRange = worksheet.Cells.CreateRange(0, 0, 1, 10);
                        Style additionalInfoStyle = workbook.CreateStyle();
                        additionalInfoStyle.HorizontalAlignment = TextAlignmentType.Left;
                        additionalInfoStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.None;
                        additionalInfoStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
                        additionalInfoStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.None;
                        additionalInfoStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.None;
                        additionalInfoRange.ApplyStyle(additionalInfoStyle, new StyleFlag() { Borders = true, HorizontalAlignment = true });

                        //set the height for the additional information rows and merge cells 0,0 and 0,1
                        worksheet.Cells.Merge(0, 0, 1, 10);

                        if (reportId == "da_anomali_nilai_agunan_deb")
                        {
                            worksheet.Cells.SetRowHeight(0, 350);
                            pageSetup.TopMargin = 1.8;
                        }
                        else
                        if (reportId == "da_anomali_nik_deb")
                        {
                            worksheet.Cells.SetRowHeight(0, 325);
                            pageSetup.TopMargin = 2;
                        }
                        else
                        if (reportId == "da_anomali_gelar_nama_deb")
                        {
                            worksheet.Cells.SetRowHeight(0, 250);
                            pageSetup.TopMargin = 1.8;
                        }
                        else
                        if (reportId == "da_anomali_alamat_deb")
                        {
                            worksheet.Cells.SetRowHeight(0, 400);
                            pageSetup.TopMargin = 1.8;
                        }
                        else
                        if (reportId == "da_anomali_bentuk_badan_usaha")
                        {
                            worksheet.Cells.SetRowHeight(0, 250);
                            pageSetup.TopMargin = 2;
                        }
                        else
                        {
                            worksheet.Cells.SetRowHeight(0, 300);
                            pageSetup.TopMargin = 1.8;
                        }

                        worksheet.Cells[0, 0].PutValue("Nama Menu: " + menuName + (deskripsiExport != null ? "\n \n" + "Deskripsi Kode Anomali: " + deskripsiExport : "") + "\n \n" + "Total Rows: " + totalRows + "\n \n" + "Bulan Data: " + bulanData + (jenisLJK != null ? "\n \n" + "Jenis LJK: " + jenisLJK : "") + (kodeLJK != null ? "\n \n" + "Kode LJK: " + kodeLJK : "") + (kantorCabang != null ? "\n \n" + "Kantor Cabang: " + kantorCabang : "") + (nilaiAgunan != null ? "\n \n" + "Nilai Agunan Menurut Pelapor ≤ " + nilaiAgunan : "") + (segmen != null ? "\n \n" + "Segmen: " + segmen : "") + (jenisDebitur != null ? "\n \n" + "Jenis Debitur: " + jenisDebitur : "" + "\n \n"));
                    }
                    //set header
                    pageSetup.SetHeaderPicture(0, binaryData);
                    pageSetup.SetHeader(0, "&G");
                    var img = pageSetup.GetPicture(true, 0);
                    img.WidthScale = 10;
                    img.HeightScale = 10;

                    //set footer
                    pageSetup.SetFooter(0, "Export Date: " + timeStamp);

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

        public object GetGridDataDA_AnomaliNilaiAgunanDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string nilaiAgunan, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringJenisDebiturs = null;
                string stringNilaiAgunan = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_nilai_agunan_deb");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (JenisDebiturs.Length > 0)
                {
                    stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                    TempData["jd"] = stringJenisDebiturs;
                }
                if (nilaiAgunan != null)
                {
                    stringNilaiAgunan = nilaiAgunan.Replace(",", "");
                    TempData["na"] = stringNilaiAgunan;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliNilaiAgunanDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringJenisDebiturs, stringNilaiAgunan, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliNIKDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_nik_deb");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliNIKDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliGelarNamaDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_gelar_nama_deb");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliGelarNamaDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliAlamatDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string segmens, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] Segmens = JsonConvert.DeserializeObject<string[]>(segmens);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringSegmens = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_alamat_deb");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (Segmens.Length > 0)
                {
                    stringSegmens = string.Join(", ", Segmens);
                    TempData["s"] = stringSegmens;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliAlamatDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringSegmens, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnalisisDebtorNomIdentitasSama(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringJenisDebiturs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_analisis_debtor_nom_identitas_sama");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (JenisDebiturs.Length > 0)
                {
                    stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                    TempData["jd"] = stringJenisDebiturs;
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

                var result = Helper.WSQueryStore.GetDA_AnalisisDebtorNomIdentitasSamaQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringJenisDebiturs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnalisisDebtorNomIdentitasBeda(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringJenisDebiturs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_analisis_debtor_nom_identitas_beda");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (JenisDebiturs.Length > 0)
                {
                    stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                    TempData["jd"] = stringJenisDebiturs;
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

                var result = Helper.WSQueryStore.GetDA_AnalisisDebtorNomIdentitasBedaQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringJenisDebiturs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliNamaIbuKandung(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }

                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_nama_ibu_kandung");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliNamaIbuKandungQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliBentukBadanUsaha(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_bentuk_badan_usaha");
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
                        TempData["m"] = stringMembers;
                    }

                }

                if (Members.Length > 0)
                {
                    TempData["memberValue"] = members;
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["mt"] = stringMemberTypes;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliBentukBadanUsahaQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliNilaiNJOPAgunan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string nilaiNJOP, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringJenisDebiturs = null;
                string stringNilaiNJOP = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_nilai_njop_agunan");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (JenisDebiturs.Length > 0)
                {
                    stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                    TempData["jd"] = stringJenisDebiturs;
                }
                if (nilaiNJOP != null)
                {
                    stringNilaiNJOP = nilaiNJOP;
                    TempData["nn"] = stringNilaiNJOP;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliNilaiNJOPAgunanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringJenisDebiturs, stringNilaiNJOP, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliPenghasilanPT(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string nilaiPenghasilan, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringNilaiPenghasilan = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_penghasilan_per_tahun");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (nilaiPenghasilan != null)
                {
                    stringNilaiPenghasilan = nilaiPenghasilan;
                    TempData["np"] = stringNilaiPenghasilan;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliPenghasilanPTQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringNilaiPenghasilan, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliNIKLahirDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_nik_lahir_debitur");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliNIKLahirDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliFormatNPWP(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_format_npwp");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliFormatNPWPQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliTempatLahirDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_tempat_lahir_debitur");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliTempatLahirDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliBakiDebetTW(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string segmens, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] Segmens = JsonConvert.DeserializeObject<string[]>(segmens);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringSegmens = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_baki_debet_tidak_wajar");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (Segmens.Length > 0)
                {
                    stringSegmens = string.Join(", ", Segmens);
                    TempData["s"] = stringSegmens;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliBakiDebetTWQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringSegmens, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliFormatTeleponDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_format_telepon_debitur");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliFormatTeleponDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliAlamatEmailDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_alamat_email_debitur");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliAlamatEmailDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliTempatBekerjaDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_tempat_bekerja_debitur");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliTempatBekerjaDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliAlamatBekerjaDeb(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_alamat_bekerja_debitur");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliAlamatBekerjaDebQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliTempatBU(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_tempat_badan_usaha");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliTempatBUQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliNomorAktaBU(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_nomor_akta_badan_usaha");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliNomorAktaBUQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliFormatPeringkatAgunan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear(); 
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringJenisDebiturs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_format_peringkat_agunan");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (JenisDebiturs.Length > 0)
                {
                    stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                    TempData["jd"] = stringJenisDebiturs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliFormatPeringkatAgunanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringJenisDebiturs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliTingkatSukuBunga(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string sukuBunga, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear();
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringSukuBunga = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_tingkat_suku_bunga");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (sukuBunga != null)
                {
                    stringSukuBunga = sukuBunga;
                    TempData["sb"] = stringSukuBunga;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliTingkatSukuBungaQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringSukuBunga, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        public object GetGridDataDA_AnomaliBuktiKepemilikanAgunan(DataSourceLoadOptions loadOptions, string memberTypes, string members, string kantorCabangs, string jenisDebiturs, string periode)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string[] periodes = JsonConvert.DeserializeObject<string[]>(periode);

            if (periodes.Length > 0)
            {
                TempData.Clear();
                string[] MemberTypes = JsonConvert.DeserializeObject<string[]>(memberTypes);
                string[] Members = JsonConvert.DeserializeObject<string[]>(members);
                string[] KantorCabangs = JsonConvert.DeserializeObject<string[]>(kantorCabangs);
                string[] JenisDebiturs = JsonConvert.DeserializeObject<string[]>(jenisDebiturs);

                Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();
                KantorCabangs = KantorCabangs.Select(x => x.Split('-').Last().TrimStart(' ')).ToArray();

                if (members != null)
                {
                    members = members.Substring(members.IndexOf("- ") + 2, members.Length - (members.IndexOf("- ") + 2));
                }
                string stringMemberTypes = null;
                string stringMembers = null;
                string stringKantorCabangs = null;
                string stringJenisDebiturs = null;
                string stringPeriode = null;
                List<DateTime> lp = new List<DateTime>();
                foreach (var i in periodes)
                {
                    lp.Add(DateTime.Parse(i.Trim().Replace("'", "")));
                }

                var cekHive = Helper.WSQueryStore.IsPeriodInHive(db, "da_anomali_bukti_kepemilikan_agunan");
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
                    var listOfKC = db.master_ljk.ToList();
                    // nih gara2 si data processing kaga pake kode di output nya -_-;
                    stringMembers = "";
                    foreach (var kc in Members)
                    {
                        var find = listOfKC.Where(x => x.kode_ljk == kc).FirstOrDefault();
                        if (find != null)
                        {
                            if (stringMembers != "") stringMembers += ", ";
                            stringMembers += find.kode_ljk;
                        }
                        TempData["m"] = stringMembers;
                    }
                }

                if (KantorCabangs.Length > 0)
                {
                    stringKantorCabangs = string.Join(", ", KantorCabangs);
                    TempData["kc"] = stringKantorCabangs;
                }
                if (JenisDebiturs.Length > 0)
                {
                    stringJenisDebiturs = string.Join(", ", JenisDebiturs);
                    TempData["jd"] = stringJenisDebiturs;
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

                var result = Helper.WSQueryStore.GetDA_AnomaliBuktiKepemilikanAgunanQuery(db, loadOptions, stringMemberTypes, stringMembers, stringKantorCabangs, stringJenisDebiturs, stringPeriode, cekHive);

                return JsonConvert.SerializeObject(result);
            }
            else
            {
                loadOptions = new DataSourceLoadOptions();

            }

            return DataSourceLoader.Load(new List<string>(), loadOptions);

        }

        #endregion

    }
}

