using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FW_Ref_Modul_MenuMeta))]
    public partial class FWRefModulMenu
    {

    }
    public class FW_Ref_Modul_MenuMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "ID Menu")]
        public long ModMenuId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "ID Modul")]
        public long ModId { get; set; }

        //[Display(Name = "Nama Menu")]
        //[StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        //public string ModMenuNama { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Aksi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string ModMenuAksi { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Url")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string ModMenuUrl { get; set; }

        [Display(Name = "Menu Disembunyikan?")]
        public bool ModMenuIsHidden { get; set; }
    }
}
