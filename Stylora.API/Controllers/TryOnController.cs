using Microsoft.AspNetCore.Mvc;
using Stylora.Domain.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Stylora.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TryOnController : ControllerBase
    {
        private readonly IVirtualTryOnService _tryOnService;

        public TryOnController(IVirtualTryOnService tryOnService)
        {
            _tryOnService = tryOnService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] TryOnRequest request)
        {
            // Simple validation
            if (!System.IO.File.Exists(request.PersonImagePath) || !System.IO.File.Exists(request.GarmentImagePath))
                return NotFound("Images not found at the specified local paths.");

            try
            {
                var personBytes = await System.IO.File.ReadAllBytesAsync(request.PersonImagePath);
                var garmentBytes = await System.IO.File.ReadAllBytesAsync(request.GarmentImagePath);

                // Call the Infrastructure service
                var resultImageBytes = await _tryOnService.ExecuteTryOnAsync(personBytes, garmentBytes);

                // Return image to browser
                return File(resultImageBytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }

    // Input DTO
    public class TryOnRequest
    {
        public string PersonImagePath { get; set; }
        public string GarmentImagePath { get; set; }
    }
}