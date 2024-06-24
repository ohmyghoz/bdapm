using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using BDA.DataModel;

namespace BDA.Models
{
    public class MenuDbModels
    {
        private SiteDbMap siteDbMap;
        private SiteDbMapNode currentNode;
        private SiteDbMapNode currentNodeBreadcrumb;
        //private List<FW_userPermission_Result> modulList;
        private List<SiteDbMapNode> allMenuForUser;
        private string currentUrl;

        private readonly BDA.DataModel.DataEntities db;
        public MenuDbModels(BDA.DataModel.DataEntities dataEntities, string _currentUrl)
        {
            db = dataEntities;
            currentUrl = _currentUrl;
        }

        public SiteDbMapNode GetCurrentNode()
        {
            if (siteDbMap == null)
            {
                var temp = GetSiteDbMap();
            }
            return currentNode;
        }

        public SiteDbMapNode GetCurrentNodeBreadcrumb()
        {
            if (siteDbMap == null)
            {
                var temp = GetSiteDbMap();
            }
            return currentNodeBreadcrumb;
        }

        public void GetChildFromBD(SiteDbMapNode param)
        {
            long? id = param.id;
            long? parentId = param.parent_id;

            //string userID = "";
            //if (db.httpContext.User.Identity.Name != null)
            //{
            //    userID = db.httpContext.User.Identity.Name.ToString();
            //}
            List<SiteDbMapNode> listMenu = (from q in allMenuForUser
                                            where q.parent_id == id
                                            orderby q.urut ascending, q.Url descending
                                            select new SiteDbMapNode()
                                            {
                                                id = q.id,
                                                parent_id = q.parent_id,
                                                Url = q.Url,
                                                Title = q.Title,
                                                Description = q.Description,
                                                Roles = q.Roles,
                                                IconClass = q.IconClass,
                                                IconText = q.IconText,
                                                ImageUrl = q.ImageUrl,
                                                IsSection = q.IsSection,
                                                IsHidden = q.IsHidden,
                                                tooltip = q.tooltip,
                                                HasAccess = q.HasAccess,
                                                LiClass = q.LiClass == null || q.LiClass == "" ? null : q.LiClass
                                            }).ToList();

            param.SiteDbMapNodes = listMenu;

            foreach (SiteDbMapNode x in param.SiteDbMapNodes)
            {
                GetChildFromBD(x);
            }
        }

        public SiteDbMap GetSiteDbMap()
        {

            if (siteDbMap == null)
            {
                SiteDbMap result = new SiteDbMap();
                result.SiteDbMapNode = new SiteDbMapNode();
                result.SiteDbMapNode.Url = "";
                result.SiteDbMapNode.Title = "";

                string userID = "";
                string roleName = "";
                if (db.httpContext.User.Identity.Name != null)
                {
                    userID = db.httpContext.User.Identity.Name.ToString();
                }
                if (db.httpContext.User.Identity.Name != null)
                {
                    roleName = db.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                }
                allMenuForUser = (from q in db.getMenuDb(userID,roleName)
                                  select new SiteDbMapNode()
                                  {
                                      id = q.Id,
                                      parent_id = q.ParentId,
                                      Url = q.Url,
                                      Title = q.Title,
                                      Description = q.Description,
                                      Roles = q.Roles,
                                      IconClass = q.IconClass,
                                      IconText = q.IconText,
                                      ImageUrl = q.ImageUrl,
                                      IsSection = q.IsSection,
                                      urut = q.Urut,
                                      IsHidden = q.IsHidden,
                                      tooltip = q.ModTooltip,
                                      HasAccess = q.HasAccess,
                                      LiClass = q.LiClass == null || q.LiClass == "" ? null : q.LiClass
                                  }).ToList();

                List<SiteDbMapNode> listMenu = (from q in allMenuForUser
                                                where q.parent_id == null
                                                orderby q.urut ascending, q.Url descending
                                                select new SiteDbMapNode()
                                                {
                                                    id = q.id,
                                                    parent_id = q.parent_id,
                                                    Url = q.Url,
                                                    Title = q.Title,
                                                    Description = q.Description,
                                                    Roles = q.Roles,
                                                    IconClass = q.IconClass,
                                                    IconText = q.IconText,
                                                    ImageUrl = q.ImageUrl,
                                                    IsSection = q.IsSection,
                                                    IsHidden = q.IsHidden,
                                                    tooltip = q.tooltip,
                                                    HasAccess = q.HasAccess,
                                                    LiClass = q.LiClass == null || q.LiClass == "" ? null : q.LiClass
                                                }).ToList();

                result.SiteDbMapNode.SiteDbMapNodes = listMenu;
                foreach (SiteDbMapNode x in result.SiteDbMapNode.SiteDbMapNodes)
                {
                    GetChildFromBD(x);
                }

                siteDbMap = result;

                ProcessSiteDbMapPermission(siteDbMap.SiteDbMapNode);

                //yang pertama harus ada m-t-30
                var firstnode = siteDbMap.SiteDbMapNode.SiteDbMapNodes.Where(x => x.HasAccess == true).FirstOrDefault();
                if (firstnode != null)
                {
                    if (firstnode.LiClass != null)
                    {
                        firstnode.LiClass += " m-t-30";
                    }
                    else
                    {
                        firstnode.LiClass = "m-t-30";
                    }
                }
                //}
            }


            return siteDbMap;
        }

        private void ProcessSiteDbMapPermission(SiteDbMapNode node)
        {
            //if (node.Roles == "" || node.Roles == "*") { node.HasAccess = true; }
            //else
            //{
            //    node.HasAccess = IsInModulTable(node.Roles);
            //}

            //var currentUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(db.httpContext.Request).ToLower();
            var nodeUrl = node.Url.ToLower();

            if (!String.IsNullOrWhiteSpace(node.Url) && currentUrl.Contains(nodeUrl.Replace("~", "").ToLower()))
            {// ini active
                node.LiClass = "open active menu-item-open menu-item-here";
                currentNodeBreadcrumb = node;
                if (currentNode == null)
                {
                    currentNode = node;
                }
                else
                {
                    //kalo ada EditRole dan Edit. maka keambil yang EditRole dan tidak ke replace dengan Edit
                    if (node.Url.Length > currentNode.Url.Length)
                    {
                        currentNode = node;
                    }
                }
            }

            //rekursif ke anak
            foreach (SiteDbMapNode child in node.SiteDbMapNodes)
            {
                child.MyParent = node;
                ProcessSiteDbMapPermission(child);
            }

            //kalau ada anak, si node url nya kosong(cuma header) dan hak akses anak2 nya ga ada semua. override hasaccess
            if (node.SiteDbMapNodes.Count() > 0 && node.SiteDbMapNodes.Where(x => x.HasAccess == true && x.IsHidden != "true").Count() == 0 &&
                String.IsNullOrWhiteSpace(node.Url))
            {
                node.HasAccess = false;
            }

            //add by yudi
            if (node.SiteDbMapNodes.Count() > 0 && node.SiteDbMapNodes.Where(x => x.HasAccess == true && x.IsHidden != "true").Count() > 0 &&
               String.IsNullOrWhiteSpace(node.Url))
            {
                node.HasAccess = true;
                node.IsHidden = "false";
            }
            //END add by yudi

            //cek anak2 nya ada yang keisi li class nya ngga
            if (node.LiClass == null)
            {
                var childLiClass = (from q in node.SiteDbMapNodes where q.LiClass != null select q);
                if (childLiClass.Any())
                {
                    node.LiClass = childLiClass.First().LiClass;
                }
            }
        }
    }


    public class SiteDbMapNode
    {
        public long? id { get; set; }
        public long? parent_id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Roles { get; set; }
        public string IconClass { get; set; }
        public string IconText { get; set; }
        public string ImageUrl { get; set; }
        public string IsSection { get; set; }
        public string IsHidden { get; set; }
        public string tooltip { get; set; }
        public long? urut { get; set; }
        public bool? HasAccess { get; set; }
        public SiteDbMapNode MyParent { get; set; }
        public string LiClass { get; set; }
        public List<SiteDbMapNode> SiteDbMapNodes { get; set; }
    }

    public class SiteDbMap
    {
        public SiteDbMapNode SiteDbMapNode { get; set; }
        public string Xmlns { get; set; }

//        public string GenerateMenuHtml()
//        {
//            var sb = new StringBuilder();

//            foreach (var group in SiteDbMapNode.SiteDbMapNodes)
//            {
//                if (Convert.ToBoolean(group.HasAccess) && group.IsHidden != "true")
//                {
//                    var hasChild = group.SiteDbMapNodes.Where(x => x.HasAccess == true && x.IsHidden != "true").Any();
//                    if (!hasChild)
//                    { 
//sb.AppendLine(@"<li class=""menu-item "+ (group.LiClass != null ? @group.LiClass : "") + (hasChild ? " menu-item-submenu" : "") + @""" aria-haspopup=""true"">");
//sb.AppendLine(@"	<a href="""+ (!String.IsNullOrWhiteSpace(group.Url) ? Url.Content(@group.Url) : " javascript:;") + @""" class=""menu-link"">");
//sb.AppendLine(@"		<span class=""svg-icon menu-icon"">");
//sb.AppendLine(@"			<span class=""flaticon-home""></span>");
//sb.AppendLine(@"		</span>");
//sb.AppendLine(@"		<span class=""menu-text"">Beranda</span>");
//sb.AppendLine(@"	</a>");
//sb.AppendLine(@"</li>");

//                    }

//                }
//            }

//            return sb.ToString();
//        }

    }
}