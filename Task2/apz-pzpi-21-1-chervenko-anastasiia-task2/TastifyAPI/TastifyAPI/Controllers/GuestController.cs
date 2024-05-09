using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;

//TODO:/login
//TODO:/get-profile
//TODO:/get-all-bookings
//TODO:/get-all-reciepts

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

        // GET api/GuestController/get-all-guests
        [HttpGet("get-all-guests")]
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
        [HttpGet("get-guest-prifile/{id:length(24)}")]
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

        // POST api/GuestController/register-new-guest/5
        [HttpPost("register-new-guest")]
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

        // PUT api/GuestController/update-guest-profile/5
        [HttpPut("update-guest-profile/{id:length(24)}")]
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

        // DELETE api/<GuestController>/delete-guest-profile/5
        [HttpDelete("delete-guest-profile/{id:length(24)}")]
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

        /*// GET api/GuestController/get-all-receipts
        [HttpGet("get-all-receipts")]
        public async Task<ActionResult<List<OrderDto>>> GetAllReceipts(string guestId)
        {
            try
            {
                var receipts = await _guestService.GetAllGuestOrdersAsync(guestId);
                var receiptDtos = _mapper.Map<List<OrderDto>>(receipts);
                return Ok(receiptDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all receipts for guest with ID {0}", guestId);
                return StatusCode(500, $"Failed to get all receipts for guest with ID {guestId}");
            }
        }*/

        // GET api/GuestController/get-guests-sorted-by-name-and-bonus
        [HttpGet("get-guests-sorted-by-name-and-bonus")]
        public async Task<ActionResult<List<GuestDto>>> GetGuestsSortedByNameAndBonus()
        {
            try
            {
                var guests = await _guestService.GetSortedByNameAndBonusAsync();
                var guestDtos = _mapper.Map<List<GuestDto>>(guests);
                return Ok(guestDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get guests sorted by name and bonus");
                return StatusCode(500, "Failed to get guests sorted by name and bonus");
            }
        }

        // POST api/GuestController/make-coupon
        [HttpPost("make-coupon")]
        public async Task<ActionResult<CouponDto>> MakeCoupon(int bonus)
        {
            try
            {
                var (discount, remainingBonus) = await _guestService.CalculateCouponAsync(bonus);
                var couponDto = new CouponDto { Discount = discount, Bonus = remainingBonus };
                return Ok(couponDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to make coupon for bonus {0}", bonus);
                return StatusCode(500, $"Failed to make coupon for bonus {bonus}");
            }
        }
    }
}
