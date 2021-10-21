using Microsoft.Extensions.DependencyInjection;
using MyOverflow.Shared;
using System.Threading.Tasks;

namespace MyOverflow.DataAccess.Cosmos
{
    public static class StartupExtensions
    {
        public static void AddQAContext(this IServiceCollection services)
        {
            // CosmosDb contexts are supposed to be re-used as much as possible.
            // So Singleton makes it the same instance to be shared across all calls to this api.
            services.AddSingleton<MyOverflowQAContextFactory>();
            services.AddSingleton<IQAContext>(svc =>
            {
                var factory = svc.GetService<MyOverflowQAContextFactory>();
                var task = Task.Run(async () =>
                {
                    return await factory.Make();
                });

                /*
                    Seeing an error like "actively refused connection on localhost:8081" ??
                    ARE YOU RUNNING AZURE STORAGE EMULATOR?
                    ARE YOU RUNNING AZURE COSMOS DB EMULATOR?
                 */
                return task.GetAwaiter().GetResult();
            });
        }
    }
}
