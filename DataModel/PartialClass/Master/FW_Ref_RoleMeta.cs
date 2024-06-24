using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FW_Ref_RoleMeta))]
    public partial class FWRefRole
    {

    }
    public class FW_Ref_RoleMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Role")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string RoleName { get; set; }


        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Catatan")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        public string RoleCatatan { get; set; }
    }
}
