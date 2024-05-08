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
    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantService _restaurantsService;
        private readonly ILogger<RestaurantsController> _logger;
        private readonly IMapper _mapper;

        public RestaurantsController(RestaurantService restaurantsService, ILogger<RestaurantsController> logger, IMapper mapper)
        {
            _restaurantsService = restaurantsService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<RestaurantDto>>> Get()
        {
            try
            {
                var restaurants = await _restaurantsService.GetAsync();
                var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
                return Ok(restaurantDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all restaurants");
                return StatusCode(500, "Failed to get all restaurants");
            }
        }

        [HttpGet("/{id:length(24)}")]
        public async Task<ActionResult<RestaurantDto>> GetById(string id)
        {
            try
            {
                var restaurant = await _restaurantsService.GetByIdAsync(id);
                if (restaurant == null)
                    return NotFound();

                var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
                return Ok(restaurantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get restaurant with ID {0}", id);
                return StatusCode(500, $"Failed to get restaurant with ID {id}");
            }
        }

        [HttpPost("create-new-restaurant")]
        public async Task<ActionResult<RestaurantDto>> Create(RestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var restaurant = _mapper.Map<Restaurant>(restaurantDto);
                await _restaurantsService.CreateAsync(restaurant);

                var createdRestaurantDto = _mapper.Map<RestaurantDto>(restaurant);
                return CreatedAtAction(nameof(GetById), new { id = createdRestaurantDto.Id }, createdRestaurantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new restaurant");
                return StatusCode(500, "Failed to create new restaurant");
            }
        }

        [HttpDelete("delete-restaurant/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var restaurant = await _restaurantsService.GetByIdAsync(id);
                if (restaurant == null)
                    return NotFound();

                await _restaurantsService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete restaurant with ID {0}", id);
                return StatusCode(500, $"Failed to delete restaurant with ID {id}");
            }
        }

        [HttpPut("update-restaurant/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, RestaurantUpdateDto updateDto)
        {
            try
            {
                var existingRestaurant = await _restaurantsService.GetByIdAsync(id);
                if (existingRestaurant == null)
                    return NotFound();

                _mapper.Map(updateDto, existingRestaurant);

                await _restaurantsService.UpdateAsync(id, existingRestaurant);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update restaurant with ID {0}", id);
                return StatusCode(500, $"Failed to update restaurant with ID {id}");
            }
        }
    }
}
