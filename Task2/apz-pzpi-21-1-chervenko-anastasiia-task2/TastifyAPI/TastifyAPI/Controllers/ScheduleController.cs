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
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;
        private readonly ILogger<ScheduleController> _logger;
        private readonly IMapper _mapper;

        public ScheduleController(ScheduleService scheduleService, ILogger<ScheduleController> logger, IMapper mapper)
        {
            _scheduleService = scheduleService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ScheduleDto>>> Get()
        {
            try
            {
                var schedules = await _scheduleService.GetAsync();
                var scheduleDtos = _mapper.Map<List<ScheduleDto>>(schedules);
                return Ok(scheduleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all schedules");
                return StatusCode(500, "Failed to get all schedules");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<ScheduleDto>> GetById(string id)
        {
            try
            {
                var schedule = await _scheduleService.GetByIdAsync(id);
                if (schedule == null)
                    return NotFound();

                var scheduleDto = _mapper.Map<ScheduleDto>(schedule);
                return Ok(scheduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get schedule with ID {0}", id);
                return StatusCode(500, $"Failed to get schedule with ID {id}");
            }
        }

        [HttpPost("new-schedule")]
        public async Task<ActionResult<ScheduleDto>> Create(ScheduleDto scheduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var schedule = _mapper.Map<Schedule>(scheduleDto);
                await _scheduleService.CreateAsync(schedule);

                var createdScheduleDto = _mapper.Map<ScheduleDto>(schedule);
                return CreatedAtAction(nameof(GetById), new { id = createdScheduleDto.Id }, createdScheduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new schedule");
                return StatusCode(500, "Failed to create new schedule");
            }
        }

        [HttpDelete("delete-schedule/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var schedule = await _scheduleService.GetByIdAsync(id);
                if (schedule == null)
                    return NotFound($"Schedule with ID {id} not found");

                await _scheduleService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete schedule with ID {0}", id);
                return StatusCode(500, "Failed to delete schedule");
            }
        }


        [HttpPut("update-schedule/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ScheduleDto scheduleDto)
        {
            try
            {
                var existingSchedule = await _scheduleService.GetByIdAsync(id);
                if (existingSchedule == null)
                    return NotFound();

                scheduleDto.Id = id;
                _mapper.Map(scheduleDto, existingSchedule);

                await _scheduleService.UpdateAsync(id, existingSchedule);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update schedule with ID {0}", id);
                return StatusCode(500, $"Failed to update schedule with ID {id}");
            }
        }
    }
}
