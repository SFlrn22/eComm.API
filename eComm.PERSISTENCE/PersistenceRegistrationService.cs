using eComm.PERSISTENCE.Contracts;
using eComm.PERSISTENCE.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace eComm.PERSISTENCE
{
    public static class PersistenceRegistrationService
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
