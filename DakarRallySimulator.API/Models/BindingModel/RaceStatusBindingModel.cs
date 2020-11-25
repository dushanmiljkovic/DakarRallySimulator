using DakarRallySimulator.Db.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models.BindingModel
{
    public class RaceStatusBindingModel
    {
        public RaceStatus RaceStatus { get; set; }
        public int StillRacingCount { get; set; }
        public int DnfCount { get; set; }
        public int CarsCount { get; set; }
        public int TruckCount { get; set; }
        public int MotoCount { get; set; } 
    }
}
