using Microsoft.AspNetCore.Mvc;
using ERPCore.Models;
using ERPCore.Services;

namespace ERPCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Product>> GetProductByCode(string code)
        {
            var product = await _productService.GetProductByCode(code);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await _productService.UpdateProduct(product);
            }
            catch (Exception)
            {
                if (!await _productService.ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Verificar se o código já existe
            if (await _productService.ProductCodeExists(product.Code))
            {
                return Conflict("Já existe um produto com este código.");
            }

            var createdProduct = await _productService.CreateProduct(product);
            return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}