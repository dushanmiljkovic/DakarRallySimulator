using DakarRallySimulator.API.Models.Requests;
using DakarRallySimulator.Db.Models.Enums;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Models
{
    public static class VehicleFactory
    {
        private const int motorcyclesRepairment = 3;
        private const int carsRepairment = 5;
        private const int trucksRepairment = 7;


        private const int SportsCarTopSpeed = 140;
        private const int SportsCarLightMalfunction = 12;
        private const int SportsCarHeavyMalfunction = 2;


        private const int TerrainCarTopSpeed = 100;
        private const int TerrainCarLightMalfunction = 3;
        private const int TerrainCarHeavyMalfunction = 1;

        private const int TruckTopSpeed = 80;
        private const int TruckLightMalfunction = 6;
        private const int TruckHeavyMalfunction = 4;

        private const int CrossMotorcycleTopSpeed = 85;
        private const int CrossMotorcycleLightMalfunction = 3;
        private const int CrossMotorcycleHeavyMalfunction = 2;

        private const int SportsMotorcycleTopSpeed = 130;
        private const int SportsMotorcycleLightMalfunction = 18;
        private const int SportsMotorcycleHeavyMalfunction = 10;


        public static VehicleDTO CreateVehicle(CreateVehicleRequestModel requestModel)
        {
            if (!IsVehicleTypeValid(requestModel.Type, requestModel.SubType))
            {
                return null;
            }

            return new VehicleDTO()
            {
                Id = requestModel.VehicleId,
                RaceId = requestModel.RaceId,
                ManufacturingDate = requestModel.ManufacturingDate,
                Model = requestModel.Model,
                TeamName = requestModel.TeamName,
                Type = requestModel.Type,
                SubType = requestModel.SubType
            };
        }

        private static bool IsVehicleTypeValid(VehicleType vehicleType, VehicleSubType? vehicleSubType)
        {
            return vehicleType switch
            {
                VehicleType.Car when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Sports => true,
                VehicleType.Car when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Terrain => true,

                VehicleType.Motorcycle when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Cross => true,
                VehicleType.Motorcycle when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Sports => true,

                VehicleType.Truck when !vehicleSubType.HasValue => true,

                _ => false,
            };
        }

        public static VehicleStaticProps GetVehicleStaticProps(VehicleType vehicleType, VehicleSubType? vehicleSubType) => vehicleType switch
        {
            VehicleType.Car when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Sports
                => new VehicleStaticProps() { HeavyMalfunction = SportsCarHeavyMalfunction, LightMalfunction = SportsCarLightMalfunction, MaxSpeed = SportsCarTopSpeed, RepairmentsLast = carsRepairment },

            VehicleType.Car when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Terrain
                => new VehicleStaticProps() { HeavyMalfunction = TerrainCarHeavyMalfunction, LightMalfunction = TerrainCarLightMalfunction, MaxSpeed = TerrainCarTopSpeed, RepairmentsLast = carsRepairment },

            VehicleType.Motorcycle when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Cross
                => new VehicleStaticProps() { HeavyMalfunction = CrossMotorcycleHeavyMalfunction, LightMalfunction = CrossMotorcycleLightMalfunction, MaxSpeed = CrossMotorcycleTopSpeed, RepairmentsLast = motorcyclesRepairment },

            VehicleType.Motorcycle when vehicleSubType.HasValue && vehicleSubType == VehicleSubType.Sports
                => new VehicleStaticProps() { HeavyMalfunction = SportsMotorcycleHeavyMalfunction, LightMalfunction = SportsMotorcycleLightMalfunction, MaxSpeed = SportsMotorcycleTopSpeed, RepairmentsLast = motorcyclesRepairment },

            VehicleType.Truck when !vehicleSubType.HasValue
                => new VehicleStaticProps() { HeavyMalfunction = TruckHeavyMalfunction, LightMalfunction = TruckLightMalfunction, MaxSpeed = TruckTopSpeed, RepairmentsLast = trucksRepairment },

            _ => throw new NotImplementedException()
        };
    }

    public class VehicleStaticProps
    {
        public int MaxSpeed { get; set; }
        public int RepairmentsLast { get; set; }
        public int LightMalfunction { get; set; }
        public int HeavyMalfunction { get; set; }
    }
}
