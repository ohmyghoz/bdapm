using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(Master_KeteranganrMeta))]
    public partial class Master_Keterangan
    {

    }

    public class Master_KeteranganrMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kode Tabel")]
        public string mk_kode { get; set; }

        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama Menu")]
        public string mk_menu { get; set; }

        [StringLength(4000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Informasi Tambahan")]
        public string mk_keterangan { get; set; }

    }


}
