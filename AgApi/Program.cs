using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace AppV
{
    public class Program
    {


        public static int Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();


            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            //.WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (System.Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

           

        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseSerilog(); // <-- Add this line;

        public static IWebHost BuildWebHost(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                        .ConfigureServices(s =>
                            s.AddAutofac())
                                .Build();

    }
}
