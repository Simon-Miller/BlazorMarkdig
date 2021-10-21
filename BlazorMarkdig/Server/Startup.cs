using BlazorMarkdig.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace BlazorMarkdig.Server
{
    /// <summary>
    /// NOTE: We're dependent on some external emulators!  Are you running:
    /// https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator
    /// and
    /// https://aka.ms/cosmosdb-emulator
    /// Both run from your local machine.  Cosmos is an app - fire, and forget, until you want to explore what's stored.
    /// Blob (storage) emulator runs as command line.  Very easy, 2 mins to learn! (why we use it for demo)
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(); // registers our "proxy" api to the real MyOverflow.Api
            services.AddRazorPages();

            // for the server-side MyOverflowProxy controller to access the 'real' one.
            //services.AddScoped(sp => new HttpClient
            //{
            //    BaseAddress = new System.Uri("https://localhost:44394/")
            //});

            services.AddSignalR(options => 
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();

                // will this fix my SignalR issue?
                app.UseCors(policy =>
                    policy.WithOrigins("https://localhost:44394/") // MyOverflow.Api (fake Azure)
                          .AllowAnyMethod());
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<HelloWorldHub>("/helloWorldHub"); // dictates the "path" name used by clients.

                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
