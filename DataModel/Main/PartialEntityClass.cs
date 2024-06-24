using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Session;
using System.Security.Claims;
using System.Collections.Generic;
using Ionic.Zip;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using System.Reflection;

namespace BDA.DataModel
{
    public partial class DataEntities
    {
        public readonly Microsoft.AspNetCore.Http.HttpContext httpContext;
        public readonly IWebHostEnvironment env;
        public readonly AppSettings appSettings;

        public DataEntities(AppSettings appSettings, IHttpContextAccessor httpContext, IWebHostEnvironment env)
            : base(appSettings.ConnString)
        {
            this.httpContext = httpContext.HttpContext;
            this.env = env;
            this.appSettings = appSettings;
        }
        public DataEntities(string connString)
           : base(connString)
        {
            
        }
        public Microsoft.AspNetCore.Http.HttpContext HttpContext
        {
            get
            {
                return httpContext;
            }
        }

        public IWebHostEnvironment Env
        {
            get
            {
                return env;
            }
        }

        /*added by Riki - disesuaikan untuk aplikasi BDA*/
        public void SetStsrcModBySystem(dynamic obj)
        {
            dynamic obj2 = obj;
            if (object.ReferenceEquals(obj2.CreatedBy, System.Convert.DBNull) || obj2.CreatedBy == null)
            {
                obj2.Stsrc = "A";
                obj2.CreatedBy = "System";
                obj2.CreatedDatetime = System.DateTime.Now;
            }
            else
            {
                obj2.UpdatedBy = "System";
                obj2.UpdatedDatetime = System.DateTime.Now;
            }
        }

        public void SetStsrcFields(dynamic obj)
        {
            if (obj.GetType().GetProperty("stsrc") != null)
            {
                dynamic obj2 = obj;
                if (object.ReferenceEquals(obj2.created_by, System.Convert.DBNull) || obj2.created_by == null)
                {
                    obj2.stsrc = "A";
                    if (httpContext == null || httpContext.User.Identity.Name == null)
                    {
                        obj2.created_by = "System";
                    }
                    else
                    {
                        obj2.created_by = httpContext.User.Identity.Name;
                    }
                    obj2.date_created = System.DateTime.Now;
                }
                else
                {
                    if (httpContext == null || httpContext.User.Identity.Name == null)
                    {
                        obj2.modified_by = "System";

                    }
                    else
                    {
                        obj2.modified_by = httpContext.User.Identity.Name;
                    }
                    obj2.date_modified = System.DateTime.Now;
                }
            }
            else if (obj.GetType().GetProperty("Stsrc") != null)
            {
                dynamic obj2 = obj;
                if (object.ReferenceEquals(obj2.CreatedBy, System.Convert.DBNull) || obj2.CreatedBy == null)
                {
                    obj2.Stsrc = "A";
                    if (httpContext == null || httpContext.User.Identity.Name == null)
                    {
                        obj2.CreatedBy = "System";
                    }
                    else
                    {
                        obj2.CreatedBy = httpContext.User.Identity.Name;
                    }
                    obj2.CreatedDatetime = System.DateTime.Now;
                }
                else
                {
                    if (httpContext == null || httpContext.User.Identity.Name == null)
                    {
                        obj2.UpdatedBy = "System";

                    }
                    else
                    {
                        obj2.UpdatedBy = httpContext.User.Identity.Name;
                    }
                    obj2.UpdatedDatetime = System.DateTime.Now;
                }
            }


        }

        public void DeleteStsrc(dynamic obj)
        {
            if (obj.GetType().GetProperty("stsrc") != null)
            {

                dynamic obj2 = obj;
                obj2.stsrc = "D";
                if (httpContext == null || httpContext.User.Identity.Name == null)
                {
                    obj2.modified_by = "System";
                }
                else
                {
                    obj2.modified_by = httpContext.User.Identity.Name;
                }
                obj2.date_modified = System.DateTime.Now;
            }
            else if (obj.GetType().GetProperty("Stsrc") != null)
            {

                dynamic obj2 = obj;
                obj2.Stsrc = "D";
                if (httpContext == null || httpContext.User.Identity.Name == null)
                {
                    obj2.UpdatedBy = "System";
                }
                else
                {
                    obj2.UpdatedBy = httpContext.User.Identity.Name;
                }
                obj2.UpdatedDatetime = System.DateTime.Now;
            }

        }

        public dynamic GetSetting(string setting)
        {
            var query = from q in this.FWRefSetting where q.SetName == setting select q;
            if (query.Any())
            {
                var set = query.First();
                dynamic ret = Convert.ChangeType(set.SetValue, Type.GetType(set.SetType));
                return ret;
            }
            else
            {
                throw new InvalidOperationException("no setting [" + setting + "]");
            }
        }

        //public int GetSLA(string slaName)
        //{
        //    var query = from q in this.MasterSLA where q.SladName == slaName select q;
        //    if (query.Any())
        //    {
        //        var set = query.First();
        //        return set.SladHari;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("no SLA [" + slaName + "]");
        //    }
        //}

        public string ProcessExceptionMessage(Exception ex)
        {
            ThreadAbortException threadex = ex as ThreadAbortException;
            if (threadex != null)
            {
                //overiding
                return null;
            }else if(ex.GetType() == typeof(DbEntityValidationException))
            {
                // Retrieve the error messages as a list of strings.
                DbEntityValidationException dbex = (DbEntityValidationException)ex;
                var errorMessages = dbex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return exceptionMessage;
            }
            else
            {
                int temp = ex.Message.IndexOf("Transaction count after EXECUTE");
                string returnedMessage = "";
                if (temp == -1)
                {

                    if (ex.Message == "An error occurred while updating the entries. See the inner exception for details." | ex.Message == "An error occurred while executing the command. See the inner exception for details." | ex.Message.Contains("See the inner exception") | ex.Message == "Exception of type 'System.Web.HttpUnhandledException' was thrown.")
                    {
                        //bisa jadi ada 2 level gara2 entityframework6
                        if (ex.InnerException != null && (ex.InnerException.Message == "An error occurred while updating the entries. See the inner exception for details." | ex.InnerException.Message == "An error occurred while executing the command. See the inner exception for details." | ex.InnerException.Message.Contains("See the inner exception") | ex.InnerException.Message == "Exception of type 'System.Web.HttpUnhandledException' was thrown."))
                        {
                            returnedMessage = ex.InnerException.InnerException.Message;
                        }
                        else
                        {
                            returnedMessage = ex.InnerException.Message;
                        }
                    }
                    else
                    {
                        returnedMessage = ex.Message;
                    }

                }
                else
                {
                    returnedMessage = ex.Message.Substring(0, temp);
                }

                if (returnedMessage.Contains("was deadlocked on lock resources with another process"))
                {
                    returnedMessage = "The server is busy executing other transaction. Please try again later.";
                }
                if (returnedMessage.Contains("Parameter value") & returnedMessage.Contains("is out of range"))
                {
                    returnedMessage = returnedMessage.Replace("Parameter value", "Nilai parameter");
                    returnedMessage = returnedMessage.Replace("is out of range", "terlalu besar");
                }

                if (returnedMessage.Contains("Value was either too large or too small"))
                {
                    returnedMessage = "Nilai yang dimasukkan terlalu besar / terlalu kecil";
                }

                LogError(ex, returnedMessage);
                return returnedMessage;
            }
        }

        public void LogError(Exception ex, string returnedMessage = null)
        {
            string url = ". From DesktopApps";
            if (this.httpContext != null && this.httpContext.Request != null)
            {
                url = ". PageUrl : " + this.httpContext.Request.GetEncodedUrl();
            }

            if (returnedMessage == null)
            {
                returnedMessage = ex.Message;
            }

            if (!object.ReferenceEquals(ex.GetType(), typeof(InvalidOperationException)))
            {
                try
                {
                    //1. coba logging ke database

                    var ipaddress = "";
                    if (this.httpContext != null && this.httpContext.Request != null)
                    {
                        ipaddress = this.httpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        ipaddress = "DesktopApps";
                    }

                    //sengaja langsung inject sql command, supaya ngga ada masalah kalau ada entity error dicoba save changes

                    var sql = "INSERT INTO dbo.FWLOGError(ErrIpAddress, ErrUserId, ErrMessage, ErrDescription, ErrDate)" + Environment.NewLine +
                               "VALUES(@ipaddress,@userid,@message,@description,GETDATE())";

                    var userid = "DesktopApps;";
                    if (this.httpContext != null && this.httpContext.Request != null)
                    {
                        userid = this.httpContext.User.Identity.Name;
                    }

                    if (string.IsNullOrWhiteSpace(userid)) userid = "-";
                    this.Database.ExecuteSqlCommand(sql,
                        new SqlParameter("@ipaddress", ipaddress),
                        new SqlParameter("@userid", userid),
                        new SqlParameter("@message", returnedMessage),
                        new SqlParameter("@description", ex.ToString() + url));
                }
                catch (Exception ex2)
                {
                    try
                    {
                        using (StreamWriter write = new StreamWriter(System.IO.Path.Combine(this.env.WebRootPath, "error_log"), true))
                        {
                            //2. coba logging ke error_log
                            write.WriteLine(">>Logging Database Error - Date : " + DateTime.Now);
                            write.WriteLine(returnedMessage);
                            write.WriteLine(">>Original Error : ");
                            write.WriteLine(ex.ToString());
                            write.WriteLine(">>Logging Error : ");
                            write.WriteLine(ex2.ToString());
                            write.WriteLine();
                            write.Close();
                        }
                    }
                    catch 
                    {
                        //3. do nothing                             
                    }
                }
            }
        }

        public void SetSessionString(string name, string val)
        {
            HttpContext.Session.SetString(name, val);
        }
        public string GetSessionString(string name)
        {
            return HttpContext.Session.GetString(name);
        }
        public void RemoveSession(string name)
        {
            HttpContext.Session.Remove(name);
        }
        
        #region "Permission"
        public enum PermissionMessageType : byte
        {
            NoMessage = 0,
            //RedirectToErrorPage = 1,
            ThrowInvalidOperationException = 2
        }

        public bool CheckPermission(string modulKodeCsv, PermissionMessageType msg = PermissionMessageType.ThrowInvalidOperationException, string roleId = null)
        {
            if (HttpContext.User != null)
            {
                var checkuserid = HttpContext.User.Identity.Name;
                if (HttpContext.User.FindFirst("DelegateFor") != null)
                {
                    checkuserid = HttpContext.User.FindFirst("DelegateFor").Value;
                }

                if (roleId == null)
                {
                    //sementara biarin saja ga perlu ganti2 role
                    if (HttpContext.User.FindFirst(ClaimTypes.Role) != null)
                    {
                        roleId = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
                    }
                }
                if (CheckAuthorization(checkuserid, modulKodeCsv, roleId))
                {
                    return true;
                }
                else
                {
                    if (msg == PermissionMessageType.ThrowInvalidOperationException)
                    {
                        //string orgNama = "";
                        string temp = "Anda tidak memiliki izin untuk mengakses modul ini.";
                        //temp += " Nama Modul = " + modulKodeCsv + orgNama;                       
                        throw new InvalidOperationException(temp);
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public bool CheckAuthorization(string username, string modulCsv, string roleId = null)
        {
            try
            {
                var c = this;
                var result = c.FW_userPermission(username, modulCsv, roleId);
                if (result.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<FW_userPermission_Result> GetModulList(string username, string modulCsv, string roleId = null)
        {

            var c = this;
            var result = c.FW_userPermission(username, modulCsv, roleId);
            return result.ToList();
        }
        #endregion

        #region "Attachment"

        public FWAttachment SaveAttachment(Guid objId, string fileName, byte[] fileContent)
        {
            
            var folder = this.GetSetting("AttachmentPath") + "\\FileStorage\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString() + "\\" + DateTime.Now.Day.ToString() + "\\";
            System.IO.Directory.CreateDirectory(folder);

            var randZipName = String.Format("{0:HHmmss}", DateTime.Now) + LibFunction.CreateRandomKeyCode(4) + ".wzp";
            var zipFullPath = folder + randZipName;
            var passwordCodeNoSalt = LibFunction.CreateRandomKeyCode(6);

            using (ZipFile zip = new ZipFile())
            {
                zip.Password = this.GetSetting("AttachmentSalt") + passwordCodeNoSalt;
                zip.AddEntry(fileName, fileContent);
                zip.Save(zipFullPath);
            }

            var newAtt = new FWAttachment();
            newAtt.AttachFileLink = zipFullPath;
            newAtt.AttachFileNama = fileName;
            newAtt.AttachFileSize = (int)fileContent.Length;
            newAtt.AttachFilePwd = passwordCodeNoSalt;
            newAtt.AttachTipe = "File";
            newAtt.AttachObjId = objId;
            this.SetStsrcFields(newAtt);
            return newAtt;
        }

        public  Stream GetFileFromAttachment(FWAttachment att)
        {
            MemoryStream outputStream = null;
            using (ZipFile z = ZipFile.Read(att.AttachFileLink))
            {
                z.Password = this.GetSetting("AttachmentSalt") + att.AttachFilePwd;
                foreach (ZipEntry zEntry in z)
                {
                    outputStream = new MemoryStream();
                    zEntry.Extract(outputStream);
                    break; //ambil cuma 1 file saja
                }
            }
            return outputStream;
        }

        public Stream GetFileFromAttachment(string token)
        {
            var attId = DecryptAttId(token);
            var att = this.FWAttachment.Find(attId);
            return GetFileFromAttachment(att);
        }

        public string EncryptObjId(string objId, bool isReadOnly, string ControlID, bool isSingleOnly = false)
        {
            var tobeEcrypted = objId + "|" + isReadOnly.ToString() +  "|" + ControlID + "|" + 
                String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now.AddHours(1)) + "|" // 1 jam masa berlaku
                + isSingleOnly.ToString(); 
            return LibFunction.EncryptString(tobeEcrypted, this.GetSetting("AttachmentEncrypt"));
        }

        public AttachmentModel GetAttachmentModel(string objId, bool isReadOnly, string ControlID, bool isSingleOnly = false)
        {
            return this.DecryptObjId(this.EncryptObjId(objId, isReadOnly, ControlID, isSingleOnly));
        }

        public AttachmentModel DecryptObjId(string token)
        {
            string isi = LibFunction.DecryptString(token, this.GetSetting("AttachmentEncrypt"));
            var temp = isi.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            DateTime dateValue;
            if (temp.Count() != 5 || !DateTime.TryParse(temp[3], out dateValue))
            {
                throw new System.InvalidOperationException("Wrong Format");
            }

            if (dateValue < DateTime.Now)
            {
                throw new InvalidOperationException("Expired Token");
            }
            return new AttachmentModel() { obj_id = Guid.Parse(temp[0]), isReadOnly = Convert.ToBoolean(temp[1]), token = token, ControlID = temp[2], isSingleOnly= Convert.ToBoolean(temp[4]) };
        }



        public string EncryptAttId(long attachId)
        {
            var tobeEcrypted = attachId.ToString() + "|" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now.AddHours(1)); // 1 jam masa berlaku
            return LibFunction.EncryptString(tobeEcrypted, this.GetSetting("AttachmentEncrypt"));
        }

        public long DecryptAttId(string token)
        {
            string isi = LibFunction.DecryptString(token, this.GetSetting("AttachmentEncrypt"));
            var temp = isi.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            DateTime dateValue;
            if (temp.Count() != 2 || !DateTime.TryParse(temp[1], out dateValue))
            {
                throw new System.InvalidOperationException("Wrong Format");
            }

            if (dateValue < DateTime.Now)
            {
                throw new InvalidOperationException("Expired Token");
            }
            return Convert.ToInt64(temp[0]);
        }

        public string EncryptAttIdForDelete(long attachId)
        {
            var tobeEcrypted = attachId.ToString() + "|Delete|" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now.AddHours(1)); // 1 jam masa berlaku
            return LibFunction.EncryptString(tobeEcrypted, this.GetSetting("AttachmentEncrypt"));
        }

        public long DecryptAttIdForDelete(string token)
        {
            string isi = LibFunction.DecryptString(token, this.GetSetting("AttachmentEncrypt"));
            var temp = isi.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            DateTime dateValue;
            if (temp.Count() != 3 || !DateTime.TryParse(temp[2], out dateValue))
            {
                throw new System.InvalidOperationException("Wrong Format");
            }
            if(temp[1] != "Delete")
            {
                throw new InvalidOperationException("NotDeleteToken");
            }
            if (dateValue < DateTime.Now)
            {
                throw new InvalidOperationException("Expired Token");
            }
            return Convert.ToInt64(temp[0]);
        }
        #endregion

        #region Calendar

        public DateTime DateAddWorkday(DateTime tgl, int workday, string tipe = "Standard")
        {
            var result = this.Database.SqlQuery<DateTime>("SELECT dbo.[FW_DateAddWorkday]({0},{1},{2}) AS result",tgl,workday,tipe);
            return result.FirstOrDefault();
        }

        public DateTime NextWorkdayStart(DateTime tgl, string tipe = "Standard")
        {
            var result = this.Database.SqlQuery<DateTime>("SELECT dbo.[FW_NextWorkdayStart]({0},{1}) AS result", tgl, tipe);
            return result.FirstOrDefault();
        }


        public int WorkdayDatediffInDay(DateTime tglMulai, DateTime tglSampai, string tipe = "Standard")
        {
            var result = this.Database.SqlQuery<int>("SELECT dbo.[FW_WorkdayDatediffInDay]({0},{1},{2}) AS result",tglMulai,tglSampai,tipe);
            return result.FirstOrDefault();
        }

        public int WorkdayDatediffInSecond(DateTime tglMulai, DateTime tglSampai, string tipe = "Standard")
        {
            var result = this.Database.SqlQuery<int>("SELECT dbo.[FW_WorkdayDatediffInSecond]({0},{1},{2}) AS result", tglMulai, tglSampai, tipe);
            return result.FirstOrDefault();
        }
        #endregion
        #region "auditTrail"
        public void InsertAuditTrail(string tipe, string cause, string menu, object[] objs = null, string[] pkTable = null, string trCode = "", string namaWB = "",string usr1="")
        {
            try
            {
                var json = "";
                //json = new JavaScriptSerializer().Serialize(objs[0]);
                
                var userid = this.httpContext.User.Identity.Name;
                if (usr1 != "") {
                    userid = usr1;
                }
                if (string.IsNullOrWhiteSpace(userid)) userid = "-";

                string url = "";
                string prev_url = "";
                string ipaddress = "";
                if (this.httpContext.Request != null)
                {
                    url = this.HttpContext.Request.GetEncodedUrl();
                    ipaddress = this.httpContext.Connection.RemoteIpAddress.ToString();
                    prev_url = this.HttpContext.Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrWhiteSpace(this.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"]))
                    {
                        ipaddress = this.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                }


                //Create XML
                #region "Create XML"

                string xml = "";
                string objId = "";
                Boolean first = true;

                if (objs != null)
                {
                    Int32 counter = 0;
                    foreach (object obj in objs)
                    {
                        string tagString = "";
                        if (obj.GetType().BaseType.Name.ToString() == "Object")
                        {
                            tagString = obj.GetType().Name.ToString();
                        }
                        else
                        {
                            tagString = obj.GetType().BaseType.Name.ToString();
                        }
                        xml += "<" + tagString + ">" + Environment.NewLine;

                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (first && prop.Name.ToLower() == pkTable[counter])
                            {
                                objId = prop.GetValue(obj, null).ToString();
                                first = false;
                            }
                            if (!prop.ToString().Contains("DataModel6."))
                            {
                                object val = prop.GetValue(obj, null);
                                if (val == null)
                                {
                                    xml += "<" + prop.Name + "></" + prop.Name + ">" + Environment.NewLine;
                                }
                                else
                                {
                                    xml += "<" + prop.Name + ">" + val.ToString().Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("%", "&#37;") + "</" + prop.Name + ">" + Environment.NewLine;
                                }
                            }
                        }
                        xml += "</" + tagString + ">" + Environment.NewLine;
                        counter += 1;
                    }
                }
                #endregion
                //==========
                string tableName = "";
                if (objs != null)
                {
                    if (objs[0].GetType().BaseType.Name.ToString() == "Object")
                    {
                        tableName = objs[0].GetType().Name.ToString();
                    }
                    else
                    {
                        tableName = objs[0].GetType().BaseType.Name.ToString();
                    }
                }
                var sql = "INSERT INTO dbo.AuditTrail (AuditTipe, AuditCause, AuditMenu, AuditDate, AuditIpAddress, AuditUser, AuditDebtorName, AuditPrevUrl, AuditUrl, AuditObjType, AuditObjId, AuditObjCode, AuditJson, AuditErrMsg)" + Environment.NewLine +
                                                "VALUES (@audit_tipe, @audit_cause, @audit_menu, @audit_date, @audit_ip_address, @audit_user, @audit_debtor_name, @audit_prev_url, @audit_url, @audit_obj_type, @audit_obj_id, @audit_obj_code, @audit_json, @audit_err_msg)";
                this.Database.ExecuteSqlCommand(sql, new SqlParameter("@audit_tipe", tipe),
                                                    new SqlParameter("@audit_cause", cause),
                                                    new SqlParameter("@audit_menu", menu),
                                                    new SqlParameter("@audit_date", DateTime.Now),
                                                    new SqlParameter("@audit_ip_address", ipaddress),
                                                    new SqlParameter("@audit_user", userid),
                                                    new SqlParameter("@audit_debtor_name", namaWB),
                                                    new SqlParameter("@audit_prev_url", prev_url),
                                                    new SqlParameter("@audit_url", url),
                                                    new SqlParameter("@audit_obj_type", tableName),
                                                    new SqlParameter("@audit_obj_id", objId),
                                                    new SqlParameter("@audit_obj_code", trCode),
                                                    new SqlParameter("@audit_json", json),
                                                    new SqlParameter("@audit_err_msg", ""));
            }
            catch (Exception ex)
            {
                try
                {
                    var userid = this.httpContext.User.Identity.Name;
                    if (string.IsNullOrWhiteSpace(userid)) userid = "-";
                    string url = "";
                    string prev_url = "";
                    string ipaddress = "";
                    if (this.httpContext.Request != null)
                    {
                        url = this.HttpContext.Request.GetEncodedUrl();
                        ipaddress = this.httpContext.Connection.RemoteIpAddress.ToString();
                        prev_url = this.HttpContext.Request.Headers["Referer"].ToString();
                    }
                    var sql = "INSERT INTO dbo.AuditTrail (AuditTipe, AuditCause, AuditMenu, AuditDate, AuditIpAddress, AuditUser, AuditDebtorName, AuditPrevUrl, AuditUrl, AuditObjType, AuditObjId, AuditObjCode, AuditJson, AuditErrMsg)" + Environment.NewLine +
                                                    "VALUES (@audit_tipe, @audit_cause, @audit_menu, @audit_date, @audit_ip_address, @audit_user, @audit_debtor_name, @audit_prev_url, @audit_url, @audit_obj_type, @audit_obj_id, @audit_obj_code, @audit_json, @audit_err_msg)"; this.Database.ExecuteSqlCommand(sql, new SqlParameter("@audit_tipe", tipe),
                                                        new SqlParameter("@audit_cause", cause),
                                                        new SqlParameter("@audit_menu", menu),
                                                        new SqlParameter("@audit_date", DateTime.Now),
                                                        new SqlParameter("@audit_ip_address", ipaddress),
                                                        new SqlParameter("@audit_user", userid),
                                                        new SqlParameter("@audit_debtor_name", namaWB),
                                                        new SqlParameter("@audit_prev_url", prev_url),
                                                        new SqlParameter("@audit_url", url),
                                                        new SqlParameter("@audit_obj_type", ""),
                                                        new SqlParameter("@audit_obj_id", ""),
                                                        new SqlParameter("@audit_obj_code", trCode),
                                                        new SqlParameter("@audit_json", ""),
                                                        new SqlParameter("@audit_err_msg", ex.Message.ToString()));
                }
                catch (Exception ex2)
                {
                    try
                    {
                        var sql = "INSERT INTO dbo.AuditTrail (AuditTipe, AuditCause, AuditMenu, AuditDate, AuditIpAddress, AuditUser, AuditDebtorName, AuditPrevUrl, AuditUrl, AuditObjType, AuditObjId, AuditObjCode, AuditJson, AuditErrMsg)" + Environment.NewLine +
                                                        "VALUES (@audit_tipe, @audit_cause, @audit_menu, @audit_date, @audit_ip_address, @audit_user, @audit_debtor_name, @audit_prev_url, @audit_url, @audit_obj_type, @audit_obj_id, @audit_obj_code, @audit_json, @audit_err_msg)";
                        this.Database.ExecuteSqlCommand(sql, new SqlParameter("@audit_tipe", tipe),
                                                            new SqlParameter("@audit_cause", cause),
                                                            new SqlParameter("@audit_menu", menu),
                                                            new SqlParameter("@audit_date", DateTime.Now),
                                                            new SqlParameter("@audit_ip_address", ""),
                                                            new SqlParameter("@audit_user", ""),
                                                            new SqlParameter("@audit_debtor_name", namaWB),
                                                            new SqlParameter("@audit_prev_url", ""),
                                                            new SqlParameter("@audit_url", ""),
                                                            new SqlParameter("@audit_obj_type", ""),
                                                            new SqlParameter("@audit_obj_id", ""),
                                                            new SqlParameter("@audit_obj_code", trCode),
                                                            new SqlParameter("@audit_json", ""),
                                                            new SqlParameter("@audit_err_msg", ex2.Message.ToString()));
                    }
                    catch { }
                }

            }
        }
        #endregion


        #region LOV

        //public string[] GetLOVValues(string scope)
        //{
        //    return this.LOV.Where(x => x.stsrc == "A" && x.lov_scope == scope).Select(x => x.lov_kode).ToArray();
        //}
        #endregion

        public string GetRole(string userid)
        {
            var query = from q in this.UserMaster where q.UserId == userid select q;
            if (query.Any())
            {
                var set = query.First();
                return set.UserMainRole;
            }
            else
            {
                throw new InvalidOperationException("no User [" + userid + "]");
            }
        }
    }

    public class AttachmentModel
    {        
        public string ControlID { get; set; }
        public Guid obj_id { get; set; }
        public string token { get; set; }
        public bool isReadOnly { get; set; }
        public bool isSingleOnly { get; set; }

        public AttachmentGridData GetFirstAtt(DataEntities db)
        {
            var query = (from q in db.FWAttachment
                         where q.Stsrc == "A" && q.AttachObjId == this.obj_id
                         select new AttachmentGridData() { attach_file_nama = q.AttachFileNama, attach_file_size = q.AttachFileSize, attach_id = q.AttachId }
                       ).Take(1).ToList();

            foreach (var row in query)
            {
                row.attach_token = db.EncryptAttId(row.attach_id);
                if (!this.isReadOnly)
                {
                    row.attach_delete_token = db.EncryptAttIdForDelete(row.attach_id); // delete token cuma di supply kalau tidak readonly
                }
                row.ControlID = this.ControlID;
            }
            return query.FirstOrDefault();
        }
    }

    public class AttachmentGridData
    {
        public string attach_file_nama { get; set; }
        public int attach_file_size { get; set; }
        public string attach_token { get; set; }
        public string attach_delete_token { get; set; }
        public long attach_id { get; set; }
        public string ControlID { get; set; }

    }

    

}