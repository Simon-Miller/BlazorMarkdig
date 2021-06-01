using BlazorMarkdig.Client.Classes.CodeServices;
using BlazorMarkdig.Client.Hubs;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BlazorMarkdig.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            ConfigureServices(builder, builder.Services);

            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(WebAssemblyHostBuilder builder, IServiceCollection services)
        {
            //services.AddSingleton(provider =>
            //{
            //    var config = provider.GetService<IConfiguration>();
            //    return config.GetSection("App").Get<IConfiguration>();
            //});

            // the HTTP Client within webAssembly points back to Blazor server ??
            //services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            services.AddSingleton<MarkdigParser>();
            services.AddSingleton<IChatHubProxy, ChatHubProxy>();

            //services.AddHttpClient<IMyOverflowProxy, MyOverflowProxy>(Client =>)




            //services.AddScoped((serv) =>
            //{
            //    //var uri = serv.GetService<IConfiguration>().GetValue<string>("MyOverflowUri");

            //    return new MyOverflowProxy("https://localhost:44394/");
            //});



        }
    }
}
