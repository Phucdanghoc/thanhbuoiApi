using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThanhBuoiAPI.Services;
using ThanhBuoiAPI.Models;
using ThanhBuoiAPI.Models.DTO;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;

namespace ThanhBuoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly JWTService _jwtService;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly IEmailService _emailService;


        public AuthController(UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager, JWTService jWTService, IEmailService emailService)
        {
            _userManager = userManager;
            _jwtService = jWTService;
            _signInManager = signInManager;
            _emailService = emailService;
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
        public async Task<ActionResult> Register([FromBody] RegisterDTO model, [FromQuery] string returnUrl = null)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Ten) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.SDT))
            {
                return BadRequest("All fields are required.");
            }

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
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            await _userManager.AddToRoleAsync(newUser, "USER");
            if (result.Succeeded)
            {

                var userId = await _userManager.GetUserIdAsync(newUser);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var confirmationLink = $"https://localhost:7000/api/Auth/ConfirmEmail?userId={userId}&token={encodedToken}&returnUrl={HtmlEncoder.Default.Encode(returnUrl ?? "/")}";

                await _emailService.SendEmailAsync(model.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

                return Ok("User registered successfully");
            }

            return BadRequest(result.Errors);
        }


        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Token không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return Ok("Đã xác thực email .");
            }

            return BadRequest("Lỗi xác thực email.");
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
