using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradoParaleloController : ControllerBase
    {

        private readonly IGradoParalelo _gradoParalelo;

        public GradoParaleloController(IGradoParalelo gradoParalelo)
        {
            _gradoParalelo = gradoParalelo;
        }

        // GET: api/GradoParalelo/Disponibles?anioLectivoId=1&gradoId=2
        [HttpGet("Disponibles")]
        public async Task<IActionResult> Disponibles([FromQuery] long anioLectivoId, [FromQuery] long gradoId)
        {
            if (anioLectivoId <= 0 || gradoId <= 0)
                return BadRequest("anioLectivoId y gradoId son obligatorios.");

            var data = await _gradoParalelo.GetDisponibles(anioLectivoId, gradoId);
            return Ok(data);
        }


        // GET: api/GradoParalelo/Disponibles?anioLectivoId=1&gradoId=2
        [HttpGet("GetCuposDisponibles")]
        public async Task<IActionResult> GetCuposDisponibles(long idEstudiante)
        {
           

            var data = await _gradoParalelo.GetCuposDisponibles(idEstudiante);

            return Ok(data);
        }



        [HttpGet("SelectorGradoParalelo")]
        public async Task<IActionResult> SelectorGradoParalelo(long idAnioLectivo)
        {


            var data = await _gradoParalelo.SelectorGradoParalelo(idAnioLectivo);

            return Ok(data);
        }

        // POST: api/GradoParalelo/Crear
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] CreateGradoParaleloDto dto)
        {
            if (dto.GradoId <= 0 || dto.ParaleloId <= 0 || dto.AnioLectivoId <= 0)
                return BadRequest("Grado, Paralelo y Año Lectivo son obligatorios.");

            try
            {
                var id = await _gradoParalelo.CrearOfertaAsync(dto);
                return Ok(new
                {
                    message = "Oferta creada correctamente",
                    gradoParaleloId = id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }







    }
}
