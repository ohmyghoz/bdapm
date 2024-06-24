using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{

    [ModelMetadataType(typeof(LOVMeta))]
    public partial class LOV

    {

    }

    public class LOVMeta
    {

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kode DP")]
        public string lov_kode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Deskripsi")]
        public string lov_nama { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Cakupan DP")]
        public string lov_scope { get; set; }





    }
}
