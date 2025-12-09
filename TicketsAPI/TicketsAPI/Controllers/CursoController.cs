using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursoController : ControllerBase
    {
        private readonly ICurso _cursoRepository;

        public CursoController(ICurso cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }


        [HttpGet("GetAllCursos")]
        public async Task<IActionResult> GetAllCursos()
        {
            var response = await _cursoRepository.GetAllCursos();

            return Ok(response);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetCursoById(long id)
        {
            var response = await _cursoRepository.GetCursoById(id);
            return Ok(response);
        }

        [HttpPost("CrearCurso")]
        public async Task<IActionResult> CrearCurso(CursoDTO cursoDTO)
        {
            var response = await _cursoRepository.CrearCurso(cursoDTO);
            return Ok(response);    
        }


        [HttpPut("ActualizarCurso")]
        public async Task<IActionResult> ActualizarCurso(CursoDTO cursoDTO)
        {
            var response = await _cursoRepository.EditarCurso(cursoDTO);

            return Ok(response);
        }

        [HttpDelete("EliminarCurso")]
        public async Task<IActionResult> EliminarCurso(long idCurso)
        {
            var response = await _cursoRepository.EliminarCurso(idCurso);

            return Ok(response);

        }
        
    }
}
