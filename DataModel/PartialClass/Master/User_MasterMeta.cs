using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(User_MasterMeta))]
    public partial class UserMaster
    {

    }
    public class User_MasterMeta
    {
        //[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "User ID")]
        //[StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        ////[RegularExpression(@"^[\S]+$", ErrorMessage = "[{0}] harus berupa alfanumerik")]
        //public string UserId { get; set; }

        //[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Nama")]
        //[StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        ////[RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "[{0}] harus berupa huruf dan spasi")]
        //public string UserNama { get; set; }

        ////[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Password")]
        //[StringLength(100, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        //public string UserPassword { get; set; }

        //[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Email")]
        //[StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        ////[RegularExpression(@"^([\w\.\-]+)@((?!\.|\-)[\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Format email tidak sesuai")]
        //public string UserEmail { get; set; }

        ////[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Alamat")]
        //[StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        //public string UserAlamat { get; set; }

        ////[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Telepon")]
        //[StringLength(100, ErrorMessage = "Panjang [{0}] maksimal {1} karakter")]
        //public string UserTelp { get; set; }

        //[Required(ErrorMessage = "[{0}] harus diisi")]
        //[Display(Name = "Role")]
        //public string UserMainRole { get; set; }

        //[Display(Name = "Send Red Alert?")]
        //public string user_is_notifredalert { get; set; }

        //[Display(Name = "Send Yellow Alert?")]
        //public string user_is_notifyellowalert { get; set; }
    }
}
