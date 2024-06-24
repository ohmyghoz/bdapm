using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using DevExpress.XtraReports.UI;
using BDA.DataModel;

namespace BDA
{
    public class WSReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        private DataEntities db;
        const string FileExtension = ".repx";
        public WSReportStorageWebExtension(DataEntities db)
        {
            this.db = db;
        }

        public override bool CanSetData(string url)
        {
            // Determines whether or not it is possible to store a report by a given URL. 
            // For instance, make the CanSetData method return false for reports that should be read-only in your storage. 
            // This method is called only for valid URLs (i.e., if the IsValidUrl method returned true) before the SetData method is called.

            return true;
        }

        public override bool IsValidUrl(string url)
        {
            // Determines whether or not the URL passed to the current Report Storage is valid. 
            // For instance, implement your own logic to prohibit URLs that contain white spaces or some other special characters. 
            // This method is called before the CanSetData and GetData methods.

            return true;
        }

        public override byte[] GetData(string url)
        {
            // Returns report layout data stored in a Report Storage using the specified URL. 
            // This method is called only for valid URLs after the IsValidUrl method is called.
            var findRep = db.DxReport.Where(x => x.DxRepKode == url && x.DxRepTipe == "Report").FirstOrDefault();
            if(findRep == null)
            {
                throw new InvalidOperationException(string.Format("Could not find report '{0}'.", url));
            }
            else
            {
                return findRep.DxRepXml;
            }
        }

        public override Dictionary<string, string> GetUrls()
        {
            // Returns a dictionary of the existing report URLs and display names. 
            // This method is called when running the Report Designer, 
            // before the Open Report and Save Report dialogs are shown and after a new report is saved to a storage.
            var findRepList = db.DxReport.Where(x => x.DxRepTipe == "Report").Select(x => new { x.DxRepKode, x.DxRepNama }).ToDictionary(x=> x.DxRepKode, x=> x.DxRepNama);
            return findRepList;
        }

        public override void SetData(DevExpress.XtraReports.UI.XtraReport report, string url)
        {
            // Stores the specified report to a Report Storage using the specified URL. 
            // This method is called only after the IsValidUrl and CanSetData methods are called.
            var findRep = db.DxReport.Where(x => x.DxRepKode == url && x.DxRepTipe == "Report").FirstOrDefault();
            if (findRep == null)
            {
                throw new InvalidOperationException(string.Format("Could not find report '{0}'.", url));
            }
            else
            {
                findRep.DxRepNama = report.DisplayName;
                using (var ms = new MemoryStream())
                {
                    report.SaveLayoutToXml(ms);
                    var reportBytes = ms.ToArray();
                    findRep.DxRepXml = reportBytes;
                }
                db.SaveChanges();
            }            
        }

        public override string SetNewData(DevExpress.XtraReports.UI.XtraReport report, string defaultUrl)
        {
            var newRep = new DxReport();
            newRep.DxRepTipe = "Report";
            newRep.DxRepNama = report.DisplayName;
            newRep.DxRepKode = defaultUrl;
            using (var ms = new MemoryStream())
            {
                report.SaveLayoutToXml(ms);
                var reportBytes = ms.ToArray();
                newRep.DxRepXml = reportBytes;
            }
            db.DxReport.Add(newRep);
            db.SaveChanges();
            return defaultUrl;
        }
    }
}