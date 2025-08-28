using BDA.Models;
using System.Collections.Generic;

namespace BDA.Areas.BDAPM.Models
{
    public class LeadersAndLaggardsPageViewModel
    {
        public List<LeaderLaggardViewModel> Leaders { get; set; }
        public List<LeaderLaggardViewModel> Laggards { get; set; }

        // Independent pagination (fixed page size = 10)
        public int PageLeaders { get; set; } = 1;
        public int PageLaggards { get; set; } = 1;

        public int PageSizeLeaders { get; set; } = 10;
        public int PageSizeLaggards { get; set; } = 10;

        public int TotalLeaders { get; set; }
        public int TotalLaggards { get; set; }

        public int TotalPagesLeaders { get; set; }
        public int TotalPagesLaggards { get; set; }

        public bool HasLeaders => TotalLeaders > 0;
        public bool HasLaggards => TotalLaggards > 0;

        public LeadersAndLaggardsPageViewModel()
        {
            Leaders = new List<LeaderLaggardViewModel>();
            Laggards = new List<LeaderLaggardViewModel>();
        }
    }
}