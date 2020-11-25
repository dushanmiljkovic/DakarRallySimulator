using DakarRallySimulator.API.Models;
using DakarRallySimulator.API.Models.BindingModel;
using DakarRallySimulator.API.Models.Requests;
using DakarRallySimulator.API.Services;
using DakarRallySimulator.Db;
using DakarRallySimulator.Db.Models;
using DakarRallySimulator.Db.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Controllers
{
    [Route("api/[controller]")]
    public class RaceController : Controller
    {
        private readonly IRaceService raceService;

        public RaceController(IRaceService raceService)
        {
            this.raceService = raceService;
        }

        [HttpPost("create-race/{year}")]
        public async Task<IActionResult> CreateRace([FromRoute] int year)
        {
            if (year < DateTime.UtcNow.Year) { return BadRequest(); }

            var result = await raceService.CreateRace(year);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            return Ok();
        }

        [HttpPost("start-race/{raceId}")]
        public async Task<IActionResult> StartRace([FromRoute] int raceId)
        { 
            if (raceId < 0) { return BadRequest(); }

            var result = await raceService.StartRace(raceId);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            return Ok();
        }

        [HttpPost("add-vehicle")]
        public async Task<IActionResult> AddVehicle(CreateVehicleRequestModel createVehicleRequestModel)
        {

            if (createVehicleRequestModel.RaceId < 0) { return BadRequest(); }

            var vehicleDto = VehicleFactory.CreateVehicle(createVehicleRequestModel);
            if (vehicleDto == null) { return BadRequest(); }

            var result = await raceService.AddVehicle(vehicleDto);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            return Ok();
        }

        [HttpPatch("update-vehicle")]
        public async Task<IActionResult> UpdateVehicle(CreateVehicleRequestModel createVehicleRequestModel)
        {
            if (createVehicleRequestModel.VehicleId < 0) { return BadRequest(); } 
            if (createVehicleRequestModel.RaceId < 0) { return BadRequest(); }

            var vehicleDto = VehicleFactory.CreateVehicle(createVehicleRequestModel);
            if (vehicleDto == null) { return BadRequest(); }

            var result = await raceService.UpdateVehicle(vehicleDto);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            return Ok();
        }

        [HttpDelete("remove-vehicle/{vehicleId}")]
        public async Task<IActionResult> RemoveVehicle([FromRoute] int vehicleId)
        {
            if (vehicleId < 0) { return BadRequest(); }

            var result = await raceService.RemoveVehicle(vehicleId);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            return Ok();
        }

        [HttpGet("get-leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var result = await raceService.GetLeaderboard();
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            var bindingModel = result.Result.Any()
                                ? result.Result.Select(p => ModelFactory.CreateLeaderboardBindingModel(p)).ToList()
                                : new List<LeaderboardBindingModel>();
            return Ok(bindingModel);
        }

        [HttpGet("get-leaderboard/{vehicleType}")]
        public async Task<IActionResult> GetLeaderboard([FromRoute] VehicleType vehicleType)
        {
            var result = await raceService.GetLeaderboard(vehicleType);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            var bindingModel = result.Result.Any()
                    ? result.Result.Select(p => ModelFactory.CreateLeaderboardBindingModel(p)).ToList()
                    : new List<LeaderboardBindingModel>();
            return Ok(bindingModel);
        }

        [HttpGet("get-vehicle-statistics/{vehicleId}")]
        public async Task<IActionResult> GetVehicleStatistics([FromRoute] int vehicleId)
        {
            if (vehicleId < 0) { return BadRequest(); }

            var result = await raceService.GetVehicleStatistics(vehicleId);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }
             
            return Ok(ModelFactory.CreateVehicleStatisticsdBindingModel(result.Result));
        }

        [HttpGet("find-vehicles")]
        public async Task<IActionResult> FindVehicle(string teamName, string model, DateTime? manufacturingDate, bool? isDNF, double? millage, bool? sortOrder)
        {

            var result = await raceService.FindVehicles(teamName, model, manufacturingDate, isDNF, millage, sortOrder);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            var bindingModel = result.Result.Any()
                   ? result.Result.Select(p => ModelFactory.CreateVehicleBindingModel(p)).ToList()
                   : new List<VehicleBindingModel>();
            return Ok(bindingModel);
        }


        [HttpGet("get-race-status/{raceId}")]
        public async Task<IActionResult> GetRaceStatus([FromRoute] int raceId)
        {
            if (raceId < 0) { return BadRequest(); }

            var result = await raceService.GetRaceStatus(raceId);
            if (!result.ResultInfo.IsOk) { return Conflict(result.ResultInfo.Error); }

            return Ok(new RaceStatusBindingModel()
            {
                RaceStatus = result.Result.raceStatus,
                StillRacingCount = result.Result.stillRacingCount,
                DnfCount = result.Result.dnfCount,
                CarsCount = result.Result.carsCount,
                MotoCount = result.Result.motoCount,
                TruckCount = result.Result.truckCount
            });
        }
    }
}
