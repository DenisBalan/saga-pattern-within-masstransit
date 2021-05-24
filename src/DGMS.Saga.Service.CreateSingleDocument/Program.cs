using DGMS.CrossCutting.Configuration;
using DGMS.CrossCutting.Messages;
using DGMS.Saga.CreateSingleDocument.Saga;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace DGMS.Saga.CreateSingleDocument
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Configure.Logging<Program>();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .UseDGMSConfiguration<Worker>(x =>
                    {
                        x.AddSagaStateMachine<CreateSingleDocumentSagaMachine, SingleDocumentSaga>()
                        .MongoDbRepository(r =>
                        {
                            r.Connection = "mongodb://127.0.0.1";
                            r.DatabaseName = "single-document-saga-db";
                        });
                    });
        }
    }
}