using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {

        private readonly FormInterface _formInterface;

        public FormController(FormInterface formInterface)
        {
            _formInterface = formInterface;
        }

        [HttpPost("CrearFormulario")]
        public async Task<ActionResult> CrearFormulario(FormDTO formDTO)
        {
            var response = await _formInterface.CrearFormulario(formDTO);

            return Ok(response);
        }


        [HttpPut("ActualizarFormulario")]
        public async Task<ActionResult> ActualizarFormulario(FormDTO formDTO)
        {
            var response = await _formInterface.ActualizarFormulario(formDTO);

            return Ok(response);
        }

        [HttpDelete("EliminarFormulario")]
        public async Task<ActionResult> EliminarFormulario(long id)
        {
            var response = await _formInterface.ELiminarFormulario(id);

            return Ok(response);
        }


        [HttpGet("ObtenerFormularioPorId")]
        public async Task<ActionResult> ObtenerFormularioPorId(long id)
        {
            var response = await _formInterface.ObtenerFormularioPorId(id);

            return Ok(response);
        }

        [HttpGet("ObtenerTodosLosFormularios")]
        public async Task<ActionResult> ObtenerTodosLosFormularios()
        {
            var response = await _formInterface.ObtenerFormularios();

            return Ok(response);
        }


        [HttpGet("SelectorFormularios")]
        public async Task<ActionResult> SelectorFormularios()
        {
            var response = await _formInterface.KeyValueFormuario();

            return Ok(response);
        }


    }
}
