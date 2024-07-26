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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace BDA.Controllers
{
    [Area("BDAPM")]
    public class SampleGridController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public SampleGridController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            string pageTitle = currentNode != null ? currentNode.Title : ""; //menampilkan data menu

            db.CheckPermission("Sample View", DataEntities.PermissionMessageType.ThrowInvalidOperationException); //check permission nya view/lihat nya
            ViewBag.Export = db.CheckPermission("Sample Export", DataEntities.PermissionMessageType.NoMessage); //check permission export
            db.InsertAuditTrail("Sample_Akses_Page", "Akses Page Sample", pageTitle); //simpan kedalam audit trail

            return View(SampleData.SimpleArrayCustomers);
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
                var result = Helper.WSQueryStore.GetBDAPMQuery(db, loadOptions, reportId, stringPeriodeAwal, stringPE, stringStatus, cekHive);
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
        public partial class SampleData
        {
            public static readonly IEnumerable<Customer> SimpleArrayCustomers = new[] {
            new Customer {
                ID = 1,
                CompanyName = "Super Mart of the West",
                Address = "702 SW 8th Street",
                City = "Bentonville",
                State = "Arkansas",
                Zipcode = 72716,
                Phone = "(800) 555-2797",
                Fax = "(800) 555-2171",
                Website = "http://www.nowebsitesupermart.dx"
            },
            new Customer {
                ID = 2,
                CompanyName = "Electronics Depot",
                Address = "2455 Paces Ferry Road NW",
                City = "Atlanta",
                State = "Georgia",
                Zipcode = 30339,
                Phone = "(800) 595-3232",
                Fax = "(800) 595-3231",
                Website = "http://www.nowebsitedepot.dx"
            },
            new Customer {
                ID = 3,
                CompanyName = "K&S Music",
                Address = "1000 Nicllet Mall",
                City = "Minneapolis",
                State = "Minnesota",
                Zipcode = 55403,
                Phone = "(612) 304-6073",
                Fax = "(612) 304-6074",
                Website = "http://www.nowebsitemusic.dx"
            },
            new Customer {
                ID = 4,
                CompanyName = "Tom's Club",
                Address = "999 Lake Drive",
                City = "Issaquah",
                State = "Washington",
                Zipcode = 98027,
                Phone = "(800) 955-2292",
                Fax = "(800) 955-2293",
                Website = "http://www.nowebsitetomsclub.dx"
            },
            new Customer {
                ID = 5,
                CompanyName = "E-Mart",
                Address = "3333 Beverly Rd",
                City = "Hoffman Estates",
                State = "Illinois",
                Zipcode = 60179,
                Phone = "(847) 286-2500",
                Fax = "(847) 286-2501",
                Website = "http://www.nowebsiteemart.dx"
            },
            new Customer {
                ID = 6,
                CompanyName = "Walters",
                Address = "200 Wilmot Rd",
                City = "Deerfield",
                State = "Illinois",
                Zipcode = 60015,
                Phone = "(847) 940-2500",
                Fax = "(847) 940-2501",
                Website = "http://www.nowebsitewalters.dx"
            },
            new Customer {
                ID = 7,
                CompanyName = "StereoShack",
                Address = "400 Commerce S",
                City = "Fort Worth",
                State = "Texas",
                Zipcode = 76102,
                Phone = "(817) 820-0741",
                Fax = "(817) 820-0742",
                Website = "http://www.nowebsiteshack.dx"
            },
            new Customer {
                ID = 8,
                CompanyName = "Circuit Town",
                Address = "2200 Kensington Court",
                City = "Oak Brook",
                State = "Illinois",
                Zipcode = 60523,
                Phone = "(800) 955-2929",
                Fax = "(800) 955-9392",
                Website = "http://www.nowebsitecircuittown.dx"
            },
            new Customer {
                ID = 9,
                CompanyName = "Premier Buy",
                Address = "7601 Penn Avenue South",
                City = "Richfield",
                State = "Minnesota",
                Zipcode = 55423,
                Phone = "(612) 291-1000",
                Fax = "(612) 291-2001",
                Website = "http://www.nowebsitepremierbuy.dx"
            },
            new Customer {
                ID = 10,
                CompanyName = "ElectrixMax",
                Address = "263 Shuman Blvd",
                City = "Naperville",
                State = "Illinois",
                Zipcode = 60563,
                Phone = "(630) 438-7800",
                Fax = "(630) 438-7801",
                Website = "http://www.nowebsiteelectrixmax.dx"
            },
            new Customer {
                ID = 11,
                CompanyName = "Video Emporium",
                Address = "1201 Elm Street",
                City = "Dallas",
                State = "Texas",
                Zipcode = 75270,
                Phone = "(214) 854-3000",
                Fax = "(214) 854-3001",
                Website = "http://www.nowebsitevideoemporium.dx"
            },
            new Customer {
                ID = 12,
                CompanyName = "Screen Shop",
                Address = "1000 Lowes Blvd",
                City = "Mooresville",
                State = "North Carolina",
                Zipcode = 28117,
                Phone = "(800) 445-6937",
                Fax = "(800) 445-6938",
                Website = "http://www.nowebsitescreenshop.dx"
            }
        };
        }
        public class Customer
        {
            public int ID { get; set; }
            public string CompanyName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public int Zipcode { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string Website { get; set; }
        }
    }
}
