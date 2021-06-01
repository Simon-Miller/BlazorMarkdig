using Microsoft.Extensions.DependencyInjection;

namespace MyOverflow.DataAccess.Blob
{
    public static class StartupExtensions
    {
        public static void AddBlobContext(this IServiceCollection services)
        {
            services.AddSingleton<IBlobContext, BlobContext>(); // zero configuration, as we can use out-of-the-box defaults
        }
    }
}
