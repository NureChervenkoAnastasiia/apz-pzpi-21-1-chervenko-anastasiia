using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;
using TastifyAPI.Services;

//TODO: /login

namespace TastifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        /*private readonly StaffService _staffService;
        private readonly ILogger<StaffController> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Staff> _passwordHasher;
        private readonly JwtService _jwtService;

        public StaffController(StaffService staffService, ILogger<StaffController> logger,
            IMapper mapper, IPasswordHasher<Staff> passwordHasher,JwtService jwtService)
        {
            _staffService = staffService;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffDto>>> Get()
        {
            try
            {
                var staff = await _staffService.GetAsync();
                var staffDtos = _mapper.Map<List<StaffDto>>(staff);
                return Ok(staffDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all staff");
                return StatusCode(500, "Failed to get all staff");
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<StaffDto>> Get(string id)
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

        [HttpPost("create")]
        public async Task<ActionResult<StaffDto>> CreateNewStaff(StaffCreateDto createDto)
        {
            try
            {
                var staff = _mapper.Map<Staff>(createDto);

                staff.PasswordHash = _passwordHasher.HashPassword(staff, createDto.PasswordHash);

                await _staffService.CreateAsync(staff);

                var createdStaffDto = _mapper.Map<StaffDto>(staff);
                return CreatedAtAction(nameof(Get), new { id = createdStaffDto.Id }, createdStaffDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new staff");
                return StatusCode(500, "Failed to create new staff");
            }
        }


        [HttpDelete("delete/{id:length(24)}")]
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

        /*
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(StaffLoginDto loginDto)
        {
            try
            {
                // Получаем работника по логину
                var staff = await _staffService.GetByLoginAsync(loginDto.Login);

                // Проверяем, найден ли работник и соответствует ли пароль
                if (staff == null || !_passwordHasher.VerifyHashedPassword(staff, staff.PasswordHash, loginDto.PasswordHash).Equals(PasswordVerificationResult.Success))
                    return Unauthorized();

                // Генерируем JWT токен для работника
                var token = _jwtService.GenerateToken(staff.Id);

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to login staff");
                return StatusCode(500, "Failed to login staff");
            }
        }
        */
    }
}
