using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.HDDModels.RDTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Services.HDDService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics.CodeAnalysis;

namespace ByMyPC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HddsController(IHDDService service) : ControllerBase
    {
        private readonly IHDDService service = service;

        #region Get
        /// <summary>
        /// Get Card collection HDD's
        /// </summary>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Card collection HDD's</returns>
        [HttpGet]
        public async Task<IActionResult> GetCard(CancellationToken cancellationToken) {
            var data = await service.GetCardModelsAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect is null");
        }

        /// <summary>
        /// Get full info collection HDD's
        /// </summary>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Get full info collection HDD's</returns>
        [HttpGet("full")]
        public async Task<IActionResult> GetFull(CancellationToken cancellationToken)
        {
            var data = await service.GetModelsAsync(cancellationToken);
            return data is not null ? Ok(data) : Problem(detail: "Collection unexpect is null");
        }


        /// <summary>
        /// Get by id full information for HDD
        /// </summary>
        /// <param name="id">Id Db Motherboard</param>
        /// <returns>full information for HDD</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) {
            var item = await service.GetByIdAsync(id);
            return item is not null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Get Card collection HDD's with pagination
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count items</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Card collection HDD's</returns>
        [HttpGet("card-pag")]
        public async Task<IActionResult> GetCardWithPag([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken) {
            var data = await service.GetCardWithPagination(page, pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Search in db HDD's with name = You_Name
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Collection full info HDD's</returns>
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken) {
            IEnumerable<RDTOHDDModel>? data = await service.SearchByName(name, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Search in db HDD's with name = You_Name and pagination
        /// </summary>
        /// <param name="name">the name by which you want to search</param>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count items</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>Collection card HDD's</returns>
        [HttpGet("search-name-pag")]
        public async Task<IActionResult> SearchByNameWithPag([FromQuery] string name, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.SearchByNameWithPag(name, page, pageSize, cancellationToken);
            return Ok(data);
        }

        /// <summary>
        /// Get collecttion full motherboard's info by filters
        /// </summary>
        /// <param name="filter">You can filter by name, size disk, connector type</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>collecttion full HDD's info by filters</returns>
        [HttpGet("by-filter")]
        public async Task<IActionResult> GetByFilter([FromQuery] DTOHDDFilter filter, CancellationToken cancellationToken) {
            IEnumerable<RDTOHDDModel>? data = await service.GetByFilter(filter,cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">You can filter by name, size disk, connector type</param>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count items</param>
        /// <param name="cancellationToken">Default param</param>
        /// <returns>collecttion card HDD's info by filters</returns>
        [HttpGet("by-filter-pag")]
        public async Task<IActionResult> GetByFiterPagination([FromQuery] DTOHDDFilter filter, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var data = await service.GetCardByFilterWithPag(filter,page,pageSize, cancellationToken);
            return data is not null ? Ok(data) : NotFound();
        }
        #endregion

        #region Update
        /// <summary>
        /// Put method for update HDD
        /// </summary>
        /// <param name="model">Model new infomation for HDD</param>
        /// <returns>Updated model HDD</returns>
        /// <remarks>Do not fotgot hand over id in model</remarks>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTOHDDUpdateModel model) {
            var result = await service.UpdateAsync(model);
            return result is not null ? Ok(result) : BadRequest();
        }
        #endregion

        #region Create
        /// <summary>
        /// Post method for create new HDD
        /// </summary>
        /// <param name="model">New HDD</param>
        /// <returns>Guid new HDD</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOHDDCreateModel model) {
            var result = await service.CreateAsync(model);
            return Ok(result);
        }
        /// <summary>
        /// Post method for create new HDD and attach to pc 
        /// </summary>
        /// <param name="model">New HDD</param>
        /// <param name="id">The computer ID to which you want to attach the HDD</param>
        /// <returns>Guid new HDD</returns>
        /// <remarks>If an error occurs during the creation or attachment attempt, the entire operation is rolled back; the method is atomic.</remarks>
        [HttpPost("create-attach")]
        public async Task<IActionResult> CreateAndAttach([FromBody] DTOHDDCreateModel model,[FromQuery] Guid id)
        {
            var result = await service.CreateAndAttachAsync(model,id);
            return Ok(result);
        }
        #endregion


        #region Remove

        /// <summary>
        /// Delete method for remove HDD in db
        /// </summary>
        /// <param name="id">Guid HDD which you want to delete</param>
        /// <returns>Ok)</returns>
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] Guid id) {
            await service.RemoveAsync(id);
            return Ok();
        }
        #endregion

        /// <summary>
        /// Put method for Attach HDD to Pc
        /// </summary>
        /// <param name="model">ID of the disk you want to attach,ID of the computer to which you want to attach the disk</param>
        /// <returns>Ok or BadRequest</returns>
        [HttpPut("attach")]
        public async Task<IActionResult> Attach([FromBody] DTOHddOperations model) {
            bool result = await service.AttachHdd(model.PcId, model.HddId);
            return result == true ? Ok(result) : Problem();
        }

        /// <summary>
        /// Put method for Deattach HDD to Pc
        /// </summary>
        /// <param name="model">ID of the disk you want to Deattach,ID of the computer to which you want to Deattach the disk</param>
        /// <returns></returns>
        [HttpPut("deattach")]
        public async Task<IActionResult> Deattach([FromBody] DTOHddOperations model)
         {
            bool result = await service.DeattachHdd(model.PcId, model.HddId);
            return result == true ? Ok(result) : Problem();
        }
    }
}
