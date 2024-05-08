using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
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

        // GET api/MenuController
        /*[HttpGet]
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
        }*/
        // GET api/MenuController
        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> Get()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all menus");

                var menuItems = await _menuService.GetAsync();
                _logger.LogInformation($"Retrieved {menuItems.Count} menus");

                var menuDtos = _mapper.Map<List<MenuDto>>(menuItems);
                _logger.LogInformation("Mapped menu items to MenuDto");

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

        // POST api/MenuController/create-new-menu/5
        [HttpPost("create-new-menu")]
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

        // PUT api/MenuController/update-menu/5
        [HttpPut("update-menu/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, MenuDto updateDto)
        {
            try
            {
                var existingMenu = await _menuService.GetByIdAsync(id);
                if (existingMenu == null)
                    return NotFound();

                existingMenu.Id = id;
                _mapper.Map(updateDto, existingMenu);

                await _menuService.UpdateAsync(id, existingMenu);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Menu with ID {0}", id);
                return StatusCode(500, $"Failed to update Menu with ID {id}");
            }
        }

        // DELETE api/<MenuController>/delete-menu/5
        [HttpDelete("delete-menu/{id:length(24)}")]
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
    }
}
