using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models.BindingModel
{
    public class VehicleStatisticsdBindingModel
    {
        public double Distance { get; set; }
        public int NumberOfLightMalfunction { get; set; }
        public VehicleStatus VehicleStatus { get; set; }
        public DateTime? FinishedAt { get; set; } 
    }
}
