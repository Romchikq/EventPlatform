using EventPlatform.DTOs;
using EventPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            try
            {
                var user = await _authService.Register(userDto);
                return Ok(new { message = "User registered successfully. Please check your email for confirmation." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {
            try
            {
                var token = await _authService.Login(userDto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            var result = await _authService.ConfirmEmail(token);
            return result ? Ok(new { message = "Email confirmed successfully" }) : BadRequest(new { message = "Invalid or expired token" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.RequestPasswordReset(forgotPasswordDto.Email);
            return result ? Ok(new { message = "Password reset email sent" }) : BadRequest(new { message = "Email not found" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetDto)
        {
            var result = await _authService.ResetPassword(resetDto);
            return result ? Ok(new { message = "Password reset successfully" }) : BadRequest(new { message = "Invalid or expired token" });
        }
    }
}