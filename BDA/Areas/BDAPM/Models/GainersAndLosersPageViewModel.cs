using BDA.Models;
using System.Collections.Generic;

namespace BDA.Areas.BDAPM.Models
{
    public class GainersAndLosersPageViewModel
    {
        public List<GainerLoserViewModel> Gainers { get; set; }
        public List<GainerLoserViewModel> Losers { get; set; }

        public GainersAndLosersPageViewModel()
        {
            Gainers = new List<GainerLoserViewModel>();
            Losers = new List<GainerLoserViewModel>();
        }
    }
}