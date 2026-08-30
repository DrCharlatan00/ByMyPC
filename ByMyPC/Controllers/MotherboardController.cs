using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Services.MotherboardService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotherboardController(IMotherboardService service) : ControllerBase
    {
        private readonly IMotherboardService service = service;

        [HttpGet]
        public async Task<IActionResult> GetCard(CancellationToken cancellationToken) {
            var data = await service.GetCardMotherboardAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect return null data");
        }

        [HttpGet("Full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken) {
            var data = await service.GetFullMotherboardAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect return null data");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) {
            RDTOModelMotherboard? data = await service.GetByIdAsync(id);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetCardWithPag(int page,int pageSize,CancellationToken cancellationToken)
        {
            var data = await service.GetCardWithPaginationAsync(page,pageSize,cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName(string name, CancellationToken cancellationToken) {
            var data = await service.SearchByNameAsync(name,cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameWithPaginationAsync(name,page,pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> Update(DTOMotherboardUpdateModel model) {
            RDTOModelMotherboard? result = await service.UpdateAsync(model);
            return result is not null ? Ok(result) : Problem(detail: "Unexpect you model not update, check data in model");
        }

        [HttpPost]
        public async Task<IActionResult> Create(DTOMotherboardCreateModel model)
        {
            return Ok(service.CreateAsync(model));    
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(Guid id) {
            await service.RemoveAsync(id);
            return Ok();
        }


    }
}
