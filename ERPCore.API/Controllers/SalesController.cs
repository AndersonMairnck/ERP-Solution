using Microsoft.AspNetCore.Mvc;
using ERPCore.Models;
using ERPCore.Services;

namespace ERPCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale([FromBody] Sale sale)
        {
            try
            {
                var createdSale = await _saleService.CreateSale(sale);
                return CreatedAtAction(nameof(GetSale), new { id = createdSale.Id }, createdSale);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _saleService.GetSaleById(id);

            if (sale == null)
                return NotFound();

            return sale;
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<List<Sale>>> GetSalesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRange(startDate, endDate);
            return Ok(sales);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Sale>>> GetSalesByUser(int userId)
        {
            var sales = await _saleService.GetSalesByUser(userId);
            return Ok(sales);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelSale(int id)
        {
            var result = await _saleService.CancelSale(id);

            if (!result)
                return NotFound();

            return Ok(new { message = "Venda cancelada com sucesso" });
        }

        [HttpGet("daily-total")]
        public async Task<ActionResult<decimal>> GetDailyTotal([FromQuery] DateTime date)
        {
            var total = await _saleService.GetDailySalesTotal(date);
            return Ok(total);
        }
    }
}