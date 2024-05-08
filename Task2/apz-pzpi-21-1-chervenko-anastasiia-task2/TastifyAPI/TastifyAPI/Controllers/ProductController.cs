using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.CreateDTOs;
using TastifyAPI.DTOs.UpdateDTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(ProductService productsService, ILogger<ProductController> logger, IMapper mapper)
        {
            _productService = productsService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> Get()
        {
            try
            {
                var products = await _productService.GetAsync();
                var productDtos = _mapper.Map<List<ProductDto>>(products);
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all products");
                return StatusCode(500, "Failed to get all products");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<ProductDto>> GetById(string id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound();

                var productDto = _mapper.Map<ProductDto>(product);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product with ID {0}", id);
                return StatusCode(500, $"Failed to get product with ID {id}");
            }
        }

        [HttpPost("new-product")]
        public async Task<ActionResult<ProductDto>> Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productService.CreateAsync(product);

                var createdProductDto = _mapper.Map<ProductDto>(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProductDto.Id }, createdProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new product");
                return StatusCode(500, "Failed to create new product");
            }
        }

        [HttpDelete("delete-product/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                await _productService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product with ID {0}", id);
                return StatusCode(500, "Failed to delete product");
            }
        }


        [HttpPut("update-product/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ProductDto productDto)
        {
            try
            {
                var existingProduct = await _productService.GetByIdAsync(id);
                if (existingProduct == null)
                    return NotFound();

                productDto.Id = id;
                _mapper.Map(productDto, existingProduct);

                await _productService.UpdateAsync(id, existingProduct);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product with ID {0}", id);
                return StatusCode(500, $"Failed to update product with ID {id}");
            }
        }
    }
}
