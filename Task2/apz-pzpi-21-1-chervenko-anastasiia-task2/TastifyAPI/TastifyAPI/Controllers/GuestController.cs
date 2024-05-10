using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
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
        private readonly IPasswordHasher<Guest> _passwordHasher;


        public GuestController(GuestService GuestService, ILogger<GuestController> logger, IMapper mapper, IPasswordHasher<Guest> passwordHasher)
        {
            _guestService = GuestService;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        // GET api/GuestController/all-guests
        [HttpGet("all-guests")]
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

        // GET api/guest-profile/5
        [HttpGet("guest-profile/{id:length(24)}")]
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

        // POST api/GuestController/register
        [HttpPost("guest-register")]
        public async Task<ActionResult> Register(GuestRegistrationDto guestRegistrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _guestService.AnyAsync(g => g.Email == guestRegistrationDto.Email))
                {
                    return BadRequest("Guest with such email already exists");
                }

                var newGuest = new Guest
                {
                    Name = guestRegistrationDto.Name,
                    MobileNumber = guestRegistrationDto.Phone,
                    Email = guestRegistrationDto.Email,
                    Bonus = 0, 
                    Login = guestRegistrationDto.Login
                };

                newGuest.PasswordHash = _passwordHasher.HashPassword(newGuest, guestRegistrationDto.Password);

                await _guestService.CreateAsync(newGuest);

                return Ok("New guest registration was successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during guest registration");
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/GuestController/login
        [HttpPost("guest-login")]
        public async Task<IActionResult> Login(GuestLoginDto guestLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guest = await _guestService.GetByLoginAsync(guestLoginDto.Login);

                if (guest == null)
                {
                    return BadRequest("Guest with such login does not exist");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(guest, guest.PasswordHash, guestLoginDto.Password);

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return BadRequest("Invalid login or password");
                }

                return Ok(guest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, ex.Message);
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

        // GET api/GuestController/sorted-by-name-and-bonus
        [HttpGet("sorted-by-name-and-bonus")]
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
