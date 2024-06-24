using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BDA.Models;
using BDA.DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

namespace BDA.Controllers
{
    [Area("Website")]
    public class HomeController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        
        public HomeController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
            
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string ReturnUrl,string errmsg)
        {
            //if (!String.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
            //{
            //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //    HttpContext.Session.Clear();
            //    db.SetSessionString("sctext", "Sukses logout.");
            //    return RedirectToAction("Login");
            //}
            if (!string.IsNullOrWhiteSpace(errmsg)) {
                db.SetSessionString("errtext", errmsg);

            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string user_id, string user_password, string ReturnUrl)
        {
            //var isDelegate = false;
            //if (user_id.ToLower().StartsWith("delegate\\"))
            //{
            //    isDelegate = true;
            //    user_id = user_id.ToLower().Replace("delegate\\", "");
            //}

            try
            {
                UserMaster dataUser = null;

                //challenge cam dolo kalo gagal baru login db normal
                bool isUsingCam = db.GetSetting("CamLogin");
                if (isUsingCam)
                {   
                    //Helper.CAMHelper camHelp = new Helper.CAMHelper(db);
                   // dataUser = camHelp.CAMLoginChallenge(user_id, user_password);
                }
                if (dataUser == null)
                {
                    dataUser = new AuthHelper().AuthenticateUser(db, user_id, user_password);
                }

                if (dataUser != null)
                {
                    //check apakah user punya role
                    var userRole = (from q in db.FWUserRole
                                    join r in db.FWRefRole on q.RoleId equals r.RoleId
                                    where /*r.role_is_internal == true &&*/ q.UserId == dataUser.UserId
                                    select q).ToList();

                    if (userRole.Count() == 0)
                    {
                        throw new InvalidOperationException("User " + dataUser.UserId + " tidak memiliki role.");
                    }
                    else
                    {
                        //check apakah role punya hak akses
                        var roleRight = (from q in db.FWUserRole
                                         join r in db.FWRoleRight on q.RoleId equals r.RoleId
                                         where q.UserId == dataUser.UserId
                                         select q).ToList();
                        if (!roleRight.Any())
                        {
                            throw new InvalidOperationException("Role user " + dataUser.UserId + " tidak memiliki hak akses.");
                        }
                    }
                    var usrMainRole= (from q in db.FWUserRole
                                      join r in db.FWRefRole on q.RoleId equals r.RoleId
                                      orderby q.RoleId
                                      where q.Stsrc=="A" && q.UserId==dataUser.UserId
                                      select r.RoleName);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, dataUser.UserId),
                        new Claim("FullName", dataUser.UserNama),
                        new Claim(ClaimTypes.Email , dataUser.UserEmail),
                        new Claim(ClaimTypes.Role, usrMainRole.FirstOrDefault()),
                    };

                    
                    if (dataUser.SatkerId != null)
                    {
                        claims.Add(new Claim("Satkerid", dataUser.SatkerId.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(db.appSettings.LoginTimeOut)),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        IssuedUtc = DateTimeOffset.UtcNow,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    //if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                    //    && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                    //{
                    //    return Redirect(ReturnUrl);
                    //}
                    //else
                    //{
                    var rawTarget = HttpContext.Features.Get<IHttpRequestFeature>().RawTarget;
                    var originalBody = HttpContext.Response.Body;
                    var RequestLine = $"{HttpContext.Request.Method} {rawTarget} {HttpContext.Request.Protocol}";
                    db.Database.ExecuteSqlCommand("INSERT INTO dbo.LOG_AKSES(RemoteHost, AuthUser, StartDate, RequestLine, StatusCode) VALUES(@p0,@p1,@p2,@p3,@p4)",
                    HttpContext.Connection.RemoteIpAddress.ToString(), dataUser.UserId, DateTime.Now, RequestLine, "200");
                    db.InsertAuditTrail("Login", "Masuk", "Login",null,null,"","",dataUser.UserId);
                    //if (dataUser.UserMainRole == "UserSprint" || dataUser.UserMainRole == "AdminSprint")
                    //    return RedirectToAction("Index", "JumlahPerizinan", new { area = "SPRINT" });
                    //else if (dataUser.UserMainRole == "UserEWSAsuransi" || dataUser.UserMainRole== "AdminEWSAsuransi")
                    //    return RedirectToAction("Index", "Overview", new { area = "Asuransi" });
                    //else if (dataUser.UserMainRole == "DataSteward")
                    //    return RedirectToAction("Index", "Indeks", new { area = "DataSet" });
                    //else if (dataUser.UserMainRole == "AdminAplikasi")
                    //    return RedirectToAction("Index", "User_Master", new { area = "Master" });
                    //else if (dataUser.UserMainRole == "AdminBPRBPRS")
                    //    return RedirectToAction("Index", "BPR", new { area = "Overview" });
                    //else if (dataUser.UserMainRole == "AdminSatkerBPRBPRS")
                    //    return RedirectToAction("Index", "BPR", new { area = "Overview" });
                    //else if (dataUser.UserMainRole == "PengawasBPRBPRS")
                    //    return RedirectToAction("Index", "BPR", new { area = "Overview" });
                    return RedirectToAction("Index", "Default", new { area = "Website" });
                }
                else
                {
                    throw new InvalidOperationException("Please fill user id.");
                }
            }
            catch (Exception ex)
            {
                //var dataUser = db.UserMaster.Find(user_id);
                //if (dataUser != null)
                //{
                //    if (dataUser.UserFailedLoginCount >= 3)
                //    {

                //        var mb = new UserMasterReturnDTO();
                //        mb.ticket_kode = dataUser.UserNama;
                //        mb.ticket_title = "Pemberitahuan Gagal Login";
                //        var getter = new BusinessLayer.EmailGetter(db);
                //        var dict = new Dictionary<string, object>();
                //        //dict.Add("Email", getter.GetTicketEmail(Convert.ToInt32(dataUser.user_id), dataUser.user_email));
                //        dict.Add("Email", dataUser.UserEmail);
                //        dict.Add("Tipe", "Baru");
                //        dict.Add("Pesan", "Password yang anda masukan sudah salah sebanyak 3 x ");
                //        var help = new BusinessLayer.NoticeTemplateHelper(db, "Ticket Post", mb, dict);
                //        var notice = help.GenerateNotice(false);
                //        db.SaveChanges();
                //    }
                //}
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return RedirectToAction("Login", "Home", new { ReturnUrl = ReturnUrl, area = "Website" });
        }
        public class UserMasterReturnDTO
        {
            public string ticket_kode { get; set; }
            public string ticket_title  { get; set; }

    }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
            {
                UserMaster dataUser = db.UserMaster.Find(HttpContext.User.Identity.Name);
                dataUser.LastTimeCookies = null;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                db.InsertAuditTrail("Logout", "Keluar", "Logout");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                db.SetSessionString("sctext", "Sukses logout.");
                return RedirectToAction("Login"); //kalo mau login, biar ga kena blank page
            }
            return RedirectToAction("Login");
        }
    }


}
