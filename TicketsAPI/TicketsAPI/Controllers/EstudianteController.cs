using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {

        private IEstudiante _estudiante;

        public EstudianteController(IEstudiante estudiante)
        {
            _estudiante = estudiante;
        }


        [HttpGet("GetAllEstudiante")]
        public async  Task<IActionResult> GetAllEstudiantes()
        {

            var response = await _estudiante.GetAllEstudiante();

            return Ok(response);
        }


        [HttpGet("GetEstudianteById")]
        public async Task<IActionResult> GetEstudianteById(long idEstudiante)
        {

            var response = await _estudiante.GetEstudianteById(idEstudiante);

            return Ok(response);
        }


        [HttpPost("CrearEstudiante")]
        public async Task<IActionResult> CrearEstudiante(EstudianteDTO estudiante)
        {

            var response = await _estudiante.CrearEstudiante(estudiante);

            return Ok(response);
        }

        [HttpPut("ActualizarEstudiante")]
        public async Task<IActionResult> EditarEstudiante(EstudianteDTO estudianteDTO)
        {
            var response = await _estudiante.EditarEstudiante(estudianteDTO);

            return Ok(response);

        }

        [HttpDelete("EliminarEstudiante")] 
        public async Task<IActionResult> EliminarEstudiante(long idEstudiante)
        {
            var response = await _estudiante.EliminarEstudiante(idEstudiante);

            return Ok(response);


        }
    }
}
