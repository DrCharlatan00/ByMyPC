using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ByMyPc.IntegrationTesting
{
    public class IntegrationTestsCPU : IClassFixture<TestWebApplicationFactory>
    {
        private readonly ITestOutputHelper output;
        private readonly ICpuRepo cpuRepo;
        private readonly HttpClient httpClient;

        public IntegrationTestsCPU(TestWebApplicationFactory factory, ITestOutputHelper output)
        {
            var scope = factory.Services.CreateScope();
            cpuRepo = scope.ServiceProvider.GetRequiredService<ICpuRepo>();
            httpClient = factory.CreateClient();
            this.output = output;
        }

        [Fact]
        public async Task TestGetCard()
        {
            var request = await httpClient.GetAsync("/api/cpu");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
            Assert.True(request.IsSuccessStatusCode);
            Assert.NotNull(await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuSmallModel>>());
        }

        [Fact]
        public async Task TestGetFull()
        {
            var request = await httpClient.GetAsync("/api/cpu/full");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
            Assert.True(request.IsSuccessStatusCode);
            Assert.NotNull(await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuModel>>());
        }

        [Fact]
        public async Task TestGetFullPag()
        {
            var request = await httpClient.GetAsync("/api/cpu/full-pag?page=1&pageSize=1");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
            Assert.True(request.IsSuccessStatusCode);
            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuModel>>();
            Assert.NotNull(data);
            Assert.Single(data);
        }

        [Fact]
        public async Task TestGetByID()
        {
            var id = await CreateCpuDirectly();
            try
            {
                var request = await httpClient.GetAsync($"/api/cpu/{id}");
                if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
                Assert.True(request.IsSuccessStatusCode);
                var data = await request.Content.ReadFromJsonAsync<RDTOCpuModel>();
                Assert.NotNull(data);
                Assert.Equal("Test", data.Name);
            }
            finally { await cpuRepo.RemoveAsync(id); }
        }

        [Fact]
        public async Task TestGetCardPag()
        {
            var request = await httpClient.GetAsync("/api/cpu/card-pag?page=1&pageSize=1");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
            Assert.True(request.IsSuccessStatusCode);
            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuSmallModel>>();
            Assert.NotNull(data);
            Assert.Single(data);
        }

        [Fact]
        public async Task TestSearchCardPag()
        {
            var id = await CreateCpuDirectly();
            try
            {
                var request = await httpClient.GetAsync("/api/cpu/search-name-pag?name=Test&page=1&pageSize=1");
                if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
                Assert.True(request.IsSuccessStatusCode);
                var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuSmallModel>>();
                Assert.NotNull(data);
                Assert.Contains(data, item => item.Name == "Test");
            }
            finally { await cpuRepo.RemoveAsync(id); }
        }

        [Fact]
        public async Task TestSearchCard()
        {
            var id = await CreateCpuDirectly();
            try
            {
                var request = await httpClient.GetAsync("/api/cpu/search-name?name=Test");
                if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);
                Assert.True(request.IsSuccessStatusCode);
                var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuSmallModel>>();
                Assert.NotNull(data);
                Assert.All(data, item => Assert.Contains("Test", item.Name));
            }
            finally { await cpuRepo.RemoveAsync(id); }
        }

        [Fact]
        public async Task TestFilters()
        {
            var result = await httpClient.GetAsync("/api/cpu/by-filter?ByLive=true");
            if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);
            Assert.True(result.IsSuccessStatusCode);
            var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuModel>>();
            Assert.NotNull(data);
            Assert.All(data, item => Assert.True(item.IsLive));
        }

        [Fact]
        public async Task TestFiltersWithPagination()
        {
            var result = await httpClient.GetAsync("/api/cpu/by-filter-pag?ByLive=true&page=1&pageSize=1");
            if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);
            Assert.True(result.IsSuccessStatusCode);
            var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuSmallModel>>();
            Assert.NotNull(data);
        }

        [Fact]
        public async Task TestUpdate()
        {
            var id = await CreateCpuDirectly();
            try
            {
                var updateModel = new DTOCpuUpdateModel(id, "TTTT", "TTTT", 0, 0, false);
                using var request = new HttpRequestMessage(HttpMethod.Put, "/api/cpu/update/v2")
                {
                    Content = JsonContent.Create(updateModel)
                };
                request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) output.WriteLine(response.ReasonPhrase);
                Assert.True(response.IsSuccessStatusCode);
                var data = await response.Content.ReadFromJsonAsync<RDTOCpuModel>();
                Assert.NotNull(data);
                Assert.Equal("TTTT", data.Name);
            }
            finally { await cpuRepo.RemoveAsync(id); }
        }

        [Fact]
        public async Task TestCreate()
        {
            var testModel = new DTOCpuCreateModel("Test", "Test", 0, 0, false);
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/cpu/create/v2")
            {
                Content = JsonContent.Create(testModel)
            };
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) output.WriteLine(response.ReasonPhrase);
            Assert.True(response.IsSuccessStatusCode);
            var id = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.NotNull(await cpuRepo.GetByIDAsync(id));
            await cpuRepo.RemoveAsync(id);
        }

        [Fact]
        public async Task TestRemove()
        {
            var id = await CreateCpuDirectly();
            var result = await httpClient.DeleteAsync($"/api/cpu/{id}");
            if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);
            Assert.True(result.IsSuccessStatusCode);
            Assert.Null(await cpuRepo.GetByIDAsync(id));
        }

        private async Task<Guid> CreateCpuDirectly()
        {
            var model = new CpuCreateModel("Test", "Test");
            var (id, _) = await cpuRepo.CreateAsync(
                model,
                Guid.NewGuid().ToString(),
                CancellationToken.None);
            return id;
        }
    }
}
