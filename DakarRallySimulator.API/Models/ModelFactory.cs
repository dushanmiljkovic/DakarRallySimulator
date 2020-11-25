using DakarRallySimulator.API.Models.BindingModel;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models
{
    public static class ModelFactory
    {
        public static LeaderboardBindingModel CreateLeaderboardBindingModel(VehicleDTO vehicleDTO)
        {
            return new LeaderboardBindingModel()
            {
                IsDNF = vehicleDTO.IsDNF,
                Millage = vehicleDTO.Millage,
                Model = vehicleDTO.Model,
                Position = vehicleDTO.Position,
                TeamName = vehicleDTO.TeamName
            };
        }

        public static VehicleStatisticsdBindingModel CreateVehicleStatisticsdBindingModel(VehicleDTO vehicleDTO)
        {
            return new VehicleStatisticsdBindingModel()
            {
                Distance = vehicleDTO.Millage,
                FinishedAt = vehicleDTO.FinishedAt,
                NumberOfLightMalfunction = vehicleDTO.NumberOfLightMalfunction,
                VehicleStatus = GetVehicleStatus(vehicleDTO)
            };
        }

        
         public static VehicleBindingModel CreateVehicleBindingModel(VehicleDTO vehicleDTO)
        {
            return new VehicleBindingModel()
            {
               FinishedAt = vehicleDTO.FinishedAt,
               IsDNF = vehicleDTO.IsDNF,
               ManufacturingDate = vehicleDTO.ManufacturingDate,
               Millage = vehicleDTO.Millage,
               Model = vehicleDTO.Model,
               ModifiedAt = vehicleDTO.ModifiedAt,
               NumberOfLightMalfunction = vehicleDTO.NumberOfLightMalfunction,
               Position = vehicleDTO.Position,
               SubType = vehicleDTO.SubType,
               TeamName = vehicleDTO.TeamName,
               Type = vehicleDTO.Type,
            };
        }
        private static VehicleStatus GetVehicleStatus(VehicleDTO vehicleDTO)
        {
            if (vehicleDTO.IsDNF)
            {
                return VehicleStatus.DNF;
            }
            else
            if (vehicleDTO.FinishedAt.HasValue)
            {
                return VehicleStatus.Finished;
            }
            else
            if (vehicleDTO.ModifiedAt.HasValue)
            {
                return VehicleStatus.Racing;
            } 
            return VehicleStatus.Panding; 
        }
    }
}
