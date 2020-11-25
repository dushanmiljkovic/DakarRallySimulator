using Accord.Statistics.Distributions.Univariate;
using DakarRallySimulator.API.Models;
using DakarRallySimulator.API.Services;
using DakarRallySimulator.Common;
using DakarRallySimulator.Db;
using DakarRallySimulator.Db.Models;
using DakarRallySimulator.Db.Models.Enums;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRallySimulator.API.Implementations
{
    public class RaceService : IRaceService
    {
        private readonly DakarRellyContext context;
        private const int RACEDISTANCE = 10000;
        public RaceService(DakarRellyContext context)
        {
            this.context = context;
        }


        public async Task<(ResultInfo ResultInfo, bool Result)> CreateRace(int year)
        {
            try
            {
                var dbModel = new Race()
                {
                    CreatedAt = DateTime.UtcNow,
                    Status = RaceStatus.Pennding,
                    Distance = RACEDISTANCE,
                    Year = year,
                    Vehicles = new List<Vehicle>()
                };

                await context.Races.AddAsync(dbModel);
                await context.SaveChangesAsync();

                return (ResultInfo.Ok(), true);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(bool));
            }
        }

        public async Task<(ResultInfo ResultInfo, bool Result)> AddVehicle(VehicleDTO model)
        {
            try
            {
                var race = await context.Races.Where(x => x.Id == model.RaceId && x.Status == RaceStatus.Pennding).Include(c => c.Vehicles).FirstOrDefaultAsync();
                if (race == null)
                {
                    return (ResultInfo.Fail(ErrorType.ValidationException, "Invalid Race"), default(bool));
                }
                
                var dbModel = new Vehicle()
                {
                    ManufacturingDate = model.ManufacturingDate,
                    Model = model.Model,
                    Position = 0,
                    TeamName = model.TeamName,
                    Type = model.Type,
                    SubType = model.SubType,
                    IsDNF = false
                };

                if (race.Vehicles == null)
                {
                    race.Vehicles = new List<Vehicle>();
                }

                race.Vehicles.Add(dbModel);

                await context.SaveChangesAsync();

                return (ResultInfo.Ok(), true);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(bool));
            }

        }

        public async Task<(ResultInfo ResultInfo, bool Result)> UpdateVehicle(VehicleDTO model)
        {
            try
            {
 

                var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == model.Id && x.Race.Status == RaceStatus.Pennding);
                if (vehicle == null)
                {
                    return (ResultInfo.Fail(ErrorType.ValidationException, "Invalid VehicleID"), default(bool));
                }
                 
                vehicle.ManufacturingDate = model.ManufacturingDate;
                vehicle.Model = model.Model;
                vehicle.TeamName = model.TeamName;
                vehicle.Type = model.Type;
                vehicle.SubType = model.SubType;

                context.Update(vehicle);
                await context.SaveChangesAsync();

                return (ResultInfo.Ok(), true);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(bool));
            }
        }

        public async Task<(ResultInfo ResultInfo, bool Result)> RemoveVehicle(int id)
        {
            try
            {
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id && x.Race.Status == RaceStatus.Pennding);
                if (vehicle == null)
                {
                    return (ResultInfo.Fail(ErrorType.ValidationException, "Invalid VehicleID"), default(bool));
                }

                context.Remove(vehicle);
                await context.SaveChangesAsync();

                return (ResultInfo.Ok(), true);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(bool));
            }
        }

        public async Task<(ResultInfo ResultInfo, bool Result)> StartRace(int id)
        {
            try
            {
                if (HasRaceAlreadyStarted)
                {
                    return (ResultInfo.Fail(ErrorType.DomainException, "Race Already Started"), default(bool));
                }

                if (HasRaceAlreadyFinished)
                {
                    return (ResultInfo.Fail(ErrorType.DomainException, "Race Already Finished"), default(bool));
                }

                var race = context.Races.FirstOrDefault(x => x.Id == id && x.Status == RaceStatus.Pennding);
                if (race == null)
                {
                    return (ResultInfo.Fail(ErrorType.ValidationException, "Invalid RaceID"), default(bool));
                }

                race.Status = RaceStatus.Started;
                race.StartedAt = DateTime.UtcNow;

                context.Update(race);
                await context.SaveChangesAsync();

                return (ResultInfo.Ok(), true);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(bool));
            }
        }

        public async Task<(ResultInfo ResultInfo, (RaceStatus raceStatus, int stillRacingCount, int dnfCount, int carsCount, int truckCount, int motoCount) Result)> GetRaceStatus(int raceId)
        {
            try
            {
                var race = context.Races.Include(p => p.Vehicles).FirstOrDefault(x => x.Id == raceId);
                if (race == null)
                {
                    return (ResultInfo.Fail(ErrorType.ValidationException, "Invalid RaceID"), (default(RaceStatus), default(int), default(int), default(int), default(int), default(int)));
                }

                if (race.Status == RaceStatus.Started)
                {
                    //update
                    race.Vehicles = MoveVehicles(race.Vehicles.ToList(), DateTime.UtcNow, race.StartedAt.Value, race.Distance);

                    if (IsRaceDone(race.Vehicles.ToList()))
                    {

                        race.EndedAt = race.Vehicles.Where(x => !x.IsDNF && x.FinishedAt.HasValue).OrderByDescending(lst => lst.FinishedAt).First().FinishedAt;
                        race.Status = RaceStatus.Over;

                        race.Vehicles = OrderRanksByTime(race.Vehicles.ToList());
                    }

                    context.Update(race);
                    await context.SaveChangesAsync();
                }

                var groups = race.Vehicles.GroupBy(n => n.IsDNF)
                                .Select(n => new
                                {
                                    IsDnf = n.Key,
                                    VCount = n.Count()
                                }
                                );
  
                var vType = race.Vehicles.GroupBy(n => n.Type)
                                .Select(n => new
                                {
                                    VType = n.Key,
                                    VCount = n.Count()
                                }
                                );
                  
                return (ResultInfo.Ok(), (race.Status, groups.First(x => !x.IsDnf).VCount
                                                     , groups.First(x => x.IsDnf).VCount
                                                     , vType.First(x => x.VType == VehicleType.Car).VCount
                                                     , vType.First(x => x.VType == VehicleType.Truck).VCount
                                                     , vType.First(x => x.VType == VehicleType.Motorcycle).VCount));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), (default(RaceStatus), default(int), default(int), default(int), default(int), default(int)));
            }
        }


        public async Task<(ResultInfo ResultInfo, List<VehicleDTO> Result)> FindVehicles(string teamName, string model, DateTime? manufacturingDate, bool? isDNF, double? millage, bool? sortOrder)
        {
            try
            {
                var result = await context.Vehicles.Where(x => (string.IsNullOrEmpty(teamName) || x.TeamName == teamName)
                                                            && (string.IsNullOrEmpty(model) || x.Model == model)
                                                            && (!manufacturingDate.HasValue || x.ManufacturingDate == manufacturingDate)
                                                            && (!isDNF.HasValue || x.IsDNF == isDNF)
                                                            && (!millage.HasValue || x.Millage == millage)
                                                         ).Select(x => new VehicleDTO()
                                                         {

                                                             FinishedAt = x.FinishedAt,
                                                             IsDNF = x.IsDNF,
                                                             ManufacturingDate = x.ManufacturingDate,
                                                             Millage = x.Millage,
                                                             Model = x.Model,
                                                             ModifiedAt = x.ModifiedAt,
                                                             Position = x.Position,
                                                             TeamName = x.TeamName,
                                                             Type = x.Type,
                                                             SubType = x.SubType
                                                         }).ToListAsync();

                if (sortOrder.HasValue)
                {
                    if (sortOrder.Value)
                    {
                        //.ThenBy
                        result.OrderBy(x => x.Position);
                    }
                    else
                    {
                        result.OrderByDescending(x => x.Position);
                    }
                }

                return (ResultInfo.Ok(), result);

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(List<VehicleDTO>));
            }
        }
        public async Task<(ResultInfo ResultInfo, VehicleDTO Result)> GetVehicleStatistics(int id)
        {
            try
            { 
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
                if (vehicle == null)
                {
                    return (ResultInfo.Fail(ErrorType.ValidationException, "Invalid VehicleID"), default(VehicleDTO));
                }

                var newResult = new VehicleDTO()
                {
                    Millage = vehicle.Millage,
                    NumberOfLightMalfunction = vehicle.NumberOfLightMalfunction,
                    ModifiedAt = vehicle.ModifiedAt,
                    IsDNF = vehicle.IsDNF,
                    FinishedAt = vehicle.FinishedAt
                };
                 
                return (ResultInfo.Ok(), newResult);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(VehicleDTO));
            }
        }

        public async Task<(ResultInfo ResultInfo, List<VehicleDTO> Result)> GetLeaderboard(VehicleType? vehicleType = null)
        {
            try
            {
                var race = context.Races.Include(p => p.Vehicles).SingleOrDefault(x => x.Status == RaceStatus.Started);
                if (race == null)
                {
                    return (ResultInfo.Fail(ErrorType.DomainException, "No one Racing ATM"), default(List<VehicleDTO>));
                }

                // Update All Vhiacles  
                //update
                race.Vehicles = MoveVehicles(race.Vehicles.ToList(), DateTime.UtcNow, race.StartedAt.Value, race.Distance);

                if (IsRaceDone(race.Vehicles.ToList()))
                {

                    race.EndedAt = race.Vehicles.Where(x => !x.IsDNF && x.FinishedAt.HasValue).OrderByDescending(lst => lst.FinishedAt).First().FinishedAt;
                    race.Status = RaceStatus.Over;

                    race.Vehicles = OrderRanksByTime(race.Vehicles.ToList());
                }

                context.Update(race);
                await context.SaveChangesAsync();


                // Filter Data
                if (vehicleType.HasValue)
                {
                    race.Vehicles = race.Vehicles.Where(x => x.Type == vehicleType).ToList();
                }

                var result = race.Vehicles.Select(x => new VehicleDTO()
                {
                    Millage = x.Millage,
                    Position = x.Position,
                    TeamName = x.TeamName,
                    Model = x.Model,
                    IsDNF = x.IsDNF
                    
                }).OrderBy(y=> y.Position).ToList();

                return (ResultInfo.Ok(), result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message);
                return (ResultInfo.Fail(ErrorType.UnhandledException, ex.Message), default(List<VehicleDTO>));
            }
        }

        private bool HasRaceAlreadyStarted => context.Races.Any(x => x.Status == RaceStatus.Started);
        private bool HasRaceAlreadyFinished => context.Races.Any(x => x.Status == RaceStatus.Over);

        // Race Must Be Started 
        private IList<Vehicle> MoveVehicles(IList<Vehicle> vehicless, DateTime momentSized, DateTime raceStartedAt, int raceDistance)
        { 
            var vehicles = vehicless.ToList();
            // Move vehicles 
            foreach (var vehicle in vehicles.Where(x => !x.IsDNF && !x.FinishedAt.HasValue))
            {
                var lastCheckInSec = TimeSpanInSecunds(vehicle.ModifiedAt.HasValue ? vehicle.ModifiedAt.Value : raceStartedAt, momentSized);

                // if lastCheck is negative number vehicle got 
                if (lastCheckInSec > 0)
                {
                    var moreInfo = VehicleFactory.GetVehicleStaticProps(vehicle.Type, vehicle.SubType);
                    var distanceTraveled = moreInfo.MaxSpeed * (lastCheckInSec / 3600);

                    // 1. Move
                    if (raceDistance >= distanceTraveled + vehicle.Millage)
                    {
                        vehicle.Millage += distanceTraveled;
                        vehicle.ModifiedAt = momentSized;
                    }
                    else
                    {
                        //var timeFinishedLastSection = moreInfo.MaxSpeed / (RACEDISTANCE - distanceTraveled);
                        vehicle.FinishedAt = vehicle.ModifiedAt.Value.AddHours(moreInfo.MaxSpeed / (raceDistance - distanceTraveled));
                    }

                    // 2. Calculate Fail
                    if (!vehicle.FinishedAt.HasValue)
                    {
                        double rnd = new Random().Next(0, 100);
                        double heavyError = moreInfo.HeavyMalfunction * (lastCheckInSec / 3600);

                        if (rnd < heavyError)
                        {
                            // DNF error 
                            vehicle.IsDNF = true;
                            vehicle.ModifiedAt = momentSized;
                            continue;
                        }

                        double lightError = moreInfo.LightMalfunction * (lastCheckInSec / 3600);
                        if (rnd < lightError)
                        {
                            // Light error  
                            vehicle.NumberOfLightMalfunction++;
                            vehicle.ModifiedAt = momentSized.AddHours(moreInfo.RepairmentsLast);
                        }
                    }
                }

            }
            return vehicles;
        }

        private IList<Vehicle> MathSolutionInOneGo(IList<Vehicle> vehicless, DateTime momentSized, DateTime raceStartedAt, int raceDistanc)
        {
            var vehicles = vehicless.ToList();

            // Move vehicles  
            foreach (var vehicle in vehicles.Where(x => !x.IsDNF && !x.FinishedAt.HasValue))
            {
                var moreInfo = VehicleFactory.GetVehicleStaticProps(vehicle.Type, vehicle.SubType);
                var idealTimeToFinishRace = raceDistanc / moreInfo.MaxSpeed;

                var chancesNotToDNF = idealTimeToFinishRace * ((100 - moreInfo.HeavyMalfunction) / 100);
                var isDNF = InversePoisson(chancesNotToDNF);

                if (isDNF < moreInfo.HeavyMalfunction)
                {
                    vehicle.IsDNF = true;
                    vehicle.ModifiedAt = momentSized;
                    continue;
                }

                var chancesNotToMalfunction = idealTimeToFinishRace * ((100 - moreInfo.LightMalfunction) / 100);
                var malfunctionOccurrenc = Math.Round(InversePoisson(chancesNotToMalfunction));
                var timeFixing = malfunctionOccurrenc * moreInfo.RepairmentsLast;

                vehicle.FinishedAt = raceStartedAt.AddHours(idealTimeToFinishRace + timeFixing);
                vehicle.NumberOfLightMalfunction = (int)malfunctionOccurrenc;
                vehicle.Millage = raceDistanc;

            }
            return vehicles;
        }

        private bool IsRaceDone(IList<Vehicle> vehicless) => !vehicless.Any(x => !x.IsDNF && !x.FinishedAt.HasValue);

        private IList<Vehicle> OrderRanksByTime(IList<Vehicle> vehicless)
        {

            var sortedList = vehicless.OrderBy(o => o.FinishedAt ?? o.ModifiedAt).ToList();
            sortedList.ForEach(x => x.Position = vehicless.IndexOf(x));
            return sortedList;
        }

        private double InversePoisson(double notToBe) => Math.Log(notToBe);

        private double TimeSpanInSecunds(DateTime lastCheck, DateTime momment)
        {
            TimeSpan ts = momment - lastCheck;
            return ts.TotalSeconds;
        }


    }
}
