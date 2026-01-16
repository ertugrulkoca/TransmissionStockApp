using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransmissionBrandController : ControllerBase
    {
        private readonly ITransmissionBrandService _service;

        public TransmissionBrandController(ITransmissionBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransmissionBrandCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransmissionBrandUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("Id uyuşmuyor.");
            
            var result = await _service.UpdateAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok() : BadRequest(result);
        }
    }

}
