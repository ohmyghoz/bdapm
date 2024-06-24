using BDA.DataModel;
using BDA.Helper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BDA.Models;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Template;

namespace BDA.Controllers
{
    [Area("FW")]
    public class EmailController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;
        public EmailController(DataEntities db, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this.db = db;
            _env = env;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public ActionResult Index()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Template View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Add = db.CheckPermission("Template Add", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Edit = db.CheckPermission("Template Edit", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Delete = db.CheckPermission("Template Delete", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("TemplateEmail_Akses_Page", "Akses Page Template Email", pageTitle);
            return View();
        }

        public IActionResult Create()
        {
            var obj = new FWNoticeTemplate();
            return View("Template", obj);
        }

        
        public ActionResult Template(string id)
        {
            FWNoticeTemplate mdl = null;
            if (id != null) mdl = db.FWNoticeTemplate.Where(x => x.NottId == id).FirstOrDefault();

            if (mdl == null) mdl = new FWNoticeTemplate();

            return View(mdl);
        }


        [HttpPost]
        public IActionResult Template([Bind("NottId", "NottModelType", "RdefKode", "NottRdefParamCsv", "NottKeyType", "NottTitle", "NottTo", "NottCc", "NottBcc", "NottBatch", "NottSender", "NottContent", "NottSmallContent", "NottCatatan", "NottOneEmailPerUser", "NottRefId")] FWNoticeTemplate obj)
        {
            try
            {
                db.CheckPermission("Template Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                if (!string.IsNullOrWhiteSpace(obj.NottId))
                {
                    var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                    var currentNode = mdl.GetCurrentNode();

                    string pageTitle = currentNode != null ? currentNode.Title : "";

                    var oldObj = db.FWNoticeTemplate.Find(obj.NottId);
                    WSMapper.CopyFieldValues(obj, oldObj, "NottModelType,RdefKode,NottRdefParamCsv,NottKeyType,NottTitle,NottTo,NottCc,NottBcc,NottBatch,NottSender,NottContent,NottSmallContent,NottCatatan,NottOneEmailPerUser,NottRefId");

                    // kalau tidak dibeginikan bikin error
                    oldObj.NottContent = oldObj.NottContent.Replace("&quot;", "\"");

                    db.SaveChanges();
                    db.InsertAuditTrail("Email_Template", "Edit Template", pageTitle, new object[] { obj }, new string[] { "NottId" }, obj.NottId.ToString());

                    db.SetSessionString("sctext", "Berhasil menyimpan ke Notifikasi.");
                    return RedirectToAction("Template", new { id = obj.NottId });
                }
            }
            catch (System.Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Create([Bind("NottId", "NottModelType", "RdefKode", "NottRdefParamCsv", "NottKeyType", "NottTitle", "NottTo", "NottCc", "NottBcc", "NottBatch", "NottSender", "NottContent", "NottSmallContent", "NottCatatan", "NottOneEmailPerUser", "NottRefId")] FWNoticeTemplate obj)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Template Add", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                if (db.FWNoticeTemplate.Where(x => x.NottId == obj.NottId).Any())
                {
                    throw new InvalidOperationException("Template dengan nama ini sudah ada");
                }

                obj.NottId = obj.NottId.Trim();
                obj.NottContent = obj.NottContent.Replace("&quot;", "\"");
                db.FWNoticeTemplate.Add(obj);
                db.SetStsrcFields(obj);
                db.SaveChanges();
                db.InsertAuditTrail("Email_Create", "Tambah Template", pageTitle, new object[] { obj }, new string[] { "NottId" }, obj.NottId.ToString());

                db.SetSessionString("sctext", "Berhasil menyimpan Notifikasi");
                //return RedirectToAction("Template", new { id = obj.nott_id });

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            return View("Template", obj);
        }
        public object GetGridData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWNoticeTemplate
                        where q.Stsrc == "A"
                        select new { q.NottId, q.NottTitle };
            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpPost]
        public IActionResult Delete(string param1)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Template Delete", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            var obj = db.FWNoticeTemplate.First(o => o.NottId == param1);
            db.DeleteStsrc(obj);
            db.SaveChanges();

            db.InsertAuditTrail("Email_Delete", "Hapus Template", pageTitle, new object[] { obj }, new string[] { "NottId" }, obj.NottId.ToString());

            var resp = "Berhasil menghapus data Notifikasi";
            return new JsonResult(resp);
        }

        [HttpGet]
        public IActionResult PopupSample(string nottId, string testId)
        {
            try
            {
                if (nottId != null)
                {
                    var nott = db.FWNoticeTemplate.Find(nottId);
                    if (nott != null)
                    {
                        if (!string.IsNullOrWhiteSpace(testId))
                        {
                            var curEntityPI = db.GetType().GetProperties().Where(pr => pr.Name == nott.NottModelType).FirstOrDefault();
                            if (curEntityPI == null)
                            {
                                throw new InvalidOperationException("Cannot find type : " + nott.NottModelType);
                            }

                            //default
                            if (string.IsNullOrWhiteSpace(nott.NottKeyType)) nott.NottKeyType = "System.Int64";


                            var curEntityType = curEntityPI.PropertyType.GetGenericArguments().First();
                            object result;
                            if (nott.NottKeyType == "System.Guid")
                            {
                                result = db.Set(curEntityType).Find(Guid.Parse(testId));
                            }
                            else
                            {
                                result = db.Set(curEntityType).Find(Convert.ChangeType(testId, Type.GetType(nott.NottKeyType)));

                            }
                            if (result == null)
                            {
                                throw new InvalidOperationException("Cannot find entity for that testid");
                            }

                            var help = new BusinessLayer.NoticeTemplateHelper(db, nottId, result);
                            nott.NottLastTestId = testId;
                            db.SaveChanges();
                            var notice = help.GenerateNotice(false);



                            return View(notice);
                        }
                        else
                        {

                            var help = new BusinessLayer.NoticeTemplateHelper(db, nottId, null);
                            var notice = help.GenerateNotice();
                            return View(notice);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                db.SetSessionString("errtext", db.ProcessExceptionMessage(ex));
            }
            var notice2 = new FWNotice();
            return View(notice2);

        }

        [HttpGet]
        public IActionResult Queue()
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Queue View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Edit = db.CheckPermission("Queue Edit", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("EmailQueue_Akses_Page", "Akses Page Email Queue", pageTitle);

            return View();
        }

        [HttpPost]
        public IActionResult Resend(long key)
        {
            try
            {
                var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
                var currentNode = mdl.GetCurrentNode();

                string pageTitle = currentNode != null ? currentNode.Title : "";

                db.CheckPermission("Queue Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

                var obj = db.FWEmailQueue.Find(key);
                if (obj == null)
                {
                    throw new InvalidOperationException("Antrian email tidak ditemukan");
                }

                obj.EmailqStatus = 0;
                obj.EmailqSentTry = 0;
                db.SetStsrcFields(obj);
                db.SaveChanges();

                db.InsertAuditTrail("Email_Resend", "Kirim Ulang Antrian Email", pageTitle, new object[] { obj }, new string[] { "EmailqId" }, obj.EmailqId.ToString());

                return new JsonResult("Sukses kirim ulang antrian email");
            }
            catch (Exception ex)
            {
                return new JsonResult(db.ProcessExceptionMessage(ex));
            }
        }

        [HttpGet]
        public IActionResult PopupBody(long id)
        {
            var eq = db.FWEmailQueue.Find(id);
            if (eq.Stsrc != "A") return NotFound();
            return View(eq);
        }

        [HttpGet]
        public object GetGridQueueData(DataSourceLoadOptions loadOptions)
        {
            var query = from q in db.FWEmailQueue
                        where q.Stsrc == "A"
                        orderby q.EmailqId descending
                        select new {
                            q.EmailqErrorText,
                            q.EmailqSentTry,
                            q.EmailqSubject,
                            q.EmailqQueueDate,
                            EmailqStatus = q.EmailqStatus == 0 ? "Pending" : q.EmailqStatus == 1 ? "Sukses" : "Gagal",
                            q.EmailqBcc,
                            q.EmailqCc,
                            q.EmailqFrom, 
                            q.EmailqTo,
                            q.EmailqId
                        };
            return DataSourceLoader.Load(query, loadOptions);
        }



        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;



        //private void RecursiveSiteMap(SiteMapNode node)
        //{
        //    if (node.MyParent != null)
        //    {
        //        node.Description = node.MyParent.Description + " > " + node.Title;
        //    }
        //    foreach (var child in node.SiteMapNodes)
        //    {
        //        RecursiveSiteMap(child);
        //    }
        //}

        //private SiteMapNode currentNode;
        //private void RecursiveFind(string url, SiteMapNode node)
        //{
        //    var nodeUrl = node.Url != null ? node.Url.ToLower() : "";
        //    if (!String.IsNullOrWhiteSpace(node.Url) && url.Contains(nodeUrl.Replace("~", "")))
        //    {
        //        currentNode = node;
        //    }

        //    foreach (var child in node.SiteMapNodes)
        //    {
        //        RecursiveFind(url, child);
        //    }
        //}

        //[HttpGet]
        //public object GetRoutesData(DataSourceLoadOptions loadOptions)
        //{
        //    db.CheckPermission(db.appSettings.SuperAdminRolesCSV);

        //    var mdl = new Models.MenuModels(db);
        //    var siteMap = mdl.GetSiteMap();
        //    //generate MenuTree 
        //    siteMap.SiteMapNode.Description = "";
        //    RecursiveSiteMap(siteMap.SiteMapNode);



        //    var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.ToList();
        //    var mdlList = new List<RouteModel>();
        //    foreach (var rt in routes.Select(x => new { Area = x.RouteValues["area"], Controller = x.RouteValues["controller"], Action = x.RouteValues["action"] }).Distinct())
        //    {
        //        var newModel = new RouteModel() { Area = rt.Area, Controller = rt.Controller, Action = rt.Action };
        //        var url = Url.Action(newModel.Action, newModel.Controller, new { area = newModel.Area }).ToLower();

        //        currentNode = null;
        //        RecursiveFind(url, siteMap.SiteMapNode);

        //        if (currentNode != null)
        //        {
        //            newModel.IsHidden = currentNode.IsHidden;
        //            newModel.Url = currentNode.Url;
        //            newModel.MenuTree = currentNode.Description;
        //            newModel.Roles = currentNode.Roles;
        //            newModel.Title = currentNode.Title;
        //        }
        //        else
        //        {
        //            newModel.Title = "<not in sitemap>";
        //        }
        //        mdlList.Add(newModel);
        //    }

        //    return DataSourceLoader.Load(mdlList, loadOptions);
        //}

        [HttpPost]
        public IActionResult MarkNoticeRead()
        {
            try
            {
                var usr = db.UserMaster.Where(x => x.UserId == db.HttpContext.User.Identity.Name).FirstOrDefault();
                if (usr != null)
                {
                    usr.UserNoticeLastRead = DateTime.Now;
                    db.SetStsrcFields(usr);
                    db.SaveChanges();
                }
                var resp = "Berhasil menyimpan status read.";
                return new JsonResult(resp);
            }
            catch (Exception ex)
            {
                var resp = db.ProcessExceptionMessage(ex);
                return new JsonResult(resp);
            }
        }

        public IActionResult CekRoutes()
        {
            return View();
        }
        internal class RouteModel
        {
            public string Area { get; set; }
            public string Controller { get; set; }
            public string Action { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public string IsHidden { get; set; }
            public string MenuTree { get; set; }
            public string Roles { get; set; }


        }
    }


}