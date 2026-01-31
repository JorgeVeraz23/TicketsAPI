using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentanteController : ControllerBase
    {
        private readonly IRepresentanteService _service;
        public RepresentanteController(IRepresentanteService service) => _service = service;

        private string UserName => User?.Identity?.Name ?? "system";

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? q, CancellationToken ct)
            => Ok(await _service.ListAsync(q, ct));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id, CancellationToken ct)
            => Ok(await _service.GetAsync(id, ct));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RepresentanteCreateDto dto, CancellationToken ct)
        {
            var id = await _service.CreateAsync(dto, UserName, ct);
            return Ok(new { id });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] RepresentanteUpdateDto dto, CancellationToken ct)
        {
            await _service.UpdateAsync(id, dto, UserName, ct);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, UserName, ct);
            return NoContent();
        }


        [HttpGet("selector")]
        public async Task<IActionResult> Selector()
        {
            var result = await _service.SelectorRepresentanteAsync();
            return Ok(result);
        }

    }
}
