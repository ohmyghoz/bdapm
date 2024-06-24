using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA.BusinessLayer;
using BDA.DataModel;
using DevExpress.DataAccess.Native.Json;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Test")]
    public class TestingEmailAlertController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public TestingEmailAlertController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(DateTime dtPeriode)
        {
            db.Database.CommandTimeout = 600;
            try
            {
                var listPengawas = db.getPengawasDailyALert(dtPeriode,"Bulanan").ToList().Take(5);
                foreach (var i in listPengawas)
                {
                    var usr = db.UserMaster.Find(i.user_login_id);

                    var d = new AlertEmail();
                    d.tipePeriode = "Daily";
                    d.objDate = dtPeriode.ToString("dd MMM yyyy");

                    d.email_to = usr.UserEmail;
                    d.dateProcess = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
                    d.totalPelapor = db.getTotalPelapor(i.user_login_id).FirstOrDefault().totalPelapor.ToString();
                    if (usr.user_is_notifredalert == true)
                    {
                        d.levelAlert = "Red";
                        var listData = db.GetDataAlert(i.user_login_id, "3", dtPeriode,"Bulanan",Base64Encode(i.user_login_id)).ToList();
                        if (listData.Count() != 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (Html.Table table = new Html.Table(sb, id: "some-id"))
                            {
                                table.StartHead();
                                using (var thead = table.AddRow())
                                {
                                    thead.AddCell("No");
                                    thead.AddCell("Tipe");
                                    thead.AddCell("Jumlah Pelapor");
                                    thead.AddCell("Jumlah Alert");
                                    thead.AddCell("Total Permintaan");
                                    thead.AddCell("Individu");
                                    thead.AddCell("Non Individu");
                                    thead.AddCell("Batch");
                                    thead.AddCell("Interactive");
                                }
                                table.EndHead();
                                table.StartBody();
                                int no = 1;
                                int totALert = 0;
                                foreach (var alert in listData)
                                {

                                    using (var tr = table.AddRow(classAttributes: "someattributes"))
                                    {
                                        tr.AddCell(no.ToString());
                                        tr.AddCell(alert.tipe);
                                        tr.AddCell(alert.jumlahPelapor.ToString());
                                        tr.AddCell(alert.jumlahAlert.ToString());
                                        tr.AddCell(alert.tp);
                                        tr.AddCell(alert.ind);
                                        tr.AddCell(alert.nind);
                                        tr.AddCell(alert.bat);
                                        tr.AddCell(alert.inte);
                                    }
                                    no = no + 1;
                                    totALert = totALert + Convert.ToInt32(alert.jumlahAlert);
                                }
                                table.EndBody();
                                d.htmlTable = sb.ToString();
                                d.TotalAlert = totALert.ToString();
                                var nott = new NoticeTemplateHelper(db, "Alert Email", d);
                                nott.GenerateNotice(false);
                            }
                        }
                    }
                    if (usr.user_is_notifyellowalert == true)
                    {
                        d.levelAlert = "Yellow";
                        var listData = db.GetDataAlert(i.user_login_id, "2", dtPeriode, "Bulanan", Base64Encode(i.user_login_id)).ToList();
                        if (listData.Count() != 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (Html.Table table = new Html.Table(sb, id: "some-id"))
                            {
                                table.StartHead();
                                using (var thead = table.AddRow())
                                {
                                    thead.AddCell("No");
                                    thead.AddCell("Tipe");
                                    thead.AddCell("Jumlah Pelapor");
                                    thead.AddCell("Jumlah Alert");
                                    thead.AddCell("Total Permintaan");
                                    thead.AddCell("Individu");
                                    thead.AddCell("Non Individu");
                                    thead.AddCell("Batch");
                                    thead.AddCell("Interactive");
                                }
                                table.EndHead();
                                table.StartBody();
                                int no = 1;
                                int totALert = 0;
                                foreach (var alert in listData)
                                {

                                    using (var tr = table.AddRow(classAttributes: "someattributes"))
                                    {
                                        tr.AddCell(no.ToString());
                                        tr.AddCell(alert.tipe);
                                        tr.AddCell(alert.jumlahPelapor.ToString());
                                        tr.AddCell(alert.jumlahAlert.ToString());
                                        tr.AddCell(alert.tp);
                                        tr.AddCell(alert.ind);
                                        tr.AddCell(alert.nind);
                                        tr.AddCell(alert.bat);
                                        tr.AddCell(alert.inte);
                                    }
                                    no = no + 1;
                                    totALert = totALert + Convert.ToInt32(alert.jumlahAlert);
                                }
                                table.EndBody();
                                d.htmlTable = sb.ToString();
                                d.TotalAlert = totALert.ToString();
                                var nott = new NoticeTemplateHelper(db, "Alert Email", d);
                                nott.GenerateNotice(false);
                            }
                        }
                    }
                }
                db.SaveChanges();

                db.SetSessionString("sctext", "Berhasil mengirim email");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
