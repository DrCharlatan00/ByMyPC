using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ByMyPc.IntegrationTesting;

public class InterationTestsMotherboard : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient httpClient;
    private readonly ITestOutputHelper output;
    private readonly IMotherboardRepo repo;

    public InterationTestsMotherboard(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        var scope = factory.Services.CreateScope();
        repo = scope.ServiceProvider.GetRequiredService<IMotherboardRepo>();
        httpClient = factory.CreateClient();
        this.output = output;
    }

    [Fact]
    public async Task TestGetCard()
    {
        var result = await httpClient.GetAsync("/api/motherboard/");

        if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

        Assert.True(result.IsSuccessStatusCode);

        var data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboardCard>>();

        Assert.NotNull(data);
                
    }

    [Fact]
    public async Task TestFullGet() {
        var result = await httpClient.GetAsync("/api/motherboard/full");

        if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

        Assert.True(result.IsSuccessStatusCode);

        IEnumerable<RDTOModelMotherboard>? data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboard>>();

        Assert.NotNull(data);
    }

    [Fact]
    public async Task TestPag()
    {
        var result = await httpClient.GetAsync("/api/motherboard/card-pag?page=1&pageSize=1");

        if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

        Assert.True(result.IsSuccessStatusCode);

        IEnumerable<RDTOModelMotherboardCard>? data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboardCard>>();

        Assert.NotNull(data);

        Assert.Single(data);
    }

    [Fact]
    public async Task TestGetByID() {
        Guid guid = Guid.Empty;
        try
        {
            MotherboardCreateModel testModel = new(
                name: "Test",
                socket: "Test",
                0,
                0,
                0,
                false,
                false,
                default
                );
            guid = await repo.CreateAsync(testModel);
            if (guid == Guid.Empty) Assert.Fail("Id test model not get");

        }
        catch (Exception ex) {
            Assert.Fail("Can't create new Motherboard \nException: " + ex.Message);
        }

        try
        {
            var result = await httpClient.GetAsync($"/api/motherboard/{guid}");

            if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

            Assert.True(result.IsSuccessStatusCode);

            RDTOModelMotherboardCard? data = await result.Content.ReadFromJsonAsync<RDTOModelMotherboardCard>();

            Assert.NotNull(data);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally {
            try
            {
                await repo.RemoveAsync(guid);
            }
            catch {
                output.WriteLine("Test data not removed");
            }
        }
    }


    [Fact]
    public async Task TestSearchGet() {
        Guid guid = Guid.Empty;
        try
        {
            MotherboardCreateModel testModel = new(
                name: "Test",
                socket: "Test",
                0,
                0,
                0,
                false,
                false,
                default
                );
            guid = await repo.CreateAsync(testModel);
            if (guid == Guid.Empty) Assert.Fail("Id test model not get");

        }
        catch (Exception ex)
        {
            Assert.Fail("Can't create new Motherboard \nException: " + ex.Message);
        }

        try {
            var result = await httpClient.GetAsync($"/api/motherboard/search-name?name=Test");

            if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<RDTOModelMotherboardCard>? data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboardCard>>();

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
                await repo.RemoveAsync(guid);
            }
            catch
            {
                output.WriteLine("Test data not removed");
            }
        }

    }


    [Fact]
    public async Task TestSearchGetPag()
    {
        Guid guid = Guid.Empty;
        try
        {
            MotherboardCreateModel testModel = new(
                name: "Test",
                socket: "Test",
                0,
                0,
                0,
                false,
                false,
                default
                );
            guid = await repo.CreateAsync(testModel);
            if (guid == Guid.Empty) Assert.Fail("Id test model not get");

        }
        catch (Exception ex)
        {
            Assert.Fail("Can't create new Motherboard \nException: " + ex.Message);
        }

        try
        {
            var result = await httpClient.GetAsync("/api/motherboard/search-name-pag?name=Test&page=1&pageSize=1");

            if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<RDTOModelMotherboardCard>? data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboardCard>>();

            Assert.NotNull(data);
            Assert.Equal("Test", data.First().Name);
            Assert.Single(data);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            try
            {
                await repo.RemoveAsync(guid);
            }
            catch
            {
                output.WriteLine("Test data not removed");
            }
        }
    }


    [Fact]
    public async Task TestFilters()
    {
        DTOMotherboardFilter filter = new DTOMotherboardFilter
        {
            ByHaveIntegratedGPU = false,
            ByLive = false,
            ByName = null,
            BySocket = null
        };

        var result = await httpClient.GetAsync($"/api/motherboard/by-filter?ByName={filter.ByName}&ByLive={filter.ByLive}&BySocket={filter.BySocket}&ByHaveIntegratedGPU={filter.ByHaveIntegratedGPU}");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);

        IEnumerable<RDTOModelMotherboard>? data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboard>>();
        Assert.NotNull(data);
        Assert.False(data.First().IsLive);
        //foreach (var item in data)
        //{
        //    output.WriteLine($"Item {item.Name}  {item.IsLive}");
        //}

    }

    [Fact]
    public async Task TestFiltersWithPagination()
    {
        DTOMotherboardFilter filter = new DTOMotherboardFilter
        {
            ByHaveIntegratedGPU = false,
            ByLive = false,
            ByName = null,
            BySocket = null
        };

        var result = await httpClient.GetAsync($"/api/cpu/by-filter-pag?ByName={filter.ByName}&ByLive={filter.ByLive}&BySocket={filter.BySocket}&ByHaveIntegratedGPU={filter.ByHaveIntegratedGPU}&page=1&pageSize=1");
        if (!result.IsSuccessStatusCode) output.WriteLine(result.ReasonPhrase);

        Assert.True(result.IsSuccessStatusCode);

        IEnumerable<RDTOModelMotherboardCard>? data = await result.Content.ReadFromJsonAsync<IEnumerable<RDTOModelMotherboardCard>>();
        Assert.NotNull(data);
        Assert.False(data.First().IsLive);
        //foreach (var item in data)
        //{
        //    output.WriteLine($"Item {item.Name}  {item.IsLive}");
        //}


    }





    [Fact]
    public async Task TestUpdateData() {
        Guid guid = Guid.Empty;
        try
        {
            MotherboardCreateModel testModel = new(
                name: "Test",
                socket: "Test",
                0,
                0,
                0,
                false,
                false,
                default
                );
            guid = await repo.CreateAsync(testModel);
            if (guid == Guid.Empty) Assert.Fail("Id test model not get");

        }
        catch (Exception ex)
        {
            Assert.Fail("Can't create new Motherboard \nException: " + ex.Message);
        }

        try
        {
            DTOMotherboardUpdateModel testUpdatedModel = new(
                guid,
                "TTTT",
                "TTTT",
                false
                );
            var result = await httpClient.PutAsJsonAsync("/api/motherboard/", testUpdatedModel);

            if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

            Assert.True(result.IsSuccessStatusCode);

            RDTOModelMotherboard? data = await result.Content.ReadFromJsonAsync<RDTOModelMotherboard>();

            Assert.NotNull(data);

            Assert.Equal("TTTT", data.Name);
            Assert.Equal("TTTT", data.Socket);
            Assert.False(data.IsLive);


        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally {
            try
            {
                await repo.RemoveAsync(guid);
            }
            catch
            {
                output.WriteLine("Test data not removed");
            }
        }
    }

    [Fact]
    public async Task CreateTest()
    {
        DTOMotherboardCreateModel motherboardCreateModel = new(
         "Test",
         "Test",
        0,
        0,
        0,
        false,
        false,
        default
        );

        var result = await httpClient.PostAsJsonAsync("/api/motherboard/",motherboardCreateModel);

        if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

        Assert.True(result.IsSuccessStatusCode);
        Guid data = await result.Content.ReadFromJsonAsync<Guid>();

        MotherboardDbModel? item = await repo.GetByIDAsync(data);

        Assert.NotNull(item);

        Assert.Equal("Test",item.Name);

    }

    [Fact]
    public async Task DeleteTest()
    {
        Guid guid = Guid.Empty;
        try
        {
            MotherboardCreateModel testModel = new(
                name: "Test",
                socket: "Test",
                0,
                0,
                0,
                false,
                false,
                default
                );
            guid = await repo.CreateAsync(testModel);
            if (guid == Guid.Empty) Assert.Fail("Id test model not get");
        }
        catch (Exception ex)
        {
            Assert.Fail("Can't create new Motherboard \nException: " + ex.Message);
        }

        var result = await httpClient.DeleteAsync($"api/motherboard/?id={guid}");

        if (!result.IsSuccessStatusCode) { output.WriteLine(result.ReasonPhrase); }

        Assert.True(result.IsSuccessStatusCode);


    }

}


