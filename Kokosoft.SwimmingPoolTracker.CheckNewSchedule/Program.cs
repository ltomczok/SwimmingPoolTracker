using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Serilog;

namespace Kokosoft.SwimmingPoolTracker.CheckNewSchedule
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
                        services.AddSingleton<IBus>(f => RabbitHutch.CreateBus(hostContext.Configuration.GetSection("RabbitMQ")["Host"]));
                        services.AddSingleton(new MongoClient(hostContext.Configuration.GetSection("MongoDB")["Host"]));
                        services.AddSingleton<IHostedService, CheckNewScheduleBackgroundService>();
                        services.AddSingleton<IMonthHelpers, MonthHelpers>();
                    })
                    .ConfigureLogging((hostContext, logging) =>
                    {
                        logging.AddSerilog();
                    })
                    .Build();

                await builder.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host CheckNewSchedule terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
