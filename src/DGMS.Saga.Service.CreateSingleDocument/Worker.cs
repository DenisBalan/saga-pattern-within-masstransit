using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DGMS.Saga.CreateSingleDocument
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus bus;
        private readonly IBusControl busControl;

        public Worker(ILogger<Worker> logger, IBus bus, IBusControl busControl)
        {
            _logger = logger;
            this.bus = bus;
            this.busControl = busControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await busControl.StartAsync(stoppingToken);
        }
    }
}
