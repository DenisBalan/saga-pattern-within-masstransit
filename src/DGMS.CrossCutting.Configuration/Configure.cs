using Automatonymous;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace DGMS.CrossCutting.Configuration
{
    public static class Configure
    {
        public static IHostBuilder UseDGMSConfiguration<THostedService>(this IHostBuilder hostBuilder, 
            Action<IServiceCollectionBusConfigurator> sagaConfigure = null, 
            int prefetch = 1
        ) where THostedService : class, IHostedService
        {
            hostBuilder
                    .UseSerilog()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddTransient<ContextualLogging>();
                        services.AddHostedService<THostedService>();
                        services.AddMassTransit(x =>
                {
                    x.AddConsumersFromNamespaceContaining(typeof(THostedService));
                    x.AddBus(ConfigureBus(prefetch));
                    sagaConfigure?.Invoke(x);
                });
                    });
            return hostBuilder;
        }
        public static void Logging<T>() where T : class
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .Enrich.WithProperty(nameof(DGMS), typeof(T).Assembly.GetName().Name)
                 .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
        public static Func<IBusRegistrationContext, IBusControl> ConfigureBus(int prefetch)
        {
            return provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("rabbitmq://localhost");
                cfg.PrefetchCount = prefetch;

                cfg.ConfigureEndpoints(provider);
            });
        }
        public static void NOOP<TInstance>(BehaviorContext<TInstance> message)
        {

        }
    }
}
