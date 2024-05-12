using AutoMapper;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TastifyAPI.Helpers;

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

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
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

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(string id)
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

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("guest-bookings/{id:length(24)}")]
        public async Task<ActionResult<List<BookingDto>>> GetAllGuestBookings(string id)
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync(id);
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all bookings for guest with ID {0}", id);
                return StatusCode(500, $"Failed to get all bookings for guest with ID {id}");
            }
        }

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("bookins-by-date")]
        public async Task<ActionResult<BookingDto>> GetBookingsByDate([FromQuery] DateTime date)
        {
            try
            {
                var booking = await _bookingService.GetByDateAsync(date);
                if (booking == null)
                    return NotFound($"Booking for date {date} not found");

                var bookingDto = _mapper.Map<BookingDto>(booking);
                return Ok(bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get bookings for date {0}", date);
                return StatusCode(500, $"Failed to get bookings for date {date}");
            }
        }

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpGet("sorted-bookings-by-date")]
        public async Task<ActionResult<List<BookingDto>>> GetSortedBookingsByDate([FromQuery] DateTime date)
        {
            try
            {
                var sortedBookings = await _bookingService.GetSortedByDateAsync(date);
                var bookingDtos = _mapper.Map<List<BookingDto>>(sortedBookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sorted bookings by date");
                return StatusCode(500, "Failed to get sorted bookings by date");
            }
        }

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(BookingDto bookingDto)
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

                return CreatedAtAction(nameof(GetBookingById), new { id = createdBookingDto.Id }, createdBookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new booking");
                return StatusCode(500, "Failed to create new booking");
            }
        }

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateBooking(string id, BookingDto bookingDto)
        {
            try
            {
                var existingBooking = await _bookingService.GetByIdAsync(id);
                if (existingBooking == null)
                    return NotFound();

                bookingDto.Id = id;
                _mapper.Map(bookingDto, existingBooking);

                await _bookingService.UpdateAsync(id, existingBooking);

                return Ok("Booking updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update booking with ID {0}", id);
                return StatusCode(500, $"Failed to update booking with ID {id}");
            }
        }

        [Authorize(Roles = Roles.Worker + "," + Roles.Administrator)]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound($"Booking with ID {id} not found");

                await _bookingService.DeleteAsync(id);

                return Ok("Booking deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete booking with ID {0}", id);
                return StatusCode(500, "Failed to delete booking");
            }
        }  
    }
}
