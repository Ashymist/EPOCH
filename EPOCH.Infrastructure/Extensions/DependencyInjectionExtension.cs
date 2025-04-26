using EPOCH.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Infrastructure.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString) {
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            return services;
        }
    }
}
