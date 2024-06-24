using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using BDA.DataModel;
using System;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.IO;
using Aspose.Cells;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;
using System.Security.Claims;

namespace BDA.Controllers
{
    [Area("Master")]
    public class User_MasterController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public User_MasterController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string title = null;
            if (currentNode != null) { title = currentNode.Title; }
            if (ViewBag.HeaderTitle != null) { title = ViewBag.HeaderTitle; }

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var user = HttpContext.User.Identity.Name;

            ViewBag.id = "User_Master";
            ViewBag.Export = true; // TODO ubah permission disini

            db.CheckPermission("Master User View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Add = db.CheckPermission("Master User Add", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Edit = db.CheckPermission("Master User Edit", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Delete = db.CheckPermission("Master User Delete", DataEntities.PermissionMessageType.NoMessage);

            db.InsertAuditTrail("MasterUser_Akses_Page", "Akses Page Master User", pageTitle);
            return View();
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
                //db.CheckPermission("MA Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (reportId == "User_Master")
                {
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }

                db.InsertAuditTrail("ExportIndex_MU_" + reportId, "Export Data", pageTitle);
                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = db.ProcessExceptionMessage(ex) });
            }
        }

        public IActionResult ExportPDF(string reportId, IFormFile file)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                //TODO : tambah permission
                if (reportId == "User_Master")
                {
                    //db.CheckPermission("CM01 Export", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                }

                db.InsertAuditTrail("ExportIndex_MU_" + reportId, "Export Data", pageTitle);

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
                var fileName = "MU_" + reportId + "_" + timeStamp + ".pdf";
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
            var fileName = "MU_" + reportId + "_" + timeStamp + ".pdf";
            var filePath = Path.Combine(directory, fileName);
            var fileByte = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileByte, "application/pdf", fileName);
        }

        public IActionResult Create()
        {
            //ViewBag.Mode = "Create";
            ViewBag.IsEdit = false;
            ViewBag.IsLdap = false;
            var obj = new UserMaster();
            return View("Edit", obj);
        }

        [HttpPost]
        public IActionResult Create([Bind("UserId", "UserNama", "UserPassword", "UserEmail", "UserMainRole", "user_is_notifredalert", "user_is_notifyellowalert")] UserMaster obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master User Add", DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                if (string.IsNullOrWhiteSpace(obj.UserPassword))
                {
                    throw new InvalidOperationException("Password harus diisi");
                }
                /*insert UserMaster*/
                var oldUser = db.UserMaster.Where(x => x.UserId == obj.UserId).ToList();
                if (oldUser.Any()) /*check kalau ada/pernah ada user dengan id yang sama */
                {
                    if (oldUser.FirstOrDefault().Stsrc == "A")
                    {
                        throw new InvalidOperationException("User ID sudah terdaftar, gunakan User ID lain");
                    }
                    else
                    {
                        var oldObj = oldUser.FirstOrDefault();
                        WSMapper.CopyFieldValues(obj, oldObj, "UserId,UserNama,UserEmail,UserMainRole,user_is_notifredalert,user_is_notifyellowalert");
                        //oldObj.UserNama = obj.UserNama;
                        //oldObj.UserPassword = LibFunction.HashPasswordSHA256(obj.UserPassword);
                        //oldObj.UserEmail = obj.UserEmail;
                        //oldObj.user_is_notifredalert = obj.user_is_notifredalert;
                        //oldObj.user_is_notifyellowalert = obj.user_is_notifyellowalert;
                        oldObj.UserStatus = "Aktif";
                        oldObj.Stsrc = "A";
                        db.SetStsrcFields(oldObj);

                        var oldUserRole = db.FWUserRole.FirstOrDefault(x => x.UserId == oldObj.UserId);
                        oldUserRole.UserMaster = oldObj;
                        oldUserRole.RoleId = db.FWRefRole.FirstOrDefault(x => x.RoleName == obj.UserMainRole).RoleId;
                        oldUserRole.Stsrc = "A";
                        db.SetStsrcFields(oldUserRole);
                    }
                }
                else 
                {
                    db.UserMaster.Add(obj);
                    //obj.UserKode = obj.UserId; /*UserKode untuk apa?*/
                    //obj.UserLdap = obj.UserId; /*UserLdap untuk apa?*
                    obj.UserPassword = LibFunction.HashPasswordSHA256(obj.UserPassword);
                    obj.UserStatus = "Aktif";
                    db.SetStsrcFields(obj);

                    /*insert FWUserRole*/
                    var userRole = new FWUserRole(); /*1 user bisa multiple role atau hanya 1?*/
                    db.FWUserRole.Add(userRole);
                    userRole.UserMaster = obj;
                    userRole.RoleId = db.FWRefRole.FirstOrDefault(x => x.RoleName == obj.UserMainRole).RoleId;
                    db.SetStsrcFields(userRole);
                }

                db.SaveChanges();
                db.InsertAuditTrail("UserMaster_TambahUser", "Tambah User", pageTitle, new object[] { obj }, new string[] { "UserId" }, obj.UserId.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
                obj.UserPassword = null;
                ViewBag.IsEdit = false;
                ViewBag.IsLdap = false;
            }
            return View("Edit", obj);
        }

        public IActionResult CreateLDAP()
        {
            var obj = new UserMaster();
            ViewBag.IsEdit = false;
            ViewBag.IsLdap = true;
            TempData["idlldap"] = null;
            return View("EditLDAP", obj);
        }

        [HttpPost]
        public IActionResult CreateLDAP([Bind("UserId", "UserMainRole","UserEmail", "user_is_notifredalert", "user_is_notifyellowalert")] UserMaster obj, string[] listRole)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                /*perlu check cookies?*/
                var user = HttpContext.User.Identity.Name;
                var checkUserCookies = db.UserMaster.Find(user);
                if (checkUserCookies.LastTimeCookies  == null)
                {
                    throw new InvalidOperationException("Anda tidak berhak mengakses");
                }

                /*mulai insert UserMaster*/
                var usr = new UserMaster();
                var a = obj.UserId + "@ojk.go.id";
                var checkUser = from q in db.UserMaster
                                where q.Stsrc == "A" && q.UserId == a
                                select q;
                if (checkUser.Any())
                {
                    throw new InvalidOperationException("User " + obj.UserId + " sudah terdaftar");
                }
                string sDomainLDAP = db.GetSetting("DomainLDAP");
                //var checkLDAP = SearchLDAP("corp.winning-soft.com", "CN=Users,DC=corp,DC=.winning-soft,DC=com", obj.UserId, false);
                var checkLDAP = SearchLDAP(sDomainLDAP, "CN=Users,OU=Permintaan Khusus,DC=corp,DC=ojk,DC=go,DC=id", obj.UserId, false);
                if (!checkLDAP.Any())
                {
                    throw new InvalidOperationException("Data LDAP " + obj.UserId + " tidak ditemukan");
                }
                else
                {
                    usr.UserNama = checkLDAP.FirstOrDefault().Displaynama;
                }
                var check2 = from q in db.UserMaster
                             where q.Stsrc == "D" && q.UserId == a
                             select q;
                if (check2.Any()) {
                    usr = db.UserMaster.Find(a);
                    usr.Stsrc="A";
                }
                string lr = "";
                foreach (var i in listRole) {
                    if (lr == "")
                    {
                        lr = i;
                    }
                    else {
                        lr = lr + "," + i;
                    }
                }
                usr.UserStatus = "Aktif";
                usr.UserPassword = "XXX";
                usr.UserId = obj.UserId + "@ojk.go.id";
                usr.UserLdap = obj.UserId;
                usr.UserMainRole = lr;
                usr.UserEmail = obj.UserEmail;

                string[] roleList = usr.UserMainRole.Split(",");
                foreach (var i in roleList)
                {
                    string x = i.Trim();

                    FWUserRole roler =  new FWUserRole();
                    db.FWUserRole.Add(roler);
                    roler.UserMaster = usr;

                    
                    roler.RoleId = (from q in db.FWRefRole where q.Stsrc=="A" && q.RoleName==x
                                   select q.RoleId).FirstOrDefault(); 
                    db.SetStsrcFields(roler);
                }

                //var urs = new FWUserRole();
                //urs.RoleId = db.FWRefRole.FirstOrDefault(x => x.RoleName == obj.UserMainRole).RoleId;
                //urs.UserId = usr.UserId;
                if (check2.Any())
                {

                }
                else {
                    db.UserMaster.Add(usr);
                }
                
                db.SetStsrcFields(usr);

                //db.FWUserRole.Add(urs);
                //db.SetStsrcFields(urs);

                db.SaveChanges();
                db.InsertAuditTrail("UserMaster_TambahUser", "Tambah User LDAP", pageTitle, new object[] { obj }, new string[] { "UserId" }, obj.UserId.ToString());
                db.SetSessionString("sctext", "Sukses menyimpan data");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
                ViewBag.IsEdit = false;
                ViewBag.IsLdap = true;
            }
            return View("EditLDAP", obj);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.Mode = "Edit";
            if (id == null) return BadRequest();

            var obj = db.UserMaster.Find(id);
            if (obj == null) return NotFound();
            if (obj.Stsrc != "A") return NotFound();
            var usrMainRole= (from q in db.FWUserRole
                              join r in db.FWRefRole on q.RoleId equals r.RoleId
                              where q.Stsrc=="A" && q.UserId==id
                              select r.RoleName).ToList();
            obj.UserMainRole = string.Join(",", usrMainRole);
            obj.UserPassword = null;
            ViewBag.IsEdit = true;
            if (obj.UserLdap != null)
            {
                ViewBag.IsLdap = true;
            }
            else {
                ViewBag.IsLdap = false;
            }
            
            return View("Edit", obj);
        }

        [HttpPost]
        public IActionResult Edit([Bind("UserId", "UserNama", "UserPassword", "UserEmail", "UserMainRole", "user_is_notifredalert", "user_is_notifyellowalert")] UserMaster obj, string[] listRole)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Master User Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var oldObj = db.UserMaster.Find(obj.UserId);

                
                //obj.UserPassword = LibFunction.HashPasswordSHA256(obj.UserPassword);

                WSMapper.CopyFieldValues(obj, oldObj, "UserId,UserNama,UserEmail,UserMainRole,user_is_notifredalert,user_is_notifyellowalert"); /*UserId bisa di-edit?*/

                //oldObj.UserKode = obj.UserId; /*UserKode untuk apa?*/
                //oldObj.UserLdap = obj.UserId; /*UserLdap untuk apa?*/
                if (!string.IsNullOrWhiteSpace(obj.UserPassword))
                {
                    oldObj.UserPassword = LibFunction.HashPasswordSHA256(obj.UserPassword);
                }
                string lr = "";
                foreach (var i in listRole)
                {
                    if (lr == "")
                    {
                        lr = i;
                    }
                    else
                    {
                        lr = lr + "," + i;
                    }
                }
                oldObj.UserMainRole = lr;
                db.SetStsrcFields(oldObj);

                /*edit FWUserRole*/
                //string[] roleList = oldObj.UserMainRole.Split(",");
                var toBeDeletedList = (from q in db.FWUserRole where q.Stsrc == "A" && q.UserId == oldObj.UserId select q).ToList();
                foreach (var a in listRole)
                {
                    var x = a.Trim();
                    var r = (from q in db.FWRefRole
                             where q.Stsrc == "A" && q.RoleName == x
                             select q.RoleId).FirstOrDefault();
                    var query = from q in toBeDeletedList where q.RoleId == r select q;

                    FWUserRole roler = null;
                    if (query.Count() > 0)
                    {
                        roler = query.First();
                        toBeDeletedList.Remove(roler);
                    }
                    else
                    {
                        roler = new FWUserRole();
                        db.FWUserRole.Add(roler);
                        roler.UserMaster = oldObj;
                    }
                    roler.RoleId = r;
                    db.SetStsrcFields(roler);
                }
                foreach (var x in toBeDeletedList)
                {
                    db.DeleteStsrc(x);
                }

                //var oldUserRole = db.FWUserRole.FirstOrDefault(x => x.UserId == obj.UserId);
                //if (oldUserRole != null)
                //{
                //    oldUserRole.RoleId = db.FWRefRole.FirstOrDefault(x => x.RoleName == obj.UserMainRole).RoleId;
                //    db.SetStsrcFields(oldUserRole);
                //}
                //else
                //{
                //    var newUserRole = new FWUserRole();
                //    newUserRole.RoleId = db.FWRefRole.FirstOrDefault(x => x.RoleName == obj.UserMainRole).RoleId;
                //    newUserRole.UserId = oldObj.UserId;
                //    db.FWUserRole.Add(newUserRole);
                //    db.SetStsrcFields(newUserRole);
                //}

                db.SaveChanges();
                db.InsertAuditTrail("UserMaster_UbahUser", "Ubah User", pageTitle, new object[] { obj }, new string[] { "UserId" }, obj.UserId.ToString());
                db.SetSessionString("sctext", "Berhasil menyimpan data");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                obj.UserPassword = null;
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
                ViewBag.IsEdit = true;
                if (obj.UserLdap != null)
                {
                    ViewBag.IsLdap = true;
                }
                else
                {
                    ViewBag.IsLdap = false;
                }
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(string param1)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Master User Delete", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            var obj = db.UserMaster.FirstOrDefault(o => o.UserId == param1);

            var userRole = db.FWUserRole.FirstOrDefault(o => o.UserId == obj.UserId);

            db.DeleteStsrc(userRole);
            db.DeleteStsrc(obj);
            db.SaveChanges();

            db.InsertAuditTrail("UserMaster_HapusUser", "Hapus User", pageTitle, new object[] { obj }, new string[] { "UserId" }, obj.UserId.ToString());
            var resp = "Berhasil menghapus data";
            return new JsonResult(resp);
        }

        private List<Helper.LDAPUserDTO> SearchLDAP(string domain, string ou, string username, bool isGrid, string extraFilter = "(objectCategory=user)")
        {
            //string whitelist = @"^[a-zA-Z1-9\-\.@_']*$";
            username = username.Trim();
            string hakname = db.GetSetting("DomainLDAP_User");
            string pass = db.GetSetting("DomainLDAP_Pass");
            
            //var hakname = "yoke.p";
            //var pass = "a123456!";
            //Regex pattern = new Regex(whitelist);
            //if (!pattern.IsMatch(username))
            //{
            //    throw new InvalidOperationException("User ID hanya dapat terdiri atas angka dan huruf");
            //}
            //else
            //{
                using (DirectoryEntry de = new DirectoryEntry(("LDAP://" + domain), (hakname), pass))
                {
                    using (DirectorySearcher search = new DirectorySearcher(de))
                    {
                        try
                        {
                            search.PropertiesToLoad.Add("SAMAccountName");
                            search.PropertiesToLoad.Add("displayname");
                            if (isGrid == true)
                            {
                                search.Filter = ("(&" + extraFilter + "(SAMAccountName=*" + username + "*))");
                            }
                            else
                            {
                                search.Filter = ("(&" + extraFilter + "(SAMAccountName=" + username + "))");
                            }
                            SearchResultCollection result = search.FindAll();
                            var asd = result.Cast<SearchResult>().Select(sr => sr.GetDirectoryEntry());
                            var users = result.Cast<SearchResult>().Select(sr => sr.GetDirectoryEntry()).Select(a => new
                            {
                                userid = a.Properties["SAMAccountName"] != null ? a.Properties["SAMAccountName"].Value.ToString() : "",
                                //displayname = a.Properties["Name"] != null ? a.Properties["Name"].Value.ToString() : ""
                                displayname = a.Properties["displayname"] != null ? a.Properties["displayname"].Value.ToString() : "",
                               // mail = a.Properties["mail"] != null ? a.Properties["mail"].Value.ToString() : ""
                            }).ToList();
                            var models = new List<Helper.LDAPUserDTO>();
                            foreach (var i in users)
                            {
                                models.Add(new Helper.LDAPUserDTO { Userid = i.userid, Displaynama = i.displayname});
                            }
                            //DataGridView1.DataSource = users;
                            if ((users != null))
                                return models;
                            else
                                models.Add(new Helper.LDAPUserDTO { Userid = "-", Displaynama = "-" });
                            return null;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
            //}
        }

        [HttpGet]
        public IActionResult AddLDAP (string id)
        {
            var obj = new UserMaster();
            TempData["useldapcou"] = 0;
            TempData["idlldap"] = null;
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    string whitelist = @"^[a-zA-Z1-9\-\.@_']*$";
                    var username = id.Trim();
                    Regex pattern = new Regex(whitelist);
                    if (!pattern.IsMatch(username))
                    {
                        throw new InvalidOperationException("User ID hanya dapat terdiri atas angka dan huruf");
                    }
                    string sDomainLDAP = db.GetSetting("DomainLDAP");
                    //var CekLDAP = SearchLDAP("corp.winning-soft.com", "CN=Users,DC=corp,DC=.winning-soft,DC=com", id, true);
                    var CekLDAP = SearchLDAP(sDomainLDAP, "CN=Users,OU=Permintaan Khusus,DC=corp,DC=ojk,DC=go,DC=id", id, true);
                    if (CekLDAP != null)
                    {
                        TempData["idlldap"] = id;
                        TempData["useldapcou"] = CekLDAP.Count();
                        obj.UserId = "";
                    }
                    else
                    {
                        db.SetSessionString("errtext", "Data Tidak Ditemukan");
                    }
                }
                catch (Exception ex)
                {
                    db.SetSessionString("errtext", db.ProcessExceptionMessage(ex)); ;
                }
            }
            return View("edit", obj);
        }

        [HttpGet]
        public FileResult ExportCSV(string reportId)
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();
            var user = User.Identity.Name;

            string pageTitle = currentNode != null ? currentNode.Title : "";

            var query = from q in db.vw_ListUser
                        select new
                        {
                            q.UserId,
                            q.UserNama,
                            q.UserTelp,
                            q.UserAlamat,
                            q.RoleName,
                            q.UserEmail,
                            q.user_is_notifredalert,
                            q.user_is_notifyellowalert
                        };

            var data = query.ToList(); // Execute the query

            var csv = new StringBuilder();
            csv.AppendLine("UserId,UserNama,UserTelp,UserAlamat,RoleName,UserEmail,user_is_notifredalert,user_is_notifyellowalert");

            foreach (var item in data)
            {
                csv.AppendLine($"{item.UserId},{item.UserNama},{item.UserTelp},{item.UserAlamat},{item.RoleName},{item.UserEmail},{item.user_is_notifredalert},{item.user_is_notifyellowalert}");
            }

            db.InsertAuditTrail("ExportCSV_MU_UserMaster", "Export Data", pageTitle);

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", user + "_" + pageTitle + "_" + DateTime.Now + ".csv");
        }

        #region "GetGridData"
        [HttpGet]
        public object GetGridData(DataSourceLoadOptions options)
        {
            //var query = from q1 in db.UserMaster
            //            join q2 in db.FWUserRole on q1.UserId equals q2.UserId
            //            join q3 in db.FWRefRole on q2.RoleId equals q3.RoleId
            //            where q1.Stsrc == "A"
            //            select new {
            //                q1.UserId,
            //                q1.UserNama,
            //                q1.UserEmail,
            //                q1.UserAlamat,
            //                q1.UserTelp,
            //                q3.RoleName,q1.user_is_notifredalert,q1.user_is_notifyellowalert
            //            };

            //var query = (from q1 in db.UserMaster
            //             join a in db.FWUserRole on q1.UserId equals a.UserId into temp1
            //             from q2 in temp1.DefaultIfEmpty()
            //             join b in db.FWRefRole on q2.RoleId equals b.RoleId into temp2
            //             from q3 in temp2.DefaultIfEmpty()
            //             where q1.Stsrc == "A"
            //             select new
            //             {
            //                 q1.UserId,
            //                 q1.UserNama,
            //                 q1.UserEmail,
            //                 q1.UserAlamat,
            //                 q1.UserTelp,
            //                 q3.RoleName,
            //                 q1.user_is_notifredalert,
            //                 q1.user_is_notifyellowalert
            //             });
            var query = from q in db.vw_ListUser
                        select new
                        {
                            q.UserId,
                            q.UserNama,
                            q.UserTelp,
                            q.UserAlamat,
                            q.RoleName,
                            q.UserEmail,
                            q.user_is_notifredalert,
                            q.user_is_notifyellowalert
                        };
            return DataSourceLoader.Load(query, options);
        }

        [HttpGet]
        public IActionResult GetUserIdLDAP(DataSourceLoadOptions loadOptions, string user)
        {
            if (!string.IsNullOrWhiteSpace(user))
            {
                if (user.Length < 3)
                {
                    throw new InvalidOperationException("Minimal 3 karakter untuk perncarian");
                }
            }

            var list = new List<Helper.LDAPUserDTO>();

            if (!string.IsNullOrWhiteSpace(user))
            {
                string sDomainLDAP = db.GetSetting("DomainLDAP");
                list = SearchLDAP(sDomainLDAP, "CN=Users,DC=corp,DC=.winning-soft,DC=com", user, true);
                //list = SearchLDAP(sDomainLDAP, "CN=Users,OU=Permintaan Khusus,DC=corp,DC=ojk,DC=go,DC=id",  user, true);
                if (list == null) {
                    list = new List<Helper.LDAPUserDTO>();
                }
                if (!list.Any())
                {
                    list = new List<Helper.LDAPUserDTO>();
                }
            }
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(list, loadOptions)), "application/json");
        }
        #endregion


        #region "RefGetter"
        [HttpGet]
        public IActionResult GetRole(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWRefRole
                        where q.Stsrc == "A"
                        select new { q.RoleId, q.RoleName };
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(query, loadOptions)), "application/json");
        }
        #endregion
    }
}

namespace BDA.Helper
{
    public class LDAPUserDTO
    {
        public string Userid { get; set; }
        public string Displaynama { get; set; }
        public string kelsatker { get; set; }
        public string mail { get; set; }
    }
}