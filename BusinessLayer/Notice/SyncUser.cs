using BDA.DataModel;
using RazorEngineCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.BusinessLayer
{
    public class syncUser
    {
        DataEntities db;
        public syncUser(DataEntities db)
        {
            this.db = db;
        }
        public void AddUser()
        {
            using (db = new DataEntities())
            {
                var rp = (from q in db.FWRefRole
                          where q.Stsrc == "A" && q.RoleName == "PengawasLJK"
                          select q).ToList();
                var listUser = db.getListUserSync().ToList();
                foreach (var i in listUser)
                {
                    var chkStsrcUser = db.UserMaster.Find(i.user_login_id);
                    if (chkStsrcUser != null)
                    {
                        chkStsrcUser.Stsrc = "A";
                        chkStsrcUser.UserLdap = i.user_login_id;
                        chkStsrcUser.UserEmail = i.user_login_id;
                        chkStsrcUser.UserPassword = "XXX";
                        chkStsrcUser.UserNama = i.user_login_id.Replace("@ojk.go.id", "");
                        chkStsrcUser.UserMainRole = "PengawasLJK";
                        chkStsrcUser.UserFailedLoginCount = 0;
                        chkStsrcUser.UserStatus = "Aktif";
                        chkStsrcUser.UserRolesCsv = "PengawasLJK";
                        db.SetStsrcFields(chkStsrcUser);
                        foreach (var r in rp)
                        {
                            var urole = new FWUserRole();
                            urole.RoleId = Convert.ToInt64(r.RoleId);
                            urole.UserMaster = chkStsrcUser;
                            db.FWUserRole.Add(urole);
                            db.SetStsrcFields(urole);
                        }
                    }
                    else
                    {
                        var usr = new UserMaster();
                        usr.UserId = i.user_login_id;
                        usr.UserLdap = i.user_login_id;
                        usr.UserEmail = i.user_login_id;
                        usr.UserPassword = "XXX";
                        usr.UserNama = i.user_login_id.Replace("@ojk.go.id", "");
                        usr.UserMainRole = "PengawasLJK";
                        usr.UserFailedLoginCount = 0;
                        usr.UserStatus = "Aktif";
                        usr.UserRolesCsv = "PengawasLJK";
                        db.UserMaster.Add(usr);
                        db.SetStsrcFields(usr);
                        foreach (var r in rp)
                        {
                            var urole = new FWUserRole();
                            urole.RoleId = Convert.ToInt64(r.RoleId);
                            urole.UserMaster = usr;
                            db.FWUserRole.Add(urole);
                            db.SetStsrcFields(urole);
                        }
                    }
                }
                db.SaveChanges();
            }
            
        }
        public void DeleteUser()
        {
            using (db = new DataEntities())
            {
                var listDelete = db.getListUserPengawasForDelete().ToList();
                foreach (var ld in listDelete)
                {
                    var um = db.UserMaster.Find(ld.UserId);
                    var listUserRole = (from q in db.FWUserRole
                                        where q.Stsrc == "A" && q.UserId == ld.UserId
                                        select q).ToList();
                    var countRole=listUserRole.Count();
                    int i =0;
                    foreach (var i2 in listUserRole)
                    {
                        var rr=db.FWRefRole.Find(i2.RoleId);
                        if (rr.RoleName == "PengawasLJK") {
                            db.DeleteStsrc(i2);
                            i = i + 1;
                        } 
                        
                    }
                    if (countRole == i) {
                        db.DeleteStsrc(um);
                    }
                }
                db.SaveChanges();
            }


        }
    }
}
