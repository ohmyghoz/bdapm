using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FWKodeDetailMeta))]
    public partial class FwKodeDetail
    {

    }

    public class FWKodeDetailMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long KodId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(20, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kof Id")]
        public string KofId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(20, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Tipe")]
        public string KodTipe { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Urut")]
        public int KodUrut { get; set; }

        [Display(Name = "Length")]
        public int KodLength { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Catatan")]
        public string KodCatatan { get; set; }

        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Char")]
        public string KodChar { get; set; }

        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Param Kode")]
        public string KodParamKode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Param As Counter")]
        public bool KodParamAsCounter { get; set; }


    }


}
