using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Services.PCService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PcController(IPcService service) : ControllerBase
    {
        private readonly IPcService service = service;
        #region Get Methods
        [HttpGet]
        public async Task<IActionResult> GetSmall(CancellationToken cancellationToken)
        {
            var data = await service.GetSmallAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken)
        {
            var data = await service.GetFullAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        [HttpGet("full-pag")]
        public async Task<IActionResult> GetFullPaginated([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetFullPaginatedAsync(page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetSmallWithPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetSmallPaginatedAsync(page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsync(name, cancellationToken);
            return Ok(name);
        }

        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNamePaginated([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNamePaginatedAsync(name, page, pageSize, cancellationToken);
            return Ok(name);
        }

        [HttpGet("by-filter")]
        public async Task<IActionResult> GetByFilter([FromQuery] DTOPCFilter filter,CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterAsync(filter,cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpGet("by-filter-card")]
        public async Task<IActionResult> GetByFilterPaginated([FromQuery] DTOPCFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterAsyncPaginated(filter, page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTOPCUpdateModel dto)
        {
            var item = await service.UpdateAsync(dto);
            return item is not null ? Ok(item) : NotFound();
        }

        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Create(DTOPCCreateModel dto)
        {
            var item = await service.CreateAsync(dto);
            return Ok(item);
        }
        #endregion

        #region Remove
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            var result = await service.RemoveAsync(id);
            return Ok(result);
        }
        #endregion

        #region Attach And Deattach
        [HttpPut("attach")]
        public async Task<IActionResult> AttachComponents([FromBody] DTOPCAttachOrDeattach dto)
        {
            var result = await service.AttachAsync(dto.pc,dto.Item,dto.WithAttachToPcCode);
            return Ok(result);
        }

        [HttpPut("deattach")]
        public async Task<IActionResult> DeattachComponents([FromBody] DTOPCAttachOrDeattach dto)
        {
            var result = await service.DeattachAsync(dto.pc, dto.Item, dto.WithAttachToPcCode);
            return Ok(result);
        }
        #endregion
    }
}
