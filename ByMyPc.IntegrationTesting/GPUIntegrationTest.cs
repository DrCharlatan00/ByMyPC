using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.GPUModels.RDTO;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace ByMyPc.IntegrationTesting;

public class GPUIntegrationTest : IClassFixture<TestWebApplicationFactory>
{
    private readonly IGPURepo repo;
    private readonly HttpClient client;

    public GPUIntegrationTest(TestWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        repo = scope.ServiceProvider.GetRequiredService<IGPURepo>();
        client = factory.CreateClient();
    }
    #region Test Get Methods
    [Fact]
    public async Task TestGetAll()
    {
        var request = await client.GetAsync("api/gpu/full");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUModel>>();

        Assert.NotNull(data);
        Assert.NotEmpty(data);
    }

    [Fact]
    public async Task TestGetSmallAll()
    {
        var request = await client.GetAsync("api/gpu/");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUSmallModel>>();

        Assert.NotNull(data);
        Assert.NotEmpty(data);
    }


    [Fact]
    public async Task TestGetByID()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "Test",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync( model );
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }

        var request = await client.GetAsync($"api/gpu/{id}");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<RDTOGPUModel>();

        Assert.NotNull(data);
        Assert.Equal("Test",data.name);
        Assert.Equal("GDDR6",data.typeMemory);
   }

    [Fact]
    public async Task TestGetWithPagination()
    {
        var request = await client.GetAsync("api/gpu/card-pag?page=1&pageSize=1");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUSmallModel>>();
        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Single(data);
    }

    [Fact]
    public async Task TestSearch()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "Test",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync(model);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }
        var request = await client.GetAsync("api/gpu/search-name?name=Test");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUModel>>();
        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Equal("Test", data.First().name);
    }

    [Fact]
    public async Task TestSearchByPagination()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "Test",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync(model);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }
        var request = await client.GetAsync("api/gpu/search-name-pag?name=Test&page=1&pageSize=1");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUSmallModel>>();
        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Equal("Test", data.First().name);
    }

    [Fact]
    public async Task TestGetByFilter()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "Test",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync(model);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }

        var request = await client.GetAsync("api/gpu/by-filter?name=Test&videoSizeMemory=2096");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUModel>>();

        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Equal("Test",data.First().name);
        Assert.True(2096 == data.First().videoMemorySize);

    }

    [Fact]
    public async Task TestGetByFilterWithPagination()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "Test",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync(model);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }

        var request = await client.GetAsync("api/gpu/by-filter-pag?name=Test&videoSizeMemory=2096&page=1&pageSize=1");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOGPUSmallModel>>();

        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Single(data);
        Assert.Equal("Test", data.First().name);
        Assert.True(2096 == data.First().videoMemorySize);

    }
    #endregion

    #region Test Update 
    [Fact(Skip = "Interesting bug")]
    public async Task TestUpdate()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "Test",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync(model);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }
        DTOGPUUpdateModel modelUpdate = new(id,
                                      "TTTT",
                                      null,
                                      null,
                                      null,
                                      null,
                                      null);
        var request = await client.PutAsJsonAsync("api/gpu/", modelUpdate);
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await repo.GetByID(id); //this not send actual information, but he not have bug

        Assert.NotNull(data);
        Assert.Equal("TTTT",data.Name);
    }
    #endregion

    #region Create
    [Fact]
    public async Task TestCreate()
    {
        DTOGPUCreateModel create = new("TestCreate",2096,Postgresql.Models.VideoSlots.PCI_E,256,"PCI_E 3.0","GDDR3");
        var request = await client.PostAsJsonAsync("api/gpu/", create);
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);
        var ans = await request.Content.ReadFromJsonAsync<Guid>();
        var data = await repo.GetByID(ans);
        Assert.NotNull(data);
        Assert.Equal("TestCreate",data.Name);
    }
    #endregion
    #region Remove
    [Fact]
    public async Task TestRemove()
    {
        Guid id = Guid.Empty;
        try
        {
            GPUCreateModel model = new GPUCreateModel(
                "TestDelete",
                2096,
                Postgresql.Models.VideoSlots.PCI_E,
                916,
                "PCI_E",
                "GDDR6"
                );
            id = await repo.CreateAsync(model);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Can't create test data ex, {ex}");
        }

        var request = await client.DeleteAsync($"api/gpu/{id}");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);
        await foreach (var item in repo.GetSmallModelsDbAsync(default))
        {
            if (item.Name == "TestDelete") Assert.Fail("data not removed");
        }
    }
    #endregion

}
