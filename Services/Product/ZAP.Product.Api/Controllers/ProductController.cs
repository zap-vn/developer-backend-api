using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZAP.Product.Domain.Entities;
using ZAP.Product.Domain.Interfaces;
using ZAP.Product.Api;

namespace ZAP.Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll([FromQuery] string? lang)
        {
            var entities = await _productRepository.GetAllAsync();
            var response = entities.Select(e => ProductResponseDto.FromEntity(e, lang));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id, [FromQuery] string? lang)
        {
            var entity = await _productRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(ProductResponseDto.FromEntity(entity, lang));
        }

        [HttpPost]
        public async Task<ActionResult<ProductEntity>> Create(ProductEntity product)
        {
            var createdProduct = await _productRepository.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductEntity product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var updated = await _productRepository.UpdateAsync(product);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetByCategory(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            return Ok(products);
        }
    }
}
