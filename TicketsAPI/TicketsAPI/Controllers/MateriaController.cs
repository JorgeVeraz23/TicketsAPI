using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

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

        [HttpGet("GetAllMaterias")]
        public async Task<IActionResult> GetAllMaterias()
        {
            var response = await _materia.GetAllMaterias();

            return Ok(response);
        }


        [HttpGet("GetMateria")]
        public async Task<IActionResult> GetMateria(long id)
        {
            var response = await _materia.GetMateria(id);

            return Ok(response);

        }

        [HttpPost("CrearMateria")]
        public async Task<IActionResult> CrearMateria(MateriaDTO materiaDTO)
        {
            var response = await _materia.CrearMateria(materiaDTO);

            return Ok(response);
        }

        [HttpPut("ActualizarMateria")]
        public async Task<IActionResult> ActualizarMateria(MateriaDTO materiaDTO)
        {
            var response = await _materia.EditarMateria(materiaDTO);

            return Ok(response);

        }

        [HttpDelete("EliminarMateria")]
        public async Task<IActionResult> EliminarMateria(long idMateria)
        {
            var response = await _materia.EliminarMateria(idMateria);

            return Ok(response);
        }
    }
}
