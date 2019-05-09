using EasyNetQ;
using Kokosoft.SwimmingPoolTracker.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kokosoft.SwimmingPoolTracker.ImportSchedule
{
    public class OnNewSchedule : IHostedService
    {
        IBus messageBus;
        private readonly ILogger<OnNewSchedule> logger;
        private readonly IServiceProvider services;

        public OnNewSchedule(IBus bus, ILogger<OnNewSchedule> logger, IServiceProvider services)
        {
            this.messageBus = bus;
            this.logger = logger;
            this.services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            messageBus.Receive<NewSchedule>("swimmingpooltracker", message => OnNewMessage(message));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            messageBus.Dispose();
            return Task.CompletedTask;
        }

        public async Task OnNewMessage(NewSchedule newSchedule)
        {
            using (var scope = services.CreateScope())
            {
                var importScheduleService =
                    scope.ServiceProvider
                        .GetRequiredService<IImportPoolSchedule>();

                await importScheduleService.ImportSchedule(newSchedule.Pool.ToString(), newSchedule.StartDate, newSchedule.EndDate, newSchedule.Link);
            }
        }
    }
}
