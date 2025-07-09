using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SyncWebService.DataModel
{
    public partial class Log_ETL
    {
        public long log_id { get; set; }
        public DateTime log_date { get; set; }
        public string log_tipe { get; set; }
        public string log_periode { get; set; }
        public long log_delete_cnt { get; set; }
        public long log_insert_cnt { get; set; }
        public DateTime log_start { get; set; }
        public DateTime log_end { get; set; }
        public string log_status { get; set; }
        public string log_errmessage { get; set; }
       
    }
}
