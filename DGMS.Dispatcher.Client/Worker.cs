using DGMS.CrossCutting.Messages;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DGMS.Saga.Client
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus bus;

        public Worker(ILogger<Worker> logger, IBus bus)
        {
            _logger = logger;
            this.bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Guid guid = Guid.NewGuid();
                _logger.LogInformation("Client Saga Id {id}", guid);
                await bus.Publish<ICreateMultipleDocumentsSagaRequest>(new
                {
                    CorrelationId = guid,
                    InitiatedBy = "Denis",
                    Documents = new[]
                        {
                            new
                            {
                                DocumentId = "11",
                                Fullname = "Car Hero Sonata",
                                ForAsset = "car",
                            },
                            new
                            {
                                DocumentId = "22",
                                Fullname = "Home on str. Puskin 32, Chisinau",
                                ForAsset = "home",
                            }
                        }

                });
                Console.ReadLine();
            }
        }
    }
}
