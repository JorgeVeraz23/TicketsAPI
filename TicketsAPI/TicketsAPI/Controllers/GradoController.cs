using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradoController : ControllerBase
    {
        private readonly IGrado _grado;

        public GradoController(IGrado grado)
        {
            _grado = grado;
        }

        [HttpGet("SelectorGrados")]
        public async Task<IActionResult> SelectorGrados()
        {
            try
            {
                var grados = await _grado.SelectorGrado(); 

                return Ok(grados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los grados: {ex.Message}");
            }
        }

    }
}
