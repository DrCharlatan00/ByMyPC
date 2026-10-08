using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Services.GPUService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPUController(IGPUService service) : ControllerBase
    {
        private readonly IGPUService service = service;
        #region Get
        [HttpGet]
        public async Task<IActionResult> GetSmall(CancellationToken cancellationToken)
        {
            var data = await service.GetSmallModelsAsync(cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellation)
        {
            var data = await service.GetFullModelsAsync(cancellation);
            return data != null ? Ok(data) : NotFound();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var item = await service.GetByIDAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetWithPag([FromQuery] int page,[FromQuery] int pageSize, CancellationToken cancellation)
        {
            var data = await service.GetSmallWithPag(page, pageSize, cancellation);
            return Ok(data);
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery]string name, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsync(name, cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNameWithPag([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsyncWithPag(name, page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("by-filter")]
        public async Task<IActionResult> GetByFilter([FromQuery] DTOGPUFilter filter,CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterAsync(filter,cancellationToken);
            return Ok(data);
        }


        [HttpGet("by-filter-pag")]
        public async Task<IActionResult> GetByFilterWithPag([FromQuery] DTOGPUFilter filter, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterWithPagAsync(filter, page,pageSize,cancellationToken);
            return Ok(data);
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]DTOGPUUpdateModel model) 
        {
            var result = await service.UpdateAsync(model);
            return result is not null ? Ok(result) : BadRequest();
        }

        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOGPUCreateModel model)
        {
            var result  = await service.CreateAsync(model);
            return Ok(result);
        }
        #endregion


        #region Remove
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery]Guid id)
        {
            var result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : BadRequest();
        }
        #endregion

    }
}
