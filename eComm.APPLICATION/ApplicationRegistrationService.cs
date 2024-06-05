using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Helpers;
using eComm.APPLICATION.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace eComm.APPLICATION
{
    public static class ApplicationRegistrationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>()
                    .AddScoped<IRecommenderService, RecommenderService>()
                    .AddScoped<IProductService, ProductService>()
                    .AddScoped<ICartManagementService, CartManagementService>()
                    .AddScoped<IAuthHelper, AuthHelper>()
                    .AddSingleton<IShareService, ShareService>();
            return services;
        }
    }
}
