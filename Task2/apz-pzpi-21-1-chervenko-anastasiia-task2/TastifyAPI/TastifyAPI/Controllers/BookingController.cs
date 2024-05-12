﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using TastifyAPI.Services;

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

        [HttpPost]
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

        [HttpPut("{id:length(24)}")]
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

        [HttpDelete("{id:length(24)}")]
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

        // GET api/GuestController/all-guest-bookings
        [HttpGet("guest-bookings/{id:length(24)}")]
        public async Task<ActionResult<List<BookingDto>>> GetAllGuestBookings(string guestId)
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync(guestId);
                var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all bookings for guest with ID {0}", guestId);
                return StatusCode(500, $"Failed to get all bookings for guest with ID {guestId}");
            }
        }

        [HttpGet("bookins-by-date")]
        public async Task<ActionResult<BookingDto>> GetByDate([FromQuery] DateTime date)
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
    }
}
