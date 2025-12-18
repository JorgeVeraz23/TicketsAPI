using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaController : ControllerBase
    {
        private readonly IMateria _materia;
        public MateriaController(IMateria materia)
        {
            _materia = materia;
        }
        // Crear materia
        [HttpPost]
        public async Task<IActionResult> CrearMateria([FromBody] MateriaDTO materiaDto)
        {
            try
            {
                var materia = await _materia.CrearMateriaAsync(materiaDto);
                return CreatedAtAction(nameof(ObtenerMateriaPorId), new { id = materia.Id }, materia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la materia: {ex.Message}");
            }
        }

        // Obtener materia por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerMateriaPorId(int id)
        {
            try
            {
                var materia = await _materia.ObtenerMateriaPorIdAsync(id);
                if (materia == null)
                {
                    return NotFound($"Materia con ID {id} no encontrada.");
                }
                return Ok(materia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la materia con ID {id}: {ex.Message}");
            }
        }

        // Obtener todas las materias de un grado
        [HttpGet("grado/{gradoId}")]
        public async Task<IActionResult> ObtenerMateriasPorGrado(int gradoId)
        {
            try
            {
                var materias = await _materia.ObtenerMateriasPorGradoAsync(gradoId);
                return Ok(materias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las materias del grado {gradoId}: {ex.Message}");
            }
        }

        // Actualizar materia
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarMateria(int id, [FromBody] MateriaDTO materiaDto)
        {
            try
            {
                var result = await _materia.ActualizarMateriaAsync(id, materiaDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la materia: {ex.Message}");
            }
        }

        // Eliminar materia
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMateria(int id)
        {
            try
            {
                var result = await _materia.EliminarMateriaAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la materia: {ex.Message}");
            }
        }
    }
}
