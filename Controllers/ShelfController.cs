using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfController : ControllerBase
    {
        private readonly IShelfService _shelfService;

        public ShelfController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _shelfService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _shelfService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShelfCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _shelfService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShelfUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id uyuşmuyor.");

            var result = await _shelfService.UpdateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _shelfService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-warehouse/{warehouseId}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            var result = await _shelfService.GetByWarehouseAsync(warehouseId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }

}
