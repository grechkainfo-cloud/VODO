using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vodo.DAL;
using Vodo.DAL.Context;

namespace Vodo.UnitTests
{
    public class VodoContext_SpatialiteExtension_IsLoaded
    {
        [Fact]
        public async Task SpatialiteExtension_IsLoaded_WhenCreatingVodoContext()
        {
            // Arrange: ������������ � in-memory SQLite
            var settings = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:"
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);

            // ������������ DAL (������ AddDAL ���������� ������� LoadExtension)
            services.AddDAL(configuration);

            await using var provider = services.BuildServiceProvider();
            await using var scope = provider.CreateAsyncScope();

            // Act: ��������� �������� � ��� ������ ��������� ���, ����������� ����������
            VodoContext? context = null;
            try
            {
                context = scope.ServiceProvider.GetRequiredService<VodoContext>();

                // ��������, ��� ����������� ������� (��� in-memory �����, ����� ���������� ���� ���� � �� ��)
                var conn = context.Database.GetDbConnection() as SqliteConnection
                           ?? throw new InvalidOperationException("��������� SqliteConnection.");

                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                // �������� ����� �������, ��������������� SpatiaLite
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT spatialite_version();";
                var versionObj = await cmd.ExecuteScalarAsync();

                // Assert
                Assert.NotNull(versionObj);
                var versionStr = versionObj.ToString();
                Assert.False(string.IsNullOrWhiteSpace(versionStr), "spatialite_version() ������ ������ ������.");
            }
            catch (Exception ex)
            {
                // � ����� ����� ���� � ������������ � ���� ���������� �� ��������, ������� �������
                Assert.False(true, $"������ ��� �������� �������� mod_spatialite: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                if (context != null)
                {
                    await context.DisposeAsync();
                }
            }
        }
    }
}