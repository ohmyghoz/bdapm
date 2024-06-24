using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{

    [ModelMetadataType(typeof(MasterSLANoticeMeta))]
    public partial class MasterSLANotice

    {

    }
    class MasterSLANoticeMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long SlanId { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Nama SLA Notice")]
        public string SladName { get; set; }

       
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Slan HK")]
        public int SlanHk { get; set; }


        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Slan Notice Template")]
        public string SlanNottId { get; set; }

        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Slan To")]
        public string SlanTo { get; set; }   
    }
}
