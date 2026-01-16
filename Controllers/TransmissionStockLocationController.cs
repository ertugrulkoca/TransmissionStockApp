using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Models.DTOs;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransmissionStockLocationController : ControllerBase
    {
        private readonly ITransmissionStockLocationService _service;

        public TransmissionStockLocationController(ITransmissionStockLocationService service)
        {
            _service = service;
        }

        // GET: api/TransmissionStockLocation/5
        [HttpGet("{transmissionStockId}")]
        public async Task<IActionResult> GetAll(int transmissionStockId)
        {
            var result = await _service.GetByTransmissionStockIdAsync(transmissionStockId);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }

        // POST: api/TransmissionStockLocation
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TransmissionStockLocationCreateDto dto)
        {
            var result = await _service.AddAsync(dto.TransmissionStockId, dto.ShelfCode, dto.Quantity);
            if (!result.Success)
                return BadRequest(result);

            return Ok("Ekleme başarılı.");
        }

        // PUT: api/TransmissionStockLocation
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity([FromBody] TransmissionStockLocationUpdateDto dto)
        {
            var result = await _service.UpdateQuantityAsync(dto.TransmissionStockId, dto.ShelfId, dto.Quantity);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok("Güncelleme başarılı.");
        }

        // DELETE: api/TransmissionStockLocation?transmissionStockId=5&stockLocationId=2
        [HttpDelete]
        public async Task<IActionResult> Delete(int transmissionStockId, int shelfId)
        {
            var result = await _service.DeleteAsync(transmissionStockId, shelfId);
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok("Silme başarılı.");
        }
    }

}
