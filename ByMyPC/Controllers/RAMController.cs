using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Models.RAMModels.RDTO;
using ByMyPC.Services.RamService;
using Microsoft.AspNetCore.Mvc;


namespace ByMyPC.Controllers
{
    /// <summary>
    /// Provides HTTP endpoints for retrieving, creating, updating, and deleting RAM modules,
    /// as well as attaching RAM modules to and detaching them from PCs.
    /// </summary>
    /// <param name="service">The service responsible for RAM-related operations.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class RAMController(IRamService service) : ControllerBase
    {
        private readonly IRamService service = service;

        #region Get Methods

        /// <summary>
        /// Retrieves a collection of RAM modules in a lightweight representation.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>
        /// An HTTP 200 response containing the RAM collection,
        /// or an HTTP 500 response if the service returns null.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetSmall(CancellationToken cancellationToken)
        {
            IEnumerable<RDTORAMSmallModel>? data = await service.GetSmallAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem();
        }

        /// <summary>
        /// Retrieves the full representations of all RAM modules.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>
        /// An HTTP 200 response containing the RAM collection,
        /// or an HTTP 500 response if the service returns null.
        /// </returns>
        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken)
        {
            var data = await service.GetFullAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem();
        }

        /// <summary>
        /// Retrieves a RAM module by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the RAM module.</param>
        /// <returns>
        /// An HTTP 200 response containing the RAM module if found;
        /// otherwise, an HTTP 404 response.
        /// </returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Retrieves full RAM module representations using pagination.
        /// </summary>
        /// <param name="page">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>An HTTP 200 response containing the requested page of RAM modules.</returns>
        [HttpGet("full-pag")]
        public async Task<IActionResult> GetFullWithPagination(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.GetFullWithPagination(page, pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Retrieves lightweight RAM module representations using pagination.
        /// </summary>
        /// <param name="page">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>An HTTP 200 response containing the requested page of RAM modules.</returns>
        [HttpGet("card-pag")]
        public async Task<IActionResult> GetSmallWithPagination(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.GetSmallWithPagination(page, pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Searches for RAM modules by name and returns their full representations.
        /// </summary>
        /// <param name="name">The name or search term to look for.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>An HTTP 200 response containing the matching RAM modules.</returns>
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string name,
            CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsyncFull(name, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Searches for RAM modules by name and returns lightweight results using pagination.
        /// </summary>
        /// <param name="name">The name or search term to look for.</param>
        /// <param name="page">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>An HTTP 200 response containing the requested page of matching RAM modules.</returns>
        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNameWithPagination(
            [FromQuery] string name,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameAsyncSmallPaginated(
                name, page, pageSize, cancellationToken);

            return Ok(data);
        }

        /// <summary>
        /// Retrieves full RAM module representations matching the specified filters.
        /// </summary>
        /// <param name="filter">The filtering criteria to apply.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>An HTTP 200 response containing the matching RAM modules.</returns>
        [HttpGet("by-filter")]
        public async Task<IActionResult> GetFullByFilter(
            [FromQuery] DTORAMFilter filter,
            CancellationToken cancellationToken)
        {
            var data = await service.GetFullByFilterAsync(filter, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Retrieves lightweight RAM module representations matching the specified filters,
        /// using pagination.
        /// </summary>
        /// <param name="filter">The filtering criteria to apply.</param>
        /// <param name="page">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>An HTTP 200 response containing the requested page of matching RAM modules.</returns>
        [HttpGet("by-filter-pag")]
        public async Task<IActionResult> GetSmallByFilterWithPagination(
            [FromQuery] DTORAMFilter filter,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var data = await service.GetSmallByFilterAsyncPaginated(
                filter, page, pageSize, cancellationToken);

            return Ok(data);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing RAM module.
        /// </summary>
        /// <param name="dto">The data used to update the RAM module.</param>
        /// <returns>
        /// An HTTP 200 response containing the update result if successful;
        /// otherwise, an HTTP 404 response.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTORAMUpdate dto)
        {
            var result = await service.UpdateAsync(dto);
            return result is not null ? Ok(result) : NotFound();
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates a new RAM module.
        /// </summary>
        /// <param name="dto">The data used to create the RAM module.</param>
        /// <returns>An HTTP 200 response containing the creation result.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTORAMCreate dto)
        {
            var result = await service.CreateAsync(dto);
            return Ok(result);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Deletes a RAM module by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the RAM module to delete.</param>
        /// <returns>
        /// An HTTP 200 response if the deletion succeeds;
        /// otherwise, an HTTP 404 response.
        /// </returns>
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            var result = await service.RemoveAsync(id);
            return result == true ? Ok(result) : NotFound();
        }

        #endregion

        #region Other Operation

        /// <summary>
        /// Attaches a RAM module to a PC.
        /// </summary>
        /// <param name="idPc">The unique identifier of the PC.</param>
        /// <param name="idRam">The unique identifier of the RAM module.</param>
        /// <returns>
        /// An HTTP 200 response if the RAM module is attached successfully;
        /// otherwise, an HTTP 404 response.
        /// </returns>
        [HttpPut("Attach")]
        public async Task<IActionResult> Attach([FromQuery] Guid idPc,
                                                [FromQuery] Guid idRam)
        {
            var result = await service.AttachAsync(idPc, idRam);
            return result == true ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Detaches a RAM module from a PC.
        /// </summary>
        /// <param name="idPc">The unique identifier of the PC.</param>
        /// <param name="idRam">The unique identifier of the RAM module.</param>
        /// <returns>
        /// An HTTP 200 response if the RAM module is detached successfully;
        /// otherwise, an HTTP 404 response.
        /// </returns>
        [HttpPut("DeAttach")]
        public async Task<IActionResult> DeAttach([FromQuery] Guid idPc,
                                                  [FromQuery] Guid idRam)
        {
            var result = await service.DeAttachAsync(idPc, idRam);
            return result == true ? Ok(result) : NotFound();
        }

        #endregion
    }
}
