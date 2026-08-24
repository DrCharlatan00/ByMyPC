using Microsoft.AspNetCore.Mvc;
using ByMyPC.Services.CpuService;
using Microsoft.AspNetCore.Components;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using ByMyPC.Models.CpuModels.DTO;


namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPUController(ICpuService cpuService) : ControllerBase
    {
        private readonly ICpuService cpuService = cpuService;


        [HttpGet]
        public async Task<IActionResult> GetCardInfo(CancellationToken cancellationToken) {
            var data = await cpuService.GetRDTOSmallModelAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.");
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullInfo(CancellationToken cancellationToken) {
            var data = await cpuService.GetFullCpuAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.Possible full collection offline now");
        }

        [HttpGet("full-pag")]
        public async Task<IActionResult> GetFullPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await cpuService.GetFullCpuPagination(page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) {
            var data = await cpuService.GetById(id);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetCardPagination([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await cpuService.GetSmallModelsWithPaginationAsync(page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.");
        }

        [HttpGet("seach-name")]
        public async Task<IActionResult> SeachByName([FromQuery] string name, CancellationToken cancellationToken) {
            var data = await cpuService.SearchByNameAsync(name, cancellationToken);
            return Ok(data);
        }


        [HttpGet("seach-name-pag")]
        public async Task<IActionResult> SeachByNameCardPagination([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await cpuService.SearchByNameWithPaginationAsync(name,page,pageSize,cancellationToken);
            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTOCpuUpdateModel model) {
            var result = await cpuService.UpdateAsync(model);
            return result is null ? Ok(result) : Problem(detail: "Data unexpect not updated");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOCpuCreateModel model) {
            var result = await cpuService.CreateAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) {
            await cpuService.RemoveAsync(id);
            return Ok();
        }
        

    }
}
