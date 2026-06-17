using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.Services.Implementations;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Registers a new student account.
        /// </summary>
        [HttpPost("register/student")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterDto dto)
        {
            if (await _authService.UserExistAsync(dto.Username))
                return BadRequest("Username already exists.");
            var result = await _authService.RegisterStudentAsync(dto);
            if (!result)
                return BadRequest("Registration failed.");
            return Ok("User registered successfully.");
        }

        /// <summary>
        /// Registers a new teacher account.
        /// </summary>
        [HttpPost("register/teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterDto dto)
        {
            if (await _authService.UserExistAsync(dto.Username))
                return BadRequest("Username already exists.");
            var result = await _authService.RegisterTeacherAsync(dto);
            if (!result)
                return BadRequest("Registration failed.");
            return Ok("User registered successfully.");
        }

        /// <summary>
        /// Registers a new admin account (Admin only).
        /// </summary>
        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto dto)
        {
            if (await _authService.UserExistAsync(dto.Username))
                return BadRequest("Username already exists.");
            var result = await _authService.RegisterAdminAsync(dto);
            if (!result)
                return BadRequest("Registration failed.");
            return Ok("Admin registered successfully.");
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <returns>A message and the access token if authentication is successful.</returns>
        /// <response code="200">Returns the success message and the JWT access token.</response>
        /// <response code="401">If the username or password is incorrect.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.CheckLoginAsync(dto);

            if (token == null)
                return Unauthorized("Username or Password is incorrect!");

            return Ok(new
            {
                Message = "Login Successful",
                AccessToken = token
            });
        }
    }
}
