using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BDA.DataModel
{
    public class AuthHelper
    {
        public UserMaster AddLdapToDatabase(DataEntities c, SearchResult result)
        {
            //Dim eprocUserId = "bank_indonesia\" & result.Properties.Item("samaccountname").Item(0).ToString

            var appUserId = result.Properties["samaccountname"][0].ToString() + "@corp.winning-soft.com";

            var query = from q in c.UserMaster where q.UserId == appUserId select q;

            UserMaster user;
            if (query.Any())
            {
                //sudah ada di db
                user = query.First();
            }
            else
            {
                user = new UserMaster();
                c.UserMaster.Add(user);
                user.UserId = appUserId;
                user.UserStatus = "Aktif";
                user.UserPassword = "tidakdipakai!#@";
            }
            if (result.Properties["mail"].Count > 0)
            {
                user.UserEmail = result.Properties["mail"][0].ToString();
            }

            if (result.Properties["displayname"].Count > 0)
            {
                user.UserNama = result.Properties["displayname"][0].ToString();
            }

            FWUserRole urole = null;
            var roleQuery = from q in c.FWUserRole where q.UserId == user.UserId /*&& q.RoleId == "User"*/ select q;
            if (roleQuery.Count() == 0)
            {
                //urole = new FWUser_Role();
                urole = new FWUserRole();
                c.FWUserRole.Add(urole);
                urole.UserId = user.UserId;
                //urole.RoleId = "User"; //di comment untuk sementara
                c.SetStsrcFields(urole);
            }

            user.Stsrc = "A";
            user.UserLastLogin = DateTime.Now;
            c.SetStsrcFields(user);
            c.SaveChanges();
            return user;
        }

        private SearchResult SearchLdap(string domain, string ou, string username, string password, string extraFilter = "(objectCategory=user)")
        {

            string whitelist = @"^[a-zA-Z1-9\-\.@_']*$";
            username = username.Trim();
            Regex pattern = new Regex(whitelist);
            if (!pattern.IsMatch(username))
            {
                throw new InvalidOperationException("User Id hanya dapat terdiri atas angka dan huruf.");
            }
            else
            {
                using (DirectoryEntry de = new DirectoryEntry(("LDAP://" + domain), (username), password))
                {
                    using (DirectorySearcher search = new DirectorySearcher(de))
                    {
                        search.Filter = ("(&" + extraFilter + "(SAMAccountName=" + username + "))");
                        search.PropertiesToLoad.Add("displayname");
                        search.PropertiesToLoad.Add("samaccountname");
                        search.PropertiesToLoad.Add("givenname");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("title");
                        SearchResult result = null;
                        result = search.FindOne();

                        if ((result != null))
                            return result;
                        else
                            return null;
                    }
                }
            }

        }

        private static UserMaster SearchDB(DataEntities c, string username, string password)
        {
            string hashedPassword = LibFunction.HashPasswordSHA256(password);
            var query = from q in c.UserMaster
                        where q.Stsrc == "A" && q.UserStatus == "Aktif"
                        where q.UserId == username & q.UserPassword == hashedPassword
                        select q;

            if (query.Any())
                return query.First();
            else
                return null;
        }

        public UserMaster AuthenticateUser(DataEntities c, string userName, string password)
        {

            userName = userName.ToLower();
            //case insensitive
            string userId;
            UserMaster user = null;
            bool isAuthenticated = false;
            string errMsg = "";
            try
            {
                //1. Cek LDAP/Bukan
                if (userName.Contains("corpojk\\") || userName.Contains("@corp.ojk.go.id") || userName.Contains("@ojk.go.id"))
                //if (userName.Contains("corpws\\") | userName.Contains("@corp.winning-soft.com"))
                {
                    userName = userName.Replace("corpojk\\", "").Replace("@corp.ojk.go.id", "").Replace("@ojk.go.id", "");
                    //userName = userName.Replace("corpws\\", "").Replace("@corp.winning-soft.com", "");
                    //cleanup login dari user untuk LDAP auth

                    userId = userName + "@ojk.go.id";

                    //userId = userName + "@corp.winning-soft.com";

                    //apa yang disimpan kedalam database local
                    //1a. Cek apa sudah di daftarkan ke database
                    var findUser = from q in c.UserMaster where q.UserId == userId && q.Stsrc == "A" && q.UserStatus=="Aktif" select q;
                    //cari dulu sudah ada di db belum
                    if (findUser.Any())
                    {
                        user = findUser.First();

                        //1b. Cek apa ldap login benar atau tidak
                        string sDomainLDAP = c.GetSetting("DomainLDAP");
                        var result = SearchLdap(sDomainLDAP, "", userName, password);
                        //var result = SearchLdap("corp.winning-soft.com", "OU=WSDev,OU=WS,DC=corp,DC=winning-soft,DC=com", userName, password);
                        //setting domain server LDAP disini
                        if (result != null)
                        {
                            isAuthenticated = true;
                        }
                        else
                        {
                            errMsg = "User ID (" + userName + ") atau Password LDAP anda Salah.";
                        }
                    }
                    
                }
                else
                {
                    //2. Cek DB
                    userId = userName;
                    //2a. Cek Password dan User DB
                    user = SearchDB(c, userId, password);
                    if (user != null)
                    {
                        isAuthenticated = true;
                    }
                    else
                    {
                        var findUser = from q in c.UserMaster where q.UserId == userId && q.Stsrc == "A" && q.UserStatus == "Aktif" select q;
                        //cari dulu sudah ada di db belum
                        if (findUser.Any())
                        {
                            user = findUser.First();
                        }
                        errMsg = "User ID (" + userName + ")  atau Password anda Salah.";
                    }
                }

                //3. Cek apakah si user sudah kena blok atau belum
                if (user != null)
                {
                    if (user.UserBlockedDate != null)
                    {
                        int blockTime = c.GetSetting("UserFailedPasswordBlockTime");

                        var batasBlokir = user.UserBlockedDate.Value.AddMinutes(blockTime);
                        if (System.DateTime.Now < batasBlokir)
                        {
                            isAuthenticated = false;
                            errMsg = "User ID " + userId + " sedang diblokir sampai tanggal " + String.Format("{0:dd MMM yyyy}", batasBlokir) + " jam " + String.Format("{0:HH:mm:ss}", batasBlokir) + " WIB.";
                        }
                        else
                        {
                            user.UserBlockedDate = null;
                            //bersihkan waktu block date nya.
                            user.UserFailedLoginCount = 0;
                        }
                    }
                }


                if (isAuthenticated && user != null)
                {
                    //kalau berhasil authenticated diatas
                    //Logging sederhana kapan terakhir kali login. Bila diperlukan silahkan di extend
                    user.UserLastLogin = DateTime.Now;
                    user.IpAddress = c.httpContext.Connection.RemoteIpAddress.ToString();
                    user.LastTimeCookies = DateTime.Now;
                    user.UserAgent = c.httpContext.Request.Headers["User-Agent"].FirstOrDefault();
                    //reset jumlah password salah nya
                    user.UserFailedLoginCount = 0;
                    user.IpAddress = "RemoteIP : " + c.httpContext.Connection.RemoteIpAddress.ToString() + " - LocalIP : " + c.httpContext.Connection.LocalIpAddress.ToString();
                    if (!string.IsNullOrWhiteSpace(c.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"]))
                    {
                        if (user.IpAddress != "") { user.IpAddress += " - "; }
                        user.IpAddress += "HXFF : " + c.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString();
                    }

                    if (Dns.GetHostName() != null)
                    {
                        if (user.IpAddress != "") { user.IpAddress += " - "; }
                        user.IpAddress += "IP.AddressFamily : " + Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();

                        if (user.IpAddress != "") { user.IpAddress += " - "; }
                        user.IpAddress += "HostName : " + Dns.GetHostEntry(Dns.GetHostName()).HostName;
                    }

                    c.SetStsrcFields(user);
                    c.SaveChanges();
                    return user;

                }
                else
                {
                    //5. Gagal authentikasi
                    //kalau gagal dan usernya ada, maka ditambahkan failed login count
                    if (user != null)
                    {
                        //tambah user count nya
                        user.UserFailedLoginCount = Convert.ToInt32(user.UserFailedLoginCount) + 1;
                        if (user.UserFailedLoginCount >= c.GetSetting("UserMaxFailedPassword"))
                        {
                            user.UserBlockedDate = System.DateTime.Now;
                        }
                        c.SaveChanges();

                        if (user.UserBlockedDate.HasValue)
                        {
                            int blockTime = c.GetSetting("UserFailedPasswordBlockTime");
                            var batasBlokir = user.UserBlockedDate.Value.AddMinutes(blockTime);
                            errMsg = "User ID (" + userName + ") anda diblokir sampai tgl : " + String.Format("{0:dd-MMM-yyyy HH:mm:ss}", batasBlokir);
                        }
                        else
                        {
                            if (c.GetSetting("UserFailedPasswordBlockTime") == 1440)
                            {
                                errMsg = string.Format("User ID (" + userName + ") atau Password anda Salah. {0}/{1} kali sebelum anda diblokir selama 24 jam.", user.UserFailedLoginCount, c.GetSetting("UserMaxFailedPassword"));
                            }
                            else {
                                errMsg = string.Format("User ID (" + userName + ") atau Password anda Salah. {0}/{1} kali sebelum anda diblokir selama {2} menit.", user.UserFailedLoginCount, c.GetSetting("UserMaxFailedPassword"), c.GetSetting("UserFailedPasswordBlockTime"));
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errMsg = c.ProcessExceptionMessage(ex);
                if (errMsg.Contains("The server is not operational."))
                {
                    errMsg = "Server LDAP tidak dalam status operasional atau koneksi ke server terputus. Silahkan hubungi Administrator.";
                }
            }

            if (errMsg != "")
            {
                //logging error
                c.LogError(new Exception(errMsg));
                throw new InvalidOperationException(errMsg);
            }
            return null;
        }
    }
}
