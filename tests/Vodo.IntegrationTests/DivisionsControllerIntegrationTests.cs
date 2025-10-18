using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Vodo.IntegrationTests
{
    public class DivisionsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly CustomWebApplicationFactory factory;

        public DivisionsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task GetList_ReturnsOk_AndContainsSeededItems()
        {
            var client = factory.CreateClient();

            var resp = await client.GetAsync("/api/divisions");
            resp.EnsureSuccessStatusCode();

            var items = await resp.Content.ReadFromJsonAsync<object[]>();
            Assert.NotNull(items);
            Assert.True(items.Length >= 2);
        }
        
        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var client = factory.CreateClient();

            var payload = new { 
                name = "Integration Division",
                geometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.5,20.5]}"

            }; // Geometry опускаем
            var resp = await client.PostAsJsonAsync("/api/divisions", payload);

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

            // можно получить id из тела
            var id = await resp.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenExists()
        {
            var client = factory.CreateClient();

            // создаём новый элемент
            var createResp = await client.PostAsJsonAsync("/api/divisions", new { name = "ToUpdate" });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            // обновляем
            var updatePayload = new { id = id, name = "UpdatedName" };
            var putResp = await client.PutAsJsonAsync($"/api/divisions/{id}", updatePayload);
            putResp.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenExists()
        {
            var client = factory.CreateClient();

            var resp = await client.GetAsync("/api/divisions");
            resp.EnsureSuccessStatusCode();
            var items2 = await resp.Content.ReadFromJsonAsync<object[]>();

            var createResp = await client.PostAsJsonAsync("/api/divisions", new { name = "ToDelete" });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();


            var resp3 = await client.GetAsync("/api/divisions");
            resp3.EnsureSuccessStatusCode();
            var items23 = await resp3.Content.ReadFromJsonAsync<object[]>();

            var delResp = await client.DeleteAsync($"/api/divisions/{id}");
            Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);
        }
    }
}