using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models.BindingModel
{
    public class LeaderboardBindingModel
    {
        public int Position { get; set; }
        public double Millage { get; set; }
        public string TeamName { get; set; }
        public string Model { get; set; }
        public bool IsDNF { get; set; }
    }
}
