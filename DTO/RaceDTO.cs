using DakarRallySimulator.Db.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class RaceDTO
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public RaceStatus Status { get; set; }
        public int Distance { get; set; }
        public ICollection<VehicleDTO> Vehicles { get; set; }
    }
}
