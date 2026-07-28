using test3.DAL.test3.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace test3.DAL
{
    public static class DI
    {
        public static IServiceCollection ConnDB(this IServiceCollection services, String? ConnTest3 = null)
        {
            services.AddDbContext<test3Context>(options => { options.UseSqlServer(ConnTest3, optionsX => optionsX.UseCompatibilityLevel(120)); });

            return services;
        }
    }
}