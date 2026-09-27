using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.PSUModels.RDTO;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net.Http.Json;
using System.Net.WebSockets;
using Xunit.Abstractions;

namespace ByMyPc.IntegrationTesting;

public class TestPSUCRUD : IClassFixture<TestWebApplicationFactory>
{
    private readonly ITestOutputHelper output;
    IPSURepo repo;

    private readonly HttpClient client;

    public TestPSUCRUD(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        var scope = factory.Services.CreateScope();
        repo = scope.ServiceProvider.GetRequiredService<IPSURepo>();
        client = factory.CreateClient();
        this.output = output;
    }
    [Fact]
    public async Task TestGetSmallAll()
    {
        var request = await client.GetAsync("api/psu");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOPSUSmallModel>>();

        Assert.NotNull(data);
    }

    [Fact]
    public async Task TestGetFullAll()
    {
        var request = await client.GetAsync("api/psu/full");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOPSUModel>>();

        Assert.NotNull(data);
    }

    [Fact]
    public async Task TestGetWithID()
    {
        Guid id = Guid.Empty;
        try
        {
            PSUCreateModel model = new PSUCreateModel
            {
                Name = "Test",
                IsLive = true,
                IsModular = true,
                IsCertified = true,
                PowerWatt = 450,
                Size = PSU_SIZE.ATX,
            };
            id = await repo.CreateAsync(model);


        }
        catch
        {
            Assert.Fail("Not created Test Model");
        }

        try
        {
            var request = await client.GetAsync($"api/psu/{id}");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<RDTOPSUModel>();

            Assert.NotNull(data);
            Assert.Equal("Test", data.Name);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }


    }

    [Fact]
    public async Task TestGetWithNameSearch()
    {
        Guid id = Guid.Empty;
        try
        {
            PSUCreateModel model = new PSUCreateModel
            {
                Name = "Test",
                IsLive = true,
                IsModular = true,
                IsCertified = true,
                PowerWatt = 450,
                Size = PSU_SIZE.ATX,
            };
            id = await repo.CreateAsync(model);


        }
        catch
        {
            Assert.Fail("Not created Test Model");
        }
        try
        {
            var request = await client.GetAsync($"api/psu/search-name?name=Test");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOPSUSmallModel>>();

            Assert.NotNull(data);
            Assert.Equal("Test", data.First().Name);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }

    }


    [Fact]
    public async Task TestGetWithPag() {
        try
        {
            var request = await client.GetAsync($"api/psu/card-pag?page=1&pageSize=1");
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<IEnumerable<RDTOPSUSmallModel>>();

            Assert.NotNull(data);
            Assert.Single(data);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Fact]
    public async Task TestUpdate() {
        Guid id = Guid.Empty;
        try
        {
            PSUCreateModel model = new PSUCreateModel
            {
                Name = "Test",
                IsLive = true,
                IsModular = true,
                IsCertified = true,
                PowerWatt = 450,
                Size = PSU_SIZE.ATX,
            };
            id = await repo.CreateAsync(model);


        }
        catch
        {
            Assert.Fail("Not created Test Model");
        }

        try {
            DTOPSUModelUpdate update = new DTOPSUModelUpdate(
                id,
                "TTTT",
                null,
                false,
                null,
                null,
                null
                );

            var request = await client.PutAsJsonAsync($"api/psu/",update);
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<RDTOPSUModel>();
            
            Assert.NotNull(data);
            Assert.Equal(update.Name, data.Name);
        }

        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            await repo.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task TestCreate() {
        try
        {
            DTOPSUModelCreate create = new DTOPSUModelCreate(
                "TTTT",
                0,
                false,
                PSU_SIZE.TFX,
                false,
                false
                );

            var request = await client.PostAsJsonAsync($"api/psu/", create);
            Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

            var data = await request.Content.ReadFromJsonAsync<Guid>();
            var item = await repo.GetByIdAsync(data);
            Assert.NotNull(item);
            Assert.Equal(create.Name, item.Name);
        }

        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Fact]
    public async Task TestDelete() {
        Guid id = Guid.Empty;
        try
        {
            PSUCreateModel model = new PSUCreateModel
            {
                Name = "Test",
                IsLive = true,
                IsModular = true,
                IsCertified = true,
                PowerWatt = 450,
                Size = PSU_SIZE.ATX,
            };
            id = await repo.CreateAsync(model);


        }
        catch
        {
            Assert.Fail("Not created Test Model");
        }
        var request = await client.DeleteAsync($"api/psu/{id}");
        Assert.True(request.IsSuccessStatusCode, request.ReasonPhrase);

        Assert.Null(await repo.GetByIdAsync(id));

    }
}
