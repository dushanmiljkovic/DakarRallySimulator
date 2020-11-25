using DakarRallySimulator.Db.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DakarRallySimulator.Db.Models
{
    public class Race
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public RaceStatus Status { get; set; } 
        public int Distance { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
      
    }
}
