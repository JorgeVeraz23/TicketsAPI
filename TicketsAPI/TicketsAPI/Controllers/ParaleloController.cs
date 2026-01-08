using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class ParaleloController : ControllerBase
    {
        private readonly IParalelo _paraleloRepository;
        public ParaleloController(IParalelo paralelo)
        {
            _paraleloRepository = paralelo;
        }


        // Crear paralelo
        [HttpPost("CrearParalelo")]
        public async Task<IActionResult> CrearParalelo([FromBody] ParaleloDTO paraleloDto)
        {
            var paralelo = await _paraleloRepository.CrearParaleloAsync(paraleloDto);
            return Ok(paralelo);
        }

        // Obtener paralelo por ID
        [HttpGet("ObtenerParaleloPorId")]
        public async Task<IActionResult> ObtenerParaleloPorId(long id)
        {
            var paralelo = await _paraleloRepository.ObtenerParaleloPorIdAsync(id);
            if (paralelo == null)
            {
                return NotFound($"Paralelo con ID {id} no encontrado.");
            }
            return Ok(paralelo);
        }

 
        [HttpGet("GetAllParalelos")]
        public async Task<IActionResult> ObtenerParalelos()
        {
            var paralelos = await _paraleloRepository.ObtenerParalelosAsync();
            return Ok(paralelos);
        }

        [HttpGet("SelectorParalelos")]
        public async Task<IActionResult> SelectorParalelos()
        {
            var paralelos = await _paraleloRepository.SelectorParalelo();
            return Ok(paralelos);
        }


        // Actualizar paralelo
        [HttpPut("ActualizarParalelo/{id:long}")]
        public async Task<IActionResult> ActualizarParalelo(long id, [FromBody] ParaleloDTO paraleloDto)
        { 
            var result = await _paraleloRepository.ActualizarParaleloAsync(id, paraleloDto);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // Eliminar paralelo
        [HttpDelete("EliminarParalelo")]
        public async Task<IActionResult> EliminarParalelo(long id)
        {
            var result = await _paraleloRepository.EliminarParaleloAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
