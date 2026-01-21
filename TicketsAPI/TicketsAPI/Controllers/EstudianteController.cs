using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        [HttpPost("CrearEstudiante")]
        public async Task<IActionResult> CrearEstudiante([FromBody] EstudianteCreateDto estudianteDto)
        {
            try
            {
                var estudiante = await _estudianteRepository.CrearEstudianteAsync(estudianteDto);
                return Ok(estudiante);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el estudiante: {ex.Message}");
            }
        }

        [HttpGet("ObtenerEstudiante")]
        public async Task<IActionResult> ObtenerEstudiantePorId(long id)
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

        [HttpGet("GetAllEstudiantes")]
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

        [HttpPut("ActualizarEstudiante")]
        public async Task<IActionResult> ActualizarEstudiante(long id, [FromBody] EstudianteCreateDto estudianteDto)
        {
            try
            {
                var result = await _estudianteRepository.ActualizarEstudianteAsync(id, estudianteDto);
                if (!result)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el estudiante: {ex.Message}");
            }
        }

        [HttpDelete("EliminarEstudiante")]
        public async Task<IActionResult> EliminarEstudiante(long id)
        {
            try
            {
                var result = await _estudianteRepository.EliminarEstudianteAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el estudiante: {ex.Message}");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int take = 10)
        {
            var result = await _estudianteRepository.SearchAsync(query, take);
            return Ok(result);
        }

        [HttpGet("SelectorEstudiante")]
        public async Task<IActionResult> SelectorEstudiante()
        {
            var result = await _estudianteRepository.SelectorEstudiante();
            return Ok(result);
        }

    }
}
