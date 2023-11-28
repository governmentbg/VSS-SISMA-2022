using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SISMA.Worker.Extensions;

var host = BuildHost(args);
host.Run();
Console.WriteLine("Started");
Console.ReadLine();

static IHost BuildHost(string[] args)
{
    return new HostBuilder()
                    .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.SetBasePath(Directory.GetCurrentDirectory());
                        configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                        configHost.AddCommandLine(args);
                    })
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     configApp.SetBasePath(Directory.GetCurrentDirectory());
                     configApp.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                     configApp.AddJsonFile($"appsettings.json", true);
                     configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                     configApp.AddCommandLine(args);
                 })
                 .ConfigureLogging((hostContext, configLogging) =>
                 {
                     configLogging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                     configLogging.AddConsole();
                     configLogging.AddDebug();
                 })
                 .ConfigureServices((hostContext, services) =>
                 {

                     services.AddApplicationDbContext(hostContext.Configuration);
                     services.ConfigureServices(hostContext.Configuration);

                     services.AddQuartConfiguration(hostContext.Configuration);
                 })
                .Build();
}