using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using static TicketsAPI.Interfaces.IEstudiante;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {
        private readonly IEstudiante _estudianteRepository;

        public EstudianteController(IEstudiante estudianteRepository)
        {
            _estudianteRepository = estudianteRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CrearEstudiante([FromBody] EstudianteDTO estudianteDto)
        {
            try
            {
                var estudiante = await _estudianteRepository.CrearEstudianteAsync(estudianteDto);
                return CreatedAtAction(nameof(ObtenerEstudiantePorId), new { id = estudiante.Id }, estudiante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el estudiante: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEstudiantePorId(int id)
        {
            try
            {
                var estudiante = await _estudianteRepository.ObtenerEstudiantePorIdAsync(id);
                if (estudiante == null)
                {
                    return NotFound($"Estudiante con ID {id} no encontrado.");
                }
                return Ok(estudiante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el estudiante con ID {id}: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosEstudiantes()
        {
            try
            {
                var estudiantes = await _estudianteRepository.ObtenerTodosEstudiantesAsync();
                return Ok(estudiantes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener todos los estudiantes: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarEstudiante(int id, [FromBody] EstudianteDTO estudianteDto)
        {
            try
            {
                var result = await _estudianteRepository.ActualizarEstudianteAsync(id, estudianteDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el estudiante: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEstudiante(int id)
        {
            try
            {
                var result = await _estudianteRepository.EliminarEstudianteAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el estudiante: {ex.Message}");
            }
        }
    }
}
