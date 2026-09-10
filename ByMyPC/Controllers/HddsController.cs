using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Services.HDDService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HddsController(IHDDService service) : ControllerBase
    {
        private readonly IHDDService service = service;

        #region Get
        [HttpGet]
        public async Task<IActionResult> GetCard(CancellationToken cancellationToken) {
            var data = await service.GetCardModelsAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect is null");
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken)
        {
            var data = await service.GetModelsAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect is null");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetCardWithPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await service.GetCardWithPagination(page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken) {
            var data = await service.SearchByName(name, cancellationToken);
            return Ok(data);
        }

        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNameWithPag([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameWithPag(name, page, pageSize, cancellationToken);
            return Ok(data);
        }

        [HttpGet("by-filter")]
        public async Task<IActionResult> GetByFilter(DTOHDDFilter filter, CancellationToken cancellationToken) {
            var data = await service.GetByFilter(filter,cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpGet("by-filter-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboardCard>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFiterPagination([FromQuery] DTOHDDFilter filter, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetCardByFilterWithPag(filter,page,pageSize, cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTOHDDUpdateModel model) {
            var result = await service.UpdateAsync(model);
            return result is not null ? Ok(result) : BadRequest();
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOHDDCreateModel model) {
            var result = await service.CreateAsync(model);
            return Ok(result);
        }

        [HttpPost("create-attach")]
        public async Task<IActionResult> CreateAndAttach([FromBody] DTOHDDCreateModel model,[FromQuery] Guid id)
        {
            var result = await service.CreateAndAttachAsync(model,id);
            return Ok(result);
        }
        #endregion

        #region Remove
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] Guid id) {
            await service.RemoveAsync(id);
            return Ok();
        }
        #endregion

        [HttpPut("attach")]
        public async Task<IActionResult> Attach([FromQuery] Guid idHdd, [FromQuery] Guid IdPc) {
            bool result = await service.AttachHdd(IdPc, idHdd);
            return result == true ? Ok(result) : Problem();
        }


        [HttpPut("deattach")]
        public async Task<IActionResult> Deattach([FromQuery] Guid idHdd, [FromQuery] Guid IdPc)
        {
            bool result = await service.DeattachHdd(IdPc, idHdd);
            return result == true ? Ok(result) : Problem();
        }
    }
}
