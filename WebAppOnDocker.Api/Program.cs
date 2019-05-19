using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace WebAppOnDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureLogging((hostingContext, builder) =>
                   {
                       builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                       builder.AddConsole();
                       builder.AddDebug();
                       builder.AddApplicationInsights(hostingContext.Configuration["ApplicationInsights:InstrumentationKey"]);

                       builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Trace);
                   });
    }
}