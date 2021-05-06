using DGMS.CrossCutting.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DGMS.Service.HtmlToPdf
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
                    .UseDGMSConfiguration<Worker>(prefetch: 10)
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(x => new PugPdf.Core.HtmlToPdf());
                    });
        }
    }
}