using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockLocationController : ControllerBase
    {
        private readonly IStockLocationService _stockLocationService;

        public StockLocationController(IStockLocationService stockLocationService)
        {
            _stockLocationService = stockLocationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _stockLocationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _stockLocationService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StockLocationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _stockLocationService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StockLocationUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id uyuşmuyor.");

            var result = await _stockLocationService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stockLocationService.DeleteAsync(id);
            return result.Success ? Ok() : BadRequest(result);
        }
    }
}
