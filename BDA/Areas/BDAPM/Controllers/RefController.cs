using System;
using System.Collections.Generic;
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
    [Area("BDAPM")]
    public class RefController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public RefController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public static bool IsPengawasLJK(DataEntities db)
        {
            var roleId = db.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (roleId.Contains("PengawasLJK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string[] GetFilteredMemberTypes(DataEntities db, string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.deskripsi_jenis_ljk).Distinct().ToArray();
            return filter;
        }
        public static string[] GetFilteredMemberTypes2(DataEntities db, string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_type_code).Distinct().ToArray();
            return filter;
        }
        public static string[] GetFilteredMembers(DataEntities db, string login)
        {
            var filter = db.getLJKPengawas(login).Select(x => x.member_code).ToArray();
            return filter;
        }

        #region "RefGetter"
        public IActionResult GetMemberTypes(DataSourceLoadOptions loadOptions)
        {
            var login = this.User.Identity.Name;

            var query = db.master_ljk_type.Where(x => x.status_aktif == "Y" && x.status_delete == "T").Select(x => new { x.kode_jenis_ljk, Display = x.kode_jenis_ljk + " - " + x.deskripsi_jenis_ljk }).ToList();

            if (IsPengawasLJK(db))
            {
                var filter = GetFilteredMemberTypes2(db, login);
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
                loadOptions.Take = 20;
            }

            string[] MemberTypes = string.IsNullOrEmpty(memberTypes) ? new string[] { } : memberTypes.Split(",");

            //var query = db.master_ljk.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, Display = x.kode_ljk + " - " + x.nama_ljk }).ToList();
            var query = db.vw_getMasterLJK.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, x.nama_ljk, x.CompositeKey, x.Display }).ToList();

            if (IsPengawasLJK(db))
            {
                var filter = GetFilteredMembers(db, login);
                query = query.Where(x => filter.Contains(x.kode_ljk)).ToList();
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }
        public IActionResult GetKantorCabangs(DataSourceLoadOptions loadOptions, string memberTypes, string members, bool reset)
        {
            var login = this.User.Identity.Name;

            if (reset)
            {
                loadOptions.Skip = 0;
                loadOptions.Take = 20;
            }

            string[] MemberTypes = string.IsNullOrEmpty(memberTypes) ? new string[] { } : memberTypes.Split(",");

            string[] Members = string.IsNullOrEmpty(members) ? new string[] { } : members.Split(",");

            Members = Members.Select(x => x.Substring(x.IndexOf("- ") + 2, x.Length - (x.IndexOf("- ") + 2))).ToArray();

            //var query = db.master_ljk.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, Display = x.kode_ljk + " - " + x.nama_ljk }).ToList();
            var query = db.vw_getMasterOfficeLJK.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && Members.Contains(x.kode_ljk) && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { text = x.Display, kode = x.CompositeKey }).Distinct().OrderBy(x => x.text).ToList();

            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }
        public IActionResult GetMembersOnly(DataSourceLoadOptions loadOptions)
        {
            var login = this.User.Identity.Name;

            //var query = db.master_ljk.Where(x => x.status_aktif == "Y" && x.status_delete == "T" && MemberTypes.Contains(x.kode_jenis_ljk)).Select(x => new { x.kode_ljk, Display = x.kode_ljk + " - " + x.nama_ljk }).ToList();
            var query = db.vw_getMasterLJK.Where(x => x.status_aktif == "Y" && x.status_delete == "T").Select(x => new { x.kode_ljk, x.nama_ljk, x.CompositeKey, x.Display,x.kode_jenis_ljk }).ToList();

            if (IsPengawasLJK(db))
            {
                var filter = GetFilteredMembers(db, login);
                query = query.Where(x => filter.Contains(x.kode_ljk)).ToList();
                var filter2 = GetFilteredMemberTypes2(db, login);
                query = query.Where(x => filter2.Contains(x.kode_jenis_ljk)).ToList();
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }
        public IActionResult GetPeriode(DataSourceLoadOptions loadOptions)
        {
            var startDate = new DateTime(2019, 1, 1);
            var list = new List<PeriodeDto>();
            while (startDate < DateTime.Now)
            {
                list.Add(new PeriodeDto() { value = String.Format("{0:yyyy-MM-dd}", startDate), text = String.Format("{0:yyyy-MM}", startDate) });
                startDate = startDate.AddMonths(1);
            }
            return Json(DataSourceLoader.Load(list, loadOptions));
        }

        public class PeriodeDto
        {
            public string value { get; set; }
            public string text { get; set; }
        }
        public class RefDto
        {
            public string kode { get; set; }
            public string text { get; set; }
        }

        public IActionResult GetJenisAgunans(DataSourceLoadOptions loadOptions)
        {
            var q = db.ref_collateral_types.Select(x => new { x.dm_jenis_agunan, kode = x.dm_jenis_agunan }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }

        public IActionResult GetPekerjaans(DataSourceLoadOptions loadOptions)
        {
            var q = db.ref_pekerjaan_temp.Select(x => new { text = x.dm_profesi_debitur, kode = x.dm_kode_profesi_debitur }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }

        public IActionResult GetJenisPenggunaans(DataSourceLoadOptions loadOptions)
        {
            //napa ini ga ada kode nya sih
            var q = db.ref_credit_usages.Select(x => new { text = x.dm_deskripsi_jenis_penggunaan, kode = x.dm_deskripsi_jenis_penggunaan }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetJenisPinjamans(DataSourceLoadOptions loadOptions)
        {
            //napa ini ga ada kode nya sih
            var q = db.ref_credit_types.Select(x => new { text = x.dm_deskripsi_jenis_kredit_pembiayaan, kode = x.dm_deskripsi_jenis_kredit_pembiayaan }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetKodeJenisPinjamans(DataSourceLoadOptions loadOptions)
        {
            var q = db.ma_kolektibilitas_kesalahan_ljk.Select(x => new { text = x.dm_kode_jenis_pinjaman, kode = x.dm_kode_jenis_pinjaman }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetTipeForecastings(DataSourceLoadOptions loadOptions)
        {
            var q = db.macro_output_forecast_level_ljk.Select(x => new { text = x.dm_tipe_forecasting, kode = x.dm_tipe_forecasting }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetJenisPertumbuhans(DataSourceLoadOptions loadOptions)
        {
            var q = db.macro_pertumbuhan_pinjaman_level_ljk.Select(x => new { text = "%" + x.dm_jenis_pertumbuhan, kode = x.dm_jenis_pertumbuhan }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetJenisDebiturs(DataSourceLoadOptions loadOptions)
        {
            //var q = db.macro_penetrasi_lending_ljk.Select(x => new { text = x.dm_jenis_debitur, kode = x.dm_jenis_debitur }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Badan Usaha", kode = "Badan Usaha" });
            list.Add(new RefDto() { text = "Individu", kode = "Individu" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetKategori(DataSourceLoadOptions loadOptions)
        {
            var q = db.macro_penetrasi_lending_ljk.Select(x => new { text = x.dm_kategori, kode = x.dm_kategori }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetDeskripsiKategoris(DataSourceLoadOptions loadOptions, string kategori)
        {
            var q = db.vw_RefDeskripsiKategori.Where(x => x.tipe == kategori).Select(x => new { text = x.desk, kode = x.desk.Replace("-", "~").Replace(",", "|") }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetPolicyJenisDebiturs(DataSourceLoadOptions loadOptions)
        {
            var q = db.macro_policy_evaluation_analysis.Select(x => new { text = x.dm_jenis_debitur, kode = x.dm_jenis_debitur.Replace("-", "~").Replace(",", "|") }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetJenisDebiturDAs(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_analisis_pencurian_identitas.Select(x => new { text = x.dm_similarity_result==0?"Tidak":"Ya", kode = x.dm_similarity_result }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "D01 - Debitur Perserorangan", kode = "D01 ~ Debitur Perserorangan" });
            list.Add(new RefDto() { text = "D02 - Debitur Badan usaha", kode = "D02 ~ Debitur Badan usaha" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetSegmens(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_analisis_pencurian_identitas.Select(x => new { text = x.dm_similarity_result==0?"Tidak":"Ya", kode = x.dm_similarity_result }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "M01 - Pengurus/Pemilik", kode = "M01 ~ Pengurus/Pemilik" });
            list.Add(new RefDto() { text = "D01 - Debitur Perserorangan", kode = "D01 ~ Debitur Perserorangan" });
            list.Add(new RefDto() { text = "D02 - Debitur Badan usaha", kode = "D02 ~ Debitur Badan usaha" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetSegmenFs(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_analisis_pencurian_identitas.Select(x => new { text = x.dm_similarity_result==0?"Tidak":"Ya", kode = x.dm_similarity_result }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "F01 - Kredit/Pembiayaan", kode = "F01 ~ Kredit/Pembiayaan" });
            list.Add(new RefDto() { text = "F02 - Kredit/Pembiayaan Joint Account", kode = "F02 ~ Kredit/Pembiayaan Joint Account" });
            list.Add(new RefDto() { text = "F03 - Surat Berharga", kode = "F03 ~ Surat Berharga" });
            list.Add(new RefDto() { text = "F04 - Irrevocable L/C", kode = "F04 ~ Irrevocable L/C" });
            list.Add(new RefDto() { text = "F05 - Garansi yang Diberikan", kode = "F05 ~ Garansi yang Diberikan" });
            list.Add(new RefDto() { text = "F06 - Fasilitas Lain", kode = "F06 ~ Fasilitas Lain" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetKolektibilitass(DataSourceLoadOptions loadOptions)
        {
            var list = new List<PeriodeDto>();
            for (var i = 1; i <= 5; i++)
            {
                list.Add(new PeriodeDto() { value = i.ToString(), text = i.ToString() });
            }
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetKolektibilitasSSs(DataSourceLoadOptions loadOptions)
        {
            var list = new List<PeriodeDto>();
            for (var i = 1; i <= 5; i++)
            {
                for (var j = 1; j <= 5; j++)
                {
                    list.Add(new PeriodeDto() { value = i.ToString() + "~" + j.ToString(), text = i.ToString() + "-" + j.ToString() });
                }
            }
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetKategoriAsesmens(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_analisis_pencurian_identitas.Select(x => new { text = x.dm_kategori_asesmen, kode = x.dm_kategori_asesmen }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Nomor KTP Sama", kode = "Nomor KTP Sama" });
            list.Add(new RefDto() { text = "Informasi Pribadi Debitur", kode = "Informasi Pribadi Debitur" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetSimilarityResults(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_analisis_pencurian_identitas.Select(x => new { text = x.dm_similarity_result==0?"Tidak":"Ya", kode = x.dm_similarity_result }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Tidak", kode = "0" });
            list.Add(new RefDto() { text = "Ya", kode = "1" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetKelasPlafons(DataSourceLoadOptions loadOptions)
        {
            //var q = db.micro_plafond_usability_acc_detail.Select(x => new { text = x.dm_kelas_plafon, kode = x.dm_kelas_plafon.Replace("-", "~") }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "IDR 0 - 10 Juta", kode = "IDR 0 ~ 10 Juta" });
            list.Add(new RefDto() { text = "IDR 10 - 100 Juta", kode = "IDR 10 ~ 100 Juta" });
            list.Add(new RefDto() { text = "IDR 100 - 1000 Juta", kode = "IDR 100 ~ 1000 Juta" });
            list.Add(new RefDto() { text = "IDR 1 - 10 Miliar", kode = "IDR 1 ~ 10 Miliar" });
            list.Add(new RefDto() { text = "IDR 10 - 100 Miliar", kode = "IDR 10 ~ 100 Miliar" });
            list.Add(new RefDto() { text = "IDR 100 - 1000 Miliar", kode = "IDR 100 ~ 1000 Miliar" });
            list.Add(new RefDto() { text = "IDR 1 - 10 Triliun", kode = "IDR 1 ~ 10 Triliun" });
            list.Add(new RefDto() { text = "IDR > 10 Triliun", kode = "IDR > 10 Triliun" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetNPLs(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_rekening_baru_non_restrukturisasi_npl.Select(x => new { text = x.dm_npl, kode = x.dm_npl }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "2", kode = "2" });
            list.Add(new RefDto() { text = "3", kode = "3" });
            list.Add(new RefDto() { text = "4", kode = "4" });
            list.Add(new RefDto() { text = "5", kode = "5" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetLokasiAgunans(DataSourceLoadOptions loadOptions)
        {
            var q = db.ref_cities.Select(x => new { text = x.dm_nama_kota, kode = x.dm_nama_kota }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetJenisTA(DataSourceLoadOptions loadOptions)
        {
            //var q = db.ma_analisis_pencurian_identitas.Select(x => new { text = x.dm_kategori_asesmen, kode = x.dm_kategori_asesmen }).Distinct();
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Anomali Format Penulisan ID KTP", kode = "Anomali Penulisan ID KTP" });
            list.Add(new RefDto() { text = "Anomali Gender dengan Format ID KTP Valid", kode = "Anomali Gender dengan KTP Valid" });
            list.Add(new RefDto() { text = "Anomali Dugaan Duplikasi Nama Debitur terhadap ID KTP yang sama", kode = "Anomali Suspek Duplikasi Nama Debitur" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetJenisAgunanDADokumens(DataSourceLoadOptions loadOptions)
        {
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Tanah", kode = "Tanah" });
            //list.Add(new RefDto() { text = "Obligasi Ritel Indonesia (ORI)", kode = "Obligasi Ritel Indonesia (ORI)" });
            list.Add(new RefDto() { text = "Tabungan", kode = "Tabungan" });
            list.Add(new RefDto() { text = "Rumah Tinggal", kode = "Rumah Tinggal" });
            list.Add(new RefDto() { text = "Setoran Jaminan", kode = "Setoran Jaminan" });
            list.Add(new RefDto() { text = "Rumah Toko/Rumah Kantor/Kios", kode = "Rumah Toko/Rumah Kantor/Kios" });
            list.Add(new RefDto() { text = "Agunan Lain", kode = "Agunan Lain" });
            list.Add(new RefDto() { text = "Jaminan Lainnya", kode = "Jaminan Lainnya" });
            list.Add(new RefDto() { text = "Hotel", kode = "Hotel" });
            list.Add(new RefDto() { text = "Reksadana", kode = "Reksadana" });
            list.Add(new RefDto() { text = "Surat Perbendaharaan Negara (SPN)", kode = "Surat Perbendaharaan Negara (SPN)" });
            list.Add(new RefDto() { text = "Kendaraan", kode = "Kendaraan" });
            list.Add(new RefDto() { text = "Resi Gudang", kode = "Resi Gudang" });
            list.Add(new RefDto() { text = "Saham", kode = "Saham" });
            list.Add(new RefDto() { text = "Apartemen/Rumah Susun", kode = "Apartemen/Rumah Susun" });
            list.Add(new RefDto() { text = "Properti Komersial Lainnya", kode = "Properti Komersial Lainnya" });
            list.Add(new RefDto() { text = "Surat Berharga Lainnya", kode = "Surat Berharga Lainnya" });
            list.Add(new RefDto() { text = "Gedung", kode = "Gedung" });
            //list.Add(new RefDto() { text = "Rumah", kode = "Rumah" });
            list.Add(new RefDto() { text = "Persediaan", kode = "Persediaan" });
            //list.Add(new RefDto() { text = "Sertifikat Deposito Bank Indonesia", kode = "Sertifikat Deposito Bank Indonesia" });
            //list.Add(new RefDto() { text = "Sukuk Lainnya", kode = "Sukuk Lainnya" });
            list.Add(new RefDto() { text = "Mesin", kode = "Mesin" });
            list.Add(new RefDto() { text = "Sertifikat Bank Indonesia (SBI)", kode = "Sertifikat Bank Indonesia (SBI)" });
            list.Add(new RefDto() { text = "Asuransi Pembiayaan", kode = "Asuransi Pembiayaan" });
            list.Add(new RefDto() { text = "Giro", kode = "Giro" });
            list.Add(new RefDto() { text = "Garansi", kode = "Garansi" });
            //list.Add(new RefDto() { text = "Rumah Toko/Rumah Kantor", kode = "Rumah Toko/Rumah Kantor" });
            list.Add(new RefDto() { text = "Emas", kode = "Emas" });
            list.Add(new RefDto() { text = "Gudang", kode = "Gudang" });
            list.Add(new RefDto() { text = "Obligasi Negara (ON)", kode = "Obligasi Negara (ON)" });
            list.Add(new RefDto() { text = "Pesawat Udara", kode = "Pesawat Udara" });
            list.Add(new RefDto() { text = "Kapal Laut atau Alat Transportasi Air", kode = "Kapal Laut atau Alat Transportasi Air" });
            list.Add(new RefDto() { text = "Standby L/C (SBLC)", kode = "Standby L/C (SBLC)" });
            list.Add(new RefDto() { text = "Simpanan Berjangka", kode = "Simpanan Berjangka" });
            list.Add(new RefDto() { text = "Kendaraan Bermotor", kode = "Kendaraan Bermotor" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetJenisAgunanDAIDs(DataSourceLoadOptions loadOptions)
        {
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Tanah", kode = "Tanah" });
            list.Add(new RefDto() { text = "Obligasi Ritel Indonesia (ORI)", kode = "Obligasi Ritel Indonesia (ORI)" });
            list.Add(new RefDto() { text = "Tabungan", kode = "Tabungan" });
            list.Add(new RefDto() { text = "Rumah Tinggal", kode = "Rumah Tinggal" });
            list.Add(new RefDto() { text = "Setoran Jaminan", kode = "Setoran Jaminan" });
            list.Add(new RefDto() { text = "Rumah Toko/Rumah Kantor/Kios", kode = "Rumah Toko/Rumah Kantor/Kios" });
            list.Add(new RefDto() { text = "Agunan Lain", kode = "Agunan Lain" });
            list.Add(new RefDto() { text = "Jaminan Lainnya", kode = "Jaminan Lainnya" });
            list.Add(new RefDto() { text = "Hotel", kode = "Hotel" });
            list.Add(new RefDto() { text = "Reksadana", kode = "Reksadana" });
            list.Add(new RefDto() { text = "Surat Perbendaharaan Negara (SPN)", kode = "Surat Perbendaharaan Negara (SPN)" });
            list.Add(new RefDto() { text = "Kendaraan", kode = "Kendaraan" });
            list.Add(new RefDto() { text = "Resi Gudang", kode = "Resi Gudang" });
            list.Add(new RefDto() { text = "Saham", kode = "Saham" });
            list.Add(new RefDto() { text = "Apartemen/Rumah Susun", kode = "Apartemen/Rumah Susun" });
            list.Add(new RefDto() { text = "Properti Komersial Lainnya", kode = "Properti Komersial Lainnya" });
            list.Add(new RefDto() { text = "Surat Berharga Lainnya", kode = "Surat Berharga Lainnya" });
            list.Add(new RefDto() { text = "Gedung", kode = "Gedung" });
            list.Add(new RefDto() { text = "Rumah", kode = "Rumah" });
            list.Add(new RefDto() { text = "Persediaan", kode = "Persediaan" });
            list.Add(new RefDto() { text = "Sertifikat Deposito Bank Indonesia", kode = "Sertifikat Deposito Bank Indonesia" });
            list.Add(new RefDto() { text = "Sukuk Lainnya", kode = "Sukuk Lainnya" });
            list.Add(new RefDto() { text = "Mesin", kode = "Mesin" });
            list.Add(new RefDto() { text = "Sertifikat Bank Indonesia (SBI)", kode = "Sertifikat Bank Indonesia (SBI)" });
            list.Add(new RefDto() { text = "Asuransi Pembiayaan", kode = "Asuransi Pembiayaan" });
            list.Add(new RefDto() { text = "Giro", kode = "Giro" });
            list.Add(new RefDto() { text = "Garansi", kode = "Garansi" });
            list.Add(new RefDto() { text = "Rumah Toko/Rumah Kantor", kode = "Rumah Toko/Rumah Kantor" });
            list.Add(new RefDto() { text = "Emas", kode = "Emas" });
            list.Add(new RefDto() { text = "Gudang", kode = "Gudang" });
            list.Add(new RefDto() { text = "Obligasi Negara (ON)", kode = "Obligasi Negara (ON)" });
            list.Add(new RefDto() { text = "Pesawat Udara", kode = "Pesawat Udara" });
            list.Add(new RefDto() { text = "Kapal Laut atau Alat Transportasi Air", kode = "Kapal Laut atau Alat Transportasi Air" });
            list.Add(new RefDto() { text = "Standby L/C (SBLC)", kode = "Standby L/C (SBLC)" });
            list.Add(new RefDto() { text = "Simpanan Berjangka", kode = "Simpanan Berjangka" });
            list.Add(new RefDto() { text = "Kendaraan Bermotor", kode = "Kendaraan Bermotor" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetOwnershipDocuments(DataSourceLoadOptions loadOptions)
        {
            var q = db.ref_ownership_document_temp.Select(x => new { text = x.dm_dokumen_kepemilikan_agunan, kode = x.dm_dokumen_kepemilikan_agunan.Replace("-", "~") }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetStatusPengecekans(DataSourceLoadOptions loadOptions)
        {
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Pengecekan sebelum pencairan", kode = "Pengecekan sebelum pencairan" });
            list.Add(new RefDto() { text = "Pengecekan setelah pencairan", kode = "Pengecekan setelah pencairan" });
            list.Add(new RefDto() { text = "Tidak Dilakukan Pengecekan", kode = "Tidak Dilakukan Pengecekan" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetDQs(DataSourceLoadOptions loadOptions)
        {
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "DA01", kode = "debitur_anomali_format_ktp" });
            list.Add(new RefDto() { text = "DA02", kode = "debitur_anomali_gender" });
            list.Add(new RefDto() { text = "DA03", kode = "debitur_anomali_duplikasi_nama_debitur" });
            list.Add(new RefDto() { text = "DA04", kode = "agunan_id_anomali" });
            list.Add(new RefDto() { text = "DA05", kode = "agunan_dokumen_anomali" });
            list.Add(new RefDto() { text = "DA06", kode = "da_anomali_nilai_agunan_deb" });
            list.Add(new RefDto() { text = "DA07", kode = "da_anomali_nik_deb" });
            list.Add(new RefDto() { text = "DA08", kode = "da_anomali_gelar_nama_deb" });
            list.Add(new RefDto() { text = "DA09", kode = "da_anomali_alamat_deb" });
            list.Add(new RefDto() { text = "DA10", kode = "da_analisis_debtor_nom_identitas_sama" });
            list.Add(new RefDto() { text = "DA11", kode = "da_analisis_debtor_nom_identitas_beda" });
            list.Add(new RefDto() { text = "DA12", kode = "da_anomali_nama_ibu_kandung" });
            list.Add(new RefDto() { text = "DA13", kode = "da_anomali_bentuk_badan_usaha" });
            list.Add(new RefDto() { text = "DA14", kode = "da_anomali_nilai_njop_agunan" });
            list.Add(new RefDto() { text = "DA15", kode = "da_anomali_penghasilan_per_tahun" });
            list.Add(new RefDto() { text = "DA16", kode = "da_anomali_nik_lahir_debitur" });
            list.Add(new RefDto() { text = "DA17", kode = "da_anomali_format_npwp" });
            list.Add(new RefDto() { text = "DA18", kode = "da_anomali_tempat_lahir_debitur" });
            list.Add(new RefDto() { text = "DA19", kode = "da_anomali_baki_debet_tidak_wajar" });
            list.Add(new RefDto() { text = "DA20", kode = "da_anomali_format_telepon_debitur" });
            list.Add(new RefDto() { text = "DA21", kode = "da_anomali_alamat_email_debitur" });
            list.Add(new RefDto() { text = "DA22", kode = "da_anomali_tempat_bekerja_debitur" });
            list.Add(new RefDto() { text = "DA23", kode = "da_anomali_alamat_bekerja_debitur" });
            list.Add(new RefDto() { text = "DA24", kode = "da_anomali_tempat_badan_usaha" });
            list.Add(new RefDto() { text = "DA25", kode = "da_anomali_nomor_akta_badan_usaha" });
            list.Add(new RefDto() { text = "DA26", kode = "da_anomali_format_peringkat_agunan" });
            list.Add(new RefDto() { text = "DA27", kode = "da_anomali_tingkat_suku_bunga" });
            list.Add(new RefDto() { text = "DA28", kode = "da_anomali_bukti_kepemilikan_agunan" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetSukuBungas(DataSourceLoadOptions loadOptions)
        {
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "200% - 500%", kode = " >= 200 AND dm_suku_bunga_atau_imbalan <= 500" });
            list.Add(new RefDto() { text = "501% - 1000%", kode = " >= 501 AND dm_suku_bunga_atau_imbalan <= 1000" });
            list.Add(new RefDto() { text = ">1000%", kode = " > 1000" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetStatusInquirys(DataSourceLoadOptions loadOptions)
        {
            var list = new List<RefDto>();
            list.Add(new RefDto() { text = "Pencairan sebelum inquiry", kode = "Pencairan sebelum inquiry" });
            list.Add(new RefDto() { text = "Pencairan setelah inquiry", kode = "Pencairan setelah inquiry" });
            return Json(DataSourceLoader.Load(list, loadOptions));
        }
        public IActionResult GetProvinsi(DataSourceLoadOptions loadOptions)
        {
            var q = db.ref_cities.Where(x => x.dm_kode_kota.EndsWith("00") == true).Select(x => new { text = x.dm_nama_kota, kode = x.dm_nama_kota }).Distinct();
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        public IActionResult GetKota(DataSourceLoadOptions loadOptions,string prov,bool reset)
        {
            if (reset)
            {
                loadOptions.Skip = 0;
                loadOptions.Take = 0;
            }
            string[] lp = null;
            List<string> kp = new List<string>();
            if (prov != null) {
                lp = prov.Split(",");
            }
            var cty = from q1 in db.ref_cities
                      //where lp.Contains(q1.dm_nama_kota)
                      select q1;
            if (prov != null)
            {
                cty = cty.Where(x => lp.Contains(x.dm_nama_kota));
                foreach (var i in cty) {
                    kp.Add(i.dm_kode_provinsi);
                }
            }
            else {
                cty = cty.Where(x => x.dm_nama_kota==null);
            }
            //var q = db.ref_cities.Where(x => x.dm_kode_provinsi == kodeKota && x.dm_kode_kota != kodeProv).Select(x => new { text = x.dm_nama_kota, kode = x.dm_nama_kota,x.dm_kode_provinsi }).Distinct();
            var q = db.ref_cities.Select(x => new { text = x.dm_nama_kota, kode = x.dm_nama_kota, x.dm_kode_provinsi }).Distinct();
            if (cty.Any())
            {
                q = q.Where(x => kp.Contains(x.dm_kode_provinsi) && x.dm_kode_provinsi.EndsWith("00") == false);
                //kodeKota = cty.FirstOrDefault().dm_kode_provinsi;
                //kodeProv = kodeKota + "00";
            }
            else {
                q = q.Where(x => x.dm_kode_provinsi == null);
            }
            
            return Json(DataSourceLoader.Load(q, loadOptions));
        }
        #endregion
    }
}
