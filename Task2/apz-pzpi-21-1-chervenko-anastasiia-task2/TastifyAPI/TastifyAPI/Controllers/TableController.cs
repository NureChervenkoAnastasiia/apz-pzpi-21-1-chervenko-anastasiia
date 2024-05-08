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
    public class TableController : ControllerBase
    {
        private readonly TableService _tableService;
        private readonly ILogger<TableController> _logger;
        private readonly IMapper _mapper;

        public TableController(TableService tableService, ILogger<TableController> logger, IMapper mapper)
        {
            _tableService = tableService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TableDto>>> Get()
        {
            try
            {
                var tables = await _tableService.GetAsync();
                var tableDtos = _mapper.Map<List<TableDto>>(tables);
                return Ok(tableDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all tables");
                return StatusCode(500, "Failed to get all tables");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<TableDto>> GetById(string id)
        {
            try
            {
                var table = await _tableService.GetByIdAsync(id);
                if (table == null)
                    return NotFound();

                var tableDto = _mapper.Map<TableDto>(table);
                return Ok(tableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get table with ID {0}", id);
                return StatusCode(500, $"Failed to get table with ID {id}");
            }
        }

        [HttpPost("new-table")]
        public async Task<ActionResult<TableDto>> Create(TableDto tableDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var table = _mapper.Map<Table>(tableDto);
                await _tableService.CreateAsync(table);

                var createdTableDto = _mapper.Map<TableDto>(table);
                return CreatedAtAction(nameof(GetById), new { id = createdTableDto.Id }, createdTableDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new table");
                return StatusCode(500, "Failed to create new table");
            }
        }

        [HttpDelete("delete-table/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var table = await _tableService.GetByIdAsync(id);
                if (table == null)
                    return NotFound($"Table with ID {id} not found");

                await _tableService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete table with ID {0}", id);
                return StatusCode(500, "Failed to delete table");
            }
        }


        [HttpPut("update-table/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, TableDto tableDto)
        {
            try
            {
                var existingTable = await _tableService.GetByIdAsync(id);
                if (existingTable == null)
                    return NotFound();

                tableDto.Id = id;
                _mapper.Map(tableDto, existingTable);

                await _tableService.UpdateAsync(id, existingTable);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update table with ID {0}", id);
                return StatusCode(500, $"Failed to update table with ID {id}");
            }
        }
    }
}
