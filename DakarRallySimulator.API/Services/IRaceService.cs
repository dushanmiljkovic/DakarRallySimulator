using DakarRallySimulator.Common;
using DakarRallySimulator.Db.Models.Enums;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Services
{
    public interface IRaceService
    {
        Task<(ResultInfo ResultInfo, bool Result)> CreateRace(int year);
        Task<(ResultInfo ResultInfo, bool Result)> StartRace(int id);
        Task<(ResultInfo ResultInfo, (RaceStatus raceStatus, int stillRacingCount, int dnfCount, int carsCount, int truckCount, int motoCount) Result)> GetRaceStatus(int raceId);

        Task<(ResultInfo ResultInfo, bool Result)> AddVehicle(VehicleDTO model);
        Task<(ResultInfo ResultInfo, bool Result)> UpdateVehicle(VehicleDTO model);
        Task<(ResultInfo ResultInfo, bool Result)> RemoveVehicle(int id);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="model"></param>
        /// <param name="manufacturingDate"></param>
        /// <param name="isDNF"></param>
        /// <param name="millage"></param>
        /// <param name="sortOrder"> True = ascending , False= descending , Null = none </param>
        /// <returns></returns>
        Task<(ResultInfo ResultInfo, List<VehicleDTO> Result)> FindVehicles(string teamName, string model , DateTime? manufacturingDate, bool? isDNF, double? millage, bool? sortOrder);
        Task<(ResultInfo ResultInfo, VehicleDTO Result)> GetVehicleStatistics(int id);
        Task<(ResultInfo ResultInfo, List<VehicleDTO> Result)> GetLeaderboard(VehicleType? vehicleType = null);
    }
}
