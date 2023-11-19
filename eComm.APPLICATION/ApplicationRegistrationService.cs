﻿using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace eComm.APPLICATION
{
    public static class ApplicationRegistrationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            return services;
        }
    }
}