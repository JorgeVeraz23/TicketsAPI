using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionesController : ControllerBase
    {
        private readonly ICalificacion _repo;

        public CalificacionesController(ICalificacion repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CalificacionCreateDto dto)
            => Ok(await _repo.CrearAsync(dto));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var item = await _repo.ObtenerPorIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] string? periodo, [FromQuery] string? materia)
            => Ok(await _repo.ListarAsync(periodo, materia));

        [HttpGet("estudiante/{estudianteId:long}")]
        public async Task<IActionResult> PorEstudiante(long estudianteId, [FromQuery] string? periodo)
            => Ok(await _repo.ListarPorEstudianteAsync(estudianteId, periodo));

        [HttpGet("profesor/{profesorId:long}")]
        public async Task<IActionResult> PorProfesor(long profesorId, [FromQuery] string? periodo)
            => Ok(await _repo.ListarPorProfesorAsync(profesorId, periodo));

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] CalificacionCreateDto dto)
        {
            var ok = await _repo.ActualizarAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var ok = await _repo.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
