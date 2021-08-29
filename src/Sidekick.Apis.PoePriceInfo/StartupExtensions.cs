using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.PoePriceInfo
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoePriceInfoApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoePriceInfoClient, PoePriceInfoClient>();

            return services;
        }
    }
}