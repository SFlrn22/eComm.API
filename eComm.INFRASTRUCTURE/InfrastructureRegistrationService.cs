using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using eComm.INFRASTRUCTURE.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace eComm.INFRASTRUCTURE
{
    public static class InfrastructureRegistrationService
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string user = configuration.GetSection("AppSettings:WrapperConfiguration:Username").Value!;
            string password = configuration.GetSection("AppSettings:WrapperConfiguration:Password").Value!;
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

            services.AddScoped<IExternalDepRepository, ExternalDepRepository>()
                    .AddScoped<IPaymentService, PaymentService>()
                    .AddScoped<IEmailService, EmailService>()
                    .AddScoped<IScrapperService, ScrapperService>()
                    .AddHttpClient("Wrapper", c =>
                    {
                        c.BaseAddress = new Uri("http://127.0.0.1:8000");
                        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
                    });

            return services;
        }
    }
}
