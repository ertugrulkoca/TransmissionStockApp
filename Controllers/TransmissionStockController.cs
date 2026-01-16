using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransmissionStockController : ControllerBase
    {
        private readonly ITransmissionStockService _transmissionStockService;

        public TransmissionStockController(ITransmissionStockService service)
        {
            _transmissionStockService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _transmissionStockService.GetAllAsync();
            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _transmissionStockService.GetByIdAsync(id);
            return result.Success
                ? Ok(result)
                : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransmissionStockCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _transmissionStockService.CreateAsync(model);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
                : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransmissionStockUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _transmissionStockService.UpdateAsync(id, model);
            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _transmissionStockService.DeleteAsync(id);
            return result.Success
                ? NoContent()
                : BadRequest(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var result = await _transmissionStockService.GetAllPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("check-duplicate")]
        public async Task<IActionResult> CheckDuplicate([FromBody] DuplicateCheckDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SparePartNo))
                return BadRequest("SparePartNo zorunlu.");

            var result = await _transmissionStockService.CheckDuplicateAsync(
                dto.TransmissionBrandId, dto.SparePartNo, dto.TransmissionStatusId);

            return Ok(result);
        }

    }
}
