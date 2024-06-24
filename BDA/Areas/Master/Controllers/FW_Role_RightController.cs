using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BDA.Controllers
{
    [Area("Master")]
    public class FW_Role_RightController : Controller
    {
        private DataEntities db;
        private IWebHostEnvironment _env;

        public FW_Role_RightController(DataEntities db, IWebHostEnvironment env)
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

            db.CheckPermission("Hak Akses View", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            ViewBag.Add = db.CheckPermission("Hak Akses Add", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Edit = db.CheckPermission("Hak Akses Edit", DataEntities.PermissionMessageType.NoMessage);
            ViewBag.Delete = db.CheckPermission("Hak Akses Delete", DataEntities.PermissionMessageType.NoMessage);
            db.InsertAuditTrail("HakAkses_Akses_Page", "Akses Page Hak Akses", pageTitle);
            return View();
        }

        public IActionResult Edit(long? id)
        {
            ViewBag.Mode = "Edit";
            if (id == null) return BadRequest();

            var obj = db.FWRoleRight.Where(x => x.RoleId == id  && x.Stsrc == "A");
            if (obj == null) return NotFound();

            TempData["RoleId"] = id;

            return View();
        }

        [HttpPut]
        public IActionResult Put(long key, string values)
        {
            var mdl = new BDA.Models.MenuDbModels(db, Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower());
            var currentNode = mdl.GetCurrentNode();

            string pageTitle = currentNode != null ? currentNode.Title : "";

            db.CheckPermission("Hak Akses Edit", DataEntities.PermissionMessageType.ThrowInvalidOperationException);

            var RoleId = Convert.ToInt64(TempData.Peek("RoleId"));

            var obj = db.FWRoleRight.FirstOrDefault(x => x.ModId == key && x.RoleId == RoleId);

            if (obj != null)
            {
                JsonConvert.PopulateObject(values, obj);

                db.SetStsrcFields(obj);

                if (!TryValidateModel(obj))
                {
                    return BadRequest(ModelState.GetFullErrorMessage());
                }
                db.InsertAuditTrail("FW_Role_Right_UbahHakAkses", "Edit Role Right", pageTitle, new object[] { obj }, new string[] { "RightId" }, obj.RightId.ToString());

            }
            else
            {
                var newObj = new FWRoleRight();

                JsonConvert.PopulateObject(values, newObj);

                db.SetStsrcFields(newObj);

                if (!TryValidateModel(newObj))
                {
                    return BadRequest(ModelState.GetFullErrorMessage());
                }

                newObj.RoleId = RoleId;
                newObj.ModId = key;

                db.FWRoleRight.Add(newObj);
                db.SaveChanges();
                db.InsertAuditTrail("FW_Role_Right_UbahHakAkses", "Edit Role Right", pageTitle, new object[] { newObj }, new string[] { "RightId" }, newObj.RightId.ToString());
            }

            var childNodes = (from q in  db.getModulChildren(key) select q.ModId).ToList();
            //var childIds = childNodes.Select(c => c.ModId).ToList();

            //var childrenToUpdate = db.FWRoleRight.Where(x => childNodes.Contains(x.ModId) && x.RoleId == RoleId).ToList();
            foreach (var i in childNodes) {
                var chkRole=from q in db.FWRoleRight
                            where q.Stsrc=="A" && q.RoleId==RoleId && q.ModId==i
                            select q;
                if (chkRole.Count() > 0)
                {
                    var objRole=chkRole.FirstOrDefault();
                    JsonConvert.PopulateObject(values, objRole);
                    db.SetStsrcFields(objRole);
                }
                else {
                    var newObj = new FWRoleRight();
                    db.FWRoleRight.Add(newObj);
                    //JsonConvert.PopulateObject(values, newObj);
                    if (values.Contains("IsView")) {
                        if (obj != null)
                        {
                            newObj.IsView = obj.IsView;
                        }
                        else {
                            newObj.IsView = true;
                        }
                    }
                    if (values.Contains("IsAdd"))
                    {
                        if (obj != null)
                        {
                            newObj.IsAdd = obj.IsAdd;
                        }
                        else
                        {
                            newObj.IsAdd = true;
                        }
                    }
                    if (values.Contains("IsEdit"))
                    {
                        if (obj != null)
                        {
                            newObj.IsEdit = obj.IsEdit;
                        }
                        else
                        {
                            newObj.IsEdit = true;
                        }
                    }
                    if (values.Contains("IsDelete"))
                    {
                        if (obj != null)
                        {
                            newObj.IsDelete = obj.IsDelete;
                        }
                        else
                        {
                            newObj.IsDelete = true;
                        }
                    }
                    if (values.Contains("IsExport"))
                    {
                        if (obj != null)
                        {
                            newObj.IsExport = obj.IsExport;
                        }
                        else
                        {
                            newObj.IsExport = true;
                        }
                    }
                   
                    newObj.RoleId = RoleId;
                    newObj.ModId = Convert.ToInt32( i);
                    db.SetStsrcFields(newObj);
                    db.SaveChanges();


                }
            }
            //foreach (var childNode in childrenToUpdate)
            //{
            //    JsonConvert.PopulateObject(values, childNode);
            //    db.SetStsrcFields(childNode);
            //}

            db.SaveChanges();

            return Ok();
        }


        #region "GetGridData"
        [HttpGet]
        public object GetGridDataRole(DataSourceLoadOptions loadOptions)
        {
            var query = db.FWRefRole.Where(x => x.Stsrc == "A").ToList();

            return DataSourceLoader.Load(query, loadOptions);
        }

        [HttpGet]
        public object GetGridDataDetailRole(DataSourceLoadOptions loadOptions, long RoleId)
        {
            var query = (from q in db.FWRoleRight
                         join r in db.FWRefModul on q.ModId equals r.ModId
                         where q.Stsrc == "A" && q.RoleId == RoleId && r.Stsrc == "A"
                         orderby (r.ParentModId == null ? 0 : 1), r.ModUrut
                         select new
                         {
                             q.RightId,
                             q.RoleId,
                             q.ModId,
                             r.ParentModId,
                             r.ModNama,
                             r.ModUrut,
                             q.IsView,
                             q.IsAdd,
                             q.IsEdit,
                             q.IsDelete,
                             q.IsExport
                             //q.IsReview,
                             //q.IsApprove,
                             //q.IsOpen,
                             //q.IsPrint
                         }).ToList();

            return DataSourceLoader.Load(query, loadOptions);

        }

        [HttpGet]
        public object GetGridDataRight(DataSourceLoadOptions loadOptions)
        {
            var RoleId = Convert.ToInt64(TempData.Peek("RoleId"));

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            var query = (from q in db.FWRefModul
                         join r in (from x in db.FWRoleRight where x.RoleId == RoleId && x.Stsrc == "A" select x) on q.ModId equals r.ModId
                         into grp
                         from r in grp.DefaultIfEmpty()
                         where q.Stsrc == "A"
                         orderby q.ModUrut
                         select new
                         {
                             tempKey = q.ModId.ToString() + " - " + r.RightId.ToString(),
                             q.ModId,
                             q.ParentModId,
                             q.ModNama,
                             IsView = r.IsView == null ? false : r.IsView,
                             IsAdd = r.IsAdd == null ? false : r.IsAdd,
                             IsEdit = r.IsEdit == null ? false : r.IsEdit,
                             IsDelete = r.IsDelete == null ? false : r.IsDelete,
                             IsExport = r.IsExport == null ? false : r.IsExport,
                             IsReview = r.IsReview == null ? false : r.IsReview,
                             IsApprove = r.IsApprove == null ? false : r.IsApprove,
                             IsOpen = r.IsOpen == null ? false : r.IsOpen,
                             IsPrint = r.IsPrint == null ? false : r.IsPrint
                         }).ToList();
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

            return DataSourceLoader.Load(query, loadOptions);
        }
        #endregion
    }
}
