using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    // Extension Methods are used to extend services and always should be static
    // No need to create new instance of these classes
    public static class ApplicationServiceExtensions
    {
        // use this keyword to extend a service always
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            // add dbcontext class here so that it can be injected to other parts of application
            services.AddDbContext<DataContext>(Options =>
            {
                Options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}