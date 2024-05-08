using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;

        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMenuItem(MenuDto menuItemDTO)
        {
            try
            {
                await _menuService.AddMenuItemAsync(menuItemDTO);
                return Ok("Menu item added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to add menu item: {ex.Message}");
            }
        }

        [HttpPut("update/{id:length(24)}")]
        public async Task<IActionResult> UpdateMenuItem(string id, MenuDto menuItemDTO)
        {
            try
            {
                var updated = await _menuService.UpdateMenuItemAsync(id, menuItemDTO);
                if (updated)
                    return Ok("Menu item updated successfully");
                else
                    return NotFound("Menu item not found");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to update menu item: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id:length(24)}")]
        public async Task<IActionResult> DeleteMenuItem(string id)
        {
            try
            {
                var deleted = await _menuService.DeleteMenuItemAsync(id);
                if (deleted)
                    return Ok("Menu item deleted successfully");
                else
                    return NotFound("Menu item not found");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to delete menu item: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMenuItems()
        {
            try
            {
                var menuItems = await _menuService.GetAllMenuItemsAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to retrieve menu items: {ex.Message}");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> GetMenuItemById(string id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemByIdAsync(id);
                if (menuItem != null)
                    return Ok(menuItem);
                else
                    return NotFound("Menu item not found");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to retrieve menu item: {ex.Message}");
            }
        }
    }
}
