using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using System.Security.Policy;
using System.Text;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _userService;
        private UserManager<User> _userManager;

        public AccountController(IAccount context, UserManager<User> userManager)
        {
            _userService = context;
            _userManager = userManager;
        }
        [HttpPost("Student/Register")] //Register
        public async Task<ActionResult<DtoUserResponse>> RegisterStudent(DtoUserRegisterRequest registerDto)
        {
            var user = await _userService.Register(registerDto, this.ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (user == null) return Unauthorized();

            return Ok($"{user.Username} registered successfully.");
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("Teacher/Register")] //Register
        public async Task<ActionResult<DtoUserResponse>> RegisterTeacher(DtoUserRegisterRequest registerDto)
        {         
             var user = await _userService.Register(registerDto, this.ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (user == null) return Unauthorized();

            return Ok($"{user.Username} registered successfully.");
        }

        [HttpPost("Login")] //Login
        public async Task<ActionResult<DtoUserResponse>> Login(DtoUserLoginRequest loginDto)
        {
            var user = await _userService.Login(loginDto);
            if (user == null) return Unauthorized("Invalid username or password.");

            if (user.Message=="Email not confirmed")
                return Unauthorized("Email not confirmed. Please check your email for the confirmation link.");
            
            if (user.Roles != null && user.Roles.Contains("Admin"))
                return Unauthorized();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Set cookie expiration
            };
            var cookieOptionsRefresh = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Set cookie expiration
            };
            Response.Cookies.Append("AuthToken", user.AccessToken, cookieOptions);
            Response.Cookies.Append("RefreshToken", user.RefreshToken, cookieOptionsRefresh);

            return Ok(user);
        }

        [Authorize(Roles = "Admin, Student, Teacher")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout(User);
            Response.Cookies.Delete("AuthToken");
            return Ok(new { message = "Successfully logged out" });
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<ActionResult<DtoUserResponse>> Refresh(TokenDto tokenDto)
        {
            try
            {
                var result = await _userService.RefreshToken(tokenDto);
                return Ok(result);
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordReqDTO forgotPasswordDto)
        {
            var result = await _userService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result)
            {
                return BadRequest("Failed to send reset email.");
            }

            return Ok("Password reset link sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordReqDTO resetPasswordDto)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordDto);
            if (!result)
            {
                return BadRequest("Error resetting password.");
            }

            return Ok("Password reset successfully.");
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            //return Ok(new
            //{
            //    Email = email,
            //    Token = token
            //});
            //return Ok("done successfully");
            var res = new ResetPasswordResDTO
            {
                Email = email,
                Token = token
            };

            return Ok(new
            {
                res,
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Data are empty or null");
            }

            var result = await _userService.ConfirmEmailAsync(email, code);
            // Log detailed result information
            if (result)
                return Ok("Email confirmed successfully. You can now log in.");

            return BadRequest("Error confirming email.");
        }

        [Authorize]
        [HttpGet("Validate")]
        public IActionResult Validate()
        {
            return Ok(new { message = "User is authenticated" });
        }
    }
}
