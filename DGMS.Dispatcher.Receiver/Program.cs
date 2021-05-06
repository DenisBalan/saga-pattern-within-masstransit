using DGMS.CrossCutting.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGMS.Saga.Receiver
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