using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{

    [ModelMetadataType(typeof(RptGrid_ParamMeta))]
    public partial class RptGrid_Param

    {

    }

    public class RptGrid_ParamMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long rgpr_id { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Rg Id")]
        public long rg_id { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kode")]
        public string rgpr_kode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama")]
        public string rgpr_nama { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Catatan")]
        public string rgpr_catatan { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "DataType")]
        public string rgpr_datatype { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "ControlType")]
        public string rgpr_controltype { get; set; }

        [Display(Name = "Allow Null")]
        public bool? rgpr_allow_null { get; set; }

        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Null Text")]
        public string rgpr_null_text { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Value Default")]
        public string rgpr_value_default { get; set; }

        [StringLength(2000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Value Csv")]
        public string rgpr_value_csv { get; set; }

        [StringLength(2000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Value Query")]
        public string rgpr_value_query { get; set; }



    }
}
