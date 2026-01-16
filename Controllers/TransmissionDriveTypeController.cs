using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransmissionDriveTypeController : ControllerBase
    {
        private readonly ITransmissionDriveTypeService _service;

        public TransmissionDriveTypeController(ITransmissionDriveTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransmissionDriveTypeCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransmissionDriveTypeUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id uyuşmuyor.");

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
