using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Helpers;
using TastifyAPI.IServices;
using TastifyAPI.Services;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly GuestService _guestService;
        private readonly JwtService _jwtService;
        private readonly ILogger<GuestController> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Guest> _passwordHasher;

        public GuestController(GuestService guestService,
                               ILogger<GuestController> logger,
                               IPasswordHasher<Guest> passwordHasher,
                               IMapper mapper,
                               IConfiguration config)
        {
            _guestService = guestService;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = new JwtService(config);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpGet]
        public async Task<ActionResult<List<GuestDto>>> GetAllGuests()
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

        [Authorize(Roles = Roles.Guest + "," + Roles.Administrator)]
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<GuestDto>> GetGuestById(string id)
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

        [Authorize(Roles = Roles.Administrator)]
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

        [Authorize(Roles = Roles.Guest)]
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

        [HttpPost("register")]
        public async Task<ActionResult> Register(GuestRegistrationDto guestRegistrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _guestService.AnyAsync(g => g.Email == guestRegistrationDto.Email))
                    return BadRequest("Guest with such email already exists");

                var newGuest = _mapper.Map<Guest>(guestRegistrationDto);

                newGuest.Bonus = 0;
                newGuest.PasswordHash = _passwordHasher.HashPassword(newGuest, guestRegistrationDto.Password);

                await _guestService.CreateAsync(newGuest);

                var token = _jwtService.GenerateGuestToken(newGuest);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during guest registration");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(GuestLoginDto guestLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var guest = await _guestService.GetByLoginAsync(guestLoginDto.Login);

                if (guest == null)
                    return BadRequest("Guest with such login does not exists");


                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(guest, guest.PasswordHash, guestLoginDto.Password);

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return BadRequest("Invalid login or password");
                }

                var token = _jwtService.GenerateGuestToken(guest);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = Roles.Guest + "," + Roles.Administrator)]
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateGuest(string id, GuestDto guestDto)
        {
            try
            {
                var existingGuest = await _guestService.GetByIdAsync(id);
                if (existingGuest == null)
                    return NotFound();

                guestDto.Id = id;
                _mapper.Map(guestDto, existingGuest);

                await _guestService.UpdateAsync(id, existingGuest);

                return Ok("Guest updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Guest with ID {0}", id);
                return StatusCode(500, $"Failed to update Guest with ID {id}");
            }
        }

        [Authorize(Roles = Roles.Guest + "," + Roles.Administrator)]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteGuest(string id)
        {
            try
            {
                var Guest = await _guestService.GetByIdAsync(id);
                if (Guest == null)
                    return NotFound();

                await _guestService.RemoveAsync(id);

                return Ok("Guest deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete Guest with ID {0}", id);
                return StatusCode(500, $"Failed to delete Guest with ID {id}");
            }
        }
    }
}
