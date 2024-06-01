
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ThanhBuoiAPI.Data;
using ThanhBuoiAPI.Models;

namespace ThanhBuoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TinTucsController : ControllerBase
    {
        private readonly DataContext _context;

        public TinTucsController(DataContext context)
        {
            _context = context;
        }

        // GET: TinTucs
        public async Task<IActionResult> Index()
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
