using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesorController : ControllerBase
    {
        private readonly IProfesor _profesorRepo;

        public ProfesorController(IProfesor profesorRepo)
        {
            _profesorRepo = profesorRepo;
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProfesorCreateDto dto)
        {
            var result = await _profesorRepo.CrearProfesorAsync(dto);
            return Ok(result);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id:long}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var result = await _profesorRepo.ObtenerProfesorPorIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var list = await _profesorRepo.ObtenerTodosProfesoresAsync();
            return Ok(list);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ProfesorCreateDto dto)
        {
            var ok = await _profesorRepo.ActualizarProfesorAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        // =========================
        // DELETE (soft delete)
        // =========================
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var ok = await _profesorRepo.EliminarProfesorAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // =========================
        // SEARCH (autocomplete / selector)
        // =========================
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int take = 10)
        {
            var result = await _profesorRepo.SearchAsync(q, take);
            return Ok(result);
        }
    }
}
