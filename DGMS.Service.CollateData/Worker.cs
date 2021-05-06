using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DGMS.Service.CollateData
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusControl busControl;

        public Worker(ILogger<Worker> logger, IBusControl busControl)
        {
            _logger = logger;
            this.busControl = busControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await busControl.StartAsync(stoppingToken);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }
    }
}
