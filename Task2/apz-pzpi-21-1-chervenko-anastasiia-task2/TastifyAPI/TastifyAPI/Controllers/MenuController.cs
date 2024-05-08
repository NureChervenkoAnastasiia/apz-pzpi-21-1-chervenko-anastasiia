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
        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> Get()
        {
            try
            {
                var Menus = await _menuService.GetAsync();
                var MenuDtos = _mapper.Map<List<MenuDto>>(Menus);
                return Ok(MenuDtos);
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
                var Menu = await _menuService.GetByIdAsync(id);
                if (Menu == null)
                    return NotFound();

                var MenuDto = _mapper.Map<MenuDto>(Menu);
                return Ok(MenuDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Menu with ID {0}", id);
                return StatusCode(500, $"Failed to get Menu with ID {id}");
            }
        }

        // POST api/MenuController/create-new-menu/5
        [HttpPost("create-new-menu")]
        public async Task<ActionResult<MenuDto>> Create(MenuDto MenuDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var Menu = _mapper.Map<Menu>(MenuDto);
                await _menuService.CreateAsync(Menu);

                var createdMenuDto = _mapper.Map<MenuDto>(Menu);
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
                var Menu = await _menuService.GetByIdAsync(id);
                if (Menu == null)
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
