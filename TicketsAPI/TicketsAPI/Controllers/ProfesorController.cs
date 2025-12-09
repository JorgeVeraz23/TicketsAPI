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

        private readonly IProfesor _profesor;

        public ProfesorController(IProfesor profesor)
        {
            _profesor = profesor;
        }

        [HttpGet("GetAllProfesores")]
        public async Task<IActionResult> GetAllProfesores()
        {
            var response = await _profesor.GetAllProfesor();

            return Ok(response);
        }

        [HttpGet("GetProfesorById")]
        public async Task<IActionResult> GetProfesorById(long idProfesor)
        {
            var response = await _profesor.GetProfesorById(idProfesor);

            return Ok(response);
        }

        [HttpPost("CrearProfesor")]
        public async Task<IActionResult> CrearProfesor(ProfesorDTO profesorDTO)
        {
            var response = await _profesor.CrearProfesor(profesorDTO);

            return Ok(response);

        }

        [HttpPut("ActualizarProfesor")]
        public async Task<IActionResult> ActualizarProfesor(ProfesorDTO profesorDTO)
        {
            var response = await _profesor.EditarProfesor(profesorDTO);
            return Ok(response);

        }

        [HttpDelete("EliminarProfesor")]
        public async Task<IActionResult> EliminarProfesor(long idProfesor)
        {
            var response = await _profesor.EliminarProfesor(idProfesor);

            return Ok(response);
        }

    }
}
