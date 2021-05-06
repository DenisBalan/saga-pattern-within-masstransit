using DGMS.CrossCutting.Configuration;
using Microsoft.Extensions.Hosting;

namespace DGMS.Saga.Client
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
                    .UseDGMSConfiguration<Worker>();
        }
    }
}