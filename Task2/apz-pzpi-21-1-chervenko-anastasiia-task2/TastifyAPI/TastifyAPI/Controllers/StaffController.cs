using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using TastifyAPI.Services;
using TastifyAPI.Helpers;
using TastifyAPI.DTOs.Features_DTOs;

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _staffService;
        private readonly ILogger<StaffController> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Staff> _passwordHasher;
        private readonly JwtService _jwtService;

        public StaffController(
            StaffService staffService,
            ILogger<StaffController> logger,
            IMapper mapper,
            IPasswordHasher<Staff> passwordHasher,
            IConfiguration config)
        {
            _staffService = staffService;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = new JwtService(config);
        }

        // GET api/StaffController/all-staff
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("all-staff")]
        public async Task<ActionResult<List<StaffDto>>> Get()
        {
            try
            {
                var staffList = await _staffService.GetAsync();
                var staffDtoList = _mapper.Map<List<StaffDto>>(staffList);
                return Ok(staffDtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all staff");
                return StatusCode(500, "Failed to get all staff");
            }
        }

        // GET api/staff-profile/5
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("staff-profile/{id:length(24)}")]
        public async Task<ActionResult<StaffDto>> GetById(string id)
        {
            try
            {
                var staff = await _staffService.GetByIdAsync(id);
                if (staff == null)
                    return NotFound();

                var staffDto = _mapper.Map<StaffDto>(staff);
                return Ok(staffDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get staff with ID {0}", id);
                return StatusCode(500, $"Failed to get staff with ID {id}");
            }
        }

        // POST api/StaffController/register
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("staff-register")]
        public async Task<ActionResult> Register(StaffRegistrationDto staffRegistrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _staffService.AnyAsync(s => s.Login == staffRegistrationDto.Login))
                {
                    return BadRequest("Staff with such login already exists");
                }

                var newStaff = new Staff
                {
                    Name = staffRegistrationDto.Name,
                    Position = staffRegistrationDto.Position,
                    HourlySalary = staffRegistrationDto.HourlySalary,
                    Phone = staffRegistrationDto.Phone,
                    AttendanceCard = 0,
                    Login = staffRegistrationDto.Login
                };

                newStaff.PasswordHash = _passwordHasher.HashPassword(newStaff, staffRegistrationDto.Password);

                await _staffService.CreateAsync(newStaff);

                return Ok("New staff registration was successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during staff registration");
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/StaffController/login
        [HttpPost("staff-login")]
        public async Task<IActionResult> Login(StaffLoginDto staffLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var staff = await _staffService.GetByLoginAsync(staffLoginDto.Login);

                if (staff == null)
                {
                    return BadRequest("Staff with such login does not exist");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(staff, staff.PasswordHash, staffLoginDto.Password);

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return BadRequest("Invalid login or password");
                }

                var token = _jwtService.GenerateStaffToken(staff);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/StaffController/update-staff-profile/5
        [Authorize(Roles = Roles.Administrator)]
        [HttpPut("update-staff-profile/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, StaffDto staffDto)
        {
            try
            {
                var existingStaff = await _staffService.GetByIdAsync(id);
                if (existingStaff == null)
                    return NotFound();

                staffDto.Id = id;
                _mapper.Map(staffDto, existingStaff);

                await _staffService.UpdateAsync(id, existingStaff);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update staff with ID {0}", id);
                return StatusCode(500, $"Failed to update staff with ID {id}");
            }
        }

        // DELETE api/StaffController/delete-staff-profile/5
        [Authorize(Roles = Roles.Administrator)]
        [HttpDelete("delete-staff-profile/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var staff = await _staffService.GetByIdAsync(id);
                if (staff == null)
                    return NotFound();

                await _staffService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete staff with ID {0}", id);
                return StatusCode(500, $"Failed to delete staff with ID {id}");
            }
        }
    }
}
