using DakarRallySimulator.Db.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models.Requests
{
    public class CreateVehicleRequestModel
    {  
        public int VehicleId { get; set; }
        public int RaceId { get; set; }
        [StringLength(255)]
        public string TeamName { get; set; }
        [StringLength(255)]
        public string Model { get; set; }
        public DateTime ManufacturingDate { get; set; }
        [Required]
        public VehicleType Type { get; set; }
        public VehicleSubType? SubType { get; set; }
    }
}
