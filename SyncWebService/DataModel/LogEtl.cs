using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SyncWebService.DataModel
{
    public partial class LogEtl
    {
        public long LogId { get; set; }

        public DateTime LogDate { get; set; }

        public string LogTipe { get; set; } = null!;

        public string? LogPeriode { get; set; }

        public long? LogDeleteCnt { get; set; }

        public long? LogInsertCnt { get; set; }

        public DateTime? LogStart { get; set; }

        public DateTime? LogEnd { get; set; }

        public string? LogStatus { get; set; }

        public string? LogErrmessage { get; set; }

    }
}
