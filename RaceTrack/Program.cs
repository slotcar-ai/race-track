using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scai.RaceTrack.Arduino;

namespace Scai.RaceTrack
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();
            var host = hostBuilder
                .ConfigureAppConfiguration((context, config) =>
                {
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TrackService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging
                        .AddConsole()
                        .AddDebug();
                });

            await host.RunConsoleAsync();
        }
    }
}