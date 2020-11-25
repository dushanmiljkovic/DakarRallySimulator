using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace LogService
{
    public static class RegisterSerilogServices
    {
        public static IServiceCollection AddSerilogServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSerilogServices(
                new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext());
        }

        public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
        {
            Log.Logger = configuration.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services.AddSingleton(Log.Logger);
        }

        public static IServiceCollection AddSerilogServices(this IServiceCollection services)
        {
            return services.AddSerilogServices(
                new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console());
        }
    }
}
