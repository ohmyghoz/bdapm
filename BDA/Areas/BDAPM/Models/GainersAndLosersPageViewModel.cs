using BDA.Models;
using System.Collections.Generic;

namespace BDA.Areas.BDAPM.Models
{
    public class GainersAndLosersPageViewModel
    {
        public List<GainerLoserViewModel> Gainers { get; set; }
        public List<GainerLoserViewModel> Losers { get; set; }

        public int PageGainers { get; set; } = 1;
        public int PageLosers { get; set; } = 1;

        // Fixed page size (always 10)
        public int PageSizeGainers { get; set; } = 10;
        public int PageSizeLosers { get; set; } = 10;

        public int TotalGainers { get; set; }
        public int TotalLosers { get; set; }

        public int TotalPagesGainers { get; set; }
        public int TotalPagesLosers { get; set; }

        public bool HasGainers => TotalGainers > 0;
        public bool HasLosers => TotalLosers > 0;

        public GainersAndLosersPageViewModel()
        {
            Gainers = new List<GainerLoserViewModel>();
            Losers = new List<GainerLoserViewModel>();
        }
    }
}