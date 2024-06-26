using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;
using ThanhBuoiAPI.Data;
using ThanhBuoiAPI.Models;
using ThanhBuoiAPI.Models.DTO;

namespace ThanhBuoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChuyenController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly int PageSize = 10;

        public ChuyenController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "USER")]
        public ActionResult Get(int page = 1, string? from = null, string? to = null, DateTime? datetime = null, string? type = null)
        {
            try
            {
                var listChuyen = GetPaginatedChuyens(page, from, to, datetime, type);
                var jsonResult = JsonConvert.SerializeObject(listChuyen, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                return Ok(jsonResult);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private IEnumerable<Chuyen> GetPaginatedChuyens(int page, string from = null, string to = null, DateTime? datetime = null, string type = null)
        {
            var query = _context.Chuyens
                .Include(x => x.Xe)
                    .ThenInclude(l => l.LoaiXe)
                .Include(t => t.Tuyen)
                    .ThenInclude(t => t.DiemDi)
                .Include(t => t.Tuyen)
                    .ThenInclude(t => t.DiemDen)
                .OrderByDescending(c => c.ThoiGianDi)
                .Where(c => c.ThoiGianDi > DateTime.Now);

            // Các điều kiện lọc
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "N")
                {
                    query = query.Where(c => c.Xe.LoaiXe.LoaiGheXe == LoaiGheXe.Ngoi);
                }
                else
                {
                    query = query.Where(c => c.Xe.LoaiXe.LoaiGheXe == LoaiGheXe.GiuongNam);
                }
            }
            if (!string.IsNullOrEmpty(from))
            {
                query = query.Where(c => c.Tuyen.DiemDi.Ten.Contains(from));
            }

            if (!string.IsNullOrEmpty(to))
            {
                query = query.Where(c => c.Tuyen.DiemDen.Ten.Contains(to));
            }

            if (datetime != null)
            {
                query = query.Where(c => c.ThoiGianDi.Date == datetime.Value.Date);
            }

            // Lấy dữ liệu phân trang
            int startIndex = (page - 1) * PageSize;
            var listChuyen = query.Skip(startIndex).Take(PageSize);

            return listChuyen; // Trả về kết quả là IEnumerable<Chuyen>
        }


        // GET: api/Chuyen/5
        [HttpGet("{id}")]
        [Authorize(Roles = "USER")] 
        public async Task<ActionResult<Chuyen>> GetByIdAsync(int id)
        {
            var chuyen = await _context.Chuyens
                .Include(x => x.Xe)
                .ThenInclude(l => l.LoaiXe)
                .Include(t => t.Tuyen)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chuyen == null)
            {
                Ok(chuyen);
            }
            var jsonResult = JsonConvert.SerializeObject(chuyen, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(jsonResult, "application/json");
        }

        // POST: api/Chuyen
        [HttpPost]
        [Authorize(Roles = "USER")] 
        public async Task<ActionResult<Chuyen>> PostAsync(Chuyen chuyen)
        {
            _context.Chuyens.Add(chuyen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = chuyen.Id }, chuyen);
        }
        [HttpGet]
        [Authorize(Roles = "USER")]
        [Route("ve/{id}")]
        public async Task<ActionResult> GetVeByChuyens(int id)
        {
            var ve = await _context.Ves.Include(c => c.Chuyen)
                .Include(a => a.Ghe).ThenInclude(h => h.Hang)
                .Where(v => v.Chuyen.Id == id )
                .ToListAsync();

            if (!ve.Any())
            {
                return NotFound();
            }

            var jsonResult = JsonConvert.SerializeObject(ve, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Ok(jsonResult);
        }

        [HttpPost("search")]
        public IActionResult SearchChuyens([FromBody] SearchDTO searchModel)
        {
            var chuyens = GetPaginatedChuyens(searchModel);

            var jsonResult = JsonConvert.SerializeObject(chuyens, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Ok(jsonResult);
        }

        private IEnumerable<Chuyen> GetPaginatedChuyens(SearchDTO searchModel)
        {
            var query = _context.Chuyens
                .Include(x => x.Xe)
                    .ThenInclude(l => l.LoaiXe)
                .Include(t => t.Tuyen)
                    .ThenInclude(t => t.DiemDi)
                .Include(t => t.Tuyen)
                    .ThenInclude(t => t.DiemDen)
                .OrderByDescending(c => c.ThoiGianDi)
                .Where(c => c.ThoiGianDi > DateTime.Now)
                .AsQueryable();

            // Apply filters based on search model
            if (!string.IsNullOrEmpty(searchModel.SeatType))
            {
                query = searchModel.SeatType switch
                {
                    "Ngồi" => query.Where(c => c.Xe.LoaiXe.LoaiGheXe == LoaiGheXe.Ngoi),
                    "Giường Nằm" => query.Where(c => c.Xe.LoaiXe.LoaiGheXe == LoaiGheXe.GiuongNam),
                    _ => query,
                };
            }
            if (!string.IsNullOrEmpty(searchModel.FromLocation))
            {
                query = query.Where(c => c.Tuyen.DiemDi.Ten.Contains(searchModel.FromLocation));
            }

            if (!string.IsNullOrEmpty(searchModel.ToLocation))
            {
                query = query.Where(c => c.Tuyen.DiemDen.Ten.Contains(searchModel.ToLocation));
            }

            if (searchModel.SelectedDate != null)
            {
                query = query.Where(c => c.ThoiGianDi.Date == searchModel.SelectedDate.Value.Date);
            }

     

            var listChuyen = query.ToList();

            // Calculate total pages
         
       
            return listChuyen;
        }

    }
}
