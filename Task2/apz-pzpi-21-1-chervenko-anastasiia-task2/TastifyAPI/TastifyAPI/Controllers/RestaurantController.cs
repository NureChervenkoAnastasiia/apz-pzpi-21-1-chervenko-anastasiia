using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
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

        // GET api/RestaurantController
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

        // GET api/RestaurantController/5
        [HttpGet("restaurant/{id:length(24)}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurantById(string id)
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

        // POST api/RestaurantController/create-new-restaurant/5
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
                return CreatedAtAction(nameof(GetRestaurantById), new { id = createdRestaurantDto.Id }, createdRestaurantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new restaurant");
                return StatusCode(500, "Failed to create new restaurant");
            }
        }

        // PUT api/RestaurantController/update-restaurant/5
        [HttpPut("update-restaurant/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, RestaurantDto restaurantDto)
        {
            try
            {
                var existingRestaurant = await _restaurantsService.GetByIdAsync(id);
                if (existingRestaurant == null)
                    return NotFound();

                restaurantDto.Id = id;
                _mapper.Map(restaurantDto, existingRestaurant);

                await _restaurantsService.UpdateAsync(id, existingRestaurant);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update restaurant with ID {0}", id);
                return StatusCode(500, $"Failed to update restaurant with ID {id}");
            }
        }

        // DELETE api/<RestaurantController>/delete-restaurant/5
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
        
    }
}
