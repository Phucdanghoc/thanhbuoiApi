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
using Newtonsoft.Json;


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
        private readonly IEmailService _emailService;
        public VeController(JWTService jwtService,DataContext dataContext, IEmailService emailService)
        {
            _jwtService = jwtService;
            _context = dataContext;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string userId = _jwtService.GetUserIdFromAuthorizationHeader(HttpContext);
                var veList = _context.Ves
                    .Include(g => g.Ghe).ThenInclude(h => h.Hang)
                    .Include(c => c.Chuyen).ThenInclude(x => x.Xe).ThenInclude(l => l.LoaiXe)
                    .Where(t => t.TaiKhoan.Id == userId && t.TrangThai == TrangThaiVe.Booked)
                    .ToList();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var jsonResult = JsonConvert.SerializeObject(veList, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                return Ok(jsonResult);
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
        public async Task<ActionResult<IEnumerable<Ve>>> Create([FromBody] BookingRequest bookingRequest)
        {
            try
            {
                string userId = _jwtService.GetUserIdFromAuthorizationHeader(HttpContext);
                List<Ve> createdTickets = new List<Ve>();

                foreach (var veDTO in bookingRequest.Bookings)
                {
                    Ve? ve = await _context.Ves
                        .Include(c => c.Chuyen)
                        .ThenInclude(x => x.Xe).ThenInclude(x => x.LoaiXe)
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
                    ve.Hanhli = veDTO.HanhLi;
                    ve.NgayTao = DateTime.Now;
                    ve.email = bookingRequest.Email;
                    ve.phuongthucthanhtoan = bookingRequest.Payment;
                    if (ve.Hanhli > 20)
                    {
                        ve.Tien = (double)(ve.Chuyen.Gia +
                                          Math.Round((ve.Hanhli - 20) * 0.1 * ve.Chuyen.Gia) +
                                          (ve.Chuyen.Gia * ve.Chuyen.GiaTang));
                    }
                    else
                    {
                        ve.Tien = (double)(ve.Chuyen.Gia +
                                            ve.Chuyen.Gia * ve.Chuyen.GiaTang);
                    }
             
                    _context.Chuyens.Update(ve.Chuyen);
                    _context.Ves.Update(ve);
                    createdTickets.Add(ve);
                }
                if (bookingRequest.Email != "")
                {
                    var emailBody = _emailService.makeBodyTicketBooked(createdTickets);
                    await _emailService.SendEmailAsync(bookingRequest.Email, "Xác nhận vé xe", emailBody);
                }
                await _context.SaveChangesAsync();
                return Ok(createdTickets);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


        [HttpDelete]
        [Route("Cancel")]
        public async Task<ActionResult> Cancel([FromBody] int id)
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
                ve.TrangThai = TrangThaiVe.Empty;
                ve.Ten = null;
                ve.CMND = null;
                ve.Sdt = null;
                ve.MaVe = null;
                ve.TaiKhoan = null;
                ve.Hanhli = 0;
                ve.Tien = 0;
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
