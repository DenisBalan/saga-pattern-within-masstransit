using DGMS.CrossCutting.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorEngine.Templating;

namespace DGMS.Service.ConvertToHtml
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
                    .UseDGMSConfiguration<Worker>()
                    .ConfigureServices(services =>
                    {
                        services.AddTransient<IRazorEngineService>(x => RazorEngineService.Create());
                    });
        }
    }
}