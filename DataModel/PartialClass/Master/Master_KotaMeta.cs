using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(Master_KotaMeta))]
    public partial class Master_Kota
    {

    }

    public class Master_KotaMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Kota Id")]
        public long RefKotaId { get; set; }

        [Display(Name = "Domisili Kota")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        public long RefKotaDomisili { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama Kota")]
        public string RefKotaNama { get; set; }

        [Display(Name = "Propinsi")]
      
        public long? RefPropinsiId { get; set; }
    
        [StringLength(4, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kode Kota")]
        public string RefKotaKode { get; set; }


    }


}
