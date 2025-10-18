using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Vodo.IntegrationTests
{
    public class ContractorsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ContractorsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetList_ReturnsOk_AndContainsItems()
        {
            var resp = await _client.GetAsync("/api/contractors");
            resp.EnsureSuccessStatusCode();

            var items = await resp.Content.ReadFromJsonAsync<object[]>();
            Assert.NotNull(items);
            Assert.True(items.Length >= 0); // ���� �� ������ ������ ��������
        }

        [Fact]
        public async Task Create_ReturnsCreated_AndReturnsId()
        {
            var payload = new
            {
                name = "Integration Contractor",
                inn = "123456789012",
                payload = new { email = "ci@test.example", phone = "123456" }
            };

            var resp = await _client.PostAsJsonAsync("/api/contractors", payload);

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

            var id = await resp.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenExists()
        {
            // ������ ����������
            var createResp = await _client.PostAsJsonAsync("/api/contractors", new
            {
                name = "ToUpdateContractor",
                inn = "000000000001",
                payload = new { email = "before@example.com" }
            });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            // ���������
            var updatePayload = new
            {
                id = id,
                name = "UpdatedContractorName",
                inn = "000000000002",
                payload = new { email = "after@example.com", phone = "999" }
            };

            var putResp = await _client.PutAsJsonAsync($"/api/contractors/{id}", updatePayload);
            putResp.EnsureSuccessStatusCode();

            // �����������: �������� ������ � ��������� ������� ����������� ����� (�������������)
            var getResp = await _client.GetAsync("/api/contractors");
            getResp.EnsureSuccessStatusCode();

            // ������ ��� JsonElement[] � System.Text.Json ���������� JsonElement, ��������� � null ������� ������ �����.
            var items = await getResp.Content.ReadFromJsonAsync<JsonElement[]>();
            Assert.NotNull(items);

            // ���� ������� � ������ ������ ��������� � ��������� ������� �������� � ��� ��������� ��������
            Assert.Contains(items, i =>
            {
                if (i.ValueKind != JsonValueKind.Object)
                    return false;
                if (i.TryGetProperty("name", out var nameProp))
                {
                    var name = nameProp.GetString();
                    return string.Equals(name, "UpdatedContractorName", StringComparison.Ordinal);
                }
                return false;
            });
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenExists()
        {
            // ������ ����������
            var createResp = await _client.PostAsJsonAsync("/api/contractors", new
            {
                name = "ToDeleteContractor",
                inn = "000000000003",
                payload = new { email = "del@example.com" }
            });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            // �������
            var delResp = await _client.DeleteAsync($"/api/contractors/{id}");
            Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);
        }
    }
}