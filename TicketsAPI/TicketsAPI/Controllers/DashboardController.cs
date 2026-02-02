using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using TicketsAPI.Services;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private readonly IDashboardService _service;
        private readonly ApplicationDbContext _context;

        public DashboardController(IDashboardService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("ResumenVigente")]
        public async Task<ActionResult<DashboardResumenDto>> ResumenVigente(CancellationToken ct)
        {
            var anioLectivoId = await _context.AnioLectivo
                .AsNoTracking()
                .Where(x => x.IsActive == true && x.Vigente == true)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (anioLectivoId <= 0) return BadRequest("No existe año lectivo vigente.");

            var result = await _service.GetResumenAsync(anioLectivoId, ct);
            return Ok(result);
        }



    }
}
