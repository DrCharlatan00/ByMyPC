using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.GPUModels.RDTO;
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

        /// <summary>
        /// Gets a lightweight (small) collection of GPUs.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of small GPU models.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUSmallModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSmall(CancellationToken cancellationToken)
        {
            var data = await service.GetSmallModelsAsync(cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }
        /// <summary>
        /// Gets the full information for all GPUs.
        /// </summary>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A collection of full GPU models.</returns>
        /// <remarks>
        /// This endpoint may return a large amount of data. Prefer paginated endpoints for large result sets.
        /// </remarks>
        [HttpGet("full")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFull(CancellationToken cancellation)
        {
            var data = await service.GetFullModelsAsync(cancellation);
            return data != null ? Ok(data) : NotFound();
        }

        /// <summary>
        /// Gets a single GPU by its identifier.
        /// </summary>
        /// <param name="id">The GUID of the GPU to retrieve.</param>
        /// <returns>The full GPU model.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RDTOGPUModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var item = await service.GetByIDAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Gets small GPU models with pagination.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A page of small GPU models.</returns>
        [HttpGet("card-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUSmallModel>))]
        public async Task<IActionResult> GetWithPag([FromQuery] int page,[FromQuery] int pageSize, CancellationToken cancellation)
        {
            var data = await service.GetSmallWithPag(page, pageSize, cancellation);
            return Ok(data);
        }

        /// <summary>
        /// Searches GPUs by name. Returns small GPU models.
        /// </summary>
        /// <param name="name">Part of the name to search for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of matching small GPU models.</returns>
        [HttpGet("search-name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUSmallModel>))]
        public async Task<IActionResult> SearchByName([FromQuery]string name, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsync(name, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Searches GPUs by name with pagination. Returns small GPU models.
        /// </summary>
        /// <param name="name">Part of the name to search for.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A page of matching small GPU models.</returns>
        [HttpGet("search-name-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUSmallModel>))]
        public async Task<IActionResult> SearchByNameWithPag([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsyncWithPag(name, page, pageSize, cancellationToken);
            return Ok(data);
        }



        /// <summary>
        /// Gets GPUs matching the specified filter.
        /// </summary>
        /// <param name="filter">Filter criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of matching GPUs.</returns>
        [HttpGet("by-filter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUModel>))]
        public async Task<IActionResult> GetByFilter([FromQuery] DTOGPUFilter filter,CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterAsync(filter,cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Gets GPUs matching the specified filter with pagination.
        /// </summary>
        /// <param name="filter">Filter criteria.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A page of matching GPUs.</returns>
        [HttpGet("by-filter-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOGPUModel>))]
        public async Task<IActionResult> GetByFilterWithPag([FromQuery] DTOGPUFilter filter, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterWithPagAsync(filter, page,pageSize,cancellationToken);
            return Ok(data);
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates an existing GPU.
        /// </summary>
        /// <param name="model">The DTO containing fields to update.</param>
        /// <returns>The updated GPU model.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RDTOGPUModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody]DTOGPUUpdateModel model) 
        {
            var result = await service.UpdateAsync(model);
            return result is not null ? Ok(result) : NotFound();
        }

        #endregion

        #region Create
        /// <summary>
        /// Creates a new GPU.
        /// </summary>
        /// <param name="model">The DTO containing data for the new GPU.</param>
        /// <returns>The GUID of the created GPU.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> Create([FromBody] DTOGPUCreateModel model)
        {
            var result  = await service.CreateAsync(model);
            return Ok(result);
        }
        #endregion


        #region Remove
        /// <summary>
        /// Deletes a GPU by its identifier.
        /// </summary>
        /// <param name="id">The GUID of the GPU to delete.</param>
        /// <returns>True if the GPU was deleted.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : NotFound();
        }
        #endregion

    }
}
