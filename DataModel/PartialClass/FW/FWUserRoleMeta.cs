using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FWUserRoleMeta))]
    public partial class FWUserRole
    {

    }

    public class FWUserRoleMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Urole Id")]
        public long UroleId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(40, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Role Id")]
        public string RoleId { get; set; }

        [Display(Name = "Post Id")]
        public long? PostId { get; set; }


    }


}
