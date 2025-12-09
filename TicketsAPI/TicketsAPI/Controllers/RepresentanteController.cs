using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentanteController : ControllerBase
    {

        private readonly IRepresentate _representante;

        public RepresentanteController(IRepresentate representante)
        {
            _representante = representante;
        }

        [HttpPost("CrearRepresentante")]
        public async Task<IActionResult> CrearRepresentante(RepresentanteDTO representanteDTO)
        {
            var response = await _representante.CrearRepresentate(representanteDTO);

            return Ok(response);
        }

        [HttpDelete("EliminarRepresentante")]
        public async Task<IActionResult> EliminarRepresentante(long idReprensentante)
        {
            var response = await _representante.DeleteRepresentate(idReprensentante);

            return Ok(response);
        }

        [HttpPut("ActualizarRepresentante")]
        public async Task<IActionResult> ActualizarRepresentante(RepresentanteDTO representanteDTO)
        {

            var response = await _representante.EditarRepresentate(representanteDTO);
            return Ok(response);
        }


        [HttpGet("GetAllRepresentante")]
        public async Task<IActionResult> GetAllRepresentante()
        {
            var response = await _representante.GetAllRepresentate();

            return Ok(response);
        }

        [HttpGet("ObtenerRepresentanteById")]
        public async Task<IActionResult> ObtenerRepresentanteById(long idRepresentante)
        {
            var response = await _representante.GetRepresentateById(idRepresentante);

            return Ok(response);
        }

    }
}
