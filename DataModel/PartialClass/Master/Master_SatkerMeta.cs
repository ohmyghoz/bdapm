using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(MasterSatkerMeta))]
    public partial class MasterSatker
    {

    }

    public class MasterSatkerMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long SatkerId { get; set; }

        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Kode")]
        public string SatkerKode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Tipe")]
        public string SatkerTipe { get; set; }

        [Display(Name = "Parent Id")]
        public long? SatkerParentId { get; set; }

        [Display(Name = "Kota")]
        public long? RefKotaId { get; set; }


    }


}
