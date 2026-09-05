using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Services.CpuService;
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


        /// <summary>
        /// Get Card collection Motherboard
        /// </summary>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Card collection Motherboard</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboardCard>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCard(CancellationToken cancellationToken) {
            var data = await service.GetCardMotherboardAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect return null data");
        }

        /// <summary>
        /// Get full collection Motherboard
        /// </summary>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>full collection Motherboard</returns>
        /// <remarks>Do not use if you do not want to get all the information accurately, otherwise you will create unnecessary overload</remarks>
        [HttpGet("full")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboard>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken) {
            var data = await service.GetFullMotherboardAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect return null data");
        }

        /// <summary>
        /// Get by id full information for Motherboard 
        /// </summary>
        /// <param name="id">Id Db Motherboard</param>
        /// <returns>full information for Motherboard </returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RDTOModelMotherboard))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id) {
            RDTOModelMotherboard? data = await service.GetByIdAsync(id);
            return data is not null ? Ok(data) : NotFound();
        }

        /// <summary>
        /// Get Card collection motherboard with pagination
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count items</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Card collection motherboard</returns>
        [HttpGet("card-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboardCard>))]
        public async Task<IActionResult> GetCardWithPag([FromQuery] int page,[FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var data = await service.GetCardWithPaginationAsync(page,pageSize,cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Search in db Motherboard's with name = You_Name
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Collection card motherboards</returns>
        [HttpGet("search-name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboardCard>))]

        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken) {
            var data = await service.SearchByNameAsync(name,cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Search in db Motherboard's with name = You_Name and pagination
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count items</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Collection card motherboards</returns>
        [HttpGet("search-name-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboardCard>))]

        public async Task<IActionResult> SearchByNameWithPag([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameWithPaginationAsync(name,page,pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Get collecttion full motherboard's info by filters
        /// </summary>
        /// <param name="dto">You can filter by name, live, socket, and the presence of integrated graphics.</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>collecttion full motherboard's info by filters</returns>
        [HttpGet("by-filter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboard>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFilter([FromQuery] DTOMotherboardFilter dto, CancellationToken cancellationToken) {
            IEnumerable<RDTOModelMotherboard>? data = await service.GetByFilterAsync(dto,cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }


        /// <summary>
        /// Get collecttion card motherboard's info by filters
        /// </summary>
        /// <param name="filter">You can filter by name, live, socket, and the presence of integrated graphics.</param>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count items</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>collecttion card motherboard's info by filters</returns>
        [HttpGet("by-filter-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOModelMotherboardCard>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByFiterPaginationg([FromQuery] DTOMotherboardFilter filter, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterWithPagAsync(filter, page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }


        /// <summary>
        /// Put method for update motherboard
        /// </summary>
        /// <param name="model">Model new infomation for motherboard</param>
        /// <returns>Updated Motherboard</returns>
        /// <remarks>Do not fotgot hand over id in model </remarks>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RDTOModelMotherboard))]
        public async Task<IActionResult> Update([FromBody] DTOMotherboardUpdateModel model) {
            RDTOModelMotherboard? result = await service.UpdateAsync(model);
            return result is not null ? Ok(result) : Problem(detail: "Unexpect you model not update, check data in model");
        }

        /// <summary>
        /// Post method for create new model
        /// </summary>
        /// <param name="model">New motherboard</param>
        /// <returns>Guid new model</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> Create([FromBody] DTOMotherboardCreateModel model)
        {
            Guid id = await service.CreateAsync(model);
            return Ok(id);    
        }

        /// <summary>
        /// Delete method for remove motherboard in db
        /// </summary>
        /// <param name="id">Guid motherboard which you want to delete</param>
        /// <returns>Ok)</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove([FromQuery]Guid id) {
            await service.RemoveAsync(id);
            return Ok();
        }


    }
}
