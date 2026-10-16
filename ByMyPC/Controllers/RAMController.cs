using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Services.RamService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StackExchange.Redis;
using System.Runtime.InteropServices;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RAMController(IRamService service) : ControllerBase
    {
        private readonly IRamService service = service;

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
        public async Task<IActionResult> GetFullWithPagination([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetFullWithPagination(page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetSmallWithPagination([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetSmallWithPagination(page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsyncFull(name, cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNameWithPagination([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsyncSmallPaginated(name, page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("by-filter")]
        public async Task<IActionResult> GetFullByFilter([FromQuery] DTORAMFilter filter, CancellationToken cancellationToken)
        {
            var data = await service.GetFullByFilterAsync(filter, cancellationToken);
            return Ok(data);
        }


        [HttpGet("by-filter-card")]
        public async Task<IActionResult> GetSmallByFilterWithPagination([FromQuery] DTORAMFilter filter, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetSmallByFilterAsyncPaginated(filter, page, pageSize, cancellationToken);
            return Ok(data);
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTORAMUpdate dto)
        {
            var result = await service.UpdateAsync(dto);
            return result is not null ? Ok(result) : NotFound();
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTORAMCreate dto)
        {
            var result = await service.CreateAsync(dto);
            return Ok(result);
        }
        #endregion

        #region Remove
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            var result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : NotFound();
        }
        #endregion

        #region Other Operation
        [HttpPut("Attach")]
        public async Task<IActionResult> Attach([FromQuery] Guid idPc, [FromQuery] Guid idRam)
        {
            var result = await service.AttachAsync(idPc, idRam);
            return result == true ? Ok(result) : NotFound();
        }

        [HttpPut("DeAttach")]
        public async Task<IActionResult> DeAttach([FromQuery] Guid idPc, [FromQuery] Guid idRam)
        {
            var result = await service.DeAttachAsync(idPc, idRam);
            return result == true ? Ok(result) : NotFound();
        }
        #endregion
    }
}
