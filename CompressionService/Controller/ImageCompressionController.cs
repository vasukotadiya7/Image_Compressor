using ImageCompressorDemo;
using Microsoft.AspNetCore.Mvc;

namespace CompressionSerive.Controller  
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageCompressionController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Image Compression Service is running.");
        }
        private readonly Compression _service;

        public ImageCompressionController(Compression service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok("Service is healthy");
        }
        [HttpPost]
        public IActionResult Compress([FromBody] ImageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.InputPath))
                    return BadRequest("Input path is required.");

                Result result = _service.CompressImage(request);
                if(result.IsSucess)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
