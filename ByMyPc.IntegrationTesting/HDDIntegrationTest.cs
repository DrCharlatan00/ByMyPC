using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.HDDModels.RDTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ByMyPc.IntegrationTesting;


public class HDDIntegrationTest : IClassFixture<TestWebApplicationFactory>
{
    private readonly IHddRepo repo;
    private readonly HttpClient httpClient;
    private readonly ITestOutputHelper output;

    public HDDIntegrationTest(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        var Scope = factory.Services.CreateScope();
        repo = Scope.ServiceProvider.GetRequiredService<IHddRepo>();
        httpClient = factory.CreateClient();
        this.output = output;
    }

    [Fact]
    public async Task TestGetCard() {
        var request = await httpClient.GetAsync("/api/hdds");
        if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

        Assert.True(request.IsSuccessStatusCode);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOHDDCardModel>>();

        Assert.NotNull(data);
    }

    [Fact]
    public async Task TestGetFull()
    {
        var request = await httpClient.GetAsync("/api/hdds/full");
        if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

        Assert.True(request.IsSuccessStatusCode);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOHDDModel>>();

        Assert.NotNull(data);
    }

        

    [Fact]
    public async Task TestGetById()
    {
        Guid id = Guid.NewGuid();
        try
        {
            HDDCreateModel model = new HDDCreateModel(
                name: "Test",
                100,
                HddConnector.SATA
                );
            id = await repo.CreateAsync(model);
        }
        catch {
            Assert.Fail("Test model not created, Test abort");
        }
        try
        {
            var request = await httpClient.GetAsync($"/api/hdds/{id}");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

            Assert.True(request.IsSuccessStatusCode);

            var data = await request.Content.ReadFromJsonAsync<RDTOHDDModel>();

            Assert.NotNull(data);
            Assert.Equal("Test", data.Name);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally {
            try
            {
                await repo.RemoveAsync(id);
            }
            catch {
                output.WriteLine("test data not removed");
            }
        }
    }

    [Fact]
    public async Task TestGetWithPag()
    {
        var request = await httpClient.GetAsync("/api/hdds/card-pag?page=1&pageSize=1");
        if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

        Assert.True(request.IsSuccessStatusCode);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOHDDCardModel>>();

        Assert.NotNull(data);
        Assert.Single(data);
    }

    [Fact]
    public async Task TestGetByName()
    {
        Guid id = Guid.NewGuid();
        try
        {
            HDDCreateModel model = new HDDCreateModel(
                name: "Test",
                100,
                HddConnector.SATA
                );
            id = await repo.CreateAsync(model);
        }
        catch
        {
            Assert.Fail("Test model not created, Test abort");
        }
        try
        {
            var request = await httpClient.GetAsync($"/api/hdds/search-name?name=Test");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

            Assert.True(request.IsSuccessStatusCode);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOHDDModel>>();

            Assert.NotNull(data);
            Assert.Equal("Test", data.First().Name);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            try
            {
                await repo.RemoveAsync(id);
            }
            catch
            {
                output.WriteLine("test data not removed");
            }
        }
    }

    [Fact]
    public async Task TestGetByNameWithPag()
    {
        Guid id = Guid.NewGuid();
        try
        {
            HDDCreateModel model = new HDDCreateModel(
                name: "Test",
                100,
                HddConnector.SATA
                );
            id = await repo.CreateAsync(model);
        }
        catch
        {
            Assert.Fail("Test model not created, Test abort");
        }
        try
        {
            var request = await httpClient.GetAsync($"/api/hdds/search-name-pag?name=Test&page=1&pageSize=1");
            if (!request.IsSuccessStatusCode) output.WriteLine(request.ReasonPhrase);

            Assert.True(request.IsSuccessStatusCode);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOHDDCardModel>>();

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
            try
            {
                await repo.RemoveAsync(id);
            }
            catch
            {
                output.WriteLine("test data not removed");
            }
        }
    }


    /// <summary>
    /// Test AI created
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestGetByFilter()
    {
        var id = await repo.CreateAsync(
            new HDDCreateModel(
                "Test",
                100,
                HddConnector.SATA));

        try
        {
            var request = await httpClient.GetAsync(
                "/api/hdds/by-filter?name=Test&Connector=SATA");

            Assert.True(
                request.IsSuccessStatusCode,
                $"Request failed: {request.ReasonPhrase}");

            var data = await request.Content
                .ReadFromJsonAsync<IEnumerable<RDTOHDDModel>>();

            Assert.NotNull(data);
            Assert.Contains(data, x => x.Name == "Test");
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    /// <summary>
    /// Test Created AI
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestGetByFilterWithPag()
    {
        var id = await repo.CreateAsync(
            new HDDCreateModel(
                "Test",
                100,
                HddConnector.SATA));

        try
        {
            var request = await httpClient.GetAsync(
                "/api/hdds/by-filter-pag?name=Test&Connector=SATA&page=1&pageSize=1");

            Assert.True(
                request.IsSuccessStatusCode,
                $"Request failed: {request.ReasonPhrase}");

            var data = await request.Content
                .ReadFromJsonAsync<IEnumerable<RDTOHDDModel>>();

            Assert.NotNull(data);
            Assert.Single(data);
            Assert.Equal("Test", data.First().Name);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task TestUpdate() {
        var id = await repo.CreateAsync(
        new HDDCreateModel(
            "Test",
            100,
            HddConnector.SATA));

        DTOHDDUpdateModel updateModel = new DTOHDDUpdateModel(id,"TTTT",100,ByMyPC.Models.HDDModels.HddConnectorType.SATA);
        var request = await httpClient.PutAsJsonAsync(
        "/api/hdds/",updateModel);


        Assert.True(
            request.IsSuccessStatusCode,
            $"Request failed: {request.ReasonPhrase}");

        var data = await request.Content.ReadFromJsonAsync<RDTOHDDModel>();

        Assert.NotNull(data);
        Assert.Equal("TTTT", data.Name);
    }

    [Fact]
    public async Task TestCreate()
    {

        DTOHDDCreateModel createModel = new DTOHDDCreateModel("TTTT", 100, ByMyPC.Models.HDDModels.HddConnectorType.SATA);
        var request = await httpClient.PostAsJsonAsync(
        "/api/hdds/", createModel);


        Assert.True(
            request.IsSuccessStatusCode,
            $"Request failed: {request.ReasonPhrase}");

        Guid data = await request.Content.ReadFromJsonAsync<Guid>();

        HDDDbModel? obj = await repo.GetByID(data);

        Assert.NotNull(obj);
        Assert.Equal("TTTT", obj.Name);
    }

    [Fact]
    public async Task TestDelete()
    {

        var id = await repo.CreateAsync(
  new HDDCreateModel(
      "Test",
      100,
      HddConnector.SATA));
        var request = await httpClient.DeleteAsync(
        $"/api/hdds?id={id}");


        Assert.True(
            request.IsSuccessStatusCode,
            $"Request failed: {request.ReasonPhrase}");

        HDDDbModel? obj = await repo.GetByID(id);

        Assert.Null(obj);
    }

    [Fact]
    public async Task TestCreateAndAttach() {
        
        DTOHDDCreateModel createModel = new DTOHDDCreateModel("TTTT", 100, ByMyPC.Models.HDDModels.HddConnectorType.SATA);
        var request = await httpClient.PostAsJsonAsync(
        $"/api/hdds/create-attach/?id={new Guid("590e5d08-46b0-4e08-bf0a-9c3b7f8afb65")}", createModel);

        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        output.WriteLine("Result see in db");
    }


    [Fact]
    public async Task TestAttach()
    {
        var id = await repo.CreateAsync(
new HDDCreateModel(
    "Test",
    100,
    HddConnector.SATA));
        DTOHddOperations test = new(new Guid("590e5d08-46b0-4e08-bf0a-9c3b7f8afb65"),id);
        var request = await httpClient.PutAsJsonAsync(
        $"/api/hdds/attach/",test);

        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        output.WriteLine("Result see in db");
    }


    [Fact]
    public async Task TestDeAttach()
    {
        var id = await repo.CreateAsync(
new HDDCreateModel(
    "Test",
    100,
    HddConnector.SATA));
        DTOHddOperations test = new(new Guid("590e5d08-46b0-4e08-bf0a-9c3b7f8afb65"), id);
        var request = await httpClient.PutAsJsonAsync(
        $"/api/hdds/attach/", test);

        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var requestDeattach = await httpClient.PutAsJsonAsync(
$"/api/hdds/deattach/", test);

        Assert.True(requestDeattach.IsSuccessStatusCode, requestDeattach.ReasonPhrase);



        output.WriteLine("Result see in db");
    }

}
