using BusinessObjects.Models;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly PriceService _priceService;

        public PricesController()
        {
            _priceService = new PriceService();
        }


        [HttpGet("branchId={branchId}")]
        public async Task<ActionResult<IEnumerable<Price>>> GetPrices(string branchId)
        {
            return _priceService.GetPriceByBranch(branchId).ToList();
        }

        // GET: api/Prices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Price>> GetPrice(string id)
        {
            var Price = _priceService.GetPrice(id);

            if (Price == null)
            {
                return NotFound();
            }

            return Price;
        }
        [HttpPost]
        public async Task<ActionResult<Price>> PostPrice(Price price)
        {

            var Price = _priceService.AddPrice(price);

            return CreatedAtAction("GetPrice", new { id = Price.PriceId }, Price);
        }
        // PUT: api/Prices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrice(string id, Price price)
        {
            var Price = _priceService.GetPrice(id);
            if (id != Price.PriceId)
            {
                return BadRequest();
            }

            _priceService.UpdatePrice(id, price);

            return CreatedAtAction("GetPrice", new { id = Price.PriceId }, Price);
        }

        // POST: api/Prices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet]
        public IActionResult ShowPrice(string branchId, DateOnly slotDate)
        {

            var price = _priceService.ShowPrice(branchId, slotDate);

            return Ok(price);
        }

        // DELETE: api/Prices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(string id)
        {
            var Price = _priceService.GetPrice(id);
            if (Price == null)
            {
                return NotFound();
            }

            _priceService.DeletePrice(id);

            return NoContent();
        }
    }
}
