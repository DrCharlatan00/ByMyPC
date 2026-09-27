using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.PSUModels.RDTO;
using ByMyPC.Services.PSUService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PSUController(IPSUService service) : ControllerBase
    {
        private readonly IPSUService service = service;

        #region Get
        [HttpGet]
        public async Task<IActionResult> GetSmall(CancellationToken cancellation) {
            IEnumerable<RDTOPSUSmallModel>? data = await service.GetSmallAsync(cancellation);
            return data is not null ?  Ok(data) : Problem(detail: "Data is null");
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUModel>? data = await service.GetFullAsync(cancellation);
            return data is not null ? Ok(data) : Problem(detail: "Data is null");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) {
            RDTOPSUModel? item = await service.GetByID(id);
            return item is not null ? Ok(item) : NotFound();
        }


        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name,CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUSmallModel>? data = await service.SearchByName(name,cancellation);
            return data is not null ? Ok(data) : Problem(detail: "Data is null");
        }

        [HttpGet("card-pag")]
        public async Task<IActionResult> GetSmallWithPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUSmallModel>? data = await service.GetSmallWithPag(page, pageSize, cancellation);
            return Ok(data);
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTOPSUModelUpdate model) {
            RDTOPSUModel? data = await service.UpdateAsync(model);
            return data is not null?  Ok(data) : NotFound();
        }


        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOPSUModelCreate model)
        {
            Guid id = await service.CreateAsync(model);
            return Ok(id);
        }
        #endregion

        #region Remove
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remove(Guid id) {
            bool result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : NotFound();
        }
        #endregion
    }
}
