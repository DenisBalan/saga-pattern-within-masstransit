using DGMS.CrossCutting.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGMS.Saga.Receiver
{
    public class CreateMultipleDocumentsCompletedConsumer : IConsumer<ICreateMultipleDocumentsCompleted>
    {
        private readonly ILogger<CreateMultipleDocumentsCompletedConsumer> logger;

        public CreateMultipleDocumentsCompletedConsumer(ILogger<CreateMultipleDocumentsCompletedConsumer> logger)
        {
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<ICreateMultipleDocumentsCompleted> context)
        {
            var output = new[]
            {
                $"Status={context.Message.Status.ToUpperInvariant()}",
                string.Join($"|{Environment.NewLine}", context.Message.Uris),
                new string('*', 11)
            }.Aggregate((a, b) => a + b);
            logger.LogInformation("Status = {status}, documents = {documents}", context.Message.Status.ToUpperInvariant(), JsonConvert.SerializeObject(context.Message.Uris, Formatting.Indented));
        }
    }
}
