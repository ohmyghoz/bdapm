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
    public class EmailGetter
    {
        DataEntities db;
        public EmailGetter(DataEntities db)
        {
            this.db = db;
        }

       
        public string GetRoleEmail(string roleCsv)
        {
            var emailList = db.getRoleEmail(roleCsv);
            var emailCsv = "";
            foreach (var eml in emailList)
            {
                if (emailCsv != "") emailCsv += ", ";
                emailCsv += eml.email;
            }
            return emailCsv;
        }
        public string GetRoleWithSatkerEmail(string roleCsv, long satkerId)
        {
            var emailList = db.getRoleWithSatkerEmail(roleCsv,satkerId);
            var emailCsv = "";
            foreach (var eml in emailList)
            {
                if (emailCsv != "") emailCsv += ", ";
                emailCsv += eml.email;
            }
            return emailCsv;
        }
    }
    public class DefaultSetting
    {
        private DataEntities db;
        public DefaultSetting(DataEntities db)
        {
            this.db = db;
        }


        public string DefaultSender
        {
            get
            {
                return Convert.ToString(db.GetSetting("Mail_DefaultSender"));
            }
        }
        public string LinkPrefix
        {
            get
            {
                return Convert.ToString(db.GetSetting("Link_Prefix"));
            }
        }
  

        public string Footer
        {
            get
            {
                var nottFooter = db.FWNoticeTemplate.Where(x => x.NottId == "Footer").FirstOrDefault();
                if (nottFooter != null)
                {
                    return nottFooter.NottContent;
                }
                return "";
            }
        }

    }
    public class NoticeTemplateHelper
    {
        private object Model { get; set; }
        //private DynamicViewBag vwBag { get; set; }
        private FWNoticeTemplate Nott { get; set; }
        private DataEntities db;
        private DefaultSetting def;
        private EmailGetter eml;
        private object mydict;
        public NoticeTemplateHelper(DataEntities db, string nottId, object model, object dict = null)
        {
            this.db = db;
            Model = model;
            def = new DefaultSetting(db);
            eml = new EmailGetter(db);
            this.mydict = dict;


            Nott = db.FWNoticeTemplate.Find(nottId);

            if (Nott == null)
            {
                throw new InvalidOperationException("Nott ID tidak ditemukan");
            }
        }

        public const string SEPARATOR = "\n__XXXYXXX___\n";

        private static ConcurrentDictionary<string, RazorEngineCompiledTemplate> TemplateCache = new ConcurrentDictionary<string, RazorEngineCompiledTemplate>();
        private static string RenderTemplate(string templateName, string templateContent, object model)
        {
            int hashCode = templateContent.GetHashCode();
            RazorEngineCompiledTemplate compiledTemplate = TemplateCache.GetOrAdd(templateName + hashCode.ToString(), i =>
            {
                RazorEngine razorEngine = new RazorEngine();
                return razorEngine.Compile(templateContent);
            });

            return compiledTemplate.Run(model);
        }

        public FWNotice GenerateNotice(bool doSaveChanges = true)
        {
            if (Nott == null) throw new InvalidOperationException("Nott tidak ada");

            var notice = new FWNotice();

            // cukup 1 kali parse supaya cepat (Razor Engine LEMOT!)
            // kalaupun ketemu separator di template, langusng set jadi "".
            var template = "";
            template = (Nott.NottTitle ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottTo ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottCc ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottBcc ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottBatch ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottSender ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottContent ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottSmallContent ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottRefId ?? "").Replace(SEPARATOR, "") + SEPARATOR +
                (Nott.NottRdefParamCsv ?? "").Replace(SEPARATOR, "");


            var mdl = new
            {
                Obj = Model,
                EmailGetter = eml,
                DefaultSetting = def,
                dict = mydict
            };

            //var result = Engine.Razor.RunCompile(template, Nott.nott_id, null, Model, vwBag);
            var result = RenderTemplate(Nott.NottId, template, mdl);
            var temp = result.Split(new string[] { SEPARATOR }, StringSplitOptions.None);
            notice.NoticeTitle = temp[0];
            notice.NoticeTo = temp[1];
            notice.NoticeCc = temp[2];
            notice.NoticeBcc = temp[3];
            notice.NoticeBatchUsers = temp[4];
            notice.NoticeSender = temp[5];
            notice.NoticeContent = temp[6];
            notice.NoticeSmallContent = temp[7];
            notice.NoticeRefId = temp[8];
            notice.NoticeRdefParamCsv = temp[9];

            notice.FWNoticeTemplate = Nott;

            //untuk attachment * link dengan reportdef
            //if (Nott.rdef_kode != null)
            //{
            //    var rpt = ReportDefHelper.GetReportByDefKode(Nott.rdef_kode);
            //    if (!String.IsNullOrWhiteSpace(notice.notice_rdef_param_csv))
            //    {
            //        ReportDefHelper.SetReportParameterFromCsv(rpt, notice.notice_rdef_param_csv);
            //    }
            //    using (var strm = new System.IO.MemoryStream())
            //    {
            //        rpt.ExportToPdf(strm);
            //        var att = FWL.SaveAttachment(Guid.NewGuid(), Nott.rdef_kode + ".pdf", strm);
            //        att.FW_Notice = notice;
            //        c.FW_Attachment.Add(att);
            //    }
            //}

            db.SetStsrcFields(notice);
            db.FWNotice.Add(notice);

            //insert ke email queue khusus yang ada to/cc/bcc
            if (!(string.IsNullOrEmpty(notice.NoticeTo) & string.IsNullOrEmpty(notice.NoticeCc)))
            {
                if (notice.FWNoticeTemplate.NottOneEmailPerUser)
                {
                    string allEmail = notice.NoticeTo + "," + notice.NoticeCc;
                    string[] emailTos = allEmail.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> alreadyInserted = new List<string>();

                    foreach (string tempTo in emailTos)
                    {
                        var emailTo = tempTo.Trim();
                        if (emailTo != "")
                        {
                            //mencegah supaya dobel dikirim nya
                            if (!alreadyInserted.Contains(emailTo))
                            {
                                alreadyInserted.Add(emailTo);

                                FWEmailQueue eq = new FWEmailQueue();
                                eq.EmailqTo = emailTo;
                                //bedanya cuma disini
                                eq.EmailqFrom = notice.NoticeSender;
                                eq.EmailqProcess = notice.FWNoticeTemplate.NottId;
                                eq.FWNotice = notice;
                                eq.EmailqQueueDate = DateTime.Now;
                                eq.EmailqScheduledSent = DateTime.Now;
                                eq.EmailqSentTry = 0;
                                eq.EmailqStatus = 0;
                                eq.EmailqBcc = notice.NoticeBcc;
                                eq.EmailqSubject = notice.NoticeTitle;
                                eq.EmailqBody = notice.NoticeContent;
                                db.SetStsrcFields(eq);
                                db.FWEmailQueue.Add(eq);
                            }
                        }
                    }
                }
                else
                {
                    FWEmailQueue eq = new FWEmailQueue();
                    //bedanya cuma disini
                    eq.EmailqTo = notice.NoticeTo;
                    eq.EmailqCc = notice.NoticeCc;
                    eq.EmailqBcc = notice.NoticeBcc;

                    eq.EmailqFrom = notice.NoticeSender;
                    eq.EmailqProcess = notice.FWNoticeTemplate.NottId;
                    eq.FWNotice = notice;
                    eq.EmailqQueueDate = DateTime.Now;
                    eq.EmailqScheduledSent = DateTime.Now;
                    eq.EmailqSentTry = 0;
                    eq.EmailqStatus = 0;
                    eq.EmailqSubject = notice.NoticeTitle;
                    eq.EmailqBody = notice.NoticeContent;
                    db.SetStsrcFields(eq);
                    db.FWEmailQueue.Add(eq);
                }
            }

            var needprocessed = from q in db.FWNotice where q.Stsrc == "A" & q.NoticeBatchStatus == false & q.NoticeBatchUsers.Trim() != "" select q;

            foreach (var nt in needprocessed)
            {
                string[] userIds = nt.NoticeBatchUsers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                List<string> alreadySentUserIds = new List<string>();
                foreach (string tempUser in userIds)
                {
                    var userId = tempUser.Trim().ToLower();
                    if (userId != "" & !userId.Contains("@") & !alreadySentUserIds.Contains(userId))
                    {
                        FWNoticeQueue nq = new FWNoticeQueue();
                        nq.NotqContent = nt.NoticeSmallContent.Replace("\n", "<br />");
                        nq.NotqStatus = 0;
                        nq.NottDate = DateTime.Now;
                        nq.NotqTitle = nt.NoticeTitle;
                        nq.NotqUser = userId;
                        nq.NottId = nt.NottId;
                        db.FWNoticeQueue.Add(nq);
                        db.SetStsrcFields(nq);
                        //supaya tidak dobel-dobel
                        alreadySentUserIds.Add(userId);
                    }
                }
                nt.NoticeBatchStatus = true;
            }

            if (doSaveChanges)
            {
                db.SaveChanges();
            }

            return notice;
        }

    }

    public class NoticeHelper
    {
       
    }

}
