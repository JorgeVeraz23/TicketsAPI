using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParaleloController : ControllerBase
    {
        private readonly IParalelo _paralelo;
        public ParaleloController(IParalelo paralelo)
        {
            _paralelo = paralelo;
        }


        [HttpPost("CrearParalelo")]
        public async Task<IActionResult> CrearParalelo(ParaleloDTO paraleloDTO)
        {
            var response = await _paralelo.CrearParalelo(paraleloDTO);

            return Ok(response);
        }

        [HttpPut("ActualizarParalelo")]
        public async Task<IActionResult> ActualizarParalelo(ParaleloDTO paraleloDTO)
        {
            var responnse = await _paralelo.EditarParalelo(paraleloDTO);

            return Ok(responnse);
        }

        [HttpDelete("EliminarParalelo")]
        public async Task<IActionResult> EliminarParalelo(long id)
        {
            var response = await _paralelo.EliminarParalelo(id);

            return Ok(response);

        }


        [HttpGet("GetAllParalelos")]
        public async Task<IActionResult> GetAllParalelos()
        {
            var response = await _paralelo.GetParaleloList();

            return Ok(response);

        }

        [HttpGet("GetParalelo")]
        public async Task<IActionResult> GetParalelo(long id)
        {
            var response = await _paralelo.GetParalelo(id);

            return Ok(response);
        }
    }
}
