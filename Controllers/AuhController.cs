using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThanhBuoiAPI.Services;
using ThanhBuoiAPI.Models;
using ThanhBuoiAPI.Models.DTO;

namespace ThanhBuoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly JWTService _jwtService;
        private readonly SignInManager<TaiKhoan> _signInManager;

        public AuthController(UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager, JWTService jWTService)
        {
            _userManager = userManager;
            _jwtService = jWTService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                string token = _jwtService.GenerateToken(user.Id, user.Email, roles);

                var response = new { accessToken = token };
                return Ok(response);
            }
            return BadRequest("Invalid email or password");
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists");
            }

            var newUser = new TaiKhoan
            {
                Email = model.Email,
                Ten = model.Ten,
                UserName = model.Email,
                PhoneNumber = model.SDT,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            await _userManager.AddToRoleAsync(newUser, "USER");
            if (result.Succeeded)
            {
                return Ok("User registered successfully");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful");
        }

        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }

            return BadRequest(result.Errors);
        }
    }
}
