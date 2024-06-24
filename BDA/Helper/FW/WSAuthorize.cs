using BDA.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;


namespace BDA.Helper
{

    public class WSAuthorizeRequirement : IAuthorizationRequirement
    {
        public string Roles { get; }

        public WSAuthorizeRequirement(string roles = null)
        {
            Roles = roles;
        }

    }
    public class WSAuthorizationHandler : AuthorizationHandler<WSAuthorizeRequirement>
    {
        private DataEntities db;
        private AppSettings appSet;
        public WSAuthorizationHandler(DataEntities db, AppSettings appSet)
        {
            this.db = db;
            this.appSet = appSet;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WSAuthorizeRequirement requirement)
        {

            if (context.Resource is AuthorizationFilterContext)
            {
                var ctx = (AuthorizationFilterContext)context.Resource;
                //var intention = ctx.HttpContext.Request.Method.AsIntention();
                var roles = "*";
                var resource = ctx.RouteData.Values["controller"].ToString();
                var path = ctx.HttpContext.Request.Path;
                //var permission = $"{resource}:{intention}".ToLowerInvariant();


                if (String.IsNullOrWhiteSpace(requirement.Roles))
                {
                    //var mnu = new BDA.Models.MenuModels(db);
                    var mnu = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());

                    var curNode = mnu.GetCurrentNode();
                    if (curNode == null)
                    {
                        //kalau tidak ketemu, pakai appsetting
                        roles = appSet.SuperAdminRolesCSV;
                        //context.Succeed(requirement);
                        //return Task.CompletedTask;
                    }
                    else
                    {
                        roles = curNode.Roles;
                    }
                }
                else
                {
                    roles = requirement.Roles;
                }

                //buat cek kalo akses base urlnya -> akan redirect ke home..
                string defaultURL = Convert.ToString(db.GetSetting("Link_Prefix")).ToLower();
                defaultURL = defaultURL.Replace("https", "http");
                string repxUrl = Convert.ToString(db.GetSetting("Link_Prefix")).ToLower() + "/dxxrdv";
                repxUrl = repxUrl.Replace("https", "http");
                string linkBrowser = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower();
                linkBrowser = linkBrowser.Replace("https", "http");

                //* atau kosong = semua hak akses boleh, kalo index web boleh, kalo repx juga boleh
                //if (roles.Trim() == "*" || roles.Trim() == "" || linkBrowser.Replace("/", "") == defaultURL.Replace("/", "")
                //    || linkBrowser.Substring(0, repxUrl.Length > linkBrowser.Length ? linkBrowser.Length : repxUrl.Length) == repxUrl)
                //{
                //    context.Succeed(requirement);
                //    return Task.CompletedTask;
                //}
                //else if (linkBrowser.ToLower() == defaultURL + "/website/home/logout") //ditambahkan biar ga cek permission saat mau logout
                //{
                //    context.Succeed(requirement);
                //    return Task.CompletedTask;
                //}
                if (roles.Trim() == "*" || roles.Trim() == "" || roles.Trim()=="AdminAplikasi")
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                else if (linkBrowser.ToLower() == defaultURL + "/website/home/logout") //ditambahkan biar ga cek permission saat mau logout
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                try
                {
                    db.CheckPermission(roles, msg: DataEntities.PermissionMessageType.ThrowInvalidOperationException);
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    db.SetSessionString("errtext", ex.Message);
                }               
            }

            return Task.CompletedTask;
        }
    }
   
    //public class WSAuthorize : AuthorizeAttribute
    //{

    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        var roles = "*";

    //        if (String.IsNullOrWhiteSpace(this.Roles)) //kalau authorize nya default (dari global filter)
    //        {
    //            var mnu = new SIMODISWEB.Models.MenuModels();
    //            var curNode = mnu.GetCurrentNode();
    //            if (curNode == null)
    //            {
    //                return true;
    //            }
    //            roles = curNode.Roles;
    //        }
    //        else
    //        {
    //            roles = this.Roles;
    //        }

    //        if (roles.Trim() == "*" || roles.Trim() == "") //* atau kosong = semua hak akses boleh
    //        {
    //            return true;
    //        }
    //        return DataModel.PermissionHelper.CheckPermission(roles, msg: DataModel.PermissionHelper.PermissionMessageType.NoMessage);

    //    }

    //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    //    {
    //        if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
    //        {
    //            filterContext.Result = new HttpUnauthorizedResult();
    //        }
    //        else
    //        {

    //            filterContext.Result = new RedirectToRouteResult(new
    //                RouteValueDictionary(new { controller = "ErrorHandler", area = "", msg = "Anda tidak memiliki hak akses" }));
    //        }
    //    }
    //}
}