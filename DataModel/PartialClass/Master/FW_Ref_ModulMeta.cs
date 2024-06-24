using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FW_Ref_ModulMeta))]
    public partial class FWRefModul
    {

    }
    public class FW_Ref_ModulMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "ID Modul")]
        public long ModId { get; set; }

        [Display(Name = "Modul Parent")]
        public long? ParentModId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Kode Modul")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string ModKode { get;set; }

        //[Required(ErrorMessage = "[{0}} harus diisi")]
        //[Display(Name = "Nama Modul")]
        //[StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        //public string ModNama { get; set; }

        [Display(Name = "Catatan")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string ModCatatan { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Urutan")]
        public long ModUrut { get; set; }

        [Display(Name = "Modul Publik?")]
        public bool ModIsPublic { get; set; }

        [Display(Name = "Modul Disembunyikan?")]
        public bool ModIsHidden { get; set; }
        [Display(Name = "Tooltip")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string ModTooltip { get; set; }
    }
}
