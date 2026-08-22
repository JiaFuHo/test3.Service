using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using test3.DAL.test3.Context;

namespace test3.DAL
{
    public static class DI
    {
        public static IServiceCollection ConnDB(this IServiceCollection services, String? ConnTest3 = null)
        {
            services.AddDbContext<test3Context>(options =>
            {
                options.UseSqlServer(ConnTest3).ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.SqlServerEventId.ByteIdentityColumnWarning));
            });

            return services;
        }
    }
}