using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAccount _userService;
        public AdminController(IAccount context)
        {
            _userService = context;
        }

        [HttpPost("Register")] //Register
        public async Task<ActionResult<DtoUserResponse>> Register(DtoAdminRegisterRequest registerDto)
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
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout(User);
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
        }


    }
}
