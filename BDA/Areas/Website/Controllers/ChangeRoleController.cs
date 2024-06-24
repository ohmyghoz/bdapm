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
using System.Web;
using Microsoft.AspNetCore.Authorization;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;

namespace NewAcerTravel.Controllers
{
    [AllowAnonymous]
    [Area("Website")]
    public class ChangeRoleController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public ChangeRoleController(DataEntities db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        [HttpGet]
        public object GetRoleUser(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.User.Identity.Name;

            var query = from q in db.FWUserRole
                        join s in db.FWRefRole on q.RoleId equals s.RoleId
                        where q.UserId == userId && q.Stsrc=="A" && s.Stsrc=="A"  
                        select new { q.UroleId,role_id=s.RoleName };

            return DataSourceLoader.Load(query, loadOptions);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChangeRole()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role) != null)
            {
                ViewBag.roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            }
            else {
                ViewBag.ClosePopup = "Session already finished";
            }
            return View("ChangeRole");
        }

        [HttpPost]
        public ActionResult ChangeRole(string role_id)
        {
            try
            {
                var userId = HttpContext.User.Identity.Name;
                var usr = db.UserMaster.Find(userId);
                //var satkerId = HttpContext.User.FindFirst("satker_id");
                var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(ClaimTypes.Name, userId),
                        new System.Security.Claims.Claim(ClaimTypes.Role, role_id),
                        new System.Security.Claims.Claim("FullName", usr.UserNama),
                        new System.Security.Claims.Claim(ClaimTypes.Email , usr.UserEmail),
                        new System.Security.Claims.Claim("CurrentRoleName", role_id),
                    };

                //claims.Add(new Claim("satker_id", satkerId.Value));
                var claimsIdentity = new ClaimsIdentity(
                                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {

                };

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                ViewBag.ClosePopup = "Success change role";
                return View();
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
                ViewBag.roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                return View();
            }
        }
    }
}
