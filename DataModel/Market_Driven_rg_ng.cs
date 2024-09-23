using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.DataModel
{
    internal class market_driven_rg_ng
    {
        public string HistoryType { get; set; }
        public string PeriodeLvl1 { get; set; }
        public string PeriodeLvl2 { get; set; }
        public string PeriodeLvl3 { get; set; }
        public string SecurityCode { get; set; }
        public string Market { get; set; }
        public int Volume { get; set; }
        public decimal Value { get; set; }
        public int Freq { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Close { get; set; }
        public string Periode { get; set; }
    }
}
}
