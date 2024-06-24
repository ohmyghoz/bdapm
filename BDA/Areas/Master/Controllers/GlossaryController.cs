using BDA.DataModel;
using BDA.Helper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BDA.Controllers
{
    [Area("Master")]
    public class GlossaryController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public GlossaryController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var user = HttpContext.User.Identity.Name;

            db.CheckPermission("Glossary View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Edit = db.CheckPermission("Glossary Edit", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("Glossary_Akses_Page", "Akses Page Glossary", pageTitle);

            return View();
        }

        public IActionResult Edit(long? id)
        {
            if (id == null) return BadRequest();

            var obj = (from q1 in db.Glossary
                       join q2 in db.Alert_Master on q1.GloDimensi1 equals q2.KODE_ALERT
                       where q1.stsrc == "A" && q1.GloId == id
                       select new GlossaryDTO
                       {
                           GloId = q1.GloId,
                           GloTipe = q1.GloTipe,
                           GloPIdeb = q1.GloPIdeb,
                           GloDimensi1 = q1.GloDimensi1,
                           GloDimensi2 = q1.GloDimensi2,
                           GloKetNilai1 = q1.GloKetNilai1,
                           GloKetNilaiRata2 = q1.GloKetNilaiRata2,
                           GloNamaAlert = q2.NAMA_ALERT
                       }).FirstOrDefault();

            if (obj == null) return NotFound();

            return View("Edit", obj);
        }

        [HttpPost]
        public IActionResult Edit([Bind("GloId", "GloTipe", "GloPIdeb", "GloDimensi1", "GloDimensi2", "GloKetNilai1", "GloKetNilaiRata2", "GloNamaAlert")] GlossaryDTO obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Glossary Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var oldObj = db.Glossary.Find(obj.GloId);

                WSMapper.CopyFieldValues(obj, oldObj, "GloKetNilai1,GloKetNilaiRata2"); /*Yang bisa diedit hanya Keterangan Nilai1 dan Keterangan Nilai Rata-rata*/

                db.SetStsrcFields(oldObj);
                db.SaveChanges();
                db.InsertAuditTrail("Glossary_UbahGlossary", "Edit Glossary", pageTitle, new object[] { obj }, new string[] { "GloId" }, oldObj.GloId.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View(obj);
        }

        #region "GetGridData"
        [HttpGet]
        public object GetGridData(DataSourceLoadOptions options)
        {
            var query = (from q1 in db.Glossary
                         join q2 in db.Alert_Master on q1.GloDimensi1 equals q2.KODE_ALERT
                         where q1.stsrc == "A"
                         select new
                         {
                             q1.GloId,
                             q1.GloTipe,
                             q1.GloPIdeb,
                             q1.GloDimensi1,
                             q1.GloDimensi2,
                             q1.GloKetNilai1,
                             q1.GloKetNilaiRata2,
                             q2.NAMA_ALERT
                         });

            return DataSourceLoader.Load(query, options);
        }
        #endregion
    }
}

namespace BDA.Helper
{
    public class GlossaryDTO
    {
        public long GloId { get; set; }
        public string GloTipe { get; set; }

        [Display(Name = "Permintaan IDeb")]
        public string GloPIdeb { get; set; }
        public string GloDimensi1 { get; set; }
        public string GloDimensi2 { get; set; }

        [Display(Name = "Keterangan Jumlah Permintaan")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string GloKetNilai1 { get; set; }

        [Display(Name = "Keterangan Nilai Rata-rata")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string GloKetNilaiRata2 { get; set; }

        [Display(Name = "Nama Alert")]
        public string GloNamaAlert { get; set; }
    }
}