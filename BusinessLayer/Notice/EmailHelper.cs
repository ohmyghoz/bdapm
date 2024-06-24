using MailKit;
using MailKit.Net.Smtp;
using Microsoft.CodeAnalysis.Options;
using MimeKit;
using BDA.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BDA.BusinessLayer
{
    public struct DateTimeSpan
    {
        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }
    public class AlertEmail
    {
        public string tipePeriode { get; set; }
        public string objDate { get; set; }
        public string levelAlert { get; set; }
        public string TotalAlert { get; set; }
        public string email_to { get; set; }
        public string dateProcess { get; set; }
        public string totalPelapor { get; set; }
        public string htmlTable { get; set; }
    }
    public class EmailHelper
    {
        DataEntities db;
        public EmailHelper(DataEntities db)
        {
            this.db = db;
        }

        public void SendEmail(FWEmailQueue eqObj = null)
        {
            using (db = new DataEntities())
            {
                if (eqObj != null)
                {
                    SendEmail(eqObj, db);
                    eqObj.EmailqStatus = 1;
                    eqObj.EmailqSentDate = DateTime.Now;
                    db.SetStsrcModBySystem(eqObj);
                    db.SaveChanges();
                }
                else
                {
                    foreach (var eq in db.FWEmailQueue.Where(x => x.Stsrc == "A" & x.EmailqStatus == 0 || x.EmailqStatus == 3 &&
                                                            x.EmailqSentTry < 5).Take(100).ToList())
                    {
                        try
                        {
                            SendEmail(eq, db);
                            //Console.WriteLine("Sent : " + eq.emailq_subject + " [" + eq.emailq_to + "]");
                            eq.EmailqStatus = 1;
                            db.SetStsrcModBySystem(eq);
                            db.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            eq.EmailqStatus = 3;
                            eq.EmailqErrorText = ex.Message;
                            //Console.WriteLine("Error : " + eq.emailq_subject + " [" + eq.emailq_id + "] : " + Environment.NewLine + ex.ToString());
                            db.SetStsrcModBySystem(eq);
                            db.SaveChanges();
                        }

                    }
                }
                //ambil queue per 100                
            }
        }

        public static void SendEmail(FWEmailQueue eq, DataEntities db)
        {
            var receiver = eq.EmailqTo;
            var subject = eq.EmailqSubject;
            var sender = eq.EmailqFrom;
            var body = eq.EmailqBody;

            //hilangkan duplicated email
            List<string> emailList = new List<string>();

            //send ke "To"
            string[] toList = receiver.Split(","[0]);
            receiver = "";
            bool first = true;
            foreach (var tempEmail in toList)
            {
                var email = tempEmail.Trim();
                if (email != "")
                {
                    if (!emailList.Contains(email))
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            receiver += ",";
                        }
                        receiver += email;
                        emailList.Add(email);
                    }
                }
            }

            MimeMessage message = new MimeMessage();

            //sender
            message.From.Add(new MailboxAddress(sender));

            //TO/CC/BCC
            string debugMail = db.GetSetting("Mail_DebugMail");
            if (String.IsNullOrWhiteSpace(debugMail))
            {
                foreach (var email in emailList)
                {
                    message.To.Add(new MailboxAddress(email));
                }

                if (!String.IsNullOrWhiteSpace(eq.EmailqCc))
                {
                    foreach (var email in eq.EmailqCc.Split(","[0]).Distinct().ToList())
                    {
                        message.Cc.Add(new MailboxAddress(email));
                    }
                }

                if (!String.IsNullOrWhiteSpace(eq.EmailqBcc))
                {
                    foreach (var email in eq.EmailqBcc.Split(","[0]).Distinct().ToList())
                    {
                        message.Bcc.Add(new MailboxAddress(email));
                    }
                }
            }
            else
            {
                foreach (var email in debugMail.Split(","[0]).Distinct().ToList())
                {
                    message.To.Add(new MailboxAddress(email));
                }
                //message.To.Add(new MailboxAddress(db.GetSetting("Mail_DebugMail").ToString()));
            }




            //subject
            subject = subject.Replace(Environment.NewLine, "");
            message.Subject = subject;


            //body
            //if (debugMail != "")
            //{
            //    string debugHeader = "TO : ";
            //    foreach (string temp in emailList)
            //    {
            //        debugHeader += temp + ", ";
            //    }
            //    debugHeader += "<br/>";

            //    if (!String.IsNullOrWhiteSpace(eq.emailq_cc))
            //        debugHeader += "CC : " + eq.emailq_cc + "<br />";


            //    if (!String.IsNullOrWhiteSpace(eq.emailq_bcc))
            //        debugHeader += "BCC : " + eq.emailq_bcc;

            //    debugHeader += "<br /><br/>";
            //    body = debugHeader + body;
            //}


            var builder = new BodyBuilder();


            builder.HtmlBody = body;
            //if (eq.FW_Notice.FW_Attachment.Any())
            //{
            //    foreach (var att in eq.FW_Notice.FW_Attachment)
            //    {
            //        var strm = db.GetFileFromAttachment(att);
            //        strm.Position = 0;

            //        builder.Attachments.Add(att.attach_file_nama, strm);
            //    }
            //}


            message.Body = builder.ToMessageBody();


            //attachment
            using (SmtpClient client = new SmtpClient())
            {
                Boolean s = true;
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = AcceptAllCertificate;
                client.Connect(db.GetSetting("Mail_SMTP_Server").ToString(), Convert.ToInt32(db.GetSetting("Mail_SMTP_Port")), Convert.ToBoolean(db.GetSetting("Mail_SMTP_UseSSL")));
                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate(db.GetSetting("Mail_Username").ToString(), db.GetSetting("Mail_Password").ToString());

                client.Send(message);
                eq.EmailqErrorText = message.MessageId;
                client.Disconnect(true);
            }

        }

        public void GeneraterAlertEmailDaily()
        {
            using (db = new DataEntities())
            {

                var start = new TimeSpan(0, 0, 0);
                var end = new TimeSpan(7, 0, 0);
                var lastDateExecute = db.GetSetting("AlertDailyLastDate");
                if (DateTime.Now.TimeOfDay >= start && DateTime.Now.TimeOfDay <= end)
                {
                    DateTime lastDateAlert = db.GetSetting("AlertDailyLastDate");
                    var dateSpan = (DateTime.Now - lastDateAlert).Days;
                    if (dateSpan > 0)
                    {
                        for (int ld = 0; ld < dateSpan; ld++)
                        {
                            var listPengawas = db.getPengawasDailyALert(DateTime.Now.AddDays(ld - dateSpan + 1), "Harian");
                            foreach (var i in listPengawas)
                            {
                                var usr = db.UserMaster.Find(i.user_login_id);

                                var d = new AlertEmail();
                                d.tipePeriode = "Daily";
                                d.objDate = DateTime.Now.AddDays(-1).ToString("dd MMM yyyy");

                                d.email_to = usr.UserEmail;
                                d.dateProcess = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
                                d.totalPelapor = db.getTotalPelapor(i.user_login_id).FirstOrDefault().totalPelapor.ToString();
                                if (usr.user_is_notifredalert == true)
                                {
                                    d.levelAlert = "Red";
                                    var listData = db.GetDataAlert(i.user_login_id, "3", DateTime.Now.AddDays(ld - dateSpan), "Harian", Base64Encode(i.user_login_id)).ToList();
                                    if (listData.Count() != 0)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        using (Html.Table table = new Html.Table(sb, id: "some-id"))
                                        {
                                            table.StartHead();
                                            using (var thead = table.AddRow())
                                            {
                                                thead.AddCell("No");
                                                thead.AddCell("Tipe");
                                                thead.AddCell("Jumlah Pelapor");
                                                thead.AddCell("Jumlah Alert");
                                                thead.AddCell("Total Permintaan");
                                                thead.AddCell("Individu");
                                                thead.AddCell("Non Individu");
                                                thead.AddCell("Batch");
                                                thead.AddCell("Interactive");
                                            }
                                            table.EndHead();
                                            table.StartBody();
                                            int no = 1;
                                            int totALert = 0;
                                            foreach (var alert in listData)
                                            {

                                                using (var tr = table.AddRow(classAttributes: "someattributes"))
                                                {
                                                    tr.AddCell(no.ToString());
                                                    tr.AddCell(alert.tipe);
                                                    tr.AddCell(alert.jumlahPelapor.ToString());
                                                    tr.AddCell(alert.jumlahAlert.ToString());
                                                    tr.AddCell(alert.tp);
                                                    tr.AddCell(alert.ind);
                                                    tr.AddCell(alert.nind);
                                                    tr.AddCell(alert.bat);
                                                    tr.AddCell(alert.inte);
                                                }
                                                no = no + 1;
                                                totALert = totALert + Convert.ToInt32(alert.jumlahAlert);
                                            }
                                            table.EndBody();
                                            d.htmlTable = sb.ToString();
                                            d.TotalAlert = totALert.ToString();
                                            var nott = new NoticeTemplateHelper(db, "Alert Email", d);
                                            nott.GenerateNotice(false);
                                        }
                                    }
                                }
                                if (usr.user_is_notifyellowalert == true)
                                {
                                    d.levelAlert = "Yellow";
                                    var listData = db.GetDataAlert(i.user_login_id, "2", DateTime.Now.AddDays(ld - dateSpan), "Harian", Base64Encode(i.user_login_id)).ToList();
                                    if (listData.Count() != 0)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        using (Html.Table table = new Html.Table(sb, id: "some-id"))
                                        {
                                            table.StartHead();
                                            using (var thead = table.AddRow())
                                            {
                                                thead.AddCell("No");
                                                thead.AddCell("Tipe");
                                                thead.AddCell("Jumlah Pelapor");
                                                thead.AddCell("Jumlah Alert");
                                                thead.AddCell("Total Permintaan");
                                                thead.AddCell("Individu");
                                                thead.AddCell("Non Individu");
                                                thead.AddCell("Batch");
                                                thead.AddCell("Interactive");
                                            }
                                            table.EndHead();
                                            table.StartBody();
                                            int no = 1;
                                            int totALert = 0;
                                            foreach (var alert in listData)
                                            {

                                                using (var tr = table.AddRow(classAttributes: "someattributes"))
                                                {
                                                    tr.AddCell(no.ToString());
                                                    tr.AddCell(alert.tipe);
                                                    tr.AddCell(alert.jumlahPelapor.ToString());
                                                    tr.AddCell(alert.jumlahAlert.ToString());
                                                    tr.AddCell(alert.tp);
                                                    tr.AddCell(alert.ind);
                                                    tr.AddCell(alert.nind);
                                                    tr.AddCell(alert.bat);
                                                    tr.AddCell(alert.inte);
                                                }
                                                no = no + 1;
                                                totALert = totALert + Convert.ToInt32(alert.jumlahAlert);
                                            }
                                            table.EndBody();
                                            d.htmlTable = sb.ToString();
                                            d.TotalAlert = totALert.ToString();
                                            var nott = new NoticeTemplateHelper(db, "Alert Email", d);
                                            nott.GenerateNotice(false);
                                        }
                                    }
                                }
                            }
                            if (listPengawas.Count() != 0) {
                                lastDateExecute = DateTime.Now.AddDays(ld - dateSpan + 1);
                            }
                        }
                    }

                }
                var rs = db.FWRefSetting.Find("AlertDailyLastDate");
                rs.SetValue = lastDateExecute.ToString();
                db.SaveChanges();
            }

        }
        public void GeneraterAlertEmailMonthly()
        {
            using (db = new DataEntities())
            {
                if (DateTime.Now.Day == 1)
                {
                    var start = new TimeSpan(0, 0, 0);
                    var end = new TimeSpan(7, 0, 0);
                    var lastDateExecute = db.GetSetting("AlertMonthlyLastDate");
                    if (DateTime.Now.TimeOfDay >= start && DateTime.Now.TimeOfDay <= end)
                    {
                        DateTime lastDateAlert = db.GetSetting("AlertMonthlyLastDate");
                        var dateSpan = DateTimeSpan.CompareDates(lastDateAlert, DateTime.Now);
                        if (dateSpan.Months > 0)
                        {
                            for (int lm = 0; lm < dateSpan.Months; lm++)
                            {
                                var listPengawas = db.getPengawasDailyALert(DateTime.Now.AddMonths(lm - dateSpan.Months + 1), "Bulanan");
                                foreach (var i in listPengawas)
                                {
                                    var usr = db.UserMaster.Find(i.user_login_id);

                                    var d = new AlertEmail();
                                    d.tipePeriode = "Monthly";
                                    d.objDate = DateTime.Now.AddDays(-1).ToString("MMM yyyy");

                                    d.email_to = usr.UserEmail;
                                    d.dateProcess = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
                                    d.totalPelapor = db.getTotalPelapor(i.user_login_id).FirstOrDefault().totalPelapor.ToString();
                                    if (usr.user_is_notifredalert == true)
                                    {
                                        d.levelAlert = "Red";
                                        var listData = db.GetDataAlert(i.user_login_id, "3", DateTime.Now.AddMonths(lm - dateSpan.Months + 1), "Bulanan", Base64Encode(i.user_login_id)).ToList();
                                        if (listData.Count() != 0)
                                        {
                                            StringBuilder sb = new StringBuilder();
                                            using (Html.Table table = new Html.Table(sb, id: "some-id"))
                                            {
                                                table.StartHead();
                                                using (var thead = table.AddRow())
                                                {
                                                    thead.AddCell("No");
                                                    thead.AddCell("Tipe");
                                                    thead.AddCell("Jumlah Pelapor");
                                                    thead.AddCell("Jumlah Alert");
                                                    thead.AddCell("Total Permintaan");
                                                    thead.AddCell("Individu");
                                                    thead.AddCell("Non Individu");
                                                    thead.AddCell("Batch");
                                                    thead.AddCell("Interactive");
                                                }
                                                table.EndHead();
                                                table.StartBody();
                                                int no = 1;
                                                int totALert = 0;
                                                foreach (var alert in listData)
                                                {

                                                    using (var tr = table.AddRow(classAttributes: "someattributes"))
                                                    {
                                                        tr.AddCell(no.ToString());
                                                        tr.AddCell(alert.tipe);
                                                        tr.AddCell(alert.jumlahPelapor.ToString());
                                                        tr.AddCell(alert.jumlahAlert.ToString());
                                                        tr.AddCell(alert.tp);
                                                        tr.AddCell(alert.ind);
                                                        tr.AddCell(alert.nind);
                                                        tr.AddCell(alert.bat);
                                                        tr.AddCell(alert.inte);
                                                    }
                                                    no = no + 1;
                                                    totALert = totALert + Convert.ToInt32(alert.jumlahAlert);
                                                }
                                                table.EndBody();
                                                d.htmlTable = sb.ToString();
                                                d.TotalAlert = totALert.ToString();
                                                var nott = new NoticeTemplateHelper(db, "Alert Email", d);
                                                nott.GenerateNotice(false);
                                            }
                                        }
                                    }
                                    if (usr.user_is_notifyellowalert == true)
                                    {
                                        d.levelAlert = "Yellow";
                                        var listData = db.GetDataAlert(i.user_login_id, "2", DateTime.Now.AddMonths(lm - dateSpan.Months + 1), "Bulanan", Base64Encode(i.user_login_id)).ToList();
                                        if (listData.Count() != 0)
                                        {
                                            StringBuilder sb = new StringBuilder();
                                            using (Html.Table table = new Html.Table(sb, id: "some-id"))
                                            {
                                                table.StartHead();
                                                using (var thead = table.AddRow())
                                                {
                                                    thead.AddCell("No");
                                                    thead.AddCell("Tipe");
                                                    thead.AddCell("Jumlah Pelapor");
                                                    thead.AddCell("Jumlah Alert");
                                                    thead.AddCell("Total Permintaan");
                                                    thead.AddCell("Individu");
                                                    thead.AddCell("Non Individu");
                                                    thead.AddCell("Batch");
                                                    thead.AddCell("Interactive");
                                                }
                                                table.EndHead();
                                                table.StartBody();
                                                int no = 1;
                                                int totALert = 0;
                                                foreach (var alert in listData)
                                                {

                                                    using (var tr = table.AddRow(classAttributes: "someattributes"))
                                                    {
                                                        tr.AddCell(no.ToString());
                                                        tr.AddCell(alert.tipe);
                                                        tr.AddCell(alert.jumlahPelapor.ToString());
                                                        tr.AddCell(alert.jumlahAlert.ToString());
                                                        tr.AddCell(alert.tp);
                                                        tr.AddCell(alert.ind);
                                                        tr.AddCell(alert.nind);
                                                        tr.AddCell(alert.bat);
                                                        tr.AddCell(alert.inte);
                                                    }
                                                    no = no + 1;
                                                    totALert = totALert + Convert.ToInt32(alert.jumlahAlert);
                                                }
                                                table.EndBody();
                                                d.htmlTable = sb.ToString();
                                                d.TotalAlert = totALert.ToString();
                                                var nott = new NoticeTemplateHelper(db, "Alert Email", d);
                                                nott.GenerateNotice(false);
                                            }
                                        }
                                    }
                                }
                                if (listPengawas.Count() != 0)
                                {
                                    lastDateExecute = DateTime.Now.AddMonths(lm - dateSpan.Months + 1);
                                }
                            }
                        }

                    }
                    var rs = db.FWRefSetting.Find("AlertMonthlyLastDate");
                    rs.SetValue = lastDateExecute.ToString();
                    db.SaveChanges();
                }

            }

        }
        private static bool AcceptAllCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
