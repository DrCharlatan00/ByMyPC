
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Models.PCModels.RDTO;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ByMyPc.IntegrationTesting;

public class TestPcCRUD : IClassFixture<TestWebApplicationFactory>, IDisposable
{

    //AI Test
    private readonly HttpClient client;
    private readonly IServiceScope scope;
    private readonly IPcRepo repo;
    private readonly ICpuRepo cpuRepo;
    private readonly IGPURepo gpuRepo;
    private readonly IPSURepo psuRepo;
    private readonly IMotherboardRepo mbRepo;

    public TestPcCRUD(TestWebApplicationFactory factory)
    {
        scope = factory.Services.CreateScope();
        repo = scope.ServiceProvider.GetRequiredService<IPcRepo>();
        cpuRepo = scope.ServiceProvider.GetRequiredService<ICpuRepo>();
        gpuRepo = scope.ServiceProvider.GetRequiredService<IGPURepo>();
        psuRepo = scope.ServiceProvider.GetRequiredService<IPSURepo>();
        mbRepo  = scope.ServiceProvider.GetRequiredService<IMotherboardRepo>();
        client = factory.CreateClient();
    }

    #region Ai Get Tests

    private async Task<Guid> CreateTestPcAsync(string? name = null)
    {
        name ??= $"IntegrationTest-{Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync(
            "api/pc",
            new DTOPCCreateModel(name));

        response.EnsureSuccessStatusCode();

        var id = await response.Content.ReadFromJsonAsync<Guid>();

        Assert.NotEqual(Guid.Empty, id);

        return id;
    }

    private async Task<JsonDocument> GetJsonAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    [Fact]
    public async Task GetSmall_ReturnsOk()
    {
        using var response = await client.GetAsync("api/pc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await GetJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetFull_ReturnsOk()
    {
        using var response = await client.GetAsync("api/pc/full");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await GetJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetById_ReturnsCreatedPc()
    {
        var id = await CreateTestPcAsync();

        try
        {
            using var response = await client.GetAsync($"api/pc/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await GetJsonAsync(response);
            Assert.Equal(id, json.RootElement.GetProperty("id").GetGuid());
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task GetById_WithUnknownId_ReturnsNotFound()
    {
        using var response = await client.GetAsync($"api/pc/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFullPaginated_ReturnsOneItem()
    {
        using var response = await client.GetAsync(
            "api/pc/full-pag?page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await GetJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        Assert.True(json.RootElement.GetArrayLength() <= 1);
    }

    [Fact]
    public async Task GetSmallPaginated_ReturnsOneItem()
    {
        using var response = await client.GetAsync(
            "api/pc/card-pag?page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await GetJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        Assert.True(json.RootElement.GetArrayLength() <= 1);
    }

    [Fact]
    public async Task SearchByName_ReturnsMatchingPc()
    {
        var name = $"SearchTest-{Guid.NewGuid():N}";
        var id = await CreateTestPcAsync(name);

        try
        {
            using var response = await client.GetAsync(
                $"api/pc/search-name?name={Uri.EscapeDataString(name)}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await GetJsonAsync(response);

            Assert.Contains(
                json.RootElement.EnumerateArray(),
                item => item.GetProperty("name").GetString() == name);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task SearchByNamePaginated_ReturnsMatchingPc()
    {
        var name = $"SearchPageTest-{Guid.NewGuid():N}";
        var id = await CreateTestPcAsync(name);

        try
        {
            using var response = await client.GetAsync(
                $"api/pc/search-name-pag?name={Uri.EscapeDataString(name)}&page=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await GetJsonAsync(response);

            Assert.Contains(
                json.RootElement.EnumerateArray(),
                item => item.GetProperty("name").GetString() == name);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task GetByFilter_ReturnsMatchingPc()
    {
        var name = $"FilterTest-{Guid.NewGuid():N}";
        var id = await CreateTestPcAsync(name);

        try
        {
            using var response = await client.GetAsync(
                $"api/pc/by-filter?name={Uri.EscapeDataString(name)}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await GetJsonAsync(response);

            Assert.Contains(
                json.RootElement.EnumerateArray(),
                item => item.GetProperty("name").GetString() == name);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task GetByFilterPaginated_ReturnsMatchingPc()
    {
        var name = $"FilterPageTest-{Guid.NewGuid():N}";
        var id = await CreateTestPcAsync(name);

        try
        {
            using var response = await client.GetAsync(
                $"api/pc/by-filter-card?name={Uri.EscapeDataString(name)}&page=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await GetJsonAsync(response);

            Assert.Contains(
                json.RootElement.EnumerateArray(),
                item => item.GetProperty("name").GetString() == name);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task Create_ReturnsNewId()
    {
        var name = $"CreateTest-{Guid.NewGuid():N}";
        Guid id = Guid.Empty;

        try
        {
            using var response = await client.PostAsJsonAsync(
                "api/pc",
                new DTOPCCreateModel(name));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            id = await response.Content.ReadFromJsonAsync<Guid>();

            Assert.NotEqual(Guid.Empty, id);

            var item = await repo.GetByID(id);

            Assert.NotNull(item);
            Assert.Equal(name, item.Name);
        }
        finally
        {
            if (id != Guid.Empty)
                await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task Update_ChangesPcName()
    {
        var id = await CreateTestPcAsync();
        var newName = $"Updated-{Guid.NewGuid():N}";

        try
        {
            using var response = await client.PutAsJsonAsync(
                "api/pc",
                new DTOPCUpdateModel(id, newName));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var json = await GetJsonAsync(response);

            Assert.Equal(
                newName,
                json.RootElement.GetProperty("name").GetString());

            var item = await repo.GetByID(id);

            Assert.NotNull(item);
            Assert.Equal(newName, item.Name);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }


    [Fact]
    public async Task Delete_RemovesPc()
    {
        var id = await CreateTestPcAsync();

        using var response = await client.DeleteAsync($"api/pc?id={id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<bool>();

        Assert.True(result);
        Assert.Null(await repo.GetByID(id));
    }

    [Fact]
    public async Task Delete_WithUnknownId_ReturnsFalse()
    {
        using var response = await client.DeleteAsync(
            $"api/pc?id={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<bool>();

        Assert.False(result);
    }

    #endregion


    [Fact]
    public async Task TestAttachCpu()
    {
        Guid cpuid = Guid.Empty;
        Guid pcId = Guid.Empty;
        try
        {
            cpuid =  await cpuRepo.CreateAsync(new Postgresql.CRUDModel.Operation.CpuCreateModel(
                "TestAttach",
                "LGA775"
                  ));
            pcId = await repo.CreateAsync(new Postgresql.CRUDModel.Operation.PcCreateModel("testAttach"));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        try
        {
            DTOPCAttachOrDeattach dto = new DTOPCAttachOrDeattach(
        pc: pcId,
        cpuid,
        WithAttachToPcCode: WithAttachOrDeattachToPc.CPU
    );
            using var response = await client.PutAsJsonAsync("api/pc/attach", dto);
            Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);

            PcDbModel? item = await repo.GetByID(pcId);
            Assert.NotNull(item);
            if (item.CpuId != cpuid) Assert.Fail("Not Attached CPU");
        }
        finally
        {
            await repo.RemoveAsync(pcId);
            await cpuRepo.RemoveAsync(cpuid);
        }


    }

    [Fact]
    public async Task TestAttachGpu()
    {
        Guid gpuid = Guid.Empty;
        Guid pcId = Guid.Empty;
        try
        {
            gpuid = await gpuRepo.CreateAsync(new Postgresql.CRUDModel.Operation.GPUCreateModel(
                "TestAttach",
                 1888,
                 VideoSlots.UNKNOWN,
                 0,
                 "2",
                 "321"
                  ));
            pcId = await repo.CreateAsync(new Postgresql.CRUDModel.Operation.PcCreateModel("testAttach"));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        try
        {
            DTOPCAttachOrDeattach dto = new DTOPCAttachOrDeattach(
        pc: pcId,
        gpuid,
        WithAttachToPcCode: WithAttachOrDeattachToPc.GPU
    );
            using var response = await client.PutAsJsonAsync("api/pc/attach", dto);
            Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);

            PcDbModel? item = await repo.GetByID(pcId);
            Assert.NotNull(item);
            if (item.GpuId != gpuid) Assert.Fail("Not Attached GPU");
        }
        finally
        {
            await repo.RemoveAsync(pcId);
            await gpuRepo.RemoveAsync(gpuid);
        }


    }

    [Fact (DisplayName = "Test Attach Motherboard")]
    public async Task TestAttachMB()
    {
        Guid mbid = Guid.Empty;
        Guid pcId = Guid.Empty;
        try
        {
            mbid = await mbRepo.CreateAsync(new Postgresql.CRUDModel.Operation.MotherboardCreateModel(
                    "OlegNostalgy",
                    "LGA775",
                    1,
                    533,
                    2200,
                    false,
                    false,
                    VideoSlots.AGP
                  ));
            pcId = await repo.CreateAsync(new Postgresql.CRUDModel.Operation.PcCreateModel("testAttach"));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        try
        {
            DTOPCAttachOrDeattach dto = new DTOPCAttachOrDeattach(
        pc: pcId,
        mbid,
        WithAttachToPcCode: WithAttachOrDeattachToPc.MOTHERBOARD
    );
            using var response = await client.PutAsJsonAsync("api/pc/attach", dto);
            Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);

            PcDbModel? item = await repo.GetByID(pcId);
            Assert.NotNull(item);
            if (item.MotherboardId != mbid) Assert.Fail("Not Attached Motherboard");
        }
        finally
        {
            await repo.RemoveAsync(pcId);
            await mbRepo.RemoveAsync(mbid);
        }


    }

    [Fact]
    public async Task TestAttachPSU()
    {
        Guid psuid = Guid.Empty;
        Guid pcId = Guid.Empty;
        try
        {
            psuid = await psuRepo.CreateAsync(new Postgresql.CRUDModel.Operation.PSUCreateModel(
                    "Aerocool vx",
                    250,
                    false,
                    PSU_SIZE.ATX,
                    false,
                    false
                  ));
            pcId = await repo.CreateAsync(new Postgresql.CRUDModel.Operation.PcCreateModel("testAttach"));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        try
        {
            DTOPCAttachOrDeattach dto = new DTOPCAttachOrDeattach(
        pc: pcId,
        psuid,
        WithAttachToPcCode: WithAttachOrDeattachToPc.PSU
    );
            using var response = await client.PutAsJsonAsync("api/pc/attach", dto);
            Assert.True(response.IsSuccessStatusCode, response.ReasonPhrase);

            PcDbModel? item = await repo.GetByID(pcId);
            Assert.NotNull(item);
            if (item.PSUId != psuid) Assert.Fail("Not Attached PSU");
        }
        finally
        {
            await repo.RemoveAsync(pcId);
            await psuRepo.RemoveAsync(psuid);
        }


    }

    public void Dispose()
    {
        client.Dispose();
        scope.Dispose();
    }
}
