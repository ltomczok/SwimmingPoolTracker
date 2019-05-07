using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Kokosoft.SwimmingPoolTracker.ImportSchedule.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

            try
            {
                Log.Information("Starting service ImportSchedule");
                var builder = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile($"appsettings.json");
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddEntityFrameworkNpgsql();
                    services.AddDbContext<PoolsContext>(opt => opt.UseNpgsql(hostContext.Configuration.GetConnectionString("PoolsContext")), ServiceLifetime.Transient);
                    services.AddSingleton<IBus>(f => RabbitHutch.CreateBus(hostContext.Configuration.GetSection("RabbitMQ")["Host"]));
                    services.AddSingleton(new MongoClient(hostContext.Configuration.GetSection("MongoDB")["Host"]));
                    services.AddTransient<IImportPoolSchedule, ImportPoolSchedule>();
                    services.AddSingleton<IHostedService, OnNewSchedule>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddSerilog();
                });
                await builder.RunConsoleAsync();
                Log.Information("Stopping service ImportSchedule");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host ImportSchedule terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
