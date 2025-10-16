using System;
using Microsoft.Data.Sqlite;
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
            services.AddDbContext<VodoContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var connection = new SqliteConnection(connectionString);

                connection.Open();
                connection.EnableExtensions(true);

                try
                {
                    connection.LoadExtension("C:\\Users\\mgera\\Downloads\\mod_spatialite-5.1.0-win-amd64\\mod\\mod_spatialite-5.1.0-win-amd64\\mod_spatialite.dll");
                }
                catch (Exception exc)
                {
                    // при ошибке загрузки — логгировать/перекинуть с объяснением
                    throw new InvalidOperationException("Не удалось загрузить mod_spatialite. Проверьте наличие нативной библиотеки в выходной папке.");
                }

                // Инициализировать системные таблицы SpatiaLite (выполняется один раз)
                try
                {
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT InitSpatialMetadata();";
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    // если уже инициализировано — можно игнорировать
                }

                options.UseSqlite(connection, x => x.UseNetTopologySuite());
                options.EnableSensitiveDataLogging(true);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }
    }
}
