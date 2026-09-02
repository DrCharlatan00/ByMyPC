using Microsoft.AspNetCore.Mvc;
using ByMyPC.Services.CpuService;
using Microsoft.AspNetCore.Components;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;


namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPUController(ICpuService cpuService) : ControllerBase
    {
        private readonly ICpuService cpuService = cpuService;

        /// <summary>
        /// Get collection card Cpu in database
        /// </summary>
        /// <param name="cancellationToken">default param</param>
        /// <returns>collection card Cpu </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCardInfo(CancellationToken cancellationToken) {
            var data = await cpuService.GetRDTOSmallModelAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.");
        }

        /// <summary>
        /// Get in db full information collection on Cpu's
        /// </summary>
        /// <param name="cancellationToken">default param</param>
        /// <returns> full information collection on Cpu's</returns>
        /// <remarks>Do not use if you do not want to get all the information accurately, otherwise you will create unnecessary overload</remarks>
        [HttpGet("full")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFullInfo(CancellationToken cancellationToken) {
            var data = await cpuService.GetFullCpuAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.Possible full collection offline now");
        }


        /// <summary>
        /// Get in db full information collection on Cpu's with pagination
        /// </summary>
        /// <param name="page">current page</param>
        /// <param name="pageSize">count elements</param>
        /// <param name="cancellationToken">default param</param>
        /// <returns></returns>
        [HttpGet("full-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFullPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await cpuService.GetFullCpuPagination(page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.");
        }

        /// <summary>
        /// Get full information on Cpu 
        /// </summary>
        /// <param name="id">The processor ID you want to get</param>
        /// <returns>full information on Cpu </returns>
        /// <remarks>Don't forget to submit the information in the following format "api/cpu/YouId" </remarks>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuSmallModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id) {
            var data = await cpuService.GetById(id);
            return data is not null ? Ok(data) : NotFound();
        }

        /// <summary>
        /// Get collection card Cpu in database with pagination
        /// </summary>
        /// <param name="page">current page</param>
        /// <param name="pageSize">count elements</param>
        /// <param name="cancellationToken">default param</param>
        /// <returns>collection card Cpu in database with pagination</returns>
        [HttpGet("card-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCardPagination([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await cpuService.GetSmallModelsWithPaginationAsync(page, pageSize, cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "The server unexpectedly failed to return the collection.");
        }

        /// <summary>
        /// Search in db Cpu's with name = You_Name
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="cancellationToken">default param</param>
        /// <returns>Collection card Cpu's with the desired name</returns>
        [HttpGet("search-name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuSmallModel>))]
        public async Task<IActionResult> SeachByName([FromQuery] string name, CancellationToken cancellationToken) {
            var data = await cpuService.SearchByNameAsync(name, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Search in db Cpu's with name = You_Name with pagination
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="page">current page</param>
        /// <param name="pageSize">count elements</param>
        /// <param name="cancellationToken">default param</param>
        /// <returns>Collection card cpu's with the desired name</returns>
        [HttpGet("search-name-pag")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuSmallModel>))]
        public async Task<IActionResult> SeachByNameCardPagination([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await cpuService.SearchByNameWithPaginationAsync(name,page,pageSize,cancellationToken);
            return Ok(data);
        }


        /// <summary>
        /// Update Cpu in db
        /// </summary>
        /// <param name="model">The parameters you want to update </param>
        /// <returns>Return full updated model</returns>
        /// <remarks>Do not forgot add to model id</remarks>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RDTOCpuSmallModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] DTOCpuUpdateModel model) {
            RDTOCpuModel? result = await cpuService.UpdateAsync(model);
            return result is not null ? Ok(result) : Problem(detail: "Data unexpect not updated");
        }

        /// <summary>
        /// Create new Cpu in db
        /// </summary>
        /// <param name="model">The parameters you want to create</param>
        /// <returns>Guid created Cpu</returns>
        /// <remarks>ID is generated automatically in db</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] DTOCpuCreateModel model) {
            var result = await cpuService.CreateAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Delete Cpu in db
        /// </summary>
        /// <param name="id">The ID by which you want to remove the processor</param>
        /// <returns>Not return data </returns>

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id) {
            await cpuService.RemoveAsync(id);
            return Ok();
        }
        

    }
}
