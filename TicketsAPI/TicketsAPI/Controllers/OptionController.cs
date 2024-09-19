using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OptionInterface _optionInterface;
        public OptionController(ApplicationDbContext context, OptionInterface optionInterface)
        {
            _context = context;
            _optionInterface = optionInterface;
        }


        [HttpGet("ObtenerTodasLasOpciones")]
        public async Task<ActionResult> ObtenerTodasLasOpciones()
        {
            var response = await _optionInterface.ObtenerTodasLasOpciones();

            return Ok(response);
        }

        [HttpGet("ObtenerOpcionesPorId")]
        public async Task<ActionResult> ObtenerOpcionesPorId(long id)
        {
            var response = await _optionInterface.ObtenerOpcionPorId(id);

            return Ok(response);
        }

        [HttpGet("SelectorOpciones")]
        public async Task<ActionResult> SelectorOpciones()
        {
            var response = await _optionInterface.SelectorOptionDTO();

            return Ok(response);
        }

        [HttpDelete("EliminarOpcionPorId")]
        public async Task<ActionResult> EliminarOpcionPorId(long id)
        {
            var response = await _optionInterface.EliminarOptionDTO(id);

            return Ok(response);
        }


        [HttpPost("CrearOpcion")]
        public async Task<ActionResult> CrearOpcion(OptionDTO optionDTO)
        {
            var response = await _optionInterface.CrearOpcionDTO(optionDTO);

            return Ok(response);
        }

        [HttpPut("ActualizarOpciones")]
        public async Task<ActionResult> ActualizarOpciones(OptionDTO optionDTO)
        {
            var response = await _optionInterface.ActualizarOptionDTO(optionDTO);

            return Ok(response);
        }


    }
}
