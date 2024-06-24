using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SyncWebService.DataModel
{
    public partial class HiveSync
    {
        public long sync_id { get; set; }
        public string pprocess { get; set; }
        public string pperiode { get; set; }
        public string sync_status { get; set; }
    }
}
