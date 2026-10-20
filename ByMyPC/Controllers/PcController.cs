using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Services.PCService;
using Microsoft.AspNetCore.Mvc;


namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Provides API endpoints for managing personal computer configurations.
    /// </summary>
    /// <param name="service">The service used to perform PC-related operations.</param>
    public class PcController(IPcService service) : ControllerBase
    {
        private readonly IPcService service = service;

        #region Get Methods

        /// <summary>
        /// Retrieves a lightweight collection of all PC configurations.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>
        /// An <see cref="OkObjectResult"/> containing the PC configurations,
        /// or a problem response if no data is available.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetSmall(CancellationToken cancellationToken)
        {
            var data = await service.GetSmallAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem();
        }

        /// <summary>
        /// Retrieves complete information for all PC configurations.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>
        /// An <see cref="OkObjectResult"/> containing the PC configurations,
        /// or a problem response if no data is available.
        /// </returns>
        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken)
        {
            var data = await service.GetFullAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem();
        }

        /// <summary>
        /// Retrieves a PC configuration by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the PC configuration.</param>
        /// <returns>
        /// An <see cref="OkObjectResult"/> containing the PC configuration,
        /// or a not-found response if it does not exist.
        /// </returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Retrieves complete PC configuration information using pagination.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>A paginated collection of complete PC configurations.</returns>
        [HttpGet("full-pag")]
        public async Task<IActionResult> GetFullPaginated(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.GetFullPaginatedAsync(page, pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Retrieves lightweight PC configuration information using pagination.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>A paginated collection of lightweight PC configurations.</returns>
        [HttpGet("card-pag")]
        public async Task<IActionResult> GetSmallWithPag(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.GetSmallPaginatedAsync(page, pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Searches for PC configurations by name.
        /// </summary>
        /// <param name="name">The name or name fragment to search for.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The search results.</returns>
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string name,
            CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsync(name, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Searches for PC configurations by name using pagination.
        /// </summary>
        /// <param name="name">The name or name fragment to search for.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The paginated search results.</returns>
        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNamePaginated(
            [FromQuery] string name,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.SearchByNamePaginatedAsync(
                name,
                page,
                pageSize,
                cancellationToken);

            return Ok(data);
        }

        /// <summary>
        /// Retrieves PC configurations matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>
        /// The matching PC configurations, or a not-found response if no data is available.
        /// </returns>
        [HttpGet("by-filter")]
        public async Task<IActionResult> GetByFilter(
            [FromQuery] DTOPCFilter filter,
            CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterAsync(filter, cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }

        /// <summary>
        /// Retrieves filtered PC configurations using pagination.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>
        /// The paginated matching PC configurations, or a not-found response if no data is available.
        /// </returns>
        [HttpGet("by-filter-card")]
        public async Task<IActionResult> GetByFilterPaginated(
            [FromQuery] DTOPCFilter filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.GetByFilterAsyncPaginated(
                filter,
                page,
                pageSize,
                cancellationToken);

            return data is not null ? Ok(data) : NotFound();
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing PC configuration.
        /// </summary>
        /// <param name="dto">The data used to update the PC configuration.</param>
        /// <returns>
        /// The updated PC configuration, or a not-found response if it does not exist.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTOPCUpdateModel dto)
        {
            var item = await service.UpdateAsync(dto);
            return item is not null ? Ok(item) : NotFound();
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates a new PC configuration.
        /// </summary>
        /// <param name="dto">The data required to create the PC configuration.</param>
        /// <returns>The created PC configuration.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(DTOPCCreateModel dto)
        {
            var item = await service.CreateAsync(dto);
            return Ok(item);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Deletes a PC configuration.
        /// </summary>
        /// <param name="id">The unique identifier of the PC configuration to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            var result = await service.RemoveAsync(id);
            return Ok(result);
        }

        #endregion

        #region Attach And Deattach

        /// <summary>
        /// Attaches a component to a PC configuration.
        /// </summary>
        /// <param name="dto">The data describing the PC, component, and attachment options.</param>
        /// <returns>The result of the attach operation.</returns>
        [HttpPut("attach")]
        public async Task<IActionResult> AttachComponents([FromBody] DTOPCAttachOrDeattach dto)
        {
            var result = await service.AttachAsync(
                dto.pc,
                dto.Item,
                dto.WithAttachToPcCode);

            return Ok(result);
        }

        /// <summary>
        /// Detaches a component from a PC configuration.
        /// </summary>
        /// <param name="dto">The data describing the PC, component, and detachment options.</param>
        /// <returns>The result of the detach operation.</returns>
        [HttpPut("deattach")]
        public async Task<IActionResult> DeattachComponents([FromBody] DTOPCAttachOrDeattach dto)
        {
            var result = await service.DeattachAsync(
                dto.pc,
                dto.Item,
                dto.WithAttachToPcCode);

            return Ok(result);
        }

        #endregion
    }
}
