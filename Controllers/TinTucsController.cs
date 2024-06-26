using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThanhBuoiAPI.Data;
using ThanhBuoiAPI.Models;

namespace ThanhBuoiAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TinTucsController : ControllerBase
    {
        private readonly DataContext _context;

        public TinTucsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/TinTucs
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tintucs = await _context.TinTucs.ToListAsync();
            var jsonResult = JsonConvert.SerializeObject(tintucs, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Ok(jsonResult);
        }
    }
}
