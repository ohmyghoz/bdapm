using BDA.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using CAMService;
using CAM.Definition;
using CAM.Utilities.Util;
using System.ServiceModel;
using System.Xml.Linq;
using System.DirectoryServices;

namespace BDA.Helper
{


    public class CAMHelper
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        //EndpointAddress endPointAddress = new EndpointAddress("http://10.210.240.18:8888/CAMService.svc");
        EndpointAddress endPointAddress;
        private DataEntities db;
        string CamDomain;
        int CamID;
        public CAMHelper(DataEntities db)
        {
            this.db = db;
            CamDomain = db.GetSetting("CamDomain");
            endPointAddress = new EndpointAddress(CamDomain);
            CamID = db.GetSetting("CamID");
        }

        public UserMaster CAMLoginChallenge(string username, string pass)
        {
            bool isLogin = false;
            UserMaster user = null;
            string hashedPassword = LibFunction.GenerateHash(pass);
            CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
            binding.MaxReceivedMessageSize = 2147483547;

            //client.OpenAsync();

            string xmlString = client.GetSingleAksesUser2(username, string.Empty);
            string xmlRoleString = client.SearchCurrentActiveRoleByUserName(username, CamID);
            if (!string.IsNullOrEmpty(xmlString))
            {
                XDocument doc = XDocument.Parse(xmlString);
                
                Akses_User userProp = new Akses_User();
                userProp = (Akses_User)XmlHelper.MapXmlToObject(doc, userProp);
                if (userProp.ActiveFlag == false)
                {
                    throw new InvalidOperationException("User anda belum teraktivasi atau tersuspend harap hubungi admin CAM untuk Informasi lebih lanjut");
                }

                if (userProp.UsrTypeLookup == "EXT") //untuk by pass masukin jg INT
                {
                    bool isAuth = client.IsAuthenticated(username, hashedPassword, CamID);
                    if (isAuth == true)
                    {
                        // validasi jika dia bs login tapi tidak punya hak aplikasi
                        
                        //
                        XDocument doc2 = XDocument.Parse(xmlRoleString);
                        Akses_Role userRole = new Akses_Role();
                        userRole = (Akses_Role)XmlHelper.MapXmlToObject(doc2, userRole);

                        var chkRole = (from q in db.FWRefRole where q.Stsrc == "A" & q.RoleId == userRole.RoleName select q);
                        if (!chkRole.Any())
                        {
                            var rRole = new FWRefRole();
                            db.FWRefRole.Add(rRole);
                            rRole.RoleId = userRole.RoleName;
                            rRole.RoleCatatan = "Dari CAM";
                            db.SetStsrcFields(rRole);
                            db.SaveChanges();
                        }

                        var query = (from q in db.UserMaster where q.Stsrc == "A" & q.UserId == username select q);

                        if (query.Any())
                        {
                            user = query.First();
                        }
                        else
                        {
                            user = new UserMaster();
                            db.UserMaster.Add(user);
                        }
                        user.UserNama = userProp.NamaPengguna;
                        user.UserId = userProp.UserName;
                        user.CamUserId = userProp.UserId;
                        user.UserEmail = userProp.Email;
                        user.UserPassword = "#TIDAKADA";
                        user.UserStatus = "Aktif";
                        user.UserMainRole = userRole.RoleName;
                        user.IpAddress = db.httpContext.Connection.RemoteIpAddress.ToString();
                        user.UserAgent = db.httpContext.Request.Headers["User-Agent"].FirstOrDefault();
                        user.LastTimeCookies = DateTime.Now;
                        db.SetStsrcFields(user);
                        db.SaveChanges();

                        var chkUserRole = (from q in db.FWUserRole where q.Stsrc == "A" & q.UserId == username select q);
                        FWUserRole uRole;
                        if (chkUserRole.Any())
                        {
                            uRole = chkUserRole.First();
                        }
                        else
                        {
                            uRole = new FWUserRole();
                            db.FWUserRole.Add(uRole);
                        }
                        uRole.RoleId = userRole.RoleName;
                        uRole.UserId = username;
                        db.SetStsrcFields(uRole);
                        db.SaveChanges();
                    }



                    isLogin = isAuth;
                }
                else {
                    bool isAuth = IsLDAPAuthenticated(username, pass);
                    if (isAuth == true) {
                        XDocument doc2 = XDocument.Parse(xmlRoleString);
                        Akses_Role userRole = new Akses_Role();
                        userRole = (Akses_Role)XmlHelper.MapXmlToObject(doc2, userRole);
                        var chkRole = (from q in db.FWRefRole where q.Stsrc == "A" & q.RoleId == userRole.RoleName select q);
                        if (!chkRole.Any())
                        {
                            var rRole = new FWRefRole();
                            db.FWRefRole.Add(rRole);
                            rRole.RoleId = userRole.RoleName;
                            rRole.RoleCatatan = "Dari CAM";
                            db.SetStsrcFields(rRole);
                            db.SaveChanges();
                        }

                        var query = (from q in db.UserMaster where q.Stsrc == "A" & q.UserId == username select q);

                        if (query.Any())
                        {
                            user = query.First();
                        }
                        else
                        {
                            user = new UserMaster();
                            db.UserMaster.Add(user);
                        }
                        user.UserNama = userProp.NamaPengguna;
                        user.UserId = userProp.UserName;
                        user.CamUserId = userProp.UserId;
                        user.UserEmail = userProp.Email;
                        user.UserPassword = "#TIDAKADA";
                        user.UserStatus = "Aktif";
                        user.UserMainRole = userRole.RoleName;
                        user.IpAddress = db.httpContext.Connection.RemoteIpAddress.ToString();
                        user.UserAgent = db.httpContext.Request.Headers["User-Agent"].FirstOrDefault();
                        user.LastTimeCookies = DateTime.Now;
                        db.SetStsrcFields(user);
                        db.SaveChanges();

                        var chkUserRole = (from q in db.FWUserRole where q.Stsrc == "A" & q.UserId == username select q);
                        FWUserRole uRole;
                        if (chkUserRole.Any())
                        {
                            uRole = chkUserRole.First();
                        }
                        else
                        {
                            uRole = new FWUserRole();
                            db.FWUserRole.Add(uRole);
                        }
                        uRole.RoleId = userRole.RoleName;
                        uRole.UserId = username;
                        db.SetStsrcFields(uRole);
                        db.SaveChanges();
                    }
                }
            }

            //client.CloseAsync();
            return user;
        }

        public List<string> GetListCAMUserRole(UserMaster usr)
        {
            int cId = Convert.ToInt32(usr.CamUserId);
            CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
            client.OpenAsync();
            string xmlString = client.GetUserRoleOnApplication(cId, CamID);
            XDocument doc = XDocument.Parse(xmlString);
            List<string> myList = new List<string>();
            foreach (XElement item in doc.Descendants("Akses_Role"))
            {
                Akses_Role obj = new Akses_Role();
                XDocument newDoc = XDocument.Parse(item.ToString());
                obj = (Akses_Role)XmlHelper.MapXmlToObject(newDoc, obj);

                myList.Add(obj.RoleName);
            }
            client.CloseAsync();
            return myList;
        }

        //public List<MappingUserEntity> GetListCAMPUJK(string UserId)
        //{
        //    var user = db.UserMaster.Where(x => x.Stsrc == "A" && x.UserId == UserId).ToList();
        //    int cId = Convert.ToInt32(user.FirstOrDefault().CamUserId);

        //    CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
        //    binding.MaxReceivedMessageSize = 2147483547;

        //    List<MappingUserEntity> myList = new List<MappingUserEntity>();
        //    MappingUserEntity tampung;
        //    //client.CloseAsync();
        //    //if (client.State.ToString() == "Open")
        //    //{
        //    //    client.CloseAsync();
        //    //}
        //    //client.OpenAsync();
            
        //    //string xmlString = client.GetEntity(Convert.ToInt32(CamUserId), CamID, 1000);//GetUserEntity
        //    string xmlStringsector = client.GetSectorList(CamID);//Application_Sector
        //    XDocument docsector = XDocument.Parse(xmlStringsector);
        //    foreach (XElement itemsector in docsector.Descendants("Application_Sector")) //EntitySearchResultModel
        //    {
        //        CustomSectorDTO objsector = new CustomSectorDTO();
        //        XDocument newDocsector = XDocument.Parse(itemsector.ToString());
        //        objsector = (CustomSectorDTO)XmlHelper.MapXmlToObject(newDocsector, objsector);

        //        if (objsector.SectorName.Contains("Asuransi"))
        //        {
        //            string xmlString = client.GetRegisteredEntity(cId, CamID, objsector.SectorCode, "", 0, 10000);//GetUserEntity
        //            if (xmlString != "")
        //            {
        //                XDocument doc = XDocument.Parse(xmlString);
        //                //foreach (XElement item in doc.Descendants("CustomEntity")) //Akses_UserEntity
        //                foreach (XElement item in doc.Descendants("Application_Entity")) //EntitySearchResultModel
        //                {
        //                    CustomEntity obj = new CustomEntity();
        //                    XDocument newDoc = XDocument.Parse(item.ToString());
        //                    obj = (CustomEntity)XmlHelper.MapXmlToObject(newDoc, obj);

        //                    var kode = obj.EntityCode.Split("-");

        //                    tampung = new MappingUserEntity();
        //                    //db.MappingUserEntity.Add(tampung);
        //                    tampung.UserId = cId.ToString();
        //                    tampung.EntityID = Convert.ToInt64(kode[1]);
        //                    //tampung.CreatedBy = obj.EntityName;
        //                    //tampung.Stsrc = "";
        //                    //db.SetStsrcFields(tampung);
        //                    myList.Add(tampung);
        //                }
        //            }
        //        }

                

        //    }
        //    //string xmlString = client.GetRegisteredEntity(Convert.ToInt32(CamUserId),CamID,"312","",0,100000);//GetUserEntity
        //    //XDocument doc = XDocument.Parse(xmlString);
        //    //List<MappingUserEntity> myList = new List<MappingUserEntity>();
        //    //MappingUserEntity tampung;
        //    ////foreach (XElement item in doc.Descendants("CustomEntity")) //Akses_UserEntity
        //    //foreach (XElement item in doc.Descendants("Application_Entity")) //EntitySearchResultModel
        //    //{
        //    //    CustomEntity obj = new CustomEntity();
        //    //    XDocument newDoc = XDocument.Parse(item.ToString());
        //    //    obj = (CustomEntity)XmlHelper.MapXmlToObject(newDoc, obj);

        //    //    var kode = obj.EntityCode.Split("-");

        //    //    tampung = new MappingUserEntity();
        //    //    //db.MappingUserEntity.Add(tampung);
        //    //    tampung.UserId = CamUserId;
        //    //    tampung.EntityID = Convert.ToInt64(kode[1]);
        //    //    tampung.CreatedBy = obj.EntityName;
        //    //    //tampung.Stsrc = "";
        //    //    //db.SetStsrcFields(tampung);
        //    //    myList.Add(tampung);
        //    //}
        //    //client.CloseAsync();
        //    //db.SaveChanges();
        //    return myList;
        //}

        ////public List<string> GetListCAMPUJK(UserMaster usr)
        ////{
        ////    int cId = Convert.ToInt32(usr.CamUserId);
        ////    CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
        ////    client.OpenAsync();
        ////    string xmlString = client.GetEntity(cId, CamID, 100);
        ////    XDocument doc = XDocument.Parse(xmlString);
        ////    List<string> myList = new List<string>();
        ////    foreach (XElement item in doc.Descendants("CustomEntity"))
        ////    {
        ////        CustomEntity obj = new CustomEntity();
        ////        XDocument newDoc = XDocument.Parse(item.ToString());
        ////        obj = (CustomEntity)XmlHelper.MapXmlToObject(newDoc, obj);

        ////        myList.Add(obj.EntityName);
        ////    }
        ////    client.CloseAsync();
        ////    return myList;
        ////}

        //public OrganizationCAM GetCAMUserOrganization(UserMaster usr)
        //{
        //    int cId = Convert.ToInt32(usr.CamUserId);
        //    CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
        //    client.OpenAsync();
        //    string xmlString = client.GetOrganizationbyUserId(cId);
        //    XDocument doc = XDocument.Parse(xmlString);
        //    Application_Organization obj = new Application_Organization();
        //    obj = (Application_Organization)XmlHelper.MapXmlToObject(doc, obj);
        //    client.Close();
        //    var query = (from q in db.OrganizationCAM where q.OrganizationId == obj.OrganizationId select q).FirstOrDefault();
        //    return query;
        //}

        //public void GetCAMOrganization()
        //{
        //    CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
        //    binding.MaxReceivedMessageSize = 200000000;
        //    client.OpenAsync();
        //    var currObj = (from q in db.OrganizationCAM select q);
        //    for (var i = 1; i <= 7; i++) //di cek per 3 oktober 2019 level terakhir 7
        //    {
        //        var xmlString = client.GetListOfOrganizationByLevel(i); //GetApplicationOrganizationList gak dapet level 1 dan 2
        //        XDocument doc = XDocument.Parse(xmlString);
        //        foreach (XElement item in doc.Descendants("Application_Organization"))
        //        {
        //            Application_Organization objItem = new Application_Organization();
        //            XDocument newDoc = XDocument.Parse(item.ToString());
        //            objItem = (Application_Organization)XmlHelper.MapXmlToObject(newDoc, objItem);
        //            OrganizationCAM obj;
        //            var singleObj = currObj.Where(x => x.OrganizationId == objItem.OrganizationId).FirstOrDefault();
        //            if (singleObj == null)
        //            {
        //                obj = new OrganizationCAM();
        //                db.OrganizationCAM.Add(obj);
        //            }
        //            else
        //            {
        //                obj = singleObj;
        //            }
        //            obj.OrganizationId = objItem.OrganizationId;
        //            obj.OrganizationCode = objItem.OrganizationCode;
        //            obj.OrganizationName = objItem.OrganizationName;
        //            obj.OrganizationParentCode = objItem.OrganizationParentCode;
        //            obj.OrganizationLevel = objItem.OrganizationLevel;
        //            obj.OrganizationLetterCode = objItem.OrganizationLetterCode;
        //            obj.EffectiveStartDate = objItem.EffectiveStartDate;
        //            obj.ActiveFlag = objItem.ActiveFlag;
        //            obj.DeleteFlag = objItem.DeleteFlag;
        //            obj.SupervisoryUnitFlag = objItem.SupervisoryUnitFlag;
        //            obj.camXmlString = item.ToString();
        //        }
        //        db.SaveChanges();
        //    }
        //}

        //public void GetCAMEntity()
        //{
        //    CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
        //    binding.MaxReceivedMessageSize = 200000000;
        //    db.Database.CommandTimeout = 600;
        //    client.OpenAsync();
        //    var checkString = client.GetEntityList(50, "", "", 100, 32); //buat ambl max pagenya dulu
        //    XDocument xDoc = XDocument.Parse(checkString);
        //    int maxPage = Convert.ToInt32(xDoc.Element("EntitySearchResultModel").Element("MaxPages").Value);
        //    var currObj = (from q in db.EntityCAM select q);

        //    for (var i = 1; i <= maxPage; i++)
        //    {
        //        var xmlString = client.GetEntityList(50, "", "", i, 32); //max row taken 32 menurut email mas arvin 06/08/2019
        //        XDocument doc = XDocument.Parse(xmlString);
        //        foreach (XElement item in doc.Descendants("Application_Entity"))
        //        {
        //            Application_Entity objItem = new Application_Entity();
        //            XDocument newDoc = XDocument.Parse(item.ToString());
        //            objItem = (Application_Entity)XmlHelper.MapXmlToObject(newDoc, objItem);
        //            EntityCAM obj;
        //            var singleObj = currObj.Where(x => x.EntityId == objItem.EntityId).FirstOrDefault();
        //            if (singleObj == null)
        //            {
        //                obj = new EntityCAM();
        //                db.EntityCAM.Add(obj);
        //            }
        //            else
        //            {
        //                obj = singleObj;
        //            }
        //            obj.EntityId = objItem.EntityId;
        //            obj.EntityCode = objItem.EntityCode;
        //            obj.EntityName = objItem.EntityName;
        //            obj.Address1 = objItem.Address1;
        //            obj.SectorCode = objItem.SectorCode;

        //            var detailString = client.GetSingleApplicationEntity(obj.EntityCode);
        //            if (detailString != "")
        //            {
        //                XDocument detDoc = XDocument.Parse(detailString);
        //                Application_Entity objDetail = new Application_Entity();
        //                objDetail = (Application_Entity)XmlHelper.MapXmlToObject(detDoc, objDetail);
        //                obj.Address2 = objDetail.Address2;
        //                obj.Address3 = objDetail.Address3;
        //                obj.Fax = objDetail.Fax;
        //                obj.Phone = objDetail.Phone;
        //                obj.NPWP = objDetail.NPWP;
        //                obj.SectorType = objDetail.SectorType;
        //                obj.RegionCode = objDetail.RegionCode;
        //                obj.RegionName = objDetail.RegionName;
        //                obj.Website = objDetail.Website;
        //                obj.Email = objDetail.Email;
        //                obj.EntityType = objDetail.EntityType;
        //                obj.ActiveFlag = objDetail.ActiveFlag.ToString();
        //            }
        //            obj.camXmlString = (detailString != "" ? detailString : item.ToString());
        //        }
        //    }
        //    db.SaveChanges();
        //}

        //public void GetCAMSector()
        //{
        //    CAMServiceClient client = new CAMServiceClient(binding, endPointAddress);
        //    binding.MaxReceivedMessageSize = 200000000;
        //    var query = db.Master_SectorCAM.Where(x => x.SectorCode != null); //query diawal biar gak lemot pas loop
        //    client.OpenAsync();
        //    var xmlString = client.GetSectorList(50); //50 itu id aplikasi APPK di cam, kalo berubah harus diganti juga, kalo bisa ganti ke setting aja
        //    Console.WriteLine(xmlString);
        //    XDocument doc = XDocument.Parse(xmlString);
        //    foreach (XElement item in doc.Root.Elements())
        //    {
        //        Application_Sector myObject = new Application_Sector();
        //        XDocument newDoc = XDocument.Parse(item.ToString());
        //        myObject = (Application_Sector)XmlHelper.MapXmlToObject(newDoc, myObject);
        //        var check = query.Where(x => x.SectorCode == myObject.SectorCode).FirstOrDefault();
        //        Master_SectorCAM obj;
        //        if (check != null)
        //        {
        //            obj = check;
        //        }
        //        else
        //        {
        //            obj = new Master_SectorCAM();
        //            db.Master_SectorCAM.Add(obj);
        //            obj.SectorCode = myObject.SectorCode;
        //        }
        //        obj.EntityType = myObject.EntityType;
        //        obj.ParentSectorCode = myObject.ParentSectorCode;
        //        obj.SectorName = myObject.SectorName;
        //        obj.SectorId = myObject.SectorId;
        //        obj.SectorLevel = myObject.SectorLevel;
        //        obj.camXmlString = item.ToString();
        //        db.SetStsrcFields(obj);
        //    }
        //    client.Close();
        //    db.SaveChanges();
        //}

        //private bool IsLDAPAuthenticated(string pUsername, string pPassword)
        //{
        //    bool isUser = false;
        //    //string sDomainLDAP = db.GetSetting("DomainLDAP");
        //    string sDomainLDAP = "LDAP://devojk.go.id";
        //    //string [] arrUser = pUsername.Split('\\');
        //    string userLdap = pUsername.Replace("ojk\\", "");
        //    DirectoryEntry entry = new DirectoryEntry(sDomainLDAP, userLdap, pPassword);
        //    try
        //    {
        //        Object obj = entry.NativeObject;
        //        DirectorySearcher search = new DirectorySearcher(entry);
        //        search.Filter = "(SAMAccountName=" + userLdap + ")";
        //        search.PropertiesToLoad.Add("cn");
        //        //search.PropertiesToLoad.Add("users");
        //        //search.PropertiesToLoad.Add("DBS");
        //        SearchResult result = search.FindOne();
        //        if (result != null )
        //        {
        //            isUser = true;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new InvalidOperationException(db.ProcessExceptionMessage(ex));
        //        //isUser = false;
        //    }
        //    return isUser;
        //}
        private bool IsLDAPAuthenticated(string pUsername, string pPassword)
        {
            bool isUser = false;
            string sDomainLDAP = db.GetSetting("DomainLDAP");
            //string sDomainLDAP = "LDAP://devojk.go.id";
            //string sDomainLDAP = "LDAP://ojk.go.id";
            string [] arrUser = pUsername.Split('\\');
            //string userLdap = pUsername.Replace("ojk\\", "");
            DirectoryEntry entry = new DirectoryEntry(sDomainLDAP, arrUser[1], pPassword);
            try
            {
                //Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + arrUser[1] + ")";
                search.PropertiesToLoad.Add("cn");
                //search.PropertiesToLoad.Add("users");
                //search.PropertiesToLoad.Add("DBS");
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    isUser = true;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(db.ProcessExceptionMessage(ex));
                //isUser = false;
            }
            return isUser;
        }
    }
}
