using System.ComponentModel.DataAnnotations;

namespace BDA.Models // Use your project's namespace
{
    public class GainerLoserViewModel
    {
        public int Sequence { get; set; }
        public string SecurityCode { get; set; }
        public string SecurityName { get; set; } // You'll need to join this from another table

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Volume { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal Turnover { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Freq { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal NetValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal NetVolume { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Point { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal ChangePercentage { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MaxPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MinPrice { get; set; }
    }
}