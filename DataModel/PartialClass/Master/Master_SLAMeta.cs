using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(MasterSLAMeta))]
    public partial class MasterSLA

    {

    }

    public class MasterSLAMeta
    {
        //[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Id")]
        //public long ljk_id { get; set; }


        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [MaxLength(250)]
        [Display(Name = "Nama SLA")]
        public string SladName { get; set; }


     
        [Required(ErrorMessage = "[{0}] harus diisi")]
        
        [Display(Name = "Jumlah Hari SLA")]
        public int SladHari { get; set; }

        [StringLength(20, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [MaxLength(20)]
        [Display(Name = "Status SLA")]
        public string SladStatus { get; set; }

        
    }
}
