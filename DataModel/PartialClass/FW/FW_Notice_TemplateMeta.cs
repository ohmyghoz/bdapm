using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FWNoticeTemplateMeta))]
    public partial class FWNoticeTemplate
    {

    }
    public class FWNoticeTemplateMeta
    {
      
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "ID Notifikasi")]
        public string NottId { get; set; }
      
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Judul Notifikasi")]
        public string NottTitle { get; set; }

        //[RegularExpression(@"^([\w\.\-]+)@((?!\.|\-)[\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Format email tidak sesuai")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Kepada")]
        public string NottTo { get; set; }


        //[StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        //[RegularExpression("^[1-9]\\d*$", ErrorMessage = "[{0}] harus diisi.")]
        //[RegularExpression(@"^([\w\.\-]+)@((?!\.|\-)[\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Format email tidak sesuai")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Pengirim")]
        public string NottSender { get; set; }

        //[RegularExpression(@"^([\w\.\-]+)@((?!\.|\-)[\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Format email tidak sesuai")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "CC")]
        public string NottCc { get; set; }

        //[RegularExpression(@"^([\w\.\-]+)@((?!\.|\-)[\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Format email tidak sesuai")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "BCC")]
        public string NottBcc { get; set; }

        //[RegularExpression("^[1-9]\\d*$", ErrorMessage = "[{0}] harus diisi.")]
        [Display(Name = "Deskripsi Notifikasi")]
        public string NottContent { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        //[RegularExpression("^[1-9]\\d*$", ErrorMessage = "[{0}] harus diisi.")]
        [Display(Name = "ID Referensi")]
        public string NottRefId { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        //[RegularExpression("^[1-9]\\d*$", ErrorMessage = "[{0}] harus diisi.")]
        [Display(Name = "Catatan")]
        public string NottCatatan { get; set; }
    }
}
