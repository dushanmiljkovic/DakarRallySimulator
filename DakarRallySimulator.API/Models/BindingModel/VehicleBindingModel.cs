using DakarRallySimulator.Db.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models.BindingModel
{
    public class VehicleBindingModel
    {
        public string TeamName { get; set; }
        public string Model { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public VehicleType Type { get; set; }
        public VehicleSubType? SubType { get; set; } 
        public int Position { get; set; }
        public double Millage { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool IsDNF { get; set; }
        public int NumberOfLightMalfunction { get; set; }
    }
}
