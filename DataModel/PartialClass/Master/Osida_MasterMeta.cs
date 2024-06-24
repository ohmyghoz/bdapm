using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(osida_masterMeta))]
    public partial class osida_master
    {

    }

    public class osida_masterMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Kode Tabel")]
        public string kode { get; set; }

        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Judul Laporan")]
        public string judul { get; set; }

        [StringLength(4000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Skenario Pengujian")]
        public string skenario { get; set; }

        [StringLength(4000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Keterangan Output")]
        public string output { get; set; }

        [StringLength(4000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Keterangan Usulan Tindak Lanjut")]
        public string tindaklanjut { get; set; }

        [StringLength(4000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Logic")]
        public string logic { get; set; }


    }


}
