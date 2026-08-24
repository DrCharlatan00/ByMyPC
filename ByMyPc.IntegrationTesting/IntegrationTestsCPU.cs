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

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOSmallModel>>();

            Assert.NotNull(data);

        }

        [Fact]
        public async Task TestGetFull()
        {
            var request = await httpClient.GetAsync("/api/cpu/full");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

            Assert.True(request.IsSuccessStatusCode);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOCpuModel>>();

            Assert.NotNull(data);

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
        public async Task TestGetByID() {
            Guid id = Guid.Empty;
            CpuCreateModel testmodel = new CpuCreateModel
            (
                "Test",
                "Test"
            );
            try
            {
                id = await cpuRepo.CreateAsync(testmodel);
            }
            catch (Exception ex) {
                Assert.Fail($"Can't create model {ex.Message}");
            }

            try
            {
                var request = await httpClient.GetAsync($"/api/cpu/{id}");
                if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

                Assert.True(request.IsSuccessStatusCode);

                var data = await request.Content.ReadFromJsonAsync<RDTOCpuModel>();

                Assert.NotNull(data);
                Assert.Equal("Test", data.Name);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally {
                await cpuRepo.RemoveAsync(id);
            }
        }

        [Fact]
        public async Task TestGetCardPag()
        {
            var request = await httpClient.GetAsync("/api/cpu/card-pag?page=1&pageSize=1");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

            Assert.True(request.IsSuccessStatusCode);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOSmallModel>>();

            Assert.NotNull(data);

            Assert.Single(data);

        }

        [Fact]
        public async Task TestSearchCardPag()
        {
            Guid id = Guid.Empty;
            CpuCreateModel testmodel = new CpuCreateModel
            (
                "Test",
                "Test"
            );
            try
            {
                id = await cpuRepo.CreateAsync(testmodel);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Can't create model {ex.Message}");
            }
            try
            {
                var request = await httpClient.GetAsync("/api/cpu/search-name-pag?name=Test&page=1&pageSize=1");
                if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

                Assert.True(request.IsSuccessStatusCode);

                var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOSmallModel>>();

                Assert.NotNull(data);

                Assert.Single(data);
                Assert.Equal("Test", data.First().Name);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await cpuRepo.RemoveAsync(id);
            }

        }


        [Fact]
        public async Task TestSearchCard()
        {
            Guid id = Guid.Empty;
            CpuCreateModel testmodel = new CpuCreateModel
            (
                "Test",
                "Test"
            );
            try
            {
                id = await cpuRepo.CreateAsync(testmodel);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Can't create model {ex.Message}");
            }
            try
            {
                var request = await httpClient.GetAsync("/api/cpu/search-name?name=Test");
                if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

                Assert.True(request.IsSuccessStatusCode);

                var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOSmallModel>>();

                Assert.NotNull(data);
                Assert.Equal("Test", data.First().Name);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await cpuRepo.RemoveAsync(id);
            }

        }

        [Fact]
        public async Task TestUpdate() {
            Guid id = Guid.Empty;
            CpuCreateModel testmodel = new CpuCreateModel
            (
                "Test",
                "Test"
            );
            try
            {
                id = await cpuRepo.CreateAsync(testmodel);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Can't create model {ex.Message}");
            }
            try
            {
                DTOCpuUpdateModel updateModel = new DTOCpuUpdateModel(
                        id: id,
                        Name: "TTTT",
                        Socket: "TTTT",
                        Frequency: 0,
                        Count_Cores: 0,
                        false
                    );

                var result = await httpClient.PutAsJsonAsync("/api/cpu/", updateModel);
                if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

                Assert.True(result.IsSuccessStatusCode);

                var data = await result.Content.ReadFromJsonAsync<RDTOCpuModel>();

                Assert.NotNull(data);
                Assert.Equal("TTTT", data.Name);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally {
                await cpuRepo.RemoveAsync(id);
            }
        }

        [Fact]
        public async Task TestCreate() {
            CpuCreateModel testmodel = new CpuCreateModel
            (
             "Test",
             "Test"
            );
            try
            {
                var result = await httpClient.PostAsJsonAsync("/api/cpu/", testmodel);
                if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

                Assert.True(result.IsSuccessStatusCode);
                Guid guid = await result.Content.ReadFromJsonAsync<Guid>();

                if (await cpuRepo.GetByIDAsync(guid) is null) Assert.Fail("Product not in db, but answer is ok");

                await cpuRepo.RemoveAsync(guid);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        public async Task TestRemove() {
            Guid id = Guid.Empty;
            CpuCreateModel testmodel = new CpuCreateModel
            (
                "Test",
                "Test"
            );
            try
            {
                id = await cpuRepo.CreateAsync(testmodel);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Can't create model {ex.Message}");
            }

            var result = await httpClient.DeleteAsync($"/api/cpu/{id}");
            if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

            Assert.True(result.IsSuccessStatusCode);

            if (await cpuRepo.GetByIDAsync(id) is not null) Assert.Fail("Data not removed in db");

        }
    
    }
}
