
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FWRefSettingMeta))]
    public partial class FWRefSetting
    {

    }

    public class FWRefSettingMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Name")]
        public string SetName { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Type")]
        public string SetType { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Value")]
        public string SetValue { get; set; }

        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Catatan")]
        public string SetCatatan { get; set; }

        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Group")]
        public string SetGroup { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Foruser")]
        public bool SetForuser { get; set; }
    }


}
