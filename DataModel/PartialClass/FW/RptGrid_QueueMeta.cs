using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{

    [ModelMetadataType(typeof(RptGrid_QueueMeta))]
    public partial class RptGrid_Queue

    {

    }

    public class RptGrid_QueueMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long rgq_id { get; set; }

        [Display(Name = "Rg Id")]
        public long? rg_id { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Query")]
        public string rgq_query { get; set; }

        [StringLength(4000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Params")]
        public string rgq_params { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama")]
        public string rgq_nama { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Requestor")]
        public string rgq_requestor { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Tgl Antrian")]
        public DateTime rgq_date { get; set; }

        [Display(Name = "Start")]
        public DateTime? rgq_start { get; set; }

        [Display(Name = "End")]
        public DateTime? rgq_end { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(20, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Status")]
        public string rgq_status { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Priority")]
        public Byte rgq_priority { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Urut")]
        public int rgq_urut { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "ErrMsg")]
        public string rgq_error_message { get; set; }

        [Display(Name = "Filesize")]
        public int? rgq_result_filesize { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Filename")]
        public string rgq_result_filename { get; set; }

        [Display(Name = "Rowcount")]
        public int? rgq_result_rowcount { get; set; }

        public string rgq_tablename { get; set; }




    }
}
