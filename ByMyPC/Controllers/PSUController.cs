using ByMyPC.Models.PSUModels;
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
        /// <summary>
        /// Get small model PSU
        /// </summary>
        /// <param name="cancellation">Default param</param>
        /// <returns>Small Collection PSU </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOPSUSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSmall(CancellationToken cancellation) {
            IEnumerable<RDTOPSUSmallModel>? data = await service.GetSmallAsync(cancellation);
            return data is not null ?  Ok(data) : Problem(detail: "Data is null");
        }


        /// <summary>
        /// Get Full info for PSU
        /// </summary>
        /// <param name="cancellation">Default param</param>
        /// <returns>Full information PSU Collection</returns>
        /// <remarks>Don't just request it arbitrarily—it involves a massive amount of data; it is better to use pagination methods if you want the full information.</remarks>
        [HttpGet("full")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOPSUModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFull(CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUModel>? data = await service.GetFullAsync(cancellation);
            return data is not null ? Ok(data) : Problem(detail: "Data is null");
        }


        /// <summary>
        /// Get Item PSU With ID
        /// </summary>
        /// <param name="id">ID of the object you want to retrieve</param>
        /// <returns>Full information of PSU item</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RDTOPSUModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id) {
            RDTOPSUModel? item = await service.GetByID(id);
            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Legacy search by name. Returns small PSU models only.
        /// </summary>
        /// <param name="name">Part of the name to search for</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A collection of matching small PSU models.</returns>
        [HttpGet("search-name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOPSUSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LegacySearchByName([FromQuery] string name,CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUSmallModel>? data = await service.SearchByName(name,cancellation);
            return data is not null ? Ok(data) : Problem(detail: "Data is null");
        }

        /// <summary>
        /// v2 search by name
        /// </summary>
        /// <param name="name">Part of the name to search for</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>Full information collection by name</returns>
        [HttpGet("v2/search-name")]
        [Experimental("NOT_TESTED_CODE")]

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOPSUModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchByNameFull([FromQuery] string name, CancellationToken cancellation)
        {
            var data = await service.SearchByNameFullAsync(name, cancellation);
            return data is not null ? Ok(data) : Problem(detail: "Data is null");
        }

        /// <summary>
        /// v2 search by name. Returns small PSU models only.
        /// </summary>
        /// <param name="name">Part of the name to search for</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A collection of matching small PSU models.</returns>
        [HttpGet("v2/search-name-card")]
        [Experimental("NOT_TESTED_CODE")]

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOPSUSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchByNameCard([FromQuery] string name, CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUSmallModel>? data = await service.SearchByName(name, cancellation);
            return data is not null ? Ok(data) : Problem(detail: "Data is null");
        }

        /// <summary>
        /// Gets small PSU models with pagination.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A page of small PSU models.</returns>
        [HttpGet("card-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOPSUSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSmallWithPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellation)
        {
            IEnumerable<RDTOPSUSmallModel>? data = await service.GetSmallWithPag(page, pageSize, cancellation);
            return Ok(data);
        }


        /// <summary>
        /// Gets full PSU models with pagination.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A page of full PSU models.</returns>
        [HttpGet("full-pag")]
        [Experimental("NOT_TESTED_CODE")]
        public async Task<IActionResult> GetFullWithPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellation) {
            IEnumerable<RDTOPSUModel>? data = await service.GetFullWithPag(page, pageSize, cancellation);
            return Ok(data);
        }

        /// <summary>
        /// Get Small model with filter
        /// </summary>
        /// <param name="filter">DTO PSU Filter model</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection filtered PSU's</returns>
        [HttpGet("by-filter-small")]
        [Experimental("NOT_TESTED_CODE")]

        public async Task<IActionResult> GetWithPagSmall(DTOPSUFilterModel filter, CancellationToken cancellationToken)
        {
            IEnumerable<RDTOPSUSmallModel> data = await service.GetByFilterSmall(filter, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Get Full information model with filter
        /// </summary>
        /// <param name="filter">DTO PSU Filter model</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection filtered PSU's</returns>
        [HttpGet("by-filter")]
        [Experimental("NOT_TESTED_CODE")]

        public async Task<IActionResult> GetWithFilterFull(DTOPSUFilterModel filter, CancellationToken cancellationToken)
        {
            IEnumerable<RDTOPSUModel> data = await service.GetByFilterFull(filter, cancellationToken);
            return Ok(data);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing PSU.
        /// </summary>
        /// <param name="model">The DTO containing fields to update.</param>
        /// <returns>The updated PSU model.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RDTOPSUModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] DTOPSUModelUpdate model) {
            RDTOPSUModel? data = await service.UpdateAsync(model);
            return data is not null?  Ok(data) : NotFound();
        }


        #endregion

        #region Create

        /// <summary>
        /// Creates a new PSU.
        /// </summary>
        /// <param name="model">The DTO containing data for the new PSU.</param>
        /// <returns>The GUID of the created PSU.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] DTOPSUModelCreate model)
        {
            Guid id = await service.CreateAsync(model);
            return Ok(id);
        }
        #endregion

        #region Remove

        /// <summary>
        /// Deletes a PSU by its identifier.
        /// </summary>
        /// <param name="id">The GUID of the PSU to delete.</param>
        /// <returns>True if the PSU was deleted.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remove(Guid id) {
            bool result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : NotFound();
        }
        #endregion
    }
}
