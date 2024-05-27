using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using ThanhBuoiAPI.Data;
using ThanhBuoiAPI.Models;
using ThanhBuoiAPI.Services;
using ThanhBuoiAPI.Models.DTO;


namespace ThanhBuoiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VeController : ControllerBase
    {
        // GET: api/<ValuesController>
        private readonly JWTService _jwtService;
        private readonly DataContext _context;
        public VeController(JWTService jwtService,DataContext dataContext)
        {
            _jwtService = jwtService;
            _context = dataContext;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string userId = _jwtService.GetUserIdFromAuthorizationHeader(HttpContext);
                var veList = _context.Ves
                    .Include(g => g.Ghe)
                    .Include(c => c.Chuyen)
                    .Where(t => t.TaiKhoan.Id == userId)
                    .ToList();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var jsonResult = JsonSerializer.Serialize(veList, options);
                return Content(jsonResult, "application/json");
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<Ve>> Create([FromBody] VeDTO veDTO)
        {
            try
            {
                string userId = _jwtService.GetUserIdFromAuthorizationHeader(HttpContext);
                Ve? ve = await _context.Ves
                    .Include(g => g.Ghe)
                    .Include(c => c.Chuyen)
                    .ThenInclude(x => x.Xe)
                    .FirstOrDefaultAsync(v => v.Id == veDTO.Id);

                if (ve == null)
                {
                    return NotFound($"Ticket with ID {veDTO.Id} not found.");
                }

                ve.Ten = veDTO.Ten;
                ve.CMND = veDTO.CMND;
                ve.Sdt = veDTO.SDT;
                ve.MaVe = $"TB-{ve.Chuyen.ThoiGianDi.Day}-{ve.Chuyen.Xe.MaXe}{veDTO.Id}";
                ve.TrangThai = TrangThaiVe.Booked;
                ve.TaiKhoan = await _context.Users.FindAsync(userId);
                ve.Ghe.KhoangTrong = false;
                ve.Hanhli = veDTO.HanhLi;

                if (ve.Hanhli > 20)
                {
                    ve.Tien = ve.Chuyen.Gia + Math.Round((ve.Hanhli - 20) * 0.5);
                }
                else
                {
                    ve.Tien = ve.Chuyen.Gia;
                }
                _context.Chuyens.Update(ve.Chuyen);
                _context.Ves.Update(ve);
                await _context.SaveChangesAsync();
                ve.TaiKhoan = null;
                return Ok(ve);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }

        [HttpPost]
        [Route("Cancel/{id}")]
        public async Task<ActionResult> Cancel(int id)
        {
            string userId = _jwtService.GetUserIdFromAuthorizationHeader(HttpContext);
            Ve? ve = await _context.Ves
                                   .Include(g => g.Ghe).Where(t => t.TaiKhoan.Id == userId)
                                   .FirstOrDefaultAsync(v => v.Id == id);
            if (ve == null)
            {
                return NotFound(new { Message = "Không tìm thấy vé để hủy." });
            }

            try
            {
                ve.TrangThai = TrangThaiVe.Cancel;
                ve.Ten = null;
                ve.CMND = null;
                ve.Sdt = null;
                ve.MaVe = null;
                ve.TaiKhoan = null;
                ve.Hanhli = 0;
                ve.Tien = 0;
                ve.Ghe.KhoangTrong = true;
                _context.Ghes.Update(ve.Ghe);
                _context.Ves.Update(ve);
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"Đã hủy vé thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Đã xảy ra lỗi khi hủy vé: {ex.Message}" });
            }
        }

        // PUT api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string userId = _jwtService.GetUserIdFromAuthorizationHeader(HttpContext);
           var ve = await _context.Ves
                .Include(g => g.Ghe)
                .Include(c => c.Chuyen).ThenInclude(x => x.Xe)
                .FirstOrDefaultAsync(v => v.Id == id && v.TaiKhoan.Id == userId);

            if (ve == null)
            {
                return NotFound(new { Message = "Ticket not found." });
            }

            return Ok(ve);
        }

    }
}
