using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FWWorkdayMeta))]
    public partial class FWWorkday
    {

    }

    public class FWWorkdayMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(10, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Hari")]
        public string WorkdayDayName { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Mulai")]
        public DateTime WorkdayStart { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Selesai")]
        public DateTime WorkdayEnd { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Range(1753, 3000, ErrorMessage = "Nilai {0} harus bernilai diatara {1} and {2}.")]
        [Display(Name = "Tahun")]
        public int WorkdayYear { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(50, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Tipe")]
        public string CalendarType { get; set; }
    }


}
