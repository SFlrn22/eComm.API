using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace eComm.APPLICATION
{
    public static class ApplicationRegistrationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRecommenderService, RecommenderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddSingleton<IShareService, ShareService>();
            return services;
        }
    }
}
