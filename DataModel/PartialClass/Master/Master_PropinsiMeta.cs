using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(MasterPropinsiMeta))]
    public partial class MasterPropinsi
    {

    }

    public class MasterPropinsiMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long RefPropinsiId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama Provinsi")]
        public string RefPropinsiNama { get; set; }


        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Entrier")]
        public string Entrier { get; set; }

        [StringLength(2, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Sandi")]
        public string RefPropinsiKode { get; set; }


    }


}
