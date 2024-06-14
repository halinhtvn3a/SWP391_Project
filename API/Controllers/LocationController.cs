using DAOs.Models;
using DAOs.Helper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {

        [HttpPost]
        public IActionResult Post([FromBody] LocationModel location)
        {
            return Ok(new { message = "Location received successfully", location });
        }


        [HttpPost("get-geolocation")]
        public async Task<IActionResult> GetGeolocation([FromBody] string addressModel)
        {
            try
            {
                var geolocation = await GeocodingService.GetGeolocationAsync(addressModel);
                return Ok(geolocation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
