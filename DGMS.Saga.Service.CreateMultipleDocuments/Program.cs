using DGMS.CrossCutting.Configuration;
using DGMS.CrossCutting.Messages;
using DGMS.Saga.CreateMultipleDocuments.Saga;
using MassTransit;
using MassTransit.NewIdProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace DGMS.Saga.CreateMultipleDocuments
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
                        NewId.SetProcessIdProvider(new CurrentProcessIdProvider());

                        x.AddSagaStateMachine<CreateMultipleDocumentsSagaMachine, MultipleDocumentsSaga>()
                        .MongoDbRepository(r =>
                        {
                            r.Connection = "mongodb://127.0.0.1";
                            r.DatabaseName = "multiple-documents-saga-db";
                        });
                    });
        }
    }
}