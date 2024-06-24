using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(Penarikan_DataMeta))]
    public partial class Penarikan_Data
    {

    }

    public class Penarikan_DataMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long mpd_id { get; set; }

        [Display(Name = "Periode")]
        [Required(ErrorMessage = "[{0}] harus diisi")]
        public DateTime mpd_periode { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Jenis")]
        public string mpd_jenis { get; set; }



    }


}
