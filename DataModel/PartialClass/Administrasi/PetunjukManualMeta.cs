using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(PetunjukManualMeta))]
    public partial class PetunjukManual
    {

    }

    public class PetunjukManualMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Nama Dokumen Publik")]
        public string PublicDocumentName { get; set; }
        
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(200, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Deskripsi Dokumen Publik")]
        public string PublicDocumentDesc { get; set; }

        //[Required(ErrorMessage = "[{0}] harus diisi")]
        //[StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        //[Display(Name = "Dokumen")]
        //public FileContentResult PublicDocumentPath { get; set; }


    }


}
