using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Models.RAMModels.RDTO;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace ByMyPc.IntegrationTesting
{
    public class RAMIntegrationTest : IClassFixture<TestWebApplicationFactory>
    {
        private readonly IRamRepo repo;
        private readonly HttpClient client;

        public RAMIntegrationTest(TestWebApplicationFactory factory)
        {
            var scope = factory.Services.CreateScope();
            repo = scope.ServiceProvider.GetRequiredService<IRamRepo>();
            client = factory.CreateClient();
        }

        [Fact]
        public async Task TestGetFullAll()
        {
            var request = await client.GetAsync("/api/ram/full");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMModel>>();

            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task TestGetSmalllAll()
        {
            var request = await client.GetAsync("/api/ram/");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMSmallModel>>();

            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task TestGetByID()
        { Guid guid = Guid.Empty;
            try
            {
                RamCreateModel createModel = new("Test",1028,Postgresql.Models.TypeDDR.DDR2,663,true);
                guid = await repo.CreateAsync(createModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                var request = await client.GetAsync($"/api/ram/{guid}");
                Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

                var item = await request.Content.ReadFromJsonAsync<RDTORAMModel>();

                Assert.NotNull(item);
                Assert.Equal("Test", item.name);
                Assert.Equal(Postgresql.Models.TypeDDR.DDR2, item.Type);
            }
            catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await repo.RemoveAsync(guid);
            }
        }

        [Fact]
        public async Task TestGetFullPaginated()
        {
            var request = await client.GetAsync("/api/ram/full-pag?page=1&pageSize=1");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMModel>>();

            Assert.NotNull(data);
            Assert.NotEmpty(data);
            Assert.Single(data);
        }

        [Fact]
        public async Task TestGetSmallPaginated()
        {
            var request = await client.GetAsync("/api/ram/card-pag?page=1&pageSize=1");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMSmallModel>>();

            Assert.NotNull(data);
            Assert.NotEmpty(data);
            Assert.Single(data);
        }

        [Fact]
        public async Task TestSearchByName()
        {
            Guid guid = Guid.Empty;
            try
            {
                RamCreateModel createModel = new("Test", 1028, Postgresql.Models.TypeDDR.DDR2, 663, true);
                guid = await repo.CreateAsync(createModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                var request = await client.GetAsync($"/api/ram/search-name?name=Test");
                Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

                var item = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMModel>>();

                Assert.NotNull(item);
                Assert.Equal("Test", item.First().name);
                Assert.Equal(Postgresql.Models.TypeDDR.DDR2, item.First().Type);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await repo.RemoveAsync(guid);
            }
        }

        [Fact]
        public async Task TestSearchByNamePaginated()
        {
            Guid guid = Guid.Empty;
            try
            {
                RamCreateModel createModel = new("Test", 1028, Postgresql.Models.TypeDDR.DDR2, 663, true);
                guid = await repo.CreateAsync(createModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                var request = await client.GetAsync($"/api/ram/search-name-pag?name=Test&page=1&pageSize=1");
                Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

                var item = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMSmallModel>>();

                Assert.NotNull(item);
                Assert.Equal("Test", item.First().name);
                Assert.Single(item);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await repo.RemoveAsync(guid);
            }
        }

        [Fact]
        public async Task TestGetByFilter()
        {
            Guid guid = Guid.Empty;

            try
            {
                RamCreateModel createModel = new("Test", 1028, Postgresql.Models.TypeDDR.DDR2, 663, true);
                guid = await repo.CreateAsync(createModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                DTORAMFilter filter = new("Test",1028,null, null, null);
                var request = await client.GetAsync($"/api/ram/by-filter?filter={filter}");
                Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

                var item = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMModel>>();

                Assert.NotNull(item);
                Assert.NotEmpty(item);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await repo.RemoveAsync(guid);
            }
        }

        [Fact]
        public async Task TestGetByFilterPaginated()
        {
            Guid guid = Guid.Empty;

            try
            {
                RamCreateModel createModel = new("Test", 1028, Postgresql.Models.TypeDDR.DDR2, 663, true);
                guid = await repo.CreateAsync(createModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                DTORAMFilter filter = new("Test", 1028, null, null, null);
                var request = await client.GetAsync($"/api/ram/by-filter-pag?filter={filter}&page=1&pageSize=1");
                Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

                var item = await request.Content.ReadFromJsonAsync<IEnumerable<RDTORAMSmallModel>>();

                Assert.NotNull(item);
                Assert.NotEmpty(item);
                Assert.Single(item);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                await repo.RemoveAsync(guid);
            }
        }
    }
}
