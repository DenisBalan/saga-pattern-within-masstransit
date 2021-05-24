using DGMS.CrossCutting.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage.Net;

namespace DGMS.Service.UploadToStorage
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
                        services.AddTransient(x => StorageFactory.Blobs.DirectoryFiles("/temp"));
                    });
        }
    }
}