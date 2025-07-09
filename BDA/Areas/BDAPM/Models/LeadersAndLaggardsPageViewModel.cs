using BDA.Models;
using System.Collections.Generic;

namespace BDA.Areas.BDAPM.Models
{
    public class LeadersAndLaggardsPageViewModel
    {

        public List<LeaderLaggardViewModel> Leaders { get; internal set; }
        public List<LeaderLaggardViewModel> Laggards { get; internal set; }

        public LeadersAndLaggardsPageViewModel()
        {
            Leaders = new List<LeaderLaggardViewModel>();
            Laggards = new List<LeaderLaggardViewModel>();
        }
    }
}