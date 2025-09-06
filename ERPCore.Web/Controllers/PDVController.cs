using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCore.Services;
using ERPCore.Models;

namespace ERPCore.Web.Controllers
{
    [Authorize(Roles = "Admin,User,Manager")]
    public class PDVController : Controller
    {
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;
        private readonly IAuthService _authService;

        public PDVController(IProductService productService, ISaleService saleService, IAuthService authService)
        {
            _productService = productService;
            _saleService = saleService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProductByCode(string code)
        {
            var product = await _productService.GetProductByCode(code);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessSale([FromBody] Sale sale)
        {
            try
            {
                // Obter o ID do usuário logado
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                sale.UserId = userId;

                var createdSale = await _saleService.CreateSale(sale);
                return Ok(new
                {
                    success = true,
                    saleCode = createdSale.SaleCode,
                    message = "Venda processada com sucesso"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDailySales()
        {
            var dailyTotal = await _saleService.GetDailySalesTotal(DateTime.Today);
            return Ok(new { dailyTotal });
        }
    }
}