using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
//using System.Web.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BDA.DataModel;
using System.Net;

namespace BDA.Helper
{
    public class CheckCookies
    {

        public static void CheckValidCookiesLogin(DataEntities db, AppSettings appSet)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            if (db.httpContext.User.Identity.Name != "")
            {
                string userID = db.httpContext.User.Identity.Name.ToString();
                var listUser = (from q in db.UserMaster where q.UserId == userID select q);

                if (listUser.Count() > 0)
                {
                    UserMaster datauser = listUser.First();
                    bool kick = false;
                    bool resetWaktu = false;
                    string ipaddress = "";

                    ipaddress = "RemoteIP : " + db.httpContext.Connection.RemoteIpAddress.ToString() + " - LocalIP : " + db.httpContext.Connection.LocalIpAddress.ToString();
                    if (!string.IsNullOrWhiteSpace(db.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"]))
                    {
                        if (ipaddress != "") { ipaddress += " - "; }
                        ipaddress += "HXFF : " + db.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString();
                    }

                    if (Dns.GetHostName() != null)
                    {
                        if (ipaddress != "") { ipaddress += " - "; }
                        ipaddress += "IP.AddressFamily : " + Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();

                        if (ipaddress != "") { ipaddress += " - "; }
                        ipaddress += "HostName : " + Dns.GetHostEntry(Dns.GetHostName()).HostName;
                    }

                    if (datauser.IpAddress != db.httpContext.Connection.RemoteIpAddress.ToString() || datauser.UserAgent != db.httpContext.Request.Headers["User-Agent"].FirstOrDefault())
                    {
                        //beda browser -> harus di kick
                        kick = true;
                    }
                    else if (datauser.IpAddress == db.httpContext.Connection.RemoteIpAddress.ToString() && datauser.UserAgent == db.httpContext.Request.Headers["User-Agent"].FirstOrDefault())
                    {
                        //sama browser -> tapi sudah null (berarti pernah dilogout)
                        if (datauser.LastTimeCookies == null)
                        {
                            kick = true;
                            resetWaktu = true;
                        }
                    }

                    if (!kick)
                    {
                        //jika habis session di webconfig, maka perlu cek waktu nya.. biar bisa di reset jadi null
                        if (datauser.LastTimeCookies != null)
                        {
                            int sessionTimeout = Convert.ToInt32(appSet.LoginTimeOut);
                            DateTime LastTimeCookies = Convert.ToDateTime(datauser.LastTimeCookies);
                            if (LastTimeCookies <= DateTime.Now.AddMinutes(-1 * sessionTimeout))
                            {
                                kick = true;
                                resetWaktu = true;
                            }
                        }
                    }

                    if (kick)
                    {
                        if (resetWaktu)
                        {
                            //datauser.LastTimeCookies = null;
                            //db.Configuration.ValidateOnSaveEnabled = false;
                            //db.SaveChanges();

                            //sengaja langsung inject sql command, supaya ngga ada masalah kalau ada entity error dicoba save changes
                            var sql = "UPDATE dbo.UserMaster SET LastTimeCookies = NULL WHERE USERID = @userid";
                            db.Database.ExecuteSqlCommand(sql, new SqlParameter("@userid", datauser.UserId));
                        }
                        //db.InsertAuditTrail("KickSession", "Session Tidak Valid");
                        //db.httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        //db.HttpContext.Session.Clear();
                        //db.httpContext.Response.Redirect(db.httpContext.Request.PathBase + "Home/Login?errmsg=" + "Akun Anda digunakan login oleh pengguna lain. Silakan mengganti password Anda secara berkala.");
                    }
                    else
                    {
                        //datauser.LastTimeCookies = DateTime.Now;
                        //db.Configuration.ValidateOnSaveEnabled = false;
                        //db.SaveChanges();

                        //sengaja langsung inject sql command, supaya ngga ada masalah kalau ada entity error dicoba save changes
                        var sql = "UPDATE dbo.UserMaster SET LastTimeCookies = GETDATE() WHERE USERID = @userid";
                        db.Database.ExecuteSqlCommand(sql, new SqlParameter("@userid", datauser.UserId));

                    }
                }
                else
                {
                    db.InsertAuditTrail("CheckCookies", "User Tidak Ditemukan", pageTitle);
                    db.httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    db.httpContext.Response.Redirect(db.httpContext.Request.PathBase + "/Website/Home/Login");
                }
            }
        }
    }
}