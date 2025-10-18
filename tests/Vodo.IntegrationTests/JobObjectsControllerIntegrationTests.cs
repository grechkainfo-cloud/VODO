using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Vodo.IntegrationTests
{
    public class JobObjectsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public JobObjectsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetList_ReturnsOk_AndContainsItems()
        {
            var resp = await _client.GetAsync("/api/jobobjects");
            resp.EnsureSuccessStatusCode();

            var items = await resp.Content.ReadFromJsonAsync<object[]>();
            Assert.NotNull(items);
        }

        [Fact]
        public async Task Create_ReturnsCreated_AndReturnsId()
        {
            var payload = new
            {
                name = "Integration Site",
                geometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.5,20.5]}",
                address = new { line1 = "Street 1", city = "City" },
                ownerDivision = "Ops"
            };

            var resp = await _client.PostAsJsonAsync("/api/jobobjects", payload);

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

            var id = await resp.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenExists()
        {
            var createResp = await _client.PostAsJsonAsync("/api/jobobjects", new { name = "ToUpdate" });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            var updatePayload = new
            {
                id = id,
                name = "UpdatedSite",
                geometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[5,6]}"
            };

            var putResp = await _client.PutAsJsonAsync($"/api/jobobjects/{id}", updatePayload);
            putResp.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenExists()
        {
            var createResp = await _client.PostAsJsonAsync("/api/jobobjects", new { name = "ToDelete" });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            var delResp = await _client.DeleteAsync($"/api/jobobjects/{id}");
            Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);
        }
    }
}
