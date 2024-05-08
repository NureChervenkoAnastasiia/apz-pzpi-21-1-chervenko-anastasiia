using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;

//TODO: fix /get-bookings-by-date
//TODO: /get-sorted-bookings-by-date

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly ILogger<BookingController> _logger;
        private readonly IMapper _mapper;

        public BookingController(BookingService bookingService, ILogger<BookingController> logger, IMapper mapper)
        {
            _bookingService = bookingService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> Get()
        {
            try
            {
                var bookings = await _bookingService.GetAsync();
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all bookings");
                return StatusCode(500, "Failed to get all bookings");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<BookingDto>> GetById(string id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound();

                var bookingDto = _mapper.Map<BookingDto>(booking);
                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get booking with ID {0}", id);
                return StatusCode(500, $"Failed to get booking with ID {id}");
            }
        }

        [HttpPost("new-booking")]
        public async Task<ActionResult<BookingDto>> Create(BookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var booking = _mapper.Map<Booking>(bookingDto);
                await _bookingService.CreateAsync(booking);

                var createdBookingDto = _mapper.Map<BookingDto>(booking);
                return CreatedAtAction(nameof(GetById), new { id = createdBookingDto.Id }, createdBookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new booking");
                return StatusCode(500, "Failed to create new booking");
            }
        }

        [HttpDelete("delete-booking/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound($"Booking with ID {id} not found");

                await _bookingService.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete booking with ID {0}", id);
                return StatusCode(500, "Failed to delete booking");
            }
        }


        [HttpPut("update-booking/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, BookingDto bookingDto)
        {
            try
            {
                var existingBooking = await _bookingService.GetByIdAsync(id);
                if (existingBooking == null)
                    return NotFound();

                bookingDto.Id = id;
                _mapper.Map(bookingDto, existingBooking);

                await _bookingService.UpdateAsync(id, existingBooking);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update booking with ID {0}", id);
                return StatusCode(500, $"Failed to update booking with ID {id}");
            }
        }

        [HttpGet("get-bookins-by-date")]
        public async Task<ActionResult<List<BookingDto>>> GetByDate([FromQuery] DateTime date)
        {
            try
            {
                var bookings = await _bookingService.GetByDateAsync(date);
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get bookings for date {0}", date);
                return StatusCode(500, $"Failed to get bookings for date {date}");
            }
        }

    }
}
