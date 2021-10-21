using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace MyOverflow.DataAccess.Cosmos.Tests
{
    /// <summary>
    /// gives you a host, and therefore access to registered services.
    /// You shouldn't need this for anything other than code that is tied to dependencies.
    /// Ideally, that kind of code should become a factory, so you can free yourself of the configuration,
    /// making your code more testable.
    /// I refer to this kind of code below as 'jumping through hoops'.
    /// </summary>
    public class FakeStartup
    {
        public readonly IHost BuiltHost;
        public readonly Startup Instance;

        public FakeStartup()
        {
            var configuration = GetStartupConfiguration();
            this.BuiltHost = CreateHostBuilder(configuration, new string[] { }).Build();
            this.Instance = Startup.Instance;
        }

        private static IConfiguration GetStartupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.UseStartup<Startup>();
                       });
        }
    }

    public class Startup
    {
        public static Startup Instance = null;

        public Startup(IConfiguration configuration)
        {
            Instance = this;
            Configuration = configuration;
        }

        public readonly IConfiguration Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // could put DI stuff for unit tests here!
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // we don't actually WANT to make a pipeline, as we are not going to run this.
        }
    }
}
