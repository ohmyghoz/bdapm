using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BDA.DataModel
{
    [ModelMetadataType(typeof(FwEmailQueueMeta))]
    public partial class FwEmailQueue
    {

    }

    public class FwEmailQueueMeta
    {
        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Id")]
        public long EmailqId { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "To")]
        public string EmailqTo { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "From")]
        public string EmailqFrom { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Cc")]
        public string EmailqCc { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Bcc")]
        public string EmailqBcc { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Reply To")]
        public string EmailqReplyTo { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Subject")]
        public string EmailqSubject { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Body")]
        public string EmailqBody { get; set; }

        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Attch Name1")]
        public string EmailqAttchName1 { get; set; }

        
        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Attch Name2")]
        public string EmailqAttchName2 { get; set; }


        [StringLength(250, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Attch Name3")]
        public string EmailqAttchName3 { get; set; }


        [Display(Name = "Status")]
        public byte? EmailqStatus { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Queue Date")]
        public DateTime EmailqQueueDate { get; set; }

        [Required(ErrorMessage = "[{0}] harus diisi")]
        [Display(Name = "Scheduled Sent")]
        public DateTime EmailqScheduledSent { get; set; }

        [Display(Name = "Sent Date")]
        public DateTime? EmailqSentDate { get; set; }

        [Display(Name = "Sent Try")]
        public int? EmailqSentTry { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Process")]
        public string EmailqProcess { get; set; }

        [Display(Name = "Process Id")]
        public long? EmailqProcessId { get; set; }

        [StringLength(1000, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Error Text")]
        public string EmailqErrorText { get; set; }

        [Display(Name = "Notice Id")]
        public long? NoticeId { get; set; }

        [StringLength(500, ErrorMessage = "Panjang [{0}] maksimal {1} karakter.")]
        [Display(Name = "Message Id")]
        public string EmailqMessageId { get; set; }


    }


}
