using Microsoft.AspNetCore.Mvc;
using TransmissionStockApp.Services.Interfaces;

namespace TransmissionStockApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _lookupService.GetLookupDataAsync();
            return Ok(data);
        }
    }
}
