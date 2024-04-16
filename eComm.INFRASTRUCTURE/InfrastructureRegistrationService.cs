using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using eComm.INFRASTRUCTURE.Contracts;
using eComm.INFRASTRUCTURE.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace eComm.INFRASTRUCTURE
{
    public static class InfrastructureRegistrationService
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IExternalDepRepository, ExternalDepRepository>()
                    .AddScoped<IPaymentService, PaymentService>()
                    .AddScoped<IEmailService, EmailService>()
                    .AddScoped<IScrapperService, ScrapperService>();
            return services;
        }
    }
}
