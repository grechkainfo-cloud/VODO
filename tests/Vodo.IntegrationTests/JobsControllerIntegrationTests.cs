using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Vodo.IntegrationTests
{
    public class JobsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public JobsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetList_ReturnsOk_AndContainsItems()
        {
            var resp = await _client.GetAsync("/api/jobs");

            // Если ответ не успешный — соберём подробную информацию и упадём с информативным сообщением
            if (!resp.IsSuccessStatusCode)
            {
                string body = string.Empty;
                try
                {
                    body = await resp.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    body = $"(Ошибка при чтении тела ответа: {ex.Message})";
                }

                var headers = string.Join(Environment.NewLine,
                    resp.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));

                var contentHeaders = resp.Content?.Headers != null
                    ? string.Join(Environment.NewLine, resp.Content.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
                    : string.Empty;

                var message =
$@"HTTP request failed.
Status: {(int)resp.StatusCode} {resp.ReasonPhrase}
Response headers:
{headers}
Content headers:
{contentHeaders}
Body:
{body}";

                // Проваливаем тест с подробным сообщением — это даст стек в выводе теста.
                Assert.True(false, message);
            }

            resp.EnsureSuccessStatusCode();

            var items = await resp.Content.ReadFromJsonAsync<object[]>();
            Assert.NotNull(items);
        }

        [Fact]
        public async Task Create_ReturnsCreated_AndReturnsId()
        {
            // Создадим сначала JobObject, чтобы получить JobObjectId
            var sitePayload = new
            {
                name = "Integration Site for Job"
            };

            var siteResp = await _client.PostAsJsonAsync("/api/jobobjects", sitePayload);
            siteResp.EnsureSuccessStatusCode();
            var siteId = await siteResp.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, siteId);

            var payload = new
            {
                title = "Integration Job",
                description = "Integration test job",
                type = 1, // JobType.Excavation
                statusId = 1,
                jobObjectId = siteId,
                geometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.5,20.5]}"
            };

            var resp = await _client.PostAsJsonAsync("/api/jobs", payload);

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

            var id = await resp.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenExists()
        {
            // Создадим JobObject
            var siteResp = await _client.PostAsJsonAsync("/api/jobobjects", new { name = "ToCreateJobForUpdate" });
            siteResp.EnsureSuccessStatusCode();
            var siteId = await siteResp.Content.ReadFromJsonAsync<Guid>();

            // Создадим Job
            var createResp = await _client.PostAsJsonAsync("/api/jobs", new
            {
                title = "ToUpdate",
                type = 1,
                statusId = 1,
                jobObjectId = siteId
            });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            var updatePayload = new
            {
                id = id,
                title = "UpdatedJobTitle"
                // не передаём geometry для простоты (UpdateJobCommand принимает Geometry объект)
            };

            var putResp = await _client.PutAsJsonAsync($"/api/jobs/{id}", updatePayload);
            putResp.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenExists()
        {
            // Создадим JobObject
            var siteResp = await _client.PostAsJsonAsync("/api/jobobjects", new { name = "ToCreateJobForDelete" });
            siteResp.EnsureSuccessStatusCode();
            var siteId = await siteResp.Content.ReadFromJsonAsync<Guid>();

            // Создадим Job
            var createResp = await _client.PostAsJsonAsync("/api/jobs", new
            {
                title = "ToDelete",
                type = 1,
                statusId = 1,
                jobObjectId = siteId
            });
            createResp.EnsureSuccessStatusCode();
            var id = await createResp.Content.ReadFromJsonAsync<Guid>();

            var delResp = await _client.DeleteAsync($"/api/jobs/{id}");
            Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);
        }
    }
}
