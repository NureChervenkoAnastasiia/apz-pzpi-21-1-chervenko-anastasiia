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
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderController(OrderService ordersService, ILogger<OrderController> logger, IMapper mapper)
        {
            _orderService = ordersService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> Get()
        {
            try
            {
                var orders = await _orderService.GetAsync();
                var orderDtos = _mapper.Map<List<OrderDto>>(orders);
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all orders");
                return StatusCode(500, "Failed to get all orders");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<OrderDto>> GetById(string id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                    return NotFound();

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order with ID {0}", id);
                return StatusCode(500, $"Failed to get order with ID {id}");
            }
        }

        [HttpPost("new-order")]
        public async Task<ActionResult<OrderDto>> Create(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var order = _mapper.Map<Order>(orderDto);
                await _orderService.CreateAsync(order);

                var createdOrderDto = _mapper.Map<OrderDto>(order);
                return CreatedAtAction(nameof(GetById), new { id = createdOrderDto.Id }, createdOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new order");
                return StatusCode(500, "Failed to create new order");
            }
        }

        [HttpDelete("delete-order/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                    return NotFound($"Order with ID {id} not found");

                await _orderService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete order with ID {0}", id);
                return StatusCode(500, "Failed to delete order");
            }
        }

        [HttpPut("update-order/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, OrderDto orderDto)
        {
            try
            {
                var existingOrder = await _orderService.GetByIdAsync(id);
                if (existingOrder == null)
                    return NotFound();

                orderDto.Id = id;
                _mapper.Map(orderDto, existingOrder);

                await _orderService.UpdateAsync(id, existingOrder);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order with ID {0}", id);
                return StatusCode(500, $"Failed to update order with ID {id}");
            }
        }
    }
}
