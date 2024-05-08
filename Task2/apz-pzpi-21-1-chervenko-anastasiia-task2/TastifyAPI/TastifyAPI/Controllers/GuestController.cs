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
    public class GuestController : ControllerBase
    {
        private readonly GuestService _guestService;
        private readonly ILogger<GuestController> _logger;
        private readonly IMapper _mapper;


        public GuestController(GuestService GuestService, ILogger<GuestController> logger, IMapper mapper)
        {
            _guestService = GuestService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET api/GuestController
        [HttpGet]
        public async Task<ActionResult<List<GuestDto>>> Get()
        {
            try
            {
                var Guests = await _guestService.GetAsync();
                var GuestDtos = _mapper.Map<List<GuestDto>>(Guests);
                return Ok(GuestDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all Guests");
                return StatusCode(500, "Failed to get all Guests");
            }
        }

        // GET api/GuestController/5
        [HttpGet("guest/{id:length(24)}")]
        public async Task<ActionResult<GuestDto>> GetById(string id)
        {
            try
            {
                var Guest = await _guestService.GetByIdAsync(id);
                if (Guest == null)
                    return NotFound();

                var GuestDto = _mapper.Map<GuestDto>(Guest);
                return Ok(GuestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Guest with ID {0}", id);
                return StatusCode(500, $"Failed to get Guest with ID {id}");
            }
        }

        // POST api/GuestController/create-new-guest/5
        [HttpPost("create-new-guest")]
        public async Task<ActionResult<GuestDto>> Create(GuestDto guestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var Guest = _mapper.Map<Guest>(guestDto);
                await _guestService.CreateAsync(Guest);

                var createdGuestDto = _mapper.Map<GuestDto>(Guest);
                return CreatedAtAction(nameof(GetById), new { id = createdGuestDto.Id }, createdGuestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new Guest");
                return StatusCode(500, "Failed to create new Guest");
            }
        }

        // PUT api/GuestController/update-guest/5
        [HttpPut("update-guest/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, GuestDto guestDto)
        {
            try
            {
                var existingGuest = await _guestService.GetByIdAsync(id);
                if (existingGuest == null)
                    return NotFound();

                guestDto.Id = id;
                _mapper.Map(guestDto, existingGuest);

                await _guestService.UpdateAsync(id, existingGuest);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Guest with ID {0}", id);
                return StatusCode(500, $"Failed to update Guest with ID {id}");
            }
        }

        // DELETE api/<GuestController>/delete-guest/5
        [HttpDelete("delete-guest/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var Guest = await _guestService.GetByIdAsync(id);
                if (Guest == null)
                    return NotFound();

                await _guestService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Guest with ID {0}", id);
                return StatusCode(500, $"Failed to delete Guest with ID {id}");
            }
        }
    }
}
