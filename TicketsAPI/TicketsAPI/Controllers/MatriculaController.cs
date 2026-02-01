using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatricula _matriculaRepository;
        public MatriculaController(IMatricula matricula)
        {
            _matriculaRepository = matricula;
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] CrearMatriculaDto dto)
        {
            try
            {
                var result = await _matriculaRepository.CrearMatriculaAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? periodo)
        {
            try
            {
                var result = await _matriculaRepository.GetAll(periodo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }





    }
}
