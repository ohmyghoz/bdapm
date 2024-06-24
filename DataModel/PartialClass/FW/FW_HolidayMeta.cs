using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FWHolidayMeta))]
    public partial class FWHoliday
    {

    }

    public class FWHolidayMeta
    {
      

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Tanggal")]
        public DateTime HolidayDate { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Keterangan")]
        public string HolidayKeterangan { get; set; }

    }


}
