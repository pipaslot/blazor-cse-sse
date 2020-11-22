using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
//using Pipaslot.Logging;

namespace App.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build())
                .ConfigureLogging(loggingBuilder =>
                {
                    //loggingBuilder.AddRequestLogger();
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

    }
}
