using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vodo.DAL.Context;

namespace Vodo.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDAL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VodoContext>(
                options =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlite(connectionString, x => x.UseNetTopologySuite());

                    options.EnableSensitiveDataLogging(true);
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }
            );
        }
    }
}
