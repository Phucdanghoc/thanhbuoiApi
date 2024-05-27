using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThanhBuoiAPI.Data;
using ThanhBuoiAPI.Models;

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
        public IEnumerable<Chuyen> Get(int page = 1, string searchString = null)
        {
            var query = _context.Chuyens
                .Include(x => x.Xe)
                .OrderByDescending(c => c.ThoiGianDi)
                .AsQueryable()
                .Where(n => n.ThoiGianDi > DateTime.Now);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Ten.Contains(searchString));
            }

            int startIndex = (page - 1) * PageSize;
            var listChuyen = query.Skip(startIndex).Take(PageSize).ToList();

            return listChuyen;
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
                 .Include(ve => ve.Ves)
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
            var ve = await _context.Ves
                .Include(a => a.Ghe)
                .Where(v => v.Chuyen.Id == id && v.TrangThai == TrangThaiVe.Empty)
                .ToListAsync();

            if (!ve.Any())
            {
                return NotFound();
            }

            return Ok(ve);
        }

    }
}
