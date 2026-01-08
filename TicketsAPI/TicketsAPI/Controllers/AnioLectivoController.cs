using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnioLectivoController : ControllerBase
    {

        private readonly IAnioLectiivo _anioLectivo;

        public AnioLectivoController(IAnioLectiivo anioLectivo)
        {
            _anioLectivo = anioLectivo;
        }

        [HttpGet("SelectorAnioLectivo")]
        public async Task<IActionResult> SelectorAnioLectivo()
        {
            try
            {
                var aniosLectivos = await _anioLectivo.SelectorAnioLectivo();
                return Ok(aniosLectivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los años lectivos: {ex.Message}");
            }
        }

        [HttpGet("GetAllAnioLectivo")]
        public async Task<IActionResult> AnioLectivoActivo()
        {
            try
            {
                var anioLectivoActivo = await _anioLectivo.GetAllAnioLectivo();
                return Ok(anioLectivoActivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el año lectivo activo: {ex.Message}");
            }
        }
    }
}
