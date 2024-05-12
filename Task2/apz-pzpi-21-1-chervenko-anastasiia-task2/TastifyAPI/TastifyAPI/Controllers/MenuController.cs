using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;
        private readonly ILogger<MenuController> _logger;
        private readonly IMapper _mapper;

        public MenuController(MenuService menuService, ILogger<MenuController> logger, IMapper mapper)
        {
            _menuService = menuService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET api/MenuController/all-menu-positions
        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> Get()
        {
            try
            {
                var menuItems = await _menuService.GetAsync();
                var menuDtos = _mapper.Map<List<MenuDto>>(menuItems);
                return Ok(menuDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Menus");
                return StatusCode(500, "Failed to get all Menus");
            }
        }

        // GET api/MenuController/all-menu-positions-in-restaurant
        [HttpGet("restaurant/{restaurantId}/all")]
        public async Task<ActionResult<List<MenuDto>>> GetRestaurantMenu(string restaurantId)
        {
            try
            {
                var menuItems = await _menuService.GetRestaurantMenuAsync(restaurantId);
                var menuDtos = _mapper.Map<List<MenuDto>>(menuItems);
                return Ok(menuDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Menus");
                return StatusCode(500, "Failed to get all Menus");
            }
        }

        // GET api/MenuController/5
        [HttpGet("/{id:length(24)}")]
        public async Task<ActionResult<MenuDto>> GetById(string id)
        {
            try
            {
                var menu = await _menuService.GetByIdAsync(id);
                if (menu == null)
                    return NotFound();

                var menuDto = _mapper.Map<MenuDto>(menu);
                return Ok(menuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Menu with ID {0}", id);
                return StatusCode(500, $"Failed to get Menu with ID {id}");
            }
        }

        // POST api/MenuController/new-menu-position
        [HttpPost]
        public async Task<ActionResult<MenuDto>> Create(MenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var menu = _mapper.Map<Menu>(menuDto);
                await _menuService.CreateAsync(menu);

                var createdMenuDto = _mapper.Map<MenuDto>(menu);
                return CreatedAtAction(nameof(GetById), new { id = createdMenuDto.Id }, createdMenuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new Menu");
                return StatusCode(500, "Failed to create new Menu");
            }
        }

        // PUT api/MenuController/update-menu-position/5
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, MenuDto menuDto)
        {
            try
            {
                var existingMenu = await _menuService.GetByIdAsync(id);
                if (existingMenu == null)
                    return NotFound();

                menuDto.Id = id;
                _mapper.Map(menuDto, existingMenu);

                await _menuService.UpdateAsync(id, existingMenu);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Menu with ID {0}", id);
                return StatusCode(500, $"Failed to update Menu with ID {id}");
            }
        }

        // DELETE api/MenuController/delete-menu-position/5
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var menu = await _menuService.GetByIdAsync(id);
                if (menu == null)
                    return NotFound();

                await _menuService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Menu with ID {0}", id);
                return StatusCode(500, $"Failed to delete Menu with ID {id}");
            }
        }

        // GET api/MenuController/all-first-dishes-for-restaurant/5
        [HttpGet("restaurant/{restaurantId}/first-dishes")]
        public async Task<ActionResult<List<MenuDto>>> GetFirstDishesForRestaurant(string restaurantId)
        {
            try
            {
                var firstDishes = await _menuService.GetFirstDishesForRestaurantAsync(restaurantId);
                var firstDishDtos = _mapper.Map<List<MenuDto>>(firstDishes);
                return Ok(firstDishDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all First Dishes for Restaurant {0}", restaurantId);
                return StatusCode(500, $"Failed to get all First Dishes for Restaurant {restaurantId}");
            }
        }

        // GET api/MenuController/second-dishes-for-restaurant/5
        [HttpGet("restaurant/{restaurantId}/second-dishes")]
        public async Task<ActionResult<List<MenuDto>>> GetSecondDishesForRestaurant(string restaurantId)
        {
            try
            {
                var secondDishes = await _menuService.GetSecondDishesForRestaurantAsync(restaurantId);
                var secondDishDtos = _mapper.Map<List<MenuDto>>(secondDishes);
                return Ok(secondDishDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Second Dishes for Restaurant {0}", restaurantId);
                return StatusCode(500, $"Failed to get all Second Dishes for Restaurant {restaurantId}");
            }
        }

        // GET api/MenuController/drinks-for-restaurant/5
        [HttpGet("restaurant/{restaurantId}/drinks")]
        public async Task<ActionResult<List<MenuDto>>> GetDrinksForRestaurant(string restaurantId)
        {
            try
            {
                var drinks = await _menuService.GetDrinksForRestaurantAsync(restaurantId);
                var drinkDtos = _mapper.Map<List<MenuDto>>(drinks);
                return Ok(drinkDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Second Dishes for Restaurant {0}", restaurantId);
                return StatusCode(500, $"Failed to get all Second Dishes for Restaurant {restaurantId}");
            }
        }

        // GET api/MenuController/second-dishes-for-restaurant/5
        [HttpGet("restaurant/{restaurantId}/dishes-rating")]
        public async Task<ActionResult<List<DishPopularityDto>>> GetMostPopularDishes(string restaurantId)
        {
            try
            {
                var popularDishes = await _menuService.GetMostPopularDishesAsync(restaurantId);
                return Ok(popularDishes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Second Dishes for Restaurant {0}", restaurantId);
                return StatusCode(500, $"Failed to get all Second Dishes for Restaurant {restaurantId}");
            }
        }
    }
}
