using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDA.DataModel
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ConnString { get; set; }
        public string DataConnString { get; set; }

        public string StagConnString { get; set; }

        public string RecaptchaSecret { get; set; }
        public string RecaptchaSiteKey { get; set; }

        public string ExtraSalt { get; set; }

        public string SuperAdminRolesCSV { get; set; }

        public string LoginTimeOut { get; set; }
        public string ManualUploadTempFolder { get; set; }

        
    }

}
