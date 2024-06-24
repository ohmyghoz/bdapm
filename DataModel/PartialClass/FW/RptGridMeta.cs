using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{

    [ModelMetadataType(typeof(RptGridMeta))]
    public partial class RptGrid

    {

    }

    public class RptGridMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long rg_id { get; set; }

        [Display(Name = "Parent Id")]
        public long? parent_id { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama")]
        public string rg_nama { get; set; }

        [StringLength(2000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Catatan")]
        public string rg_catatan { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kode")]
        public string rg_kode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(20, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Tipe")]
        public string rg_tipe { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "DB Name")]
        public string rg_db_name { get; set; }


        [Display(Name = "Query")]
        public string rg_query { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Entrier")]
        public string rg_entrier { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Rolesetting Inherited")]
        public bool rg_rolesetting_inherited { get; set; }

        [StringLength(2000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Grid Setting Default")]
        public string rg_grid_setting_default { get; set; }






    }
}
